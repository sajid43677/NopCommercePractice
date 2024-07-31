using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using CsvHelper;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.IdentityModel.Tokens;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Mvc.Routing;
using NopStation.Plugin.Misc.FacebookShop.Areas.Admin.Factories;
using NopStation.Plugin.Misc.FacebookShop.Domains;
using NopStation.Plugin.Misc.FacebookShop.Factories;
using NopStation.Plugin.Misc.FacebookShop.Models;
using IProductModelFactory = Nop.Web.Areas.Admin.Factories.IProductModelFactory;

namespace NopStation.Plugin.Misc.FacebookShop.Services
{
    public partial class FacebookShopIOManager : IFacebookShopIOManager
    {
        #region Properties

        private readonly IFacebookShopService _facebookShopService;
        private readonly ILogger _logger;
        private readonly INopFileProvider _nopFileProvider;
        private readonly Factories.IFacebookShopModelFactory _facebookShopModelFactory;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;
        private readonly IManufacturerService _manufacturerService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IWorkContext _workContext;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IRepository<ProductAttributeCombination> _productAttributeCombinationRepository;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IShopItemModelFactory _shopItemModelFactory;
        

        private string _currencyCode = string.Empty;

        #endregion

        #region Ctor
        public FacebookShopIOManager(IFacebookShopService facebookShopService,
            ILogger logger,
            INopFileProvider nopFileProvider,
            Factories.IFacebookShopModelFactory facebookShopModelFactory,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor,
            IStoreService storeService,
            IStoreContext storeContext,
            IManufacturerService manufacturerService,
            IShoppingCartService shoppingCartService,
            ITaxService taxService,
            ICurrencyService currencyService,
            IWorkContext workContext,
            IProductModelFactory productModelFactory,
            IRepository<ProductAttributeCombination> productAttributeCombinationRepository,
            IPictureService pictureService,
            IProductAttributeParser productAttributeParser,
            IProductService productService,
            IPriceCalculationService priceCalculationService,
            IShopItemModelFactory shopItemModelFactory)
        {
            _facebookShopService = facebookShopService;
            _logger = logger;
            _nopFileProvider = nopFileProvider;
            _facebookShopModelFactory = facebookShopModelFactory;
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccessor = actionContextAccessor;
            _storeService = storeService;
            _storeContext = storeContext;
            _manufacturerService = manufacturerService;
            _shoppingCartService = shoppingCartService;
            _taxService = taxService;
            _currencyService = currencyService;
            _workContext = workContext;
            _productModelFactory = productModelFactory;
            _productAttributeCombinationRepository = productAttributeCombinationRepository;
            _pictureService = pictureService;
            _productAttributeParser = productAttributeParser;
            _productService = productService;
            _priceCalculationService = priceCalculationService;
            _shopItemModelFactory = shopItemModelFactory;
        }

        #endregion

        #region Utilities

        protected void WriteToCsvFile(string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath, false, new UTF8Encoding(true)))
            using (CsvWriter cw = new CsvWriter(sw, CultureInfo.InvariantCulture))
            {
                cw.WriteHeader<ItemModel>();
            }
        }

        #endregion

        #region Methods

        protected virtual async Task<string> RouteUrlAsync(int storeId = 0, object routeValues = null)
        {
            //try to get a store by the passed identifier
            var store = await _storeService.GetStoreByIdAsync(storeId) ?? await _storeContext.GetCurrentStoreAsync()
                ?? throw new Exception("No store could be loaded");

            //generate a URL with an absolute path
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var url = new PathString(urlHelper.RouteUrl<Product>(routeValues));

            //remove the application path from the generated URL if exists
            var pathBase = _actionContextAccessor.ActionContext?.HttpContext?.Request?.PathBase ?? PathString.Empty;
            url.StartsWithSegments(pathBase, out url);

            //compose the result
            return Uri.EscapeUriString(WebUtility.UrlDecode($"{store.Url.TrimEnd('/')}{url}"));
        }

        public async Task<string> WriteOrUpdateShopItemToExcelAsync()
        {
            var itemModels = new List<ItemModel>();

            var shopItems = await _shopItemModelFactory.PrepareShopItemAssociateWithProducts();
            if (shopItems != null && shopItems.Count != 0)
            {
                foreach (var item in shopItems)
                {
                    if (!item.ShopItem.IncludeInFacebookShop)
                        continue;
                    var model = await _facebookShopModelFactory.PrepareFacebookShopProductModelAsync(item.Product, item.ShopItem);

                    var aShopItem = new ItemModel();
                    await PrepareItemModelAsync(aShopItem, model, item);

                    var attributesWithValues = await GetItemModelsProductAttributeCombinationsWithValuesAsync(item.Product, aShopItem, model);
                    if (attributesWithValues.Count == 0)
                    {
                        itemModels.Add(aShopItem);
                    }
                    else
                    {
                        itemModels.AddRange(attributesWithValues);
                    }
                }
            }

            //Convert model to csv text
            //var csvText = ConvertToCsv(itemModels);

            //convert to xml
            var store = _storeContext.GetCurrentStore();
            var rss = convertToXml(itemModels, store);

            var filePath = _nopFileProvider.MapPath("~/Plugins/NopStation.Plugin.Misc.FacebookShop/Files/GetCatalogFeed.xml");

            // Save the RSS feed to a file
            rss.Save(filePath);

            return filePath;
        }

        private static XDocument convertToXml(List<ItemModel> itemModels, Store store)
        {
            return new XDocument(
                new XElement("rss",
                    new XAttribute("version", "2.0"),
                    new XElement("channel",
                        new XElement("title", "Shop Items"),
                        new XElement("link", store.Url),
                        new XElement("description", "List of Facebook shop items"),
                        new XElement("language", "en-us"),
                        from item in itemModels
                        select new XElement("item",
                            new XElement("title", item.Title),
                            new XElement("link", item.Link),
                            new XElement("description", item.Description ?? "No description available"),
                            new XElement("guid", item.Id),
                            new XElement("pubDate", DateTime.UtcNow.ToString("R")),
                            new XElement("id", item.Id),
                            new XElement("availability", item.Availability),
                            new XElement("condition", item.Condition),
                            new XElement("price", item.Price),
                            new XElement("image_link", item.Image_Link),
                            new XElement("brand", item.Brand),
                            new XElement("rich_text_description", item.Rich_Text_Description),
                            new XElement("sale_price", item.Sale_Price),
                            new XElement("sale_price_effective_date", item.Sale_Price_Effective_Date),
                            new XElement("item_group_id", item.Item_Group_Id),
                            new XElement("additional_image_link", item.Additional_Image_Link),
                            new XElement("color", item.Color),
                            new XElement("gender", item.Gender),
                            new XElement("size", item.Size),
                            new XElement("age_group", item.Age_Group),
                            new XElement("material", item.Material),
                            new XElement("pattern", item.Pattern),
                            new XElement("shipping", item.Shipping),
                            new XElement("shipping_weight", item.Shipping_Weight),
                            new XElement("delete", item.Delete),
                            new XElement("quantity_to_sell_on_facebook", item.Quantity_To_Sell_On_Facebook),
                            new XElement("fb_product_category", item.Fb_Product_Category),
                            new XElement("google_product_category", item.Google_Product_Category)
                        )
                    )
                )
            );
        }

        private string ConvertToCsv(List<ItemModel> itemModels)
        {
            using var writer = new StringWriter();
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(itemModels);
            return writer.ToString();
        }

        #endregion

        #region Utilities

        private async Task PrepareItemModelAsync(ItemModel itemModel, FacebookShopProductModel model, ShopItemAssociateWithProduct item)
        {
            _currencyCode = model.ProductPrice.CurrencyCode;

            var currentStoreId = (await _storeContext.GetCurrentStoreAsync()).Id;

            itemModel.Id = Convert.ToString(model.ProductId);
            itemModel.Title = model.Name;

            itemModel.Description = string.IsNullOrEmpty(model.ShortDescription)
                ? model.FullDescription
                : model.ShortDescription;

            if (!string.IsNullOrEmpty(model.FullDescription))
            {
                itemModel.Rich_Text_Description = Regex.Replace(model.FullDescription, @"[^\u0000-\u007F]+", string.Empty);
            }
            itemModel.Link = await RouteUrlAsync(currentStoreId, new { SeName = model.SeName });

            itemModel.Price = await FormatPriceForFacebookAsync(string.IsNullOrEmpty(model.ProductPrice.OldPrice)
                ? model.ProductPrice.Price
                : model.ProductPrice.OldPrice);
            itemModel.Sale_Price = await FormatPriceForFacebookAsync(string.IsNullOrEmpty(model.ProductPrice.PriceWithDiscount)
                ? model.ProductPrice.Price
                : model.ProductPrice.PriceWithDiscount);


            itemModel.Image_Link = model.DefaultPictureModel.FullSizeImageUrl;

            itemModel.Additional_Image_Link = "";
            var additional_Image_Links = (model.PictureModels.Skip(1).Select(p => p.FullSizeImageUrl));
            if(!additional_Image_Links.IsNullOrEmpty()) itemModel.Additional_Image_Link = string.Join(", ", additional_Image_Links);

            //itemModel.Delete = Convert.ToString(!item.ShopItem.IncludeInFacebookShop).ToLower();
            if (item.Product.Published && !item.Product.Deleted && item.ShopItem.IncludeInFacebookShop)
            {
                itemModel.Delete = "false";
            }
            else
            {
                itemModel.Delete = "true";
            }
            itemModel.Brand = string.IsNullOrEmpty(model.Brand) ? await GetProductManufacturerNameAsync(item.Product.Id) : model.Brand;
            if (!string.IsNullOrEmpty(model.StockAvailability))
            {
                itemModel.Availability = Regex.Replace(model.StockAvailability, @"[\d-]", string.Empty);
            }
            itemModel.Condition = model.Condition;
            itemModel.Google_Product_Category = model.GoogleCategory;
            itemModel.Gender = Convert.ToString(item.ShopItem.GenderType);
            itemModel.Item_Group_Id = Convert.ToString(model.ProductId);


            if (string.IsNullOrEmpty(itemModel.Availability))
            {
                itemModel.Availability = "In Stock"; // default value 
            }

            itemModel.Age_Group = item.ShopItem.AgeGroupType;
        }

        private async Task<List<ItemModel>> GetItemModelsProductAttributeCombinationsWithValuesAsync(Product product, ItemModel itemModel, FacebookShopProductModel rootProductModel)
        {
            #region Getting all Attribute Combition Name with thier values

            var prepareProductAttributeCombinationListModel = await _productModelFactory
                .PrepareProductAttributeCombinationListModelAsync(new ProductAttributeCombinationSearchModel { ProductId = product.Id, Length = int.MaxValue }, product);//TODO: Get only the facebook attribute combination
            var itemModels = new List<ItemModel>();

            #endregion

            if (prepareProductAttributeCombinationListModel.Data?.Any() ?? false)
            {
                foreach (var item in prepareProductAttributeCombinationListModel.Data)
                {
                    var attributeCombination = await _productAttributeCombinationRepository.GetByIdAsync(item.Id);
                    var itemModelCopy = new ItemModel
                    {
                        Id = $"{Convert.ToString(product.Id)}_{attributeCombination.Id}",
                        Title = itemModel.Title,
                        Description = itemModel.Description,
                        Rich_Text_Description = !string.IsNullOrEmpty(itemModel.Rich_Text_Description) ?
                            Regex.Replace(itemModel.Rich_Text_Description, @"[^\u0000-\u007F]+", string.Empty)
                            : "",
                        Link = itemModel.Link,
                        Price = itemModel.Price,
                        Sale_Price = await FormatPriceForFacebookAsync(await GetProductAttributeCombinationSalePriceAsync(product, attributeCombination.AttributesXml)),
                        Image_Link = await GetProductAttributePictureUrlAsync(attributeCombination.AttributesXml, item.PictureIds[0]) ?? itemModel.Image_Link,
                        Delete = itemModel.Delete,
                        Brand = itemModel.Brand,
                        Availability = Regex.Replace(await _productService.FormatStockMessageAsync(product, attributeCombination.AttributesXml), @"[\d-]", string.Empty),
                        Condition = itemModel.Condition,
                        Google_Product_Category = itemModel.Google_Product_Category,
                        Gender = itemModel.Gender,
                        Item_Group_Id = Convert.ToString(item.ProductId),
                        Age_Group = itemModel.Age_Group,
                        Additional_Image_Link = await GetProductAttributePicturesUrlAsync(item.PictureIds) ?? "",

                    };

                    if (string.IsNullOrEmpty(itemModelCopy.Availability))
                    {
                        itemModelCopy.Availability = "In Stock"; // default value 
                    }

                    var salePriceDecimal = decimal.TryParse(!string.IsNullOrEmpty(itemModelCopy.Sale_Price)
                        ? Regex.Replace(itemModelCopy.Sale_Price, @"[^0-9.]+", "")
                        : "", out var decimalPrice) ? decimalPrice : 0;
                    var priceDecimal = decimal.TryParse(Regex.Replace(itemModelCopy.Price, @"[^0-9.]+", ""), out var decimalValue) ? decimalValue : 0;
                    if (salePriceDecimal > priceDecimal)
                    {
                        itemModelCopy.Price = await FormatPriceForFacebookAsync(Convert.ToString(salePriceDecimal));
                        itemModelCopy.Sale_Price = "";
                    }

                    var attributes = item.AttributesXml.Split("<br />");
                    foreach (var attribute in attributes)
                    {
                        var nameAndValue = attribute.Split(":");
                        if (!string.IsNullOrEmpty(nameAndValue.Last()))
                        {
                            if (nameAndValue.First().Equals("Color", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var theValue = Regex.Replace(nameAndValue.Last(), "\\[[^\\]]*\\]", "");
                                theValue = HttpUtility.HtmlDecode(theValue);
                                theValue = Regex.Replace(theValue, @"[^0-9a-zA-Z]+", "");

                                itemModelCopy.Color = theValue;
                                itemModels.Add(itemModelCopy);
                            }
                            if (nameAndValue.First().Equals("Material", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var theValue = Regex.Replace(nameAndValue.Last(), "\\[[^\\]]*\\]", "");
                                theValue = HttpUtility.HtmlDecode(theValue);
                                theValue = Regex.Replace(theValue, @"[^0-9a-zA-Z]+", "");

                                var index = itemModels.FindIndex(c =>
                                    c.Id == item.Id.ToString()); // whether the item model added previously
                                if (index != -1)
                                {
                                    itemModels[index].Material = theValue; // update the item model
                                }
                                else
                                {
                                    itemModelCopy.Material = theValue;
                                    itemModels.Add(itemModelCopy);
                                }
                            }
                            if (nameAndValue.First().Equals("Size", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var theValue = Regex.Replace(nameAndValue.Last(), "\\[[^\\]]*\\]", "");
                                theValue = HttpUtility.HtmlDecode(theValue);
                                theValue = Regex.Replace(theValue, @"[^0-9a-zA-Z]+", "");

                                var index = itemModels.FindIndex(c =>
                                    c.Id == item.Id.ToString()); // whether the item model added previously
                                if (index != -1)
                                {
                                    itemModels[index].Size = theValue; // update the item model
                                }
                                else
                                {
                                    itemModelCopy.Size = theValue;
                                    itemModels.Add(itemModelCopy);
                                }
                            }
                        }
                    }
                }
            }

            itemModels = Enumerable.DistinctBy(itemModels, x => x.Id).ToList();
            itemModels = itemModels.GroupBy(m => new { m.Sale_Price, m.Color, m.Size })
                .Select(group => group.Last())  // instead of First you can also apply your logic here what you want to take, for example an OrderBy
                .ToList();
            return itemModels;
        }

        private async Task<string> GetProductManufacturerNameAsync(int productId)
        {
            var productManufacturer = (await _manufacturerService.GetProductManufacturersByProductIdAsync(productId))
                .FirstOrDefault();
            if (productManufacturer != null)
            {
                var manufacture =
                    await _manufacturerService.GetManufacturerByIdAsync(productManufacturer.ManufacturerId);
                if (manufacture != null)
                {
                    return manufacture.Name;
                }
            }

            return null;
        }

        private async Task<string> GetProductAttributeCombinationSalePriceAsync(Product product, string attributeXml)
        {
            //we do not calculate price of "customer enters price" option is enabled
            (decimal finalPrice,decimal  _, List < Discount > _) = await _shoppingCartService.GetUnitPriceAsync(product,
                await _workContext.GetCurrentCustomerAsync(),
                await _storeContext.GetCurrentStoreAsync(),
                ShoppingCartType.ShoppingCart,
                1, attributeXml, 0,
                null, null, true);
            var (finalPriceWithDiscountBase, _) = await _taxService.GetProductPriceAsync(product, finalPrice);
            var finalPriceWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithDiscountBase, await _workContext.GetWorkingCurrencyAsync());
            // discounted price
            return finalPriceWithDiscount != product.Price ?
                Convert.ToString(finalPriceWithDiscount, CultureInfo.InvariantCulture) :
                Convert.ToString(product.Price, CultureInfo.InvariantCulture);
            ;
        }

        private async Task<string> FormatPriceForFacebookAsync(string price)
        {
            if (string.IsNullOrEmpty(price))
                return string.Empty;

            //if (price.Contains("$"))
            //{
            //    price = price?.Replace("$", "");
            //}
            var converted = decimal.TryParse(price, out var decimalPrice);
            return converted ? $"{await _priceCalculationService.RoundPriceAsync(decimalPrice)} {_currencyCode}" : "";
        }

        private async Task<string> GetProductAttributePictureUrlAsync(string attributeXml, int combinationsPictureId) //TODO: Start from here
        {
            string pictureUrl;

            #region CombinationPrictureId

            if (combinationsPictureId != 0)
            {
                pictureUrl = await GetPictureUrlByAttributeCombinationPictureIdAsync(combinationsPictureId);
                if (!string.IsNullOrEmpty(pictureUrl))
                    return pictureUrl;
            }

            #endregion

            #region Attributexml

            pictureUrl = await GetPictureUrlByAttributeXmlAsync(attributeXml);
            if (!string.IsNullOrEmpty(pictureUrl))
                return pictureUrl;

            #endregion

            return null;
        }

        private async Task<string> GetProductAttributePicturesUrlAsync(IList<int> pictureIds)
        {
            if (pictureIds == null || pictureIds.Count == 0)
                return null;

            var pictureUrls = await Task.WhenAll(pictureIds.Skip(1).Select(GetPictureUrlByAttributeCombinationPictureIdAsync));

            var additionalPictureUrl = string.Join(", ", pictureUrls);
            if (!string.IsNullOrEmpty(additionalPictureUrl))
                return additionalPictureUrl;

            return null;
        }

        private async Task<string> GetPictureUrlByAttributeXmlAsync(string attributeXml)
        {
            //then, let's see whether we have attribute values with pictures
            var attributePicture = await (await _productAttributeParser.ParseProductAttributeValuesAsync(attributeXml))
                .SelectAwait(async attributeValue => await _pictureService.GetPictureByIdAsync(attributeValue?.PictureId ?? 0))
                .FirstOrDefaultAsync(picture => picture != null);

            if (attributePicture == null)
                return null;

            var (url, picture) = await _pictureService.GetPictureUrlAsync(attributePicture, showDefaultPicture: false);
            return url;

        }

        private async Task<string> GetPictureUrlByAttributeCombinationPictureIdAsync(int pictureId)
        {
            var picture = await _pictureService.GetPictureByIdAsync(pictureId);

            var (url, _) = await _pictureService.GetPictureUrlAsync(picture, showDefaultPicture: false);
            return url;
        }

        #endregion
    }
}
