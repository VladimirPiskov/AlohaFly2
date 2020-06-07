using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlohaService.Entities
{
    public class DishExternalLinks 
    {
        public long Id { get; set; }
      
        public long DishId { get; set; }
        public long MarketingChanelId { get; set; }
        public long ExternalId { get; set; }
    }
}