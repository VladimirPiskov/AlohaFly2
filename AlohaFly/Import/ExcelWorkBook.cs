using NLog;
using System;

namespace AlohaFly.Import
{
    public class ExcelWorkBook : IDisposable
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        Microsoft.Office.Interop.Excel.Application ObjExcel;
        Microsoft.Office.Interop.Excel.Workbook ObjWorkBook;
        Microsoft.Office.Interop.Excel.Worksheet ObjWorkSheet;
        public Microsoft.Office.Interop.Excel.Worksheet GetWB(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                UI.UIModify.ShowAlert($"Файл {path} отсутствует.");
                return null;
            }


            try
            {
                ObjExcel = new Microsoft.Office.Interop.Excel.Application();
                ObjExcel.Visible = false;
                //ObjWorkBook = ObjExcel.Workbooks.Open(path, 0, true, "", "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
                ObjWorkBook = ObjExcel.Workbooks.Open(path, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                ObjWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)ObjWorkBook.Sheets[1];
                return ObjWorkSheet;
            }
            catch (Exception e)
            {
                logger.Error("Error GetWB " + e.Message);
                throw e;
            }
        }

        public void Dispose()
        {
            try
            {
                if (ObjWorkBook != null)
                {
                    ObjWorkBook.Close();
                }
                if (ObjExcel != null)
                {
                    ObjExcel.Quit();
                }
            }
            catch
            { }
        }


        ~ExcelWorkBook()
        {
            Dispose();
        }




    }
}
