using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;
using NopStation.Plugin.Misc.Core.Controllers;
using NopStation.Plugin.Misc.FacebookShop.Services;

namespace NopStation.Plugin.Misc.FacebookShop.Controllers
{
    public class FacebookShopCatalogController : NopStationPublicController
    {
        #region Properties

        private readonly INopFileProvider _fileProvider;
        private readonly ILogger _logger;
        private readonly IFacebookShopIOManager _facebookShopIOManager;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public FacebookShopCatalogController(INopFileProvider fileProvider,
            ILogger logger,
            IFacebookShopIOManager facebookShopIOManager,
            IStoreContext storeContext)
        {
            _fileProvider = fileProvider;
            _logger = logger;
            _facebookShopIOManager = facebookShopIOManager;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> GetCatalogFeed()
        {
            try
            {
                var filepath = await _facebookShopIOManager.WriteOrUpdateShopItemToExcelAsync();

                return PhysicalFile(filepath, MimeTypes.ApplicationXml);
            }
            catch (Exception exc)
            {
                await _logger.ErrorAsync($"Failed to get feed: {exc.Message}", exc);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        #endregion
    }
}