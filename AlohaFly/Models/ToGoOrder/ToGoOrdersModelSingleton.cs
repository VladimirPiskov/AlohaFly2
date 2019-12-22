using AlohaFly.DataExtension;
using AlohaFly.Utils;
using AlohaService.ServiceDataContracts;
using NLog;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;

namespace AlohaFly.Models
{
    class ToGoOrdersViewModel : ViewModelPane
    {
        public ToGoOrdersViewModel()
        {
            Logger _logger = LogManager.GetCurrentClassLogger();

            // ToGoOrdersModelSingleton.Instance.OnOrderAdded += new ToGoOrdersModelSingleton.OrderAddedHandler(ord => { AfterAddOrder(); });
            AddNewOrderCommand = new DelegateCommand(_ => { MainClass.ShowUC(UI.UIModify.GetCtrlAddToGoOrder()); });
            EditOrderCommand = new DelegateCommand(_ =>
            {
                if (CurentOrder != null)
                {
                    try
                    {
                        if (!CanEditCurentOrder) { return; }

                        MainClass.StartBusy();
                        MainClass.ShowUC(UI.UIModify.GetCtrlAddToGoOrder(CurentOrder));
                    }
                    catch { }
                    finally
                    {
                        MainClass.StopBusy();
                    }
                }
            });
            CopyOrderCommand = new DelegateCommand(_ =>
            {
                if (CurentOrder != null)
                {
                    MainClass.StartBusy();
                    var CopyOrder = (OrderToGo)CurentOrder.Clone();
                    MainClass.ShowUC(UI.UIModify.GetCtrlAddToGoOrder(CopyOrder));
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
                    _logger.Debug("DeleteOrderCommand ToGo");
                    if (CurentOrder != null)
                    {
                        _logger.Debug($"DeleteOrderCommand ToGo Id: {CurentOrder.Id}");
                        var delres = UI.UIModify.ShowPromt($"Вы уверены, что хотите удалить заказ №{CurentOrder.Id + Environment.NewLine}. Если да, то введите номер заказа.",
                            "Удалить", "Отмена", "Удаление заказа");

                        if (delres != null && delres.DialogResult.GetValueOrDefault())
                        {
                            if (delres.PromptResult != null && (delres.PromptResult.Trim() == CurentOrder.Id.ToString()))
                            {

                                var resPrinted = new List<string>();
                                var printDeleted = PrintRecieps.PrintOnWinPrinter.PrintOrderToGoToKitchen(CurentOrder, out resPrinted, CurentOrder.DishPackagesForLab);

                                if (printDeleted)
                                {
                                    ToGoOrdersModelSingleton.Instance.DeleteOrder(CurentOrder);
                                }

                                else
                                {
                                    //ToGoOrdersModelSingleton.Instance.DeleteOrder(CurentOrder);
                                    UI.UIModify.ShowAlert("Ошибка при печати на кухню!" + Environment.NewLine + "Заказ удален не будет" + Environment.NewLine + string.Join(Environment.NewLine, resPrinted));
                                }

                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.Error($"DeleteOrderCommand error : {e.Message}");
                }
            });

            PrintLabelCommand = new DelegateCommand(_ =>
            {
                if (CurentOrder != null)
                {
                    UI.UIModify.ShowWndPrintLabels(CurentOrder);
                }
                else
                {
                    UI.UIModify.ShowAlert("Нет заказа для печати");
                }
            });


            RefreshCommand = new DelegateCommand(_ =>
            {
                if (!MainClass.AddToGoOrderPaneOpen())
                {
                    RealTimeUpdaterSingleton.Instance.UpdateData();
                    //ToGoOrdersModelSingleton.Instance.RefreshOrdersRange();
                    //  DataCatalogsSingleton.Instance.OrdersToGoData.FillUpdate();
                }
                else
                {
                    UI.UIModify.ShowAlert($"Для обновления данных по заказам ToGo {Environment.NewLine} необходимо закрыть все вкладки {Environment.NewLine}с добавление либо изменением заказа ToGo.");
                }
            });

            PrintInvoiceItems = new DelegateCommand(_ =>
            {
                if (CurentOrder != null)
                {
                    UI.UIModify.ShowWndPrintExcelDoc($"Накладная ToGo на к заказу №{ CurentOrder.Id}", AlohaService.ExcelExport.ExportHelper.ExportToGoToExcelWorkbookRussian(CurentOrder));
                }
                else
                {
                    UI.UIModify.ShowAlert("Нет заказа для печати");
                }
            }
            );



            ChangeStatusCommand = new DelegateCommand(_ =>
            {
                try
                {


                    if ((CurentOrder != null))
                    {
                        //CurentOrder.SendById = Authorization.CurentUser.Id;
                        //CurentOrder.SendBy = Authorization.CurentUser;
                        // UI.UIModify.ShowAlert("ChangeStatusCommand from togo orders");

                        if (CurentOrder.OrderStatus == OrderStatus.InWork)
                        {
                            CurentOrder.OrderStatus = OrderStatus.Sent;
                            SH.SHWrapper.CreateSalesInvoiceSync(CurentOrder, out string err);

                        }
                        else if (CurentOrder.OrderStatus == OrderStatus.Sent)
                        {
                            CurentOrder.OrderStatus = OrderStatus.InWork;
                        }


                        Models.ToGoOrdersModelSingleton.Instance.UpdateOrder(CurentOrder);
                        RaisePropertyChanged("SetSendStatusBntName");
                    }
                    else
                    {
                        UI.UIModify.ShowAlert("Нет заказа для отправки");
                    }
                }
                catch
                {

                }
            }
           );
        }

        public ICommand AddNewOrderCommand { get; set; }
        public ICommand EditOrderCommand { get; set; }
        public ICommand CopyOrderCommand { get; set; }
        public ICommand CloseOrderCommand { get; set; }
        public ICommand DeleteOrderCommand { get; set; }
        public ICommand PrintLabelCommand { get; set; }
        public ICommand PrintInvoiceItems { get; set; }
        public ICommand ChangeStatusCommand { get; set; }
        public ICommand RefreshCommand { get; set; }


        //public ICommand DouCommand { get; set; }

        public ICommand ShowMapCommand
        {
            get
            {
                return new DelegateCommand(id =>
                {

                    if (CurentOrder == null)
                    {
                        UI.UIModify.ShowAlert("Выберите заказ");
                    }
                    else
                    {
                        UI.UIModify.ShowWndMap(CurentOrder.Address);
                    }
                });
            }
        }

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
                return CurentOrder == null ? false : CurentOrder.OrderStatus != OrderStatus.Closed;
            }
        }
        public bool CanCloseCurentOrder
        {
            get
            {
                return CurentOrder == null ? false : CurentOrder.OrderStatus == OrderStatus.Sent;
            }
        }



        public Models.ChangeOrderRangeViewModel changeOrderRangeViewModel { set; get; } = new ChangeOrderRangeViewModel();

        public OrderToGo curentOrder { set; get; }
        public OrderToGo CurentOrder
        {
            set
            {

                if (curentOrder != value)
                {
                    curentOrder = value;
                    OrderToGoInfoViewModel.SetOrder(curentOrder);
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


        OrderInfoViewModel orderToGoInfoViewModel;
        public OrderInfoViewModel OrderToGoInfoViewModel
        {
            set
            {
            }
            get
            {

                if (orderToGoInfoViewModel == null)
                {
                    orderToGoInfoViewModel = new OrderInfoViewModel(CurentOrder);
                }
                return orderToGoInfoViewModel;
            }
        }



        ICollectionView _orders;
        public ICollectionView Orders
        {
            get
            {
                if (_orders == null)
                {
                    QueryableCollectionView collectionViewSource = new QueryableCollectionView(ToGoOrdersModelSingleton.Instance.Orders);
                    _orders = collectionViewSource;
                    _orders.SortDescriptions.Add(new SortDescription("ForSort", ListSortDirection.Descending));

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

    public sealed class ToGoOrdersModelSingleton
    {
        Logger _logger = LogManager.GetCurrentClassLogger();
        private ToGoOrdersModelSingleton()
        {
            //StartDt = GetMonth(DateTime.Now);
            //EndDt = GetMonth(DateTime.Now).AddMonths(1);
            //UpdateDateRange(startDt, endDt);
            Orders = new FullyObservableCollection<OrderToGo>();
            ordersConnector.OrderByDesc(a => a.DeliveryDate)
                            .Subsribe(DataCatalogsSingleton.Instance.OrdersToGoData, Orders);

            OrdersNonSH = new FullyObservableCollection<OrderToGo>();
            ordersNonSHConnector.Select(a=>!a.IsSHSent && a.OrderStatus!=OrderStatus.InWork)
                .OrderByDesc(a => a.DeliveryDate)
                            .Subsribe(DataCatalogsSingleton.Instance.OrdersToGoData, OrdersNonSH);
        }


        static ToGoOrdersModelSingleton instance;
        public static ToGoOrdersModelSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ToGoOrdersModelSingleton();
                }
                return instance;
            }
        }
     
        public DateTime StartDt { set; get; }
        public void UpdateDateRange(DateTime startDt, DateTime endDt)
        {
            StartDt = startDt;
            EndDt = endDt;
            ordersConnector.Select(a => (Authorization.IsDirector || a.OrderStatus != OrderStatus.Cancelled) && (a.DeliveryDate >= StartDt && a.DeliveryDate < EndDt));
         }

       
        public DateTime EndDt { set; get; }

        FullyObservableDBDataSubsriber<OrderToGo, OrderToGo> ordersConnector = new FullyObservableDBDataSubsriber<OrderToGo, OrderToGo>(a => a.Id);

        [Reactive] public FullyObservableCollection<OrderToGo> Orders { set; get; }

        FullyObservableDBDataSubsriber<OrderToGo, OrderToGo> ordersNonSHConnector = new FullyObservableDBDataSubsriber<OrderToGo, OrderToGo>(a => a.Id);
        [Reactive] public FullyObservableCollection<OrderToGo> OrdersNonSH { set; get; }


         public bool AddOrder(OrderToGo order, List<DishPackageToGoOrder> OrderDishez)
        {

            order.DishPackages = OrderDishez.ToList();
            var res = DataCatalogsSingleton.Instance.OrdersToGoData.EndEdit(order);

            
            return res.Succeess;
        }
        public bool UpdateOrder(OrderToGo order)
        {

            var res = DataCatalogsSingleton.Instance.OrdersToGoData.EndEdit(order);
            return res.Succeess;
        }


        public bool DeleteOrder(OrderToGo order)
        {
            try
            {
                order.OrderStatus = OrderStatus.Cancelled;
                var res = DataCatalogsSingleton.Instance.OrdersToGoData.EndEdit(order);
                return res.Succeess;
            }
            catch(Exception e)
            {
                _logger.Error($"ToGoOrdersModelSingleton.DeleteOrder: {e.Message}");
                return false;
            }
         
        }

        

    }
}
