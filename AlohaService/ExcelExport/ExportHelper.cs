using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Windows.Documents.Spreadsheet.Model;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Core;
using Telerik.Windows.Documents.Spreadsheet.Theming;
using System.Windows.Media;
using System.Globalization;
using Telerik.Windows.Documents.Spreadsheet.Model.Printing;
using System.Windows;

namespace AlohaService.ExcelExport
{
    public static class ExportHelper
    {
        public static byte[] ExportToExcelEnglish(ServiceDataContracts.OrderFlight order)
        {
            var formatProvider = new XlsxFormatProvider();
            return formatProvider.Export(ExportToExcelWorkbookEnglish(order));
        }

        public static byte[] ExportToExcelRussian(ServiceDataContracts.OrderFlight order)
        {
            var formatProvider = new XlsxFormatProvider();
            return formatProvider.Export(ExportToExcelWorkbookRussian(order));
        }


       

        public static Workbook ExportToExcelWorkbookEnglish(ServiceDataContracts.OrderFlight order,bool showDiscount=false)
        {
            var workbook = new Workbook();
            var worksheet = workbook.Worksheets.Add();
            worksheet.Columns[0].SetWidth(new ColumnWidth(15, false));

            int rowIndex = 0;
            int colIndex = 1;

            var border1 = new CellBorder(CellBorderStyle.Thin, ThemableColor.FromArgb(255, 0, 0, 0));

            ThemeColorScheme colorScheme = new ThemeColorScheme("ExpenseReport",
            Color.FromArgb(255, 255, 255, 255),    // Background1
            Color.FromArgb(255, 0, 0, 0), // Text1
            Color.FromArgb(255, 255, 255, 255),       // Background2
            Color.FromArgb(255, 0, 0, 0), // Text2
            Color.FromArgb(255, 116, 202, 218), // Accent1
            Color.FromArgb(255, 146, 204, 70),  // Accent2
            Color.FromArgb(255, 241, 96, 61),   // Accent3
            Color.FromArgb(255, 143, 145, 158), // Accent4
            Color.FromArgb(255, 141, 119, 251), // Accent5
            Color.FromArgb(255, 91, 119, 153),  // Accent6
            Color.FromArgb(255, 5, 99, 193),    // Hyperlink
            Color.FromArgb(255, 149, 79, 114)); // Followed hyperlink

            ThemeFontScheme fontScheme = new ThemeFontScheme("ExpenseReport", "Calibri", "Calibri");
            DocumentTheme theme = new DocumentTheme("ExpenseReport", colorScheme, fontScheme);
            workbook.Theme = theme;
           
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex].SetIsBold(true);
            worksheet.Cells[rowIndex, colIndex].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex++].SetValue("INVOICE");
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));

            CellIndex cc1 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex cc2 = new CellIndex(rowIndex, colIndex);
            CellIndex cc3 = new CellIndex(rowIndex, colIndex + 1);
            CellIndex cc4 = new CellIndex(rowIndex, colIndex + 1);
            CellIndex cc5 = new CellIndex(rowIndex, colIndex + 1);
            worksheet.Cells[cc1, cc5].Merge();


            rowIndex += 2;
            colIndex = 1;

            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Document" + Environment.NewLine + "Number");
            worksheet.Cells[rowIndex, colIndex - 1].SetIsWrapped(true);
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));
            worksheet.Rows[rowIndex].SetVerticalAlignment(RadVerticalAlignment.Center);

            CellIndex d1 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex d2 = new CellIndex(rowIndex, colIndex);
            worksheet.Cells[d1, d2].Merge();
            colIndex++;

            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("2FLY" + order.Id + "-" + order.FlightNumber);

            rowIndex += 2;
            colIndex = 1;

            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Date");
            worksheet.Rows[rowIndex].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));
            CellIndex dd1 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex dd2 = new CellIndex(rowIndex, colIndex);
            worksheet.Cells[dd1, dd2].Merge();
            colIndex++;

            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Cells[rowIndex, colIndex].SetHorizontalAlignment(RadHorizontalAlignment.Left);
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue(order.DeliveryDate.ToString("dd.MM.yyyy"));

            rowIndex += 2;
            colIndex = 1;
           
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Supplier");
            worksheet.Rows[rowIndex].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));
            CellIndex s1 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex s2 = new CellIndex(rowIndex, colIndex);
            worksheet.Cells[s1, s2].Merge();
            colIndex++;
            
            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Gallery TO FLY");
            
            rowIndex += 2;
            colIndex = 1;
           

            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Company");
            worksheet.Rows[rowIndex].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));
            CellIndex cp1 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex cp2 = new CellIndex(rowIndex, colIndex);
            worksheet.Cells[cp1, cp2].Merge();
            colIndex++;

            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue(order.AirCompany.Name);

            rowIndex += 2;
            colIndex = 1;

            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Comments");
            worksheet.Rows[rowIndex].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));
            CellIndex cm1 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex cm2 = new CellIndex(rowIndex, colIndex);
            worksheet.Cells[cm1, cm2].Merge();
            colIndex++;


            CellIndex cp11 = new CellIndex(rowIndex, colIndex);
            CellIndex cp21 = new CellIndex(rowIndex, colIndex+3);
            worksheet.Cells[cp11, cp21].Merge();

            worksheet.Cells[rowIndex, colIndex].SetIsWrapped(true);
            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue(String.Format("{0} at {1}, {2}, {3}, {4}",
             order.DeliveryPlace?.Name==null?"":  Helpers.Transliter.GetTranslit(order.DeliveryPlace?.Name),
                //order.FlightNumber,
                order.DeliveryDate.ToString("HH:mm"),
                (showDiscount ? order.OrderTotalSumm.ToString("C", CultureInfo.CreateSpecificCulture("ru-RU")) : order.OrderSumm.ToString("C", CultureInfo.CreateSpecificCulture("ru-RU"))),
                
                order.ContactPerson == null ? "" : Helpers.Transliter.GetTranslit(order.ContactPerson.FullName),
                order.PhoneNumber));

            colIndex = 1;
            rowIndex += 1;

            CellIndex c1 = new CellIndex(rowIndex, colIndex);
            worksheet.Cells[c1].SetBorders(new CellBorders(border1, border1, null, border1, null, null, null, null));

            CellIndex c2 = new CellIndex(rowIndex, colIndex + 1);
            worksheet.Cells[c2].SetBorders(new CellBorders(null, border1, null, border1, null, null, null, null));

            CellIndex c3 = new CellIndex(rowIndex, colIndex + 2);
            worksheet.Cells[c3].SetBorders(new CellBorders(null, border1, null, border1, null, null, null, null));

            CellIndex c4 = new CellIndex(rowIndex, colIndex + 3);

            /*
            worksheet.Cells[c1, c4].Merge();

            worksheet.Cells[c1].SetValue("Товар");
            worksheet.Cells[c1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[c1].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[c4].SetBorders(new CellBorders(null, border1, border1, border1, null, null, null, null));
            */

            colIndex = 1;
            //rowIndex += 1;

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(50, false));
            worksheet.Cells[rowIndex, colIndex].SetFontSize(10.5);
            worksheet.Cells[rowIndex, colIndex].SetFontFamily(new ThemableFontFamily("Arial"));
            worksheet.Cells[rowIndex, colIndex++].SetValue("№");
            worksheet.Cells[rowIndex, colIndex - 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex - 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(70, false));

            worksheet.Cells[rowIndex, colIndex].SetFontSize(10.5);
            worksheet.Cells[rowIndex, colIndex].SetFontFamily(new ThemableFontFamily("Arial"));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Code");
            worksheet.Cells[rowIndex, colIndex - 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex - 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(230, false));
            worksheet.Cells[rowIndex, colIndex].SetFontSize(10.5);
            worksheet.Cells[rowIndex, colIndex].SetFontFamily(new ThemableFontFamily("Arial"));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Items");
            worksheet.Cells[rowIndex, colIndex - 1].SetIsWrapped(true);
            worksheet.Cells[rowIndex, colIndex - 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex - 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);

           

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(70, false));
            worksheet.Cells[rowIndex, colIndex].SetFontSize(10.5);
            worksheet.Cells[rowIndex, colIndex].SetFontFamily(new ThemableFontFamily("Arial"));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Qnt");
            worksheet.Cells[rowIndex, colIndex - 1].SetIsWrapped(true);
            worksheet.Cells[rowIndex, colIndex - 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex - 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            /*
            c1 = new CellIndex(rowIndex, colIndex - 1);
            c2 = new CellIndex(rowIndex - 1, colIndex - 1);
            worksheet.Cells[c1, c2].Merge();
            */
            worksheet.Cells[rowIndex, colIndex - 1].SetBorders(new CellBorders(border1, border1, border1, null, null, null, null, null));

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(70, false));
            worksheet.Cells[rowIndex, colIndex].SetFontSize(10.5);
            worksheet.Cells[rowIndex, colIndex].SetFontFamily(new ThemableFontFamily("Arial"));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Price");
            worksheet.Cells[rowIndex, colIndex - 1].SetIsWrapped(true);
            worksheet.Cells[rowIndex, colIndex - 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex - 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            /*
            c1 = new CellIndex(rowIndex, colIndex - 1);
            c2 = new CellIndex(rowIndex - 1, colIndex - 1);
            worksheet.Cells[c1, c2].Merge();
            */
            worksheet.Cells[rowIndex, colIndex - 1].SetBorders(new CellBorders(border1, border1, border1, null, null, null, null, null));

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(70, false));
            worksheet.Cells[rowIndex, colIndex].SetFontSize(10.5);
            worksheet.Cells[rowIndex, colIndex].SetFontFamily(new ThemableFontFamily("Arial"));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Cost");
            worksheet.Cells[rowIndex, colIndex - 1].SetIsWrapped(true);
            worksheet.Cells[rowIndex, colIndex - 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex - 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            /*
            c1 = new CellIndex(rowIndex, colIndex - 1);
            c2 = new CellIndex(rowIndex - 1, colIndex - 1);
            worksheet.Cells[c1, c2].Merge();
            */
            worksheet.Cells[rowIndex, colIndex - 1].SetBorders(new CellBorders(border1, border1, border1, null, null, null, null, null));
            worksheet.Cells[rowIndex, colIndex - 1].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));


           
            var number = 0;

         

            foreach (var package in order.DishPackagesNoSpis.OrderBy(a => a.PositionInOrder))
            {
                number++;
                colIndex = 1;
                rowIndex++;

                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue(number.ToString());

                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue(package.Dish.Barcode);

                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex].SetIsWrapped(true);
                if (package.Dish.IsAlcohol)
                {
                    worksheet.Cells[rowIndex, colIndex++].SetValue("Open drink");
                }
                else
                {
                    worksheet.Cells[rowIndex, colIndex++].SetValue(package.Dish.EnglishName);
                }

                /*
                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue("порц");
                */
                //totalQnt += package.Amount;
                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue(package.Amount.ToString());

                //  totalPrice += package.TotalPrice;
                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue(package.TotalPrice.ToString());

                //  totalSumm += package.TotalSumm;
                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue(package.TotalSumm.ToString());

                /*
                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue("0");

                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue(package.TotalSumm.ToString());
                */
            }




            if (order.ExtraCharge > 0)
            {
                number++;
                colIndex = 1;
                rowIndex++;

                for (int cn = 1; cn < 7; cn++)
                {
                    worksheet.Cells[rowIndex, cn].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                }

                //   worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue(number.ToString());

                //   worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                //worksheet.Cells[rowIndex, colIndex++].SetValue(package.Dish.Barcode);
                colIndex++;

                //  worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex].SetIsWrapped(true);
                worksheet.Cells[rowIndex, colIndex++].SetValue("Extra charge for urgency 10%");
                colIndex++;
                colIndex++;
                /*
                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue("порц");
                */
                //totalQnt += package.Amount;
                //worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                //worksheet.Cells[rowIndex, colIndex++].SetValue(package.Amount.ToString());

                //  totalPrice += package.TotalPrice;
                //worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                //worksheet.Cells[rowIndex, colIndex++].SetValue(package.TotalPrice.ToString());

                //  totalSumm += package.TotalSumm;
                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue((order.OrderDishesSumm * order.ExtraCharge / 100).ToString());
            }
            if (showDiscount && order.DiscountSumm > 0)
            {
                number++;
                colIndex = 1;
                rowIndex++;

                for (int cn = 1; cn < 7; cn++)
                {
                    worksheet.Cells[rowIndex, cn].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                }
                worksheet.Cells[rowIndex, colIndex++].SetValue(number.ToString());
                colIndex++;
                worksheet.Cells[rowIndex, colIndex].SetIsWrapped(true);
                worksheet.Cells[rowIndex, colIndex++].SetValue($"Discount {(order.DiscountSumm / order.OrderSumm).ToString("P")}");
                colIndex++;
                colIndex++;
                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue((order.DiscountSumm).ToString());
            }
            rowIndex += 1;
            for (int cn = 1; cn < 7; cn++)
            {
                worksheet.Cells[rowIndex, cn].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            }
            //worksheet.Cells[rowIndex, 2].SetBorders(new CellBorders(null, border1, null, border1, null, null, null, null));
            //worksheet.Cells[rowIndex, 3].SetBorders(new CellBorders(null, border1, null, border1, null, null, null, null));


            colIndex = 3;

            //worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(null, border1, border1, border1, null, null, null, null));
            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Cells[rowIndex, colIndex++].SetValue("TOTAL");
            colIndex++;
            colIndex++;
            // worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            //worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            // worksheet.Cells[rowIndex, colIndex++].SetValue(totalQnt.ToString());

            //  worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            //   worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            // worksheet.Cells[rowIndex, colIndex++].SetValue(totalPrice.ToString());


            //  worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);

            worksheet.Cells[rowIndex, colIndex++].SetValue(showDiscount? order.OrderTotalSumm.ToString(): order.OrderSumm.ToString());
            /*
            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Cells[rowIndex, colIndex++].SetValue("0");

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Cells[rowIndex, colIndex++].SetValue(totalSumm.ToString());
            */
            return workbook;
        }


        public static Workbook ExportToGoToExcelWorkbookRussian(ServiceDataContracts.OrderToGo order)
        {


            var workbook = new Workbook();
            var worksheet = workbook.Worksheets.Add();
            worksheet.Columns[0].SetWidth(new ColumnWidth(15, false));

            int rowIndex = 0;
            int colIndex = 1;

            var border1 = new CellBorder(CellBorderStyle.Thin, ThemableColor.FromArgb(255, 0, 0, 0));

            ThemeColorScheme colorScheme = new ThemeColorScheme("ExpenseReport",
            Color.FromArgb(255, 255, 255, 255),    // Background1
            Color.FromArgb(255, 0, 0, 0), // Text1
            Color.FromArgb(255, 255, 255, 255),       // Background2
            Color.FromArgb(255, 0, 0, 0), // Text2
            Color.FromArgb(255, 116, 202, 218), // Accent1
            Color.FromArgb(255, 146, 204, 70),  // Accent2
            Color.FromArgb(255, 241, 96, 61),   // Accent3
            Color.FromArgb(255, 143, 145, 158), // Accent4
            Color.FromArgb(255, 141, 119, 251), // Accent5
            Color.FromArgb(255, 91, 119, 153),  // Accent6
            Color.FromArgb(255, 5, 99, 193),    // Hyperlink
            Color.FromArgb(255, 149, 79, 114)); // Followed hyperlink

            ThemeFontScheme fontScheme = new ThemeFontScheme("ExpenseReport", "Calibri", "Calibri");
            DocumentTheme theme = new DocumentTheme("ExpenseReport", colorScheme, fontScheme);
            workbook.Theme = theme;

            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Организация:");

            CellIndex c11 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex c22 = new CellIndex(rowIndex, colIndex);
            worksheet.Cells[c11, c22].Merge();
            colIndex++;


            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(150, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("ООО \"Флайфуд\" ИНН 7707831796, КПП 770701001, "
                + Environment.NewLine +
                "107031, г. Москва, Петровка ул. Дом 27");
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));
            worksheet.Cells[rowIndex, colIndex - 1].SetIsWrapped(true);

            CellIndex c111 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex c222 = new CellIndex(rowIndex, colIndex);
            CellIndex c333 = new CellIndex(rowIndex, colIndex + 1);
            CellIndex c444 = new CellIndex(rowIndex, colIndex + 1);
            CellIndex c555 = new CellIndex(rowIndex, colIndex + 1);
            worksheet.Cells[c111, c555].Merge();
            worksheet.Cells[c111].SetBorders(new CellBorders(null, null, null, border1, null, null, null, null));
            worksheet.Cells[c222].SetBorders(new CellBorders(null, null, null, border1, null, null, null, null));
            worksheet.Cells[c333].SetBorders(new CellBorders(null, null, null, border1, null, null, null, null));
            worksheet.Cells[c444].SetBorders(new CellBorders(null, null, null, border1, null, null, null, null));
            worksheet.Cells[c555].SetBorders(new CellBorders(null, null, null, border1, null, null, null, null));

            rowIndex += 2;
            colIndex = 3;

            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex].SetIsBold(true);
            worksheet.Cells[rowIndex, colIndex].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex++].SetValue("Заказ");
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));

            CellIndex cc1 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex cc2 = new CellIndex(rowIndex, colIndex);
            CellIndex cc3 = new CellIndex(rowIndex, colIndex + 1);
            CellIndex cc4 = new CellIndex(rowIndex, colIndex + 1);
            CellIndex cc5 = new CellIndex(rowIndex, colIndex + 1);
            worksheet.Cells[cc1, cc5].Merge();


            rowIndex += 2;
            colIndex = 1;

            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Номер" + Environment.NewLine + "документа:");
            worksheet.Cells[rowIndex, colIndex - 1].SetIsWrapped(true);
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));
            worksheet.Rows[rowIndex].SetVerticalAlignment(RadVerticalAlignment.Center);

            CellIndex d1 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex d2 = new CellIndex(rowIndex, colIndex);
            worksheet.Cells[d1, d2].Merge();
            colIndex++;

            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("ToGo" + order.Id );

            rowIndex += 2;
            colIndex = 1;

            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Дата документа:");
            worksheet.Rows[rowIndex].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));
            CellIndex dd1 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex dd2 = new CellIndex(rowIndex, colIndex);
            worksheet.Cells[dd1, dd2].Merge();
            colIndex++;

            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Cells[rowIndex, colIndex].SetHorizontalAlignment(RadHorizontalAlignment.Left);
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue(order.DeliveryDate.ToString("dd.MM.yyyy HH:mm"));
            rowIndex += 2;
            colIndex = 1;
            if (order.MarketingChannelId == 2 || order.MarketingChannelId == 5)
            {

                
                worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
                worksheet.Cells[rowIndex, colIndex++].SetValue("Комментарий:");
                worksheet.Rows[rowIndex].SetVerticalAlignment(RadVerticalAlignment.Center);
                worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));
                dd1 = new CellIndex(rowIndex, colIndex - 1);
                dd2 = new CellIndex(rowIndex, colIndex);
                worksheet.Cells[dd1, dd2].Merge();
                colIndex++;

                worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
                worksheet.Cells[rowIndex, colIndex].SetHorizontalAlignment(RadHorizontalAlignment.Left);
                worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
                worksheet.Cells[rowIndex, colIndex++].SetValue(order.MarketingChannel.Name);
            }
            rowIndex += 2;
            colIndex = 1;


            CellIndex c1 = new CellIndex(rowIndex, colIndex);
            worksheet.Cells[c1].SetBorders(new CellBorders(border1, border1, null, border1, null, null, null, null));

            CellIndex c2 = new CellIndex(rowIndex, colIndex + 1);
            worksheet.Cells[c2].SetBorders(new CellBorders(null, border1, null, border1, null, null, null, null));

            CellIndex c3 = new CellIndex(rowIndex, colIndex + 2);
            worksheet.Cells[c3].SetBorders(new CellBorders(null, border1, null, border1, null, null, null, null));

            CellIndex c4 = new CellIndex(rowIndex, colIndex + 3);



            colIndex = 1;
            //rowIndex += 1;

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(50, false));
            worksheet.Cells[rowIndex, colIndex].SetFontSize(10.5);
            worksheet.Cells[rowIndex, colIndex].SetFontFamily(new ThemableFontFamily("Arial"));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Номер");
            worksheet.Cells[rowIndex, colIndex - 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex - 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(70, false));

            worksheet.Cells[rowIndex, colIndex].SetFontSize(10.5);
            worksheet.Cells[rowIndex, colIndex].SetFontFamily(new ThemableFontFamily("Arial"));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Код");
            worksheet.Cells[rowIndex, colIndex - 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex - 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(230, false));
            worksheet.Cells[rowIndex, colIndex].SetFontSize(10.5);
            worksheet.Cells[rowIndex, colIndex].SetFontFamily(new ThemableFontFamily("Arial"));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Наименование блюда");
            worksheet.Cells[rowIndex, colIndex - 1].SetIsWrapped(true);
            worksheet.Cells[rowIndex, colIndex - 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex - 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);




            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(70, false));
            worksheet.Cells[rowIndex, colIndex].SetFontSize(10.5);
            worksheet.Cells[rowIndex, colIndex].SetFontFamily(new ThemableFontFamily("Arial"));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Количес" + Environment.NewLine + "тво");
            worksheet.Cells[rowIndex, colIndex - 1].SetIsWrapped(true);
            worksheet.Cells[rowIndex, colIndex - 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex - 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            /*
            c1 = new CellIndex(rowIndex, colIndex - 1);
            c2 = new CellIndex(rowIndex - 1, colIndex - 1);
            worksheet.Cells[c1, c2].Merge();
            */
            worksheet.Cells[rowIndex, colIndex - 1].SetBorders(new CellBorders(border1, border1, border1, null, null, null, null, null));

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(70, false));
            worksheet.Cells[rowIndex, colIndex].SetFontSize(10.5);
            worksheet.Cells[rowIndex, colIndex].SetFontFamily(new ThemableFontFamily("Arial"));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Цена" + Environment.NewLine + "продажи," + Environment.NewLine + "₽. коп.");
            worksheet.Cells[rowIndex, colIndex - 1].SetIsWrapped(true);
            worksheet.Cells[rowIndex, colIndex - 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex - 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            /*
            c1 = new CellIndex(rowIndex, colIndex - 1);
            c2 = new CellIndex(rowIndex - 1, colIndex - 1);
            worksheet.Cells[c1, c2].Merge();
            */
            worksheet.Cells[rowIndex, colIndex - 1].SetBorders(new CellBorders(border1, border1, border1, null, null, null, null, null));

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(70, false));
            worksheet.Cells[rowIndex, colIndex].SetFontSize(10.5);
            worksheet.Cells[rowIndex, colIndex].SetFontFamily(new ThemableFontFamily("Arial"));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Сумма");
            worksheet.Cells[rowIndex, colIndex - 1].SetIsWrapped(true);
            worksheet.Cells[rowIndex, colIndex - 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex - 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            /*
            c1 = new CellIndex(rowIndex, colIndex - 1);
            c2 = new CellIndex(rowIndex - 1, colIndex - 1);
            worksheet.Cells[c1, c2].Merge();
            */
            worksheet.Cells[rowIndex, colIndex - 1].SetBorders(new CellBorders(border1, border1, border1, null, null, null, null, null));
            worksheet.Cells[rowIndex, colIndex - 1].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));




            var number = 0;

            //  decimal totalQnt = 0;
            //   decimal totalPrice = 0;
            //   decimal totalSumm = 0;

            foreach (var package in order.DishPackagesNoSpis.OrderBy(a => a.PositionInOrder))
            {
                try
                {

                    number++;
                    colIndex = 1;
                    rowIndex++;

                    worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                    worksheet.Cells[rowIndex, colIndex++].SetValue(number.ToString());

                    worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                    worksheet.Cells[rowIndex, colIndex++].SetValue(package.Dish.Barcode);

                    worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                    worksheet.Cells[rowIndex, colIndex].SetIsWrapped(true);
                    worksheet.Cells[rowIndex, colIndex++].SetValue(package.Dish.RussianName);

                    /*
                    worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                    worksheet.Cells[rowIndex, colIndex++].SetValue("порц");
                    */
                    //totalQnt += package.Amount;
                    worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                    worksheet.Cells[rowIndex, colIndex++].SetValue(package.Amount.ToString());

                    //  totalPrice += package.TotalPrice;
                    worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                    worksheet.Cells[rowIndex, colIndex++].SetValue(package.TotalPrice.ToString());

                    //  totalSumm += package.TotalSumm;
                    worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                    worksheet.Cells[rowIndex, colIndex++].SetValue(package.TotalSumm.ToString());

                }
                catch(Exception e )
                {
                    
                }
            }




            if (order.DiscountPercent > 0)
            {
                number++;
                colIndex = 1;
                rowIndex++;

                for (int cn = 1; cn < 7; cn++)
                {
                    worksheet.Cells[rowIndex, cn].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                }

                //   worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue(number.ToString());

                //   worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                //worksheet.Cells[rowIndex, colIndex++].SetValue(package.Dish.Barcode);
                colIndex++;

                //  worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex].SetIsWrapped(true);
                worksheet.Cells[rowIndex, colIndex++].SetValue($"Скидка {order.DiscountPercent}%");
                colIndex++;
                colIndex++;

                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue((order.DiscountSumm).ToString());
            }


            if (order.DeliveryPrice > 0)
            {
                number++;
                colIndex = 1;
                rowIndex++;

                for (int cn = 1; cn < 7; cn++)
                {
                    worksheet.Cells[rowIndex, cn].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                }

                //   worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue(number.ToString());

                //   worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                //worksheet.Cells[rowIndex, colIndex++].SetValue(package.Dish.Barcode);
                colIndex++;

                //  worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex].SetIsWrapped(true);
                worksheet.Cells[rowIndex, colIndex++].SetValue($"Доставка ");
                colIndex++;
                colIndex++;

                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue((order.DeliveryPrice).ToString());
            }


            rowIndex += 1;
            for (int cn = 1; cn < 7; cn++)
            {
                worksheet.Cells[rowIndex, cn].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            }


            colIndex = 3;

            //worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(null, border1, border1, border1, null, null, null, null));
            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Cells[rowIndex, colIndex++].SetValue("ИТОГО");
            colIndex++;
            colIndex++;


            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Cells[rowIndex, colIndex++].SetValue(order.OrderTotalSumm.ToString());

            return workbook;
        }
        

            public static Workbook ExportToExcelWorkbookRussian(ServiceDataContracts.OrderFlight order, bool showDiscount=false)
        {
            var workbook = new Workbook();
            var worksheet = workbook.Worksheets.Add();
            worksheet.Columns[0].SetWidth(new ColumnWidth(15, false));

            int rowIndex = 0;
            int colIndex = 1;

            var border1 = new CellBorder(CellBorderStyle.Thin, ThemableColor.FromArgb(255, 0, 0, 0));

            ThemeColorScheme colorScheme = new ThemeColorScheme("ExpenseReport",
            Color.FromArgb(255, 255, 255, 255),    // Background1
            Color.FromArgb(255, 0, 0, 0), // Text1
            Color.FromArgb(255, 255, 255, 255),       // Background2
            Color.FromArgb(255, 0, 0, 0), // Text2
            Color.FromArgb(255, 116, 202, 218), // Accent1
            Color.FromArgb(255, 146, 204, 70),  // Accent2
            Color.FromArgb(255, 241, 96, 61),   // Accent3
            Color.FromArgb(255, 143, 145, 158), // Accent4
            Color.FromArgb(255, 141, 119, 251), // Accent5
            Color.FromArgb(255, 91, 119, 153),  // Accent6
            Color.FromArgb(255, 5, 99, 193),    // Hyperlink
            Color.FromArgb(255, 149, 79, 114)); // Followed hyperlink

            ThemeFontScheme fontScheme = new ThemeFontScheme("ExpenseReport", "Calibri", "Calibri");
            DocumentTheme theme = new DocumentTheme("ExpenseReport", colorScheme, fontScheme);
            workbook.Theme = theme;

            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Организация:");

            CellIndex c11 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex c22 = new CellIndex(rowIndex, colIndex);
            worksheet.Cells[c11, c22].Merge();
            colIndex++;


            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(150, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("ООО \"Флайфуд\" ИНН 7707831796, КПП 770701001, "
                + Environment.NewLine +
                "107031, г. Москва, Петровка ул. Дом 27");
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));
            worksheet.Cells[rowIndex, colIndex - 1].SetIsWrapped(true);

            CellIndex c111 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex c222 = new CellIndex(rowIndex, colIndex);
            CellIndex c333 = new CellIndex(rowIndex, colIndex + 1);
            CellIndex c444 = new CellIndex(rowIndex, colIndex + 1);
            CellIndex c555 = new CellIndex(rowIndex, colIndex + 1);
            worksheet.Cells[c111, c555].Merge();
            worksheet.Cells[c111].SetBorders(new CellBorders(null, null, null, border1, null, null, null, null));
            worksheet.Cells[c222].SetBorders(new CellBorders(null, null, null, border1, null, null, null, null));
            worksheet.Cells[c333].SetBorders(new CellBorders(null, null, null, border1, null, null, null, null));
            worksheet.Cells[c444].SetBorders(new CellBorders(null, null, null, border1, null, null, null, null));
            worksheet.Cells[c555].SetBorders(new CellBorders(null, null, null, border1, null, null, null, null));

            rowIndex += 2;
            colIndex = 3;

            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex].SetIsBold(true);
            worksheet.Cells[rowIndex, colIndex].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex++].SetValue("РАСХОДНАЯ НАКЛАДНАЯ");
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));

            CellIndex cc1 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex cc2 = new CellIndex(rowIndex, colIndex);
            CellIndex cc3 = new CellIndex(rowIndex, colIndex + 1);
            CellIndex cc4 = new CellIndex(rowIndex, colIndex + 1);
            CellIndex cc5 = new CellIndex(rowIndex, colIndex + 1);
            worksheet.Cells[cc1, cc5].Merge();


            rowIndex += 2;
            colIndex = 1;

            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Номер" + Environment.NewLine + "документа:");
            worksheet.Cells[rowIndex, colIndex - 1].SetIsWrapped(true);
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));
            worksheet.Rows[rowIndex].SetVerticalAlignment(RadVerticalAlignment.Center);

            CellIndex d1 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex d2 = new CellIndex(rowIndex, colIndex);
            worksheet.Cells[d1, d2].Merge();
            colIndex++;

            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("2FLY" + order.Id + "-" + order.FlightNumber);

            rowIndex += 2;
            colIndex = 1;

            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Дата документа:");
            worksheet.Rows[rowIndex].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));
            CellIndex dd1 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex dd2 = new CellIndex(rowIndex, colIndex);
            worksheet.Cells[dd1, dd2].Merge();
            colIndex++;

            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Cells[rowIndex, colIndex].SetHorizontalAlignment(RadHorizontalAlignment.Left);
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue(order.DeliveryDate.ToString("dd.MM.yyyy"));

            rowIndex += 2;
            colIndex = 1;
          

            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Получатель:");
            worksheet.Rows[rowIndex].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));
            CellIndex cp1 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex cp2 = new CellIndex(rowIndex, colIndex);
            worksheet.Cells[cp1, cp2].Merge();
            colIndex++;

            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue(order.AirCompany.Name);

            rowIndex += 2;
            colIndex = 1;

            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Примечание:");
            worksheet.Rows[rowIndex].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));
            CellIndex cm1 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex cm2 = new CellIndex(rowIndex, colIndex);
            worksheet.Cells[cm1, cm2].Merge();
            colIndex++;

            CellIndex cp11 = new CellIndex(rowIndex, colIndex);
            CellIndex cp21 = new CellIndex(rowIndex, colIndex + 3);
            worksheet.Cells[cp11, cp21].Merge();

            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex].SetIsWrapped(true);
            worksheet.Cells[rowIndex, colIndex++].SetValue(String.Format("{0} в {1}, {2}, {3}, {4}",
                order.DeliveryPlace?.Name,
                //order.FlightNumber,
                order.DeliveryDate.ToString("HH:mm"),
                (showDiscount? order.OrderTotalSumm.ToString("C", CultureInfo.CreateSpecificCulture("ru-RU")): order.OrderSumm.ToString("C", CultureInfo.CreateSpecificCulture("ru-RU"))),
                order.ContactPerson == null ? "" : order.ContactPerson.FullName,
                order.PhoneNumber));

            colIndex = 1;
            rowIndex += 1;

            CellIndex c1 = new CellIndex(rowIndex, colIndex);
            worksheet.Cells[c1].SetBorders(new CellBorders(border1, border1, null, border1, null, null, null, null));

            CellIndex c2 = new CellIndex(rowIndex, colIndex + 1);
            worksheet.Cells[c2].SetBorders(new CellBorders(null, border1, null, border1, null, null, null, null));

            CellIndex c3 = new CellIndex(rowIndex, colIndex + 2);
            worksheet.Cells[c3].SetBorders(new CellBorders(null, border1, null, border1, null, null, null, null));

            CellIndex c4 = new CellIndex(rowIndex, colIndex + 3);

          

            colIndex = 1;
            //rowIndex += 1;

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(50, false));
            worksheet.Cells[rowIndex, colIndex].SetFontSize(10.5);
            worksheet.Cells[rowIndex, colIndex].SetFontFamily(new ThemableFontFamily("Arial"));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Номер");
            worksheet.Cells[rowIndex, colIndex - 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex - 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(70, false));

            worksheet.Cells[rowIndex, colIndex].SetFontSize(10.5);
            worksheet.Cells[rowIndex, colIndex].SetFontFamily(new ThemableFontFamily("Arial"));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Код");
            worksheet.Cells[rowIndex, colIndex - 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex - 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(230, false));
            worksheet.Cells[rowIndex, colIndex].SetFontSize(10.5);
            worksheet.Cells[rowIndex, colIndex].SetFontFamily(new ThemableFontFamily("Arial"));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Наименование блюда");
            worksheet.Cells[rowIndex, colIndex - 1].SetIsWrapped(true);
            worksheet.Cells[rowIndex, colIndex - 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex - 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);

       


            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(70, false));
            worksheet.Cells[rowIndex, colIndex].SetFontSize(10.5);
            worksheet.Cells[rowIndex, colIndex].SetFontFamily(new ThemableFontFamily("Arial"));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Количес" + Environment.NewLine + "тво");
            worksheet.Cells[rowIndex, colIndex - 1].SetIsWrapped(true);
            worksheet.Cells[rowIndex, colIndex - 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex - 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            /*
            c1 = new CellIndex(rowIndex, colIndex - 1);
            c2 = new CellIndex(rowIndex - 1, colIndex - 1);
            worksheet.Cells[c1, c2].Merge();
            */
            worksheet.Cells[rowIndex , colIndex - 1].SetBorders(new CellBorders(border1, border1, border1, null, null, null, null, null));

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(70, false));
            worksheet.Cells[rowIndex, colIndex].SetFontSize(10.5);
            worksheet.Cells[rowIndex, colIndex].SetFontFamily(new ThemableFontFamily("Arial"));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Цена" + Environment.NewLine + "продажи," + Environment.NewLine + "₽. коп.");
            worksheet.Cells[rowIndex, colIndex - 1].SetIsWrapped(true);
            worksheet.Cells[rowIndex, colIndex - 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex - 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            /*
            c1 = new CellIndex(rowIndex, colIndex - 1);
            c2 = new CellIndex(rowIndex - 1, colIndex - 1);
            worksheet.Cells[c1, c2].Merge();
            */
            worksheet.Cells[rowIndex, colIndex - 1].SetBorders(new CellBorders(border1, border1, border1, null, null, null, null, null));

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(70, false));
            worksheet.Cells[rowIndex, colIndex].SetFontSize(10.5);
            worksheet.Cells[rowIndex, colIndex].SetFontFamily(new ThemableFontFamily("Arial"));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Сумма");
            worksheet.Cells[rowIndex , colIndex - 1].SetIsWrapped(true);
            worksheet.Cells[rowIndex , colIndex - 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex , colIndex - 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            /*
            c1 = new CellIndex(rowIndex, colIndex - 1);
            c2 = new CellIndex(rowIndex - 1, colIndex - 1);
            worksheet.Cells[c1, c2].Merge();
            */
            worksheet.Cells[rowIndex , colIndex - 1].SetBorders(new CellBorders(border1, border1, border1, null, null, null, null, null));
            worksheet.Cells[rowIndex, colIndex - 1].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));


          

            var number = 0;

         //  decimal totalQnt = 0;
         //   decimal totalPrice = 0;
         //   decimal totalSumm = 0;

            foreach (var package in order.DishPackagesNoSpis.OrderBy(a=>a.PositionInOrder))
            {
                number++;
                colIndex = 1;
                rowIndex++;

                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue(number.ToString());

                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue(package.Dish.Barcode);

                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex].SetIsWrapped(true);
                if (package.Dish.IsAlcohol)
                {
                    worksheet.Cells[rowIndex, colIndex++].SetValue("Открытый напиток");
                }
                else
                {
                    worksheet.Cells[rowIndex, colIndex++].SetValue(package.Dish.RussianName);
                }

                /*
                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue("порц");
                */
                //totalQnt += package.Amount;
                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue(package.Amount.ToString());

              //  totalPrice += package.TotalPrice;
                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue(package.TotalPrice.ToString());

              //  totalSumm += package.TotalSumm;
                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue(package.TotalSumm.ToString());

             
            }


            

            if (order.ExtraCharge > 0)
            {
                number++;
                colIndex = 1;
                rowIndex++;

                for (int cn = 1; cn < 7; cn++)
                {
                    worksheet.Cells[rowIndex, cn].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                }

                //   worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue(number.ToString());

             //   worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                //worksheet.Cells[rowIndex, colIndex++].SetValue(package.Dish.Barcode);
                colIndex++;

              //  worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex].SetIsWrapped(true);
                worksheet.Cells[rowIndex, colIndex++].SetValue("Наценка за срочность 10%");
                colIndex++;
                colIndex++;
               
                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue((order.OrderDishesSumm*order.ExtraCharge/100).ToString());

                
                
            }
            if (showDiscount && order.DiscountSumm > 0)
            {
                number++;
                colIndex = 1;
                rowIndex++;


                for (int cn = 1; cn < 7; cn++)
                {
                    worksheet.Cells[rowIndex, cn].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                }
                worksheet.Cells[rowIndex, colIndex++].SetValue(number.ToString());
                colIndex++;

                worksheet.Cells[rowIndex, colIndex].SetIsWrapped(true);
                worksheet.Cells[rowIndex, colIndex++].SetValue($"Скидка {(order.DiscountSumm / order.OrderSumm).ToString("P")}");
                colIndex++;
                colIndex++;
                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue((order.DiscountSumm).ToString());
            }


            rowIndex += 1;
            for (int cn = 1; cn < 7; cn++)
            {
                worksheet.Cells[rowIndex, cn].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            }
          

            colIndex = 3;

           //worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(null, border1, border1, border1, null, null, null, null));
            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Cells[rowIndex, colIndex++].SetValue("ИТОГО");
            colIndex++;
            colIndex++;
          
            
            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Cells[rowIndex, colIndex++].SetValue(
                showDiscount? order.OrderTotalSumm.ToString():order.OrderSumm.ToString()
                );
         
            return workbook;
        }

        public static Workbook ExportMenuToExcelWorkbook(ServiceDataContracts.OrderFlight order)
        {
            var workbook = new Workbook();
            var worksheet = workbook.Worksheets.Add();
            worksheet.WorksheetPageSetup.PageOrientation = Telerik.Windows.Documents.Model.PageOrientation.Landscape;
            PageBreaks pageBreaks = workbook.ActiveWorksheet.WorksheetPageSetup.PageBreaks;

            pageBreaks.TryInsertHorizontalPageBreak(34, 7);

            worksheet.WorksheetPageSetup.PaperType = Telerik.Windows.Documents.Model.PaperTypes.A4;
            worksheet.WorksheetPageSetup.PageOrientation = Telerik.Windows.Documents.Model.PageOrientation.Landscape;
            worksheet.WorksheetPageSetup.ScaleFactor = new Size(0.9, 0.9);
            worksheet.WorksheetPageSetup.CenterHorizontally = true;

            worksheet.Rows.SetDefaultHeight(new RowHeight(25, true));
            worksheet.Columns[0].SetWidth(new ColumnWidth(20, false));
            worksheet.Columns[1].SetWidth(new ColumnWidth(335, false));
            worksheet.Columns[2].SetWidth(new ColumnWidth(85, false));
            worksheet.Columns[3].SetWidth(new ColumnWidth(85, false));
            worksheet.Columns[4].SetWidth(new ColumnWidth(335, false));
            worksheet.Columns[5].SetWidth(new ColumnWidth(20, false));

            worksheet.Columns[0].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Columns[1].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Columns[2].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Columns[3].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Columns[4].SetHorizontalAlignment(RadHorizontalAlignment.Center);

            //worksheet.Columns[0].SetIsWrapped(true);
            worksheet.Columns[1].SetIsWrapped(true);
            worksheet.Columns[2].SetIsWrapped(true);
            worksheet.Columns[3].SetIsWrapped(true);
            worksheet.Columns[4].SetIsWrapped(true);

            int rowIndex = 0;

            var border1 = new CellBorder(CellBorderStyle.Thin, ThemableColor.FromArgb(255, 0, 0, 0));

            ThemeColorScheme colorScheme = new ThemeColorScheme("ExpenseReport",
            Color.FromArgb(255, 255, 255, 255),    // Background1
            Color.FromArgb(255, 0, 0, 0), // Text1
            Color.FromArgb(255, 255, 255, 255),       // Background2
            Color.FromArgb(255, 0, 0, 0), // Text2
            Color.FromArgb(255, 116, 202, 218), // Accent1
            Color.FromArgb(255, 146, 204, 70),  // Accent2
            Color.FromArgb(255, 241, 96, 61),   // Accent3
            Color.FromArgb(255, 143, 145, 158), // Accent4
            Color.FromArgb(255, 141, 119, 251), // Accent5
            Color.FromArgb(255, 91, 119, 153),  // Accent6
            Color.FromArgb(255, 5, 99, 193),    // Hyperlink
            Color.FromArgb(255, 149, 79, 114)); // Followed hyperlink

            ThemeFontScheme fontScheme = new ThemeFontScheme("ExpenseReport", "Cyrillic", "Cyrillic");
            DocumentTheme theme = new DocumentTheme("ExpenseReport", colorScheme, fontScheme);
            workbook.Theme = theme;

            Dictionary<ServiceDataContracts.DishKitchenGroup, List<ServiceDataContracts.Dish>> groups = new Dictionary<ServiceDataContracts.DishKitchenGroup, List<ServiceDataContracts.Dish>>();

            var otherKey = new ServiceDataContracts.DishKitchenGroup { Name = "Другое", EnglishName = "Other" };

            groups[otherKey] = new List<ServiceDataContracts.Dish>();
            foreach (var package in order.DishPackagesNoSpis)
            {
                if(package.Dish.DishKitсhenGroup == null)
                {
                    groups[otherKey].Add(package.Dish);
                    continue;
                }

                if(!groups.ContainsKey(package.Dish.DishKitсhenGroup))
                {
                    groups[package.Dish.DishKitсhenGroup] = new List<ServiceDataContracts.Dish>();
                }

                groups[package.Dish.DishKitсhenGroup].Add(package.Dish);
            }

            rowIndex++;

            foreach(var group in groups.Keys)
            {
                if (groups[group].Any(d => d.NeedPrintInMenu))
                {
                    rowIndex++;
                    worksheet.Cells[rowIndex, 1].SetValue(group.Name);
                    worksheet.Cells[rowIndex, 1].SetFontSize(21);
                    worksheet.Cells[rowIndex, 1].SetIsBold(true);
                    worksheet.Rows[rowIndex].SetHeight(new RowHeight(25, true));
                    worksheet.Cells[rowIndex, 4].SetValue(group.EnglishName);
                    worksheet.Cells[rowIndex, 4].SetFontSize(21);
                    worksheet.Cells[rowIndex, 4].SetIsBold(true);

                    foreach (var dish in groups[group])
                    {

                        if (dish.NeedPrintInMenu)
                        {
                            rowIndex++;
                            worksheet.Cells[rowIndex, 1].SetValue(dish.RussianName);
                            worksheet.Cells[rowIndex, 1].SetFontSize(16);
                            worksheet.Rows[rowIndex].SetHeight(new RowHeight(25, true));
                            worksheet.Cells[rowIndex, 4].SetValue(dish.EnglishName);
                            worksheet.Cells[rowIndex, 4].SetFontSize(16);
                        }

                    }
                }
            }

            rowIndex++;
            worksheet.Cells[rowIndex, 1].SetValue(" ");
            worksheet.Cells[rowIndex, 5].SetValue(" ");
            rowIndex++;
            worksheet.Cells[rowIndex, 1].SetValue(" ");
            worksheet.Cells[rowIndex, 5].SetValue(" ");


            return workbook;
        }

        public static byte[] ExportMenuToExcel(ServiceDataContracts.OrderFlight order)
        {
            var formatProvider = new XlsxFormatProvider();
            return formatProvider.Export(ExportMenuToExcelWorkbook(order));
        }
    }
}