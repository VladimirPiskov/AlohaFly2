using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AlohaService.Interfaces;

namespace AlohaService.ServiceDataContracts
{
    [DataContract]
    public class DishPackageFlightOrder : ICloneable, INotifyPropertyChanged, IDishPackageLabel
    {
        [DataMember]
        [Display(Name = "Внутренний идентификатор", AutoGenerateField = false)]
        public long Id { get; set; }

        [DataMember]
        [Display(Name = "Внутренний идентификатор перелёта", AutoGenerateField = false)]
        public long OrderFlightId { get; set; }

        [DataMember]
        [Display(AutoGenerateField = false)]
        public OrderFlight OrderFlight { get; set; }

        [Display(Name = "Печать наклейки", AutoGenerateField = false)]
        bool printLabel { get; set; }
        public bool PrintLabel { get; set; }


        [DataMember]
        [Display(Name = "Внутренний идентификатор блюда", AutoGenerateField = false)]
        public long DishId { get; set; }

        [DataMember]
        [Display(AutoGenerateField = false)]
        public Dish Dish { get; set; }

        [DataMember]
        [Display(Name = "Название блюда")]
        public string DishName { get; set; }

        [DataMember]
        [Display(Name = "Количество")]
        public decimal Amount { get; set; } = 1;

        [DataMember]
        [Display(Name = "Цена")]
        public decimal TotalPrice { get; set; }
        [DataMember]
        public bool Deleted { get; set; } = false;
        [DataMember]
        public int DeletedStatus { get; set; }
        [DataMember]
        public long SpisPaymentId { get; set; }
        public Payment SpisPayment { get; set; }
        public string NameWithDeletedInfo
        {
            get
            {
                switch (DeletedStatus)
                {
                    case 1:
                        return $"{DishName} (Со списанием)";
                    case 2:
                        return $"{DishName} (Без списания)";
                    default:
                        return DishName;
                        
                }
            }
        }

        public decimal TotalSumm
        {
            get
            {
                return Amount * TotalPrice;
            }
        }




        int labelSeriesCount = -1;
        public int LabelSeriesCount
        {
            get
            {
                if (labelSeriesCount != -1) return labelSeriesCount;
                if (Dish.ToFlyLabelSeriesCount == 0) return 0;
                if (Dish == null) return 0;
                if (!PrintLabel) return 0;
                return (int)Math.Ceiling(Amount / (decimal)Dish.ToFlyLabelSeriesCount);
            }
            set
            {
                labelSeriesCount = value;
            }

        }
        //AlohaFlyKitchen

        public int LabelsCount
        {
            get
            {
                if (Dish == null) return 0;
                if (Dish.ToFlyLabelSeriesCount == 0) return 0;
                return (int)Math.Ceiling(Amount / (decimal)Dish.ToFlyLabelSeriesCount);
            }
        }

        public object Clone()
        {
            var p = (DishPackageFlightOrder)this.MemberwiseClone();
            p.PropertyChanged = null;
            p.Id = 0;
            p.OrderFlight = null;
            p.OrderFlightId = 0;
            
            return p;
        }

        [DataMember]
        [Display(Name = "Коментарий")]
        public string Comment { get; set; }



        [DataMember]
        [Display(Name = "Перелёт")]
        public int PassageNumber { get; set; } = 1;

        [DataMember]
        [Display(Name = "Номер в заказе")]
        public long PositionInOrder { get; set; }

        [DataMember]
        public int? Code { get; set; }
        public bool Printed { set; get; } = true;

        /*
        [DataMember]
        public int? Code { get; set; }
        */



        #region INotifyPropertyChanged we do use Foody https://github.com/Fody/PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;


        #endregion
    }
}