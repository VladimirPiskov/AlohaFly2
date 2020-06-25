using AlohaFly.DataExtension;
using AlohaFly.Models;
using AlohaService.ServiceDataContracts;
using Microsoft.Office.Interop.Excel;
using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AlohaFly.Reports
{
    class ExcelReports
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public ExcelReports()
        { }
        Application app;
        Workbook Wb;
        Worksheet Ws;

        private void OpenXls()
        {
            app = new Microsoft.Office.Interop.Excel.Application();
            Wb = app.Workbooks.Add(true);
            Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;

            app.Visible = true;
        }


        private readonly string templateFolder = @"\Data\";
        private readonly string templateMenuPath = @"Menu.xltx";
        private readonly string templateInvoiceFlyNameRus = @"InvoiceTemplateRus.xltx";
        private readonly string templateInvoiceFlyNameEng = @"InvoiceTemplateEng.xltx";
        private readonly string templateAnalitikReport = @"AnalRepTemplate.xltx";


        public void ToFlyMenuCreate(OrderFlight order)
        {
            foreach (int pn in order.DishPackages.Select(a => a.PassageNumber).Distinct())
            {
                ToFlyMenuCreateForPassage(order, pn);
            }

        }


        public void AnaliticReportCreate(DateTime sDt, DateTime eDt)
        {
            app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Wb = app.Workbooks.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"\" + templateFolder + templateAnalitikReport);
            Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;

            var data = Analytics.ReportsGeneratorSingleton.Instance.GetSebesReportData(sDt, eDt);
        }


            public void InvoiceToFlyCreate(OrderFlight order, bool rus,bool showDiscount)
        {
            if (order == null) return;
            app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Wb = app.Workbooks.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"\" + templateFolder + (rus ? templateInvoiceFlyNameRus:templateInvoiceFlyNameEng));
            Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;

            worksheet.Cells[2, 3] = worksheet.Cells[2, 3].Value + $" №{order.Id}";
            worksheet.Cells[7, 4] = order.AirCompany?.Name;
            worksheet.Cells[8, 4] = order.FlightNumber;
            worksheet.Cells[9, 4] = order.FlightNumber2;
            worksheet.Cells[10, 4] = order.DeliveryDate.ToString("dd.MM.yyyy в HH:mm");
            worksheet.Cells[11, 4] = order.FlightDateTime?.ToString("dd.MM.yyyy в HH:mm");

            worksheet.Cells[7, 6] = order.DestPort;
            worksheet.Cells[8, 6] = order.Route;
            worksheet.Cells[9, 6] = order.DeliveryPlace?.InvoiceName;
            worksheet.Cells[10, 6] = order.Aircraft;
            worksheet.Cells[11, 6] = order.PersonCount==0 ? "": order.PersonCount.ToString();

            int rowIndex = 14;
            foreach (var d in order.DishPackages.Where(a=>!a.Deleted).OrderBy(a=>a.PositionInOrder))
            {

                if (rowIndex < order.DishPackages.Where(a => !a.Deleted).Count()+ 13)
                {
                    ((worksheet.Cells[rowIndex+1, 2] as Range).EntireRow as Range).Insert(XlInsertShiftDirection.xlShiftDown, false);

                    //worksheet.Range[worksheet.Cells[rowIndex, 3], worksheet.Cells[rowIndex, 5]].Cells.Merge(Type.Missing);
                    //worksheet.Range[worksheet.Cells[rowIndex, 1], worksheet.Cells[rowIndex, 8]].Cells.Borders.LineStyle = XlLineStyle.xlContinuous;
                }

                worksheet.Cells[rowIndex, 1] = rowIndex-13;
                worksheet.Cells[rowIndex, 2] = d.Dish.Barcode;
                if (d.Dish.IsAlcohol)
                {
                    worksheet.Cells[rowIndex, 3] = rus ? "Открытый напиток" : "Open drink";
                }
                else
                {
                    worksheet.Cells[rowIndex, 3] = rus ? d.Dish.Name : d.Dish.EnglishName;
                }
                    worksheet.Cells[rowIndex, 6] = d.Amount;
                worksheet.Cells[rowIndex, 7] = d.TotalPrice;
                worksheet.Cells[rowIndex, 8] = d.TotalSumm;
                rowIndex++;
            }
            if (order.ExtraCharge > 0)
            {
                ((worksheet.Cells[rowIndex, 2] as Range).EntireRow as Range).Insert(XlInsertShiftDirection.xlShiftDown, false);

                worksheet.Cells[rowIndex, 1] = rowIndex - 13;
                worksheet.Cells[rowIndex, 3] = rus ? "Наценка за срочность 10%" : "Extra charge for urgency 10%";
                worksheet.Cells[rowIndex, 8] = order.OrderDishesSumm * order.ExtraCharge / 100;
                rowIndex++;
            }

            if (showDiscount && order.DiscountSumm > 0)
            {

                ((worksheet.Cells[rowIndex, 2] as Range).EntireRow as Range).Insert(XlInsertShiftDirection.xlShiftDown, false);

                worksheet.Cells[rowIndex, 3] = rus ? $"Скидка ": "Discount"+ (order.DiscountSumm / order.OrderSumm) +"%";
                worksheet.Cells[rowIndex, 8] = (order.DiscountSumm );
                rowIndex++;
            }


            worksheet.Cells[rowIndex, 6] = order.DishPackages.Sum(a=>a.Amount);
            worksheet.Cells[rowIndex, 8] = showDiscount ? order.OrderTotalSumm: order.OrderSumm;
            rowIndex += 2;

            worksheet.Cells[rowIndex, 1] = worksheet.Cells[rowIndex, 1].Value.ToString() + Math.Ceiling(((decimal)rowIndex+14)/45).ToString();
            rowIndex++;
            worksheet.Cells[rowIndex, 1] = worksheet.Cells[rowIndex, 1].Value.ToString() + order.DishPackages.Where(a => !a.Deleted).Count().ToString();
            worksheet.Cells[rowIndex + 12, 3] = order.NumberOfBoxes;

        }




        public void ToFlyMenuCreateForPassage(OrderFlight order, int pn)
        {

            try
            {
                logger.Debug($"ToFlyMenuCreate order:{order.Id}; template:{templateFolder + templateMenuPath}");
                if (order == null) return;
                app = new Microsoft.Office.Interop.Excel.Application();
                Wb = app.Workbooks.Add(System.AppDomain.CurrentDomain.BaseDirectory + @"\" + templateFolder + templateMenuPath);
                Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;


                int rowIndex = 1;

                Dictionary<DishKitchenGroup, List<Dish>> groups = new Dictionary<DishKitchenGroup, List<Dish>>();


                rowIndex++;

                foreach (var KGroup in order.DishPackagesNoSpis.Where(a => a.Dish.DishKitсhenGroupId != null && a.Dish.NeedPrintInMenu && ((DishPackageFlightOrder)a).PassageNumber==pn).Select(a => a.Dish.DishKitсhenGroup).Distinct().OrderBy(a => a.Id))
                {

                    //   foreach (var group in groups.Keys)
                    //  {
                    //if (groups[group].Where(a=>a.NeedPrintInMenu).Count() < 1) continue;
                    rowIndex++;
                    worksheet.Cells[rowIndex, 2] = KGroup.Name;

                    (worksheet.Cells[rowIndex, 2] as Range).Font.Size = 16;
                    (worksheet.Cells[rowIndex, 2] as Range).Font.Bold = true;
                    (worksheet.Cells[rowIndex, 2] as Range).HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    (worksheet.Cells[rowIndex, 2] as Range).Font.Name = "Helvetica Neue";
                    (worksheet.Cells[rowIndex, 2] as Range).Font.Italic = true;
                    //worksheet.Rows[rowIndex].RowHeight = 25;// SetHeight(new RowHeight(25, true));
                    worksheet.Cells[rowIndex, 5] = KGroup.EnglishName;
                    (worksheet.Cells[rowIndex, 5] as Range).Font.Size = 16;
                    (worksheet.Cells[rowIndex, 5] as Range).Font.Bold = true;
                    (worksheet.Cells[rowIndex, 5] as Range).Font.Italic = true;
                    (worksheet.Cells[rowIndex, 5] as Range).Font.Name = "Helvetica Neue";
                    (worksheet.Cells[rowIndex, 5] as Range).HorizontalAlignment = XlHAlign.xlHAlignCenter;



                    foreach (var dish in order.DishPackagesNoSpis.Where(a => a.Dish.DishKitсhenGroupId != null && a.Dish.DishKitсhenGroupId == KGroup.Id && a.Dish.NeedPrintInMenu && ((DishPackageFlightOrder)a).PassageNumber == pn).OrderBy(a => a.PositionInOrder).Select(a => a.Dish))
                    {

                        //if (dish.NeedPrintInMenu)
                        {
                            rowIndex++;

                            worksheet.Cells[rowIndex, 2] = String.IsNullOrWhiteSpace(dish.MenuName) ? dish.Name : dish.MenuName;
                            (worksheet.Cells[rowIndex, 2] as Range).Font.Italic = true;
                            (worksheet.Cells[rowIndex, 2] as Range).Font.Size = 12;
                            (worksheet.Cells[rowIndex, 2] as Range).Font.Name = "Helvetica Neue";
                            (worksheet.Cells[rowIndex, 2] as Range).HorizontalAlignment = XlHAlign.xlHAlignCenter;
                            //worksheet.Rows[rowIndex].RowHeight = 25;
                            worksheet.Cells[rowIndex, 5] = String.IsNullOrWhiteSpace(dish.MenuEnglishName) ? dish.EnglishName : dish.MenuEnglishName;
                            (worksheet.Cells[rowIndex, 5] as Range).Font.Size = 12;
                            (worksheet.Cells[rowIndex, 5] as Range).Font.Italic = true;
                            (worksheet.Cells[rowIndex, 5] as Range).HorizontalAlignment = XlHAlign.xlHAlignCenter;
                            (worksheet.Cells[rowIndex, 5] as Range).Font.Name = "Helvetica Neue";
                        }

                    }
                }
                rowIndex++;
                rowIndex++;
                foreach (var dish in order.DishPackagesNoSpis.Where(a => a.Dish.DishKitсhenGroupId == null && a.Dish.NeedPrintInMenu && ((DishPackageFlightOrder)a).PassageNumber == pn).OrderBy(a => a.PositionInOrder).Select(a => a.Dish))
                {

                    //if (dish.NeedPrintInMenu)
                    {
                        rowIndex++;

                        worksheet.Cells[rowIndex, 2] = String.IsNullOrWhiteSpace(dish.MenuName) ? dish.Name : dish.MenuName; ;
                        (worksheet.Cells[rowIndex, 2] as Range).Font.Size = 12;
                        (worksheet.Cells[rowIndex, 2] as Range).Font.Italic = true;
                        (worksheet.Cells[rowIndex, 2] as Range).Font.Name = "Helvetica Neue";
                        (worksheet.Cells[rowIndex, 2] as Range).HorizontalAlignment = XlHAlign.xlHAlignCenter;
                        //worksheet.Rows[rowIndex].RowHeight = 25;
                        worksheet.Cells[rowIndex, 5] = String.IsNullOrWhiteSpace(dish.MenuEnglishName) ? dish.EnglishName : dish.MenuEnglishName;

                        (worksheet.Cells[rowIndex, 5] as Range).Font.Size = 12;
                        (worksheet.Cells[rowIndex, 5] as Range).Font.Italic = true;
                        (worksheet.Cells[rowIndex, 5] as Range).Font.Name = "Helvetica Neue";
                        (worksheet.Cells[rowIndex, 5] as Range).HorizontalAlignment = XlHAlign.xlHAlignCenter;

                    }

                }


                rowIndex++;
                worksheet.Cells[rowIndex, 1] = " ";
                worksheet.Cells[rowIndex, 5] = " ";
                rowIndex++;
                worksheet.Cells[rowIndex, 1] = " ";
                worksheet.Cells[rowIndex, 5] = " ";


                app.Visible = true;
            }
            catch (Exception e)
            {
                logger.Error("Error ToFlyMenuCreate " + e.Message);
            }
        }




        private int FillOrdersTableByComp(Worksheet Ws, List<OrderFlight> Orders, int row, bool alc)
        {
            int startRow = row;
            Ws.Range[Ws.Cells[row, 2], Ws.Cells[row, 10]].Font.Bold = true;
            Ws.Range[Ws.Cells[row, 2], Ws.Cells[row, 10]].Font.Size = 11;
            Ws.Cells[row, 3] = "Дата";
            Ws.Cells[row, 4] = "Борт";
            Ws.Cells[row, 5] = "Номер";
            //Ws.Cells[7, 5] = "Стол";
            if (Orders.Sum(a => a.DiscountSumm) > 0)
            {
                Ws.Cells[row, 6] = "Сумма";
                Ws.Cells[row, 7] = "Скидка";
                Ws.Cells[row, 8] = "Итого";
            }
            else
            {
                Ws.Cells[row, 6] = "Сумма";
            }

            int num = 0;
            row++;
            //foreach (var chk in Orders.Where(a => a.GetOrderTotalSummByAlc(alc) > 0).OrderBy(a => a.DeliveryDate))
            foreach (var chk in Orders.OrderBy(a => a.DeliveryDate))
            {
                num++;
                Ws.Cells[row, 2] = num;
                Ws.Cells[row, 3] = chk.DeliveryDate.ToString("dd/MM/yyyy");
                Ws.Cells[row, 4] = chk.FlightNumber;
                Ws.Cells[row, 5] = chk.Id;
                if (Orders.Sum(a => a.DiscountSumm) > 0)
                {
                    /*
                    Ws.Cells[row, 6] = chk.GetOrderSummByAlc(alc);
                    Ws.Cells[row, 7] = chk.GetDiscountAlc(alc);
                    Ws.Cells[row, 8] = chk.GetOrderTotalSummByAlc(alc);
                    */
                    Ws.Cells[row, 6] = chk.OrderSumm;
                    Ws.Cells[row, 7] = chk.DiscountSumm;
                    Ws.Cells[row, 8] = chk.OrderTotalSumm;
                }
                else
                {
                    //Ws.Cells[row, 6] = chk.GetOrderTotalSummByAlc(alc);
                    Ws.Cells[row, 6] = chk.OrderTotalSumm;
                }
                row++;
            }
            row++;

            Ws.Range[Ws.Cells[row, 2], Ws.Cells[row, 10]].Font.Bold = true;
            Ws.Cells[row, 3] = "Итого";


            // if (Orders.Sum(a => a.GetDiscountAlc(alc)) > 0)
            if (Orders.Sum(a => a.DiscountSumm) > 0)
            {
                decimal ComplexDiscPercent = 0;

                if (Orders.Sum(a => a.DiscountSumm) != 0)
                {
                    ComplexDiscPercent = 1 - (Orders.Sum(a => a.OrderTotalSumm) / Orders.Sum(a => a.OrderSumm));
                }

                Ws.Cells[row, 6] = Orders.Sum(chk => chk.OrderSumm);
                Ws.Cells[row, 7] = Orders.Sum(chk => chk.DiscountSumm).ToString("0.00") + " (" + ComplexDiscPercent.ToString("0%") + ")";
                Ws.Cells[row, 8] = Orders.Sum(chk => chk.OrderTotalSumm);

                AllBorders(Ws.Range[Ws.Cells[startRow, 2], Ws.Cells[row, 8]].Borders);
            }
            else
            {
                Ws.Cells[row, 6] = Orders.Sum(chk => chk.OrderTotalSumm);
                AllBorders(Ws.Range[Ws.Cells[startRow, 2], Ws.Cells[row, 6]].Borders);
            }

            Ws.Columns[3].AutoFit();
            Ws.Columns[6].AutoFit();
            Ws.Columns[7].AutoFit();
            Ws.Columns[8].AutoFit();
            return row;
        }


        private void FillWSByComp(Worksheet Ws, AirCompanyOrders compOrders, DateTime sDt, DateTime eDt)
        {
            //if (CompOrders.Count == 0) { UI.UIModify.ShowAlert("Выбранный диапазон не содержит заказзов"); return null; }

            // OpenXls();

            var company = compOrders.AirCompany;
            Ws.Name = company.Name;

            Ws.get_Range("A1:M1000").Font.Name = "Antica";
            Ws.get_Range("A1:M7").Font.Name = "Helica";
            Ws.get_Range("A1:M1000").Font.Size = 10;

            (Ws.Cells[2, 4] as Range).HorizontalAlignment = XlHAlign.xlHAlignCenter;
            (Ws.Cells[2, 4] as Range).Font.Size = 12;
            (Ws.Cells[2, 4] as Range).Font.Bold = true;
            Ws.Cells[2, 4] = company.Name;

            (Ws.Cells[4, 4] as Range).HorizontalAlignment = XlHAlign.xlHAlignCenter;
            Ws.Cells[4, 4] = sDt.ToString("dd/MM/yyyy") + " - " + eDt.ToString("dd/MM/yyyy");

            (Ws.Cells[6, 3] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;
            (Ws.Cells[6, 3] as Range).Font.Size = 12;
            (Ws.Cells[6, 3] as Range).Font.Bold = true;
            Ws.Cells[6, 3] = "ФЛАЙФУД";

            int row = 8;

            row = FillOrdersTableByComp(Ws, compOrders.Orders.ToList(), row, false);

            /*
            if (compOrders.Orders.Sum(a => a.GetOrderTotalSummByAlc(true)) > 0)
            {
                row += 2;

                (Ws.Cells[row, 3] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;
                (Ws.Cells[row, 3] as Range).Font.Size = 12;
                (Ws.Cells[row, 3] as Range).Font.Bold = true;
                Ws.Cells[row, 3] = "ГАСТРОФУД";

                row += 2;
                row = FillOrdersTableByComp(Ws, compOrders.Orders.ToList(), row, true);

            }
            
            Ws.Columns[3].AutoFit();
            Ws.Columns[6].AutoFit();
            Ws.Columns[7].AutoFit();
            Ws.Columns[8].AutoFit();
            */

            //Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.Worksheets.Add();

        }

        private void AllBorders(Borders _borders)
        {
            _borders[XlBordersIndex.xlEdgeLeft].LineStyle = XlLineStyle.xlContinuous;
            _borders[XlBordersIndex.xlEdgeRight].LineStyle = XlLineStyle.xlContinuous;
            _borders[XlBordersIndex.xlEdgeTop].LineStyle = XlLineStyle.xlContinuous;
            _borders[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlContinuous;
            _borders.Color = Color.Black;
        }

        public void OrdersToExcelByComp(AirCompanyOrders compOrders, DateTime sDt, DateTime eDt)
        {

            if (compOrders.Orders.Count() == 0) { UI.UIModify.ShowAlert("Выбранный диапазон не содержит заказзов"); return; }
            OpenXls();
            FillWSByComp(Ws, compOrders, sDt, eDt);
        }

        public void AllOrdersToExcelByComps(List<AirCompanyOrders> AllCompsOrders, DateTime sDt, DateTime eDt)
        {
            if (AllCompsOrders.Count == 0) { UI.UIModify.ShowAlert("Выбранный диапазон не содержит заказзов"); return; }
            OpenXls();
            app.Visible = false;

            foreach (var compOrders in AllCompsOrders.OrderBy(a => a.Name))
            {
                FillWSByComp(Ws, compOrders, sDt, eDt);

                Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.Worksheets.Add();
            }
            Ws.Delete();
            app.Visible = true;
        }

        #region ToGoOrders

        internal void DBFGetPaymentsSummToGo(DateTime StartDate, DateTime StopDate)
        {
            OpenXls();
            Ws.Name = "Баланс";
            Ws.get_Range("A1:M1000").Font.Name = "Antica";
            Ws.get_Range("A1:M7").Font.Name = "Helica";
            Ws.get_Range("A1:M1000").Font.Size = 10;

            Ws.Cells[1, 5] = "UCS R-Keeper Reports";
            Ws.Cells[1, 6] = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            (Ws.Cells[1, 7] as Range).Font.Italic = true;
            (Ws.Cells[1, 7] as Range).Font.Size = 10;
            (Ws.Cells[1, 7] as Range).Font.Underline = true;
            Ws.Cells[1, 7] = "Галерея";



            (Ws.Cells[3, 6] as Range).Font.Bold = true;
            (Ws.Cells[3, 6] as Range).Font.Italic = true;
            (Ws.Cells[3, 6] as Range).Font.Size = 16;
            (Ws.Cells[3, 6] as Range).Font.Name = "Helica";

            Ws.Cells[3, 6] = "Баланс";
            Ws.Cells[6, 4] = "период:";
            (Ws.Cells[6, 6] as Range).Font.Bold = true;
            (Ws.Cells[6, 6] as Range).Font.Underline = true;
            (Ws.Cells[6, 6] as Range).Font.Size = 10;
            (Ws.Cells[6, 6] as Range).Font.Name = "Helica";
            Ws.Cells[6, 6] = StartDate.ToString("dd.MM.yyyy") + " - " + StopDate.ToString("dd.MM.yyyy");
            Ws.Cells[6, 7] = "вкл.";

            Ws.get_Range("A8:P8").Font.Bold = true;
            Ws.Cells[8, 2] = "чеков";
            Ws.Cells[8, 3] = "сумма";

            int row = 10;

            foreach (var p in ToGoOrdersModelSingleton.Instance.Orders.Select(b => b.PaymentType).Distinct())
            {
                if (p == null) continue;
                //if (t.Id == 0) continue;
                List<string> Companies = new List<string>();

                //int ChCount = Tmp.Where(a => Companies.Contains(a.STCompanyName)).Count();
                //decimal ChSumm = Tmp.Where(a => Companies.Contains(a.STCompanyName)).Sum(a => a.TotalSumm);
                //if ((t.Id == 0) && (ChSumm == 0)) continue;

                Ws.Cells[row, 1] = p.Name;
                (Ws.Cells[row, 1] as Range).Font.Bold = true;
                Ws.Cells[row, 2] = ToGoOrdersModelSingleton.Instance.Orders.Where(b => b.PaymentType == p).Count();
                Ws.Cells[row, 3] = ToGoOrdersModelSingleton.Instance.Orders.Where(b => b.PaymentType == p).Sum(a => a.Summ + a.DeliveryPrice);
                row++;
                /*
                foreach (var Compan in ToGoOrdersModelSingleton.Instance.Orders.Where(b => b.PaymentType == p))
                {

                    Ws.Cells[row, 1] = Compan.PaymentType.Name;
                    Ws.Cells[row, 2] = Compan.co;
                    Ws.Cells[row, 3] = Compan.OrdersTotalSumm;
                    row++;
                }

                Ws.Cells[row, 1] = "Итого " + p.Name;
                Ws.Cells[row, 2] = AirOrdersModelSingleton.Instance.AirCompanyOrders.Where(b => b.AirCompany?.PaymentType == p).Sum(a => a.OrdersCount);
                Ws.Cells[row, 3] = AirOrdersModelSingleton.Instance.AirCompanyOrders.Where(b => b.AirCompany?.PaymentType == p).Sum(a => a.OrdersTotalSumm);
                
                Range c4 = Ws.Cells[row, 1];
                Range c3 = Ws.Cells[row, 30];
                Ws.get_Range(c4, c3).Font.Bold = true;
                row++;
                row++;
                */
            }

            //  row++;


            Range c1 = Ws.Cells[row, 1];
            Range c2 = Ws.Cells[row, 10];
            Ws.get_Range(c1, c2).Font.Bold = true;


            Ws.Cells[row, 1] = "Итого";
            Ws.Cells[row, 2] = ToGoOrdersModelSingleton.Instance.Orders.Where(a => a.Closed).Count();
            Ws.Cells[row, 3] = ToGoOrdersModelSingleton.Instance.Orders.Where(a => a.Closed).Sum(a => a.Summ + a.DeliveryPrice);
            Ws.get_Range(c1, c2).EntireColumn.AutoFit();
        }

        #endregion


        internal void DBFGetPaymentsSumm(DateTime StartDate, DateTime StopDate)
        {
            try
            {
                OpenXls();
                Ws.Name = "Баланс";
                Ws.get_Range("A1:M1000").Font.Name = "Antica";
                Ws.get_Range("A1:M7").Font.Name = "Helica";
                Ws.get_Range("A1:M1000").Font.Size = 10;

                Ws.Cells[1, 5] = "UCS R-Keeper Reports";
                Ws.Cells[1, 6] = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                (Ws.Cells[1, 7] as Range).Font.Italic = true;
                (Ws.Cells[1, 7] as Range).Font.Size = 10;
                (Ws.Cells[1, 7] as Range).Font.Underline = true;
                Ws.Cells[1, 7] = "Галерея";

                (Ws.Cells[3, 6] as Range).Font.Bold = true;
                (Ws.Cells[3, 6] as Range).Font.Italic = true;
                (Ws.Cells[3, 6] as Range).Font.Size = 16;
                (Ws.Cells[3, 6] as Range).Font.Name = "Helica";

                Ws.Cells[3, 6] = "Баланс";
                Ws.Cells[6, 4] = "период:";
                (Ws.Cells[6, 6] as Range).Font.Bold = true;
                (Ws.Cells[6, 6] as Range).Font.Underline = true;
                (Ws.Cells[6, 6] as Range).Font.Size = 10;
                (Ws.Cells[6, 6] as Range).Font.Name = "Helica";
                Ws.Cells[6, 6] = StartDate.ToString("dd.MM.yyyy") + " - " + StopDate.ToString("dd.MM.yyyy");
                Ws.Cells[6, 7] = "вкл.";

                Ws.get_Range("A8:P8").Font.Bold = true;
                Ws.Cells[8, 2] = "чеков";
                Ws.Cells[8, 3] = "сумма";
                Ws.Cells[8, 5] = "доп. услуги";
                Ws.Cells[8, 6] = "продуктовая выручка";

                int row = 10;


                decimal allDopSum = 0;
                decimal allProdSum = 0;
                foreach (var p in AirOrdersModelSingleton.Instance.AirCompanyOrders.Select(a => a.AirCompany).Select(b => b.PaymentType).Distinct())
                {
                    //if (t.Id == 0) continue;
                    List<string> Companies = new List<string>();

                    //int ChCount = Tmp.Where(a => Companies.Contains(a.STCompanyName)).Count();
                    //decimal ChSumm = Tmp.Where(a => Companies.Contains(a.STCompanyName)).Sum(a => a.TotalSumm);
                    //if ((t.Id == 0) && (ChSumm == 0)) continue;

                    Ws.Cells[row, 1] = p.Name;
                    (Ws.Cells[row, 1] as Range).Font.Bold = true;
                    row++;
                    decimal dopSum = 0;
                    decimal prodSum = 0;
                    foreach (var Compan in AirOrdersModelSingleton.Instance.AirCompanyOrders.Where(b => b.AirCompany?.PaymentType == p))
                    {

                        Ws.Cells[row, 1] = Compan.AirCompany.Name;
                        Ws.Cells[row, 2] = Compan.OrdersCount;
                        Ws.Cells[row, 3] = Compan.OrdersTotalSumm;
                        dopSum += Ws.Cells[row, 5] = Compan.Orders.Sum(a => a.GetNoSpisDishesOfCatSum(7) + a.ExtraChargeSumm);
                        prodSum += Ws.Cells[row, 6] = Compan.OrdersTotalSumm - Compan.Orders.Sum(a => a.GetNoSpisDishesOfCatSum(7) + a.ExtraChargeSumm);
                        row++;
                    }

                    Ws.Cells[row, 1] = "Итого " + p.Name;
                    Ws.Cells[row, 2] = AirOrdersModelSingleton.Instance.AirCompanyOrders.Where(b => b.AirCompany?.PaymentType == p).Sum(a => a.OrdersCount);


                    Ws.Cells[row, 3] = AirOrdersModelSingleton.Instance.AirCompanyOrders.Where(b => b.AirCompany?.PaymentType == p).Sum(a => a.OrdersTotalSumm);
                    allDopSum += Ws.Cells[row, 5] = dopSum;
                    allProdSum += Ws.Cells[row, 6] = prodSum;
                    //Ws.Cells[row, 5] = AirOrdersModelSingleton.Instance.AirCompanyOrders.Where(b => b.AirCompany?.PaymentType == p).Sum(a => a.Orders.Sum( b=>b.GetNoSpisDishesOfCatSum(7)));
                    //Ws.Cells[row, 6] = AirOrdersModelSingleton.Instance.AirCompanyOrders.Where(b => b.AirCompany?.PaymentType == p).Sum(a => a.OrdersTotalSumm)-
                    AirOrdersModelSingleton.Instance.AirCompanyOrders.Where(b => b.AirCompany?.PaymentType == p).Sum(a => a.Orders.Sum(b => b.GetNoSpisDishesOfCatSum(7)));



                    Range c4 = Ws.Cells[row, 1];
                    Range c3 = Ws.Cells[row, 30];
                    Ws.get_Range(c4, c3).Font.Bold = true;
                    row++;
                    row++;
                }

                //  row++;


                Range c1 = Ws.Cells[row, 1];
                Range c2 = Ws.Cells[row, 10];
                Ws.get_Range(c1, c2).Font.Bold = true;


                Ws.Cells[row, 1] = "Итого";
                Ws.Cells[row, 2] = AirOrdersModelSingleton.Instance.AirCompanyOrders.Sum(a => a.OrdersCount);


                Ws.Cells[row, 3] = AirOrdersModelSingleton.Instance.AirCompanyOrders.Sum(a => a.OrdersTotalSumm);
                Ws.Cells[row, 5] = allDopSum;
                Ws.Cells[row, 6] = allProdSum;
                //Ws.Cells[row, 5] = AirOrdersModelSingleton.Instance.AirCompanyOrders.Sum(a => a.Orders.Sum(b => b.GetNoSpisDishesOfCatSum(7)));
                //Ws.Cells[row, 6] = AirOrdersModelSingleton.Instance.AirCompanyOrders.Sum(a => a.OrdersTotalSumm) -
                // AirOrdersModelSingleton.Instance.AirCompanyOrders.Sum(a => a.Orders.Sum(b => b.GetNoSpisDishesOfCatSum(7)));

                Ws.Columns[3].NumberFormat = "# ##0,00;-# ##0,00";
                Ws.Columns[5].NumberFormat = "# ##0,00;-# ##0,00";
                Ws.Columns[6].NumberFormat = "# ##0,00;-# ##0,00";

                Ws.get_Range(c1, c2).EntireColumn.AutoFit();
            }
            catch
            { }
        }

        private Worksheet AddWs(string Name)
        {
            Worksheet Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.Worksheets.Add();
            Ws.Name = Name;
            return Ws;
        }

        internal void DBFGetDishezByPayment(DateTime StartDate, DateTime StopDate)
        {
            try
            {
                OpenXls();
                List<ReportDish> Tmp = SQLGetRashDishez(StartDate, StopDate.AddDays(1));
                List<ReportDish> ConsolidTmp = new List<ReportDish>();
                //Dictionary<int, string> ItmGroups = FromSql.SQLGetItmGroups();
                foreach (ReportDish rd in Tmp)
                {
                    ReportDish rd2;
                    if (ConsolidTmp.Exists(a => a.Id == rd.Id))
                    {
                        rd2 = ConsolidTmp.Where(a => a.Id == rd.Id).FirstOrDefault();
                        rd2.Count += rd.Count;
                        rd2.Price += rd.Price * rd.Count;
                        rd2.DiscPrice += rd.DiscPrice * rd.Count;
                    }
                    else
                    {
                        /*
                        string GrName = "";
                        var dNCat =  DataCatalogsSingleton.Instance.Dishes.Where(a => Tmp.Select(b => b.BarCode).Contains(a.Barcode) && a.DishLogicGroupId == null).ToList();
                        if ((!DataCatalogsSingleton.Instance.Dishes.Any(a => a.Barcode == rd.BarCode))||(DataCatalogsSingleton.Instance.Dishes.FirstOrDefault(a => a.Barcode == rd.BarCode).DishLogicGroupId == null))

                        if ((!DataCatalogsSingleton.Instance.Dishes.Any(a => a.Barcode == rd.BarCode)) || (DataCatalogsSingleton.Instance.Dishes.FirstOrDefault(a => a.Barcode == rd.BarCode).DishLogicGroupId == null))

                        {
                            GrName = "Без категории";
                        }
                        else
                        {
                            GrName = DataCatalogsSingleton.Instance.Dishes.FirstOrDefault(a => a.Barcode == rd.BarCode).DishLogicGroup.Name;
                        }
                        */

                        rd2 = new ReportDish();
                        rd2.Id = rd.Id;
                        rd2.GroupeName = rd.GroupeName;
                        rd2.TenderAmount = new Dictionary<long, decimal>();
                        rd2.Name = rd.Name;
                        rd2.BarCode = rd.BarCode;
                        rd2.Count = rd.Count;
                        rd2.Price = rd.Price * rd.Count;
                        rd2.DiscPrice = rd.DiscPrice * rd.Count;
                        ConsolidTmp.Add(rd2);
                    }

                    decimal tndrAm = 0;
                    if (rd2.TenderAmount.TryGetValue(rd.TenderType, out tndrAm))
                    {
                        rd2.TenderAmount[rd.TenderType] += rd.DiscPrice * rd.Count;
                    }
                    else
                    {
                        rd2.TenderAmount.Add(rd.TenderType, rd.DiscPrice * rd.Count);
                    }
                }
                var Tndrs = new Dictionary<long, Payment>();
                foreach (var p in AirOrdersModelSingleton.Instance.AirCompanyOrders.Select(a => a.AirCompany).Select(b => b.PaymentType).Distinct())
                {
                    Tndrs.Add(p.Id, p);
                }


                List<ReportDish> Tmp2 = SQLGetRashDishezToGo(StartDate, StopDate.AddDays(1));
                List<ReportDish> ConsolidTmpToGo = new List<ReportDish>();
                //Dictionary<int, string> ItmGroups = FromSql.SQLGetItmGroups();
                foreach (ReportDish rd in Tmp2)
                {
                    ReportDish rd2;
                    if (ConsolidTmpToGo.Exists(a => a.Id == rd.Id))
                    {
                        rd2 = ConsolidTmpToGo.Where(a => a.Id == rd.Id).FirstOrDefault();
                        rd2.Count += rd.Count;
                        rd2.Price += rd.Price * rd.Count;
                        rd2.DiscPrice += rd.DiscPrice * rd.Count;
                    }
                    else
                    {
                        /*
                        string GrName = "";
                        if ((!DataCatalogsSingleton.Instance.Dishes.Any(a => a.Barcode == rd.BarCode)) || (DataCatalogsSingleton.Instance.Dishes.FirstOrDefault(a => a.Barcode == rd.BarCode).DishLogicGroupId == null))
                        {
                            GrName = "Без категории";
                        }
                        else
                        {
                            GrName = DataCatalogsSingleton.Instance.Dishes.FirstOrDefault(a => a.Barcode == rd.BarCode).DishLogicGroup.Name;
                        }
                        */
                        rd2 = new ReportDish
                        {
                            GroupeName = rd.GroupeName,
                            Id = rd.Id,
                            TenderAmount = new Dictionary<long, decimal>(),
                            Name = rd.Name,
                            BarCode = rd.BarCode,
                            Count = rd.Count,
                            Price = rd.Price * rd.Count,
                            DiscPrice = rd.DiscPrice * rd.Count
                        };
                        ConsolidTmpToGo.Add(rd2);
                    }

                    decimal tndrAm = 0;
                    if (rd2.TenderAmount.TryGetValue(rd.TenderType, out tndrAm))
                    {
                        rd2.TenderAmount[rd.TenderType] += rd.DiscPrice * rd.Count;
                    }
                    else
                    {
                        rd2.TenderAmount.Add(rd.TenderType, rd.DiscPrice * rd.Count);
                    }
                }
                //var TndrsToGo = new Dictionary<long, Payment>();
                foreach (var p in ToGoOrdersModelSingleton.Instance.Orders.Select(b => b.PaymentType).Distinct())
                {
                    if (p == null) continue;
                    Payment op = new Payment();
                    if (!Tndrs.TryGetValue(p.Id, out op))
                    {
                        Tndrs.Add(p.Id, p);
                    }
                }

                Tndrs.Add(999, new Payment() { Id = 999, Name = "Без оплаты" });




                Worksheet Ws = AddWs("Блюда по видам оплаты");
                Ws.get_Range("A1:M1000").Font.Name = "Antica";
                Ws.get_Range("A1:M7").Font.Name = "Helica";
                Ws.get_Range("A1:M1000").Font.Size = 10;

                Ws.Cells[1, 5] = "UCS R-Keeper Reports";
                Ws.Cells[1, 6] = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                (Ws.Cells[1, 7] as Range).Font.Italic = true;
                (Ws.Cells[1, 7] as Range).Font.Size = 10;
                (Ws.Cells[1, 7] as Range).Font.Underline = true;
                Ws.Cells[1, 7] = "Галерея";



                (Ws.Cells[3, 6] as Range).Font.Bold = true;
                (Ws.Cells[3, 6] as Range).Font.Italic = true;
                (Ws.Cells[3, 6] as Range).Font.Size = 16;
                (Ws.Cells[3, 6] as Range).Font.Name = "Helica";

                Ws.Cells[3, 6] = "Блюда по видам оплаты";
                Ws.Cells[6, 4] = "период:";
                (Ws.Cells[6, 6] as Range).Font.Bold = true;
                (Ws.Cells[6, 6] as Range).Font.Underline = true;
                (Ws.Cells[6, 6] as Range).Font.Size = 10;
                (Ws.Cells[6, 6] as Range).Font.Name = "Helica";
                Ws.Cells[6, 6] = StartDate.ToString("dd.MM.yyyy") + " - " + StopDate.ToString("dd.MM.yyyy");
                Ws.Cells[6, 7] = "вкл.";

                Ws.get_Range("A8:P8").Font.Bold = true;
                Ws.Cells[8, 2] = "код";
                Ws.Cells[8, 3] = "название";
                Ws.Cells[8, 3] = "количество";


                (Ws.Cells[9, 2] as Range).Font.Bold = true;
                (Ws.Cells[9, 2] as Range).Font.Italic = true;
                (Ws.Cells[9, 2] as Range).Font.Underline = true;
                (Ws.Cells[9, 2] as Range).Font.Size = 11;
                (Ws.Cells[9, 2] as Range).Font.Name = "Helica";
                Ws.Cells[9, 2] = "Самолеты";

                int TndrColStart = 6;

                for (int colN = TndrColStart; colN < TndrColStart + Tndrs.Count; colN++)
                {
                    long k = Tndrs.Keys.ToList()[colN - TndrColStart];
                    Ws.Cells[8, colN] = Tndrs[k].Name;
                }

                int Row = 10;


                List<string> Groups = ConsolidTmp.Select(a => a.GroupeName).Distinct().ToList();
                Groups.Sort();


                decimal[] AllSums = new decimal[Tndrs.Count];
                foreach (string Gr in Groups)
                {
                    Ws.Cells[Row, 3] = Gr;
                    Range c3 = Ws.Cells[Row, 3];
                    Ws.get_Range(c3, c3).Font.Bold = true;

                    decimal[] GrSums = new decimal[Tndrs.Count];
                    Row++;

                    foreach (ReportDish rd in ConsolidTmp.Where(a => a.GroupeName == Gr).OrderBy(a => a.Name))
                    {

                        Ws.Cells[Row, 2] = rd.BarCode.ToString();
                        Ws.Cells[Row, 3] = rd.Name;
                        Ws.Cells[Row, 4] = rd.Count;
                        for (int colN = TndrColStart; colN < TndrColStart + Tndrs.Count; colN++)
                        {
                            decimal PayVal = 0;
                            long k = Tndrs.Keys.ToList()[colN - TndrColStart];
                            rd.TenderAmount.TryGetValue(Tndrs[k].Id, out PayVal);
                            Ws.Cells[Row, colN] = PayVal;
                            GrSums[colN - TndrColStart] += PayVal;
                        }
                        Row++;
                    }
                    Range c4 = Ws.Cells[Row, 1];
                    c3 = Ws.Cells[Row, 50];
                    Ws.get_Range(c4, c3).Font.Bold = true;

                    Ws.Cells[Row, 3] = $"Итого {Gr}";
                    Ws.Cells[Row, 4] = ConsolidTmp.Where(a => a.GroupeName == Gr).Sum(a => a.Count);
                    for (int colN = 0; colN < Tndrs.Count; colN++)
                    {
                        Ws.Cells[Row, colN + TndrColStart] = GrSums[colN];
                        AllSums[colN] += GrSums[colN];
                    }
                    Ws.Cells[Row, TndrColStart + Tndrs.Count] = GrSums.Sum();



                    Row++;
                    Row++;
                }
                //Ws.Cells[Row-1, 3] = "Итого";

                for (int colN = 0; colN < Tndrs.Count; colN++)
                {
                    Ws.Cells[Row, colN + TndrColStart] = AllSums[colN];
                }

                Range c1 = Ws.Cells[Row, 1];
                Range c2 = Ws.Cells[Row, 50];
                Ws.get_Range(c1, c2).Font.Bold = true;
                Ws.Cells[Row, 3] = $"Итого самолеты";
                Ws.Cells[Row, TndrColStart + Tndrs.Count] = AllSums.Sum();



                for (int colN = TndrColStart; colN <= TndrColStart + Tndrs.Count; colN++)
                {

                    (Ws.Cells[Row - 1, colN] as Range).Font.Bold = true;
                    (Ws.Cells[Row - 1, colN] as Range).Font.Size = 12;
                }


                 (Ws.Cells[Row - 1, 3] as Range).Font.Bold = true;
                (Ws.Cells[Row - 1, 3] as Range).Font.Size = 12;
                Row++;
                Row++;



                (Ws.Cells[Row, 2] as Range).Font.Bold = true;
                (Ws.Cells[Row, 2] as Range).Font.Italic = true;
                (Ws.Cells[Row, 2] as Range).Font.Underline = true;
                (Ws.Cells[Row, 2] as Range).Font.Size = 11;
                (Ws.Cells[Row, 2] as Range).Font.Name = "Helica";
                Ws.Cells[Row, 2] = "ToGo";

                //TndrColStart = 5;
                List<string> GroupsToGo = ConsolidTmpToGo.Select(a => a.GroupeName).Distinct().ToList();
                GroupsToGo.Sort();
                decimal[] AllSumsToGo = new decimal[Tndrs.Count];
                Row++;

                foreach (string Gr in GroupsToGo)
                {
                    Ws.Cells[Row, 3] = Gr;
                    Range c3 = Ws.Cells[Row, 3];
                    Ws.get_Range(c3, c3).Font.Bold = true;

                    decimal[] GrSumsToGo = new decimal[Tndrs.Count];
                    Row++;

                    foreach (ReportDish rd in ConsolidTmpToGo.Where(a => a.GroupeName == Gr).OrderBy(a => a.Name))
                    {

                        Ws.Cells[Row, 2] = rd.BarCode.ToString();
                        Ws.Cells[Row, 3] = rd.Name;
                        Ws.Cells[Row, 4] = rd.Count;
                        for (int colN = TndrColStart; colN < TndrColStart + Tndrs.Count; colN++)
                        {
                            decimal PayVal = 0;
                            long k = Tndrs.Keys.ToList()[colN - TndrColStart];
                            rd.TenderAmount.TryGetValue(Tndrs[k].Id, out PayVal);
                            Ws.Cells[Row, colN] = PayVal;
                            GrSumsToGo[colN - TndrColStart] += PayVal;
                        }
                        Row++;
                    }
                    Range c4 = Ws.Cells[Row, 1];
                    c3 = Ws.Cells[Row, 50];
                    Ws.get_Range(c4, c3).Font.Bold = true;

                    Ws.Cells[Row, 3] = $"Итого {Gr}";
                    Ws.Cells[Row, 4] = ConsolidTmpToGo.Where(a => a.GroupeName == Gr).Count();
                    for (int colN = 0; colN < Tndrs.Count; colN++)
                    {
                        Ws.Cells[Row, colN + TndrColStart] = GrSumsToGo[colN];
                        AllSumsToGo[colN] += GrSumsToGo[colN];
                    }
                    Ws.Cells[Row, TndrColStart + Tndrs.Count] = GrSumsToGo.Sum();



                    Row++;
                    Row++;
                }
                //Ws.Cells[Row-1, 3] = "Итого";

                for (int colN = 0; colN < Tndrs.Count; colN++)
                {
                    Ws.Cells[Row, colN + TndrColStart] = AllSumsToGo[colN];
                }

                //Ws.Cells[Row, 3] = $"Доставка";
                //Ws.Cells[Row, TndrColStart + Tndrs.Count]=;
                //  Row++;
                c1 = Ws.Cells[Row, 1];
                c2 = Ws.Cells[Row, 50];
                Ws.get_Range(c1, c2).Font.Bold = true;
                Ws.Cells[Row, 3] = $"Итого ToGo";
                Ws.Cells[Row, TndrColStart + Tndrs.Count] = AllSumsToGo.Sum();


                for (int colN = TndrColStart; colN < TndrColStart + Tndrs.Count; colN++)
                {

                    (Ws.Cells[Row - 1, colN] as Range).Font.Bold = true;
                    (Ws.Cells[Row - 1, colN] as Range).Font.Size = 12;
                }


          (Ws.Cells[Row - 1, 3] as Range).Font.Bold = true;
                (Ws.Cells[Row - 1, 3] as Range).Font.Size = 12;
                Row++;


                c1 = Ws.Cells[Row, 1];
                c2 = Ws.Cells[Row, 50];
                Ws.get_Range(c1, c2).Font.Bold = true;
                Ws.Cells[Row, 3] = $"Итого";
                Ws.Cells[Row, TndrColStart + Tndrs.Count] = AllSumsToGo.Sum() + AllSums.Sum();


            }
            catch (Exception e)
            {
                logger.Error("DBFGetDishezByPayment " + e.Message);
            }
        }


        public static List<ReportDish> SQLGetRashDishez(DateTime StartDate, DateTime StopDate)
        {
            var Tmp = new List<ReportDish>();

            foreach (var ord in AirOrdersModelSingleton.Instance.Orders.Where(a => a.DeliveryDate >= StartDate && a.DeliveryDate < StopDate))
            {
                foreach (var dp in ord.DishPackages.Where(a=> a.DeletedStatus!=2))
                {
                    ReportDish RD = new ReportDish()
                    {
                        Id = dp.Dish.Id,
                        BarCode = dp.Dish.Barcode,
                        DiscPrice = dp.TotalPrice,
                        Price = dp.TotalPrice,
                        Count = dp.Amount,

                    };

                    if (dp.Dish.DishLogicGroupId == null)
                    {
                        RD.GroupeName = "Без категории";
                    }
                    else
                    {
                        RD.GroupeName = dp.Dish.DishLogicGroup.Name;
                    }

                    if (dp.Deleted && dp.DeletedStatus == 1 && dp.SpisPaymentId!=0)
                    {
                        try { RD.TenderType = dp.SpisPaymentId; } catch { }
                    }
                    else
                    {
                        try { RD.TenderType = ord.AirCompany.PaymentId.GetValueOrDefault(); } catch { }
                    }

                    RD.DiscPrice = RD.DiscPrice * (ord.OrderSumm == 0 ? 1 : (ord.OrderTotalSumm / ord.OrderSumm));
                   
                    RD.Name = dp.Dish.Name;

                    Tmp.Add(RD);
                }


                if (ord.ExtraChargeSumm > 0)
                {
                    ReportDish RD = new ReportDish()
                    {
                        Id = 999998,
                        BarCode = 999998,
                        DiscPrice = ord.ExtraChargeSumm,
                        Price = ord.ExtraChargeSumm,
                        Count = 1,
                        GroupeName = DataExtension.DataCatalogsSingleton.Instance.DishLogicGroupData.Data.FirstOrDefault(a => a.Id == MainClass.DopLogikCatId).Name,
                    };

                    try { RD.TenderType = ord.AirCompany.PaymentId.GetValueOrDefault(); } catch { }

                    RD.Name = "Наценка за срочность";

                    Tmp.Add(RD);
                }



            }
            return Tmp;
        }


        public static List<ReportDish> SQLGetRashDishezToGo(DateTime StartDate, DateTime StopDate)
        {
            var Tmp = new List<ReportDish>();

            foreach (var ord in ToGoOrdersModelSingleton.Instance.Orders.Where(a => a.DeliveryDate >= StartDate && a.DeliveryDate < StopDate
            //&& a.OrderStatus == OrderStatus.Closed
            ))
            {
                foreach (var dp in ord.DishPackages.Where(a=>a.DeletedStatus!=2))
                {
                    ReportDish RD = new ReportDish()
                    {
                        Id = dp.Dish.Id,
                        BarCode = dp.Dish.Barcode,
                        DiscPrice = dp.TotalPrice,
                        Price = dp.TotalPrice,
                        Count = dp.Amount,
                    };

                    if (dp.Dish.DishLogicGroupId == null)
                    {
                        RD.GroupeName = "Без категории";
                    }
                    else
                    {
                        RD.GroupeName = dp.Dish.DishLogicGroup.Name;
                    }


                    if (dp.Deleted && dp.DeletedStatus == 1 && dp.SpisPaymentId != 0)
                    {
                        try { RD.TenderType = dp.SpisPaymentId; } catch { }
                    }
                    else
                    {
                        RD.TenderType = (ord.PaymentId == null ? 999 : ord.PaymentId.GetValueOrDefault());
                    }
                    //ord.PaymentId.GetValueOrDefault(); 

                    if (ord.OrderDishesSumm == 0)
                    {
                        RD.DiscPrice = 0;
                    }
                    else
                    {
                        RD.DiscPrice = RD.DiscPrice * ((ord.OrderDishesSumm - ord.DiscountSumm) / ord.OrderDishesSumm);
                    }

                    RD.Name = dp.Dish.Name;

                    Tmp.Add(RD);
                }
                if (ord.DeliveryPrice > 0)
                {
                    ReportDish RD = new ReportDish()
                    {
                        Id = 999999,
                        BarCode = 999999,
                        DiscPrice = ord.DeliveryPrice,
                        Price = ord.DeliveryPrice,
                        Count = 1,
                        GroupeName = DataExtension.DataCatalogsSingleton.Instance.DishLogicGroupData.Data.FirstOrDefault(a => a.Id == MainClass.DopLogikCatId).Name,
                    };

                    RD.TenderType = (ord.PaymentId == null ? 999 : ord.PaymentId.GetValueOrDefault());
                    //ord.PaymentId.GetValueOrDefault(); 

                    /*
                    if (ord.OrderDishesSumm == 0)
                    {
                        RD.DiscPrice = 0;
                    }
                    else
                    {
                        RD.DiscPrice = RD.DiscPrice * ((ord.OrderDishesSumm - ord.DiscountSumm) / ord.OrderDishesSumm);
                    }
                    */
                    RD.Name = "Доставка";

                    Tmp.Add(RD);
                }
            }
            return Tmp;
        }

        public static List<ReportTNDR> SQLGetRepTNDRs(DateTime StartDate, DateTime StopDate)
        {
            List<ReportTNDR> Tmp = new List<ReportTNDR>();
            foreach (var Ord in AirOrdersModelSingleton.Instance.Orders.Where(a => a.DeliveryDate >= StartDate && a.DeliveryDate < StopDate))
            {
                ReportTNDR RD = new ReportTNDR()
                {
                    Id = Ord.AirCompany.PaymentId.GetValueOrDefault(),
                    Summ = Ord.OrderTotalSumm,
                    dt = Ord.DeliveryDate,
                    Name = Ord.AirCompany.PaymentType.Name,
                    Cash = Ord.AirCompany.PaymentType.IsCash


                };

                Tmp.Add(RD);

            }
            return Tmp;


        }

        public static List<ReportTNDR> SQLGetRepToGoTNDRs(DateTime StartDate, DateTime StopDate)
        {
            List<ReportTNDR> Tmp = new List<ReportTNDR>();
            foreach (var Ord in ToGoOrdersModelSingleton.Instance.Orders.Where(a => a.DeliveryDate >= StartDate && a.DeliveryDate < StopDate))
            {
                if (Ord.PaymentType == null)
                {
                    ReportTNDR RD = new ReportTNDR()
                    {
                        Id = 999,
                        Summ = Ord.OrderTotalSumm,
                        dt = Ord.DeliveryDate,
                        Name = "Без оплаты",
                        Cash = false
                    };

                    Tmp.Add(RD);
                }
                else
                {
                    ReportTNDR RD = new ReportTNDR()
                    {
                        Id = Ord.PaymentId.GetValueOrDefault(),
                        Summ = Ord.OrderTotalSumm,
                        dt = Ord.DeliveryDate,
                        Name = Ord.PaymentType.Name,
                        Cash = Ord.PaymentType.IsCash
                    };

                    Tmp.Add(RD);
                }

            }
            return Tmp;


        }
        internal void DBFGetSalesByDays(DateTime StartDate, DateTime StopDate)
        {
            try
            {
                OpenXls();
                List<ReportTNDR> Tmp = SQLGetRepTNDRs(StartDate, StopDate.AddDays(1));
                List<ReportTNDR> TmpToGo = SQLGetRepToGoTNDRs(StartDate, StopDate.AddDays(1));

                //List<ReportTNDR> ConsolidTmp = new List<ReportTNDR>();

                Worksheet Ws = AddWs("Выручка станций по дням");
                Ws.get_Range("A1:M1000").Font.Name = "Antica";
                Ws.get_Range("A1:M7").Font.Name = "Helica";
                Ws.get_Range("A1:M1000").Font.Size = 10;

                Ws.Cells[1, 5] = "UCS R-Keeper Reports";
                Ws.Cells[1, 6] = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                (Ws.Cells[1, 7] as Range).Font.Italic = true;
                (Ws.Cells[1, 7] as Range).Font.Size = 10;
                (Ws.Cells[1, 7] as Range).Font.Underline = true;
                Ws.Cells[1, 7] = "Галерея";

                (Ws.Cells[3, 8] as Range).Font.Bold = false;
                (Ws.Cells[3, 8] as Range).Font.Italic = true;
                (Ws.Cells[3, 8] as Range).Font.Size = 16;
                (Ws.Cells[3, 8] as Range).Font.Name = "Helica";
                (Ws.Cells[3, 8] as Range).HorizontalAlignment = XlHAlign.xlHAlignCenter;

                Ws.Cells[3, 8] = "Выручка станций по дням";
                Ws.Cells[5, 4] = "период:";
                (Ws.Cells[5, 8] as Range).Font.Bold = true;
                (Ws.Cells[5, 8] as Range).Font.Underline = true;
                (Ws.Cells[5, 8] as Range).Font.Size = 10;
                (Ws.Cells[5, 8] as Range).Font.Name = "Helica";
                Ws.Cells[5, 6] = StartDate.ToString("dd.MM.yyyy") + " - " + StopDate.ToString("dd.MM.yyyy");
                Ws.Cells[5, 7] = "вкл.";


                Ws.get_Range("A7:P8").Font.Bold = true;
                Ws.get_Range("A7:P8").HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[7, 1] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;
                (Ws.Cells[7, 2] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[7, 2] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;

                Ws.Cells[7, 2] = "код";
                Ws.Cells[7, 3] = "название";
                Ws.Cells[7, 9] = "Всего";


                int Row = 9;

                //List<int> Vals;

                for (DateTime mdt = StartDate; mdt <= StopDate; mdt = mdt.AddDays(1))
                {
                    List<ReportTNDR> Rtnd = Tmp.Where(a => a.dt.Date == mdt.Date).ToList();
                    List<ReportTNDR> RtndToGo = TmpToGo.Where(a => a.dt.Date == mdt.Date).ToList();

                    if (Rtnd.Count + RtndToGo.Count == 0)
                    {
                        continue;
                    }

                    (Ws.Cells[Row, 3] as Range).Font.Bold = true;
                    Ws.Cells[Row, 3] = mdt.ToString("dd.MM.yyyy");


                    Row++;
                    foreach (int rdId in Rtnd.Select(a => a.Id).Distinct().OrderBy(a => a))
                    {
                        List<ReportTNDR> rdList = Rtnd.Where(a => a.Id == rdId).ToList();
                        (Ws.Cells[Row, 2] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                        Ws.Cells[Row, 2] = rdId.ToString();
                        Ws.Cells[Row, 3] = rdList[0].Name;
                        (Ws.Cells[Row, 6] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                        Ws.Cells[Row, 6] = rdList.Sum(a => a.Summ);
                        (Ws.Cells[Row, 9] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                        Ws.Cells[Row, 9] = rdList.Sum(a => a.Summ);
                        Row++;
                    }

                    Ws.Cells[Row, 3] = "Итого самолеты списание";
                    (Ws.Cells[Row, 10] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    Ws.Cells[Row, 10] = Rtnd.Where(c => DataCatalogsSingleton.Instance.PaymentData.Data.Where(a => a.PaymentGroup != null && !a.PaymentGroup.Sale).Select(b => b.Id).Contains(c.Id)).Sum(a => a.Summ);
                    Row++;
                    Ws.Cells[Row, 3] = "Итого самолеты выручка";
                    (Ws.Cells[Row, 11] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    Ws.Cells[Row, 11] = Rtnd.Where(c => DataCatalogsSingleton.Instance.PaymentData.Data.Where(a => a.PaymentGroup != null && a.PaymentGroup.Sale).Select(b => b.Id).Contains(c.Id)).Sum(a => a.Summ);
                    Row++;
                    Ws.Cells[Row, 3] = "Итого самолеты (SH)";
                    (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    Ws.Cells[Row, 12] = Rtnd.Where(c => DataCatalogsSingleton.Instance.PaymentData.Data.Where(a => a.PaymentGroup != null).Select(b => b.Id).Contains(c.Id)).Sum(a => a.Summ);
                    Row++;


                    foreach (int rdId in RtndToGo.Select(a => a.Id).Distinct().OrderBy(a => a))
                    {


                        List<ReportTNDR> rdListToGo = RtndToGo.Where(a => a.Id == rdId).ToList();
                        (Ws.Cells[Row, 2] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                        Ws.Cells[Row, 2] = rdId.ToString();
                        Ws.Cells[Row, 3] = rdListToGo[0].Name;
                        (Ws.Cells[Row, 6] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                        Ws.Cells[Row, 6] = rdListToGo.Sum(a => a.Summ);
                        (Ws.Cells[Row, 9] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                        Ws.Cells[Row, 9] = rdListToGo.Sum(a => a.Summ);
                        Row++;

                    }
                    Ws.Cells[Row, 3] = "Итого ToGo списание";
                    (Ws.Cells[Row, 10] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    Ws.Cells[Row, 10] = RtndToGo.Where(c => DataCatalogsSingleton.Instance.PaymentData.Data.Where(a => a.PaymentGroup != null && !a.PaymentGroup.Sale).Select(b => b.Id).Contains(c.Id)).Sum(a => a.Summ);
                    Row++;
                    Ws.Cells[Row, 3] = "Итого ToGo выручка";
                    (Ws.Cells[Row, 11] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    Ws.Cells[Row, 11] = RtndToGo.Where(c => DataCatalogsSingleton.Instance.PaymentData.Data.Where(a => a.PaymentGroup != null && a.PaymentGroup.Sale).Select(b => b.Id).Contains(c.Id)).Sum(a => a.Summ);
                    Row++;
                    Ws.Cells[Row, 3] = "Итого ToGo (включает незакрытые)";
                    (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    Ws.Cells[Row, 12] = RtndToGo.Sum(a => a.Summ);
                    Row++;

                    Ws.Cells[Row, 3] = "Итого ";
                    (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    Ws.Cells[Row, 12] = RtndToGo.Sum(a => a.Summ) + Rtnd.Sum(a => a.Summ);
                    Row++;

                    Row++;

                }
                Row++;
                Row++;
                Ws.Cells[Row, 3] = "Итого самолеты списание";
                (Ws.Cells[Row, 3] as Range).Font.Bold = true;
                (Ws.Cells[Row, 10] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 10] as Range).Font.Bold = true;
                Ws.Cells[Row, 10] = Tmp.Where(c => DataCatalogsSingleton.Instance.PaymentData.Data.Where(a => a.PaymentGroup != null && !a.PaymentGroup.Sale).Select(b => b.Id).Contains(c.Id)).Sum(a => a.Summ);
                Row++;

                Ws.Cells[Row, 3] = "Итого самолеты выручка";
                (Ws.Cells[Row, 3] as Range).Font.Bold = true;
                (Ws.Cells[Row, 11] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 11] as Range).Font.Bold = true;
                Ws.Cells[Row, 11] = Tmp.Where(c => DataCatalogsSingleton.Instance.PaymentData.Data.Where(a => a.PaymentGroup != null && a.PaymentGroup.Sale).Select(b => b.Id).Contains(c.Id)).Sum(a => a.Summ);
                Row++;
                Ws.Cells[Row, 3] = "Итого самолеты (SH)";
                (Ws.Cells[Row, 3] as Range).Font.Bold = true;
                (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 12] as Range).Font.Bold = true;
                Ws.Cells[Row, 12] = Tmp.Sum(a => a.Summ);
                Row++;
                Ws.Cells[Row, 3] = "Итого ToGo списание";
                (Ws.Cells[Row, 3] as Range).Font.Bold = true;
                (Ws.Cells[Row, 10] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 10] as Range).Font.Bold = true;
                Ws.Cells[Row, 10] = TmpToGo.Where(c => DataCatalogsSingleton.Instance.PaymentData.Data.Where(a => a.PaymentGroup != null && !a.PaymentGroup.Sale).Select(b => b.Id).Contains(c.Id)).Sum(a => a.Summ);
                Row++;
                Ws.Cells[Row, 3] = "Итого ToGo выручка";
                (Ws.Cells[Row, 3] as Range).Font.Bold = true;
                (Ws.Cells[Row, 11] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 11] as Range).Font.Bold = true;
                Ws.Cells[Row, 11] = TmpToGo.Where(c => DataCatalogsSingleton.Instance.PaymentData.Data.Where(a => a.PaymentGroup != null && a.PaymentGroup.Sale).Select(b => b.Id).Contains(c.Id)).Sum(a => a.Summ);
                Row++;
                Ws.Cells[Row, 3] = "Итого ToGo ";
                (Ws.Cells[Row, 3] as Range).Font.Bold = true;
                (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 12] as Range).Font.Bold = true;
                Ws.Cells[Row, 12] = TmpToGo.Sum(a => a.Summ);
                Row++;

                Ws.Cells[Row, 3] = "Итого ";
                (Ws.Cells[Row, 3] as Range).Font.Bold = true;
                (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 12] as Range).Font.Bold = true;
                Ws.Cells[Row, 12] = Tmp.Sum(a => a.Summ) + TmpToGo.Sum(a => a.Summ);
                Row++;
            }
            catch
            { }
            //app.Visible = true;

        }
        internal void DBFGetSalesByCats(DateTime StartDate, DateTime StopDate)
        {
            try
            {
                OpenXls();
                List<ReportTNDR> Tmp = SQLGetRepTNDRs(StartDate, StopDate.AddDays(1));
                List<ReportTNDR> TmpToGo = SQLGetRepToGoTNDRs(StartDate, StopDate.AddDays(1));

                //List<ReportTNDR> ConsolidTmp = new List<ReportTNDR>();

                Worksheet Ws = AddWs("Выручка по категориям");
                Ws.get_Range("A1:M1000").Font.Name = "Antica";
                Ws.get_Range("A1:M7").Font.Name = "Helica";
                Ws.get_Range("A1:M1000").Font.Size = 10;

                Ws.Cells[1, 5] = "UCS R-Keeper Reports";
                Ws.Cells[1, 6] = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                (Ws.Cells[1, 7] as Range).Font.Italic = true;
                (Ws.Cells[1, 7] as Range).Font.Size = 10;
                (Ws.Cells[1, 7] as Range).Font.Underline = true;
                Ws.Cells[1, 7] = "Галерея";



                (Ws.Cells[3, 6] as Range).Font.Bold = false;
                (Ws.Cells[3, 6] as Range).Font.Italic = true;
                (Ws.Cells[3, 6] as Range).Font.Size = 16;
                (Ws.Cells[3, 6] as Range).Font.Name = "Helica";
                (Ws.Cells[3, 6] as Range).HorizontalAlignment = XlHAlign.xlHAlignCenter;

                Ws.Cells[3, 6] = "Выручка по категориям";
                Ws.Cells[5, 4] = "период:";
                (Ws.Cells[5, 6] as Range).Font.Bold = true;
                (Ws.Cells[5, 6] as Range).Font.Underline = true;
                (Ws.Cells[5, 6] as Range).Font.Size = 10;
                (Ws.Cells[5, 6] as Range).Font.Name = "Helica";
                Ws.Cells[5, 6] = StartDate.ToString("dd.MM.yyyy") + " - " + StopDate.ToString("dd.MM.yyyy");
                Ws.Cells[5, 7] = "вкл.";


                Ws.get_Range("A7:P8").Font.Bold = true;
                Ws.get_Range("A7:P8").HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[7, 4] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;



                Ws.Cells[7, 3] = "код";
                Ws.Cells[7, 4] = "название";
                Ws.Cells[7, 8] = "сумма";
                Ws.Cells[7, 10] = "сумма в баз.";



                int Row = 9;

                List<int> Vals;

                foreach (int rdId in Tmp.Select(a => a.Id).Distinct().Concat(TmpToGo.Select(a => a.Id).Distinct()))
                {
                    List<ReportTNDR> rdList = Tmp.Where(a => a.Id == rdId).ToList();
                    List<ReportTNDR> rdToGoList = TmpToGo.Where(a => a.Id == rdId).ToList();

                    (Ws.Cells[Row, 3] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    (Ws.Cells[Row, 3] as Range).Font.Bold = true;
                    Ws.Cells[Row, 3] = rdId.ToString();
                    (Ws.Cells[Row, 4] as Range).Font.Bold = true;
                    Ws.Cells[Row, 4] = rdList[0].Name;

                    Row = Row + 2;
                    Ws.Cells[Row, 3] = "Самолеты";
                    (Ws.Cells[Row, 8] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    Ws.Cells[Row, 8] = rdList.Sum(a => a.Summ);
                    (Ws.Cells[Row, 10] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    Ws.Cells[Row, 10] = rdList.Sum(a => a.Summ);

                    Row = Row + 2;
                    Ws.Cells[Row, 3] = "ToGo";
                    (Ws.Cells[Row, 8] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    Ws.Cells[Row, 8] = rdToGoList.Sum(a => a.Summ);
                    (Ws.Cells[Row, 10] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    Ws.Cells[Row, 10] = rdToGoList.Sum(a => a.Summ);

                    Row = Row + 2;

                    (Ws.Cells[Row, 6] as Range).Font.Bold = true;
                    (Ws.Cells[Row, 6] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    Ws.Cells[Row, 6] = "Всего";
                    (Ws.Cells[Row, 10] as Range).Font.Bold = true;
                    (Ws.Cells[Row, 10] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    Ws.Cells[Row, 10] = rdList.Sum(a => a.Summ) + rdToGoList.Sum(a => a.Summ);

                    Row++;


                }


                // Ws.get_Range(Ws.Cells[Row, 1], Ws.Cells[Row, 20]).Font.Bold = true;

                //  app.Visible = true;
            }
            catch
            { }
        }


        private List<ReportDish> GetSaleDishesByPaymentToGo(long pId, DateTime StartDate, DateTime StopDate)
        {
            List<ReportDish> Tmp = new List<ReportDish>();


            var ordrsToGo = ToGoOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.PaymentId == pId && a.DeliveryDate >= StartDate && a.DeliveryDate < StopDate.AddDays(1));
            foreach (var or in ordrsToGo)
            {
                try
                {
                    if (or.DishPackages == null) continue;
                    foreach (var d in or.DishPackages)
                    {
                        try
                        {
                            ReportDish rd;
                            if (!Tmp.Any(a => a.Id == d.DishId))
                            {
                                rd = new ReportDish()
                                {
                                    Id = d.DishId,
                                    BarCode = d.Dish.Barcode,
                                    Name = DataExtension.DataCatalogsSingleton.Instance.DishData.Data.SingleOrDefault(a => a.Id == d.DishId).Name

                                };
                                Tmp.Add(rd);
                            }
                            rd = Tmp.FirstOrDefault(a => a.Id == d.DishId);
                            rd.Count += d.Amount;
                            rd.Price += d.TotalSumm;
                        }
                        catch (Exception ee)
                        {
                            logger.Error($"GetSaleDishesByPaymentToGo  or.DishPackages error " + ee.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error($"GetSaleDishesByPaymentToGo  ordrsToGo error " + e.Message);
                }
            }

            return Tmp;

        }



        private List<ReportDish> GetSaleDishesByPayment(long airId, DateTime StartDate, DateTime StopDate)
        {
            List<ReportDish> Tmp = new List<ReportDish>();
            var ordrs = AirOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.AirCompany.Id == airId && a.DeliveryDate >= StartDate && a.DeliveryDate < StopDate.AddDays(1));
            foreach (var or in ordrs)
            {
                foreach (var d in or.DishPackages)
                {
                    ReportDish rd;
                    if (!Tmp.Any(a => a.Id == d.DishId))
                    {
                        rd = new ReportDish()
                        {
                            Id = d.DishId,
                            BarCode = d.Code.GetValueOrDefault(),
                            Name = DataExtension.DataCatalogsSingleton.Instance.DishData.Data.SingleOrDefault(a => a.Id == d.DishId).Name

                        };
                        Tmp.Add(rd);
                    }
                    rd = Tmp.FirstOrDefault(a => a.Id == d.DishId);
                    rd.Count += d.Amount;
                    rd.Price += d.TotalSumm;
                }

            }
            /*
            var ordrsToGo = ToGoOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.PaymentId == pId);
            foreach (var or in ordrsToGo)
            {
                foreach (var d in or.DishPackages)
                {
                    ReportDish rd;
                    if (Tmp.Any(a => a.Id == d.Id))
                    {
                        rd = new ReportDish()
                        {
                            Id = d.Id,
                            BarCode = d.Code.GetValueOrDefault(),
                            Name = DataExtension.DataCatalogsSingleton.Instance.Dishes.SingleOrDefault(a => a.Id == d.Id).Name

                        };
                        Tmp.Add(rd);
                    }
                    rd = Tmp.FirstOrDefault(a => a.Id == d.Id);
                    rd.Count += d.Amount;
                    rd.Price += d.TotalSumm;
                }

            }
            */
            return Tmp;

        }


        internal void SaleByDayAndDepReport(DateTime StartDate, DateTime StopDate)
        {
            try
            {
                OpenXls();
                Worksheet Ws = AddWs("Отчет по дням");

                Ws.Cells[3, 6] = "Отчет по дням";
                Ws.Cells[5, 4] = "период:";
                (Ws.Cells[5, 6] as Range).Font.Bold = true;
                (Ws.Cells[5, 6] as Range).Font.Underline = true;
                (Ws.Cells[5, 6] as Range).Font.Size = 10;
                (Ws.Cells[5, 6] as Range).Font.Name = "Helica";
                Ws.Cells[5, 6] = StartDate.ToString("dd.MM.yyyy") + " - " + StopDate.ToString("dd.MM.yyyy");
                Ws.Cells[5, 7] = "вкл.";
                Ws.Cells[6, 2] = "Выручка";
                Ws.Cells[6, 8] = "Списание";
                int row = 7;
                Ws.Cells[row, 1] = "Дата";
                Ws.Cells[row, 3] = "ToFly";
                Ws.Cells[row, 4] = "Гастрофуд";
                Ws.Cells[row, 5] = "ToGo";
                Ws.Cells[row, 6] = "Шереметьево";
                Ws.Cells[row, 7] = "Итого выручка";

                Ws.Cells[row, 9] = "Списание ToFly";
                Ws.Cells[row, 10] = "Списание ToGo";
                Ws.Cells[row, 11] = "Списание Шереметьево";
                Ws.Cells[row, 12] = "Итого списание";
                Ws.Cells[row, 14] = "Общая сумма";

                row++;

                for (var dt = StartDate; dt <= StopDate; dt = dt.AddDays(1))
                {

                    var saleToFly = AirOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.DeliveryDate >= dt && a.DeliveryDate < dt.AddDays(1)
                            && !DBProvider.SharAirs.Contains(a.AirCompanyId.GetValueOrDefault()) && a.AirCompany.PaymentType.PaymentGroup.Sale
                            && a.AirCompany.Id != MainClass.AirGastroFoodId
                            ).Sum(a => a.OrderTotalSumm);

                    var saleGastro = AirOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.DeliveryDate >= dt && a.DeliveryDate < dt.AddDays(1)

                            && a.AirCompany.Id == MainClass.AirGastroFoodId
                            ).Sum(a => a.OrderTotalSumm);

                    var saleToGo = ToGoOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled &&
                    (a.PaymentType == null || (a.PaymentType != null && a.PaymentType.PaymentGroup.Sale)) && a.DeliveryDate >= dt && a.DeliveryDate < dt.AddDays(1)).Sum(b => b.OrderTotalSumm);

                    /*
                    var saleShar = AirOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.DeliveryDate >= dt && a.DeliveryDate < dt.AddDays(1)
                            && DBProvider.SharAirs.Contains(a.AirCompanyId.GetValueOrDefault()) && a.AirCompany.PaymentType.PaymentGroup.Sale).Sum(a => a.OrderTotalSumm);
                            */

                    var saleShar = AirOrdersModelSingleton.Instance.SVOorders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.DeliveryDate >= dt && a.DeliveryDate < dt.AddDays(1)
                            && DBProvider.SharAirs.Contains(a.AirCompanyId.GetValueOrDefault()) && a.AirCompany.PaymentType.PaymentGroup.Sale).Sum(a => a.OrderTotalSumm);

                    var spisToFly = AirOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.DeliveryDate >= dt && a.DeliveryDate < dt.AddDays(1)
                            && !DBProvider.SharAirs.Contains(a.AirCompanyId.GetValueOrDefault()) &&
                            a.AirCompany.Id != MainClass.AirGastroFoodId &&
                            !a.AirCompany.PaymentType.PaymentGroup.Sale).Sum(a => a.OrderTotalSumm)
                            ;
                    var spisToFlyDeletedDish =
                            AirOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.DeliveryDate >= dt && a.DeliveryDate < dt.AddDays(1)
                            && !DBProvider.SharAirs.Contains(a.AirCompanyId.GetValueOrDefault())).Sum(a => a.GetSpisDishesOfPaimentId(0).Sum(b => b.TotalSumm * (100 - a.DiscountPercent) / 100))
                            ;
                    spisToFly += spisToFlyDeletedDish;
                    var spisToGo = ToGoOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled &&
                    ((a.PaymentType != null && !a.PaymentType.PaymentGroup.Sale)) && a.DeliveryDate >= dt && a.DeliveryDate < dt.AddDays(1)).Sum(b => b.OrderTotalSumm);

                    var spisToGoDeletedDish =
                            ToGoOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.DeliveryDate >= dt && a.DeliveryDate < dt.AddDays(1))
                            .Sum(a => a.GetSpisDishesOfPaimentId(0).Sum(b => b.TotalSumm * (100 - a.DiscountPercent) / 100));
                    spisToGo += spisToGoDeletedDish;


                    /*
                    var spisShar = AirOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.DeliveryDate >= dt && a.DeliveryDate < dt.AddDays(1)
                            && DBProvider.SharAirs.Contains(a.AirCompanyId.GetValueOrDefault()) && !a.AirCompany.PaymentType.PaymentGroup.Sale).Sum(a => a.OrderTotalSumm);
                            */
                    var spisShar = AirOrdersModelSingleton.Instance.SVOorders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.DeliveryDate >= dt && a.DeliveryDate < dt.AddDays(1)
                    && DBProvider.SharAirs.Contains(a.AirCompanyId.GetValueOrDefault()) && !a.AirCompany.PaymentType.PaymentGroup.Sale).Sum(a => a.OrderTotalSumm);

                    Ws.Cells[row, 1] = dt.ToString("dd/MM/yyyy");

                    Ws.Cells[row, 3] = saleToFly;
                    Ws.Cells[row, 4] = saleGastro;
                    Ws.Cells[row, 5] = saleToGo;
                    Ws.Cells[row, 6] = saleShar;
                    Ws.Cells[row, 7] = saleShar + saleToGo + saleToFly + saleGastro;

                    Ws.Cells[row, 9] = spisToFly;
                    Ws.Cells[row, 10] = spisToGo;
                    Ws.Cells[row, 11] = spisShar;
                    Ws.Cells[row, 12] = spisToFly + spisToGo + spisShar;

                    Ws.Cells[row, 14] = spisToFly + spisToGo + spisShar + saleShar + saleToGo + saleToFly + saleGastro;


                    row++;
                }
                row++;

                StopDate = StopDate.AddDays(1);
                var saleToFlyA = AirOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.DeliveryDate >= StartDate && a.DeliveryDate < StopDate
                        && !DBProvider.SharAirs.Contains(a.AirCompanyId.GetValueOrDefault())
                        && a.AirCompany.Id != MainClass.AirGastroFoodId
                        && a.AirCompany.PaymentType.PaymentGroup.Sale).Sum(a => a.OrderTotalSumm);
                var saleToGoA = ToGoOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled &&
                (a.PaymentType == null || (a.PaymentType != null && a.PaymentType.PaymentGroup.Sale)) && a.DeliveryDate >= StartDate && a.DeliveryDate < StopDate).Sum(b => b.OrderTotalSumm);

                /*
                var saleShar = AirOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.DeliveryDate >= StartDate && a.DeliveryDate < StopDate
                        && DBProvider.SharAirs.Contains(a.AirCompanyId.GetValueOrDefault()) && a.AirCompany.PaymentType.PaymentGroup.Sale).Sum(a => a.OrderTotalSumm);
                        */

                var saleSharA = AirOrdersModelSingleton.Instance.SVOorders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.DeliveryDate >= StartDate && a.DeliveryDate < StopDate
                        && DBProvider.SharAirs.Contains(a.AirCompanyId.GetValueOrDefault()) && a.AirCompany.PaymentType.PaymentGroup.Sale).Sum(a => a.OrderTotalSumm);

                var spisToFlyA = AirOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.DeliveryDate >= StartDate && a.DeliveryDate < StopDate
                        && !DBProvider.SharAirs.Contains(a.AirCompanyId.GetValueOrDefault()) && !a.AirCompany.PaymentType.PaymentGroup.Sale
                        && a.AirCompany.Id != MainClass.AirGastroFoodId
                        ).Sum(a => a.OrderTotalSumm)
                        +
                        AirOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.DeliveryDate >= StartDate && a.DeliveryDate < StopDate
                        && !DBProvider.SharAirs.Contains(a.AirCompanyId.GetValueOrDefault())).Sum(a => a.GetSpisDishesOfPaimentId(0).Sum(b => b.TotalSumm * (100 - a.DiscountPercent) / 100));


                var saleGastroA = AirOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.DeliveryDate >= StartDate && a.DeliveryDate < StopDate

                       && a.AirCompany.Id == MainClass.AirGastroFoodId
                       ).Sum(a => a.OrderTotalSumm);









                var spisToGoA = ToGoOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled &&
                ((a.PaymentType != null && !a.PaymentType.PaymentGroup.Sale)) && a.DeliveryDate >= StartDate && a.DeliveryDate < StopDate).Sum(b => b.OrderTotalSumm);


                var spisToGoDeletedDishA =
                            ToGoOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.DeliveryDate >= StartDate && a.DeliveryDate < StopDate)
                            .Sum(a => a.GetSpisDishesOfPaimentId(0).Sum(b => b.TotalSumm * (100 - a.DiscountPercent) / 100));
                spisToGoA += spisToGoDeletedDishA;

                /*
                var spisShar = AirOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.DeliveryDate >= StartDate && a.DeliveryDate < StopDate
                        && DBProvider.SharAirs.Contains(a.AirCompanyId.GetValueOrDefault()) && !a.AirCompany.PaymentType.PaymentGroup.Sale).Sum(a => a.OrderTotalSumm);
                        */
                var spisSharA = AirOrdersModelSingleton.Instance.SVOorders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.DeliveryDate >= StartDate && a.DeliveryDate < StopDate
                && DBProvider.SharAirs.Contains(a.AirCompanyId.GetValueOrDefault()) && !a.AirCompany.PaymentType.PaymentGroup.Sale).Sum(a => a.OrderTotalSumm);

                Ws.Cells[row, 1] = "Итого";

                Ws.Cells[row, 3] = saleToFlyA;
                Ws.Cells[row, 4] = saleGastroA;
                Ws.Cells[row, 5] = saleToGoA;
                Ws.Cells[row, 6] = saleSharA;
                Ws.Cells[row, 7] = saleSharA + saleToGoA + saleToFlyA + saleGastroA;

                Ws.Cells[row, 9] = spisToFlyA;
                Ws.Cells[row, 10] = spisToGoA;
                Ws.Cells[row, 11] = spisSharA;
                Ws.Cells[row, 12] = spisToFlyA + spisToGoA + spisSharA;

                Ws.Cells[row, 14] = spisToFlyA + spisToGoA + spisSharA + saleSharA + saleToGoA + saleToFlyA + saleGastroA;

                for (int i = 1; i < 15; i++)
                {
                    Ws.Columns[i].AutoFit();
                }
            }
            catch
            {

            }
        }

        internal void DBFGetVoids2(DateTime StartDate, DateTime StopDate)
        {
            try
            {
                OpenXls();
                //List<ReportTNDR> Tmp = SQLGetRepTNDRs(StartDate, StopDate.AddDays(1));

                //  List<ReportTNDR> ConsolidTmp = new List<ReportTNDR>();

                Worksheet Ws = AddWs("Отчет по отказам");
                Ws.get_Range("A1:M1000").Font.Name = "Antica";
                Ws.get_Range("A1:M7").Font.Name = "Helica";
                Ws.get_Range("A1:M1000").Font.Size = 10;

                Ws.Cells[1, 5] = "UCS R-Keeper Reports";
                Ws.Cells[1, 6] = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                (Ws.Cells[1, 7] as Range).Font.Italic = true;
                (Ws.Cells[1, 7] as Range).Font.Size = 10;
                (Ws.Cells[1, 7] as Range).Font.Underline = true;
                Ws.Cells[1, 7] = "Галерея";



                (Ws.Cells[3, 6] as Range).Font.Bold = false;
                (Ws.Cells[3, 6] as Range).Font.Italic = true;
                (Ws.Cells[3, 6] as Range).Font.Size = 16;
                (Ws.Cells[3, 6] as Range).Font.Name = "Helica";

                Ws.Cells[3, 6] = "Отчет по отказам";
                Ws.Cells[5, 4] = "период:";
                (Ws.Cells[5, 6] as Range).Font.Bold = true;
                (Ws.Cells[5, 6] as Range).Font.Underline = true;
                (Ws.Cells[5, 6] as Range).Font.Size = 10;
                (Ws.Cells[5, 6] as Range).Font.Name = "Helica";
                Ws.Cells[5, 6] = StartDate.ToString("dd.MM.yyyy") + " - " + StopDate.ToString("dd.MM.yyyy");
                Ws.Cells[5, 7] = "вкл.";


                Ws.get_Range("A7:P8").Font.Bold = true;
                Ws.get_Range("A7:P8").HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[7, 1] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;
                (Ws.Cells[7, 2] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;



                Ws.Cells[7, 3] = "код";
                Ws.Cells[7, 4] = "название";
                Ws.Cells[7, 7] = "количество";
                Ws.Cells[7, 9] = "сумма";
                Ws.Cells[7, 11] = "ср. цена";

                Range r = Ws.Range["J1", System.Type.Missing];
                r.EntireColumn.ColumnWidth = 20;
                r = Ws.Range["L1", System.Type.Missing];
                r.EntireColumn.ColumnWidth = 20;
                r = Ws.Range["H1", System.Type.Missing];
                r.EntireColumn.ColumnWidth = 20;

                int Row = 9;
                decimal allSumm = 0;
                decimal allCount = 0;
                foreach (var pg in DataExtension.DataCatalogsSingleton.Instance.PaymentGroupData.Data.Where(a => !a.Sale))
                {

                    foreach (var p in DataExtension.DataCatalogsSingleton.Instance.PaymentData.Data.Where(a => a.PaymentGroupId == pg.Id))
                    {
                        if (!p.ToGo)
                        {
                            foreach (var air in DataExtension.DataCatalogsSingleton.Instance.AirCompanyData.Data.Where(a => a.PaymentId == p.Id))
                            {

                                Ws.Cells[Row, 3] = air.Name;
                                (Ws.Cells[Row, 3] as Range).Font.Bold = true;
                                (Ws.Cells[Row, 3] as Range).Font.Italic = true;
                                (Ws.Cells[Row, 3] as Range).Font.Underline = true;
                                Row++;
                                Row++;
                                var allD = GetSaleDishesByPayment(air.Id, StartDate, StopDate);
                                foreach (var d in allD)
                                {

                                    (Ws.Cells[Row, 3] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                                    Ws.Cells[Row, 3] = d.BarCode;
                                    (Ws.Cells[Row, 4] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;
                                    Ws.Cells[Row, 4] = d.Name;
                                    (Ws.Cells[Row, 7] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                                    Ws.Cells[Row, 7] = d.Count;
                                    (Ws.Cells[Row, 9] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                                    Ws.Cells[Row, 9] = d.Price;
                                    (Ws.Cells[Row, 11] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                                    Ws.Cells[Row, 11] = Math.Round(d.Price / d.Count);
                                    Row++;
                                }

                                Row++;

                                (Ws.Cells[Row, 3] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                                Ws.Cells[Row, 3] = "Всего";
                                (Ws.Cells[Row, 4] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;
                                Ws.Cells[Row, 4] = $"({air.Name})";
                                (Ws.Cells[Row, 7] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                                Ws.Cells[Row, 7] = allD.Sum(a => a.Count);
                                (Ws.Cells[Row, 9] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                                Ws.Cells[Row, 9] = allD.Sum(a => a.Price);

                                allCount += allD.Sum(a => a.Count);
                                allSumm += allD.Sum(a => a.Price);

                                (Ws.Cells[Row, 3] as Range).Font.Bold = true;
                                (Ws.Cells[Row, 4] as Range).Font.Bold = true;
                                (Ws.Cells[Row, 7] as Range).Font.Bold = true;
                                (Ws.Cells[Row, 9] as Range).Font.Bold = true;

                                (Ws.Cells[Row, 3] as Range).Font.Italic = true;
                                (Ws.Cells[Row, 4] as Range).Font.Italic = true;
                                (Ws.Cells[Row, 7] as Range).Font.Italic = true;
                                (Ws.Cells[Row, 9] as Range).Font.Italic = true;
                                Row++;
                                Row++;
                                Row++;
                            }
                        }
                        else
                        {

                            var allD = GetSaleDishesByPaymentToGo(p.Id, StartDate, StopDate);
                            Ws.Cells[Row, 3] = p.Name;
                            (Ws.Cells[Row, 3] as Range).Font.Bold = true;
                            (Ws.Cells[Row, 3] as Range).Font.Italic = true;
                            (Ws.Cells[Row, 3] as Range).Font.Underline = true;
                            Row++;
                            Row++;

                            foreach (var d in allD)
                            {

                                (Ws.Cells[Row, 3] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                                Ws.Cells[Row, 3] = d.BarCode;
                                (Ws.Cells[Row, 4] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;
                                Ws.Cells[Row, 4] = d.Name;
                                (Ws.Cells[Row, 7] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                                Ws.Cells[Row, 7] = d.Count;
                                (Ws.Cells[Row, 9] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                                Ws.Cells[Row, 9] = d.Price;
                                (Ws.Cells[Row, 11] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                                Ws.Cells[Row, 11] = Math.Round(d.Price / d.Count);
                                Row++;
                            }

                            Row++;

                            (Ws.Cells[Row, 3] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                            Ws.Cells[Row, 3] = "Всего";
                            (Ws.Cells[Row, 4] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;
                            Ws.Cells[Row, 4] = $"({p.Name})";
                            (Ws.Cells[Row, 7] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                            Ws.Cells[Row, 7] = allD.Sum(a => a.Count);
                            (Ws.Cells[Row, 9] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                            Ws.Cells[Row, 9] = allD.Sum(a => a.Price);

                            allCount += allD.Sum(a => a.Count);
                            allSumm += allD.Sum(a => a.Price);

                            (Ws.Cells[Row, 3] as Range).Font.Bold = true;
                            (Ws.Cells[Row, 4] as Range).Font.Bold = true;
                            (Ws.Cells[Row, 7] as Range).Font.Bold = true;
                            (Ws.Cells[Row, 9] as Range).Font.Bold = true;

                            (Ws.Cells[Row, 3] as Range).Font.Italic = true;
                            (Ws.Cells[Row, 4] as Range).Font.Italic = true;
                            (Ws.Cells[Row, 7] as Range).Font.Italic = true;
                            (Ws.Cells[Row, 9] as Range).Font.Italic = true;
                            Row++;
                        }



                    }
                }


         //  Row++;
         (Ws.Cells[Row, 5] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                Ws.Cells[Row, 5] = "Всего";

                (Ws.Cells[Row, 7] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                Ws.Cells[Row, 7] = allCount;
                (Ws.Cells[Row, 9] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                Ws.Cells[Row, 9] = allSumm;



                (Ws.Cells[Row, 5] as Range).Font.Bold = true;
                (Ws.Cells[Row, 7] as Range).Font.Bold = true;
                (Ws.Cells[Row, 9] as Range).Font.Bold = true;

            }
            catch
            { }
        }


        internal void DBFGetSales2(DateTime StartDate, DateTime StopDate)
        {
            try
            {
                OpenXls();
                //List<ReportTNDR> Tmp = SQLGetRepTNDRs(StartDate, StopDate.AddDays(1));

                //  List<ReportTNDR> ConsolidTmp = new List<ReportTNDR>();

                Worksheet Ws = AddWs("Общая сумма");
                Ws.get_Range("A1:M1000").Font.Name = "Antica";
                Ws.get_Range("A1:M7").Font.Name = "Helica";
                Ws.get_Range("A1:M1000").Font.Size = 10;

                Ws.Cells[1, 5] = "UCS R-Keeper Reports";
                Ws.Cells[1, 6] = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                (Ws.Cells[1, 7] as Range).Font.Italic = true;
                (Ws.Cells[1, 7] as Range).Font.Size = 10;
                (Ws.Cells[1, 7] as Range).Font.Underline = true;
                Ws.Cells[1, 7] = "Галерея";



                (Ws.Cells[3, 6] as Range).Font.Bold = false;
                (Ws.Cells[3, 6] as Range).Font.Italic = true;
                (Ws.Cells[3, 6] as Range).Font.Size = 16;
                (Ws.Cells[3, 6] as Range).Font.Name = "Helica";

                Ws.Cells[3, 6] = "Общая выручка";
                Ws.Cells[5, 4] = "период:";
                (Ws.Cells[5, 6] as Range).Font.Bold = true;
                (Ws.Cells[5, 6] as Range).Font.Underline = true;
                (Ws.Cells[5, 6] as Range).Font.Size = 10;
                (Ws.Cells[5, 6] as Range).Font.Name = "Helica";
                Ws.Cells[5, 6] = StartDate.ToString("dd.MM.yyyy") + " - " + StopDate.ToString("dd.MM.yyyy");
                Ws.Cells[5, 7] = "вкл.";


                Ws.get_Range("A7:P8").Font.Bold = true;
                Ws.get_Range("A7:P8").HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[7, 1] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;
                (Ws.Cells[7, 2] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;



                Ws.Cells[7, 8] = "сумма";
                Ws.Cells[7, 10] = "сумма в баз.";
                Ws.Cells[7, 12] = "сумма в нац.";

                Range r = Ws.Range["J1", System.Type.Missing];
                r.EntireColumn.ColumnWidth = 20;
                r = Ws.Range["L1", System.Type.Missing];
                r.EntireColumn.ColumnWidth = 20;
                r = Ws.Range["H1", System.Type.Missing];
                r.EntireColumn.ColumnWidth = 20;

                int Row = 10;
                decimal allSumm = 0;
                foreach (var pg in DataExtension.DataCatalogsSingleton.Instance.PaymentGroupData.Data.Where(a => a.Sale))
                {
                    decimal pgSumm = 0;
                    foreach (var p in DataExtension.DataCatalogsSingleton.Instance.PaymentData.Data.Where(a => a.PaymentGroupId == pg.Id))
                    {
                        Ws.Cells[Row, 1] = p.Id.ToString();
                        Ws.Cells[Row, 2] = p.Name;
                        decimal s2 = AirOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.AirCompany.PaymentId == p.Id).Sum(a => a.OrderTotalSumm) +
                            ToGoOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.PaymentId == p.Id).Sum(a => a.OrderTotalSumm);
                        pgSumm += s2;
                        (Ws.Cells[Row, 8] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                        Ws.Cells[Row, 8] = s2;
                        (Ws.Cells[Row, 10] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                        Ws.Cells[Row, 10] = s2;
                        (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                        Ws.Cells[Row, 12] = s2;
                        Row++;
                    }
                    Row++;
                    Ws.Cells[Row, 2] = pg.Name;
                    (Ws.Cells[Row, 10] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    Ws.Cells[Row, 10] = pgSumm;
                    (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    Ws.Cells[Row, 12] = pgSumm;
                    (Ws.Cells[Row, 2] as Range).Font.Bold = true;
                    (Ws.Cells[Row, 10] as Range).Font.Bold = true;
                    (Ws.Cells[Row, 12] as Range).Font.Bold = true;
                    allSumm += pgSumm;
                    Row++;
                    Row++;
                }
            //  Row++;
            (Ws.Cells[Row, 7] as Range).Font.Bold = true;
                (Ws.Cells[Row, 10] as Range).Font.Bold = true;
                (Ws.Cells[Row, 12] as Range).Font.Bold = true;


                (Ws.Cells[Row, 10] as Range).Font.Underline = true;
                (Ws.Cells[Row, 12] as Range).Font.Underline = true;

                (Ws.Cells[Row, 7] as Range).Font.Size = 11;
                (Ws.Cells[Row, 10] as Range).Font.Size = 11;
                (Ws.Cells[Row, 12] as Range).Font.Size = 11;

                Ws.Cells[Row, 7] = "ВСЕГО: ";
                Ws.Cells[Row, 10] = allSumm;
                Ws.Cells[Row, 12] = allSumm;

                //app.Visible = true;
                //app.Save(System.Reflection.Missing.Value);

                //app = null;
                // app.Visible = true;
            }
            catch
            { }
        }




        internal void DBFGetBalance(DateTime StartDate, DateTime StopDate)
        {
            try
            {
                OpenXls();
                //List<ReportTNDR> Tmp = SQLGetRepTNDRs(StartDate, StopDate.AddDays(1));

                //  List<ReportTNDR> ConsolidTmp = new List<ReportTNDR>();

                Worksheet Ws = AddWs("Баланс");
                Ws.get_Range("A1:M1000").Font.Name = "Antica";
                Ws.get_Range("A1:M7").Font.Name = "Helica";
                Ws.get_Range("A1:M1000").Font.Size = 10;

                Ws.Cells[1, 5] = "UCS R-Keeper Reports";
                Ws.Cells[1, 6] = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                (Ws.Cells[1, 7] as Range).Font.Italic = true;
                (Ws.Cells[1, 7] as Range).Font.Size = 10;
                (Ws.Cells[1, 7] as Range).Font.Underline = true;
                Ws.Cells[1, 7] = "Галерея";



                (Ws.Cells[3, 6] as Range).Font.Bold = false;
                (Ws.Cells[3, 6] as Range).Font.Italic = true;
                (Ws.Cells[3, 6] as Range).Font.Size = 16;
                (Ws.Cells[3, 6] as Range).Font.Name = "Helica";

                Ws.Cells[3, 6] = "Баланс";
                Ws.Cells[5, 4] = "период:";
                (Ws.Cells[5, 6] as Range).Font.Bold = true;
                (Ws.Cells[5, 6] as Range).Font.Underline = true;
                (Ws.Cells[5, 6] as Range).Font.Size = 10;
                (Ws.Cells[5, 6] as Range).Font.Name = "Helica";
                Ws.Cells[5, 6] = StartDate.ToString("dd.MM.yyyy") + " - " + StopDate.ToString("dd.MM.yyyy");
                Ws.Cells[5, 7] = "вкл.";


                Ws.get_Range("A7:P8").Font.Bold = true;
                Ws.get_Range("A7:P8").HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[7, 1] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;
                (Ws.Cells[7, 2] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;


                Ws.Cells[7, 6] = "чеков";
                Ws.Cells[7, 7] = "гостей";
                Ws.Cells[7, 9] = "сумма";
                Ws.Cells[7, 11] = "на чек";
                Ws.Cells[7, 12] = "на гостя";

                Range r = Ws.Range["J1", System.Type.Missing];
                r.EntireColumn.ColumnWidth = 20;
                r = Ws.Range["L1", System.Type.Missing];
                r.EntireColumn.ColumnWidth = 20;
                r = Ws.Range["H1", System.Type.Missing];
                r.EntireColumn.ColumnWidth = 20;

                int Row = 9;
                decimal allSumm = 0;
                decimal allCount = 0;
                foreach (var pg in DataExtension.DataCatalogsSingleton.Instance.PaymentGroupData.Data.Where(a => a.Sale))
                {
                    decimal pgSumm = 0;
                    decimal pgCount = 0;
                    foreach (var p in DataExtension.DataCatalogsSingleton.Instance.PaymentData.Data.Where(a => a.PaymentGroupId == pg.Id))
                    {
                        decimal s2Summ = AirOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.AirCompany.PaymentId == p.Id).Sum(a => a.OrderTotalSumm) +
                            ToGoOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.PaymentId == p.Id).Sum(a => a.OrderTotalSumm);

                        decimal s2Count = AirOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.AirCompany.PaymentId == p.Id).Count() +
        ToGoOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != OrderStatus.Cancelled && a.PaymentId == p.Id).Count();
                        pgSumm += s2Summ;
                        pgCount += s2Count;
                    }
                    Row++;
                    Ws.Cells[Row, 2] = pg.Name;
                    (Ws.Cells[Row, 2] as Range).Font.Bold = true;
                    (Ws.Cells[Row, 2] as Range).Font.Italic = true;

                    Ws.Cells[Row, 6] = pgCount;
                    Ws.Cells[Row, 7] = pgCount;

                    Ws.Cells[Row, 9] = pgSumm;
                    Ws.Cells[Row, 11] = Math.Round(pgSumm / pgCount);
                    Ws.Cells[Row, 12] = Math.Round(pgSumm / pgCount);
                    /*
                    //(Ws.Cells[Row, 2] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;
                    Ws.Cells[Row, 10] = pgSumm;
                    (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    Ws.Cells[Row, 12] = pgSumm;
                    (Ws.Cells[Row, 2] as Range).Font.Bold = true;
                    (Ws.Cells[Row, 10] as Range).Font.Bold = true;
                    (Ws.Cells[Row, 12] as Range).Font.Bold = true;

                    Row++;
                    */
                    allSumm += pgSumm;
                    allCount += pgCount;
                    Row++;
                }
                //  Row++;
                Ws.Cells[Row, 2] = "Всего";
                (Ws.Cells[Row, 2] as Range).Font.Bold = true;
                (Ws.Cells[Row, 2] as Range).Font.Italic = true;

                Ws.Cells[Row, 6] = allCount;
                Ws.Cells[Row, 7] = allCount;

                Ws.Cells[Row, 9] = allSumm;
                Ws.Cells[Row, 11] = Math.Round(allSumm / allCount);
                Ws.Cells[Row, 12] = Math.Round(allSumm / allCount);

                //app.Visible = true;
                //app.Save(System.Reflection.Missing.Value);

                //app = null;
                // app.Visible = true;
            }
            catch
            { }
        }


        internal void DBFGetSales(DateTime StartDate, DateTime StopDate)
        {
            try
            {
                OpenXls();
                List<ReportTNDR> Tmp = SQLGetRepTNDRs(StartDate, StopDate.AddDays(1));

                List<ReportTNDR> ConsolidTmp = new List<ReportTNDR>();

                Worksheet Ws = AddWs("Общая сумма");
                Ws.get_Range("A1:M1000").Font.Name = "Antica";
                Ws.get_Range("A1:M7").Font.Name = "Helica";
                Ws.get_Range("A1:M1000").Font.Size = 10;

                Ws.Cells[1, 5] = "UCS R-Keeper Reports";
                Ws.Cells[1, 6] = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                (Ws.Cells[1, 7] as Range).Font.Italic = true;
                (Ws.Cells[1, 7] as Range).Font.Size = 10;
                (Ws.Cells[1, 7] as Range).Font.Underline = true;
                Ws.Cells[1, 7] = "Галерея";



                (Ws.Cells[3, 6] as Range).Font.Bold = false;
                (Ws.Cells[3, 6] as Range).Font.Italic = true;
                (Ws.Cells[3, 6] as Range).Font.Size = 16;
                (Ws.Cells[3, 6] as Range).Font.Name = "Helica";

                Ws.Cells[3, 6] = "Общая выручка";
                Ws.Cells[5, 4] = "период:";
                (Ws.Cells[5, 6] as Range).Font.Bold = true;
                (Ws.Cells[5, 6] as Range).Font.Underline = true;
                (Ws.Cells[5, 6] as Range).Font.Size = 10;
                (Ws.Cells[5, 6] as Range).Font.Name = "Helica";
                Ws.Cells[5, 6] = StartDate.ToString("dd.MM.yyyy") + " - " + StopDate.ToString("dd.MM.yyyy");
                Ws.Cells[5, 7] = "вкл.";


                Ws.get_Range("A7:P8").Font.Bold = true;
                Ws.get_Range("A7:P8").HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[7, 1] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;
                (Ws.Cells[7, 2] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;



                Ws.Cells[7, 8] = "сумма";
                Ws.Cells[7, 10] = "сумма в баз.";
                Ws.Cells[7, 12] = "сумма в нац.";

                Range r = Ws.Range["J1", System.Type.Missing];
                r.EntireColumn.ColumnWidth = 20;
                r = Ws.Range["L1", System.Type.Missing];
                r.EntireColumn.ColumnWidth = 20;
                r = Ws.Range["H1", System.Type.Missing];
                r.EntireColumn.ColumnWidth = 20;

                int Row = 10;
                for (int i = 0; i < 2; i++)
                {
                    List<int> Vals;

                    foreach (int rdId in Tmp.Where(d => (i == 1) ^ d.Cash).Select(a => a.Id).Distinct())
                    {
                        List<ReportTNDR> rdList = Tmp.Where(a => a.Id == rdId).ToList();
                        Ws.Cells[Row, 1] = rdId.ToString();
                        Ws.Cells[Row, 2] = rdList[0].Name;
                        (Ws.Cells[Row, 8] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                        decimal s2 = rdList.Sum(a => a.Summ);
                        Ws.Cells[Row, 8] = s2;
                        (Ws.Cells[Row, 10] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                        Ws.Cells[Row, 10] = s2;
                        (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                        Ws.Cells[Row, 12] = s2;

                        Row++;

                    }

                    Row++;
                    // Ws.get_Range(Ws.Cells[Row, 1], Ws.Cells[Row, 20]).Font.Bold = true;


                    (Ws.Cells[Row, 2] as Range).Font.Bold = true;
                    (Ws.Cells[Row, 10] as Range).Font.Bold = true;
                    (Ws.Cells[Row, 12] as Range).Font.Bold = true;

                    (Ws.Cells[Row, 2] as Range).Font.Italic = true;

                    (Ws.Cells[Row, 2] as Range).Font.Size = 11;
                    (Ws.Cells[Row, 10] as Range).Font.Size = 11;
                    (Ws.Cells[Row, 12] as Range).Font.Size = 11;

                    if (i == 0)
                    {
                        Ws.Cells[Row, 2] = "Наличные";
                    }
                    else
                    {
                        Ws.Cells[Row, 2] = "Карты";
                    };
                    decimal s1 = Tmp.Where(d => (i == 1) ^ d.Cash).Sum(a => a.Summ);
                    Ws.Cells[Row, 10] = s1;
                    Ws.Cells[Row, 12] = s1;


                    Row = Row + 2;
                }
          (Ws.Cells[Row, 7] as Range).Font.Bold = true;
                (Ws.Cells[Row, 10] as Range).Font.Bold = true;
                (Ws.Cells[Row, 12] as Range).Font.Bold = true;


                (Ws.Cells[Row, 10] as Range).Font.Underline = true;
                (Ws.Cells[Row, 12] as Range).Font.Underline = true;

                (Ws.Cells[Row, 7] as Range).Font.Size = 11;
                (Ws.Cells[Row, 10] as Range).Font.Size = 11;
                (Ws.Cells[Row, 12] as Range).Font.Size = 11;

                Ws.Cells[Row, 7] = "ВСЕГО: ";
                Ws.Cells[Row, 10] = Tmp.Sum(a => a.Summ);
                Ws.Cells[Row, 12] = Tmp.Sum(a => a.Summ);

                //app.Visible = true;
                //app.Save(System.Reflection.Missing.Value);

                //app = null;
                // app.Visible = true;
            }
            catch
            { }
        }


        internal void DBFGetRashDishezOnStationGroup(DateTime StartDate, DateTime StopDate)
        {
            try
            {
                OpenXls();
                List<ReportDish> Tmp = SQLGetRashDishez(StartDate, StopDate.AddDays(1));

                List<ReportDish> ConsolidTmp = new List<ReportDish>();

                foreach (ReportDish rd in Tmp)
                {
                    ReportDish rd2;
                    if (ConsolidTmp.Exists(a => a.BarCode == rd.BarCode))
                    {
                        rd2 = ConsolidTmp.Where(a => a.BarCode == rd.BarCode).FirstOrDefault();
                        rd2.Count += rd.Count;
                        rd2.Price += rd.Price * rd.Count;
                        rd2.DiscPrice += rd.DiscPrice * rd.Count;
                    }
                    else
                    {
                        rd2 = new ReportDish();
                        rd2.Name = rd.Name;
                        rd2.BarCode = rd.BarCode;
                        rd2.Count = rd.Count;
                        rd2.Price = rd.Price * rd.Count;
                        rd2.DiscPrice = rd.DiscPrice * rd.Count;
                        ConsolidTmp.Add(rd2);
                    }
                }


                //Ws.Cells[2, 1] = "Критерии";
                Worksheet Ws = AddWs("Расход блюд по группам станций");
                Ws.get_Range("A1:M1000").Font.Name = "Antica";
                Ws.get_Range("A1:M7").Font.Name = "Helica";
                Ws.get_Range("A1:M1000").Font.Size = 10;

                Ws.Cells[1, 5] = "UCS R-Keeper Reports";
                Ws.Cells[1, 6] = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                (Ws.Cells[1, 7] as Range).Font.Italic = true;
                (Ws.Cells[1, 7] as Range).Font.Size = 10;
                (Ws.Cells[1, 7] as Range).Font.Underline = true;
                Ws.Cells[1, 7] = "Галерея";



                (Ws.Cells[3, 6] as Range).Font.Bold = false;
                (Ws.Cells[3, 6] as Range).Font.Italic = true;
                (Ws.Cells[3, 6] as Range).Font.Size = 16;
                (Ws.Cells[3, 6] as Range).Font.Name = "Helica";

                Ws.Cells[3, 6] = "Расход блюд по группам станций";
                Ws.Cells[5, 4] = "период:";
                (Ws.Cells[5, 6] as Range).Font.Bold = true;
                (Ws.Cells[5, 6] as Range).Font.Underline = true;
                (Ws.Cells[5, 6] as Range).Font.Size = 10;
                (Ws.Cells[5, 6] as Range).Font.Name = "Helica";
                Ws.Cells[5, 6] = StartDate.ToString("dd.MM.yyyy") + " - " + StopDate.ToString("dd.MM.yyyy");
                Ws.Cells[5, 7] = "вкл.";

                (Ws.Cells[11, 2] as Range).Font.Bold = true;
                (Ws.Cells[11, 2] as Range).Font.Underline = true;
                (Ws.Cells[11, 2] as Range).Font.Italic = true;
                (Ws.Cells[11, 3] as Range).Font.Size = 11;
                (Ws.Cells[11, 2] as Range).Font.Name = "Helica";
                Ws.Cells[11, 2] = "Самолеты";

                (Ws.Cells[9, 5] as Range).Font.Bold = true;
                (Ws.Cells[9, 5] as Range).Font.Underline = true;
                (Ws.Cells[9, 5] as Range).Font.Size = 11;
                (Ws.Cells[9, 5] as Range).Font.Name = "Helica";
                (Ws.Cells[9, 5] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                Ws.Cells[9, 5] = "Группа станций:";
                (Ws.Cells[9, 6] as Range).Font.Bold = true;
                (Ws.Cells[9, 6] as Range).Font.Underline = true;
                (Ws.Cells[9, 6] as Range).Font.Size = 11;
                (Ws.Cells[9, 6] as Range).Font.Name = "Helica";
                (Ws.Cells[9, 6] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;
                Ws.Cells[9, 6] = "All";

                Ws.get_Range("A7:P7").Font.Bold = true;
                Ws.get_Range("A7:P7").HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[7, 3] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;
                Ws.Cells[7, 2] = "код";
                Ws.Cells[7, 3] = "название";
                Ws.Cells[7, 6] = "кол-во";
                Ws.Cells[7, 8] = "сумма";
                Ws.Cells[7, 9] = "ср. цена";
                Ws.Cells[7, 11] = "скидка";
                Ws.Cells[7, 12] = "оплачено";

                int Row = 13;
                foreach (ReportDish rd in ConsolidTmp)
                {
                    (Ws.Cells[Row, 2] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    if (rd.BarCode > 100000)
                    {
                        Ws.Cells[Row, 2] = (rd.BarCode - 100000).ToString();
                    }
                    else
                    {
                        Ws.Cells[Row, 2] = rd.BarCode.ToString();
                    }
                    //Ws.Cells[Row, 3] = Encoding.GetEncoding(1251).GetString(
                    //          Encoding.GetEncoding(1252).GetBytes(rd.Name));

                    Ws.Cells[Row, 3] = rd.Name;
                    (Ws.Cells[Row, 6] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    Ws.Cells[Row, 6] = rd.Count;
                    (Ws.Cells[Row, 8] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    Ws.Cells[Row, 8] = rd.Price;
                    (Ws.Cells[Row, 9] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    Ws.Cells[Row, 9] = rd.Price / rd.Count;
                    (Ws.Cells[Row, 11] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    if (rd.Price - rd.DiscPrice > 0)
                    {
                        Ws.Cells[Row, 11] = rd.Price - rd.DiscPrice;
                    }
                    (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    Ws.Cells[Row, 12] = rd.DiscPrice;
                    Row++;

                }

                Row++;
                // Ws.get_Range(Ws.Cells[Row, 1], Ws.Cells[Row, 20]).Font.Bold = true;


                (Ws.Cells[Row, 3] as Range).Font.Bold = true;
                (Ws.Cells[Row, 8] as Range).Font.Bold = true;
                (Ws.Cells[Row, 11] as Range).Font.Bold = true;
                (Ws.Cells[Row, 13] as Range).Font.Bold = true;

                (Ws.Cells[Row, 3] as Range).Font.Italic = true;
                (Ws.Cells[Row, 8] as Range).Font.Italic = true;
                (Ws.Cells[Row, 11] as Range).Font.Italic = true;
                (Ws.Cells[Row, 13] as Range).Font.Italic = true;

                Ws.Cells[Row, 3] = "Всего (Самолеты)";
                Ws.Cells[Row, 8] = ConsolidTmp.Sum(a => a.Price);
                if (ConsolidTmp.Sum(a => a.Price - a.DiscPrice) > 0)
                {
                    Ws.Cells[Row, 11] = ConsolidTmp.Sum(a => a.Price - a.DiscPrice);
                }
                Ws.Cells[Row, 13] = ConsolidTmp.Sum(a => a.DiscPrice);

                Row = Row + 2;

                (Ws.Cells[Row, 5] as Range).Font.Bold = true;
                (Ws.Cells[Row, 8] as Range).Font.Bold = true;
                (Ws.Cells[Row, 13] as Range).Font.Bold = true;

                (Ws.Cells[Row, 5] as Range).Font.Size = 11;
                (Ws.Cells[Row, 8] as Range).Font.Size = 11;
                (Ws.Cells[Row, 13] as Range).Font.Size = 11;

                Ws.Cells[Row, 5] = "Всего (All) ";
                Ws.Cells[Row, 8] = ConsolidTmp.Sum(a => a.Price);
                Ws.Cells[Row, 13] = ConsolidTmp.Sum(a => a.DiscPrice);

                //app.Save(System.Reflection.Missing.Value);

                //app = null;
                //  app.Visible = true;
            }
            catch
            { }
        }

        internal void DBFGetRashOpenDishezOnCat(DateTime StartDate, DateTime StopDate)
        {
            try
            {
                OpenXls();
                List<ReportDish> Tmp = SQLGetRashDishez(StartDate, StopDate.AddDays(1)).Where(a => DataCatalogsSingleton.Instance.OpenDishezBarCodes.Contains(a.BarCode)).ToList();
                List<ReportDish> TmpToGo = SQLGetRashDishezToGo(StartDate, StopDate.AddDays(1)).Where(a => DataCatalogsSingleton.Instance.OpenDishezToGoBarCodes.Contains(a.BarCode)).ToList();
                List<ReportDish> ConsolidTmp = Tmp;
                List<ReportDish> ConsolidTmpToGo = TmpToGo;

                /*
                foreach (ReportDish rd in Tmp)
                {
                    ReportDish rd2;
                    if (ConsolidTmp.Exists(a => a.BarCode == rd.BarCode))
                    {
                        rd2 = ConsolidTmp.Where(a => a.BarCode == rd.BarCode).FirstOrDefault();
                        rd2.Count += rd.Count;
                        rd2.Price += rd.Price * rd.Count;
                        rd2.DiscPrice += rd.DiscPrice * rd.Count;
                    }
                    else
                    {
                        rd2 = new ReportDish();
                        rd2.Name = rd.Name;
                        rd2.BarCode = rd.BarCode;
                        rd2.Count = rd.Count;
                        rd2.Price = rd.Price * rd.Count;
                        rd2.DiscPrice = rd.DiscPrice * rd.Count;
                        ConsolidTmp.Add(rd2);
                    }
                }
                List<ReportDish> ConsolidTmpToGo = new List<ReportDish>();
                foreach (ReportDish rd in TmpToGo)
                {
                    ReportDish rd2;
                    if (ConsolidTmp.Exists(a => a.BarCode == rd.BarCode))
                    {
                        rd2 = ConsolidTmp.Where(a => a.BarCode == rd.BarCode).FirstOrDefault();
                        rd2.Count += rd.Count;
                        rd2.Price += rd.Price * rd.Count;
                        rd2.DiscPrice += rd.DiscPrice * rd.Count;
                    }
                    else
                    {
                        rd2 = new ReportDish();
                        rd2.Name = rd.Name;
                        rd2.BarCode = rd.BarCode;
                        rd2.Count = rd.Count;
                        rd2.Price = rd.Price * rd.Count;
                        rd2.DiscPrice = rd.DiscPrice * rd.Count;
                        ConsolidTmpToGo.Add(rd2);
                    }
                }

                */


                //Ws.Cells[2, 1] = "Критерии";
                Worksheet Ws = AddWs("Расход открытых блюд");
                Ws.get_Range("A1:M1000").Font.Name = "Antica";
                Ws.get_Range("A1:M7").Font.Name = "Helica";
                Ws.get_Range("A1:M1000").Font.Size = 10;

                Ws.Cells[1, 5] = "UCS R-Keeper Reports";
                Ws.Cells[1, 6] = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                (Ws.Cells[1, 7] as Range).Font.Italic = true;
                (Ws.Cells[1, 7] as Range).Font.Size = 10;
                (Ws.Cells[1, 7] as Range).Font.Underline = true;
                Ws.Cells[1, 7] = "Галерея";



                (Ws.Cells[3, 6] as Range).Font.Bold = true;
                (Ws.Cells[3, 6] as Range).Font.Italic = true;
                (Ws.Cells[3, 6] as Range).Font.Size = 16;
                (Ws.Cells[3, 6] as Range).Font.Name = "Helica";
                (Ws.Cells[3, 6] as Range).HorizontalAlignment = XlHAlign.xlHAlignCenter;

                Ws.Cells[3, 6] = "Расход открытых блюд";
                Ws.Cells[6, 4] = "период:";
                (Ws.Cells[6, 6] as Range).Font.Bold = true;
                (Ws.Cells[6, 6] as Range).Font.Underline = true;
                (Ws.Cells[6, 6] as Range).Font.Size = 10;
                (Ws.Cells[6, 6] as Range).Font.Name = "Helica";
                Ws.Cells[6, 6] = StartDate.ToString("dd.MM.yyyy") + " - " + StopDate.ToString("dd.MM.yyyy");
                Ws.Cells[6, 7] = "вкл.";

                Ws.get_Range("A7:P7").Font.Bold = true;
                Ws.get_Range("A7:P7").HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[7, 3] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;
                Ws.Cells[7, 2] = "код";
                Ws.Cells[7, 3] = "название";
                Ws.Cells[7, 6] = "кол-во";
                Ws.Cells[7, 8] = "сумма";
                Ws.Cells[7, 9] = "ср. цена";
                Ws.Cells[7, 11] = "скидка";
                Ws.Cells[7, 12] = "оплачено";

                (Ws.Cells[9, 2] as Range).Font.Bold = true;
                (Ws.Cells[9, 2] as Range).Font.Italic = true;
                (Ws.Cells[9, 2] as Range).Font.Underline = true;
                (Ws.Cells[9, 2] as Range).Font.Size = 11;
                (Ws.Cells[9, 2] as Range).Font.Name = "Helica";
                Ws.Cells[9, 2] = "Самолеты";

                int Row = 11;
                foreach (ReportDish rd in ConsolidTmp)
                {
                    (Ws.Cells[Row, 2] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    (Ws.Cells[Row, 6] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    (Ws.Cells[Row, 8] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    (Ws.Cells[Row, 9] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    (Ws.Cells[Row, 11] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    Ws.Cells[Row, 2] = rd.BarCode.ToString();

                    Ws.Cells[Row, 3] = rd.Name;
                    Ws.Cells[Row, 6] = rd.Count;
                    Ws.Cells[Row, 8] = rd.Price * rd.Count;
                    Ws.Cells[Row, 9] = rd.Price;
                    if (rd.Price - rd.DiscPrice > 0)
                    {
                        Ws.Cells[Row, 11] = (rd.Price - rd.DiscPrice) * rd.Count;
                    }
                    Ws.Cells[Row, 12] = rd.DiscPrice * rd.Count;
                    Row++;

                }

                Row++;
                // Ws.get_Range(Ws.Cells[Row, 1], Ws.Cells[Row, 20]).Font.Bold = true;


                (Ws.Cells[Row, 3] as Range).Font.Bold = true;
                (Ws.Cells[Row, 4] as Range).Font.Bold = true;
                (Ws.Cells[Row, 3] as Range).Font.Italic = true;
                (Ws.Cells[Row, 4] as Range).Font.Italic = true;
                (Ws.Cells[Row, 8] as Range).Font.Bold = true;
                (Ws.Cells[Row, 12] as Range).Font.Bold = true;
                (Ws.Cells[Row, 3] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 8] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                Ws.Cells[Row, 3] = "Всего ";
                Ws.Cells[Row, 4] = "(Самолеты)";

                Ws.Cells[Row, 8] = ConsolidTmp.Sum(a => a.Price * a.Count);
                if (ConsolidTmp.Sum(a => a.Price - a.DiscPrice) > 0)
                {
                    Ws.Cells[Row, 11] = ConsolidTmp.Sum(a => (a.Price - a.DiscPrice) * a.Count);
                }
                Ws.Cells[Row, 12] = ConsolidTmp.Sum(a => a.DiscPrice * a.Count);

                Row = Row + 3;



                Ws.Cells[Row, 2] = "ToGo";
                Row = Row + 2;

                foreach (ReportDish rd in ConsolidTmpToGo)
                {
                    (Ws.Cells[Row, 2] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    (Ws.Cells[Row, 6] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    (Ws.Cells[Row, 8] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    (Ws.Cells[Row, 9] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    (Ws.Cells[Row, 11] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    Ws.Cells[Row, 2] = rd.BarCode.ToString();

                    Ws.Cells[Row, 3] = rd.Name;
                    Ws.Cells[Row, 6] = rd.Count;
                    Ws.Cells[Row, 8] = rd.Price * rd.Count;
                    Ws.Cells[Row, 9] = rd.Price;
                    if (rd.Price - rd.DiscPrice > 0)
                    {
                        Ws.Cells[Row, 11] = (rd.Price - rd.DiscPrice) * rd.Count;
                    }
                    Ws.Cells[Row, 12] = rd.DiscPrice * rd.Count; ;
                    Row++;

                }





          (Ws.Cells[Row, 2] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 6] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 8] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 9] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 11] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                Ws.Cells[Row, 2] = 0;

                Ws.Cells[Row, 3] = "Доставка";
                Ws.Cells[Row, 6] = ToGoOrdersModelSingleton.Instance.Orders.Where(a => a.DeliveryPrice > 0).Count();
                Ws.Cells[Row, 8] = ToGoOrdersModelSingleton.Instance.Orders.Sum(a => a.DeliveryPrice);
                Ws.Cells[Row, 9] = ToGoOrdersModelSingleton.Instance.Orders.Sum(a => a.DeliveryPrice) / ToGoOrdersModelSingleton.Instance.Orders.Where(a => a.DeliveryPrice > 0).Count();
                /*
                if (rd.Price - rd.DiscPrice > 0)
                {
                    Ws.Cells[Row, 11] = rd.Price - rd.DiscPrice;
                }
                */
                Ws.Cells[Row, 12] = ToGoOrdersModelSingleton.Instance.Orders.Sum(a => a.DeliveryPrice) / ToGoOrdersModelSingleton.Instance.Orders.Where(a => a.DeliveryPrice > 0).Count();
                Row++;
                Row++;

                (Ws.Cells[Row, 3] as Range).Font.Bold = true;
                (Ws.Cells[Row, 4] as Range).Font.Bold = true;
                (Ws.Cells[Row, 3] as Range).Font.Italic = true;
                (Ws.Cells[Row, 4] as Range).Font.Italic = true;
                (Ws.Cells[Row, 8] as Range).Font.Bold = true;
                (Ws.Cells[Row, 12] as Range).Font.Bold = true;
                (Ws.Cells[Row, 3] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 8] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                Ws.Cells[Row, 3] = "Всего ";
                Ws.Cells[Row, 4] = "(ToGo)";

                Ws.Cells[Row, 8] = ConsolidTmpToGo.Sum(a => a.Price * a.Count);
                if (ConsolidTmpToGo.Sum(a => a.Price - a.DiscPrice) > 0)
                {
                    Ws.Cells[Row, 11] = ConsolidTmpToGo.Sum(a => (a.Price - a.DiscPrice) * a.Count);
                }
                Ws.Cells[Row, 12] = ConsolidTmpToGo.Sum(a => a.DiscPrice * a.Count);

                Row = Row + 2;


                (Ws.Cells[Row, 5] as Range).Font.Bold = true;
                (Ws.Cells[Row, 8] as Range).Font.Bold = true;
                (Ws.Cells[Row, 11] as Range).Font.Bold = true;
                (Ws.Cells[Row, 12] as Range).Font.Bold = true;
                (Ws.Cells[Row, 5] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 8] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 5] as Range).Font.Size = 11;
                (Ws.Cells[Row, 8] as Range).Font.Size = 11;
                (Ws.Cells[Row, 11] as Range).Font.Size = 11;
                (Ws.Cells[Row, 12] as Range).Font.Size = 11;
                Ws.Cells[Row, 5] = "Всего: ";


                Ws.Cells[Row, 8] = ConsolidTmp.Sum(a => a.Price * a.Count) + ConsolidTmpToGo.Sum(a => a.Price * a.Count);
                if (ConsolidTmp.Sum(a => (a.Price - a.DiscPrice) * a.Count) + ConsolidTmpToGo.Sum(a => (a.Price - a.DiscPrice) * a.Count) > 0)
                {
                    Ws.Cells[Row, 11] = ConsolidTmp.Sum(a => (a.Price - a.DiscPrice) * a.Count) + ConsolidTmpToGo.Sum(a => (a.Price - a.DiscPrice) * a.Count);
                }
                Ws.Cells[Row, 12] = ConsolidTmp.Sum(a => a.DiscPrice * a.Count) + ConsolidTmpToGo.Sum(a => a.DiscPrice * a.Count);
                //  app.Visible = true;
            }
            catch
            { }
        }

        internal void DBFGetRashDishezOnCat(DateTime StartDate, DateTime StopDate)
        {
            try
            {
                OpenXls();
                List<ReportDish> Tmp = SQLGetRashDishez(StartDate, StopDate.AddDays(1));
                List<ReportDish> TmpToGo = SQLGetRashDishezToGo(StartDate, StopDate.AddDays(1));

                List<ReportDish> ConsolidTmp = new List<ReportDish>();
                foreach (ReportDish rd in Tmp)
                {
                    ReportDish rd2;
                    if (ConsolidTmp.Exists(a => a.BarCode == rd.BarCode))
                    {
                        rd2 = ConsolidTmp.Where(a => a.BarCode == rd.BarCode).FirstOrDefault();
                        rd2.Count += rd.Count;
                        rd2.Price += rd.Price * rd.Count;
                        rd2.DiscPrice += rd.DiscPrice * rd.Count;
                    }
                    else
                    {
                        rd2 = new ReportDish();
                        rd2.Name = rd.Name;
                        rd2.BarCode = rd.BarCode;
                        rd2.Count = rd.Count;
                        rd2.Price = rd.Price * rd.Count;
                        rd2.DiscPrice = rd.DiscPrice * rd.Count;
                        ConsolidTmp.Add(rd2);
                    }
                }


                List<ReportDish> ConsolidTmpToGo = new List<ReportDish>();
                foreach (ReportDish rd in TmpToGo)
                {
                    ReportDish rd2;
                    if (ConsolidTmp.Exists(a => a.BarCode == rd.BarCode))
                    {
                        rd2 = ConsolidTmp.Where(a => a.BarCode == rd.BarCode).FirstOrDefault();
                        rd2.Count += rd.Count;
                        rd2.Price += rd.Price * rd.Count;
                        rd2.DiscPrice += rd.DiscPrice * rd.Count;
                    }
                    else
                    {
                        rd2 = new ReportDish();
                        rd2.Name = rd.Name;
                        rd2.BarCode = rd.BarCode;
                        rd2.Count = rd.Count;
                        rd2.Price = rd.Price * rd.Count;
                        rd2.DiscPrice = rd.DiscPrice * rd.Count;
                        ConsolidTmpToGo.Add(rd2);
                    }
                }




                //Ws.Cells[2, 1] = "Критерии";
                Worksheet Ws = AddWs("Расход блюд по категориям");
                Ws.get_Range("A1:M1000").Font.Name = "Antica";
                Ws.get_Range("A1:M7").Font.Name = "Helica";
                Ws.get_Range("A1:M1000").Font.Size = 10;

                Ws.Cells[1, 5] = "UCS R-Keeper Reports";
                Ws.Cells[1, 6] = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                (Ws.Cells[1, 7] as Range).Font.Italic = true;
                (Ws.Cells[1, 7] as Range).Font.Size = 10;
                (Ws.Cells[1, 7] as Range).Font.Underline = true;
                Ws.Cells[1, 7] = "Галерея";



                (Ws.Cells[3, 6] as Range).Font.Bold = true;
                (Ws.Cells[3, 6] as Range).Font.Italic = true;
                (Ws.Cells[3, 6] as Range).Font.Size = 16;
                (Ws.Cells[3, 6] as Range).Font.Name = "Helica";
                (Ws.Cells[3, 6] as Range).HorizontalAlignment = XlHAlign.xlHAlignCenter;

                Ws.Cells[3, 6] = "Расход блюд по категориям";
                Ws.Cells[6, 4] = "период:";
                (Ws.Cells[6, 6] as Range).Font.Bold = true;
                (Ws.Cells[6, 6] as Range).Font.Underline = true;
                (Ws.Cells[6, 6] as Range).Font.Size = 10;
                (Ws.Cells[6, 6] as Range).Font.Name = "Helica";
                Ws.Cells[6, 6] = StartDate.ToString("dd.MM.yyyy") + " - " + StopDate.ToString("dd.MM.yyyy");
                Ws.Cells[6, 7] = "вкл.";

                Ws.get_Range("A7:P7").Font.Bold = true;
                Ws.get_Range("A7:P7").HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[7, 3] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;
                Ws.Cells[7, 2] = "код";
                Ws.Cells[7, 3] = "название";
                Ws.Cells[7, 6] = "кол-во";
                Ws.Cells[7, 8] = "сумма";
                Ws.Cells[7, 9] = "ср. цена";
                Ws.Cells[7, 11] = "скидка";
                Ws.Cells[7, 12] = "оплачено";

                (Ws.Cells[9, 2] as Range).Font.Bold = true;
                (Ws.Cells[9, 2] as Range).Font.Italic = true;
                (Ws.Cells[9, 2] as Range).Font.Underline = true;
                (Ws.Cells[9, 2] as Range).Font.Size = 11;
                (Ws.Cells[9, 2] as Range).Font.Name = "Helica";
                Ws.Cells[9, 2] = "Самолеты";

                int Row = 11;
                foreach (ReportDish rd in ConsolidTmp)
                {
                    (Ws.Cells[Row, 2] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    (Ws.Cells[Row, 6] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    (Ws.Cells[Row, 8] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    (Ws.Cells[Row, 9] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    (Ws.Cells[Row, 11] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    Ws.Cells[Row, 2] = rd.BarCode.ToString();

                    Ws.Cells[Row, 3] = rd.Name;
                    Ws.Cells[Row, 6] = rd.Count;
                    Ws.Cells[Row, 8] = rd.Price;
                    Ws.Cells[Row, 9] = rd.Price / rd.Count;
                    if (rd.Price - rd.DiscPrice > 0)
                    {
                        Ws.Cells[Row, 11] = rd.Price - rd.DiscPrice;
                    }
                    Ws.Cells[Row, 12] = rd.DiscPrice;
                    Row++;

                }

                Row++;
                // Ws.get_Range(Ws.Cells[Row, 1], Ws.Cells[Row, 20]).Font.Bold = true;


                (Ws.Cells[Row, 3] as Range).Font.Bold = true;
                (Ws.Cells[Row, 4] as Range).Font.Bold = true;
                (Ws.Cells[Row, 3] as Range).Font.Italic = true;
                (Ws.Cells[Row, 4] as Range).Font.Italic = true;
                (Ws.Cells[Row, 8] as Range).Font.Bold = true;
                (Ws.Cells[Row, 12] as Range).Font.Bold = true;
                (Ws.Cells[Row, 3] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 8] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                Ws.Cells[Row, 3] = "Всего ";
                Ws.Cells[Row, 4] = "(Самолеты)";

                Ws.Cells[Row, 8] = ConsolidTmp.Sum(a => a.Price);
                if (ConsolidTmp.Sum(a => a.Price - a.DiscPrice) > 0)
                {
                    Ws.Cells[Row, 11] = ConsolidTmp.Sum(a => a.Price - a.DiscPrice);
                }
                Ws.Cells[Row, 12] = ConsolidTmp.Sum(a => a.DiscPrice);

                Row = Row + 3;



                Ws.Cells[Row, 2] = "ToGo";
                Row = Row + 2;

                foreach (ReportDish rd in ConsolidTmpToGo)
                {
                    (Ws.Cells[Row, 2] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    (Ws.Cells[Row, 6] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    (Ws.Cells[Row, 8] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    (Ws.Cells[Row, 9] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    (Ws.Cells[Row, 11] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                    Ws.Cells[Row, 2] = rd.BarCode.ToString();

                    Ws.Cells[Row, 3] = rd.Name;
                    Ws.Cells[Row, 6] = rd.Count;
                    Ws.Cells[Row, 8] = rd.Price;
                    Ws.Cells[Row, 9] = rd.Price / rd.Count;
                    if (rd.Price - rd.DiscPrice > 0)
                    {
                        Ws.Cells[Row, 11] = rd.Price - rd.DiscPrice;
                    }
                    Ws.Cells[Row, 12] = rd.DiscPrice;
                    Row++;

                }





            (Ws.Cells[Row, 2] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 6] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 8] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 9] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 11] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                Ws.Cells[Row, 2] = 0;

                Ws.Cells[Row, 3] = "Доставка";
                Ws.Cells[Row, 6] = ToGoOrdersModelSingleton.Instance.Orders.Where(a => a.DeliveryPrice > 0).Count();
                Ws.Cells[Row, 8] = ToGoOrdersModelSingleton.Instance.Orders.Sum(a => a.DeliveryPrice);
                Ws.Cells[Row, 9] = ToGoOrdersModelSingleton.Instance.Orders.Sum(a => a.DeliveryPrice) / ToGoOrdersModelSingleton.Instance.Orders.Where(a => a.DeliveryPrice > 0).Count();
                /*
                if (rd.Price - rd.DiscPrice > 0)
                {
                    Ws.Cells[Row, 11] = rd.Price - rd.DiscPrice;
                }
                */
                Ws.Cells[Row, 12] = ToGoOrdersModelSingleton.Instance.Orders.Sum(a => a.DeliveryPrice) / ToGoOrdersModelSingleton.Instance.Orders.Where(a => a.DeliveryPrice > 0).Count();
                Row++;
                Row++;

                (Ws.Cells[Row, 3] as Range).Font.Bold = true;
                (Ws.Cells[Row, 4] as Range).Font.Bold = true;
                (Ws.Cells[Row, 3] as Range).Font.Italic = true;
                (Ws.Cells[Row, 4] as Range).Font.Italic = true;
                (Ws.Cells[Row, 8] as Range).Font.Bold = true;
                (Ws.Cells[Row, 12] as Range).Font.Bold = true;
                (Ws.Cells[Row, 3] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 8] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                Ws.Cells[Row, 3] = "Всего ";
                Ws.Cells[Row, 4] = "(ToGo)";

                Ws.Cells[Row, 8] = ConsolidTmpToGo.Sum(a => a.Price) + ToGoOrdersModelSingleton.Instance.Orders.Sum(a => a.DeliveryPrice);
                if (ConsolidTmpToGo.Sum(a => a.Price - a.DiscPrice) > 0)
                {
                    Ws.Cells[Row, 11] = ConsolidTmpToGo.Sum(a => a.Price - a.DiscPrice);
                }
                Ws.Cells[Row, 12] = ConsolidTmpToGo.Sum(a => a.DiscPrice) + ToGoOrdersModelSingleton.Instance.Orders.Sum(a => a.DeliveryPrice); ;

                Row = Row + 2;


                (Ws.Cells[Row, 5] as Range).Font.Bold = true;
                (Ws.Cells[Row, 8] as Range).Font.Bold = true;
                (Ws.Cells[Row, 11] as Range).Font.Bold = true;
                (Ws.Cells[Row, 12] as Range).Font.Bold = true;
                (Ws.Cells[Row, 5] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 8] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 12] as Range).HorizontalAlignment = XlHAlign.xlHAlignRight;
                (Ws.Cells[Row, 5] as Range).Font.Size = 11;
                (Ws.Cells[Row, 8] as Range).Font.Size = 11;
                (Ws.Cells[Row, 11] as Range).Font.Size = 11;
                (Ws.Cells[Row, 12] as Range).Font.Size = 11;
                Ws.Cells[Row, 5] = "Всего: ";


                Ws.Cells[Row, 8] = ConsolidTmp.Sum(a => a.Price) + ConsolidTmpToGo.Sum(a => a.Price) + ToGoOrdersModelSingleton.Instance.Orders.Sum(a => a.DeliveryPrice);
                if (ConsolidTmp.Sum(a => a.Price - a.DiscPrice) + ConsolidTmpToGo.Sum(a => a.Price - a.DiscPrice) > 0)
                {
                    Ws.Cells[Row, 11] = ConsolidTmp.Sum(a => a.Price - a.DiscPrice) + ConsolidTmpToGo.Sum(a => a.Price - a.DiscPrice);
                }
                Ws.Cells[Row, 12] = ConsolidTmp.Sum(a => a.DiscPrice) + ConsolidTmpToGo.Sum(a => a.DiscPrice) + ToGoOrdersModelSingleton.Instance.Orders.Sum(a => a.DeliveryPrice);
                //  app.Visible = true;
            }
            catch
            { }
        }


        internal void DBFGetRashDishez(DateTime StartDate, DateTime StopDate)
        {
            try
            {
                OpenXls();
                //   List<ReportDish> Tmp = FromSql.DBFGetRashDishez(StartDate, StopDate);
                List<ReportDish> Tmp = SQLGetRashDishez(StartDate, StopDate.AddDays(1));

                List<ReportDish> ConsolidTmp = new List<ReportDish>();

                foreach (ReportDish rd in Tmp)
                {
                    ReportDish rd2;
                    if (ConsolidTmp.Exists(a => a.BarCode == rd.BarCode))
                    {
                        rd2 = ConsolidTmp.Where(a => a.BarCode == rd.BarCode).FirstOrDefault();
                        rd2.Count += rd.Count;
                        rd2.Price += rd.Price * rd.Count;
                        rd2.DiscPrice += rd.DiscPrice * rd.Count;
                    }
                    else
                    {
                        rd2 = new ReportDish();
                        rd2.Name = rd.Name;
                        rd2.BarCode = rd.BarCode;
                        rd2.Count = rd.Count;
                        rd2.Price = rd.Price * rd.Count;
                        rd2.DiscPrice = rd.DiscPrice * rd.Count;
                        ConsolidTmp.Add(rd2);
                    }
                }


                //Ws.Cells[2, 1] = "Критерии";
                Worksheet Ws = AddWs("Расход блюд");
                Ws.get_Range("A1:M1000").Font.Name = "Antica";
                Ws.get_Range("A1:M7").Font.Name = "Helica";
                Ws.get_Range("A1:M1000").Font.Size = 10;

                Ws.Cells[1, 5] = "UCS R-Keeper Reports";
                Ws.Cells[1, 6] = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                (Ws.Cells[1, 7] as Range).Font.Italic = true;
                (Ws.Cells[1, 7] as Range).Font.Size = 10;
                (Ws.Cells[1, 7] as Range).Font.Underline = true;
                Ws.Cells[1, 7] = "Галерея";



                (Ws.Cells[3, 6] as Range).Font.Bold = true;
                (Ws.Cells[3, 6] as Range).Font.Italic = true;
                (Ws.Cells[3, 6] as Range).Font.Size = 16;
                (Ws.Cells[3, 6] as Range).Font.Name = "Helica";

                Ws.Cells[3, 6] = "Расход блюд";
                Ws.Cells[6, 4] = "период:";
                (Ws.Cells[6, 6] as Range).Font.Bold = true;
                (Ws.Cells[6, 6] as Range).Font.Underline = true;
                (Ws.Cells[6, 6] as Range).Font.Size = 10;
                (Ws.Cells[6, 6] as Range).Font.Name = "Helica";
                Ws.Cells[6, 6] = StartDate.ToString("dd.MM.yyyy") + " - " + StopDate.ToString("dd.MM.yyyy");
                Ws.Cells[6, 7] = "вкл.";

                Ws.get_Range("A8:P8").Font.Bold = true;
                Ws.Cells[8, 2] = "код";
                Ws.Cells[8, 3] = "название";
                Ws.Cells[8, 6] = "кол-во";
                Ws.Cells[8, 8] = "сумма";
                Ws.Cells[8, 9] = "ср. цена";
                Ws.Cells[8, 11] = "скидка";
                Ws.Cells[8, 12] = "оплачено";

                int Row = 10;
                foreach (ReportDish rd in ConsolidTmp)
                {
                    /*
                    if (rd.BarCode > 100000)
                    {
                        Ws.Cells[Row, 2] = (rd.BarCode - 100000).ToString();
                    }
                    else
                    {
                        Ws.Cells[Row, 2] = rd.BarCode.ToString();
                    }

        */
                    Ws.Cells[Row, 2] = rd.BarCode.ToString();
                    //Ws.Cells[Row, 3] =Encoding.GetEncoding(1251).GetString(Encoding.GetEncoding(1252).GetBytes( rd.Name));
                    Ws.Cells[Row, 3] = rd.Name;
                    Ws.Cells[Row, 6] = rd.Count;
                    Ws.Cells[Row, 8] = rd.Price;
                    Ws.Cells[Row, 9] = rd.Price / rd.Count;
                    if (rd.Price - rd.DiscPrice > 0)
                    {
                        Ws.Cells[Row, 11] = rd.Price - rd.DiscPrice;
                    }
                    Ws.Cells[Row, 12] = rd.DiscPrice;
                    Row++;

                }

                Row++;
                // Ws.get_Range(Ws.Cells[Row, 1], Ws.Cells[Row, 20]).Font.Bold = true;


                (Ws.Cells[Row, 5] as Range).Font.Bold = true;
                (Ws.Cells[Row, 8] as Range).Font.Bold = true;
                (Ws.Cells[Row, 12] as Range).Font.Bold = true;
                Ws.Cells[Row, 5] = "Всего";
                Ws.Cells[Row, 8] = ConsolidTmp.Sum(a => a.Price);
                if (ConsolidTmp.Sum(a => a.Price - a.DiscPrice) > 0)
                {
                    Ws.Cells[Row, 11] = ConsolidTmp.Sum(a => a.Price - a.DiscPrice);
                }
                Ws.Cells[Row, 12] = ConsolidTmp.Sum(a => a.DiscPrice);

                //app.Save(System.Reflection.Missing.Value);

                //app = null;
                //    app.Visible = true;
            }
            catch
            { }
        }


    }

    public class ReportDish
    {
        public long Id { set; get; }
        public long BarCode { set; get; }
        public decimal Count = 1;
        public int Reason { set; get; }
        public string Name { set; get; }
        public string ReasonName { set; get; }
        public decimal Price { set; get; }
        public decimal DiscPrice { set; get; }
        public long TenderType { set; get; }
        public Dictionary<long, decimal> TenderAmount { set; get; }
        public string GroupeName { set; get; }
    }

    public class ReportTNDR
    {
        public long Id { set; get; }
        public decimal Summ = 1;
        public string Name { set; get; }
        public DateTime dt { set; get; }
        public bool Cash { set; get; }

    }
}
