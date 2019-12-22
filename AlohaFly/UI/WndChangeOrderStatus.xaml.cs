using System;
using System.Windows;
using Telerik.Windows.Controls;

namespace AlohaFly.UI
{
    /// <summary>
    /// Логика взаимодействия для WndChangeOrderStatus.xaml
    /// </summary>
    public partial class WndChangeOrderStatus : RadWindow
    {
        public WndChangeOrderStatus()
        {
            InitializeComponent();
        }

        private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ((Models.ChangeOrderStatusViewModel)e.NewValue).CloseAction = new Action(this.Close);
        }
    }
}
