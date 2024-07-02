using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Widget.NopStationEmployees.Domain;

namespace Nop.Plugin.Widget.NopStationEmployees.Migrations;

[NopSchemaMigration("2024/07/01 06:20:55:1687541", "NopstationTeam.Employee base schema", MigrationProcessType.Installation)]
public class SchemaMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        Create.TableFor<NopStationEmployee>();
    }
}
