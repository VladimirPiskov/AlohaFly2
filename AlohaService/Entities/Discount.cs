using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlohaService.Entities
{
    public class Discount
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool ToGo { get; set; } = false;
        [NotMapped]
        public virtual List<DiscountRange> Ranges { get; set; }

        public Discount()
        {
            Ranges = new List<DiscountRange>();
        }
    }
}