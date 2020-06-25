using System;
using System.Windows;
using Telerik.Windows.Controls;

namespace AlohaFly.UI.ToGo
{
    
    public partial class WndMergeCustomers : RadWindow
    {
        public WndMergeCustomers()
        {
            InitializeComponent();
            
        }

        private void RadWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            (e.NewValue as Models.ToGoClient.ToGoMergeCustomers).CloseAction = new Action(this.Close);
        }
    }
}
