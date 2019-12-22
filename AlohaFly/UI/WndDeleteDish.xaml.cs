using System;
using System.Windows;
using Telerik.Windows.Controls;

namespace AlohaFly.UI
{
    /// <summary>
    /// Логика взаимодействия для WndConfirmClose.xaml
    /// </summary>
    public partial class WndDeleteDish : RadWindow
    {
        public WndDeleteDish()
        {
            InitializeComponent();
        }

        private void RadWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {


            (e.NewValue as Models.WndDeleteDishModel).CloseAction = new Action(this.Close);

        }
    }
}
