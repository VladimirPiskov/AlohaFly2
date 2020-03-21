using System.IO;
using Telerik.Windows.Controls;

namespace AlohaFly.Reports
{
    public static class ExportProvider
    {
        public static void ExportGridToExcel(RadGridView gridViewExport)
        {
            string extension = ".xls";
            /*
            SaveFileDialog dialog = new SaveFileDialog()
            {
                DefaultExt = extension,
                Filter = String.Format("{1} files (.{0})|.{0}|All files (.)|.", extension, "Excel"),
                FilterIndex = 1
            };

            if (dialog.ShowDialog() == true)
            */


            var of = new RadSaveFileDialog();
            of.Owner = MainClass.MainAppwindow;
            of.InitialDirectory = Properties.Settings.Default.LastDirectory;
            of.ShowDialog();
            of.ExpandToCurrentDirectory = false;
            // of.Filter = String.Format("{1} files (.{0})|.{0}|All files (.)|.", extension, "Excel");
            if (of.DialogResult.GetValueOrDefault())
            {
                Properties.Settings.Default.LastDirectory = of.FileName;
                Properties.Settings.Default.Save();

                using (Stream stream = new FileStream(of.FileName + extension, FileMode.Create))
                {
                    gridViewExport.Export(stream,
                     new GridViewExportOptions()
                     {
                         Format = ExportFormat.ExcelML,
                         ShowColumnHeaders = true,
                         ShowColumnFooters = true,
                         ShowGroupFooters = false,
                     });
                }
            }

         
        }

    }
}
