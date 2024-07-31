using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace NopStation.Plugin.Payments.StripeHostedCheckout.Infrastructure
{
    public class RouteProvider : BaseRouteProvider, IRouteProvider
    {
        public int Priority => -1;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var pattern = GetLanguageRoutePattern();
            endpointRouteBuilder.MapControllerRoute("StripeHostedCheckoutRedirect", pattern + "/stripehostedcheckout/redirect/{orderGuid}",
                new { controller = "StripeHostedCheckout", action = "Redirect" });
        }
    }
}
