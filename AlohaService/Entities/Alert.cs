using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlohaService.Entities
{
    public class Alert
    {
        public long Id { get; set; }
        public string Message { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Period { get; set; }
    }
}