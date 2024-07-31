using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Nop.Core;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Core.Infrastructure;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Menu;
using NopStation.Plugin.Misc.Core;
using NopStation.Plugin.Misc.Core.Services;
using NopStation.Plugin.Misc.FacebookShop;
using NopStation.Plugin.Misc.FacebookShop.Areas.Admin.Components;
using NopStation.Plugin.Misc.FacebookShop.Models;

namespace NopStation.Plugin.Misc.FacebookShop
{
    public class FacebookShopPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin, INopStationPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly INopStationCoreService _nopStationCoreService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly INopFileProvider _nopFileProvider;

        public bool HideInWidgetList => false;

        public FacebookShopPlugin(IWebHelper webHelper,
            ILocalizationService localizationService,
            INopStationCoreService nopStationCoreService,
            IScheduleTaskService scheduleTaskService,
            IPermissionService permissionService,
            IStoreService storeService,
            INopFileProvider nopFileProvider)
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
            _nopStationCoreService = nopStationCoreService;
            _scheduleTaskService = scheduleTaskService;
            _permissionService = permissionService;
            _storeService = storeService;
            _nopFileProvider = nopFileProvider;
        }

        protected void WriteToCsvFile(string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath, false, new UTF8Encoding(true)))
            using (CsvWriter cw = new CsvWriter(sw, new CultureInfo(NopCommonDefaults.DefaultLanguageCulture)))
            {
                cw.WriteHeader<ItemModel>();
            }
        }

        protected async Task CreateCsvFiles()
        {
            var stores = await _storeService.GetAllStoresAsync();
            foreach (var store in stores)
            {
                var basePath = _nopFileProvider.MapPath("~/Plugins/NopStation.Plugin.Misc.FacebookShop/Files/");
                var fileName = $"{FacebookShopDefaults.FileName}_{store.Id}.csv";
                var excelFilePath =
                    _nopFileProvider.Combine(basePath, fileName);
                WriteToCsvFile(excelFilePath);
            }
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/FacebookShop/Configure";
        }

        public override async Task InstallAsync()
        {
            await this.InstallPluginAsync(new FacebookShopPermissionProvider());

            var task = await _scheduleTaskService.GetTaskByTypeAsync("NopStation.Plugin.Misc.FacebookShop.Services.FacebookShopItemTask");
            if (task == null)
            {
                await _scheduleTaskService.InsertTaskAsync(new ScheduleTask()
                {
                    Enabled = true,
                    Name = "Facebook shop upload items",
                    Seconds = 3600,
                    Type = "NopStation.Plugin.Misc.FacebookShop.Services.FacebookShopItemTask",
                    StopOnError = false
                });
            }

            await CreateCsvFiles();
            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await this.UninstallPluginAsync(new FacebookShopPermissionProvider());

            var task = await _scheduleTaskService.GetTaskByTypeAsync("NopStation.Plugin.Misc.FacebookShop.Services.FacebookShopItemTask");
            if (task != null)
            {
                await _scheduleTaskService.DeleteTaskAsync(task);
            }

            await base.UninstallAsync();
        }

        public override async Task UpdateAsync(string currentVersion, string targetVersion)
        {
            if (targetVersion == "4.50.1.6")
            {
                var keyValuePairs = PluginResouces();
                foreach (var keyValuePair in keyValuePairs)
                {
                    await _localizationService.AddOrUpdateLocaleResourceAsync(keyValuePair.Key, keyValuePair.Value);
                }
            }
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            var menu = new SiteMapNode()
            {
                Visible = true,
                IconClass = "far fa-dot-circle",
                Title = await _localizationService.GetResourceAsync("Admin.NopStation.FacebookShop.Menu.FacebookShop")
            };

            if (await _permissionService.AuthorizeAsync(FacebookShopPermissionProvider.ManageFacebookShop))
            {
                var configItem = new SiteMapNode()
                {
                    Title = await _localizationService.GetResourceAsync("Admin.NopStation.FacebookShop.Menu.Configuration"),
                    Url = $"{_webHelper.GetStoreLocation()}Admin/FacebookShop/Configure",
                    Visible = true,
                    IconClass = "far fa-circle",
                    SystemName = "FacebookShop.Configuration"
                };
                menu.ChildNodes.Add(configItem);
            }

            if (await _permissionService.AuthorizeAsync(CorePermissionProvider.ShowDocumentations))
            {
                var documentation = new SiteMapNode()
                {
                    Title = await _localizationService.GetResourceAsync("Admin.NopStation.Common.Menu.Documentation"),
                    Url = "https://www.nop-station.com/facebook-shop-documentation?utm_source=admin-panel&utm_medium=products&utm_campaign=facebook-shop",
                    Visible = true,
                    IconClass = "far fa-circle",
                    OpenUrlInNewTab = true
                };
                menu.ChildNodes.Add(documentation);
            }
            
            await _nopStationCoreService.ManageSiteMapAsync(rootNode, menu, NopStationMenuType.Plugin);
        }

        public List<KeyValuePair<string, string>> PluginResouces()
        {
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.Menu.FacebookShop", "Facebook shop"),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.Tab.Enable", "Facebook shop"),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.Fields.IncludeInFacebookShop", "Include in facebook shop"),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.Fields.IncludeInFacebookShop.Hint", "Check if the product is included in the facebook shop."),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.Fields.GenderTypeId", "Gender"),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.Fields.GenderTypeId.Hint", "Select gender type."),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.Fields.GoogleProductCategory", "Google product category"),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.Fields.GoogleProductCategory.Hint", "Select the google product category id."),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.Fields.GoogleProductCategory.Source", "See the google product categories id from here"),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.Fields.IsOverwriteBrandSelected", "Override brand name"),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.Fields.IsOverwriteBrandSelected.Hint", "Override manufacturer name (default as brand name) with given brand name."),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.Fields.Brand", "Brand"),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.Fields.Brand.Hint", "Override the brand name. The default is the name of the manufacturer."),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.Fields.AgeGroupType", "Age group"),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.Fields.AgeGroupType.Hint", "The age group that the item is targeted towards."),
                new KeyValuePair<string, string>("Admin.Nopstation.Facebookshop.Fields.ProductCondition", "Product condition"),
                new KeyValuePair<string, string>("Admin.Nopstation.Facebookshop.Fields.ProductCondition.Hint", "Select product condition."),
                new KeyValuePair<string, string>("Admin.Nopstation.Facebookshop.Fields.CustomImageUrl", "Custom image url"),
                new KeyValuePair<string, string>("Admin.Nopstation.Facebookshop.Fields.CustomImageUrl.Hint", "Set if want to provide custom image url."),

                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.Configuration.Fields.PrimaryLanguageId", "Primary language"),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.Configuration.Fields.PrimaryLanguageId.Hint", "Primary language for facebook shop."),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.Configuration.Fields.PrimaryCurrencyId", "Primary currency"),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.Configuration.Fields.PrimaryCurrencyId.Hint", "Primary currency for facebook shop."),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.Configuration", "Facebook shop settings"),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.Menu.Configuration", "Configuration"),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.ConfigurationDetails", "To use the bulk upload feature, you'll need to upload the data feed file in Facebook Commerce Manager. Follow the following steps, <br> <b> Facebook Commerce Manger > Choose your Catalog > Data sources > Data feed > Scheduled feed > Click Next > Enter File URL: (check below) </br> Click Next > Enter the Scheduled time to update your facebook catalog items > Click Next > Configure your currency > Click Upload </b>" ),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.ConfigurationDetails.FeedUrl","Your Facebook Shop Feed Url"),

                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.AddValueForSelectedProduct", "Insert the following inputs for selected products"),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.AddAll", "Include the products (All found)"),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.AddSelected", "Include the products (Selected)"),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.RemoveAll", "Exclude the product (All found)"),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.RemoveSelected", "Exclude the product (Selected)"),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.NoProductSelected", "No products selected"),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.AddedSuccessfully", "Products are added successfully to facebook shop"),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.RemovedSuccessfully", "Products are deleted successfully from facebook shop"),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.Configuration.Updated", "Facebookshop configuration updated successfully"),
                new KeyValuePair<string, string>("Admin.NopStation.FacebookShop.List", "Facebookshop Products"),
            };

            return list;
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string>
            {
                AdminWidgetZones.ProductDetailsBlock,
                AdminWidgetZones.ProductListButtons,
            });
        }

        public Type GetWidgetViewComponent(string widgetZone)
        {
            return widgetZone.Equals(AdminWidgetZones.ProductListButtons)
                ? typeof(ProductItemsBulkUploadViewComponent)
                : typeof(FacebookShopAdminViewComponent);
        }

        
    }
}