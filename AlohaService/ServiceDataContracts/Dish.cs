using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AlohaService.ServiceDataContracts
{
    [DataContract]
    public class Dish : INotifyPropertyChanged, ICloneable
    {
        [DataMember]
        [Display(Name = "Баркод")]
        public long Barcode { get; set; }

        [DataMember]
        [Display(Name = "Id", AutoGenerateField = false)]
        public long Id { get; set; }

        [DataMember]
        [Display(Name = "SHId", AutoGenerateField = false )]
        public long SHId { get; set; }

        [DataMember]
        [Display(Name = "Наименование")]
        public string Name { get; set; }
        
        [DataMember]
        [Display(Name = "Наименование на русском", AutoGenerateField =false)]
        internal string RussianName { get
            {
                return Name;
            }

            set
            {
                Name = value;
            }
        }
        

        [DataMember]
        [Display(Name = "Наименование на английском")]
        public string EnglishName { get; set; }

        [DataMember]
        [Display(Name = "Цена для \r\n самолетов")]
        public decimal PriceForFlight { get; set; }

        [DataMember]
        [Display(Name = "Цена для \r\n доставки")]
        public decimal PriceForDelivery { get; set; }




        [DataMember]
        [Display(Name = "Белки", AutoGenerateField = true)]

        public int B { get; set; }
        [DataMember]
        [Display(Name = "Жиры", AutoGenerateField = true)]

        public int J { get; set; }
        [DataMember]
        [Display(Name = "Углеводы", AutoGenerateField = true)]

        public int U { get; set; }
        [DataMember]
        [Display(Name = "Каллории", AutoGenerateField = true)]

        public int Ccal { get; set; }


        [DataMember]
        [Display(Name = "Алк.")]
        public bool IsAlcohol { get; set; }

        [DataMember]
        [Display(Name = "Активно")]
        public bool IsActive { get; set; } = true;

        [DataMember]
        [Display(Name = "Временное")]
        public bool IsTemporary { get; set; } = false;


        [DataMember]
        [Display(Name = "Для доставки")]
        public bool IsToGo { get; set; } = false;

        [DataMember]
        [Display(Name = "Для Шереметьево")]
        public bool IsShar { get; set; } = false;

        [DataMember]
        [Display(Name = "Доп. имя")]


        public string RussianNameExt { get; set; }





        [DataMember]
        [Display(Name = "Артикул",AutoGenerateField =false)]
        public string Articul { get; set; }

        [DataMember]
        [Display(Name = "Выход", AutoGenerateField = false)]
        public string Unit { get; set; }

        [DataMember]
        //[Display(AutoGenerateField = false)]
        public long? DishKitсhenGroupId { get; set; }


        //[Display(AutoGenerateField = false)]
        [DataMember]
        [Display(Name = "Кухонная группа")]
        public DishKitchenGroup DishKitсhenGroup { get; set; }

        [DataMember]
        //[Display(AutoGenerateField = false)]
        public long? DishLogicGroupId { get; set; }

        [DataMember]
        [Display(Name ="Логическая группа")]
        public DishLogicGroup DishLogicGroup { get; set; }

        [DataMember]
        //[Display(AutoGenerateField = false)]
        [Display(Name = "Кол-во порций на одну наклейку ToFly")]
        public int ToFlyLabelSeriesCount { get; set; } = 1;

        [DataMember]
        //[Display(AutoGenerateField = false)]
        [Display(Name = "Кол-во порций на одну наклейку ToGo")]
        public int ToGoLabelSeriesCount { get; set; } = 1;

        [Display(AutoGenerateField =false)]
        public int LabelsCount { get; set; }

        [DataMember]
        [Display(Name = "Печатать ли в меню")]
        public bool NeedPrintInMenu { get; set; }

        [DataMember]
        [Display(Name = "Наименование в меню")]
        public string MenuName { get; set; } = "";

        [DataMember]
        [Display(Name = "Наименование в меню на английском")]
        public string MenuEnglishName { get; set; } = "";

        [DataMember]
        [Display(Name = "Наименование наклейки на русском")]
        public string LabelRussianName { get; set; } = "";

        [DataMember]
        [Display(Name = "Наименование наклейки на английском")]
        public string LabelEnglishName { get; set; } = "";

        [DataMember]
        [Display(Name = "Код в Гастрофуд", AutoGenerateField = true)]
        public long SHGastroId { get; set; }


        [DataMember]
        [Display(Name = "SHRid", AutoGenerateField = true)]
        public long SHIdNewBase { get; set; }



        


        [DataMember]
        [Display(AutoGenerateField = false)]
        public Guid LastUpdatedSession { get; set; }


        public object Clone()
        {
            return this.MemberwiseClone();
        }


        #region INotifyPropertyChanged we do use Foody https://github.com/Fody/PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}