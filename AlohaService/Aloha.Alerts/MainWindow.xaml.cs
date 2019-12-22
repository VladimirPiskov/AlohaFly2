using Telerik.Windows.Controls;

namespace Aloha.Alerts
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RadRibbonWindow
    {
        static MainWindow()
        {
            StyleManager.ApplicationTheme = new Office2013Theme();
            RadRibbonWindow.IsWindowsThemeEnabled = false;
        }

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
