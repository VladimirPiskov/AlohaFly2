using AlohaService.ServiceDataContracts;
using System;
using System.Collections.Generic;
using System.IO;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.Spreadsheet.Model;

namespace AlohaFly.Reports
{
    public static class PDFExport
    {

        public static void AirCopmInvoicesToPDFRus(List<OrderFlight> orders, Func<OrderFlight, bool, Workbook> getWbFunc, bool showDiscount = false)
        {
            var of = new RadOpenFolderDialog();
            of.Owner = MainClass.MainAppwindow;
            of.InitialDirectory = Properties.Settings.Default.LastDirectory;
            of.ShowDialog();
            of.ExpandToCurrentDirectory = false;
            if (of.DialogResult.GetValueOrDefault())
            {
                Properties.Settings.Default.LastDirectory = of.FileName;
                Properties.Settings.Default.Save();
                foreach (var ord in orders)
                {

                    //string filePath = of.FileName+@"\"+ord.Id.ToString()+"_"+ord.FlightNumber+".pdf";
                    string filePath = of.FileName + @"\" + ord.DeliveryDate.ToString("dd.MM.yyyy") + "_" + ord.FlightNumber + ".pdf";
                    //var wb = AlohaService.ExcelExport.ExportHelper.ExportToExcelWorkbookRussian(ord);
                    int num = 0;
                    while (File.Exists(filePath))
                    {
                        filePath = filePath.Substring(0, filePath.Length - 4) + "_" + (num++).ToString() + ".pdf";
                    }
                    var wb = getWbFunc(ord, showDiscount);
                    InvoiceToPDF(wb, filePath);
                }

            }
        }





        static void InvoiceToPDF(Workbook workbook, string filePath)
        {
            Telerik.Windows.Documents.Spreadsheet.FormatProviders.Pdf.PdfFormatProvider pdfFormatProvider = new Telerik.Windows.Documents.Spreadsheet.FormatProviders.Pdf.PdfFormatProvider();
            using (Stream output = File.OpenWrite(filePath))
            {
                //Workbook workbook = AlohaService.ExcelExport.ExportHelper // The CreateSampleWorkbook() method generates a sample spreadsheet document. Use your Workbook object here. 
                pdfFormatProvider.Export(workbook, output);
            }
        }

    }
}
