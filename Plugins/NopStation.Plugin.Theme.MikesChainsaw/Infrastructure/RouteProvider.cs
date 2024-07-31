using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Domain.Localization;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.NopStation.Theme.MikesChainsaw.Infrastructure;

public class RouteProvider : IRouteProvider
{
    public int Priority => 10;

    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        var pattern = string.Empty;
        if (DataSettingsManager.IsDatabaseInstalled())
        {
            var localizationSettings = endpointRouteBuilder.ServiceProvider.GetRequiredService<LocalizationSettings>();
            if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                var langservice = endpointRouteBuilder.ServiceProvider.GetRequiredService<ILanguageService>();
                var languages = langservice.GetAllLanguages().ToList();
                pattern = "{language:lang=" + languages.FirstOrDefault().UniqueSeoCode + "}/";
            }
        }
        endpointRouteBuilder.MapControllerRoute("NopStation.MikesChainsaw.BookAService", $"{pattern}book-a-service/",
           new { controller = "MikesChainsaw", action = "BookService" });
    }
}
