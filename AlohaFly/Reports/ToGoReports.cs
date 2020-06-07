using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using NLog;

namespace AlohaFly.Reports
{
    class ToGoReports
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public static ToGoReports Instanse
        {
            get
            {
                return new ToGoReports();
            }
        }


        Application app;
        Workbook Wb;
        Worksheet Ws;
        private void OpenXls()
        {
            app = new Microsoft.Office.Interop.Excel.Application();
            Wb = app.Workbooks.Add(true);
            Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            app.Visible = false;
        }

        private void WriteStandartHeader(string name, DateTime sDt, DateTime eDt)
        {
            Ws.Name = "Общий отчет";
            Ws.get_Range("A1:M1000").Font.Name = "Antica";
            Ws.get_Range("A1:M7").Font.Name = "Helica";
            Ws.get_Range("A1:M1000").Font.Size = 10;

            Range _excelCells1 = (Range)Ws.get_Range("A2", "F2").Cells;
            _excelCells1.Merge(Type.Missing);

            Ws.Cells[2, 1] = $"{name}";
            Ws.get_Range("A2").Font.Size = 14;
            Ws.get_Range("A2").Font.Bold = true;
            Ws.get_Range("A2").HorizontalAlignment = XlHAlign.xlHAlignCenter;

            Range _excelCells2 = (Range)Ws.get_Range("A3", "F3").Cells;
            _excelCells2.Merge(Type.Missing);

            Ws.Cells[3, 1] = $"{sDt.ToString("dd/MM/yyyy")} - {eDt.ToString("dd/MM/yyyy")}";
            Ws.get_Range("A3").Font.Size = 12;
            Ws.get_Range("A3").Font.Bold = false;
            Ws.get_Range("A3").HorizontalAlignment = XlHAlign.xlHAlignCenter;

            /*
            Ws.Cells[2, 3] = $"{sDt.ToString("dd/MM")} - {eDt.ToString("dd/MM/yyyy")}";
            Ws.get_Range("C2:C2").Font.Size = 12;
            Ws.get_Range("C2:C2").Font.Bold = false;
                       
            */
        }
        public  void ShowClientsReport(DateTime sDt, DateTime eDt)
        {
            OpenXls();
            try
            {
                WriteStandartHeader("Отчет по клиентам ToGo", sDt, eDt);



                var data = DataExtension.DataCatalogsSingleton.Instance.OrdersToGoData.Data.Where(a => a.DeliveryDate >= sDt && a.DeliveryDate < eDt && a.OrderStatus!=AlohaService.ServiceDataContracts.OrderStatus.Cancelled);
                int row = 5;
                Ws.Cells[row, 1] = "Клиент";
                Ws.Cells[row, 2] = "Телефон";
                Ws.Cells[row, 3] = "E-mail";
                int col = 5;
                foreach (var ch in DataExtension.DataCatalogsSingleton.Instance.MarketingChannelData.Data)
                {
                    Ws.Cells[row, col] = ch.Name;
                     Ws.Range[Ws.Cells[row, col], Ws.Cells[row, col+1]].Merge(Type.Missing);
                    //_excelCells1.Merge(Type.Missing);

                    Ws.Cells[row+1, col] = "Сумма заказов";
                    Ws.Cells[row+1, col+1] = "Кол-во ";
                    col =col+2;
                }
                Ws.Cells[row, col] ="Итого";
                Ws.Range[Ws.Cells[row, col], Ws.Cells[row, col + 1]].Merge(Type.Missing);
                Ws.Cells[row + 1, col] = "Сумма";
                Ws.Cells[row + 1, col + 1] = "Кол-во ";

                //Ws.Cells[row, col++] = "Сумма заказов";
                //Ws.Cells[row, col++] = "Кол-во заказов";
                (Ws.Rows[row] as Range).HorizontalAlignment = XlHAlign.xlHAlignCenter;
                (Ws.Rows[row+1] as Range).HorizontalAlignment = XlHAlign.xlHAlignCenter;
                (Ws.Rows[row] as Range).Font.Bold = true;



                int colMax = col + 1;
                row++;
                foreach (var client in data.Select(a => new { client = a.OrderCustomer
                    , sum = data.Where(c => c.OrderCustomer == a.OrderCustomer).Sum(c=>c.OrderDishesSumm)
                    , cc = data.Where(c => c.OrderCustomer == a.OrderCustomer).Count() }).Distinct().OrderByDescending(a=>a.sum))
                {
                    row++;
                    if (client.client == null) continue;
                        Ws.Cells[row, 1] = client.client.FullName;
                    Ws.Cells[row, 2].NumberFormat = "@";

                    var q = DataExtension.DataCatalogsSingleton.Instance.OrderCustomerPhoneData.Data.Where(a => a.IsPrimary && a.OrderCustomerId == client.client.Id);
                    if (q.Any()) 
                    {
                        if (q.FirstOrDefault().Phone!= null)
                        {
                            Ws.Cells[row, 2] = q.FirstOrDefault().Phone.Trim();
                        }
                    }


                    Ws.Cells[row, 3].NumberFormat = "@";
                    if (client.client.Email != null)
                    {
                        Ws.Cells[row, 3] = client.client.Email.Trim();
                    }


                    col = 5;
                    foreach (var ch in DataExtension.DataCatalogsSingleton.Instance.MarketingChannelData.Data)
                    {
                        (Ws.Cells[row, col] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                        (Ws.Cells[row, col+1] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;

                        Ws.Cells[row, col] = data.Where(c => c.OrderCustomerId == client.client.Id && c.MarketingChannelId==ch.Id).Sum(c => c.OrderDishesSumm);
                        Ws.Cells[row, col + 1] = data.Where(c => c.OrderCustomerId == client.client.Id && c.MarketingChannelId == ch.Id).Count();
                        
                        col = col + 2;
                    }


                    Ws.Cells[row, col++] = client.sum;
                    Ws.Cells[row, col++] = client.cc;
                    (Ws.Cells[row, 1] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;
                    (Ws.Cells[row, 2] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    (Ws.Cells[row, 3] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    (Ws.Cells[row, 5] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    
                }
                for (int c = 1; c <= colMax; c++)
                {
                    Ws.Columns[c].AutoFit();
                }
                
                app.Visible = true;
            }
            catch(Exception e)
            {
                logger.Error($"Error ShowClientsReport err: {e.Message}");
            }
        }

    }
}
