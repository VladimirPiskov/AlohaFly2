using AlohaFly.DataExtension;
using AlohaFly.Utils;
using AlohaService.ServiceDataContracts;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;

namespace AlohaFly.Models
{
    class AirOrdersViewModel : ViewModelPane
    {
        public AirOrdersViewModel()
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            EnableCollectionSynchronization();
            AirOrdersModelSingleton.Instance.OnOrderAdded += new AirOrdersModelSingleton.OrderAddedHandler(ord => { AfterAddOrder(); });
            AddNewOrderCommand = new DelegateCommand(_ => { MainClass.ShowUC(UI.UIModify.GetCtrlAddOrder()); });
            EditOrderCommand = new DelegateCommand(_ =>
            {
                //thread


                if (CurentOrder != null)
                {
                    if (!CanEditCurentOrder) { return; }

                    MainClass.StartBusy();
                    MainClass.ShowUC(UI.UIModify.GetCtrlAddOrder(CurentOrder));
                    MainClass.StopBusy();
                }
            });
            CopyOrderCommand = new DelegateCommand(_ =>
            {
                if (CurentOrder != null)
                {
                    MainClass.StartBusy();
                    var CopyOrder = (OrderFlight)CurentOrder.Clone();
                    MainClass.ShowUC(UI.UIModify.GetCtrlAddOrder(CopyOrder));
                    MainClass.StopBusy();
                }
            });

            CloseOrderCommand = new DelegateCommand(_ =>
            {
                if (CurentOrder != null)
                {
                    MainClass.CloseSingleCheck(CurentOrder);
                    RaisePropertyChanged("CanEditCurentOrder");
                    RaisePropertyChanged("CanCloseCurentOrder");
                }
            });
            DeleteOrderCommand = new DelegateCommand(_ =>
            {
                try
                {
                    logger.Debug($"DeleteOrderCommand ToFly");

                    if (CurentOrder != null)
                    {
                        logger.Debug($"DeleteOrderCommand ToFly Id: {CurentOrder.Id}");
                        var delres = UI.UIModify.ShowPromt($"Вы уверены, что хотите удалить заказ №{CurentOrder.Id + Environment.NewLine}. Если да, то введите номер заказа.",
                        "Удалить", "Отмена", "Удаление заказа");

                        if (delres != null && delres.DialogResult.GetValueOrDefault())
                        {
                            if (delres.PromptResult != null && (delres.PromptResult.Trim() == CurentOrder.Id.ToString()))
                            {

                                AirOrdersModelSingleton.Instance.DeleteOrder(CurentOrder);

                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error($"DeleteOrderCommand error : {e.Message}");
                }

            });

            RefreshCommand = new DelegateCommand(_ =>
            {
                if (!MainClass.AddAirOrderPaneOpen())
                {
                    AirOrdersModelSingleton.Instance.RefreshOrdersRange();
                }
                else
                {
                    UI.UIModify.ShowAlert($"Для обновления данных по заказам ToFly {Environment.NewLine} необходимо закрыть все вкладки {Environment.NewLine}с добавление либо изменением заказа ToFly.");
                }
            });

            SetSendStatusCommand = new DelegateCommand(_ =>
            {
                if (CurentOrder != null)
                {
                    logger.Debug($"SetSendStatusCommand {CurentOrder.Id}");
                    CurentOrder.SendById = Authorization.CurentUser.Id;
                    CurentOrder.SendBy = Authorization.CurentUser;
                    if (curentOrder.OrderStatus == OrderStatus.InWork)
                    {
                        CurentOrder.OrderStatus = OrderStatus.Sent;
                        SH.SHWrapper.CreateSalesInvoiceSync(CurentOrder, out string err);

                    }
                    else if (curentOrder.OrderStatus == OrderStatus.Sent)
                    {
                        CurentOrder.OrderStatus = OrderStatus.InWork;

                    }

                    Models.AirOrdersModelSingleton.Instance.UpdateOrder(CurentOrder);

                    logger.Debug($"SetSendStatusCommand end {CurentOrder.Id}");

                    RaisePropertyChanged("SetSendStatusBntName");

                }

            });


            PrintLabelCommand = new DelegateCommand(_ => { UI.UIModify.ShowWndPrintLabels(CurentOrder); });
        }

        public ICommand AddNewOrderCommand { get; set; }
        public ICommand EditOrderCommand { get; set; }
        public ICommand CopyOrderCommand { get; set; }
        public ICommand CloseOrderCommand { get; set; }
        public ICommand DeleteOrderCommand { get; set; }
        public ICommand PrintLabelCommand { get; set; }
        public ICommand RefreshCommand { get; set; }

        public ICommand SetSendStatusCommand { get; set; }

        //public ICommand DouCommand { get; set; }


        public string SetSendStatusBntName
        {
            get
            {
                if (CurentOrder != null)
                {
                    if (CurentOrder.OrderStatus == OrderStatus.InWork)
                    {
                        return "Отправить";
                    }
                    else if (CurentOrder.OrderStatus == OrderStatus.Sent)
                    {
                        return "В работу";
                    }
                }


                return "Смена статуса";

            }
        }

        public bool CanSetSendStatus
        {
            get
            {
                return CurentOrder == null ? false : (CurentOrder.OrderStatus == OrderStatus.InWork || CurentOrder.OrderStatus == OrderStatus.Sent);
            }
        }

        public bool CanEditCurentOrder
        {
            get
            {
                return CurentOrder == null ? false : !(CurentOrder.OrderStatus == OrderStatus.Closed && !Authorization.IsDirector && !Authorization.IsAdmin);
            }
        }
        public bool CanCloseCurentOrder
        {
            get
            {
                return CurentOrder == null ? false : CurentOrder.OrderStatus == OrderStatus.Sent;
            }
        }



        private List<RadMenuItem> _printInvoiceitems;
        public List<RadMenuItem> PrintInvoiceItems
        {
            get
            {

                if (_printInvoiceitems == null)
                {
                    _printInvoiceitems = new List<RadMenuItem>() {
                        new RadMenuItem()
                        {

                            Command = new DelegateCommand((_) =>
                            {
                                   if (CurentOrder!=null)
                                   {
                                        UI.UIModify.ShowWndPrintExcelDoc($"Накладная на русском к заказу №{ CurentOrder?.Id}",AlohaService.ExcelExport.ExportHelper.ExportToExcelWorkbookRussian(CurentOrder));
                                    }
                                    else
                                    {
                                        UI.UIModify.ShowAlert("Выделите заказ для печати");
                                    }

                            }),

                            Header = "На русском языке"
                        },
                        new RadMenuItem()
                        {
                            Command = new DelegateCommand((_) => {

                                if (CurentOrder!=null)
                                {
                                UI.UIModify.ShowWndPrintExcelDoc(
                                 $"Накладная на английском к заказу №{ CurentOrder?.Id}",AlohaService.ExcelExport.ExportHelper.ExportToExcelWorkbookEnglish(CurentOrder));
                                }
                                else
                                {
                                    UI.UIModify.ShowAlert("Выделите заказ для печати");
                                }
                            }),
                            Header = "На английском языке"
                        },
                        new RadMenuItem()
                        {

                            Command = new DelegateCommand((_) =>
                            {
                                   if (CurentOrder!=null)
                                   {
                                        UI.UIModify.ShowWndPrintExcelDoc($"Накладная на русском  со скидкой к заказу №{ CurentOrder?.Id}",AlohaService.ExcelExport.ExportHelper.ExportToExcelWorkbookRussian(CurentOrder,true));
                                    }
                                    else
                                    {
                                        UI.UIModify.ShowAlert("Выделите заказ для печати");
                                    }

                            }),

                            Header = "На русском языке со скидкой"
                        },
                        new RadMenuItem()
                        {
                            Command = new DelegateCommand((_) => {

                                if (CurentOrder!=null)
                                {
                                UI.UIModify.ShowWndPrintExcelDoc(
                                 $"Накладная на английском  со скидкой к заказу №{ CurentOrder?.Id}",AlohaService.ExcelExport.ExportHelper.ExportToExcelWorkbookEnglish(CurentOrder,true));
                                }
                                else
                                {
                                    UI.UIModify.ShowAlert("Выделите заказ для печати");
                                }
                            }),
                            Header = "На английском языке  со скидкой"
                        }
                        };

                }
                return _printInvoiceitems;
            }

        }

        public Models.ChangeOrderRangeViewModel changeOrderRangeViewModel { set; get; } = new ChangeOrderRangeViewModel();

        public OrderFlight curentOrder { set; get; }
        public OrderFlight CurentOrder
        {
            set
            {

                if (curentOrder != value)
                {
                    curentOrder = value;
                    OrderFlightInfoViewModel.SetOrder(curentOrder);
                    RaisePropertyChanged("CurentOrder");
                    RaisePropertyChanged("OrderHeader");
                    RaisePropertyChanged("CanEditCurentOrder");
                    RaisePropertyChanged("CanCloseCurentOrder");
                    RaisePropertyChanged("CanSetSendStatus");
                    RaisePropertyChanged("SetSendStatusBntName");
                }
            }
            get
            {
                return curentOrder;
            }
        }






        public System.Windows.Visibility BtnExcelExportVisibility
        {
            get
            {
                if (Authorization.IsDirector)
                {
                    return System.Windows.Visibility.Visible;
                }
                return System.Windows.Visibility.Collapsed;
            }
        }


        public string OrderHeader
        {
            get
            {
                if (CurentOrder == null)
                {
                    return $"Подробности заказа";
                }
                else
                {
                    return $"Подробности заказа {CurentOrder.Id}";
                }
            }
        }

        OrderInfoViewModel orderFlightInfoViewModel;
        public OrderInfoViewModel OrderFlightInfoViewModel
        {
            set
            {
            }
            get
            {

                if (orderFlightInfoViewModel == null)
                {
                    orderFlightInfoViewModel = new OrderInfoViewModel(CurentOrder);
                }
                return orderFlightInfoViewModel;
            }
        }

        public void EnableCollectionSynchronization()
        {
            BindingOperations.EnableCollectionSynchronization(Orders, AirOrdersModelSingleton.Instance.StocksLock);
        }

        ICollectionView _orders;
        public ICollectionView Orders
        {
            get
            {
                if (_orders == null)
                {
                    QueryableCollectionView collectionViewSource = new QueryableCollectionView(AirOrdersModelSingleton.Instance.Orders);
                    _orders = collectionViewSource;

                    //_orders.SortDescriptions.Add(new SortDescription("DeliveryDate", ListSortDirection.Descending));

                    _orders.MoveCurrentToFirst();
                }

                return _orders;
            }
        }
        public void AfterAddOrder()
        {
            Orders.SortDescriptions.Clear();
            Orders.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Descending));
            MoveCurrentToFirst();
        }

        public void MoveCurrentToFirst()
        {
            MainClass.Dispatcher.Invoke(() =>
            {

                _orders.MoveCurrentToFirst();
                OrdersFocused = true;
            }
            );
        }

        public bool ordersFocused { set; get; }
        public bool OrdersFocused
        {
            set
            {
                {
                    ordersFocused = value;
                    RaisePropertyChanged("OrdersFocused");
                }
            }
            get

            {
                return ordersFocused;
            }
        }


    }


    

    public sealed class AirOrdersModelSingleton
    {
        private AirOrdersModelSingleton()
        {
            /*
            var tmpData = DBProvider.GetOrdersFlightAsync(startDt, endDt);
            tmpData.ContinueWith(tsk => EndDataLoad(tsk));
            */
            StartDt = GetMonth(DateTime.Now);
            EndDt = GetMonth(DateTime.Now).AddMonths(1);


        }


        static AirOrdersModelSingleton instance;
        public static AirOrdersModelSingleton Instance
        {

            get
            {
                if (instance == null)
                {
                    instance = new AirOrdersModelSingleton();
                }
                return instance;
            }
        }


        public void Init()
        {
            GetOrdersOfMonth(DateTime.Now);


        }

        /*
        public List<OrderFlight> GetOrdersOfRange(DateTime StartDt, DateTime EndDt)
        {
            for (var m = )

            month = GetMonth(month);
            List<OrderFlight> res;
            if (!dOrders.TryGetValue(month, out res))
            {
                res = DBProvider.GetOrders(month, month.AddMonths(1));
                dOrders.Add(month, res);
            }
            return res;
        }
        */

        private object dOrdersLock = new object();
        public List<OrderFlight> GetOrdersOfMonth(DateTime month)
        {

            month = GetMonth(month);
            List<OrderFlight> res;
            List<OrderFlight> res2;
            lock (dOrdersLock)
            {
                if (!dOrders.TryGetValue(month, out res))
                {
                    res = DBProvider.GetOrders(month, month.AddMonths(1).AddDays(-1), out List<OrderFlight> SVOOrders);
                    if (!dOrders.TryGetValue(month, out res2))
                    {
                        dOrders.Add(month, res);
                        dSVOOrders.Add(month, SVOOrders);
                        SVOorders.Clear();
                        SVOorders.AddRange(SVOOrders);
                    }
                }
            }
            return res;
        }

        private void TryAddOrderToMDic(OrderFlight order)
        {
            DateTime month = GetMonth(order.DeliveryDate);
            List<OrderFlight> res;
            if (dOrders.TryGetValue(month, out res))
            {
                res.Add(order);

            }
        }


        private void TryDeleteOrderFromMDic(OrderFlight order)
        {
            DateTime month = GetMonth(order.DeliveryDate);
            List<OrderFlight> res;
            if (dOrders.TryGetValue(month, out res))
            {
                res.Remove(order);

            }
        }

        Dictionary<DateTime, List<OrderFlight>> dOrders = new Dictionary<DateTime, List<OrderFlight>>();
        Dictionary<DateTime, List<OrderFlight>> dSVOOrders = new Dictionary<DateTime, List<OrderFlight>>();

        DateTime GetMonth(DateTime dt) { return new DateTime(dt.Year, dt.Month, 1); }

        public object StocksLock = new object();
        public void EnableCollectionSynchronization()
        {
            BindingOperations.EnableCollectionSynchronization(Orders, StocksLock);
        }

        private void UpdateOrders(List<OrderFlight> ordersList)
        {
            //orders.SetEventsFreeze();
            lock (StocksLock)
            {
                orders.Clear();
                if (ordersList == null) return;
                foreach (var r in ordersList)
                {
                    orders.Add(r);
                }
            }
            /*
            airCompanyOrders.Clear();

            foreach (var r in ordersList.Where(a=>a.AirCompanyId!=null).Select(a=>a.AirCompanyId.Value).Distinct())
            {
                airCompanyOrders.Add(new Models.AirCompanyOrders(r));
            }
            */

            // orders.UnSetEventsFreeze();
        }




        public bool ThisMonth
        {
            get
            {
                return (StartDt == GetMonth(DateTime.Now) && (EndDt == GetMonth(DateTime.Now).AddMonths(1)));
            }
        }

        public bool LastMonth
        {
            get
            {
                return (StartDt == GetMonth(DateTime.Now.AddMonths(-1)) && (EndDt == GetMonth(DateTime.Now.AddMonths(-1)).AddMonths(1)));
            }
        }

        public void SetNewOrdersRange(int m = 0)
        {
            DateTime dt = DateTime.Now.AddMonths(m);
            StartDt = GetMonth(dt);
            EndDt = GetMonth(dt).AddMonths(1).AddDays(-1);
            UpdateOrders(GetOrdersOfMonth(dt));
            Calc.CalkDiscounts(orders.ToList());
 
            DataCatalogsSingleton.Instance.ChangeOrderDateRange(StartDt, EndDt.AddDays(1));
                       
        }



        public void RefreshOrdersRange()
        {

            var res = DBProvider.GetOrders(StartDt, EndDt, out List<OrderFlight> sVOOrders);
            if (res != null)
            {
                foreach (DateTime dt in dOrders.Keys)
                {
                    if ((dt >= StartDt) && (dt < EndDt))
                    {
                        List<OrderFlight> flList = new List<OrderFlight>();
                        dOrders.TryGetValue(dt, out flList);
                        if (flList != null)
                        {
                            flList.RemoveAll(a => a.DeliveryDate >= StartDt && a.DeliveryDate < EndDt);
                        }
                        flList.AddRange(res.Where(a => a.DeliveryDate >= dt && a.DeliveryDate < dt.AddMonths(1)));
                        Calc.CalkDiscounts(flList);
                    }
                }
                UpdateOrders(res);
                SVOorders.Clear();
                SVOorders.AddRange(sVOOrders);
            }
        }

        public List<OrderFlight> GetOrderFlightsOfMonth(DateTime date)
        {
            DateTime fDt = new DateTime(date.Year, date.Month, 1);
            DateTime eDt = new DateTime(date.AddMonths(1).Year, date.AddMonths(1).Month, 1);
            List<OrderFlight> flList = new List<OrderFlight>();
            if (!dOrders.TryGetValue(fDt, out flList))
            {
                flList = DBProvider.GetOrders(StartDt, EndDt.AddDays(-1), out List<OrderFlight> sVOOrders);
                dOrders.Add(StartDt, flList);
            }
            return flList;


        }


        public void SetNewOrdersRange(DateTime startDt, DateTime endDt)
        {
            if (startDt == StartDt && endDt == EndDt) return;
            var tmpData = DBProvider.GetOrders(startDt, endDt, out List<OrderFlight> sVOOrders);
            StartDt = startDt;
            EndDt = endDt;
            UpdateOrders(tmpData);
            SVOorders.Clear();
            SVOorders.AddRange(sVOOrders);

            DataCatalogsSingleton.Instance.ChangeOrderDateRange(startDt, endDt.AddDays(1));
        }
        public DateTime StartDt { set; get; }
        public DateTime EndDt { set; get; }


        public bool DeleteOrder(OrderFlight order)
        {
            order.OrderStatus = OrderStatus.Cancelled;
            order.SendBy = Authorization.CurentUser;
            var res = DBProvider.UpdateOrderFlight(order);
            if (res)
            {
                if (!SH.SHWrapper.DeleteSalesInvoice(order))
                {

                    DBProvider.UpdateOrderFlight(order);
                }

                TryDeleteOrderFromMDic(order);
                if (orders.Contains(order)) { orders.Remove(order); }

            }
            return res;
        }

        public bool AddOrder(OrderFlight order, List<DishPackageFlightOrder> OrderDishez)
        {
            order.DishPackages = OrderDishez.ToList();
            var res = DBProvider.AddOrderFlight(order, OrderDishez);
            if (res)
            {
                TryAddOrderToMDic(order);
                if ((StartDt < order.DeliveryDate) && (EndDt > order.DeliveryDate))
                {
                    orders.Add(order);
                    RaiseOnOrderAdded(order);
                }
            }

            return res;
        }

        public delegate void OrderAddedHandler(OrderFlight order);
        public event OrderAddedHandler OnOrderAdded;

        public void RaiseOnOrderAdded(OrderFlight order)
        {
            OnOrderAdded?.Invoke(order);
        }




        public bool UpdateOrder(OrderFlight order)
        {

            var res = DBProvider.UpdateOrderFlight(order);
            return res;
        }


        //FullyObservableCollection<AirCompanyOrders> airCompanyOrders = new FullyObservableCollection<AirCompanyOrders>();
        //public FullyObservableCollection<OrderFlight> Orders
        public ReadOnlyObservableCollection<AirCompanyOrders> AirCompanyOrders
        {
            get
            {

                FullyObservableCollection<AirCompanyOrders> airCompanyOrders = new FullyObservableCollection<AirCompanyOrders>();
                foreach (var r in orders.Where(a => a.AirCompanyId != null).Select(a => a.AirCompanyId.Value).Distinct())
                {
                    airCompanyOrders.Add(new Models.AirCompanyOrders(r));
                }
                return new ReadOnlyObservableCollection<AirCompanyOrders>(airCompanyOrders);
                //return new ReadOnlyObservableCollection<AirCompanyOrders>(airCompanyOrders);
            }
        }


        public FullyObservableCollection<OrderFlight> orders = new FullyObservableCollection<OrderFlight>();
        public List<OrderFlight> SVOorders = new List<OrderFlight>();

        //public FullyObservableCollection<OrderFlight> Orders
        public ReadOnlyObservableCollection<OrderFlight> Orders
        {
            get
            {
                return new ReadOnlyObservableCollection<OrderFlight>(orders);
            }
        }
        public ReadOnlyObservableCollection<OrderFlight> OrdersNonSH
        {
            get
            {
                return new ReadOnlyObservableCollection<OrderFlight>(new FullyObservableCollection<OrderFlight>(orders.Where(a => !a.IsSHSent)));
            }
        }
    }


   




}
