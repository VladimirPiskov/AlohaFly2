using AlohaService.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlohaService.Entities
{
    public class OrderToGo
    {
        public long Id { get; set; }
        public long OldId { get; set; }
        public string OrderNumber { get; set; }

        public long? CreatedById { get; set; }
     
        public DateTime CreationDate { get; set; }

        public long? OrderCustomerId { get; set; }
     
        public DateTime DeliveryDate { get; set; }

     
        public string PhoneNumber { get; set; }
        public long? AddressId { get; set; }

        public DateTime ExportTime { get; set; }

        public DateTime ReadyTime { get; set; }

        public string Comment { get; set; }

        public string CommentKitchen { get; set; }

        public int NumberOfBoxes { get; set; }

        //public long? WhoDeliveredPersonPersonId { get; set; }
        /*
        [ForeignKey("WhoDeliveredPersonPersonId")]
        public virtual DeliveryPerson WhoDeliveredPersonPerson { get; set; }
        */
        public long? DriverId { get; set; }
        /*
        [ForeignKey("CurierId")]
        public virtual Curier Curier { get; set; }
        */
        public int OrderStatus { get; set; }

        public string OrderComment { get; set; }

        //public string ExtraCharge { get; set; }

        public long? MarketingChannelId { get; set; }
        //public int? MarketingChannel { get; set; }

        //public int? Code { get; set; }
        public decimal Summ { get; set; }
        public decimal DeliveryPrice { get; set; }

        public virtual ICollection<DishPackageToGoOrder> DishPackages { get; set; }

        public long? PaymentId { get; set; }
        /*
        [ForeignKey("PaymentId")]
        public virtual Payment PaymentType { get; set; }
        */
        //public decimal DiscountSumm { get; set; }
        public decimal DiscountPercent { get; set; }


        public bool NeedPrintFR { get; set; }

        public bool NeedPrintPrecheck { get; set; }

        public bool FRPrinted { get; set; }

        public bool PreCheckPrinted { get; set; }

        public bool Closed { get; set; }

        public bool IsSHSent { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public Guid LastUpdatedSession { get; set; }

        public OrderToGo()
        {
            DishPackages = new HashSet<DishPackageToGoOrder>();
        }
    }
}