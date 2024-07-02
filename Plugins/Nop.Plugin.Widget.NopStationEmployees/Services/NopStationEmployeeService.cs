using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.Widget.NopStationEmployees.Domain;

namespace Nop.Plugin.Widget.NopStationEmployees.Services;
public class NopStationEmployeeService : INopStationEmployeeService
{
    private readonly IRepository<NopStationEmployee> _nopStationEmployeeRepository;

    public NopStationEmployeeService(IRepository<NopStationEmployee> nopStationEmployeeRepository)
    {
        _nopStationEmployeeRepository = nopStationEmployeeRepository;
    }

    public async Task AddEmployeeAsync(NopStationEmployee employee)
    {
        await _nopStationEmployeeRepository.InsertAsync(employee);
    }

    public async Task DeleteEmployeeAsync(NopStationEmployee employee)
    {
        await _nopStationEmployeeRepository.DeleteAsync(employee);
    }


    public virtual async Task<NopStationEmployee> GetEmployeeByIdAsync(int employeeId)
    {
        return await _nopStationEmployeeRepository.GetByIdAsync(employeeId);
    }

    public async Task UpdateEmployeeAsync(NopStationEmployee employee)
    {
        await _nopStationEmployeeRepository.UpdateAsync(employee);
    }

    public async Task<IPagedList<NopStationEmployee>> SearchNopStationEmployeesAsync(string name, int countryId, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var query = _nopStationEmployeeRepository.Table;

        if (!string.IsNullOrEmpty(name))
            query = query.Where(x => x.Name.Contains(name));

        if (countryId > 0)
            query = query.Where(x => x.CountryId == countryId);

        query = query.OrderBy(x => x.Name);

        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    public Task<List<NopStationEmployee>> GetAllEmployeesAsync()
    {
        return _nopStationEmployeeRepository.Table.ToListAsync();
    }
}
