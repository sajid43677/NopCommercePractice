using System;
using System.Threading.Tasks;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Stripe;

namespace NopStation.Plugin.Payments.StripeHostedCheckout.Services
{
    public class StripeServices : IStripeServices
    {
        #region Fields

        private static int RATE = 100;
        private readonly ISettingService _settingService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ICurrencyService _currencyService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public StripeServices(ISettingService settingService,
            IOrderProcessingService orderProcessingService,
            ILogger logger,
            ICurrencyService currencyService)
        {
            _settingService = settingService;
            _orderProcessingService = orderProcessingService;
            _currencyService = currencyService;
            _logger = logger;
        }

        #endregion

        #region Methods

        private async Task<string> GetApiKeyAsync(int storeId)
        {
            var setting = await _settingService.LoadSettingAsync<StripeHostedCheckoutSettings>(storeId);
            return setting.ApiKey;
        }

        public long ConvertCurrencyToLong(decimal total, decimal currencyRate)
        {
            return Convert.ToInt64(_currencyService.ConvertCurrency(total, currencyRate) * RATE);
        }

        public decimal ConvertCurrencyFromLong(decimal total, decimal currencyRate)
        {
            return _currencyService.ConvertCurrency(total / RATE, 1 / currencyRate);
        }

        public async Task<Refund> RefundAsync(Nop.Core.Domain.Orders.Order order, decimal refundAmount)
        {
            var refundPossible = await _orderProcessingService.CanRefundAsync(order);
            if (refundPossible)
            {
                try
                {
                    StripeConfiguration.ApiKey = await GetApiKeyAsync(order.StoreId);
                    var options = new RefundCreateOptions
                    {
                        PaymentIntent = order.AuthorizationTransactionId,
                        Amount = ConvertCurrencyToLong(refundAmount, order.CurrencyRate),
                    };
                    var service = new RefundService();
                    return await service.CreateAsync(options);
                }
                catch (Exception ex)
                {
                    await _logger.ErrorAsync(ex.Message);
                }
            }
            return new Refund();
        }

        public async Task<PaymentIntent> VoidAsync(Nop.Core.Domain.Orders.Order order)
        {
            try
            {
                StripeConfiguration.ApiKey = await GetApiKeyAsync(order.StoreId);
                var paymentIntentService = new PaymentIntentService();
                var voidPossible = await _orderProcessingService.CanVoidAsync(order);
                if (voidPossible)
                {
                    return await paymentIntentService.CancelAsync(order.AuthorizationTransactionId);
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message);
            }
            return new PaymentIntent();
        }

        public bool IsConfigured(StripeHostedCheckoutSettings settings)
        {
            return !string.IsNullOrEmpty(settings?.ApiKey);
        }
    }

    #endregion
}