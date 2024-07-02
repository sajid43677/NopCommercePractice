using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Widget.NopStationEmployees.Domain;
using Nop.Plugin.Widget.NopStationEmployees.Services;
using Nop.Plugin.Widgets.NopStationEmployees.Areas.Admin.Models;
using Nop.Plugin.Widgets.NopStationEmployees.Models;
using Nop.Services;
using Nop.Services.Localization;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Widgets.NopStationEmployees.Factories;
public class NopStationEmployeePublicModelFactory : INopStationEmployeePublicModelFactory
{
    private readonly INopStationEmployeeService _nopStationEmployeeService;
    private readonly ILocalizationService _localizationService;
    private readonly IMapper _mapper;

    public NopStationEmployeePublicModelFactory(INopStationEmployeeService nopStationEmployeeService, ILocalizationService localizationService, IMapper mapper)
    {
        _nopStationEmployeeService = nopStationEmployeeService;
        _localizationService = localizationService;
        _mapper = mapper;
    }

    /*public async Task<NopStationEmployeeListPublicModel> PrepareNopStationEmployeeListPublicModelAsync(NopStationEmployeeSearchPublicModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(nameof(searchModel));

        var employees = await _nopStationEmployeeService.SearchNopStationEmployeesAsync(searchModel.Name, searchModel.CountryId,
            pageIndex: searchModel.Page - 1,
            pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new NopStationEmployeeListPublicModel().PrepareToGridAsync(searchModel, employees, () =>
        {
            return employees.SelectAwait(async employee =>
            {
                return await PrepareNopStationEmployeePublicModelAsync(null, employee, true);
            });
        });

        return model;
    }

    public async Task<NopStationEmployeePublicModel> PrepareNopStationEmployeePublicModelAsync(NopStationEmployeePublicModel model, NopStationEmployee nopStationEmployee, bool excludeProperties = false)
    {
        if (nopStationEmployee != null)
        {
            if (model == null)
                model = new NopStationEmployeePublicModel()
                {
                    Id = nopStationEmployee.Id,
                    Name = nopStationEmployee.Name,
                    Designation = nopStationEmployee.Designation,
                };
            model.Country = await _localizationService.GetLocalizedEnumAsync(nopStationEmployee.Country);
        }

        return model;
    }

    public async Task<NopStationEmployeeSearchPublicModel> PrepareNopStationEmployeeSearchPublicModelAsync(NopStationEmployeeSearchPublicModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        searchModel.AvailableCountries = (await Country.Bangladesh.ToSelectListAsync()).ToList();
        var allCountry = _localizationService.GetLocaleStringResourceByName("admin.widgets.nopstationemployee.allcountry", 1);
        searchModel.AvailableCountries.Insert(0, new SelectListItem { Text = allCountry.ResourceValue, Value = "0" });
        searchModel.SetGridPageSize();
        return searchModel;
    }*/

    public async Task<List<NopStationEmployeePublicModel>> PreparePublicModel()
    {
        var employees = await _nopStationEmployeeService.GetAllEmployeesAsync();
        var employeesModel = _mapper.Map<List<NopStationEmployeePublicModel>>(employees);

        return employeesModel;
    }
}
