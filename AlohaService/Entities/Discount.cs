using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlohaService.Entities
{
    public class Discount : Interfaces.IRealTimeUpdater
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool ToGo { get; set; } = false;
        [NotMapped]
        public virtual List<DiscountRange> Ranges { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid LastUpdatedSession { get; set; }
        public Discount()
        {
            Ranges = new List<DiscountRange>();
        }
    }
}