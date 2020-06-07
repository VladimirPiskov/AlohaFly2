using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlohaService.Entities
{
    public class Payment : Interfaces.IRealTimeUpdater
    {
        public long Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public bool IsCash { get; set; }
        public long FiskalId { get; set; }
        public bool IsActive { get; set; } = true;


        
        public long FRSend { get; set; }
        
        public bool ToGo { get; set; }
        public long SHId { get; set; }

        public long SHPlaceId { get; set; }

        public long PaymentGroupId { get; set; }

        
        public PaymentGroup PaymentGroup { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid LastUpdatedSession { get; set; }
    }
}