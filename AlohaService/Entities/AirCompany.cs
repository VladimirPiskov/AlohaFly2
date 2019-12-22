using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlohaService.Entities
{
    public class AirCompany
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Inn { get; set; }
        public string IkaoCode { get; set; }
        public string IataCode { get; set; }
        public string RussianCode { get; set; }
        public bool IsActive { get; set; }

        public long? DiscountId { get; set; }

        public long SHId { get; set; }

        [ForeignKey("DiscountId")]
        public virtual Discount DiscountType { get; set; }

        public long? PaymentId { get; set; }

        [ForeignKey("PaymentId")]
        public virtual Payment PaymentType { get; set; }

        public string Code1C { get; set; }
        public string Name1C { get; set; }

    }
}