using System.Windows.Controls;
using System.Windows.Input;

namespace AlohaFly.UI
{
    /// <summary>
    /// Логика взаимодействия для CtrlToGoOrders.xaml
    /// </summary>
    public partial class CtrlToGoOrders : UserControl
    {
        public CtrlToGoOrders()
        {
            InitializeComponent();
        }

        private void RadGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                (this.DataContext as Models.ToGoOrdersViewModel).EditOrderCommand.Execute(this);
            }
            catch
            {
            }
        }
    }
}
