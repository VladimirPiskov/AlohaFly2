using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace AlohaFly.LabelsPrint
{
    /// <summary>
    /// Логика взаимодействия для CtrlLabelPapersVisual.xaml
    /// </summary>
    public partial class CtrlLabelPapersVisual : UserControl
    {
        public CtrlLabelPapersVisual()
        {
            InitializeComponent();
            /*
            var template = book1.LeftPageTemplate;
            var btnLeft = (RadButton)template.FindName("BtnLeft", book1);
            btnLeft.IsEnabled = false;
            var Rtemplate = book1.RightPageTemplate;
            var btnRight = (RadButton)template.FindName("BtnRight", book1);
            btnRight.IsEnabled = book1.PagedItems.ItemCount>2;
            */
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ChangeBtnEnabled()
        {
            var template = book1.LeftPageTemplate;
            var btnLeft = (RadButton)template.FindName("BtnLeft", book1);
            btnLeft.IsEnabled = book1.RightPageIndex > 1;
            var Rtemplate = book1.RightPageTemplate;
            var btnRight = (RadButton)template.FindName("BtnRight", book1);
            btnRight.IsEnabled = book1.RightPageIndex < book1.PagedItems.ItemCount;


        }

        private void BtnLeft_Click(object sender, RoutedEventArgs e)
        {


            if (book1.RightPageIndex > 1)
            {
                book1.RightPageIndex -= 2;
            }
            //ChangeBtnEnabled();



        }

        private void BtnRight_Click(object sender, RoutedEventArgs e)
        {
            if (book1.RightPageIndex < book1.PagedItems.ItemCount + 1)
            {
                book1.RightPageIndex += 2;
            }
            //ChangeBtnEnabled();
        }
    }
}
