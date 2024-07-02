using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widget.NopStationEmployees.Domain;
using Nop.Plugin.Widget.NopStationEmployees.Services;
using Nop.Plugin.Widgets.NopStationEmployees.Areas.Admin.Factories;
using Nop.Plugin.Widgets.NopStationEmployees.Areas.Admin.Models;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.NopStationEmployees.Areas.Admin.Controller;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
public class NopStationEmployeeController : BasePluginController
{
    private readonly INopStationEmployeeService _nopStationEmployeeService;
    private readonly INopStationEmployeeModelFactory _nopStationEmployeeModelFactory;

    public NopStationEmployeeController(INopStationEmployeeService nopStationEmployeeService, INopStationEmployeeModelFactory nopStationEmployeeModelFactory)
    {
        _nopStationEmployeeService = nopStationEmployeeService;
        _nopStationEmployeeModelFactory = nopStationEmployeeModelFactory;
    }

    public async Task<IActionResult> List()
    {
        var model = await _nopStationEmployeeModelFactory.PrepareNopStationEmployeeSearchModelAsync(new NopStationEmployeeSearchModel());
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> List(NopStationEmployeeSearchModel searchModel)
    {
        var model = await _nopStationEmployeeModelFactory.PrepareNopStationEmployeeListModelAsync(searchModel);
        return Json(model);
    }

    public async Task<IActionResult> Create()
    {
        var model = await _nopStationEmployeeModelFactory.PrepareNopStationEmployeeModelAsync(new NopStationEmployeeModel(), null);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    public async Task<IActionResult> Create(NopStationEmployeeModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var employee = new NopStationEmployee
            {
                Designation = model.Designation,
                CountryId = model.CountryId,
                Name = model.Name
            };

            await _nopStationEmployeeService.AddEmployeeAsync(employee);

            return continueEditing ? RedirectToAction("Edit", new { id = employee.Id }) : RedirectToAction("List");
        }

        model = await _nopStationEmployeeModelFactory.PrepareNopStationEmployeeModelAsync(model, null);
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _nopStationEmployeeService.GetEmployeeByIdAsync(id);
        if (employee == null)
            return RedirectToAction("List");

        var model = await _nopStationEmployeeModelFactory.PrepareNopStationEmployeeModelAsync(null, employee);
        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    public async Task<IActionResult> Edit(NopStationEmployeeModel model, bool continueEditing)
    {
        var employee = await _nopStationEmployeeService.GetEmployeeByIdAsync(model.Id);
        if (employee == null)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            employee.Designation = model.Designation;
            employee.CountryId = model.CountryId;
            employee.Name = model.Name;

            await _nopStationEmployeeService.UpdateEmployeeAsync(employee);

            return continueEditing ? RedirectToAction("Edit", new { id = employee.Id }) : RedirectToAction("List");
        }

        model = await _nopStationEmployeeModelFactory.PrepareNopStationEmployeeModelAsync(model, employee);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(NopStationEmployee model)
    {
        var employee = await _nopStationEmployeeService.GetEmployeeByIdAsync(model.Id);
        if (employee == null)
            return RedirectToAction("List");

        await _nopStationEmployeeService.DeleteEmployeeAsync(employee);
        return RedirectToAction("List");
    }



}
