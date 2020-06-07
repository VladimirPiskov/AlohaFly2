using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AlohaService.ServiceDataContracts;
using AlohaService.ServiceDataContracts.ExternalContracts;
using NLog;
using StoreHouseConnect;

namespace AlohaFlyAdmin
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static Logger _logger = LogManager.GetCurrentClassLogger();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DishDisableUpdate_Click(object sender, RoutedEventArgs e)
        {
            var d = new DishModif();
            d.DisableDish();
        }

        private void btnAddDisc_Click(object sender, RoutedEventArgs e)
        {
            new DishModif().CreateDiscounts();
        }

        private void btnAddPayment_Click(object sender, RoutedEventArgs e)
        {
            new DishModif().CreatePayments();
        }

        private void btnGetdd_Click(object sender, RoutedEventArgs e)
        {
            sh.PrintDishes();


        }



        DishesFromSH sh;
        private void btnSHConnect_Click(object sender, RoutedEventArgs e)
        {
            sh = new DishesFromSH();
            /*
            try
            {
                _logger.Debug("btnSHConnect_Click");
                var s = Dishes.Connect();
                _logger.Debug("Dishes.Connect(); ok");
                ShTb.Text = s + Environment.NewLine;
            }
            catch (Exception ee)
            {
                _logger.Error(ee.Message);
            }
            */
        }

        private void btnGetGroups_Click(object sender, RoutedEventArgs e)
        {
            var res = sh.GetGoups(Convert.ToInt32(tbGroupeNumber.Text));

            foreach (var r in res)
            {
                ShTb.Text += $"{r.Rid} {r.Name} " + Environment.NewLine;
            }
        }

        private void btnGetpl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var res = sh.GetPlaces();
                foreach (var r in res.ListPlace)
                {
                    string s = $"{r.Rid} {r.Name} " + Environment.NewLine;
                    _logger.Debug(s);
                    ShTb.Text += s;
                }
            }
            catch (Exception ee)
            {
                _logger.Debug("Error GetPlaces " + ee.Message);
            }

        }

        private void btnGetCats_Click(object sender, RoutedEventArgs e)
        {
            var res = sh.GetCats();
            foreach (var r in res.ListExpCtgs)
            {
                string s = $"{r.Rid} {r.Name} ";
                _logger.Debug(s);
                ShTb.Text += s + Environment.NewLine;
            }
        }

        private void btnGetDish_Click(object sender, RoutedEventArgs e)
        {
            (new DishesFromSH()).AddDishToSQL(false);
        }

        private void btnAddDishToCFC_Click(object sender, RoutedEventArgs e)
        {
            DishesToAloha.InsertIntoCFCSQL();
        }

        private void btnAddAirsToCFC_Click(object sender, RoutedEventArgs e)
        {
            DishesToAloha.InsertTendersIntoCFCSQL();
        }

        private void btnCreateAllInvoices_Click(object sender, RoutedEventArgs e)
        {
           // (new DishesFromSH()).CreateInvoices();
        }

        private void btnExportToGo_Click(object sender, RoutedEventArgs e)
        {
            //ToGoExport.ToGoExporter.RunExport();
        }

        private void SyncDishFromSHToGo_Click(object sender, RoutedEventArgs e)
        {
            (new DishesFromSH()).AddDishToSQL(true);
        }

        private void btnCreateNeBase_Click(object sender, RoutedEventArgs e)
        {
            SHNewBaseExport.Export();
        }

        private void btnPrintDishToGo_Click(object sender, RoutedEventArgs e)
        {
            //(new DishesFromSH()).PrintDishFromSH(false);
            (new DishesFromSH()).GetDishesHoz();
        }

        private void btnAddSharAlco_Click(object sender, RoutedEventArgs e)
        {
            SharAlcoAdd.DoIt();
        }

        private void btnAddToSQLSharAlco_Click(object sender, RoutedEventArgs e)
        {
            SharAlcoAdd.AddToCFC();
        }

        private void btnGetLog_Click(object sender, RoutedEventArgs e)
        {
            var l = LogChangeOrder.GetLogsOfOrder(tbLogOrder.Text);

            tbBefore.Text = string.Join(Environment.NewLine + Environment.NewLine, l.OrderBy(a => a.CreationDate).Select(a => $"User: {a.UserId} date: {a.CreationDate} {Environment.NewLine} {a.StateBefore}"));
            tbAfter.Text = string.Join(Environment.NewLine + Environment.NewLine, l.OrderBy(a => a.CreationDate).Select(a => $"User: {a.UserId} date: {a.CreationDate} {Environment.NewLine} {a.StateAfter}"));
        }

        private void btnTestQuery_Click(object sender, RoutedEventArgs e)
        {
            var d1 = DateTime.Now;
            //  var res =   AlohaFly.DBProvider.GetOrders(d1.AddDays(-5), d1, out List<OrderFlight> SVOOrders);


        }

        private void btnPriceUpd_Click(object sender, RoutedEventArgs e)
        {
            Tmp.Prep.Doit();
        }

        private void btnGastroRid_Click(object sender, RoutedEventArgs e)
        {
            GastroRid.Insert();
        }

        private void btnAddsiteorder_Click(object sender, RoutedEventArgs e)
        {
            var order = new ExternalToGoOrder()
            {
                Client = new ExternalClient()
                {
                    Address = "Жопа мира, д.7, кв.77",
                    Emale = "em1@gmail.com",
                    Name = "Василий Пупкин",
                    Phone = "+79265555555"
                },
                Comment = "It's comment",
                DeliveryDate = new DateTime(2020, 04, 20),
                DeliveryPrice = 300,
                ExternalId = 3,
                Summ = 2000,
                Dishes = new List<ExternalDishPackage>()
            };
            order.Dishes.Add(new ExternalDishPackage()
            {
                Comment = "Comment1",
                Count = 1,
                Id = 1447,
                Name = "Салат с лобстером, гребешком и соусом гуакамоле ",
                Price = 2000
            }
                );



          var res1=  AlohaFly.DBProvider.Client.ExternalCreateSiteToGoOrder(order);

           // var res2 = AlohaFly.DBProvider.Client.ExternalCreateDeleveryClubToGoOrder(order);
        }

        private void btnGetPRid_Click(object sender, RoutedEventArgs e)
        {
            AlohaFly.SH.SHWrapper.ConnectSH();
            AlohaFly.SH.SHWrapper.GetPRids();
        }
    }
}
