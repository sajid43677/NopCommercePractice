using Nop.Core.Configuration;
using NopStation.Plugin.Payments.StripeHostedCheckout.Models;

namespace NopStation.Plugin.Payments.StripeHostedCheckout
{
    public class StripeHostedCheckoutSettings : ISettings
    {
        public string ApiKey { get; set; }

        public decimal AdditionalFee { get; set; }

        public bool AdditionalFeePercentage { get; set; }

        public PaymentType PaymentType { get; set; }
    }
}