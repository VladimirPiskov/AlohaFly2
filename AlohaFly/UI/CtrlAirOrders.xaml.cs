using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace AlohaFly.UI
{
    /// <summary>
    /// Логика взаимодействия для CtrlAirOrders.xaml
    /// </summary>
    public partial class CtrlAirOrders : UserControl
    {
        public CtrlAirOrders()
        {
            InitializeComponent();

        }

        private void RadGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                (this.DataContext as Models.AirOrdersViewModel).EditOrderCommand.Execute(this);
            }
            catch
            {
            }
        }

        private void GridViewDataColumn_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as RadRichTextBox).ChangeParagraphLineSpacing(1);
            (sender as RadRichTextBox).Document.ParagraphDefaultSpacingAfter = 0;
            (sender as RadRichTextBox).Document.ParagraphDefaultSpacingBefore = 0;
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Reports.ExportProvider.ExportGridToExcel(ordersGrid);
        }

        private void radRichTextBox1_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as RadRichTextBox).ChangeParagraphLineSpacing(1);
            (sender as RadRichTextBox).Document.ParagraphDefaultSpacingAfter = 0;
            (sender as RadRichTextBox).Document.ParagraphDefaultSpacingBefore = 0;
            (sender as RadRichTextBox).Height = 25;
        }
    }
}
