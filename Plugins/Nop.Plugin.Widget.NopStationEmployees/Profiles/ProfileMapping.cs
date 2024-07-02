using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using Nop.Plugin.Widget.NopStationEmployees.Domain;
using Nop.Plugin.Widgets.NopStationEmployees.Models;

namespace Nop.Plugin.Widgets.NopStationEmployees.Profiles;
public class ProfileMapping : Profile
{
    public ProfileMapping()
    {
        CreateMap<NopStationEmployee, NopStationEmployeePublicModel>();
    }
}
