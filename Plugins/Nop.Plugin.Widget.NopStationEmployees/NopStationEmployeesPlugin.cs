using Nop.Core;
using Nop.Plugin.Widget.NopStationEmployees.Components;
using Nop.Services.Cms;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widget.NopStationEmployees
{
    public class NopStationEmployeesPlugin: BasePlugin, IWidgetPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;

        public NopStationEmployeesPlugin(IWebHelper webHelper, ILocalizationService localizationService)
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
        }
        
        public bool HideInWidgetList => false;

        public Type GetWidgetViewComponent(string widgetZone)
        {
            return typeof(NopStationEmployeesComponent);
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.HomepageBeforeCategories });
        }

        public override async Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Admin.Widgets.NopStationEmployee"] = "Employees",
                ["Admin.Widgets.NopStationEmployee.AddNew"] = "Add new employee",
                ["Admin.Widgets.Employees.EditDetails"] = "Edit employee details",
                ["Admin.Widgets.Employees.BackToList"] = "back to employee list",
                ["Admin.Widgets.NopStationEmployee.Fields.Name"] = "Name",
                ["Admin.Widgets.NopStationEmployee.Fields.Designation"] = "Designation",
                ["Admin.Widgets.NopStationEmployee.Fields.Country"] = "Country",
                ["Admin.Widgets.NopStationEmployee.Fields.Name.Hint"] = "Enter employee name.",
                ["Admin.Widgets.NopStationEmployee.Fields.Designation.Hint"] = "Enter employee designation.",
                ["Admin.Widgets.NopStationEmployee.Fields.Country.Hint"] = "Select employee's Country.",
                ["Admin.Widgets.NopStationEmployee.List.Name"] = "Name",
                ["Admin.Widgets.NopStationEmployee.List.Country"] = "Country",
                ["Admin.Widgets.NopStationEmployee.List.Name.Hint"] = "Search by employee name.",
                ["Admin.Widgets.NopStationEmployee.List.EmployeeStatus.Hint"] = "Search by employee's Country.",
            });

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{this._webHelper.GetStoreLocation()}Admin/NopStationEmployee/List";
        }
    }
}
