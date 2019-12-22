using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace AlohaService.ServiceDataContracts
{
    [DataContract]
    public class PageInfo
    {
        [DataMember]
        public int Skip { get; set; }

        [DataMember]
        public int Take { get; set; }
    }
}