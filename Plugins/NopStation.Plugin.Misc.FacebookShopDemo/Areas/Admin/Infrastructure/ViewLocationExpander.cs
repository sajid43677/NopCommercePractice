using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;
using Nop.Web.Framework;

namespace NopStation.Plugin.Misc.FacebookShopDemo.Areas.Admin.Infrastructure;
public class ViewLocationExpander : IViewLocationExpander
{
    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        if (context.AreaName == AreaNames.ADMIN)
        {
            viewLocations = new[] { $"/Plugins/NopStation.Misc.FacebookShopDemo/Areas/{context.AreaName}/Views/{context.ControllerName}/{context.ViewName}.cshtml" }
                .Concat(viewLocations);
        }

        return viewLocations;
    }

    public void PopulateValues(ViewLocationExpanderContext context)
    {
    }
}
