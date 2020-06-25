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
    public class DishPackageToGoOrder : ICloneable, INotifyPropertyChanged, IDishPackageLabel
    {
        [DataMember]
        [Display(Name = "Внутренний идентификатор")]
        public long Id { get; set; }

        [DataMember]
        [Display(Name = "Внутренний идентификатор заказа")]
        public long OrderToGoId { get; set; }

        [DataMember]
        public OrderToGo OrderToGo { get; set; }

        [DataMember]
        [Display(Name = "Внутренний идентификатор блюда")]
        public long DishId { get; set; }

        [DataMember]
        public Dish Dish { get; set; }

        [DataMember]
        [Display(Name = "Количество")]
        public decimal Amount { get; set; }

        [DataMember]
        [Display(Name = "Общая стоимость")]
        public decimal TotalPrice { get; set; }

        [DataMember]
        [Display(Name = "Коментарий")]
        public string Comment { get; set; }

        [DataMember]
        [Display(Name = "Название блюда")]
        public string DishName { get; set; }

        [DataMember]
        [Display(Name = "Нормер в заказе")]
        public long PositionInOrder { get; set; }

        [DataMember]
        public int? Code { get; set; }

        [DataMember]
        public bool Deleted { get; set; } = false;
        [DataMember]
        public int DeletedStatus { get; set; }
        [DataMember]
        public long SpisPaymentId { get; set; }

        [DataMember]
        public long ExternalCode { get; set; }
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



        [Display(Name = "Печать наклейки", AutoGenerateField = false)]
        bool printLabel { get; set; }
        public bool PrintLabel { get; set; }

        int labelSeriesCount = -1;
        public int LabelSeriesCount
        {
            get
            {
                if (labelSeriesCount != -1) return labelSeriesCount;
                if (Dish.ToGoLabelSeriesCount == 0) return 0;
                if (Dish == null) return 0;
                if (!PrintLabel) return 0;
                return (int)Math.Ceiling(Amount / (decimal)Dish.ToGoLabelSeriesCount);
            }
            set
            {
                labelSeriesCount = value;
            }

        }


        public int LabelsCount
        {
            get
            {
                if (Dish == null) return 0;
                if (Dish.ToGoLabelSeriesCount == 0) return 0;
                return (int)Math.Ceiling(Amount / (decimal)Dish.ToGoLabelSeriesCount);
            }
        }

        public decimal TotalSumm
        {
            get
            {
                return Amount * TotalPrice;
            }
        }

        public object Clone()
        {

            var p = (DishPackageToGoOrder)this.MemberwiseClone();
            p.PropertyChanged = null;
            p.Id = 0;
            p.OrderToGo = null;
            p.OrderToGoId = 0;

            return p;
        }

        
        public bool Printed { get; set; } = true;

        #region INotifyPropertyChanged we do use Foody https://github.com/Fody/PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}