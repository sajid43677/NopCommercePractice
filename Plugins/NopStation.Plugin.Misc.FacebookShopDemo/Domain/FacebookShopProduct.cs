using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace NopStation.Plugin.Misc.FacebookShopDemo.Domain;
public class FacebookShopProduct : BaseEntity
{
    public string title { get; set; }
    public string description { get; set; }
    public string avaibility { get; set; }
    public string condition { get; set; }
    public string price { get; set; }
    public string link { get; set; }
    public string image_link { get; set; }
    public string brand { get; set; }
    public int quantity_to_sell_on_facebook { get; set; }
    public string size { get; set; }
    public string origin_country { get; set; }
    public string impoter_name { get; set; }
    public string impoter_address { get; set; }
    public string manufacturer_info { get; set; }
    public string wa_compliance_category { get; set; }
    public string sale_price { get; set; }
    public string sale_price_effective_date { get; set; }
    public string item_group_id { get; set; }
    public string status { get; set; }
    public string additional_image_link { get; set; }
    public string google_product_category { get; set; }
    public string fb_product_category { get; set; }
    public string category_specific_fields { get; set; }
    public string color { get; set; }
    public string gender { get; set; }
    public string age_group { get; set; }
    public string material { get; set; }
    public string pattern { get; set; }
    public string rich_text_description { get; set; }
    public string video_links { get; set; }
    public string shipping { get; set; }
    public string shipping_weight { get; set; }
    public string custom_label_0 { get; set; }
    public string custom_label_1 { get; set; }
    public string custom_label_2 { get; set; }
    public string custom_label_3 { get; set; }
    public string custom_label_4 { get; set; }
    public string custom_label_5 { get; set; }
    public string custom_number_0 { get; set; }
    public string custom_number_1 { get; set; }
    public string custom_number_2 { get; set; }
    public string custom_number_3 { get; set; }
    public string custom_number_4 { get; set; }
}
