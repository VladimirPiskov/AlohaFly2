using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlohaService.Entities
{
    public class ItemLabelInfo
    {
        public long Id { get; set; }

        public long ParenItemId { get; set; }

        [ForeignKey("ParenItemId")]
        public virtual Dish Dish { get; set; }

        public long SerialNumber { get; set; }
        public string NameRus { get; set; }
        public string NameEng { get; set; }
        public string Message { get; set; }

    }
}