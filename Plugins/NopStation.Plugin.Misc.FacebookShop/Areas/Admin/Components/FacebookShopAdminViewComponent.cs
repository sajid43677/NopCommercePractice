using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Models.Catalog;
using NopStation.Plugin.Misc.Core.Components;
using NopStation.Plugin.Misc.FacebookShop.Areas.Admin.Factories;
using NopStation.Plugin.Misc.FacebookShop.Areas.Admin.Models;
using NopStation.Plugin.Misc.FacebookShop.Services;

namespace NopStation.Plugin.Misc.FacebookShop.Areas.Admin.Components
{
    public class FacebookShopAdminViewComponent : NopStationViewComponent
    {
        #region Properties

        private readonly IPermissionService _permissionService;
        private readonly IWidgetPluginManager _widgetPluginManager;
        private readonly IProductService _productService;
        private readonly IShopItemModelFactory _shopItemModelFactory;
        private readonly IFacebookShopService _facebookShopService;

        #endregion

        #region Ctor

        public FacebookShopAdminViewComponent(IPermissionService permissionService, 
            IWidgetPluginManager widgetPluginManager,
            IProductService productService,
            IShopItemModelFactory shopItemModelFactory,
            IFacebookShopService facebookShopService)
        {
            _permissionService = permissionService;
            _widgetPluginManager = widgetPluginManager;
            _productService = productService;
            _shopItemModelFactory = shopItemModelFactory;
            _facebookShopService = facebookShopService;
        }

        #endregion

        #region Methods

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            if (!await _permissionService.AuthorizeAsync(FacebookShopPermissionProvider.ManageFacebookShopProducts) ||
                !await _widgetPluginManager.IsPluginActiveAsync(FacebookShopDefaults.SystemName))
                return Content(string.Empty);

            if (additionalData.GetType() != typeof(ProductModel))
                return Content("");

            var productModel = additionalData as ProductModel;

            var product = await _productService.GetProductByIdAsync(productModel.Id);
            if (product == null || product.Deleted)
                return Content("");

            var existingShopItem = await _facebookShopService.GetShopItemByProductIdAsync(productModel.Id);

            var model = await _shopItemModelFactory.PrepareShopItemModelAsync(existingShopItem == null ? new ShopItemModel() : null, 
                existingShopItem ?? null, productModel);

            return View(model);
        }

        #endregion
    }
}
