using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widgets.NopStationEmployees.Areas.Admin.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widget.NopStationEmployees.Components;

public class NopStationEmployeesComponent : NopViewComponent
{
    private readonly INopStationEmployeeModelFactory _nopStationEmployeeModelFactory;

    public NopStationEmployeesComponent(INopStationEmployeeModelFactory nopStationEmployeeModelFactory)
    {
        _nopStationEmployeeModelFactory = nopStationEmployeeModelFactory;
    }

    public IViewComponentResult Invoke(string widgetZone, object additionalData)
    {
        return Content("Hello world");
    }
}
