using AlohaFly.Models;
using AlohaService.ServiceDataContracts;
using CefSharp;
using CefSharp.Wpf;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace AlohaFly
{
    internal static class MainClass
    {
        static Logger _logger = LogManager.GetCurrentClassLogger();





        public const long DopLogikCatId = 7;
        public const long AirGastroFoodId = 67;
        public const long AirAvangardId = 19;
        public static Dispatcher Dispatcher
        {
            get
            {
                return MainAppwindow?.Dispatcher;
            }

        }

        internal static void Init(MainWindow mw)
        {
            _logger.Trace("App Init");
            UIActivate(mw);
            Fonts.FontInstall.Install();
            //DataExtension.DataCatalogsSingleton.Instance.BindCollections();
            DateTime startDt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endDt = DateTime.Now.Date.AddMonths(1);
            //  AirOrdersModelSingleton.Instance.EnableCollectionSynchronization();
            //Models.AirOrdersModelSingleton.Instance.SetNewOrdersRange(startDt, endDt);
            var t = new Task(() =>
            {
                LoadInitData();
            });
            t.Start();
            //Task.WaitAll(t);
            UIInit();
            //UIInit();
            //DataExtension.DataCatalogsSingleton.Instance.FillAllData(); ;
            //UI.UIModify.ShowCtrlAddOrder();
            //DateTime startDt = new DateTime(2009, 10,21);
            //DateTime endDt = new DateTime(2009, 10, 23);
        }


        public static void CloseChecksByAirCompAsk(AirCompany air, List<OrderFlight> orders)
        {
            if (air == null) return;
            if (air.PaymentType == null) { UI.UIModify.ShowAlert($"Для закрытия чеков авиакомпания {air.Name} должна содержать вид оплаты"); return; }
            string promtStr = $"Закрыть все чеки по компании {Environment.NewLine} {air.Name}  {Environment.NewLine} " +
                $"с видом оплаты {air.PaymentType.Name} {Environment.NewLine} " +
                $"на общую сумму {orders.Sum(a => a.OrderTotalSumm).ToString("C")}?";
            UI.UIModify.ShowConfirm(promtStr, (_) =>
            {
                if (_)
                {
                    foreach (var o in orders.Where(a => a.OrderStatus == OrderStatus.Sent))
                    {
                        CloseCheck(o);
                    }
                }


            });
        }


        public static void CloseSingleCheck(OrderToGo ord)
        {

            if (ord == null) return;
            //if (ord.AirCompany.PaymentType == null) { UI.UIModify.ShowAlert($"Для закрытия чеков авиакомпания {ord.AirCompany.Name} должна содержать вид оплаты"); return; }
            if ((ord.OrderStatus != OrderStatus.Sent) && (ord.OrderStatus != OrderStatus.InWork))
            {
                UI.UIModify.ShowAlert($"Чек {ord.Id} имеет статус {ord.OrderStatus}  и не может быть закрыт");
            }
            UI.CtrlSetPayment sp = new UI.CtrlSetPayment();
            sp.ShowDialog();
            long pId = sp.Pid;
            if (pId > 0)
            {
                var p = DataExtension.DataCatalogsSingleton.Instance.Payments.Where(a => a.ToGo).SingleOrDefault(a => a.Id == pId);
                string promtStr = $"Закрыть чек {ord.Id}. {Environment.NewLine} Вид оплаты: {p.Name} {Environment.NewLine} На сумму: {ord.OrderTotalSumm}? ";
                ord.PaymentId = pId;
                ord.PaymentType = p;

                var CreateSHres = SH.SHWrapper.CreateSalesInvoiceSync(ord, out string err);
                if (!CreateSHres)
                {
                    UI.UIModify.ShowAlert($"{err + Environment.NewLine} Накладная будет создана при появлении связи со StoreHouse");
                    Models.ToGoOrdersModelSingleton.Instance.UpdateOrder(ord);
                }

                UI.UIModify.ShowConfirm(promtStr, (_) =>
                {
                    if (_)
                    {
                        CloseCheck(ord);
                    }
                });
            }

        }


        public static void CloseSingleCheck(OrderFlight ord)
        {
            if (ord == null) return;
            if (ord.AirCompany.PaymentType == null) { UI.UIModify.ShowAlert($"Для закрытия чеков авиакомпания {ord.AirCompany.Name} должна содержать вид оплаты"); return; }
            if (ord.OrderStatus != OrderStatus.Sent)
            {
                UI.UIModify.ShowAlert($"Чек {ord.Id} имеет статус {ord.OrderStatus}  и не может быть закрыт");
            }
            string promtStr = $"Закрыть чек {ord.Id}. {Environment.NewLine} Вид оплаты: {ord.AirCompany.PaymentType.Name} {Environment.NewLine} На сумму: {ord.OrderTotalSumm}? ";
            UI.UIModify.ShowConfirm(promtStr, (_) =>
            {
                if (_)
                {
                    CloseCheck(ord);
                }
            });
        }

        private static void CloseCheck(OrderFlight o)
        {
            if (o.OrderStatus != OrderStatus.Sent)
            {

                return;
            }
            o.Closed = true;
            o.OrderStatus = OrderStatus.Closed;
            var p = o.AirCompany.PaymentType;
            o.NeedPrintFR = p.FiskalId > 0;

            o.NeedPrintPrecheck = p.FiskalId == 0;

            DBProvider.UpdateOrderFlight(o);
        }
        private static void CloseCheck(OrderToGo o)
        {
            /*
            if (o.OrderStatus != OrderStatus.Sent)
            {

                return;
            }
            */
            o.Closed = true;
            o.OrderStatus = OrderStatus.Closed;
            var p = o.PaymentType;
            o.NeedPrintFR = p.FRSend > 0;
            //o.NeedPrintPrecheck = p.FiskalId == 0;
            o.IsSHSent = true;
            /*
            string err = "";
            var CreateSHres = SH.SHWrapper.CreateSalesInvoiceSync(o, out err);
            if (!CreateSHres)
            {
                UI.UIModify.ShowAlert($"{err + Environment.NewLine} Накладная будет создана при появлении связи со StoreHouse");
            }
            */
            DBProvider.UpdateOrderToGo(o);

        }


        static bool NeedExit = false;
        public static void SetNeedExit(string Message)
        {
            NeedExit = true;
            DataExtension.RealTimeUpdaterSingleton.Instance.StopQueue();
            UI.UIModify.ShowAlert($"Критическая ошибка программы. {Environment.NewLine + Message + Environment.NewLine} Выход");
            Application.Current.Dispatcher.Invoke(() =>
            {
                Application.Current.Shutdown();
            });
        }

        public static void StartBusy()
        {
            mainUIModel.StartBusy();
        }

        public static void StopBusy()
        {
            mainUIModel.StopBusy();
        }


        static void LoadInitData()
        {
            try
            {

                mainUIModel.StartBusy();

                try
                {
                    //Инициализация браузера
                    if (!Cef.IsInitialized)
                    {
                        Cef.Initialize(new CefSettings());
                    }
                }
                catch { }


                DataExtension.DataCatalogsSingleton.Instance.DataCatalogMessage += new EventHandler<string>((sender, s) =>
                {
                    mainUIModel.SendBusyContent(s);
                });

                DataExtension.RealTimeUpdaterSingleton.Instance.Init();
                DataExtension.DataCatalogsSingleton.Instance.DataCatalogsFill();

                mainUIModel.SendBusyContent("Загружаю заказы");
                //AirOrdersModelSingleton.Instance.Init();
                if (Authorization.IsDirector)
                {
                    AirOrdersModelSingleton.Instance.SetNewOrdersRange(-1);
                }
                AirOrdersModelSingleton.Instance.SetNewOrdersRange();
                /*
                if (Authorization.CurentUser.UserName != "sh.user")
                {
                    
                    ToGoOrdersModelSingleton.Instance.SetNewOrdersRange();
                }
                */
                mainUIModel.StopBusy();
                //if (tmpvm != null) { tmpvm.MoveCurrentToFirst(); }

                Dispatcher.Invoke(() =>
                {
                    if (Authorization.IsDirector)
                    {
                        Analytics.MenuItems.ShowMainAnalytics();
                    }
                });
                DataExtension.RealTimeUpdaterSingleton.Instance.StartQueue();
            }
                      
            catch (Exception ex)
            {
                _logger.Error($"Error LoadInitData {ex.Message}");
                if (ex is System.Reflection.ReflectionTypeLoadException)
                {
                    var typeLoadException = ex as ReflectionTypeLoadException;
                    var loaderExceptions = typeLoadException.LoaderExceptions;
                }
            }
        }


        
        static Models.MainUIModel mainUIModel;
        public static MainWindow MainAppwindow;
        private static void UIActivate(MainWindow mainAppwindow)
        {
            MainAppwindow = mainAppwindow;
            mainUIModel = new Models.MainUIModel();
            var ctrlMain = new UI.CtrlMain();
            ctrlMain.DataContext = new Models.MainUIViewModel(mainUIModel);
            mainAppwindow.SetUserName(Authorization.CurentUser.FullName);
            mainAppwindow.SetMainControl(ctrlMain);
            mainAppwindow.Closing += MainAppwindow_Closing;
        }

        private static void MainAppwindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                //Закрытие браузера
                _logger.Debug("Cef.Shutdown");
                if (Cef.IsInitialized)
                {
                    Cef.Shutdown();
                }
            }
            catch (Exception exc)
                {
                _logger.Error($"Error Cef.Shutdown {exc.Message} ");
            }
        }

        static AirOrdersViewModel tmpvm = null;
        private static void UIInit()
        {

            var uc = UI.UIModify.GetCtrlOrdersFlight();
            tmpvm = uc.DataContext as AirOrdersViewModel;
            ShowUC(uc);

        }

        public static bool AddAirOrderPaneOpen()
        {
            return mainUIModel.AddAirOrderPaneOpen();
        }
        public static bool AddToGoOrderPaneOpen()
        {
            return mainUIModel.AddToGoOrderPaneOpen();
        }

        public static void AfterSaveNewToGoOrder()
        {
            mainUIModel.AfterSaveNewToGoOrder();
        }


        public static void ShowUC(UserControl UC)
        {
            mainUIModel.AddPane(UC);
        }
        public static void HideUC(UserControl UC)
        {
            mainUIModel.RemovePane(UC);
        }
        //internal static User CurentUser { get; set; }

    }

}
