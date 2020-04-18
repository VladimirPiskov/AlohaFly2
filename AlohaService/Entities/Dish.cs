using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlohaService.Entities
{
    public class Dish : Interfaces.IRealTimeUpdater
    {
        public long Id { get; set; }
        public string RussianName { get; set; }
        public string EnglishName { get; set; }
        public long Barcode { get; set; }
        public decimal PriceForFlight { get; set; }
        public decimal PriceForDelivery { get; set; }
        public bool IsToGo { get; set; } = false;
        public bool IsShar { get; set; } = false;


        public bool IsAlcohol { get; set; }
        public bool IsActive { get; set; }
        public bool IsTemporary { get; set; } = false;

        public string Articul { get; set; }
        public string Unit { get; set; }
        public long SHGastroId { get; set; }
        public long? DishLogicGroupId { get; set; }

        /*
        [ForeignKey("DishLogicGroupId")]
        public virtual DishLogicGroup DishLogicGroup { get; set; }
        */
        public long? DishKitсhenGroupId { get; set; }

        /*
        [ForeignKey("DishKitсhenGroupId")]
        public virtual DishKitchenGroup DishKitсhenGroup { get; set; }
        */
        public int ToFlyLabelSeriesCount { get; set; } = 1;
        public int ToGoLabelSeriesCount { get; set; } = 1;


        public int B { get; set; }
        public int J { get; set; }

        public int U { get; set; }

        public int Ccal { get; set; }

        public long SHId { get; set; }
        public long SHIdNewBase { get; set; }

        public string Name { get; set; }

        public bool NeedPrintInMenu { get; set; }
        public string MenuName { get; set; } = "";
        public string MenuEnglishName { get; set; } = "";
        public string LabelRussianName { get; set; }

        public string LabelEnglishName { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid LastUpdatedSession { get; set; }
    }
}