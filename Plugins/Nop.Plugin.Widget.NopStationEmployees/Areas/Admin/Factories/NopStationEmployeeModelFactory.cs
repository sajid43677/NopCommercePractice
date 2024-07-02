using System.Resources;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Nop.Plugin.Widget.NopStationEmployees.Domain;
using Nop.Plugin.Widget.NopStationEmployees.Services;
using Nop.Plugin.Widgets.NopStationEmployees.Areas.Admin.Models;
using Nop.Services;
using Nop.Services.Localization;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Widgets.NopStationEmployees.Areas.Admin.Factories;

public class NopStationEmployeeModelFactory : INopStationEmployeeModelFactory
{
    private readonly INopStationEmployeeService _nopStationEmployeeService;
    private readonly ILocalizationService _localizationService;

    public NopStationEmployeeModelFactory(INopStationEmployeeService nopStationEmployeeService, ILocalizationService localizationService)
    {
        _nopStationEmployeeService = nopStationEmployeeService;
        _localizationService = localizationService;
    }

    public async Task<NopStationEmployeeSearchModel> PrepareNopStationEmployeeSearchModelAsync(NopStationEmployeeSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        searchModel.AvailableCountries = (await Country.Bangladesh.ToSelectListAsync()).ToList();
        var allCountry = _localizationService.GetLocaleStringResourceByName("admin.widgets.nopstationemployee.allcountry", 1);
        searchModel.AvailableCountries.Insert(0, new SelectListItem { Text = allCountry.ResourceValue, Value = "0" });
        searchModel.SetGridPageSize();
        return searchModel;
    }

    public async Task<NopStationEmployeeModel> PrepareNopStationEmployeeModelAsync(NopStationEmployeeModel model, NopStationEmployee nopStationEmployee, bool excludeProperties = false)
    {
        if (nopStationEmployee != null)
        {
            if (model == null)
                model = new NopStationEmployeeModel()
                {
                    Id = nopStationEmployee.Id,
                    Name = nopStationEmployee.Name,
                    Designation = nopStationEmployee.Designation,
                    CountryId = nopStationEmployee.CountryId
                };
            model.Country = await _localizationService.GetLocalizedEnumAsync(nopStationEmployee.Country);
        }

        if (!excludeProperties)
            model.AvailableCountries = (await Country.Bangladesh.ToSelectListAsync()).ToList();

        return model;
    }

    public async Task<NopStationEmployeeListModel> PrepareNopStationEmployeeListModelAsync(NopStationEmployeeSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(nameof(searchModel));

        var employees = await _nopStationEmployeeService.SearchNopStationEmployeesAsync(searchModel.Name, searchModel.CountryId,
            pageIndex: searchModel.Page - 1,
            pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new NopStationEmployeeListModel().PrepareToGridAsync(searchModel, employees, () =>
        {
            return employees.SelectAwait(async employee =>
            {
                return await PrepareNopStationEmployeeModelAsync(null, employee, true);
            });
        });

        return model;
    }
}
