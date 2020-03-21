using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace AlohaService.ServiceDataContracts
{
    [DataContract]
    public class AnalitikData
    {
        [DataMember]
        public DateTime sDate { set; get; }
        [DataMember]
        public DateTime eDate { set; get; }
        [DataMember]
        public List<AnalitikDataRecord> Data { set; get; } = new List<AnalitikDataRecord>();
        
    }
    public class AnalitikDataRecord
    {
        [DataMember]
        public DateTime Date { set; get; }
        [DataMember]
        public Decimal Value { set; get; }
        [DataMember]
        public Decimal Data1 { set; get; }
        [DataMember]
        public Decimal Data2 { set; get; }
    }


    public class AnalitikOrderData
    {
        [DataMember]
        public DateTime DeleveryDate { set; get; }
        [DataMember]
        public long? CustomerID { set; get; }
        [DataMember]
        public decimal Summ { set; get; }
        [DataMember]
        public long? MarketingChanel { set; get; }
        [DataMember]
        public long? PaymentId { set; get; }


    }
}