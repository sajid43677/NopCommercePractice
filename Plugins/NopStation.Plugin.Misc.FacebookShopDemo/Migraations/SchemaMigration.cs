using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Data.Extensions;
using NopStation.Plugin.Misc.FacebookShopDemo.Domain;

namespace NopStation.Plugin.Misc.FacebookShopDemo.Migraations;
[NopSchemaMigration("2024/07/09 01:00:55:1687541", "NopstationTeam.FacebookShopProduct base schema", MigrationProcessType.Installation)]
public class SchemaMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        Create.TableFor<FacebookShopProduct>();
    }
}
