using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widgets.NopStationEmployees.Areas.Admin.Factories;
using Nop.Plugin.Widgets.NopStationEmployees.Factories;
using Nop.Plugin.Widgets.NopStationEmployees.Models;
using Nop.Services.Plugins;
using Nop.Web.Framework.Components;
using StackExchange.Redis;

namespace Nop.Plugin.Widget.NopStationEmployees.Components;

public class NopStationEmployeesComponent : NopViewComponent
{
    private readonly INopStationEmployeePublicModelFactory _nopStationEmployeePublicModelFactory;

    public NopStationEmployeesComponent(INopStationEmployeePublicModelFactory nopStationEmployeePublicModelFactory)
    {
        _nopStationEmployeePublicModelFactory = nopStationEmployeePublicModelFactory;
    }

    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        var model = await _nopStationEmployeePublicModelFactory.PreparePublicModel();
        return View(model);
    }
}
