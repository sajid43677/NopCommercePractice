using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.Omnisend.Infrastructure;
using Nop.Plugin.Widget.NopStationEmployees.Services;
using Nop.Plugin.Widgets.NopStationEmployees.Areas.Admin.Factories;
using Nop.Plugin.Widgets.NopStationEmployees.Factories;

namespace Nop.Plugin.Widget.NopStationEmployees.Infrastructure;
public class NopStartup : INopStartup
{
    public int Order => 3000;

    public void Configure(IApplicationBuilder application)
    {
    }

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RazorViewEngineOptions>(options =>
        {
            options.ViewLocationExpanders.Add(new ViewLocationExpanderAdmin());
        });
        services.AddScoped<INopStationEmployeeService, NopStationEmployeeService>();
        services.AddScoped<INopStationEmployeeModelFactory, NopStationEmployeeModelFactory>();
        services.AddScoped<INopStationEmployeePublicModelFactory, NopStationEmployeePublicModelFactory>();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }
}
