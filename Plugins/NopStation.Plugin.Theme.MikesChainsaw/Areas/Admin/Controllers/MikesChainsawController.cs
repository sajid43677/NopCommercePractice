using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using NopStation.Plugin.Misc.Core;
using Nop.Plugin.NopStation.Theme.MikesChainsaw.Areas.Admin.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using System.Threading.Tasks;

namespace Nop.Plugin.NopStation.Theme.MikesChainsaw.Areas.Admin.Controllers;

public class MikesChainsawController : BaseAdminController
{
    #region Fields

    private readonly IPermissionService _permissionService;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;

    #endregion

    #region Ctor

    public MikesChainsawController(IPermissionService permissionService,
        ISettingService settingService,
        IStoreContext storeContext,
        ILocalizationService localizationService,
        INotificationService notificationService)
    {
        _permissionService = permissionService;
        _settingService = settingService;
        _storeContext = storeContext;
        _localizationService = localizationService;
        _notificationService = notificationService;
    }

    #endregion

    #region Methods

    public async Task<IActionResult> ConfigureAsync()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
            return AccessDeniedView();

        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var berrySettings = _settingService.LoadSetting<MikesChainsawSettings>(storeId);
        var model = berrySettings.ToSettingsModel<ConfigurationModel>();

        model.ActiveStoreScopeConfiguration = storeId;

        if (storeId <= 0)
            return View(model);

        model.CustomCss_OverrideForStore = _settingService.SettingExists(berrySettings, x => x.CustomCss, storeId);
        model.EnableImageLazyLoad_OverrideForStore = _settingService.SettingExists(berrySettings, x => x.EnableImageLazyLoad, storeId);
        model.FooterEmail_OverrideForStore = _settingService.SettingExists(berrySettings, x => x.FooterEmail, storeId);
        model.FooterLogoPictureId_OverrideForStore = _settingService.SettingExists(berrySettings, x => x.FooterLogoPictureId, storeId);
        model.LazyLoadPictureId_OverrideForStore = _settingService.SettingExists(berrySettings, x => x.LazyLoadPictureId, storeId);
        model.OrderCompletedPagePictureId_OverrideForStore = _settingService.SettingExists(berrySettings, x => x.OrderCompletedPagePictureId, storeId);
        model.ShowLogoAtPageFooter_OverrideForStore = _settingService.SettingExists(berrySettings, x => x.ShowLogoAtPageFooter, storeId);
        model.ShowPictureOnOrderCompletedPage_OverrideForStore = _settingService.SettingExists(berrySettings, x => x.ShowPictureOnOrderCompletedPage, storeId);
        model.ShowSupportedCardsPictureAtPageFooter_OverrideForStore = _settingService.SettingExists(berrySettings, x => x.ShowSupportedCardsPictureAtPageFooter, storeId);
        model.SupportedCardsPictureId_OverrideForStore = _settingService.SettingExists(berrySettings, x => x.SupportedCardsPictureId, storeId);
        model.BreadcrumbBackgroundPictureId_OverrideForStore = _settingService.SettingExists(berrySettings, x => x.BreadcrumbBackgroundPictureId, storeId);
        model.FooterBackgroundPictureId_OverrideForStore = _settingService.SettingExists(berrySettings, x => x.FooterBackgroundPictureId, storeId);
        model.NewsletterBackgroundPictureId_OverrideForStore = _settingService.SettingExists(berrySettings, x => x.NewsletterBackgroundPictureId, storeId);

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ConfigureAsync(ConfigurationModel model)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
            return AccessDeniedView();

        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var berrySettings = _settingService.LoadSetting<MikesChainsawSettings>(storeScope);
        berrySettings = model.ToSettings(berrySettings);

        await _settingService.SaveSettingOverridablePerStoreAsync(berrySettings, x => x.CustomCss, model.CustomCss_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(berrySettings, x => x.EnableImageLazyLoad, model.EnableImageLazyLoad_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(berrySettings, x => x.FooterEmail, model.FooterEmail_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(berrySettings, x => x.FooterLogoPictureId, model.FooterLogoPictureId_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(berrySettings, x => x.LazyLoadPictureId, model.LazyLoadPictureId_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(berrySettings, x => x.OrderCompletedPagePictureId, model.OrderCompletedPagePictureId_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(berrySettings, x => x.ShowLogoAtPageFooter, model.ShowLogoAtPageFooter_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(berrySettings, x => x.ShowPictureOnOrderCompletedPage, model.ShowPictureOnOrderCompletedPage_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(berrySettings, x => x.ShowSupportedCardsPictureAtPageFooter, model.ShowSupportedCardsPictureAtPageFooter_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(berrySettings, x => x.SupportedCardsPictureId, model.SupportedCardsPictureId_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(berrySettings, x => x.BreadcrumbBackgroundPictureId, model.BreadcrumbBackgroundPictureId_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(berrySettings, x => x.FooterBackgroundPictureId, model.FooterBackgroundPictureId_OverrideForStore, storeScope, false);
        await _settingService.SaveSettingOverridablePerStoreAsync(berrySettings, x => x.NewsletterBackgroundPictureId, model.NewsletterBackgroundPictureId_OverrideForStore, storeScope, false);

        _settingService.ClearCache();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.NopStation.Theme.MikesChainsaw.Configuration.Updated"));

        return RedirectToAction("Configure");
    }

    #endregion
}
