using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlohaService.Entities
{
    public class TransactionTime
    {
        public long Id { get; set; }
        public Guid Transaction { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public DateTime? LastConfirmedTime { get; set; }
        public bool Confirmed { get; set; }
    }
}