using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AlohaFly.PrintRecieps
{
    /// <summary>
    /// Логика взаимодействия для ctrlCheckVisual.xaml
    /// </summary>
    public partial class ctrlCheckVisual : UserControl
    {
        public ctrlCheckVisual()
        {
            InitializeComponent();
        }

        public void CreateCheck(List<FiscalCheckVisualString> Strs)
        {
            StPMain.Children.Clear();
            foreach (FiscalCheckVisualString s in Strs)
            {
                StPMain.Children.Add(CreateGridForFiskalString(s));
            }

            Height = Strs.Count * 25 + 140;

        }

        private Grid CreateGridForFiskalString(FiscalCheckVisualString Str)
        {
            int StandartFont = 16;
            int BigFont = 27;
            int Font = (Str.bigFont) ? BigFont : StandartFont;
            if (Str.bigFont)
            {
                int d = 0;
            }

            Grid Gr = new Grid()
            {
                Margin = new Thickness(0, 3, 3, 0),

                LayoutTransform = new System.Windows.Media.ScaleTransform(0.74, 1.0)
                //  LayoutTransform = new System.Windows.Media.ScaleTransform(0.5, 1.0)
            };

            Gr.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            Gr.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(45) });

            var TbLeft = new TextBlock()
            {
                //BorderThickness = new Thickness(0),
                FontSize = Font,
                //FontFamily = new FontFamily("Calibri"),
                FontFamily = new FontFamily("Consolas"),
                //FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "../Resources/#Consolas"),
                //  FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "/Resources/#Teminus (TTF)for Windows"),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                FontWeight = System.Windows.FontWeights.DemiBold,
                //FontStretch = FontStretches.UltraCondensed,
                Text = Str.strLeft,
                TextWrapping = TextWrapping.Wrap,
                //Margin=new Thickness(0, 0, 10, 0),

                //Padding = new Thickness(0, -3, -3, 0),
            };
            Grid.SetColumn(TbLeft, 0);

            var TbRight = new TextBlock()
            {
                //BorderThickness = new Thickness(0),
                FontSize = Font,
                FontFamily = new FontFamily("Consolas"),
                //  FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "../Resources/#Consolas"),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                FontWeight = System.Windows.FontWeights.DemiBold,
                //FontStretch = FontStretches.UltraCondensed,
                Text = Str.strRight,
                // Padding = new Thickness(0, -3, -3, 0),
                //Margin = new Thickness(20, 0, 0, 0),
                //MinWidth =40,
            };
            Grid.SetColumn(TbRight, 1);
            Gr.Children.Add(TbLeft);
            Gr.Children.Add(TbRight);
            return Gr;
        }

    }


    public class FiscalCheckVisualString
    {
        public FiscalCheckVisualString(string StringForPrint, bool Middle = true, bool BigFont = false)
        {
            strLeft = StringForPrint;
            strRight = "";
            Middle = true;
            bigFont = BigFont;
        }
        public FiscalCheckVisualString(string Left, string Right, bool BigFont = false)
        {
            strLeft = Left;
            strRight = Right;
            bigFont = BigFont;
            Middle = false;
        }
        /*
        public FiscalCheckVisualString(string Left, string Right,bool BigFont)
        {
            strLeft = Left;
            strRight = Right;
            bigFont = BigFont;
        }
        */
        public string strLeft { set; get; }
        public string strRight { set; get; }
        public bool bigFont { set; get; }
        public bool Middle { set; get; }

    }



}
