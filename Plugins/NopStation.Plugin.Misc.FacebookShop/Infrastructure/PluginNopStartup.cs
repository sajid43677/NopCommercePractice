using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Data;
using NopStation.Plugin.Misc.Core.Infrastructure;
using NopStation.Plugin.Misc.FacebookShop.Domains;
using NopStation.Plugin.Misc.FacebookShop.Factories;
using NopStation.Plugin.Misc.FacebookShop.Services;

namespace NopStation.Plugin.Misc.FacebookShop.Infrastructure
{
    public class PluginNopStartup : INopStartup
    {
        public int Order => 11;

        public void Configure(IApplicationBuilder application)
        {
        }


        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddNopStationServices("NopStation.Plugin.Misc.FacebookShop");

            services.AddScoped<IFacebookShopService, FacebookShopService>();
            services.AddScoped<IFacebookShopModelFactory, FacebookShopModelFactory>();
            services.AddScoped<IFacebookShopIOManager, FacebookShopIOManager>();
        }
    }
}