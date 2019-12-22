using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace AlohaFly.UI
{
    /// <summary>
    /// Логика взаимодействия для CtrlAddOrder.xaml
    /// </summary>
    public partial class CtrlAddOrder : UserControl
    {
        public CtrlAddOrder()
        {
            InitializeComponent();
            radRichTextBoxOrderComment.ChangeParagraphLineSpacing(1);
            //this.radRichTextBoxAddDisshComment.Document.Style.SpanProperties.FontSize = Unit.PointToDip(20);
            //this.radRichTextBoxOrderComment.Document.Style.SpanProperties.FontSize = Unit.PointToDip(20);

        }

        private void radAutoCompleteBox_SearchTextChanged(object sender, EventArgs e)
        {

        }

        private void radAutoCompleteBox_SearchTextChanged_1(object sender, EventArgs e)
        {


        }

        private void radAutoCompleteBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (((RadAutoCompleteBox)sender).SearchText != null)
            {
                ((RadAutoCompleteBox)sender).SearchText = ((RadAutoCompleteBox)sender).SearchText.ToUpper();

            }
        }

        private void radRichTextBoxAddDisshComment_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                model.AddDishToOrderCommand.Execute(null);
            }
        }

        Models.AddOrderViewModel model = null;
        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            model = e.NewValue as Models.AddOrderViewModel;
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

        private void radRichTextBox1_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as RadRichTextBox).ChangeParagraphLineSpacing(1);
            (sender as RadRichTextBox).Document.ParagraphDefaultSpacingAfter = 0;
            (sender as RadRichTextBox).Document.ParagraphDefaultSpacingBefore = 0;

        }

        private void radRichTextBoxOrderComment_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as RadRichTextBox).ChangeParagraphLineSpacing(1);
            (sender as RadRichTextBox).Document.ParagraphDefaultSpacingAfter = 0;
            (sender as RadRichTextBox).Document.ParagraphDefaultSpacingBefore = 0;
        }
    }
}
