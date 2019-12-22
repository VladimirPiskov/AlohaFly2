using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace AlohaFly
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainClass.Init(this);
        }
        public void SetUserName(string name)
        {
            this.Title = $"AlohaFly. {DBProvider.TestStr} Пользователь: {name}";
        }
        public void SetMainControl(UserControl ctrl)
        {
            mainGrid.Children.Add(ctrl);
        }
        public void SetMainControl(RadWindow ctrl)
        {
            mainGrid.Children.Add(ctrl);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var res = UI.UIModify.ShowConfirm("Вы действительно хотите выйти?");
            if (res != null)
            {
                if (!res.DialogResult.GetValueOrDefault())
                {
                    e.Cancel = true;
                    return;
                }
                
            }
            DataExtension.RealTimeUpdaterSingleton.Instance.StopQueue();
        }
    }
}
