using System.Threading.Tasks;
using Nop.Core.Domain.Orders;

namespace NopStation.Plugin.Payments.StripeHostedCheckout.Services
{
    public interface IStripeServices
    {
        public Task<Stripe.Refund> RefundAsync(Order order, decimal refundAmount);

        public Task<Stripe.PaymentIntent> VoidAsync(Order order);

        public long ConvertCurrencyToLong(decimal total, decimal currencyRate);

        public decimal ConvertCurrencyFromLong(decimal total, decimal currencyRate);

        public bool IsConfigured(StripeHostedCheckoutSettings settings);
    }
}