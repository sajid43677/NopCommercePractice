using Microsoft.AspNetCore.Mvc;
using NopStation.Plugin.Misc.Core.Services;
using Nop.Plugin.NopStation.Theme.MikesChainsaw.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.NopStation.Theme.MikesChainsaw.Components;

public class MikesChainsawViewComponent : NopViewComponent
{
    private readonly MikesChainsawSettings _berrySettings;

    public MikesChainsawViewComponent(MikesChainsawSettings berrySettings)
    {
        _berrySettings = berrySettings;
    }

    public IViewComponentResult Invoke(string widgetZone)
    {
        var model = new PublicModel()
        {
            CustomCss = _berrySettings.CustomCss,
            EnableImageLazyLoad = _berrySettings.EnableImageLazyLoad
        };

        return View(model);
    }
}
