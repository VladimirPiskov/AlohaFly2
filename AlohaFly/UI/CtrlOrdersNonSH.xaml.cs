using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace AlohaFly.UI
{
    /// <summary>
    /// Логика взаимодействия для CtrlOrdersNonSH.xaml
    /// </summary>
    public partial class CtrlOrdersNonSH : UserControl
    {
        public CtrlOrdersNonSH()
        {
            InitializeComponent();
        }


        private void GridViewDataColumn_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as RadRichTextBox).ChangeParagraphLineSpacing(1);
            (sender as RadRichTextBox).Document.ParagraphDefaultSpacingAfter = 0;
            (sender as RadRichTextBox).Document.ParagraphDefaultSpacingBefore = 0;
        }


    }
}
