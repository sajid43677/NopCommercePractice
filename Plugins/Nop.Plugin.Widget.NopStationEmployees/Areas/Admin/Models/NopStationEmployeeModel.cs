using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.NopStationEmployees.Areas.Admin.Models;
public record NopStationEmployeeModel : BaseNopEntityModel
{
    public NopStationEmployeeModel()
    {
        AvailableCountries = new List<SelectListItem>();
    }

    [NopResourceDisplayName("Admin.Widgets.NopStationEmployee.Fields.Name")]
    public string Name { get; set; }
    [NopResourceDisplayName("Admin.Widgets.NopStationEmployee.Fields.Designation")]
    public string Designation { get; set; }
    [NopResourceDisplayName("Admin.Widgets.NopStationEmployee.Fields.Country")]
    public string Country { get; set; }
    [NopResourceDisplayName("Admin.Widgets.NopStationEmployee.Fields.CountryId")]
    public int CountryId { get; set; }

    public IList<SelectListItem> AvailableCountries { get; set; }
}
