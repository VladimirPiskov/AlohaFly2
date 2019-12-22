using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace AlohaFly.UI
{
    /// <summary>
    /// Логика взаимодействия для CtrlAddToGoOrder.xaml
    /// </summary>
    public partial class CtrlAddToGoOrder : UserControl
    {
        public CtrlAddToGoOrder()
        {
            InitializeComponent();
        }

        Models.AddToGoOrderViewModel model = null;
        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            model = e.NewValue as Models.AddToGoOrderViewModel;
            model.SetFocus = new Action<string>((_) => { SetFocus(_); });
        }

        public void SetFocus(string ctrlName)
        {
            var el = this.FindName(ctrlName);
            if (el is UIElement)
            {
                (el as UIElement).Focus();
            }
        }

        private void radRichTextBox1_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as RadRichTextBox).ChangeParagraphLineSpacing(1);
            (sender as RadRichTextBox).Document.ParagraphDefaultSpacingAfter = 0;
            (sender as RadRichTextBox).Document.ParagraphDefaultSpacingBefore = 0;

        }

        private void radRichTextBoxAddDisshComment_PreviewEditorKeyDown(object sender, Telerik.Windows.Documents.PreviewEditorKeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.SuppressDefaultAction = true;
                model.AddDishToOrderCommand.Execute(null);

            }
            else if (e.Key == Key.Tab)
            {
                model.AddDishToOrderCommand.Execute(null);

            }


        }

        private void RadGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                (this.DataContext as Models.ToGoOrdersViewModel).EditOrderCommand.Execute(this);
            }
            catch
            {
            }
        }
    }
}
