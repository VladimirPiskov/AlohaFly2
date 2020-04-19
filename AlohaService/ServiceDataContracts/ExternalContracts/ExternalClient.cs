using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace AlohaService.ServiceDataContracts.ExternalContracts
{
    public class ExternalClient
    {
        [DataMember]
        public string Phone { set; get; }

        [DataMember]
        public string Name { set; get; }

        [DataMember]
        public string Address { set; get; }
        [DataMember]
        public string Emale { set; get; }


    }
}