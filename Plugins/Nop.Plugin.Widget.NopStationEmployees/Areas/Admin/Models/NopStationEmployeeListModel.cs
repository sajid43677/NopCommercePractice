using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.NopStationEmployees.Areas.Admin.Models;
public record NopStationEmployeeListModel : BasePagedListModel<NopStationEmployeeModel>
{
}
