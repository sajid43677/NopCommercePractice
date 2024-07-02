using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.NopStationEmployees.Areas.Admin.Models;
public record NopStationEmployeeSearchModel : BaseSearchModel
{
    public NopStationEmployeeSearchModel()
    {
        AvailableCountries = new List<SelectListItem>();
    }

    [NopResourceDisplayName("Admin.Widgets.NopStationEmployee.List.Name")]
    public string Name { get; set; }
    [NopResourceDisplayName("Admin.Widgets.NopStationEmployee.List.Country")]
    public int CountryId { get; set; }

    public IList<SelectListItem> AvailableCountries { get; set; }
}
