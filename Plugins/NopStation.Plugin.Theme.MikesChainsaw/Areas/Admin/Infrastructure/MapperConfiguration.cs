using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.NopStation.Theme.MikesChainsaw.Areas.Admin.Models;

namespace Nop.Plugin.NopStation.Theme.MikesChainsaw.Areas.Admin.Infrastructure;

public class MapperConfiguration : Profile, IOrderedMapperProfile
{
    public int Order => 1;

    public MapperConfiguration()
    {
        CreateMap<MikesChainsawSettings, ConfigurationModel>()
                .ForMember(model => model.CustomCss_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EnableImageLazyLoad_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.FooterEmail_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.FooterLogoPictureId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.LazyLoadPictureId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.OrderCompletedPagePictureId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowLogoAtPageFooter_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowPictureOnOrderCompletedPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowSupportedCardsPictureAtPageFooter_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.SupportedCardsPictureId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.BreadcrumbBackgroundPictureId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.FooterBackgroundPictureId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.NewsletterBackgroundPictureId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CustomProperties, options => options.Ignore())
                .ForMember(model => model.ActiveStoreScopeConfiguration, options => options.Ignore());
        CreateMap<ConfigurationModel, MikesChainsawSettings>();
    }
}
