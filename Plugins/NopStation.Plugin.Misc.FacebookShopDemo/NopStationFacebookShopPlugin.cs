using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Plugins;

namespace NopStation.Plugin.Misc.FacebookShopDemo
{
    public class NopStationFacebookShopPlugin : BasePlugin, IMiscPlugin
    {
        private readonly IWebHelper _webHelper;

        public NopStationFacebookShopPlugin(IWebHelper webHelper)
        {
            _webHelper = webHelper;
        }

        /*public bool HideInWidgetList => false;

        public Type GetWidgetViewComponent(string widgetZone)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            throw new NotImplementedException();
        }*/

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/FacebookShopAdmin/configure";
        }

        public override async Task InstallAsync()
        {
            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }
    }
}
