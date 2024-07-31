using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using NopStation.Plugin.Misc.Core.Controllers;
using NopStation.Plugin.Misc.Core.Filters;
using NopStation.Plugin.Payments.StripeHostedCheckout.Models;

namespace NopStation.Plugin.Payments.StripeHostedCheckout.Controllers
{
    public class PaymentStripeHostedCheckoutController : NopStationAdminController
    {
        #region Fileds

        private readonly IPermissionService _permissionService;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;

        #endregion

        #region Ctor

        public PaymentStripeHostedCheckoutController(IPermissionService permissionService,
            IStoreContext storeContext,
            ISettingService settingService,
            ILocalizationService localizationService,
            INotificationService notificationService)
        {
            _permissionService = permissionService;
            _storeContext = storeContext;
            _settingService = settingService;
            _localizationService = localizationService;
            _notificationService = notificationService;
        }

        #endregion

        #region Methods
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var stripeHostedCheckoutSettings = _settingService.LoadSetting<StripeHostedCheckoutSettings>(storeScope);

            var model = new ConfigurationModel
            {
                ApiKey = stripeHostedCheckoutSettings.ApiKey,
                AdditionalFee = stripeHostedCheckoutSettings.AdditionalFee,
                AdditionalFeePercentage = stripeHostedCheckoutSettings.AdditionalFeePercentage,
                ActiveStoreScopeConfiguration = storeScope,
                PaymentTypeId = (int)stripeHostedCheckoutSettings.PaymentType
            };
            model.PaymentTypes = (await PaymentType.Capture.ToSelectListAsync(false))
                .Select(item => new SelectListItem(item.Text, item.Value)).ToList();

            if (storeScope <= 0)
                return View("~/Plugins/NopStation.Plugin.Payments.StripeHostedCheckout/Views/Configure.cshtml", model);

            model.ApiKey_OverrideForStore = await _settingService.SettingExistsAsync(stripeHostedCheckoutSettings, x => x.ApiKey, storeScope);
            model.AdditionalFee_OverrideForStore = await _settingService.SettingExistsAsync(stripeHostedCheckoutSettings, x => x.AdditionalFee, storeScope);
            model.PaymentTypeId_OverrideForStore = await _settingService.SettingExistsAsync(stripeHostedCheckoutSettings, x => x.PaymentType, storeScope);
            model.AdditionalFeePercentage_OverrideForStore = await _settingService.SettingExistsAsync(stripeHostedCheckoutSettings, x => x.AdditionalFeePercentage, storeScope);

            return View("~/Plugins/NopStation.Plugin.Payments.StripeHostedCheckout/Views/Configure.cshtml", model);
        }

        [EditAccess, HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var stripeHostedCheckoutSettings = await _settingService.LoadSettingAsync<StripeHostedCheckoutSettings>(storeScope);

            stripeHostedCheckoutSettings.ApiKey = model.ApiKey;
            stripeHostedCheckoutSettings.AdditionalFee = model.AdditionalFee;
            stripeHostedCheckoutSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            stripeHostedCheckoutSettings.PaymentType = (PaymentType)model.PaymentTypeId;

            await _settingService.SaveSettingOverridablePerStoreAsync(stripeHostedCheckoutSettings, x => x.ApiKey, model.ApiKey_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(stripeHostedCheckoutSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(stripeHostedCheckoutSettings, x => x.PaymentType, model.PaymentTypeId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(stripeHostedCheckoutSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));
            return await Configure();
        }
        #endregion
    }
}