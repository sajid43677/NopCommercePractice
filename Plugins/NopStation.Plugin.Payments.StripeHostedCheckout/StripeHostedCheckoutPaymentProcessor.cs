using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Framework.Menu;
using Nop.Web.Framework.Mvc.Routing;
using NopStation.Plugin.Misc.Core;
using NopStation.Plugin.Misc.Core.Services;
using NopStation.Plugin.Payments.StripeHostedCheckout.Components;
using NopStation.Plugin.Payments.StripeHostedCheckout.Services;
using Stripe.Checkout;

namespace NopStation.Plugin.Payments.StripeHostedCheckout
{
    public class StripeHostedCheckoutPaymentProcessor : BasePlugin, IPaymentMethod, INopStationPlugin, IAdminMenuPlugin
    {
        #region Fields

        private const int RATE = 100;
        private readonly IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeParser;
        private readonly ICustomerService _customerService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IProductService _productService;
        private readonly ISettingService _settingService;
        private readonly ITaxService _taxService;
        private readonly IWebHelper _webHelper;
        private readonly StripeHostedCheckoutSettings _stripeHostedCheckoutSettings;
        private readonly IStripeServices _stripeServices;
        private readonly IWorkContext _workContext;
        private readonly INopStationCoreService _nopStationCoreService;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly ILogger _logger;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public StripeHostedCheckoutPaymentProcessor(IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeParser,
            ICustomerService customerService,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IOrderService orderService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IProductService productService,
            ISettingService settingService,
            ITaxService taxService,
            IWebHelper webHelper,
            IStripeServices stripeServices,
            IWorkContext workContext,
            INopStationCoreService nopStationCoreService,
            IActionContextAccessor actionContextAccessor,
            IUrlHelperFactory urlHelperFactory,
            ILogger logger,
            IPermissionService permissionService,
            StripeHostedCheckoutSettings stripeHostedCheckoutSettings)
        {
            _checkoutAttributeParser = checkoutAttributeParser;
            _customerService = customerService;
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _orderService = orderService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _productService = productService;
            _settingService = settingService;
            _taxService = taxService;
            _webHelper = webHelper;
            _stripeServices = stripeServices;
            _workContext = workContext;
            _nopStationCoreService = nopStationCoreService;
            _actionContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
            _stripeHostedCheckoutSettings = stripeHostedCheckoutSettings;
            _logger = logger;
            _permissionService = permissionService;
        }

        #endregion

        #region Utilities

        private async Task<Stripe.Product> CreateStripeProductAsync(OrderItem item = null, string name = null, string description = null)
        {
            if (item != null)
            {
                var itemProduct = await _productService.GetProductByIdAsync(item.ProductId);
                if (name == null)
                    name = itemProduct.Name;
                if (description == null)
                    description = itemProduct.ShortDescription;
            }
            var productOptions = new Stripe.ProductCreateOptions
            {
                Name = name,
                Description = description,
                Active = true,
            };

            var productService = new Stripe.ProductService();
            var product = await productService.CreateAsync(productOptions);
            return product;
        }

        private async Task<(SessionLineItemOptions, decimal)> GetAdditionalFeeAsync(Order order, decimal additionalFee, bool isPercentage)
        {
            decimal additionalCost;
            if (isPercentage)
                additionalCost = order.OrderTotal * additionalFee / RATE;
            else
                additionalCost = additionalFee;

            var productOptions = new Stripe.ProductCreateOptions
            {
                Name = "Payment method fee",
                Description = "This is the cost of using stripe payment method",
                Active = true,
            };
            var productService = new Stripe.ProductService();
            var product = await productService.CreateAsync(productOptions);

            var priceOptions = new Stripe.PriceCreateOptions
            {
                UnitAmount = _stripeServices.ConvertCurrencyToLong(additionalCost, order.CurrencyRate),
                Currency = order.CustomerCurrencyCode,
                Product = product.Id,
            };
            var priceService = new Stripe.PriceService();
            var price = priceService.Create(priceOptions);
            var items = new SessionLineItemOptions
            {
                Quantity = 1,
                Price = price.Id,
            };
            return (items, additionalCost);
        }

        private async Task<Stripe.Price> CreateStripePriceAsync(OrderItem item, string productId, Order order, decimal attributePrice = 0)
        {
            if (item != null)
                attributePrice = item.UnitPriceExclTax;

            var priceOptions = new Stripe.PriceCreateOptions
            {
                UnitAmount = _stripeServices.ConvertCurrencyToLong(attributePrice, order.CurrencyRate),
                Currency = order.CustomerCurrencyCode,
                Product = productId,
            };
            var priceService = new Stripe.PriceService();
            var price = await priceService.CreateAsync(priceOptions);
            return price;
        }

        private SessionLineItemOptions CreateSessionItem(string priceId, int quantity)
        {
            return new SessionLineItemOptions
            {
                Quantity = quantity,
                Price = priceId,
            };
        }

        private async Task<List<SessionLineItemOptions>> GetLineItemsAsync(Order order)
        {
            var orderItems = await _orderService.GetOrderItemsAsync(order.Id);
            var lineItems = new List<SessionLineItemOptions>();
            foreach (var item in orderItems)
            {
                var product = await CreateStripeProductAsync(item);
                var price = await CreateStripePriceAsync(item, product.Id, order);
                var sessionItem = CreateSessionItem(price.Id, item.Quantity);
                lineItems.Add(sessionItem);
            }
            return lineItems;
        }

        private async Task<List<SessionLineItemOptions>> GetCheckoutAttribute(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var item = new List<SessionLineItemOptions>();
            var checkoutAttributeValues = _checkoutAttributeParser.ParseAttributeValues(postProcessPaymentRequest.Order.CheckoutAttributesXml);
            var customer = await _customerService.GetCustomerByIdAsync(postProcessPaymentRequest.Order.CustomerId);
            var order = postProcessPaymentRequest.Order;
            await foreach (var (attribute, values) in checkoutAttributeValues)
            {
                await foreach (var attributeValue in values)
                {
                    var (attributePrice, _) = await _taxService.GetCheckoutAttributePriceAsync(attribute, attributeValue, false, customer);

                    //add attribute parameters
                    if (attribute == null || attributePrice <= decimal.Zero)
                        continue;
                    var product = await CreateStripeProductAsync(null, attribute.Name, $"{attribute.Name} cost");
                    var price = await CreateStripePriceAsync(null, product.Id, order, attributePrice);
                    var sessionItem = CreateSessionItem(price.Id, 1);
                    item.Add(sessionItem);
                }
            }
            return item;
        }

        private async Task<SessionLineItemOptions> CreateShippingItem(Order order)
        {
            var shippingRate = order.OrderShippingExclTax;
            var stripeProduct = await CreateStripeProductAsync(null, "Shipping rate", "This is shipping rate ");
            var stripePrice = await CreateStripePriceAsync(null, stripeProduct.Id, order, shippingRate);
            var sessionItem = CreateSessionItem(stripePrice.Id, 1);
            return sessionItem;
        }

        private async Task<SessionLineItemOptions> CreateTaxItem(Order order)
        {
            var stripeProduct = await CreateStripeProductAsync(null, "Tax", "This is tax rate");
            var stripePrice = await CreateStripePriceAsync(null, stripeProduct.Id, order, order.OrderTax);
            var sessionItem = CreateSessionItem(stripePrice.Id, 1);
            return sessionItem;
        }

        private async Task<Stripe.Coupon> CreateDiscountItem(decimal discountAmount, Order order)
        {
            var options = new Stripe.CouponCreateOptions
            {
                AmountOff = _stripeServices.ConvertCurrencyToLong(discountAmount, order.CurrencyRate),
                Duration = "once",
                Currency = order.CustomerCurrencyCode,
            };
            var service = new Stripe.CouponService();
            var coupon = await service.CreateAsync(options);
            return coupon;
        }

        #endregion

        #region Methods

        public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            return Task.FromResult(new ProcessPaymentResult());
        }

        public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            var order = postProcessPaymentRequest.Order;
            var setting = _settingService.LoadSetting<StripeHostedCheckoutSettings>(order.StoreId);
            try
            {
                Stripe.StripeConfiguration.ApiKey = setting.ApiKey;
                var lineItems = await GetLineItemsAsync(order);
                var subtotal = order.OrderSubtotalExclTax;
                if (setting.AdditionalFee > 0)
                {
                    var (item, value) = await GetAdditionalFeeAsync(order, setting.AdditionalFee, setting.AdditionalFeePercentage);
                    lineItems.Add(item);
                    subtotal += value;
                }
                var checkoutAttributeItems = await GetCheckoutAttribute(postProcessPaymentRequest);
                if (checkoutAttributeItems.Count > 0)
                    lineItems.AddRange(checkoutAttributeItems);

                var shippingRate = postProcessPaymentRequest.Order.OrderShippingExclTax;
                if (shippingRate > decimal.Zero)
                {
                    var shippingItem = await CreateShippingItem(order);
                    lineItems.Add(shippingItem);
                    subtotal += shippingRate;
                }
                // Add tax as a separate line item.
                var taxAmount = postProcessPaymentRequest.Order.OrderTax;
                if (taxAmount > decimal.Zero)
                {
                    var taxItem = await CreateTaxItem(order);
                    lineItems.Add(taxItem);
                    subtotal += taxAmount;
                }
                var discountOptions = new List<SessionDiscountOptions>();
                // Add discount in line item
                var discountAmount = subtotal - order.OrderTotal;
                if (discountAmount > decimal.Zero)
                {
                    var discountItem = await CreateDiscountItem(discountAmount, order);
                    var disocunt = new SessionDiscountOptions { Coupon = discountItem.Id };
                    discountOptions.Add(disocunt);
                }
                var options = new SessionCreateOptions
                {
                    LineItems = lineItems,
                    Mode = StripeHostedCheckoutDefaults.CapturePayment,
                    CustomerEmail = currentCustomer.Email,
                    ClientReferenceId = currentCustomer?.CustomerGuid.ToString(),
                    CancelUrl = new Uri(_webHelper.GetStoreLocation()).Concat(urlHelper.RouteUrl("StripeHostedCheckoutRedirect", new { orderGuid = order.OrderGuid })).AbsoluteUri,
                    SuccessUrl = new Uri(_webHelper.GetStoreLocation()).Concat(urlHelper.RouteUrl("StripeHostedCheckoutRedirect", new { orderGuid = order.OrderGuid })).AbsoluteUri,
                    PaymentIntentData = new SessionPaymentIntentDataOptions
                    {
                        CaptureMethod = (setting.PaymentType == Models.PaymentType.Authorize) ? StripeHostedCheckoutDefaults.CaptureMethodManual : StripeHostedCheckoutDefaults.CaptureMethodAutomatic,
                        Description = StripeHostedCheckoutDefaults.PaymentIntentDescription,
                    },
                };
                if (discountOptions.Count > 0)
                {
                    options.Discounts = discountOptions;
                }
                var service = new SessionService();
                var session = service.Create(options);

                order.AuthorizationTransactionId = session.PaymentIntentId;
                await _orderService.UpdateOrderAsync(order);

                _httpContextAccessor.HttpContext.Response.Redirect(session.Url);
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message);
            }
        }

        public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            var notConfigured = !_stripeServices.IsConfigured(_stripeHostedCheckoutSettings);
            return Task.FromResult(notConfigured);
        }

        public async Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
            return await _orderTotalCalculationService.CalculatePaymentAdditionalFeeAsync(cart,
                _stripeHostedCheckoutSettings.AdditionalFee, _stripeHostedCheckoutSettings.AdditionalFeePercentage);
        }

        public async Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            try
            {
                var captureOptions = new Stripe.PaymentIntentCaptureOptions()
                {
                    AmountToCapture = _stripeServices.ConvertCurrencyToLong(capturePaymentRequest.Order.OrderTotal, capturePaymentRequest.Order.CurrencyRate),
                };

                var chargeService = new Stripe.PaymentIntentService(new Stripe.StripeClient(_stripeHostedCheckoutSettings.ApiKey));

                var stripeCharge = await chargeService.CaptureAsync(capturePaymentRequest.Order.AuthorizationTransactionId, captureOptions);
                if (stripeCharge != null && stripeCharge.Status == "succeeded")
                {
                    result.NewPaymentStatus = PaymentStatus.Paid;
                    result.CaptureTransactionId = stripeCharge.Id;
                    result.CaptureTransactionResult = stripeCharge.Status;
                }
                else
                    result.AddError(_localizationService.GetResourceAsync("NopStation.Stripe.CaptureFailed").Result);
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);
                result.AddError("Capture request failed");
            }
            return result;
        }

        public async Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            try
            {
                var refundAmount = refundPaymentRequest.AmountToRefund != refundPaymentRequest.Order.OrderTotal ? refundPaymentRequest.AmountToRefund 
                    : refundPaymentRequest.Order.OrderSubtotalExclTax;

                var refund = await _stripeServices.RefundAsync(refundPaymentRequest.Order, refundAmount);
                if (refund.Status == "succeeded")
                {
                    result.NewPaymentStatus = refundPaymentRequest.IsPartialRefund ? PaymentStatus.PartiallyRefunded : PaymentStatus.Refunded;
                    await _orderService.InsertOrderNoteAsync(new()
                    {
                        CreatedOnUtc = DateTime.UtcNow,
                        DisplayToCustomer = false,
                        OrderId = refundPaymentRequest.Order.Id,
                        Note = "Stripe refund info:\n" + refund.RawJObject.ToString(Newtonsoft.Json.Formatting.None)
                    });
                }
                else
                {
                    result.AddError(refund.FailureReason);
                    if (refund.Status != null)
                        await _logger.ErrorAsync(refund.RawJObject.ToString(Newtonsoft.Json.Formatting.None));
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.ToString());
            }
            return result;
        }

        public async Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            try
            {
                var voidResult = await _stripeServices.VoidAsync(voidPaymentRequest.Order);
                if (voidResult.Status == "canceled")
                {
                    result.NewPaymentStatus = PaymentStatus.Voided;
                    await _orderService.InsertOrderNoteAsync(new()
                    {
                        CreatedOnUtc = DateTime.UtcNow,
                        DisplayToCustomer = false,
                        OrderId = voidPaymentRequest.Order.Id,
                        Note = "Stripe void info:\n" + voidResult.RawJObject.ToString(Newtonsoft.Json.Formatting.None)
                    });
                }
                else
                {
                    result.AddError("void request failed");
                    if (voidResult.Status != null)
                        await _logger.ErrorAsync(voidResult.RawJObject.ToString(Newtonsoft.Json.Formatting.None));
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message);
            }
            return result;
        }

        public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            return Task.FromResult(new ProcessPaymentResult { Errors = new[] { "Recurring payment not supported" } });
        }

        public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return Task.FromResult(new CancelRecurringPaymentResult { Errors = new[] { "Recurring payment not supported" } });
        }

        public Task<bool> CanRePostProcessPaymentAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if ((DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds < 5)
                return Task.FromResult(false);

            return Task.FromResult(true);
        }

        public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        {
            return Task.FromResult<IList<string>>(new List<string>());
        }

        public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
        {
            return Task.FromResult(new ProcessPaymentRequest());
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PaymentStripeHostedCheckout/Configure";
        }

        public override async Task InstallAsync()
        {
            var settings = new StripeHostedCheckoutSettings
            {
                PaymentType = Models.PaymentType.Capture,
                AdditionalFeePercentage = false,
            };
            await _settingService.SaveSettingAsync(settings);
            await this.InstallPluginAsync(new StripeHostedCheckoutPermissionProvider());
            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await _settingService.DeleteSettingAsync<StripeHostedCheckoutSettings>();
            await this.UninstallPluginAsync(new StripeHostedCheckoutPermissionProvider());
            await base.UninstallAsync();
        }

        public async Task<string> GetPaymentMethodDescriptionAsync()
        {
            return await _localizationService.GetResourceAsync("NopStation.StripeHostedCheckout.PaymentMethodDescription");
        }

        public Type GetPublicViewComponent()
        {
            return typeof(StripeHostedCheckoutViewComponent);
        }

        public List<KeyValuePair<string, string>> PluginResouces()
        {
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Admin.NopStation.StripeHostedCheckout.Menu.StripeHostedCheckout", "Stripe hosted checkout"),
                new KeyValuePair<string, string>("Admin.NopStation.StripeHostedCheckout.Menu.Configuration", "Configuration"),
                new KeyValuePair<string, string>("Admin.NopStation.StripeHostedCheckout.Configuration", "Stripe hosted checkout configuration"),

                new KeyValuePair<string, string>("Admin.NopStation.StripeHostedCheckout.Fields.AdditionalFee", "Additional fee"),
                new KeyValuePair<string, string>("Admin.NopStation.StripeHostedCheckout.Fields.AdditionalFee.Hint", "Enter additional fee to charge your customers."),
                new KeyValuePair<string, string>("Admin.NopStation.StripeHostedCheckout.Fields.AdditionalFeePercentage", "Additional fee. Use percentage"),
                new KeyValuePair<string, string>("Admin.NopStation.StripeHostedCheckout.Fields.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used."),
                new KeyValuePair<string, string>("Admin.NopStation.StripeHostedCheckout.Fields.ApiKey", "Secret key"),
                new KeyValuePair<string, string>("Admin.NopStation.StripeHostedCheckout.Fields.ApiKey.Hint", "Specify secret key"),
                new KeyValuePair<string, string>("Admin.NopStation.StripeHostedCheckout.Fields.PaymentType", "Payment type"),
                new KeyValuePair<string, string>("Admin.NopStation.StripeHostedCheckout.Fields.PaymentType.Hint", "Select payment type authorization or capture"),

                new KeyValuePair<string, string>("NopStation.StripeHostedCheckout.Fields.RedirectionTip", "You will be redirected to stripe site to complete the payment."),
                new KeyValuePair<string, string>("NopStation.StripeHostedCheckout.PaymentMethodDescription", "Stripe payment method is a worldwide acceptable payment method"),
            };
            return list;
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            var menuItem = new SiteMapNode()
            {
                Title = await _localizationService.GetResourceAsync("Admin.NopStation.StripeHostedCheckout.Menu.StripeHostedCheckout"),
                Visible = true,
                IconClass = "far fa-dot-circle",
            };

            if (await _permissionService.AuthorizeAsync(StripeHostedCheckoutPermissionProvider.ManageConfiguration))
            {
                var configItem = new SiteMapNode()
                {
                    Title = await _localizationService.GetResourceAsync("Admin.NopStation.StripeHostedCheckout.Menu.Configuration"),
                    Url = $"{_webHelper.GetStoreLocation()}Admin/PaymentStripeHostedCheckout/Configure",
                    Visible = true,
                    IconClass = "far fa-circle",
                    SystemName = "StripeHostedCheckout.Configuration"
                };
                menuItem.ChildNodes.Add(configItem);
            }

            if (await _permissionService.AuthorizeAsync(CorePermissionProvider.ShowDocumentations))
            {
                var documentation = new SiteMapNode()
                {
                    Title = await _localizationService.GetResourceAsync("Admin.NopStation.Common.Menu.Documentation"),
                    Url = "https://www.nop-station.com/stripe-hosted-checkout-documentation?utm_source=admin-panel&utm_medium=products&utm_campaign=stripe-hosted-checkout",
                    Visible = true,
                    IconClass = "far fa-circle",
                    OpenUrlInNewTab = true
                };
                menuItem.ChildNodes.Add(documentation);
            }
            await _nopStationCoreService.ManageSiteMapAsync(rootNode, menuItem, NopStationMenuType.Plugin);
        }

        #endregion

        #region Properties

        public bool SupportCapture => true;

        public bool SupportPartiallyRefund => true;

        public bool SupportRefund => true;

        public bool SupportVoid => true;

        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        public bool SkipPaymentInfo => false;

        #endregion
    }
}