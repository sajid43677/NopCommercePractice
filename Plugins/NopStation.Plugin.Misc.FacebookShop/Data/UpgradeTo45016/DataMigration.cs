using System;
using System.Linq;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Data.Mapping;
using Nop.Data.Migrations;
using Nop.Data;
using NopStation.Plugin.Misc.FacebookShop.Domains;

namespace NopStation.Plugin.Misc.FacebookShop.Data.UpgradeTo45016
{
    [NopMigration("2023-02-01 00:00:00", "NopStation.Plugin.Misc.FacebookShop table update for 4.50.1.6", MigrationProcessType.Update)]
    public class DataMigration : Migration
    {
        private readonly INopDataProvider _dataProvider;

        public DataMigration(INopDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public override void Up()
        {
            var shopItemTableName = NameCompatibilityManager.GetTableName(typeof(ShopItem));

            //remove column
            var facebookProductCategoryColumnName = "FacebookProductCategory";

            if (Schema.Table(shopItemTableName).Column(facebookProductCategoryColumnName).Exists())
                Delete.Column(facebookProductCategoryColumnName).FromTable(shopItemTableName);

            //rename column
            var productConditionColumnName = "ProductCondition";
            var newProductConditionColumnName = nameof(ShopItem.ProductConditionTypeId);

            if (Schema.Table(shopItemTableName).Column(productConditionColumnName).Exists())
            {
                Rename.Column(productConditionColumnName).OnTable(shopItemTableName).To(newProductConditionColumnName);
            }

            //add permission
            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageNopStationFacebookShopProducts", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageFacebookShopProductsPermission = _dataProvider.InsertEntity(
                    new PermissionRecord
                    {
                        Name = "NopStation FacebookShop. Manage facebookshop products",
                        SystemName = "ManageNopStationFacebookShopProducts",
                        Category = "NopStation"
                    }
                );

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == NopCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = manageFacebookShopProductsPermission.Id
                    }
                );
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
