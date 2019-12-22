using AlohaFly.DataExtension;
using AlohaFly.Models.ToGoClient;
using AlohaFly.Reports;
using AlohaFly.Utils;
using AlohaService.Interfaces;
using AlohaService.ServiceDataContracts;
using NLog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using static AlohaFly.Models.AddOrderModel;

namespace AlohaFly.Models
{
    public class AddToGoOrderModel
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public bool NewOrder = false;

        public AddToGoOrderModel()
        {
            Order = new OrderToGo();
            NewOrder = true;
            Order = new OrderToGo()
            {

                CreatedBy = Authorization.CurentUser,
                OrderStatus = OrderStatus.InWork,

            };
            OrderDishez = new FullyObservableCollection<DishPackageToGoOrder>();
            EventsInit();
            //Хлеб и приборы в новый заказ
            OrderDishez.Add(new DishPackageToGoOrder()
            {
                Amount = 1,
                Code = 2244,
                Dish = DataCatalogsSingleton.Instance.Dishes.FirstOrDefault(a => a.Barcode == 2244),
                DishName = DataCatalogsSingleton.Instance.Dishes.FirstOrDefault(a => a.Barcode == 2244)?.Name,
                DishId = DataCatalogsSingleton.Instance.Dishes.FirstOrDefault(a => a.Barcode == 2244)?.Id ?? 0,
                PositionInOrder = 1,
                TotalPrice = 0,
                Printed = false


            });
            OrderDishez.Add(new DishPackageToGoOrder()
            {
                Amount = 1,
                Code = 2245,
                Dish = DataCatalogsSingleton.Instance.Dishes.FirstOrDefault(a => a.Barcode == 2245),
                DishName = DataCatalogsSingleton.Instance.Dishes.FirstOrDefault(a => a.Barcode == 2245)?.Name,
                DishId = DataCatalogsSingleton.Instance.Dishes.FirstOrDefault(a => a.Barcode == 2245)?.Id ?? 0,
                PositionInOrder = 2,
                TotalPrice = 0,
                Printed = false
            });

            Order.DeliveryDate = DateTime.Now;
            Order.ExportTime = DateTime.Now;
            Order.CreationDate = DateTime.Now;
            Order.ReadyTime = DateTime.Now;
        }

        private void EventsInit()
        {
            OrderDishez.CollectionChanged += ((sender, e) => { Order.DishPackages = OrderDishez.ToList(); OrderChanged?.Invoke(); }); //OrderDishez_CollectionChanged;
            OrderDishez.ItemPropertyChanged += ((sender, e) => { OrderChanged?.Invoke(); });
            Order.PropertyChanged += ((sender, e) =>
            {
                OrderChanged?.Invoke();
            });
        }

        public event OrderChangedHandler OrderChanged;

        public OrderToGo Order;
        public AddToGoOrderModel(OrderToGo order)
        {

            Order = order;
            OrderDishez = new FullyObservableCollection<DishPackageToGoOrder>(Order.DishPackages);
            EventsInit();


        }


        public FullyObservableCollection<DishPackageToGoOrder> OrderDishez { set; get; }


        public DishPackageToGoOrder AddToOrderDish { set; get; } = new DishPackageToGoOrder();
        public DishPackageToGoOrder RemoveToOrderDish { set; get; } = new DishPackageToGoOrder();
        public string OpenDishName { set; get; }
        public Dish SelectedOpenDish { set; get; } = null;

        public decimal GetDeleveryPrice()
        {
            if (Order?.Address == null)
            { return 0; }
            switch (Order.Address.ZoneId)
            {
                case 1:
                    if (Order.DeliveryDate.Hour < 6) { return 300; }
                    break;
                case 2: if (Order.DeliveryDate.Hour < 6) return 500; else return 300;

                case 3: if (Order.DeliveryDate.Hour < 6) return 1500; else return 1300;

                default:
                    break;

            }
            return 0;
        }


        public void AddDishToOrder()
        {
            try
            {
                if ((AddToOrderDish == null) || (AddToOrderDish.Dish == null) || (AddToOrderDish.Dish.Barcode == 0)) return;
                bool res = true;
                if (DataCatalogsSingleton.Instance.OpenDishezToGoBarCodes.Contains(AddToOrderDish.Dish.Barcode))
                {
                    if (SelectedOpenDish == null)
                    {
                        var od = new Dish()
                        {
                            Name = OpenDishName,
                            LabelRussianName = OpenDishName,
                            //Name = OpenDishName,
                            SHId = AddToOrderDish.Dish.SHId,
                            Barcode = AddToOrderDish.Dish.Barcode,
                            PriceForFlight = AddToOrderDish.TotalPrice,
                            IsAlcohol = AddToOrderDish.Dish.IsAlcohol,
                            IsTemporary = true,
                            IsActive = true,
                            IsToGo = true,

                        };
                        var vm = new CreateTmpDishViewModel(od);
                        var wnd = new UI.WndCreateTmpDish() { DataContext = vm };
                        wnd.Owner = MainClass.MainAppwindow;
                        wnd.ShowDialog();

                        if (vm.Result)
                        {
                            res = DataCatalogsSingleton.Instance.AddOpenDish(od);
                            AddToOrderDish.Dish = od;
                            AddToOrderDish.TotalPrice = AddToOrderDish.Dish.PriceForFlight;
                        }
                        else
                        {
                            res = false;
                        }
                    }
                    else
                    {
                        AddToOrderDish.Dish = SelectedOpenDish;
                    }
                }
                if (res)
                {
                    AddToOrderDish.DishId = AddToOrderDish.Dish.Id;
                    AddToOrderDish.DishName = AddToOrderDish.Dish.Name;
                    AddToOrderDish.PositionInOrder = OrderDishez.Count + 1;
                    AddToOrderDish.Printed = false;
                    OrderDishez.Add((DishPackageToGoOrder)AddToOrderDish.Clone());
                }
            }
            catch
            { }
        }


        public void RemoveDishFromOrder()
        {
            if (RemoveToOrderDish != null)
            {
                if (Order.OrderStatus == OrderStatus.InWork)
                {
                    bool printDeleted = true;
                    var resPrinted = new List<string>();
                    if (RemoveToOrderDish.Printed)
                    {
                        printDeleted = PrintRecieps.PrintOnWinPrinter.PrintOrderToGoToKitchen(Order, out resPrinted, new List<IDishPackageLabel>() { RemoveToOrderDish });
                    }

                    if (printDeleted)
                    {
                        foreach (var ord in OrderDishez.Where(a => a.PositionInOrder > RemoveToOrderDish.PositionInOrder))
                        {
                            ord.PositionInOrder--;
                        }
                        OrderDishez.Remove(RemoveToOrderDish);
                    }
                    else
                    {
                        UI.UIModify.ShowAlert("Ошибка при печати на кухню!" + Environment.NewLine + "Блюдо удалено не будет" + Environment.NewLine + string.Join(Environment.NewLine, resPrinted));
                    }

                }
                else
                {
                    if (WndDeleteDishModel.ShowWndDeleteDish(RemoveToOrderDish))
                    {

                        bool printDeleted = true;
                        var resPrinted = new List<string>();
                        {
                            printDeleted = PrintRecieps.PrintOnWinPrinter.PrintOrderToGoToKitchen(Order, out resPrinted, new List<IDishPackageLabel>() { RemoveToOrderDish });
                        }

                    }
                }
            }
        }

        public void DishInOrderUp()
        {
            if (RemoveToOrderDish.PositionInOrder < OrderDishez.Count)
            {
                OrderDishez.Move((int)RemoveToOrderDish.PositionInOrder - 1, (int)RemoveToOrderDish.PositionInOrder);
                OrderDishez[(int)RemoveToOrderDish.PositionInOrder - 1].PositionInOrder--;
                RemoveToOrderDish.PositionInOrder++;

            }
        }

        public void DishInOrderDown()
        {
            if (RemoveToOrderDish.PositionInOrder > 1)
            {
                OrderDishez.Move((int)RemoveToOrderDish.PositionInOrder - 1, (int)RemoveToOrderDish.PositionInOrder - 2);
                OrderDishez[(int)RemoveToOrderDish.PositionInOrder - 1].PositionInOrder++;
                RemoveToOrderDish.PositionInOrder--;
            }

        }


        public bool SaveOrder()
        {
            logger.Debug($"SaveOrder {Order.Id}");

            bool NewOrder = (Order.Id == 0);
            try
            {
                if (Order.OrderCustomer == null)
                {
                    logger.Debug($"Order.OrderCustomer== null {Order?.Id}");
                    UI.UIModify.ShowAlert("Нельзя сохранить заказ без указания клиента");
                    logger.Debug($"Order.OrderCustomer== null end {Order.Id}");
                    return false;
                }

                List<long> oldItms = OrderDishez.Where(a => a.Printed).Select(a => a.Id).ToList();
                Order.DishPackages = OrderDishez.ToList();
                Order.IsSHSent = true;

                var sRes = DataCatalogsSingleton.Instance.OrdersToGoData.EndEdit(Order);
                if (!sRes.Succeess)
                {
                    UI.UIModify.ShowAlert($"Ошибка при сохранении заказа {sRes.ErrorMessage}");
                    return false;
                }

                try
                {
                    foreach (var dp in sRes.UpdatedItem.DishPackages)
                    {
                        dp.Printed = oldItms.Contains(dp.Id);
                    }

                    if (sRes.UpdatedItem.DishPackages != null && sRes.UpdatedItem.DishPackages.Any(a => !a.Printed))
                    {
                        //Печатаем
                        List<string> outErr = new List<string>();
                        if (!PrintRecieps.PrintOnWinPrinter.PrintOrderToGoToKitchen(sRes.UpdatedItem, out outErr))
                        {
                            foreach (string s in outErr)
                            {
                                UI.UIModify.ShowAlert(s);
                            }
                        }
                        else
                        {
                            foreach (var d in sRes.UpdatedItem.DishPackages)
                            {
                                d.Printed = true;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Error save print " + e.Message);
                }

                if (Order.OrderStatus != OrderStatus.InWork)
                {

                    var CreateSHres = SH.SHWrapper.CreateSalesInvoiceSync(Order, out string err);
                    if (!CreateSHres)
                    {
                        UI.UIModify.ShowAlert($"{err + Environment.NewLine} Накладная будет создана при появлении связи со StoreHouse");
                        Models.ToGoOrdersModelSingleton.Instance.UpdateOrder(Order);
                    }
                }


                if (NewOrder)
                {
                    MainClass.AfterSaveNewToGoOrder();
                }

                return true;
            }



            catch (Exception e)
            {
                logger.Error($"SaveOrder Error {Order.Id} Mess: {e.Message}");
                return false;
            }
            finally
            {

            }



        }

        public decimal OrderSumm
        {
            get
            {
                return OrderDishez.Where(a => !a.Deleted).Sum(a => a.TotalSumm);
            }
        }

        public decimal OrderDiscount
        {
            get
            {
                return Math.Round(OrderDishez.Where(a => !a.Deleted).Sum(a => a.TotalSumm) * Order.DiscountPercent / 100, 2);
            }
        }

        public decimal OrderTotalSumm
        {
            get
            {
                return OrderDishez.Where(a => !a.Deleted).Sum(a => a.TotalSumm) - OrderDiscount + Order.DeliveryPrice;
            }
        }



        public FullyObservableCollection<Dish> DishCollection
        {
            get
            {
                return DataExtension.DataCatalogsSingleton.Instance.ActiveDishesToGo;
            }
        }

    }

    class AddToGoOrderViewModel : ViewModelPaneReactiveObject
    {
        public AddToGoOrderViewModel() : base()
        {

        }
        AddToGoOrderModel model;
        Logger logger = LogManager.GetCurrentClassLogger();


        public new ICommand SaveOrderCommand => base.SaveOrderCommand;

        public AddToGoOrderViewModel(AddToGoOrderModel _model) : base()
        {

            FindDataContext = new ToGoClient.ToGoClientFinderViewModel();
            FindDataContext.ItemSelected += FindDataContext_ItemSelected;
            CtrlClientToGoEditDetailsVisibility = Visibility.Hidden;

            model = _model;

            bool needUpd = false;
            this.WhenAnyValue(a => a.OrderAddress, b => b.OrderAddress.ZoneId)
                .Subscribe(_ =>
                {
                    if (needUpd)
                    {
                        if ((DeleveryPrice == 0) || (UI.UIModify.ShowConfirm($"Меняем сумму доставки на { model.GetDeleveryPrice().ToString()}?").DialogResult.GetValueOrDefault()))
                        {
                            DeleveryPrice = model.GetDeleveryPrice();
                        }
                    };
                }

                );

            this.WhenAnyValue(a => a.Client, b => b.Client.DiscountPercent)
                .Subscribe(_ =>
                {
                    if (needUpd)
                        if (needUpd)
                        {
                            if ((DeleveryPrice == 0) || (UI.UIModify.ShowConfirm($"Меняем скидку на { Client.DiscountPercent.ToString()}% ?").DialogResult.GetValueOrDefault()))
                            {
                                DiscountPercent = Client.DiscountPercent;
                            }
                        };

                });


            this.WhenAnyValue(a => a.Client, b => b.Client.Comments)
                .Subscribe(_ =>
                {
                    ClientCommentVis = String.IsNullOrWhiteSpace(Client?.Comments) ? Visibility.Collapsed : Visibility.Visible;
                }
                );

            needUpd = true;
            ClientPanelVis = model.Order.OrderCustomer == null ? Visibility.Collapsed : Visibility.Visible;
            Changed = model.Order.Id == 0;
            model.OrderChanged += Model_OrderChanged;
            SetFocusedCommands();

            DataExtension.DataCatalogsSingleton.Instance.PropertyChanged += DataCatalog_PropertyChanged;

            AddClientCommand = new DelegateCommand(_ =>
            {
                ClientToGoDetailsEditContext = new ToGoClientEditViewModel(null, true);
                ClientToGoDetailsEditContext.Exited += ClientToGoDetailsEditContext_Exited;
                CtrlClientToGoEditDetailsVisibility = Visibility.Visible;
                ClientStackPanelVis = Visibility.Collapsed;
                GbOrderContendColumn = 1;
            });

            EditClientCommand = new DelegateCommand(_ =>
            {
                ClientToGoDetailsEditContext = new ToGoClientEditViewModel(Client);
                ClientToGoDetailsEditContext.Exited += ClientToGoDetailsEditContext_Exited;

                CtrlClientToGoEditDetailsVisibility = Visibility.Visible;
                ClientStackPanelVis = Visibility.Collapsed;
                GbOrderContendColumn = 1;
            });

            ExitCommand = new DelegateCommand(_ =>
            {
                CloseAction();
            });


            SaveChanesQuestion = $"Вы закрываете окно с измененным заказом №{model.Order.Id} {Environment.NewLine} Сохранить изменения?";
            SaveChanesFunction = (() =>
            {
                logger.Debug($"SaveChanesFunction Order {model.Order.Id}");
                var r = model.SaveOrder();
                Changed = !r;
                logger.Debug($"SaveChanesFunction Order {model.Order.Id} result {r}");
                return r;


            });


            AddDishToOrderCommand = new DelegateCommand(_ =>
            {
                try
                {
                    NewItemNameSetFocusCommand.Execute(null);
                    if (SelectedDish == null || SelectedDish.Barcode != 0)
                    {
                        model.AddDishToOrder();
                        //SelectedDish = new Dish();
                        SelectedDish = null;
                        SelectedOpenDish = null;
                        SelectedOpenDishName = "";
                        OpenDishVisibility = Visibility.Collapsed;
                        SelectedDish = new Dish();
                        AddToOrderDish.Amount = 1;
                        AddToOrderDish.TotalPrice = 0;
                        AddToOrderDish.Comment = "";
                    }
                    this.RaisePropertyChanged("OrderTotalStr");
                }
                catch
                { }
                //NewDishSelectedValue = "";
            });

            RemoveDishFromOrderCommand = new DelegateCommand(_ =>
            {
                model.RemoveDishFromOrder();
                this.RaisePropertyChanged("OrderTotalStr");
                this.RaisePropertyChanged("DeleteDishBtnName");
                this.RaisePropertyChanged("DishSpisPaymentColumnVis");
            });
            DishInOrderUpCommand = new DelegateCommand(_ => { model.DishInOrderUp(); });
            DishInOrderDownCommand = new DelegateCommand(_ => { model.DishInOrderDown(); });

            //ChangeStatusCommand = new DelegateCommand(_ => { UI.UIModify.ShowWndChangeOrderStatus(model.Order); });
            ExitCommand = new DelegateCommand(_ =>
            {
                CloseAction();
                //SaveChanesAsk();
            });

            PrintLabelCommand = new DelegateCommand(_ =>
            {
                try
                {
                    logger.Debug("PrintLabelCommand");
                    model.Order.DishPackages = model.OrderDishez.ToList();
                    UI.UIModify.ShowWndPrintLabels(model.Order);
                }
                catch (Exception e)
                {
                    logger.Debug($"Error PrintLabelCommand {model?.Order?.Id} " + e.Message);
                    UI.UIModify.ShowAlert("Ошибка при печати наклеек " + Environment.NewLine + e.Message);
                }
            });


            PrintForKitchenCommand = new DelegateCommand(_ =>
            {

                try
                {
                    logger.Debug($"ToGo PrintForKitchenCommand Order: {model?.Order?.Id} полная печать");
                    model.Order.DishPackages = model.OrderDishez.ToList();
                    List<string> outErr = new List<string>();

                    foreach (var s in model.Order.DishPackages)
                    {
                        s.Printed = false;
                    }

                    if (!PrintRecieps.PrintOnWinPrinter.PrintOrderToGoToKitchen(model.Order, out outErr))
                    {
                        foreach (string s in outErr)
                        {
                            UI.UIModify.ShowAlert(s);
                        }
                    }
                    foreach (var s in model.Order.DishPackages)
                    {
                        s.Printed = true;
                    }
                    logger.Debug($"ToGo PrintForKitchenCommand Order: {model?.Order?.Id} полная печать успешно");
                }
                catch (Exception e)
                {
                    logger.Error("Error PrintForKitchenCommand " + e.Message);
                }

            });


            PrintInvoiceItems = new DelegateCommand(_ =>
            {
                try
                {
                    logger.Debug($"PrintInvoiceItems {model?.Order?.Id}");
                    model.Order.DishPackages = model.OrderDishez.ToList();

                    UI.UIModify.ShowWndPrintExcelDoc(
                                    $"Накладная ToGo на к заказу №{ model.Order.Id}", AlohaService.ExcelExport.ExportHelper.ExportToGoToExcelWorkbookRussian(model.Order)
                                 );
                }
                catch (Exception e)
                {
                    logger.Debug($"Error PrintInvoiceItems {model?.Order?.Id} " + e.Message);
                    UI.UIModify.ShowAlert("Ошибка при печати накладной " + Environment.NewLine + e.Message);
                }
            });



            PrintToWordCommand = new DelegateCommand(_ =>
            {
                model.Order.DishPackages = model.OrderDishez.ToList();
                //UI.UIModify.ShowWndPrintForKitchen(model.Order);


                if (model.Order == null) { UI.UIModify.ShowAlert("Нет выделенного заказа для печати"); return; }
                if ((model.Order.DishPackagesNoSpis == null) || (model.Order.DishPackagesNoSpis.Count() == 0)) { UI.UIModify.ShowAlert("Нет блюд в заказе для печати"); return; }

            (new WordReports()).PrintKitchenDocumentToGo(model.Order);

                //List<string> outErr = new List<string>();
                // PrintRecieps.PrintOnWinPrinter.PrintOrderToGoToKitchen(model.Order, out outErr);

            });

            ChangeStatusCommand = new DelegateCommand(_ =>
            {
                try
                {
                    logger.Debug($"ChangeStatusCommand togo {model.Order.Id}");
                    // model.Order.DishPackages = model.OrderDishez.ToList();
                    if (model != null)
                    {
                        model.Order.OrderStatus = OrderStatus.Sent;
                        if ((model.Order != null) && (model.OrderDishez != null))
                        {
                            model.Order.DishPackages = model.OrderDishez.ToList();
                        }
                    }

                    this.RaisePropertyChanged("OrderStatusStr");
                }
                catch (Exception e)
                {
                    logger.Error($"ChangeStatusCommand togo {model?.Order?.Id} " + e.Message);
                }
            });

        }

        private void ClientToGoDetailsEditContext_Exited(object sender, ToGoClientEditViewModel.ExitedEventArgs e)
        {
            CtrlClientToGoEditDetailsVisibility = Visibility.Collapsed;
            ClientStackPanelVis = Visibility.Visible;
            GbOrderContendColumn = 0;

            if (e.Saved)
            {
                Client = e.Client;

            }
        }

        public OrderCustomerInfo ClientInfo
        {
            get
            {
                return Client?.OrderCustomerInfo;

            }
        }
        [Reactive] public Visibility CtrlClientToGoEditDetailsVisibility { set; get; } = Visibility.Collapsed;
        [Reactive] public Visibility ClientStackPanelVis { set; get; } = Visibility.Visible;
        [Reactive] public Visibility ClientPanelVis { set; get; } = Visibility.Collapsed;

        [Reactive] public Visibility ClientCommentVis { set; get; } = Visibility.Collapsed;
        [Reactive] public int GbOrderContendColumn { set; get; } = 0;

        //[Reactive] public Visibility CtrlClientToGoDetailsVisibility { set; get; }

        // [Reactive] public ToGoClientViewModel ClientToGoDetailsContext { set; get; }
        [Reactive] public ToGoClientEditViewModel ClientToGoDetailsEditContext { set; get; }
        private void FindDataContext_ItemSelected(object sender, ToGoClient.ItemSelectedEventArgs e)
        {
            // ClientToGoDetailsContext = new ToGoClientViewModel(e.SelectedOrderCustomer);
            Client = e.SelectedOrderCustomer;
            //CtrlClientToGoDetailsVisibility = Visibility.Visible;
        }

        public Models.ToGoClient.ToGoClientFinderViewModel FindDataContext { set; get; }

        public ICommand ShowMapCommand
        {
            get
            {
                return new DelegateCommand(id =>
                {


                    UI.UIModify.ShowWndMap(OrderAddress);
                });
            }
        }

        public OrderCustomer Client
        {
            set
            {
                if (model.Order.OrderCustomer != value)
                {
                    model.Order.OrderCustomer = value;
                    model.Order.OrderCustomerId = value.Id;
                    this.RaisePropertyChanged();
                    //if (model.Order.OrderCustomer!=nu)

                    if (OrderPhone != Client.GetPrimaryPhone()?.Phone)
                    {
                        if ((String.IsNullOrWhiteSpace(OrderPhone)) ||
                            (UI.UIModify.ShowConfirm($"Меняем телефон в заказе на {Client.GetPrimaryPhone()?.Phone}?").DialogResult.GetValueOrDefault()))
                        {
                            OrderPhone = Client.GetPrimaryPhone()?.Phone;
                        }
                    }
                    if (OrderAddress != Client.GetPrimaryAddress())
                    {
                        if ((OrderAddress == null) ||
                            (UI.UIModify.ShowConfirm("Меняем адрес в заказе?").DialogResult.GetValueOrDefault()))
                        {
                            OrderAddress = Client.GetPrimaryAddress();
                        }
                    }

                    //OrderPhone = model.Order?.OrderCustomer?.GetPrimaryPhone()?.Phone;
                    //OrderAddress = model.Order?.OrderCustomer?.GetPrimaryAddress();
                }
                ClientPanelVis = model.Order.OrderCustomer == null ? Visibility.Collapsed : Visibility.Visible;
            }
            get { return model.Order.OrderCustomer; }
        }


        public bool DishSpisPaymentColumnVis
        {
            get
            {
                return model.Order.DishPackages != null && model.Order.DishPackages.Any(x => x.Deleted);

            }
        }
        private void Model_OrderChanged()
        {

            Changed = true;
            this.RaisePropertyChanged("OrderTotalStr");
            this.RaisePropertyChanged("OrderStatusStr");
            this.RaisePropertyChanged("OrderSenderStr");
            this.RaisePropertyChanged("DeliveryDate");
        }

        public string DeleteDishBtnName
        {
            get
            {
                if (RemoveFromOrderDish == null || !(RemoveFromOrderDish.Deleted))
                {
                    return "Удалить блюдо";
                }
                return "Редактировать удаление";
            }
        }
        public ICommand ExitCommand { get; set; }



        public ICommand SetSendStatusCommand { get; set; }



        public ICommand OpenDishSetFocusCommand { get; set; }
        public ICommand NumCountSetFocusCommand { get; set; }
        public ICommand DishCommentSetFocusCommand { get; set; }
        public ICommand PriceSetFocusCommand { get; set; }
        public ICommand NewItemNameSetFocusCommand { get; set; }

        public ICommand AddDishToOrderCommand { get; set; }
        public ICommand RemoveDishFromOrderCommand { get; set; }
        public ICommand DishInOrderUpCommand { get; set; }
        public ICommand DishInOrderDownCommand { get; set; }
        public ICommand ChangeStatusCommand { get; set; }

        public ICommand PrintLabelCommand { get; set; }
        public ICommand PrintForKitchenCommand { get; set; }
        public ICommand PrintToWordCommand { get; set; }


        public ICommand PrintInvoiceItems { get; set; }


        public ICommand AddClientCommand { get; set; }
        public ICommand EditClientCommand { get; set; }

        private void DataCatalog_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DishCollectionSource.Refresh();
            this.RaisePropertyChanged("DishCollectionSource");
        }


        ICollectionView _dishCollectionSource;
        public ICollectionView DishCollectionSource
        {
            get
            {
                if (_dishCollectionSource == null)
                {
                    QueryableCollectionView collectionViewSource = new QueryableCollectionView(model.DishCollection);

                    _dishCollectionSource = collectionViewSource;
                    _dishCollectionSource.SortDescriptions.Add(
                        new SortDescription("Name", ListSortDirection.Ascending));

                    _dishCollectionSource.MoveCurrentToFirst();
                }

                return _dishCollectionSource;
            }
        }


        public decimal DeleveryPrice
        {
            get
            {
                return model.Order.DeliveryPrice;
            }
            set
            {
                model.Order.DeliveryPrice = value;
                this.RaisePropertyChanged("DeleveryPrice");
                this.RaisePropertyChanged("OrderTotalStr");
            }
        }

        public decimal DiscountPercent
        {
            get
            {
                return model.Order.DiscountPercent;
            }
            set
            {
                model.Order.DiscountPercent = value;
                this.RaisePropertyChanged("DiscountPercent");
                this.RaisePropertyChanged("OrderTotalStr");
            }
        }



        public Action<string> SetFocus;
        void SetFocusedCommands()
        {
            NumCountSetFocusCommand = new DelegateCommand(_ => { SetFocus("numCount"); });
            DishCommentSetFocusCommand = new DelegateCommand(_ => { SetFocus("radRichTextBoxAddDisshComment"); });
            PriceSetFocusCommand = new DelegateCommand(_ => { SetFocus("upDownPrice"); });
            NewItemNameSetFocusCommand = new DelegateCommand(_ => { SetFocus("abNewItemName"); });
            OpenDishSetFocusCommand = new DelegateCommand(_ => { SetFocus("ComboOpenDishez"); });
        }


        public string OrderPhone
        {
            set
            {
                if (model.Order.PhoneNumber != value)
                {

                    model.Order.PhoneNumber = value;
                    this.RaisePropertyChanged();
                }

            }
            get
            {
                return model.Order.PhoneNumber;
            }
        }

        public OrderCustomerAddress OrderAddress
        {
            set
            {
                if (model.Order.Address != value)
                {
                    model.Order.Address = value;
                    DeleveryPrice = model.GetDeleveryPrice();
                    this.RaisePropertyChanged();
                }
            }
            get
            {
                return model.Order.Address;
            }
        }


        public string SelectedOpenDishName
        {
            get
            {
                return model.OpenDishName;
            }
            set
            {
                if (model.OpenDishName != value)
                {
                    model.OpenDishName = value;
                    this.RaisePropertyChanged("SelectedOpenDishName");
                }
            }
        }



        //public Dish selectedOpenDish;
        public Dish SelectedOpenDish
        {
            get
            {
                return model.SelectedOpenDish;
            }
            set
            {
                if (model.SelectedOpenDish != value)
                {
                    model.SelectedOpenDish = value;
                    this.RaisePropertyChanged("SelectedOpenDish");
                }
            }
        }

        ICollectionView _openDishez;
        public ICollectionView OpenDishez
        {
            get
            {
                if (model != null && SelectedDish != null)
                {
                    QueryableCollectionView collectionViewSource = new QueryableCollectionView(DataExtension.DataCatalogsSingleton.Instance.GetOpenDishes(SelectedDish.Barcode));
                    _openDishez = collectionViewSource;
                    _openDishez.MoveCurrentToFirst();
                }
                else
                {
                    _openDishez = new QueryableCollectionView(new List<Dish>());
                }
                return _openDishez;
            }
        }



        public CatalogComboBoxViewModel<MarketingChannel> marketingChannelVM;
        public CatalogComboBoxViewModel<MarketingChannel> MarketingChannelVM
        {
            get
            {
                if (marketingChannelVM == null)
                {
                    marketingChannelVM = new CatalogComboBoxViewModel<MarketingChannel>()
                    {
                        DataCatalog = DataCatalogsSingleton.Instance.MarketingChannels,
                        Header = "Канал продаж",
                        EmptyText = "Укажите канал продаж..",
                        DisplayMemberPathName = "Name"
                    };
                    //  try { marketingChannelVM.SelectedData = model.MarketingChannelVM.Single(a => a.Id == SelectedDeliveryPerson.Id); } catch { }
                    marketingChannelVM.SelectedData = model.Order.MarketingChannel;
                    marketingChannelVM.SelectedDataChanged += new EventHandler<MarketingChannel>((sender, e) =>
                    {
                        model.Order.MarketingChannel = e;
                    });
                }
                return marketingChannelVM;
            }
        }

        public DateTime DeliveryDate
        {
            get
            {
                return model.Order.DeliveryDate;
            }
            set
            {
                model.Order.DeliveryDate = value;
                this.RaisePropertyChanged("DeliveryDate");
                //  ExportTime = value.AddHours(-2);


            }
        }

        public DateTime ExportTime
        {
            get
            {
                return model.Order.ExportTime;
            }
            set
            {
                model.Order.ExportTime = value;
                this.RaisePropertyChanged("ExportTime");
                //ReadyTime = value.AddHours(-1);
            }
        }
        public DateTime ReadyTime
        {
            get
            {
                return model.Order.ReadyTime;
            }
            set
            {
                model.Order.ReadyTime = value;
                this.RaisePropertyChanged("ReadyTime");
            }
        }
        public Driver SelectedDeliveryPerson
        {
            get
            {
                return model.Order.Driver;
            }
            set
            {
                model.Order.Driver = value;
            }
        }

        public CatalogComboBoxViewModel<Driver> deliveryPerconVM;
        public CatalogComboBoxViewModel<Driver> DeliveryPerconVM
        {
            get
            {
                if (deliveryPerconVM == null)
                {
                    deliveryPerconVM = new CatalogComboBoxViewModel<Driver>()
                    {
                        DataCatalog = DataExtension.DataCatalogsSingleton.Instance.Drivers,
                        Header = "Кто отвез",
                        EmptyText = "Укажите кто отвез..",
                        DisplayMemberPathName = "FullName"
                    };
                    //try { deliveryPerconVM.SelectedData = model.DeliveryPerson.Single(a => a.Id == SelectedDeliveryPerson.Id); } catch { }
                    deliveryPerconVM.SelectedData = model.Order.Driver;
                    deliveryPerconVM.SelectedDataChanged += new EventHandler<Driver>((sender, e) =>
                    {
                        SelectedDeliveryPerson = e;
                    });
                }
                return deliveryPerconVM;
            }

        }
        public string OrderTotalStr
        {
            get
            {

                string s = $"Сумма заказа: {model?.OrderSumm.ToString("c", CultureInfo.GetCultureInfo("ru-Ru"))} {Environment.NewLine}";
                //  s += $"Сумма надбавки: {(model?.OrderSumm * model?.Order.ExtraCharge / 100).GetValueOrDefault().ToString("c", CultureInfo.GetCultureInfo("ru-Ru"))} {Environment.NewLine}";
                s += $"Сумма доставки: {DeleveryPrice.ToString("c", CultureInfo.GetCultureInfo("ru-Ru"))} {Environment.NewLine}";
                s += $"Сумма скидки: {model?.OrderDiscount.ToString("c", CultureInfo.GetCultureInfo("ru-Ru"))} {Environment.NewLine}";
                s += $"Итого: {model?.OrderTotalSumm.ToString("c", CultureInfo.GetCultureInfo("ru-Ru")) } ";

                return s;
            }
        }
        public string OrderCreatorStr
        {
            get
            {
                return $"Заказ принял: {model.Order.CreatedBy?.FullName}";
            }
        }



        ICollectionView _orderDishez;
        public ICollectionView OrderDishez
        {
            get
            {
                if (_orderDishez == null)
                {
                    QueryableCollectionView collectionViewSource = new QueryableCollectionView(model?.OrderDishez);
                    _orderDishez = collectionViewSource;
                    _orderDishez.MoveCurrentToFirst();
                }

                return _orderDishez;
            }
        }

        /*
        public string OrderSenderStr
        {
            get
            {
                switch (model.Order.OrderStatus)
                {
                    case OrderStatus.InWork:
                        return "";
                    case OrderStatus.Cancelled:
                        return $"Заказ отменил: {model.Order.SendBy?.FullName}";
                    case OrderStatus.Sent:
                        return $"Заказ отправил: {model.Order.SendBy?.FullName}";
                    case OrderStatus.CancelledWithRemains:
                        return $"Заказ отменил: {model.Order.SendBy?.FullName}";
                    case OrderStatus.Closed:
                        return $"Заказ отправил: {model.Order.SendBy?.FullName}";
                    default:
                        break;
                }
                return "";
            }
        }
        */


        public string OrderComment
        {

            set
            {
                if (model.Order != null)
                {
                    model.Order.OrderComment = value;
                }
            }
            get
            {
                return model.Order?.OrderComment;
            }
        }

        public string CommentKitchen
        {

            set
            {
                if (model.Order != null)
                {
                    model.Order.CommentKitchen = value;
                }
            }
            get
            {
                return model.Order?.CommentKitchen;
            }
        }

        public String OrderStatusStr
        {
            get
            {
                return $"Статус заказа: {model.Order.OrderStatus.GetDescription()}";
            }

        }



        Visibility openDishVisibility = Visibility.Collapsed;
        public Visibility OpenDishVisibility
        {
            set
            {
                if (openDishVisibility != value)
                {
                    openDishVisibility = value;
                    this.RaisePropertyChanged("OpenDishVisibility");
                }
            }
            get
            {
                return openDishVisibility;
            }
        }

        Dish _selectedDish;
        public Dish SelectedDish
        {
            get
            {
                return _selectedDish;
            }
            set
            {
                _selectedDish = value;
                if ((value != null) && (value.Barcode != 0))
                {


                    AddToOrderDish.Dish = value;
                    AddToOrderDish.DishName = value.Name;
                    AddToOrderDishAmount = 1;
                    AddToOrderDishPrice = _selectedDish.PriceForDelivery;
                    AddToOrderDishComment = "";


                    if ((DataCatalogsSingleton.Instance.OpenDishezBarCodes.Contains(SelectedDish.Barcode)))
                    {
                        this.RaisePropertyChanged("OpenDishez");

                        OpenDishVisibility = Visibility.Visible;
                        OpenDishSetFocusCommand.Execute(null);
                        //OpenDishFocused = true;
                    }
                    else
                    {
                        NumCountSetFocusCommand.Execute(null);
                        //NumCountFocused = true;
                    }

                }
                else
                {
                    OpenDishVisibility = Visibility.Collapsed;
                }
                this.RaisePropertyChanged("SelectedDish");
            }
        }

        public DishPackageToGoOrder AddToOrderDish
        {
            get
            {
                return model.AddToOrderDish;
            }

        }

        public DishPackageToGoOrder RemoveFromOrderDish
        {
            get
            {
                return model.RemoveToOrderDish;
            }
            set
            {
                model.RemoveToOrderDish = value;
            }
        }


        public long AddToOrderDishBarCode
        {
            get
            {
                return model.AddToOrderDish.Dish.Barcode;
            }
        }




        public decimal AddToOrderDishAmount
        {
            get
            {
                return model.AddToOrderDish.Amount;
            }
            set
            {
                model.AddToOrderDish.Amount = value;
                this.RaisePropertyChanged("AddToOrderDishAmount");
                this.RaisePropertyChanged("AddToOrderDishSumm");
            }

        }

        public decimal AddToOrderDishPrice
        {
            get
            {
                return model.AddToOrderDish.TotalPrice;
            }
            set
            {
                model.AddToOrderDish.TotalPrice = value;
                this.RaisePropertyChanged("AddToOrderDishPrice");
                this.RaisePropertyChanged("AddToOrderDishSumm");
            }
        }

        public decimal AddToOrderDishSumm
        {
            get
            {
                return model.AddToOrderDish.TotalSumm;
            }
        }

        public string AddToOrderDishComment
        {
            get
            {
                return model.AddToOrderDish.Comment;
            }
            set
            {
                model.AddToOrderDish.Comment = value;
                this.RaisePropertyChanged("AddToOrderDishComment");
            }


        }

    }




}
