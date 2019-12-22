using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace AlohaService.ServiceDataContracts
{
    [DataContract]
    public class DishFilter
    {
        [DataMember]
        public string RussianNameLike { get; set; }
        [DataMember]
        public decimal? FlightPriceStart { get; set; }
        [DataMember]
        public decimal? FlightPriceEnd { get; set; }

        [DataMember]
        public bool? IsActive { get; set; }
        [DataMember]
        public bool? IsTemporary { get; set; }
        [DataMember]
        public bool? IsAlcohol { get; set; }
    }
}