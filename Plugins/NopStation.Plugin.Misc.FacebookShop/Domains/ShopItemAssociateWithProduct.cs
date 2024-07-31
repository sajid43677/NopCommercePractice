using Nop.Core.Domain.Catalog;

namespace NopStation.Plugin.Misc.FacebookShop.Domains
{
    public partial class ShopItemAssociateWithProduct
    {
        public Product Product { get; set; }
        public ShopItem ShopItem { get; set; }
    }
}
