using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.NopStation.Theme.MikesChainsaw.Areas.Admin.Models;

public record ConfigurationModel : BaseNopModel, ISettingsModel
{
    [NopResourceDisplayName("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.EnableImageLazyLoad")]
    public bool EnableImageLazyLoad { get; set; }
    public bool EnableImageLazyLoad_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.LazyLoadPictureId")]
    [UIHint("Picture")]
    public int LazyLoadPictureId { get; set; }
    public bool LazyLoadPictureId_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.ShowPictureOnOrderCompletedPage")]
    public bool ShowPictureOnOrderCompletedPage { get; set; }
    public bool ShowPictureOnOrderCompletedPage_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.OrderCompletedPagePictureId")]
    [UIHint("Picture")]
    public int OrderCompletedPagePictureId { get; set; }
    public bool OrderCompletedPagePictureId_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.ShowSupportedCardsPictureAtPageFooter")]
    public bool ShowSupportedCardsPictureAtPageFooter { get; set; }
    public bool ShowSupportedCardsPictureAtPageFooter_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.SupportedCardsPictureId")]
    [UIHint("Picture")]
    public int SupportedCardsPictureId { get; set; }
    public bool SupportedCardsPictureId_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.ShowLogoAtPageFooter")]
    public bool ShowLogoAtPageFooter { get; set; }
    public bool ShowLogoAtPageFooter_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.FooterLogoPictureId")]
    [UIHint("Picture")]
    public int FooterLogoPictureId { get; set; }
    public bool FooterLogoPictureId_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.FooterEmail")]
    public string FooterEmail { get; set; }
    public bool FooterEmail_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.CustomCss")]
    public string CustomCss { get; set; }
    public bool CustomCss_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.FooterBackgroundPictureId")]
    [UIHint("Picture")]
    public int FooterBackgroundPictureId { get; set; }
    public bool FooterBackgroundPictureId_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.NewsletterBackgroundPictureId")]
    [UIHint("Picture")]
    public int NewsletterBackgroundPictureId { get; set; }
    public bool NewsletterBackgroundPictureId_OverrideForStore { get; set; }

    [NopResourceDisplayName("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.BreadcrumbBackgroundPictureId")]
    [UIHint("Picture")]
    public int BreadcrumbBackgroundPictureId { get; set; }
    public bool BreadcrumbBackgroundPictureId_OverrideForStore { get; set; }

    public int ActiveStoreScopeConfiguration { get; set; }
}
