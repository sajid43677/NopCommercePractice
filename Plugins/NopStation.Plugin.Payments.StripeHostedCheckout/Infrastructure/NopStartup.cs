using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using NopStation.Plugin.Payments.StripeHostedCheckout.Services;

namespace NopStation.Plugin.Payments.StripeHostedCheckout.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IStripeServices, StripeServices>();
        }

        public void Configure(IApplicationBuilder application)
        {
        }
        public int Order => 3000;
    }
}