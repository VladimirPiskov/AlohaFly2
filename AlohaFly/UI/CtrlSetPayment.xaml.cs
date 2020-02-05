using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace AlohaFly.UI
{
    /// <summary>
    /// Логика взаимодействия для CtrlSetPayment.xaml
    /// </summary>
    public partial class CtrlSetPayment : RadWindow
    {
        public CtrlSetPayment()
        {
            InitializeComponent();
            Init();
        }
        public void Init()
        {
            var pps = DataExtension.DataCatalogsSingleton.Instance.PaymentData.Data.Where(a => a.ToGo);
            foreach (var p in pps)
            {
                Button btn = new Button()
                {
                    Width = 200,
                    Height = 40,
                    Margin = new Thickness(15),
                    Content = p.Name,
                    Tag = p.Id
                };
                btn.Click += Btn_Click;
                stMain.Children.Add(btn);
            }

        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            long pId = Convert.ToInt32(((Button)sender).Tag);

            Pid = pId;
            this.Close();

        }
        public long Pid = 0;
    }
}
