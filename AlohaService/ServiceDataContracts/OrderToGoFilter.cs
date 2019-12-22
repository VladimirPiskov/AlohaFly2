using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AlohaService.ServiceDataContracts
{
    [DataContract]
    public class OrderToGoFilter
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
        public long? OrderCustomerId { get; set; }

        [DataMember]
        public DateTime? DeliveryDateStart { get; set; }

        [DataMember]
        public DateTime? DeliveryDateEnd { get; set; }

        [DataMember]
        public long? DeliveryPlaceId { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

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
        public long? CurierId { get; set; }

        [DataMember]
        public OrderStatus? OrderStatus { get; set; }

        [DataMember]
        public string OrderComment { get; set; }

        [DataMember]
        public string ExtraCharge { get; set; }

        [DataMember]
        public int? Code { get; set; }

        [DataMember]
        public MarketingChannel MarketingChannel { get; set; }
    }
}