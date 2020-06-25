using System;
using System.Windows;
using Telerik.Windows.Controls;

namespace AlohaFly.UI
{
    
    public partial class WndSetExternalDishLink : RadWindow
    {
        public WndSetExternalDishLink()
        {
            InitializeComponent();
            
        }

        private void RadWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            (e.NewValue as Models.SetExternalDishLinkModel).CloseAction = new Action(this.Close);
        }
    }
}
