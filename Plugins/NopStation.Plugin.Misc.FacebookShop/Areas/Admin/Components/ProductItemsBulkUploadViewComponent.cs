using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Cms;
using Nop.Services.Security;
using NopStation.Plugin.Misc.Core.Components;

namespace NopStation.Plugin.Misc.FacebookShop.Areas.Admin.Components
{
    public class ProductItemsBulkUploadViewComponent : NopStationViewComponent
    {
        #region Properties

        private readonly IPermissionService _permissionService;
        private readonly IWidgetPluginManager _widgetPluginManager;

        #endregion

        #region Ctor

        public ProductItemsBulkUploadViewComponent(IPermissionService permissionService, 
            IWidgetPluginManager widgetPluginManager)
        {
            _permissionService = permissionService;
            _widgetPluginManager = widgetPluginManager;
        }

        #endregion

        #region Methods

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            if (!await _permissionService.AuthorizeAsync(FacebookShopPermissionProvider.ManageFacebookShopProducts) ||
                !await _widgetPluginManager.IsPluginActiveAsync(FacebookShopDefaults.SystemName))
                return Content(string.Empty);

            return View();
        }

        #endregion
    }
}
