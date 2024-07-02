using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Widget.NopStationEmployees.Domain;
public class NopStationEmployee : BaseEntity
{
    public string Name { get; set; }
    public string Designation { get; set; }
    public int CountryId { get; set; }
    public Country Country
    {
        get => (Country)CountryId;
        set => CountryId = (int)value;
    }
}

public enum Country
{
    USA = 1,
    Canada = 2,
    UK = 3,
    Bangladesh = 4
}
