using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;

namespace Nop.Plugin.NopStation.Theme.MikesChainsaw;

public class MikesChainsawPermissionProvider : IPermissionProvider
{
    public static readonly PermissionRecord ManageMikesChainsaw = new PermissionRecord { Name = "MikesChainsaw theme. Manage NopStation MikesChainsaw theme", SystemName = "ManageNopStationMikesChainsaw", Category = "NopStation" };

    public virtual IEnumerable<PermissionRecord> GetPermissions()
    {
        return new[]
        {
            ManageMikesChainsaw
        };
    }

    HashSet<(string systemRoleName, PermissionRecord[] permissions)> IPermissionProvider.GetDefaultPermissions()
    {
        return new HashSet<(string, PermissionRecord[])>
        {
            (
                NopCustomerDefaults.AdministratorsRoleName,
                new[]
                {
                    ManageMikesChainsaw
                }
            )
        };
    }
}
