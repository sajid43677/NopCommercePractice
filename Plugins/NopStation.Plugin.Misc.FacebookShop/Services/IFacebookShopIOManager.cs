using System.Threading.Tasks;

namespace NopStation.Plugin.Misc.FacebookShop.Services
{
    public partial interface IFacebookShopIOManager
    {
        Task<string> WriteOrUpdateShopItemToExcelAsync();
    }
}
