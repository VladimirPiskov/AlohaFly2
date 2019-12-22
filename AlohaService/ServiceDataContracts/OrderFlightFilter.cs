using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace AlohaService.ServiceDataContracts
{
    [DataContract]
    public class OrderFlightFilter
    {
        [DataMember]
        public string OrderNumber { get; set; }

        [DataMember]
        public DateTime? CreationDateStart { get; set; }

        [DataMember]
        public DateTime? CreationDateEnd { get; set; }

        [DataMember]
        public long? CreatedById { get; set; }

        [DataMember]
        public long? AirCompanyId { get; set; }

        [DataMember]
        public DateTime? DeliveryDateStart { get; set; }

        [DataMember]
        public DateTime? DeliveryDateEnd { get; set; }

        [DataMember]
        public long? DeliveryPlaceId { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public long? ContactPersonId { get; set; }

        [DataMember]
        public DateTime? ExportTimeStart { get; set; }

        [DataMember]
        public DateTime? ExportTimeEnd { get; set; }

        [DataMember]
        public DateTime? ReadyTimeStart { get; set; }

        [DataMember]
        public DateTime? ReadyTimeEnd { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public int? NumberOfBoxes { get; set; }

        [DataMember]
        public long? WhoDeliveredPersonPersonId { get; set; }

        [DataMember]
        public long? DriverId { get; set; }

        [DataMember]
        public OrderStatus? OrderStatus { get; set; }

        [DataMember]
        public string OrderComment { get; set; }

        [DataMember]
        public decimal? ExtraCharge { get; set; }

        [DataMember]
        public string FlightNumber { get; set; }

        [DataMember]
        public int? Code { get; set; }
    }
}