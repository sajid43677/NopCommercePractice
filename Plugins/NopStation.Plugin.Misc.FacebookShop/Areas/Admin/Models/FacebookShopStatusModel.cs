using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace NopStation.Plugin.Misc.FacebookShop.Areas.Admin.Models;
public record FacebookShopStatusModel : ProductModel
{
    public bool IncludeInFacebookShop { get; set; }
}
