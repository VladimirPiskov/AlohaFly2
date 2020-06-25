using AlohaService.ServiceDataContracts;
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
            
        }
        public void Init(OrderToGo ord)
        {
            int n = 0;
            var pps = DataExtension.DataCatalogsSingleton.Instance.PaymentData.Data.Where(a => a.ToGo);
            foreach (var p in pps)
            {
                if ((p.Id == 38) && (ord.OrderTotalSumm > ord.OrderCustomer?.OrderCustomerInfo?.CashBackSumm))
                {
                    continue;
                }

                Button btn = new Button()
                {
                    Width = 200,
                    Height = 40,
                    Margin = new Thickness(15),
                    Content = p.Name,
                    Tag = p.Id
                };
                btn.Click += Btn_Click;
                if (n < 8)
                {
                    stMain.Children.Add(btn);
                }
                else
                {
                    stMain2.Children.Add(btn);
                }
                n++;
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
