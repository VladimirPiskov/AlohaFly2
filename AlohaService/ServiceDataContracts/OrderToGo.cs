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
    public class OrderToGo : INotifyPropertyChanged, ICloneable, IOrderLabel
    {
        [DataMember]
        [Display(Name = "Внутренний идентификатор")]
        public long Id { get; set; }

        [DataMember]
        public long OldId { get; set; }

        [DataMember]
        [Display(Name = "Номер заказа")]
        public string OrderNumber { get; set; }

        [DataMember]
        [Display(Name = "Время создания заказа в системе")]
        public DateTime? CreationDate { get; set; }

        [DataMember]
        public long? CreatedById { get; set; }

        [DataMember]
        public User CreatedBy { get; set; }

        [DataMember]
        [Display(Name = "Внутренний идентификатор заказчика")]
        public long? OrderCustomerId { get; set; }

        [DataMember]
        public OrderCustomer OrderCustomer { get; set; }

        [DataMember]
        [Display(Name = "Время доставки")]
        public DateTime DeliveryDate { get; set; }

        
        [DataMember]
        [Display(Name = "Телефон")]
        public string PhoneNumber { get; set; }


        [DataMember]
        [Display(Name = "Адрес доставки")]
        public long? AddressId { get; set; }


        [Display(Name = "Адрес доставки")]
        public OrderCustomerAddress Address { get; set; }

        [DataMember]
        [Display(Name = "Время подачи")]
        public DateTime ExportTime { get; set; }

        [DataMember]
        [Display(Name = "Время готовности")]
        public DateTime ReadyTime { get; set; }
       
        [DataMember]
        [Display(Name = "Коментарий для кухни")]
        public string CommentKitchen { get; set; }
        
     


        [DataMember]
        [Display(Name = "Внутренний идентификатор курьера")]
        public long? DriverId { get; set; }

        [DataMember]
        public Driver Driver { get; set; }

        [DataMember]
        [Display(Name = "Cтатуса заказа")]
        public OrderStatus OrderStatus { get; set; }

        [DataMember]
        [Display(Name = "Коментарий")]
        public string OrderComment { get; set; }



        [DataMember]
        public decimal Summ { get; set; }
        [DataMember]
        public decimal DeliveryPrice { get; set; }

        [DataMember]
        public long? MarketingChannelId { get; set; }

        [DataMember]
        public MarketingChannel MarketingChannel { get; set; }

        [DataMember]
        public List<DishPackageToGoOrder> DishPackages { get; set; }

        //public int ForSort = 0;

        public List<IDishPackageLabel> DishPackagesNoSpis {
            get
            {
                return DishPackages?.Where(x => !x.Deleted).Select(x => (IDishPackageLabel)x).ToList();
            } }

        public List<IDishPackageLabel> DishPackagesSpis
        {
            get
            {
                return DishPackages?.Where(x => x.Deleted).Select(x => (IDishPackageLabel)x).ToList();
            }
        }
        public List<IDishPackageLabel> DishPackagesForLab
        {
            get
            {
                //if (DishPackages == null) return null;
                return DishPackagesNoSpis?.Select(a => (IDishPackageLabel)a).ToList();

            }
        }


        [DataMember]
        public long? PaymentId { get; set; }

        [DataMember]
        [Display(Name = "Вид платежа")]
        public Payment PaymentType { get; set; }

        [DataMember]
        [Display(Name = "Скидка")]
        public decimal DiscountPercent { get; set; }

        [DataMember]
        [Display(AutoGenerateField = false)]
        public bool NeedPrintFR { get; set; }

        [DataMember]
        [Display(AutoGenerateField = false)]
        public bool NeedPrintPrecheck { get; set; }

        [DataMember]
        [Display(AutoGenerateField = false)]
        public bool FRPrinted { get; set; }

        [DataMember]
        [Display(AutoGenerateField = false)]
        public bool Closed { get; set; }

        [DataMember]
        [Display(AutoGenerateField = false)]
        public bool PreCheckPrinted { get; set; }


        [DataMember]
        [Display(AutoGenerateField = false)]
        public bool IsSHSent { get; set; } = false;

        [DataMember]
        public Guid LastUpdatedSession { get; set; }

        public decimal DiscountSumm
        {
            get
            {
                return Math.Round(OrderDishesSumm * DiscountPercent / 100, 2);
            }
        }


        public decimal OrderTotalSumm
        {
            get
            {
                return OrderDishesSumm - DiscountSumm+ DeliveryPrice;
            }
        }

        public decimal OrderDishesSumm
        {
            get
            {
                if (DishPackages == null) return 0;
                if (OrderStatus == OrderStatus.Cancelled) return 0;
                return DishPackagesNoSpis.Sum(a => a.TotalSumm);
            }
        }

        public decimal CancelledOrderDishesSumm
        {
            get
            {
                if (DishPackages == null) return 0;
               // if (OrderStatus == OrderStatus.Cancelled) return 0;
                return DishPackages.Sum(a => a.TotalSumm);
            }
        }


        
     



        #region INotifyPropertyChanged we do use Foody https://github.com/Fody/PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion


        public object Clone()
        {
            var ord = (OrderToGo)this.MemberwiseClone();
            ord.PropertyChanged = null;
            ord.Id = 0;
            ord.DishPackages = new List<DishPackageToGoOrder>();
            foreach (var d in DishPackages)
            {
                var pack = (DishPackageToGoOrder)d.Clone();
                ord.DishPackages.Add(pack);
                pack.OrderToGo = ord;
                pack.OrderToGoId = ord.Id;
            }


            /*
                foreach (var d in ord.DishPackages)
            {
                d.Id = 0;
                d.OrderToGoId = 0;
                d.OrderToGo = ord;
            }
            */
            return ord;
        }
    }
}