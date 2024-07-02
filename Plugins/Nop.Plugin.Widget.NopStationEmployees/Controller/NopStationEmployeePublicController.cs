using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widgets.NopStationEmployees.Areas.Admin.Models;
using Nop.Plugin.Widgets.NopStationEmployees.Factories;
using Nop.Plugin.Widgets.NopStationEmployees.Models;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.NopStationEmployees.Controller;
public class NopStationEmployeePublicController : BasePluginController
{
    private readonly INopStationEmployeePublicModelFactory _nopStationEmployeePublicModelFactory;

    public NopStationEmployeePublicController(INopStationEmployeePublicModelFactory nopStationEmployeePublicModelFactory)
    {
        _nopStationEmployeePublicModelFactory = nopStationEmployeePublicModelFactory;
    }

    /*public async Task<IActionResult> List()
    {
        var model = await _nopStationEmployeePublicModelFactory.PrepareNopStationEmployeeSearchPublicModelAsync(new NopStationEmployeeSearchPublicModel());
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> List(NopStationEmployeeSearchPublicModel searchModel)
    {
        var model = await _nopStationEmployeePublicModelFactory.PrepareNopStationEmployeeListPublicModelAsync(searchModel);
        return Json(model);
    }*/
}
