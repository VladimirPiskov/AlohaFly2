using AlohaService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace AlohaService.ServiceDataContracts
{
    [DataContract]
    public class UpdateResult
    {
        [DataMember]
        public Guid SessionId { set; get; }
        [DataMember]
        public List<OrderCustomerAddress> OrderCustomerAddresss { set; get; }
        [DataMember]
        public List<OrderCustomerPhone> OrderCustomerPhones { set; get; }
        [DataMember]
        public List<OrderCustomer> OrderCustomers { set; get; }

        [DataMember]
        public List<OrderToGo> OrderToGos { set; get; }

        [DataMember]
        public DateTime UpdatesTime { set; get; }
    }
}   