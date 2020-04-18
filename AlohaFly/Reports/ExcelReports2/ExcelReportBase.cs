using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace AlohaFly.Reports.ExcelReports2
{
    public  class ExcelReportBase
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public void Create()
        {
            /*
            logger.Debug($"ToFlyMenuCreate order:{order.Id}; template:{templateFolder + templateMenuPath}");
            if (order == null) return;
            app = new Microsoft.Office.Interop.Excel.Application();
            Wb = app.Workbooks.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"\" + templateFolder + templateMenuPath);
            Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            */
        }
    }
}
