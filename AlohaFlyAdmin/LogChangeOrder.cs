using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlohaService;

namespace AlohaFlyAdmin
{
    class LogChangeOrder
    {
       static public List<AlohaService.ServiceDataContracts.LogItem> GetLogsOfOrder(string orderId)
        {
            var dtEnd = DateTime.Now;
            var dtStart = dtEnd.AddMonths(-1);
            var res =AlohaFly.DBProvider.Client.GetLogItems(dtStart, dtEnd);
            if (res.Success)
            {
                return res.Result.Where(a => a.StateBefore.Contains(orderId)).ToList();
            }
            return null;
        }
    }

}
