using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlohaService.Entities
{
    public class OrderCustomerInfo
    {
        public long Id { get; set; }
        public long OrderCustomerId { get; set; }
        public int OrderCount { get; set; }

        public int OrderCountAll { get; set; }
        public decimal MoneyCount { get; set; }

        public decimal CashBackSumm { get; set; }

    }
}