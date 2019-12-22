using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AlohaService.Entities
{
    public class OrderCustomer
    {
        public long Id { get; set; }
        public long OldId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }


        public string SecondName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string Comments { get; set; }

        public decimal DiscountPercent { get; set; }

        public virtual ICollection<OrderCustomerPhone> Phones { get; set; }

        public virtual ICollection<OrderCustomerAddress> Addresses { get; set; }

        public OrderCustomer()
        {
            Phones = new HashSet<OrderCustomerPhone>();
            Addresses = new HashSet<OrderCustomerAddress>();
        }

        public DateTime? UpdatedDate { get; set; }
        public Guid LastUpdatedSession { get; set; }

       
        

    }
}