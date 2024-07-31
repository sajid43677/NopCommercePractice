using AutoMapper;
using Nop.Core.Domain.Catalog;
using Nop.Web.Areas.Admin.Models.Catalog;
using NopStation.Plugin.Misc.FacebookShop.Areas.Admin.Models;

namespace Nop.Plugin.Widgets.NopStationEmployees.Profiles;
public class ProfileMapping : Profile
{
    public ProfileMapping()
    {
        CreateMap<ProductModel, FacebookShopStatusModel>();
    }
}
