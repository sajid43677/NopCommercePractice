using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Web.Areas.Admin.Models.Catalog;
using NopStation.Plugin.Misc.FacebookShop.Areas.Admin.Models;
using NopStation.Plugin.Misc.FacebookShop.Domains;

namespace NopStation.Plugin.Misc.FacebookShop.Areas.Admin.Factories
{
    public partial interface IShopItemModelFactory
    {
        Task<ProductWithShopItemListModel> PrepareProductWithShopItemModel(ProductSearchModel searchModel);
        Task<List<ShopItemAssociateWithProduct>> PrepareShopItemAssociateWithProducts();
        Task<ShopItemModel> PrepareShopItemModelAsync(ShopItemModel model, ShopItem shopItem, ProductModel productModel, bool excludeProperties = false);
    }
}
