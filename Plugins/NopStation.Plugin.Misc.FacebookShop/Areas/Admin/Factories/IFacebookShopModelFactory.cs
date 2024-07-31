using System.Threading.Tasks;
using NopStation.Plugin.Misc.FacebookShop.Areas.Admin.Models;

namespace NopStation.Plugin.Misc.FacebookShop.Areas.Admin.Factories
{
    public partial interface IFacebookShopModelFactory
    {
        Task<ConfigurationModel> PrepareConfigurationModelAsync();
    }
}
