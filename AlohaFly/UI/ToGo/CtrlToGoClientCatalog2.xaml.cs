using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace AlohaFly.UI.ToGo
{
    /// <summary>
    /// Логика взаимодействия для CtrlToGoClientCatalog.xaml
    /// </summary>
    public partial class CtrlToGoClientCatalog2 : UserControl
    {
        public CtrlToGoClientCatalog2()
        {
            InitializeComponent();
        }

        private void RadGridView_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            var gv = sender as RadGridView;

            gv.ScrollIntoViewAsync(gv.SelectedItem, _ => { });
        }
    }
}
