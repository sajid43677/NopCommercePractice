using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.NopStationEmployees.Models;
public record NopStationEmployeeSearchPublicModel : BaseSearchModel
{
    public NopStationEmployeeSearchPublicModel()
    {
        AvailableCountries = new List<SelectListItem>();
    }

    [NopResourceDisplayName("Widgets.NopStationEmployee.List.Name")]
    public string Name { get; set; }
    [NopResourceDisplayName("Widgets.NopStationEmployee.List.Country")]
    public int CountryId { get; set; }

    public IList<SelectListItem> AvailableCountries { get; set; }
}
