using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlohaService.Entities
{
    public class DishPackageFlightOrder
    {
        public long Id { get; set; }

        public long OrderFlightId { get; set; }
        [ForeignKey("OrderFlightId")]
        public virtual OrderFlight OrderFlight { get; set; }

        public long DishId { get; set; }
        [ForeignKey("DishId")]
        public virtual Dish Dish { get; set; }

        public decimal Amount { get; set; }
        public decimal TotalPrice { get; set; }

        public string Comment { get; set; }
        public string DishName { get; set; }

        public int PassageNumber { get; set; }

        public long PositionInOrder { get; set; }

        public int? Code { get; set; }

        public bool Deleted { get; set; } = false;
        public int DeletedStatus { get; set; }
        public long SpisPaymentId { get; set; }

    }
}