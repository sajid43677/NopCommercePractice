using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.NopStationEmployees.Areas.Admin.Models;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.NopStationEmployees.Models;
public record NopStationEmployeeListPublicModel : BasePagedListModel<NopStationEmployeePublicModel>
{
}
