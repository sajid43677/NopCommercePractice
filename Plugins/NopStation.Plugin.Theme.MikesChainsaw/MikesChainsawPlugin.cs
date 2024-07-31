using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Infrastructure;
using NopStation.Plugin.Misc.Core.Services;
using Nop.Plugin.NopStation.Theme.MikesChainsaw.Components;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Menu;
using NopStation.Plugin.Misc.Core;

namespace Nop.Plugin.NopStation.Theme.MikesChainsaw;

public class MikesChainsawPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin, INopStationPlugin
{
    #region Fields

    public bool HideInWidgetList => false;

    private readonly ISettingService _settingService;
    private readonly IWebHelper _webHelper;
    private readonly IPictureService _pictureService;
    private readonly INopFileProvider _fileProvider;
    private readonly ILocalizationService _localizationService;
    private readonly IPermissionService _permissionService;
    private readonly INopStationCoreService _nopStationCoreService;

    #endregion

    #region Ctor

    public MikesChainsawPlugin(ISettingService settingService,
        IWebHelper webHelper,
        INopFileProvider nopFileProvider,
        IPictureService pictureService,
        ILocalizationService localizationService,
        IPermissionService permissionService,
        INopStationCoreService nopStationCoreService)
    {
        _settingService = settingService;
        _webHelper = webHelper;
        _fileProvider = nopFileProvider;
        _pictureService = pictureService;
        _localizationService = localizationService;
        _permissionService = permissionService;
        _nopStationCoreService = nopStationCoreService;
    }

    #endregion

    #region Utilities

    private async void CreateSampleData()
    {
        var sampleImagesPath = _fileProvider.MapPath("~/Plugins/NopStation.Theme.MikesChainsaw/Content/sample/");

        var settings = new MikesChainsawSettings()
        {
            EnableImageLazyLoad = true,
            FooterEmail = "sales@nop-station.com",
            LazyLoadPictureId = (await _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "lazy-load.png")), MimeTypes.ImagePng, "lazy-load")).Id,
            OrderCompletedPagePictureId = (await _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "thank-you-for-your-order.png")), MimeTypes.ImagePng, "thank-you-for-your-order")).Id,
            SupportedCardsPictureId = (await _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "footer-card-icons.png")), MimeTypes.ImagePng, "footer-cards")).Id,
            FooterLogoPictureId = (await _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "footer-logo-white.png")), MimeTypes.ImagePng, "footer-logo")).Id,
            BreadcrumbBackgroundPictureId = (await _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "breadcrumb-background.jpg")), MimeTypes.ImageJpeg, "breadcrumb-background")).Id,
            FooterBackgroundPictureId = (await _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "footer-background.jpg")), MimeTypes.ImageJpeg, "footer-background")).Id,
            NewsletterBackgroundPictureId = (await _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "newsletter-background.jpg")), MimeTypes.ImageJpeg, "newsletter-background")).Id,
            ShowLogoAtPageFooter = true,
            ShowSupportedCardsPictureAtPageFooter = true,
            ShowPictureOnOrderCompletedPage = true
        };
        _settingService.SaveSetting(settings);
    }

    #endregion

    #region Methods

    public async Task<IList<string>> GetWidgetZonesAsync()
    {
        await Task.CompletedTask;
        return [
            PublicWidgetZones.HeadHtmlTag,
            PublicWidgetZones.HeaderMenuBefore
        ];
    }

    public override string GetConfigurationPageUrl()
    {
        return _webHelper.GetStoreLocation() + "Admin/MikesChainsaw/Configure";
    }

    public override async Task InstallAsync()
    {
        CreateSampleData();
        await this.InstallPluginAsync(new MikesChainsawPermissionProvider());
        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        await this.UninstallPluginAsync(new MikesChainsawPermissionProvider());
        await base.UninstallAsync();
    }

    public async Task ManageSiteMapAsync(SiteMapNode rootNode)
    {
        if (await _permissionService.AuthorizeAsync(MikesChainsawPermissionProvider.ManageMikesChainsaw))
        {
            var menuItem = new SiteMapNode()
            {
                Title = await _localizationService.GetResourceAsync("Admin.NopStation.Theme.MikesChainsaw.Menu.MikesChainsaw"),
                Visible = true,
                IconClass = "fa-circle-o",
            };

            var configItem = new SiteMapNode()
            {
                Title = await _localizationService.GetResourceAsync("Admin.NopStation.Theme.MikesChainsaw.Menu.Configuration"),
                Url = "/Admin/MikesChainsaw/Configure",
                Visible = true,
                IconClass = "fa-genderless",
                SystemName = "MikesChainsaw.Configuration"
            };
            menuItem.ChildNodes.Add(configItem);

            await _nopStationCoreService.ManageSiteMapAsync(rootNode, menuItem, NopStationMenuType.Theme);
        }
    }

    public Type GetWidgetViewComponent(string widgetZone)
    {
        if (widgetZone == PublicWidgetZones.HeaderMenuBefore)
            return typeof(BookAServiceViewComponent);

        return typeof(MikesChainsawViewComponent);
    }

    public List<KeyValuePair<string, string>> PluginResouces()
    {
        var list = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Menu.MikesChainsaw", "MikesChainsaw"),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Menu.Configuration", "Configuration"),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.EnableImageLazyLoad", "Enable image lazy-load"),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.EnableImageLazyLoad.Hint", "Check to enable lazy-load for product box image."),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.LazyLoadPictureId", "Lazy-load picture"),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.LazyLoadPictureId.Hint", "This picture will be displayed initially in product box. Uploaded picture size should not be more than 4-5 KB."),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.ShowPictureOnOrderCompletedPage", "Show picture on order completed page"),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.ShowPictureOnOrderCompletedPage.Hint", "Determines whether design image will be displayed on order completed page or not."),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.OrderCompletedPagePictureId", "Order completed page picture"),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.OrderCompletedPagePictureId.Hint", "This picture will be displayed in order completed page."),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.FooterBackgroundPictureId", "Footer background picture"),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.FooterBackgroundPictureId.Hint", "This picture will be displayed in footer background."),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.NewsletterBackgroundPictureId", "Newsletter background picture"),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.NewsletterBackgroundPictureId.Hint", "This picture will be displayed in newsletter background."),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.BreadcrumbBackgroundPictureId", "Breadcrumb background picture"),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.BreadcrumbBackgroundPictureId.Hint", "This picture will be displayed in breadcrumb background."),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.ShowSupportedCardsPictureAtPageFooter", "Show supported cards picture at page footer"),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.ShowSupportedCardsPictureAtPageFooter.Hint", "Determines whether supported cards picture will be displayed at page footer or not."),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.SupportedCardsPictureId", "Supported cards picture"),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.SupportedCardsPictureId.Hint", "The single picture of supported cards (expected image height 30px)."),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.ShowLogoAtPageFooter", "Show logo at page footer"),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.ShowLogoAtPageFooter.Hint", "Determines whether logo will be displayed at page footer or not."),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.FooterLogoPictureId", "Footer logo"),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.FooterLogoPictureId.Hint", "This footer logo for this store (expected image height 40px)."),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.CustomCss", "Custom Css"),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.CustomCss.Hint", "Write custom CSS for your site. It will be rendered in head section of html page."),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.FooterEmail", "Footer email"),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Fields.FooterEmail.Hint", "Specify email which which will be displayed at page footer."),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration", "MikesChainsaw settings"),
            new KeyValuePair<string, string>("Admin.NopStation.Theme.MikesChainsaw.Configuration.Updated", "MikesChainsaw configuration updated successfully."),
            new KeyValuePair<string, string>("NopStation.Theme.MikesChainsaw.ProductDetailsPage.Overview", "Overview"),
            new KeyValuePair<string, string>("NopStation.Theme.MikesChainsaw.ProductDetailsPage.Specifications", "Specifications"),
            new KeyValuePair<string, string>("NopStation.Theme.MikesChainsaw.ProductDetailsPage.ProductTags", "Product Tags"),
            new KeyValuePair<string, string>("NopStation.Theme.MikesChainsaw.ShoppingCart.Info", "Info"),
            new KeyValuePair<string, string>("NopStation.Theme.MikesChainsaw.bookservice.Email", "Email"),
            new KeyValuePair<string, string>("NopStation.Theme.MikesChainsaw.bookservice.Subject", "Subject"),
            new KeyValuePair<string, string>("NopStation.Theme.MikesChainsaw.bookservice.PhoneNumber", "Phone Number"),
            new KeyValuePair<string, string>("NopStation.Theme.MikesChainsaw.bookservice.PickupRequired", "Pick-up Required? (if yes please provide pick up address and preferred time)"),
            new KeyValuePair<string, string>("NopStation.Theme.MikesChainsaw.bookservice.MachineMakeModel", "Machine Make &amp; Model"),
            new KeyValuePair<string, string>("NopStation.Theme.MikesChainsaw.bookservice.BriefSummary", "Brief summary of work to be carried out"),
            new KeyValuePair<string, string>("NopStation.Theme.MikesChainsaw.bookservice.FullName", "Full Name"),
            new KeyValuePair<string, string>("PageTitle.BookAService", "Book A Service"),
            new KeyValuePair<string, string>("BookAService.YourEnquiryHasBeenSent", "Your request has been received"),
            new KeyValuePair<string, string>("BookService.Button", "Submit"),
            new KeyValuePair<string, string>("NopStation.Theme.MikesChainsaw.bookservice.Menu", "Book A Service"),
            new KeyValuePair<string, string>("NopStation.Theme.MikesChainsaw.bookservice.Name.Required", "Name is required!"),
            new KeyValuePair<string, string>("NopStation.Theme.MikesChainsaw.bookservice.Email.Required", "Email is required!"),
            new KeyValuePair<string, string>("NopStation.Theme.MikesChainsaw.bookservice.PhoneNumber.Required", "Phone Number is required!"),
            new KeyValuePair<string, string>("NopStation.Theme.MikesChainsaw.bookservice.PickupRequired.Required", "Pickup details is required!"),
            new KeyValuePair<string, string>("NopStation.Theme.MikesChainsaw.bookservice.MachineMakeModel.Required", "Machine Make & Model is required!"),
            new KeyValuePair<string, string>("NopStation.Theme.MikesChainsaw.bookservice.BriefSummary.Required", "Summary is required!")
        };



        return list;
    }

    #endregion
}