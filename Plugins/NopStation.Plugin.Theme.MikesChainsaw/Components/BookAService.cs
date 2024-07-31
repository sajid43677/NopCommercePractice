using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.NopStation.Theme.MikesChainsaw.Components;

public class BookAServiceViewComponent : NopViewComponent
{
    public IViewComponentResult Invoke(string widgetZone)
    {
        return View();
    }
}
