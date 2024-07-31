using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Core;
using Nop.Services;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using NopStation.Plugin.Misc.FacebookShop.Areas.Admin.Models;
using NopStation.Plugin.Misc.FacebookShop.Domains;
using NopStation.Plugin.Misc.FacebookShop.Services;
using Nop.Services.Catalog;
using Nop.Services.Media;
using Nop.Services.Seo;
using DocumentFormat.OpenXml.Spreadsheet;
using AutoMapper;
using NopStation.Plugin.Misc.FacebookShop.Models;
using Nop.Data;

namespace NopStation.Plugin.Misc.FacebookShop.Areas.Admin.Factories
{
    public partial class ShopItemModelFactory : IShopItemModelFactory
    {
        #region Properties

        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IFacebookShopService _facebookShopService;
        private readonly IWorkContext _workContext;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ShopItem> _shopItemRepository;

        #endregion

        #region Ctor

        public ShopItemModelFactory(ILocalizationService localisationService,
            ILocalizedModelFactory localizedModelFactory,
            IFacebookShopService facebookShopService,
            ICategoryService categoryService,
            IProductService productService,
            IWorkContext workContext,
            IMapper mapper,
            IRepository<Product> productRepository,
            IRepository<ShopItem> shopItemRepository)
        {
            _localizationService = localisationService;
            _localizedModelFactory = localizedModelFactory;
            _facebookShopService = facebookShopService;
            _categoryService = categoryService;
            _productService = productService;
            _workContext = workContext;
            _mapper = mapper;
            _productRepository = productRepository;
            _shopItemRepository = shopItemRepository;
        }

        #endregion

        #region Methods

        protected async Task PrepareGenderTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var availableDataSourceTypes = (await GenderType.Male.ToSelectListAsync(false)).ToList();
            foreach (var source in availableDataSourceTypes)
            {
                items.Add(source);
            }

            if (withSpecialDefaultItem)
                items.Insert(0, new SelectListItem()
                {
                    Text = await _localizationService.GetResourceAsync("Admin.Common.All"),
                    Value = "0"
                });
        }

        protected async Task PrepareProductConditionTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var availableDataSourceTypes = (await ProductConditionType.New.ToSelectListAsync(false)).ToList();
            foreach (var source in availableDataSourceTypes)
            {
                items.Add(source);
            }

            if (withSpecialDefaultItem)
                items.Insert(0, new SelectListItem()
                {
                    Text = await _localizationService.GetResourceAsync("Admin.Common.All"),
                    Value = "0"
                });
        }

        protected List<SelectListItem> AvailableAgeGroupsItems()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Selected = false, Text = "All Ages", Value = "all ages"},
                new SelectListItem { Selected = false, Text = "Adult", Value = "adult"},
                new SelectListItem { Selected = false, Text = "Teen", Value = "teen"},
                new SelectListItem { Selected = false, Text = "Kids", Value = "kids"},
                new SelectListItem { Selected = false, Text = "Toddler", Value = "toddler"},
                new SelectListItem { Selected = false, Text = "Infant", Value = "infant"},
                new SelectListItem { Selected = false, Text = "New Born", Value = "newborn"},
            };
        }

        public async Task<ShopItemModel> PrepareShopItemModelAsync(ShopItemModel model, ShopItem shopItem, ProductModel productModel, bool excludeProperties = false)
        {
            Func<ShopItemLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (shopItem != null)
            {
                if (model == null)
                {
                    model = shopItem.ToModel<ShopItemModel>();
                    model.ProductId = productModel.Id;
                }
                if (!excludeProperties)
                {
                    localizedModelConfiguration = async (locale, languageId) =>
                    {
                        locale.Brand = await _localizationService.GetLocalizedAsync(shopItem, entity => entity.Brand, languageId, false, false);
                        if (!string.IsNullOrEmpty(locale.Brand))
                            model.IsOverwriteBrandSelected = true;
                    };
                }
            }

            if (!excludeProperties)
            {
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);
                await PrepareGenderTypesAsync(model.AvailableGenders, false);
                await PrepareProductConditionTypesAsync(model.AvailableProductConditions, false);
            }

            if (!string.IsNullOrEmpty(model.Brand))
                model.IsOverwriteBrandSelected = true;
            model.AvailableAgeGroups = AvailableAgeGroupsItems();

            return model;
        }

        public virtual async Task<ProductWithShopItemListModel> PrepareProductWithShopItemModel(ProductSearchModel searchModel)
        {
            ArgumentNullException.ThrowIfNull(searchModel);

            //get parameters to filter comments
            var overridePublished = searchModel.SearchPublishedId == 0 ? null : (bool?)(searchModel.SearchPublishedId == 1);
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
                searchModel.SearchVendorId = currentVendor.Id;
            var categoryIds = new List<int> { searchModel.SearchCategoryId };
            if (searchModel.SearchIncludeSubCategories && searchModel.SearchCategoryId > 0)
            {
                var childCategoryIds = await _categoryService.GetChildCategoryIdsAsync(parentCategoryId: searchModel.SearchCategoryId, showHidden: true);
                categoryIds.AddRange(childCategoryIds);
            }

            //get products
            var products = await _productService.SearchProductsAsync(showHidden: true,
                categoryIds: categoryIds,
                manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                warehouseId: searchModel.SearchWarehouseId,
                productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
                keywords: searchModel.SearchProductName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize,
                overridePublished: overridePublished);

            var facebookShopStatusModels = new List<FacebookShopStatusModel>();
            foreach (var product in products)
            {
                var shopItem = await _facebookShopService.GetShopItemByProductIdAsync(product.Id);
                var tempmodel = product.ToModel<ProductModel>();
                var productmodel = _mapper.Map<FacebookShopStatusModel>(tempmodel);
                if (shopItem != null)
                {
                    productmodel.IncludeInFacebookShop = shopItem.IncludeInFacebookShop;
                }
                facebookShopStatusModels.Add(productmodel);
            }

            var facebookShopStatusModelsIlist = new PagedList<FacebookShopStatusModel>(facebookShopStatusModels, searchModel.Page - 1, searchModel.PageSize, products.TotalCount);

            //prepare list model
            var model = await new ProductWithShopItemListModel().PrepareToGridAsync(searchModel, facebookShopStatusModelsIlist, () =>
            {
                return facebookShopStatusModelsIlist.SelectAwait(async product =>
                {
                    return product;
                });
            });

            return model;
        }

        public async Task<List<ShopItemAssociateWithProduct>> PrepareShopItemAssociateWithProducts()
        {
            var itemModels = from p in _productRepository.Table
                             join si in _shopItemRepository.Table on p.Id equals si.ProductId
                             select new ShopItemAssociateWithProduct
                             {
                                 Product = p,
                                 ShopItem = si
                             };
            return await itemModels.ToListAsync();
        }

        #endregion
    }
}