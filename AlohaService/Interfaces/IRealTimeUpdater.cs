using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlohaService.Interfaces
{
    public interface IRealTimeUpdater
    {
        DateTime UpdatedDate { get; set; }
    }
}
