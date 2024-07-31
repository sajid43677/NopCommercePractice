namespace NopStation.Plugin.Misc.FacebookShop
{
    public static class FacebookShopDefaults
    {
        /// <summary>
        /// Gets a plugin system name
        /// </summary>
        public static string SystemName => "NopStation.Plugin.Misc.FacebookShop";

        /// <summary>
        /// Gets a plugin system name
        /// </summary>
        public static string DefaultFileName => "catalog_products.csv";

        /// <summary>
        /// Gets a plugin system name
        /// </summary>
        public static string FileName => "catalog_products_store";

        /// <summary>
        /// Gets the name of the view component to Facebook Shop Admin
        /// </summary>
        public const string FACEBOOK_SHOP_ADMIN_VIEW_COMPONENT_NAME = "FacebookShopAdmin";

        /// <summary>
        /// Gets the name of the view component to Product Items Bulk Upload
        /// </summary>
        public const string BULK_UPLOAD_SHOP_ITEMS_VIEW_COMPONENT_NAME = "ProductItemsBulkUpload";
    }
}
