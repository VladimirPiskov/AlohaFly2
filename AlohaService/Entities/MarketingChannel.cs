using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlohaService.Entities
{
    public class MarketingChannel : Interfaces.IRealTimeUpdater
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public Guid LastUpdatedSession { get; set; }

    }
}