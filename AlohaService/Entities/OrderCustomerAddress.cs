using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlohaService.Entities
{
    public class OrderCustomerAddress
    {
        public long Id { get; set; }
        public long OldId { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrimary { get; set; }

        public string MapUrl { get; set; }
        public string SubWay { get; set; }
        public string Comment { get; set; }
        public long ZoneId { get; set; }

        public long OrderCustomerId { get; set; }
        [ForeignKey("OrderCustomerId")]
        public virtual OrderCustomer OrderCustomer { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public Guid LastUpdatedSession { get; set; }

    }
}