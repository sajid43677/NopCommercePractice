using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Plugin.Widget.NopStationEmployees.Domain;

namespace Nop.Plugin.Widget.NopStationEmployees.Services;
public interface INopStationEmployeeService
{
    Task AddEmployeeAsync(NopStationEmployee employee);
    Task UpdateEmployeeAsync(NopStationEmployee employee);
    Task DeleteEmployeeAsync(NopStationEmployee employee);
    Task<NopStationEmployee> GetEmployeeByIdAsync(int employeeId);
    Task<List<NopStationEmployee>> GetAllEmployeesAsync();

    Task<IPagedList<NopStationEmployee>> SearchNopStationEmployeesAsync(string name, int countryId, int pageIndex = 0, int pageSize = int.MaxValue);
}
