using Nop.Plugin.Widget.NopStationEmployees.Domain;
using Nop.Plugin.Widgets.NopStationEmployees.Areas.Admin.Models;

namespace Nop.Plugin.Widgets.NopStationEmployees.Areas.Admin.Factories;

public interface INopStationEmployeeModelFactory
{
    Task<NopStationEmployeeListModel> PrepareNopStationEmployeeListModelAsync(NopStationEmployeeSearchModel searchModel);
    Task<NopStationEmployeeModel> PrepareNopStationEmployeeModelAsync(NopStationEmployeeModel model, NopStationEmployee nopStationEmployee, bool excludeProperties = false);
    Task<NopStationEmployeeSearchModel> PrepareNopStationEmployeeSearchModelAsync(NopStationEmployeeSearchModel searchModel);
}