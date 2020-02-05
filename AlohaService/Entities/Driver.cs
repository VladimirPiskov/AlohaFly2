using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlohaService.Entities
{
    public class Driver : Interfaces.IRealTimeUpdater
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid LastUpdatedSession { get; set; }
    }
}