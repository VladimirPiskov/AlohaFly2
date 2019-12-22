using System;
using System.Windows;
using Telerik.Windows.Controls;

namespace AlohaFly.UI
{
    /// <summary>
    /// Логика взаимодействия для WndConfirmClose.xaml
    /// </summary>
    public partial class WndConfirmClose : RadWindow
    {
        public WndConfirmClose()
        {
            InitializeComponent();
        }

        private void RadWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {


            (e.NewValue as Models.WndConfirmCloseModel).CloseAction = new Action(this.Close);

        }
    }
}
