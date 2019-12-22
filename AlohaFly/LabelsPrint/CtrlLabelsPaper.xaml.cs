using System.Windows.Controls;

namespace AlohaFly.LabelsPrint
{
    /// <summary>
    /// Логика взаимодействия для CtrlLabelsPaper.xaml
    /// </summary>
    public partial class CtrlLabelsPaper : UserControl
    {
        public CtrlLabelsPaper()
        {
            InitializeComponent();
        }
        int curentitemNum;



        public bool AddLabel(CtrlLabelImage l)
        {
            if (curentitemNum >= 12) return false;
            int col = curentitemNum % 3;
            Grid.SetColumn(l, col);
            Grid.SetRow(l, (curentitemNum - col) / 3 + 1);
            Grid mGr = (((this.Content as Grid).Children[0] as Viewbox).Child as Border).Child as Grid;

            //MainGrid1.Children.Add(l);
            mGr.Children.Add(l);
            curentitemNum++;
            return curentitemNum < 12;
        }
    }

}
