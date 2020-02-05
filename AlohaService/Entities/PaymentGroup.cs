using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlohaService.Entities
{
    public class PaymentGroup : Interfaces.IRealTimeUpdater
    {
        public long Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        
        
        public bool IsActive { get; set; } = true;

        public bool Sale { get; set; } = true;


        public DateTime? UpdatedDate { get; set; }
        public Guid LastUpdatedSession { get; set; }

    }
}