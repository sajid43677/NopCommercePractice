using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Nop.Plugin.NopStation.Theme.MikesChainsaw.Infrastructure;

public class ViewLocationExpander : IViewLocationExpander
{
    public void PopulateValues(ViewLocationExpanderContext context)
    {
    }

    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        if (context.AreaName == "Admin")
        {
            viewLocations = new[] {
                $"/Plugins/NopStation.Theme.MikesChainsaw/Areas/Admin/Views/Shared/{{0}}.cshtml",
                $"/Plugins/NopStation.Theme.MikesChainsaw/Areas/Admin/Views/{{1}}/{{0}}.cshtml"
            }.Concat(viewLocations);
        }
        else
        {
            viewLocations = new[] {
                $"/Plugins/NopStation.Theme.MikesChainsaw/Views/Shared/{{0}}.cshtml",
                $"/Plugins/NopStation.Theme.MikesChainsaw/Views/{{1}}/{{0}}.cshtml"
            }.Concat(viewLocations);
        }
        return viewLocations;
    }
}
