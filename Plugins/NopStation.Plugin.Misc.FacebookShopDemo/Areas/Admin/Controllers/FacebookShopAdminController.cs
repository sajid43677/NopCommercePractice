using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace NopStation.Plugin.Misc.FacebookShopDemo.Areas.Admin.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
public class FacebookShopAdminController : BasePluginController
{
    public async Task<IActionResult> Configure()
    {
        return View();
    }
}
