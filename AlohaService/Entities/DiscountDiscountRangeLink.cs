    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlohaService.Entities
{
    public class DiscountDiscountRangeLink
    {
        public long Id { get; set; }
        public long DiscountId { get; set; }
        public long DiscountRangeId { get; set; }
    }
}