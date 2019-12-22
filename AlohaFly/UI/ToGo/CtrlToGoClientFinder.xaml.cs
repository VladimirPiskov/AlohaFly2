using System.Windows.Controls;
using System.Windows.Input;

namespace AlohaFly.UI
{
    /// <summary>
    /// Логика взаимодействия для CtrlToGoFinder.xaml
    /// </summary>
    public partial class CtrlToGoClientFinder : UserControl
    {
        public CtrlToGoClientFinder()
        {
            InitializeComponent();
        }

        private void FindWTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                if (ListBoxResult.Items != null && ListBoxResult.Items.Count > 0)
                {

                    ListBoxResult.Focus();
                }
            }

        }

        private void ListBoxResult_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                if (ListBoxResult.SelectedIndex == 0)
                {
                    FindWTB.Focus();
                }
            }
            else if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                var dc = (Models.ToGoClient.ToGoClientFinderViewModel)this.DataContext;
                dc.SelectItemCommand.Execute(null);

            }
        }

        private void ListBoxResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dc = (Models.ToGoClient.ToGoClientFinderViewModel)this.DataContext;
            dc.SelectItemCommand.Execute(null);

        }

        private void ListBoxResult_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var dc = (Models.ToGoClient.ToGoClientFinderViewModel)this.DataContext;
            dc.SelectItemCommand.Execute(null);
        }
    }



}
