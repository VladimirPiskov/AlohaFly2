using System;
using Telerik.Windows.Documents.Spreadsheet.Model;

namespace AlohaFly.Models
{
    public class InvoiceGenerateViewModel : ViewModelPane
    {

        //OrderFlight Order;
        //InvoiceGenerateType GenerateType;
        Func<Workbook> GetExcelDocFunc;

        public InvoiceGenerateViewModel(Workbook wb)
        {
            //Order = order;
            InvoiceWorkbook = wb;
        }


        private Workbook invoiceWorkbook;

        public Workbook InvoiceWorkbook
        {
            get
            {


                return this.invoiceWorkbook;
            }
            set
            {
                if (this.invoiceWorkbook != value)
                {
                    this.invoiceWorkbook = value;
                    this.OnPropertyChanged("InvoiceWorkbook");
                }
            }
        }

    }

    public enum InvoiceGenerateType
    {
        Russian = 0,
        English = 1,
        RussianD = 2,
        EnglishD = 3,
    }
}
