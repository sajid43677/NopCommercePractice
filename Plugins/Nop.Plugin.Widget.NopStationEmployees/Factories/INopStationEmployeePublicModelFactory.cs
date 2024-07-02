using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Widget.NopStationEmployees.Domain;
using Nop.Plugin.Widgets.NopStationEmployees.Areas.Admin.Models;
using Nop.Plugin.Widgets.NopStationEmployees.Models;

namespace Nop.Plugin.Widgets.NopStationEmployees.Factories;
public interface INopStationEmployeePublicModelFactory
{
    /*Task<NopStationEmployeeListPublicModel> PrepareNopStationEmployeeListPublicModelAsync(NopStationEmployeeSearchPublicModel searchModel);
    Task<NopStationEmployeePublicModel> PrepareNopStationEmployeePublicModelAsync(NopStationEmployeePublicModel model, NopStationEmployee nopStationEmployee, bool excludeProperties = false);
    Task<NopStationEmployeeSearchPublicModel> PrepareNopStationEmployeeSearchPublicModelAsync(NopStationEmployeeSearchPublicModel searchModel);*/
    Task<List<NopStationEmployeePublicModel>> PreparePublicModel();
}
