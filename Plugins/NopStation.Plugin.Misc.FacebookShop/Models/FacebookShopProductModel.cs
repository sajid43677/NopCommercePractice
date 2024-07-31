using System.Collections.Generic;
using Nop.Web.Models.Media;

namespace NopStation.Plugin.Misc.FacebookShop.Models
{
    public partial class FacebookShopProductModel
    {
        public FacebookShopProductModel()
        {
            DefaultPictureModel = new PictureModel();
            ProductPrice = new ProductPriceModel();
        }

        // pictures
        public PictureModel DefaultPictureModel { get; set; }
        //additional images
        public IList<PictureModel> PictureModels { get; set; }

        public int ProductId { get; set; }

        public string Name { get; set; }

        public string ShortDescription { get; set; }

        public string FullDescription { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public string MetaTitle { get; set; }

        public string SeName { get; set; }

        public string Brand { get; set; }

        public string GoogleCategory { get; set; }

        public string Sku { get; set; }

        public string StockAvailability { get; set; }

        public string Condition { get; set; }

        public ProductPriceModel ProductPrice { get; set; }

        public partial record ProductPriceModel
        {
            public string CurrencyCode { get; set; }

            public string OldPrice { get; set; }

            public string Price { get; set; }

            public string PriceWithDiscount { get; set; }

            public decimal PriceValue { get; set; }

            public bool CustomerEntersPrice { get; set; }

            public bool CallForPrice { get; set; }

            public int ProductId { get; set; }

            public bool HidePrices { get; set; }

            public bool IsRental { get; set; }

            public string RentalPrice { get; set; }

            public bool DisplayTaxShippingInfo { get; set; }

            public string BasePricePAngV { get; set; }
        }
    }
}
