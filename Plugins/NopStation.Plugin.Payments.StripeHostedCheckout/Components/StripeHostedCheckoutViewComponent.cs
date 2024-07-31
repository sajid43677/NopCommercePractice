using Microsoft.AspNetCore.Mvc;
using NopStation.Plugin.Misc.Core.Components;

namespace NopStation.Plugin.Payments.StripeHostedCheckout.Components
{
    public class StripeHostedCheckoutViewComponent : NopStationViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/NopStation.Plugin.Payments.StripeHostedCheckout/Views/PaymentInfo.cshtml");
        }
    }
}