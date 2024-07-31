using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Orders;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Orders;
using NopStation.Plugin.Misc.Core.Controllers;
using NopStation.Plugin.Payments.StripeHostedCheckout.Services;
using Stripe;

namespace NopStation.Plugin.Payments.StripeHostedCheckout.Controllers
{
    public class StripeHostedCheckoutController : NopStationPublicController
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IStripeServices _stripeServices;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public StripeHostedCheckoutController(ISettingService settingService,
            IOrderService orderService,
            IStripeServices stripeServices,
            ILogger logger,
            IOrderProcessingService orderProcessingService)
        {
            _settingService = settingService;
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _stripeServices = stripeServices;
            _logger = logger;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Redirect(Guid orderGuid)
        {
            try
            {
                var order = await _orderService.GetOrderByGuidAsync(orderGuid);
                if (order == null || order.Deleted)
                    return RedirectToRoute("Homepage");

                var stripePaymentSettings = await _settingService.LoadSettingAsync<StripeHostedCheckoutSettings>(order.StoreId);
                var service = new PaymentIntentService(new StripeClient(apiKey: stripePaymentSettings.ApiKey));

                var paymentIntent = await service.GetAsync(order.AuthorizationTransactionId);
                if (paymentIntent.AmountReceived >= _stripeServices.ConvertCurrencyToLong(order.OrderTotal, order.CurrencyRate) && _orderProcessingService.CanMarkOrderAsPaid(order))
                {
                    await _orderProcessingService.MarkOrderAsPaidAsync(order);
                    return RedirectToAction("completed", "checkout");
                }
                if (paymentIntent.AmountCapturable >= _stripeServices.ConvertCurrencyToLong(order.OrderTotal, order.CurrencyRate) && _orderProcessingService.CanMarkOrderAsAuthorized(order))
                {
                    await _orderProcessingService.MarkAsAuthorizedAsync(order);
                    return RedirectToAction("completed", "checkout");
                }

                await _orderService.InsertOrderNoteAsync(new OrderNote()
                {
                    CreatedOnUtc = DateTime.UtcNow,
                    OrderId = order.Id,
                    DisplayToCustomer = false,
                    Note = "Stripe payment info:\n" + paymentIntent.RawJObject.ToString(Newtonsoft.Json.Formatting.None)
                });
                return RedirectToRoute("OrderDetails", new { orderId = order.Id });
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message);
            }
            return RedirectToRoute("Homepage");
        }

        #endregion
    }
}