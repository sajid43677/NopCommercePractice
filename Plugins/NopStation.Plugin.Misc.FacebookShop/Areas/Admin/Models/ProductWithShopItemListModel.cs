using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models;

namespace NopStation.Plugin.Misc.FacebookShop.Areas.Admin.Models;
public record ProductWithShopItemListModel : BasePagedListModel<FacebookShopStatusModel>
{
}
