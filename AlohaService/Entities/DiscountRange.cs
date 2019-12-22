using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlohaService.Entities
{
    public class DiscountRange
    {
        public long Id { get; set; }
        public long Start { get; set; }
        public long? End { get; set; }
        public decimal DiscountPercent { get; set; }
    }
}