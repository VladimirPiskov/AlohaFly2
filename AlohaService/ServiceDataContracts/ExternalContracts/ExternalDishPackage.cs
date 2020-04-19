using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace AlohaService.ServiceDataContracts.ExternalContracts
{
    public class ExternalDishPackage
    {
        [DataMember]
        public int Id { set; get; }
        [DataMember]
        public string Name { set; get; }
        [DataMember]
        public decimal Price { set; get; }
        [DataMember]
        public decimal Count { set; get; }
        [DataMember]
        public string Comment { set; get; }


    }
}