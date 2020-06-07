using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace AlohaService.ServiceDataContracts
{
    public class OrderCustomerInfo
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public long OrderCustomerId { get; set; }
        [DataMember]
        public int OrderCount { get; set; }
        [DataMember]
        public decimal MoneyCount { get; set; }


        [DataMember]
        public decimal MoneyCount2 { get; set; }

        [DataMember]
        public decimal CashBackSumm { get; set; }


        public decimal AvgCheck
        {
            get {
                return OrderCount != 0 ? MoneyCount / (decimal)OrderCount : 0;
            }
        }


    }
}