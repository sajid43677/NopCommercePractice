using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.NopStationEmployees.Models;
public record NopStationEmployeePublicModel : BaseNopEntityModel
{
    
    [NopResourceDisplayName("Widgets.NopStationEmployee.Fields.Name")]
    public string Name { get; set; }
    [NopResourceDisplayName("Widgets.NopStationEmployee.Fields.Designation")]
    public string Designation { get; set; }
    [NopResourceDisplayName("Widgets.NopStationEmployee.Fields.Country")]
    public string Country { get; set; }

}
