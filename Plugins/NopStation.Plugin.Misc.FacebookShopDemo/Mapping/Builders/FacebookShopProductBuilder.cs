using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using NopStation.Plugin.Misc.FacebookShopDemo.Domain;

namespace NopStation.Plugin.Misc.FacebookShopDemo.Mapping.Builders;
public class FacebookShopProductBuilder : NopEntityBuilder<FacebookShopProduct>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table.WithColumn(nameof(FacebookShopProduct.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(FacebookShopProduct.title)).AsString().NotNullable()
            .WithColumn(nameof(FacebookShopProduct.description)).AsString().NotNullable()
            .WithColumn(nameof(FacebookShopProduct.avaibility)).AsString().NotNullable()
            .WithColumn(nameof(FacebookShopProduct.condition)).AsString().NotNullable()
            .WithColumn(nameof(FacebookShopProduct.price)).AsString().NotNullable()
            .WithColumn(nameof(FacebookShopProduct.link)).AsString().NotNullable()
            .WithColumn(nameof(FacebookShopProduct.image_link)).AsString().NotNullable()
            .WithColumn(nameof(FacebookShopProduct.brand)).AsString().NotNullable()
            .WithColumn(nameof(FacebookShopProduct.quantity_to_sell_on_facebook)).AsInt32().Nullable()
            .WithColumn(nameof(FacebookShopProduct.size)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.origin_country)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.impoter_name)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.impoter_address)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.manufacturer_info)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.wa_compliance_category)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.sale_price)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.sale_price_effective_date)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.item_group_id)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.status)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.additional_image_link)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.google_product_category)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.fb_product_category)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.category_specific_fields)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.color)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.gender)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.age_group)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.material)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.pattern)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.rich_text_description)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.video_links)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.shipping)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.shipping_weight)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.custom_label_0)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.custom_label_1)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.custom_label_2)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.custom_label_3)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.custom_label_4)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.custom_label_5)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.custom_number_0)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.custom_number_1)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.custom_number_2)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.custom_number_3)).AsString().Nullable()
            .WithColumn(nameof(FacebookShopProduct.custom_number_4)).AsString().Nullable();
    }

}
