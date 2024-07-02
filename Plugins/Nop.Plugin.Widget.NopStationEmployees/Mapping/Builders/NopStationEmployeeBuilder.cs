using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widget.NopStationEmployees.Domain;

namespace Nop.Plugin.Widget.NopStationEmployees.Mapping.Builders;
public class NopStationEmployeeBuilder : NopEntityBuilder<NopStationEmployee>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table.WithColumn(nameof(NopStationEmployee.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(NopStationEmployee.Name)).AsString().NotNullable()
                .WithColumn(nameof(NopStationEmployee.Designation)).AsString().NotNullable()
                .WithColumn(nameof(NopStationEmployee.CountryId)).AsInt32().NotNullable();
    }
}
