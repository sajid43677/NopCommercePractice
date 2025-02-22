﻿using Nop.Core;
using Nop.Core.Domain.Localization;

namespace NopStation.Plugin.Misc.FacebookShop.Domains
{
    public partial class ShopItem : BaseEntity, ILocalizedEntity
    {
        public int ProductId { get; set; }

        public bool IncludeInFacebookShop { get; set; }

        public int GenderTypeId { get; set; }

        public int GoogleProductCategory { get; set; }

        public string Brand { get; set; }

        public string AgeGroupType { get; set; }

        public string CustomImageUrl { get; set; }

        public int ProductConditionTypeId { get; set; }

        public GenderType GenderType
        {
            get => (GenderType)GenderTypeId;
            set => GenderTypeId = (int)value;
        }

        public ProductConditionType ProductConditionType
        {
            get => (ProductConditionType)ProductConditionTypeId;
            set => ProductConditionTypeId = (int)value;
        }
    }
}