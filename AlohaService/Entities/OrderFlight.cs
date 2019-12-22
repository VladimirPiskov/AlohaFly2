using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlohaService.Entities
{
    public class OrderFlight
    {
        public long Id { get; set; }
        public string OrderNumber { get; set; }

        public long? CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public virtual User CreatedBy { get; set; }

        public long? SendById { get; set; }
        [ForeignKey("SendById")]
        public virtual User SendBy { get; set; }

        public DateTime CreationDate { get; set; }

        public long? AirCompanyId { get; set; }
        [ForeignKey("AirCompanyId")]
        public virtual AirCompany AirCompany { get; set; }

        public DateTime DeliveryDate { get; set; }

        public long? DeliveryPlaceId { get; set; }
        [ForeignKey("DeliveryPlaceId")]
        public virtual DeliveryPlace DeliveryPlace { get; set; }

        public virtual string DeliveryAddress { get; set; }

        public string PhoneNumber { get; set; }

        public long? ContactPersonId { get; set; }
        [ForeignKey("ContactPersonId")]
        public virtual ContactPerson ContactPerson { get; set; }

        public DateTime ExportTime { get; set; }

        public DateTime ReadyTime { get; set; }

        public string Comment { get; set; }

        public int NumberOfBoxes { get; set; }

        public long? WhoDeliveredPersonPersonId { get; set; }
        [ForeignKey("WhoDeliveredPersonPersonId")]
        public virtual DeliveryPerson WhoDeliveredPersonPerson { get; set; }

        public long? DriverId { get; set; }
        [ForeignKey("DriverId")]
        public virtual Driver Driver { get; set; }

        public int OrderStatus { get; set; }

        public string OrderComment { get; set; }

        public decimal ExtraCharge { get; set; }

        public string FlightNumber { get; set; }

        public int? Code { get; set; }

        public Guid? AlohaGuidId { get; set; }


        public virtual ICollection<DishPackageFlightOrder> DishPackages { get; set; }

        public long? PaymentId { get; set; }
        [ForeignKey("PaymentId")]
        public virtual Payment PaymentType { get; set; }

        public decimal DiscountSumm { get; set; }

        public bool Closed { get; set; }

        public bool NeedPrintFR { get; set; }

        public bool NeedPrintPrecheck { get; set; }

        public bool FRPrinted { get; set; }

        public bool PreCheckPrinted { get; set; }

        public bool IsSHSent { get; set; }

        public OrderFlight()
        {
            DishPackages = new HashSet<DishPackageFlightOrder>();
        }

    }
}