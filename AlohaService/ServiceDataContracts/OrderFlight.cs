using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ComponentModel;
using AlohaService.Interfaces;

namespace AlohaService.ServiceDataContracts
{




    [DataContract]
    public class OrderFlight : ICloneable, IOrderLabel, INotifyPropertyChanged
    {
        [DataMember]
        [Display(Name = "Внутренний идентификатор")]
        public long Id { get; set; }

        [DataMember]
        [Display(Name = "Номер заказа")]
        public string OrderNumber { get; set; }

        [DataMember]
        [Display(Name = "Время создания заказа в системе")]
        public DateTime? CreationDate { get; set; }

        [DataMember]
        public long? CreatedById { get; set; }

        [Display(Name = "Кто принял")]
        [DataMember]
        public User CreatedBy { get; set; }

        [DataMember]
        public long? SendById { get; set; }

        [Display(Name = "Кто отдал")]
        [DataMember]
        public User SendBy { get; set; }

        [DataMember]
        [Display(Name = "Внутренний идентификатор авиакомпании")]
        public long? AirCompanyId { get; set; }

        [DataMember]
        public virtual AirCompany AirCompany { get; set; }

        [DataMember]
        [Display(Name = "Время доставки")]
        public DateTime DeliveryDate { get; set; }

        [DataMember]
        [Display(Name = "Внутренний идентификатор места доставки")]
        public long? DeliveryPlaceId { get; set; }

        [DataMember]
        public virtual DeliveryPlace DeliveryPlace { get; set; }

        [DataMember]
        [Display(Name = "Адрес доставки")]
        public virtual string DeliveryAddress { get; set; }

        [DataMember]
        [Display(Name = "Телефон")]
        public string PhoneNumber { get; set; }

        [DataMember]
        [Display(Name = "Внутренний идентификатор контакта для связи")]
        public long? ContactPersonId { get; set; }

        [DataMember]
        public virtual ContactPerson ContactPerson { get; set; }

        [DataMember]
        [Display(Name = "Время подачи")]
        public DateTime ExportTime { get; set; }

        [DataMember]
        [Display(Name = "Время готовности")]
        public DateTime ReadyTime { get; set; }

        [DataMember]
        [Display(Name = "Коментарий")]
        public string Comment { get; set; }


        public string CommentKitchen
        {
            get { return Comment; }
        }

        [DataMember]
        [Display(Name = "Число коробок")]
        public int NumberOfBoxes { get; set; } = 1;

        [DataMember]
        [Display(Name = "Внутренний идентификатор курьера")]
        public long? WhoDeliveredPersonPersonId { get; set; }

        [DataMember]
        public virtual DeliveryPerson WhoDeliveredPersonPerson { get; set; }

        [DataMember]
        [Display(Name = "Внутренний идентификатор водителя")]
        public long? DriverId { get; set; }

        [DataMember]
        public Driver Driver { get; set; }

        [DataMember]
        [Display(Name = "Cтатус заказа")]
        public OrderStatus OrderStatus { get; set; }

        [DataMember]
        [Display(Name = "Коментарий")]
        public string OrderComment { get; set; }

        [DataMember]
        [Display(Name = "Надбавка")]
        public decimal ExtraCharge { get; set; }

        [DataMember]
        [Display(Name = "Борт")]
        public string FlightNumber { get; set; }

        [DataMember]
        public int? Code { get; set; }


        [DataMember]
        public Guid? AlohaGuidId { get; set; }

        [DataMember]
        public List<DishPackageFlightOrder> DishPackages { get; set; }

        public List<IDishPackageLabel> DishPackagesNoSpis
        {
            get
            {
                return DishPackages?.Where(x => !x.Deleted).Select(x => (IDishPackageLabel)x).ToList();
            }
        }

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
                //if (DishPackages == null ) return null;
                return DishPackagesNoSpis?.Select(a => (IDishPackageLabel)a).ToList();

            }
        }


        public bool DishPackagesChanged { set; get; }

        [DataMember]
        public long? PaymentId { get; set; }

        [DataMember]
        [Display(Name = "Вид платежа")]
        public Payment PaymentType { get; set; }

        [DataMember]
        [Display(Name = "Скидка")]
        public decimal DiscountSumm { get; set; }


        [DataMember]
        [Display(Name = "Заказ закрыт")]
        public bool Closed { get; set; }

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
        public bool PreCheckPrinted { get; set; }

        [DataMember]
        [Display(AutoGenerateField = false)]
        public bool IsSHSent { get; set; }

        public decimal OrderTotalSumm
        {
            get
            {
                return OrderSumm - DiscountSumm;
            }
        }

        public decimal DiscountPercent
        {
            get
            {
                if (OrderSumm == 0) return 0;
                return DiscountSumm*100 / OrderSumm;
            }
            set { }
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
                //if (OrderStatus == OrderStatus.Cancelled) return 0;
                return DishPackages.Sum(a => a.TotalSumm);
            }
        }



        public decimal OrderSumm
        {
            get
            {
                if (DishPackagesNoSpis == null) return 0;
                if (OrderStatus == OrderStatus.Cancelled) return 0;
                return DishPackagesNoSpis.Sum(a => a.TotalSumm) * (1 + ExtraCharge / 100);
            }
        }

        public decimal ExtraChargeSumm
        {
            get
            {
                if (DishPackagesNoSpis == null) return 0;
                if (OrderStatus == OrderStatus.Cancelled) return 0;
                // if (OrderSumm == 0) return 0;

                //(x.OrderSumm == 0 ? 1 : (x.OrderTotalSumm / x.OrderSumm)

                var dp = (OrderSumm == 0? 0:DiscountSumm / OrderSumm);
                //return OrderDishesSumm * (ExtraCharge / 100) * (1 - dp);
                return OrderDishesSumm * (ExtraCharge / 100) * (1 - DiscountPercent / 100);
            }
        }




        #region INotifyPropertyChanged we do use Foody https://github.com/Fody/PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;


        #endregion
        public object Clone()
        {
            var ord = (OrderFlight)this.MemberwiseClone();
            ord.PropertyChanged = null;

            ord.Id = 0;

            ord.DishPackages = new List<DishPackageFlightOrder>();
            foreach (var d in DishPackages)
            {
                var pack = (DishPackageFlightOrder)d.Clone();
                ord.DishPackages.Add(pack);
                pack.OrderFlight = ord;
                pack.OrderFlightId = ord.Id;
            }

            /*
            foreach (var d in ord.DishPackages)
            {
                d.Id = 0;
                d.OrderFlightId = 0;
                d.OrderFlight = ord;
            }
            */
            return ord;
        }

    }
}