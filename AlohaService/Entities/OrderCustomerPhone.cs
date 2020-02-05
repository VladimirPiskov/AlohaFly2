using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlohaService.Entities
{
    public class OrderCustomerPhone : Interfaces.IRealTimeUpdater
    {
        public long Id { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrimary { get; set; }

        public long OrderCustomerId { get; set; }
        [ForeignKey("OrderCustomerId")]
        public virtual OrderCustomer OrderCustomer { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public Guid LastUpdatedSession { get; set; }

    }
}