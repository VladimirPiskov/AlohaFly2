using System.Windows.Controls;

namespace AlohaFly.UI
{
    /// <summary>
    /// Логика взаимодействия для CtrlMain.xaml
    /// </summary>
   // public partial class CtrlMain : Telerik.Windows.Controls.RadWindow
    public partial class CtrlMain : UserControl
    {
        public CtrlMain()
        {
            InitializeComponent();

        }

        private void RadDocking_PreviewClose(object sender, Telerik.Windows.Controls.Docking.StateChangeEventArgs e)
        {
            (mainUC.DataContext as Models.MainUIViewModel).model.RadDocking_PreviewClose(sender, e);
        }
    }
}
