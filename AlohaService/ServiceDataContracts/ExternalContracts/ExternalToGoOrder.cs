using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace AlohaService.ServiceDataContracts.ExternalContracts
{
    public class ExternalToGoOrder
    {
        [DataMember]
        public int ExternalId { set; get; }

        [DataMember]
        public string ExternalStringId { set; get; }

        [DataMember]
        public decimal Summ { set; get; }
        [DataMember]
        public decimal DeliveryPrice { set; get; }


        
        [DataMember]
        public string Comment { set; get; }

        [DataMember]
        public List<ExternalDishPackage> Dishes { set; get; }
        
        [DataMember]
        public ExternalClient Client { set; get; }



        [DataMember]
        public DateTime DeliveryDate { set; get; }

        
    }
}