using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlohaService.Entities
{
    public class LogItem
    {
        public long Id { get; set; }
        public string MethodName { get; set; }

        public string ActionName { get; set; }
        public string ActionDescription { get; set; }

        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User CreatedBy { get; set; }

        public string StateBefore { get; set; }

        public string StateAfter { get; set; }

        public DateTime CreationDate { get; set; }
    }
}