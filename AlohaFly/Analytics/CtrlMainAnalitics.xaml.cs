using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace AlohaFly.Analytics
{
    /// <summary>
    /// Логика взаимодействия для CtrlMainAnalitics.xaml
    /// </summary>
    public partial class CtrlMainAnalitics : UserControl
    {
        public CtrlMainAnalitics()
        {
            InitializeComponent();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnToFlyExport_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcel(GrPenToFly);
        }
        private void ExportToExcel(RadGridView gv)
        {
            string extension = "xls";
            RadSaveFileDialog dialog = new RadSaveFileDialog()
            {
                DefaultExt = extension,
                Filter = String.Format("{1} files (.{0})|.{0}|All files (.)|.", extension, "Excel"),
                FilterIndex = 1
            };
            if (dialog.ShowDialog() == true)
            {
                using (Stream stream = dialog.OpenFile())
                {
                    gv.Export(stream,
                         new GridViewExportOptions()
                         {
                             Format = ExportFormat.Html,
                             ShowColumnHeaders = true,
                             ShowColumnFooters = true,
                             ShowGroupFooters = false,
                         });
                }
            }
        }

        private void btnToGoExport_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcel(GrPenToGo);
        }
    }
}
