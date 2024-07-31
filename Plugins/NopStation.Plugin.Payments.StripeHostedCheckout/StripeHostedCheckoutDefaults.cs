namespace NopStation.Plugin.Payments.StripeHostedCheckout
{
    public class StripeHostedCheckoutDefaults
    {
        public static string CapturePayment => "payment";
        public static string PaymentIntentDescription => "Payment for product";
        public static string CaptureMethodManual => "manual";
        public static string CaptureMethodAutomatic => "automatic";
    }
}
