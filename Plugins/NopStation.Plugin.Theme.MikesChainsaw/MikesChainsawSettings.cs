using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Plugin.NopStation.Theme.MikesChainsaw;

public class MikesChainsawSettings : ISettings
{
    public bool EnableImageLazyLoad { get; set; }

    public int LazyLoadPictureId { get; set; }

    public bool ShowPictureOnOrderCompletedPage { get; set; }

    public int OrderCompletedPagePictureId { get; set; }

    public bool ShowSupportedCardsPictureAtPageFooter { get; set; }

    public int SupportedCardsPictureId { get; set; }

    public bool ShowLogoAtPageFooter { get; set; }

    public int FooterLogoPictureId { get; set; }

    public string FooterEmail { get; set; }

    public string CustomCss { get; set; }

    public int FooterBackgroundPictureId { get; set; }

    public int NewsletterBackgroundPictureId { get; set; }

    public int BreadcrumbBackgroundPictureId { get; set; }
}
