
using AlohaFly.DataExtension;
using AlohaFly.Utils;
using AlohaService.Interfaces;
using AlohaService.ServiceDataContracts;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;

namespace AlohaFly.Models
{
    public class AddOrderViewModel : ViewModelPane
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        AddOrderModel Model;
        public AddOrderViewModel() : base()
        {
        }

        public AddOrderViewModel(AddOrderModel model) : base()
        {
            Model = model;
            Changed = model.Order.Id == 0;
            Model.OrderChanged += Model_OrderChanged;
            ExtraChargeEnable = Model.Order.ExtraCharge > 0;


            DataExtension.DataCatalogsSingleton.Instance.PropertyChanged += DataCatalog_PropertyChanged;
            AddDishToOrderCommand = new DelegateCommand(_ =>
            {
                try
                {
                    AbNewItemNameFocused = true;
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
                }
                catch
                { }
                //NewDishSelectedValue = "";
            });
            RemoveDishFromOrderCommand = new DelegateCommand(_ =>
            {
                model.RemoveDishFromOrder();
                RaisePropertyChanged("DeleteDishBtnName");
                RaisePropertyChanged("DishSpisPaymentColumnVis");

            });
            DishInOrderUpCommand = new DelegateCommand(_ => { model.DishInOrderUp(); });
            DishInOrderDownCommand = new DelegateCommand(_ => { model.DishInOrderDown(); });


            ChangeStatusCommand = new DelegateCommand(_ => { UI.UIModify.ShowWndChangeOrderStatus(model.Order); });
            ExitCommand = new DelegateCommand(_ =>
            {
                CloseAction();
                //SaveChanesAsk();
            });
            PrintLabelCommand = new DelegateCommand(_ =>
            {
                Model.Order.DishPackages = Model.OrderDishez.ToList();
                UI.UIModify.ShowWndPrintLabels(Model.Order);
            });
            PrintForKitchenCommand = new DelegateCommand(_ =>
            {
                Model.Order.DishPackages = Model.OrderDishez.ToList();
                UI.UIModify.ShowWndPrintForKitchen(Model.Order);

                //List<string> outErr = new List<string>();
                // PrintRecieps.PrintOnWinPrinter.PrintOrderToGoToKitchen(model.Order, out outErr);

            });

            SelectedDishEnterCommand = new DelegateCommand(_ =>
            {
                /*
                if ((SelectedDish!=null) && (DataCatalogsSingleton.Instance.OpenDishezBarCodes.Contains(SelectedDish.Barcode)))
                {
                    RaisePropertyChanged("OpenDishez");
                    OpenDishVisibility = Visibility.Visible;
                    OpenDishFocused = true;
                }
                else
                {
                    NumCountFocused = true;
                }
                */

            });


            NumCountSetFocusCommand = new DelegateCommand(_ => { NumCountFocused = true; });
            DishCommentSetFocusCommand = new DelegateCommand(_ => { DishCommentFocused = true; });
            PriceSetFocusCommand = new DelegateCommand(_ => { PriceFocused = true; });

            PrintMenuCommand = new DelegateCommand(_ =>
            {
                Model.Order.DishPackages = Model.OrderDishez.ToList();
                new Reports.ExcelReports().ToFlyMenuCreate(Model.Order);
                /*
                UI.UIModify.ShowWndPrintExcelDoc(
                        $"Меню к заказу №{ Model.Order.Id}",
                        AlohaService.ExcelExport.ExportHelper.ExportMenuToExcelWorkbook(Model.Order));
                  */


            });

            SaveChanesQuestion = $"Вы закрываете окно с измененным заказом №{Model.Order.Id} {Environment.NewLine} Сохранить изменения?";
            SaveChanesFunction = (() =>
            {
                logger.Debug($"SaveChanesFunction ToFly Order {model.Order.Id}");
                var r = model.SaveOrder();
                Changed = !r;
                logger.Debug($"SaveChanesFunction ToFly Order {model.Order.Id} result {r}");
                return r;


            });

            SetSendStatusCommand = new DelegateCommand(_ =>
            {
                model.Order.SendById = Authorization.CurentUser.Id;
                model.Order.SendBy = Authorization.CurentUser;
                model.Order.OrderStatus = OrderStatus.Sent;
                model.SaveOrder();

            });

            SetCancelWithRemCommand = new DelegateCommand(_ =>
            {
                model.Order.SendById = Authorization.CurentUser.Id;
                model.Order.SendBy = Authorization.CurentUser;
                model.Order.OrderStatus = OrderStatus.CancelledWithRemains;
            });



            /*
            ReturnChangesAction = () => {
                Model.Order = 
            };
            */


            DishEdit = new DelegateCommand(_ =>
                {
                    model.Order.SendById = Authorization.CurentUser.Id;
                    model.Order.SendBy = Authorization.CurentUser;
                    model.Order.OrderStatus = OrderStatus.CancelledWithRemains;
                });

            PrintForKitchenSlipCommand = new DelegateCommand(_ =>
            {
                logger.Debug("PrintForKitchenCommand");
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

            });

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

        private void Model_OrderChanged()
        {

            Changed = true;
            OnPropertyChanged("OrderTotalStr");
            OnPropertyChanged("OrderStatusStr");
            OnPropertyChanged("OrderSenderStr");
            OnPropertyChanged("DeliveryDate");

        }

        /*
        public void BeforeClosing()
        {
            if (Changed)
            {
                //UI.UIModify.ShowConfirm
            }
            CloseAction();
        }
        */

        public ICommand AddDishToOrderCommand { get; set; }
        public ICommand RemoveDishFromOrderCommand { get; set; }
        public ICommand DishInOrderUpCommand { get; set; }
        public ICommand DishInOrderDownCommand { get; set; }

        public ICommand ChangeStatusCommand { get; set; }
        public ICommand ExitCommand { get; set; }
        public ICommand PrintLabelCommand { get; set; }
        public ICommand PrintForKitchenCommand { get; set; }
        public ICommand PrintForKitchenSlipCommand { get; set; }
        public ICommand PrintMenuCommand { get; set; }
        public ICommand SetSendStatusCommand { get; set; }
        public ICommand SetCancelWithRemCommand { get; set; }

        public ICommand SelectedDishEnterCommand { get; set; }

        public ICommand DishEdit { get; set; }

        public new ICommand SaveOrderCommand => base.SaveOrderCommand;
        //public new Action CloseAction => base.CloseAction;




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
                            Command = new DelegateCommand((_) => {
                                try{
                                    UI.UIModify.ShowWndPrintExcelDoc(
                                        $"Накладная на русском к заказу №{ Model.Order.Id}",AlohaService.ExcelExport.ExportHelper.ExportToExcelWorkbookRussian(Model.Order)
                                    );
                                }
                                catch(Exception e )
                                {
                                    logger.Error($"Error _printInvoiceitems  {e.Message}");
                                }
                                }),
                            Header = "На русском языке"
                        },
                        new RadMenuItem()
                        {
                            Command = new DelegateCommand((_) => {
                                try
                                {
                                    UI.UIModify.ShowWndPrintExcelDoc(
                                    $"Накладная на английском к заказу №{ Model.Order.Id}",AlohaService.ExcelExport.ExportHelper.ExportToExcelWorkbookEnglish(Model.Order)
                                    );
                                }
                                catch(Exception e )
                                {
                                    logger.Error($"Error _printInvoiceitems  {e.Message}");
                                }
                            }),
                            Header = "На английском языке"
                        },
                        new RadMenuItem()
                        {
                            Command = new DelegateCommand((_) => {
                                try{
                                    UI.UIModify.ShowWndPrintExcelDoc(
                                        $"Накладная на русском со скидкой к заказу №{ Model.Order.Id}",AlohaService.ExcelExport.ExportHelper.ExportToExcelWorkbookRussian(Model.Order,true)
                                    );
                                }
                                catch(Exception e )
                                {
                                    logger.Error($"Error _printInvoiceitems  {e.Message}");
                                }
                                }),
                            Header = "На русском языке со скидкой"
                        },
                        new RadMenuItem()
                        {
                            Command = new DelegateCommand((_) => {
                                try
                                {
                                    UI.UIModify.ShowWndPrintExcelDoc(
                                    $"Накладная на английском со скидкой к заказу №{ Model.Order.Id}",AlohaService.ExcelExport.ExportHelper.ExportToExcelWorkbookEnglish(Model.Order,true)
                                    );
                                }
                                catch(Exception e )
                                {
                                    logger.Error($"Error _printInvoiceitems  {e.Message}");
                                }
                            }),
                            Header = "На английском языке со скидкой"
                        }
                        };

                }
                return _printInvoiceitems;
            }
        }

        #region Focuced

        public ICommand NumCountSetFocusCommand { get; set; }
        public ICommand DishCommentSetFocusCommand { get; set; }
        public ICommand PriceSetFocusCommand { get; set; }

        bool priceFocused;
        public bool PriceFocused
        {
            set
            {
                {
                    priceFocused = value;
                    RaisePropertyChanged("PriceFocused");
                }
            }
            get
            {
                return priceFocused;
            }
        }

        bool openDishFocused;
        public bool OpenDishFocused
        {
            set
            {
                {
                    SetFocus("ComboOpenDishez");
                    //openDishFocused = value;
                    //RaisePropertyChanged("OpenDishFocused");
                    //openDishFocused = false;
                }
            }
            get

            {
                return openDishFocused;
            }
        }



        bool dishCommentFocused;
        public bool DishCommentFocused
        {
            set
            {
                {
                    dishCommentFocused = value;
                    RaisePropertyChanged("DishCommentFocused");
                    dishCommentFocused = false;
                }
            }
            get

            {
                return dishCommentFocused;
            }
        }

        bool bortNumberFocused;
        public bool BortNumberFocused
        {
            set
            {
                {
                    bortNumberFocused = value;
                    RaisePropertyChanged("BortNumberFocused");
                }
            }
            get

            {
                return bortNumberFocused;
            }
        }

        bool numCountFocused;
        public bool NumCountFocused
        {
            set
            {
                //if (numCountFocused != value)
                {
                    numCountFocused = value;
                    RaisePropertyChanged("NumCountFocused");
                    numCountFocused = false;
                }
            }
            get

            {
                return numCountFocused;
            }
        }

        public Action<string> SetFocus;
        public bool abNewItemNameFocused { set; get; }
        public bool AbNewItemNameFocused
        {
            set
            {
                {
                    abNewItemNameFocused = value;
                    SetFocus("abNewItemName");
                }
            }
            get

            {
                return abNewItemNameFocused;
            }
        }

        #endregion 

        string newDishSelectedValue;
        public string NewDishSelectedValue
        {
            set
            {
                newDishSelectedValue = value;
                RaisePropertyChanged("NewDishSelectedValue");

            }
            get
            {
                return newDishSelectedValue;
            }
        }




        private void DataCatalog_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DishCollectionSource.Refresh();
            OnPropertyChanged("DishCollectionSource");
        }
        decimal ExtraChargeValueDefualt = 10;

        public decimal ExtraChargeValue
        {
            set
            {
                if (value != Model.Order.ExtraCharge)
                {
                    if (extraChargeEnable)
                    {
                        Model.Order.ExtraCharge = value;
                    }
                    OnPropertyChanged("ExtraChargeValue");
                }
            }
            get
            {
                if ((Model == null) || (Model.Order == null)) return 0;
                return Model.Order.ExtraCharge;
            }
        }

        bool extraChargeEnable = false;
        public bool ExtraChargeEnable
        {
            set
            {
                if (value != extraChargeEnable)
                {

                    if (value)
                    {
                        extraChargeEnable = value;
                        if (Model.Order.ExtraCharge == 0)
                        {
                            ExtraChargeValue = ExtraChargeValueDefualt;
                        }
                    }
                    else
                    {
                        ExtraChargeValue = 0;
                        extraChargeEnable = value;
                    }

                    OnPropertyChanged("ExtraChargeEnable");
                    OnPropertyChanged("ExtraChargeValue");
                }
            }
            get
            {
                return extraChargeEnable;
            }
        }

        public AirCompany SelectedAirCompany
        {
            get
            {
                return Model.Order.AirCompany;
            }
            set
            {
                Model.Order.AirCompany = value;
            }
        }
        public ObservableCollection<string> FlightNumbers
        {
            get
            {
                return new ObservableCollection<string>(Models.AirOrdersModelSingleton.Instance.Orders.Select(a => a.FlightNumber).Distinct());
            }
        }
        public DeliveryPlace SelectedDeliveryPlace
        {
            get
            {
                return Model.Order.DeliveryPlace;
            }
            set
            {
                Model.Order.DeliveryPlace = value;
            }
        }

        public Driver SelectedDeliveryPerson
        {
            get
            {
                return Model.Order.Driver;
            }
            set
            {
                Model.Order.Driver = value;
            }
        }


        public string SelectedContactPersonName
        {
            set
            {
                Model.SelectedContactPersonName = value;
            }
        }

        public string SelectedContactPersonEmail
        {
            set
            {
                Model.SelectedContactPersonEmail = value;
            }
        }
        public string SelectedContactPersonPhone
        {
            set
            {
                Model.SelectedContactPersonPhone = value;
            }
        }


        public ContactPerson SelectedContactPerson
        {
            get
            {
                return Model.Order.ContactPerson;
            }
            set
            {
                Model.Order.ContactPerson = value;
                OnPropertyChanged("SelectedContactPerson");
            }
        }

        public User SelectedManagerOperator
        {
            set; get;
        }

        public bool DishSpisPaymentColumnVis
        {
            get
            {
                return Model.Order.DishPackages != null && Model.Order.DishPackages.Any(x => x.Deleted);

            }
        }

        ICollectionView _orderDishez;
        public ICollectionView OrderDishez
        {
            get
            {
                if (_orderDishez == null)
                {
                    QueryableCollectionView collectionViewSource = new QueryableCollectionView(Model?.OrderDishez);
                    _orderDishez = collectionViewSource;
                    _orderDishez.MoveCurrentToFirst();
                }

                return _orderDishez;
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
                    RaisePropertyChanged("OpenDishVisibility");
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
                    AddToOrderDishPrice = _selectedDish.PriceForFlight;
                    AddToOrderDishComment = "";


                    if ((DataCatalogsSingleton.Instance.OpenDishezBarCodes.Contains(SelectedDish.Barcode)))
                    {
                        RaisePropertyChanged("OpenDishez");
                        OpenDishFocused = false;
                        OpenDishVisibility = Visibility.Visible;
                        OpenDishFocused = true;
                    }
                    else
                    {
                        NumCountFocused = true;
                    }

                }
                else
                {
                    OpenDishVisibility = Visibility.Collapsed;
                }
                OnPropertyChanged("SelectedDish");
            }
        }

        public DishPackageFlightOrder AddToOrderDish
        {
            get
            {
                return Model.AddToOrderDish;
            }

        }

        public DishPackageFlightOrder RemoveFromOrderDish
        {
            get
            {
                return Model.RemoveToOrderDish;
            }
            set
            {
                Model.RemoveToOrderDish = value;
                RaisePropertyChanged("DeleteDishBtnName");
            }
        }


        public long AddToOrderDishBarCode
        {
            get
            {
                return Model.AddToOrderDish.Dish.Barcode;
            }
        }


        public int AddToOrderDishOrderFlightId
        {
            get
            {
                return Model.AddToOrderDish.PassageNumber;
            }
            set
            {
                Model.AddToOrderDish.PassageNumber = value;
            }

        }

        public decimal AddToOrderDishAmount
        {
            get
            {
                return Model.AddToOrderDish.Amount;
            }
            set
            {
                Model.AddToOrderDish.Amount = value;
                OnPropertyChanged("AddToOrderDishAmount");
                OnPropertyChanged("AddToOrderDishSumm");
            }

        }

        public decimal AddToOrderDishPrice
        {
            get
            {
                return Model.AddToOrderDish.TotalPrice;
            }
            set
            {
                Model.AddToOrderDish.TotalPrice = value;
                OnPropertyChanged("AddToOrderDishPrice");
                OnPropertyChanged("AddToOrderDishSumm");
            }
        }

        public decimal AddToOrderDishSumm
        {
            get
            {
                return Model.AddToOrderDish.TotalSumm;
            }
        }

        public string AddToOrderDishComment
        {
            get
            {
                return Model.AddToOrderDish.Comment;
            }
            set
            {
                Model.AddToOrderDish.Comment = value;
                OnPropertyChanged("AddToOrderDishComment");
            }


        }


        ICollectionView _dishCollectionSource;
        /*
        public event PropertyChangedEventHandler PropertyChanged;
        void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        */

        public ICollectionView DishCollectionSource
        {
            get
            {
                if (_dishCollectionSource == null)
                {
                    QueryableCollectionView collectionViewSource = new QueryableCollectionView(Model.DishCollection);

                    _dishCollectionSource = collectionViewSource;
                    _dishCollectionSource.SortDescriptions.Add(
                        new SortDescription("Name", ListSortDirection.Ascending));

                    _dishCollectionSource.MoveCurrentToFirst();
                }

                return _dishCollectionSource;
            }
        }

        public string FlightNumber
        {
            get
            {
                return Model.Order.FlightNumber;
            }
            set
            {
                Model.Order.FlightNumber = value;
            }
        }

        public string OrderTotalStr
        {
            get
            {
                Calc.CalcOrderDisc(Model.Order);
                string s = $"Сумма заказа: {Model?.OrderSumm.ToString("c", CultureInfo.GetCultureInfo("ru-Ru"))} {Environment.NewLine}";
                s += $"Сумма надбавки: {(Model?.OrderSumm * Model?.Order.ExtraCharge / 100).GetValueOrDefault().ToString("c", CultureInfo.GetCultureInfo("ru-Ru"))} {Environment.NewLine}";
                s += $"Сумма скидки: {Model?.OrderDiscount.ToString("c", CultureInfo.GetCultureInfo("ru-Ru"))} {Environment.NewLine}";
                s += $"Итого: {Model?.OrderTotalSumm.ToString("c", CultureInfo.GetCultureInfo("ru-Ru")) } ";

                return s;
            }
        }
        public string OrderCreatorStr
        {
            get
            {
                return $"Заказ принял: {Model.Order.CreatedBy.FullName}";
            }
        }

        public string OrderSenderStr
        {
            get
            {
                switch (Model.Order.OrderStatus)
                {
                    case OrderStatus.InWork:
                        return "";
                    case OrderStatus.Cancelled:
                        return $"Заказ отменил: {Model.Order.SendBy?.FullName}";
                    case OrderStatus.Sent:
                        return $"Заказ отправил: {Model.Order.SendBy?.FullName}";
                    case OrderStatus.CancelledWithRemains:
                        return $"Заказ отменил: {Model.Order.SendBy?.FullName}";
                    case OrderStatus.Closed:
                        return $"Заказ отправил: {Model.Order.SendBy?.FullName}";
                    default:
                        break;
                }
                return "";
            }
        }



        public String OrderStatusStr
        {
            get
            {
                return $"Статус заказа: {Model.Order.OrderStatus.GetDescription()}";
            }

        }


        public int NumberOfBoxes
        {
            get
            {
                return Model.Order.NumberOfBoxes;
            }
            set

            {
                Model.Order.NumberOfBoxes = value;
            }
        }

        public DateTime DeliveryDate
        {
            get
            {
                return Model.Order.DeliveryDate;
            }
            set
            {
                Model.Order.DeliveryDate = new DateTime(value.Ticks, DateTimeKind.Utc);


                ReadyTime = value;
                ExportTime = value;
                OnPropertyChanged("DeliveryDate");
                //  ExportTime = value.AddHours(-2);


            }
        }

        public DateTime ExportTime
        {
            get
            {
                return Model.Order.ExportTime;
            }
            set
            {
                Model.Order.ExportTime = new DateTime(value.Ticks, DateTimeKind.Utc); ;
                OnPropertyChanged("ExportTime");
                //ReadyTime = value.AddHours(-1);
            }
        }
        public DateTime ReadyTime
        {
            get
            {
                return Model.Order.ReadyTime;
            }
            set
            {
                Model.Order.ReadyTime = new DateTime(value.Ticks, DateTimeKind.Utc); ;
                OnPropertyChanged("ReadyTime");
            }
        }
        public string OrderComment
        {
            get
            {
                return Model.Order.Comment;
            }
            set
            {
                if (Model != null && Model.Order != null && TextHelper.GetTextFromRadDocText(Model.Order.Comment) != TextHelper.GetTextFromRadDocText(value))
                {
                    Model.Order.Comment = value;
                    OnPropertyChanged("OrderComment");
                }
            }
        }


        public string SelectedOpenDishName
        {
            get
            {
                return Model.OpenDishName;
            }
            set
            {
                if (Model.OpenDishName != value)
                {
                    Model.OpenDishName = value;
                    RaisePropertyChanged("SelectedOpenDishName");
                }
            }
        }



        public Dish selectedOpenDish;
        public Dish SelectedOpenDish
        {
            get
            {
                return Model.SelectedOpenDish;
            }
            set
            {
                if (Model.SelectedOpenDish != value)
                {
                    Model.SelectedOpenDish = value;
                    RaisePropertyChanged("SelectedOpenDish");
                }
            }
        }

        ICollectionView _openDishez;
        public ICollectionView OpenDishez
        {
            get
            {
                if (Model != null && SelectedDish != null)
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


        /*
        public CatalogComboBoxEnumViewModel<OrderStatus> OrderStatusVM
        {
            get
            {
                return
                    new CatalogComboBoxEnumViewModel<OrderStatus>()
                    {
                        DataCatalog = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>() ,
                        Header = "Статус заказа",
                        SelectedData = OrderStatusStr
                    };
            }
        }
        */
        CatalogComboBoxViewModel<AirCompany> airCompaniesVM;
        public CatalogComboBoxViewModel<AirCompany> AirCompaniesVM
        {
            get
            {
                if (airCompaniesVM == null)
                {
                    airCompaniesVM = new CatalogComboBoxViewModel<AirCompany>()
                    {
                        DataCatalog = Model.AirCompanyes,
                        Header = "Авиакомпания",
                        EmptyText = "Укажите авиакомпанию..",
                        //SelectedData = SelectedAirCompany
                        DisplayMemberPathName = "Name",

                    };
                    airCompaniesVM.IsFocused = Model.NewOrder;
                    airCompaniesVM.ReturnCommand = new DelegateCommand(_ => { BortNumberFocused = true; BortNumberFocused = false; });
                    //Инициализация ComboBox
                    try { airCompaniesVM.SelectedData = Model.AirCompanyes.Single(a => a.Id == SelectedAirCompany.Id); } catch { }



                    airCompaniesVM.SelectedDataChanged += new EventHandler<AirCompany>((sender, e) =>
                        {
                            SelectedAirCompany = e;
                        });
                }
                return airCompaniesVM;
            }

        }

        public DateTime DateNow
        {
            get { return DateTime.Now.Date; }
        }

        private void AirCompaniesVM_SelectedDataChanged(object sender, AirCompany e)
        {
            throw new NotImplementedException();
        }


        public CatalogComboBoxViewModel<DeliveryPlace> deliveryPlaceVM;
        public CatalogComboBoxViewModel<DeliveryPlace> DeliveryPlaceVM
        {
            get
            {
                if (deliveryPlaceVM == null)
                {
                    deliveryPlaceVM = new CatalogComboBoxViewModel<DeliveryPlace>()
                    {
                        DataCatalog = Model.DeliveryPlaces,
                        Header = "Место доставки",
                        EmptyText = "Укажите место доставки..",
                        //SelectedData = SelectedDeliveryPlace,
                        DisplayMemberPathName = "Name",
                    };
                    try { deliveryPlaceVM.SelectedData = Model.DeliveryPlaces.Single(a => a.Id == SelectedDeliveryPlace.Id); } catch { }

                    deliveryPlaceVM.SelectedDataChanged += new EventHandler<DeliveryPlace>((sender, e) =>
                   {
                       SelectedDeliveryPlace = e;
                   });
                }
                return deliveryPlaceVM;
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
                        DataCatalog = Model.DeliveryPerson,
                        Header = "Кто отвез",
                        EmptyText = "Укажите кто отвез..",
                        DisplayMemberPathName = "FullName"
                    };
                    try { deliveryPerconVM.SelectedData = Model.DeliveryPerson.Single(a => a.Id == SelectedDeliveryPerson.Id); } catch { }
                    deliveryPerconVM.SelectedDataChanged += new EventHandler<Driver>((sender, e) =>
                    {
                        SelectedDeliveryPerson = e;
                    });
                }
                return deliveryPerconVM;
            }
        }

        public FullyObservableCollection<ContactPerson> ContactPersonPhoneCatalog
        {
            get
            {
                return new FullyObservableCollection<ContactPerson>(Model.ContactPerson.Where(a => a.Phone != ""));
            }
        }



        public FullyObservableCollection<ContactPerson> ContactPersonCatalog
        {
            get
            {
                return Model.ContactPerson;
            }
        }




        /*
        public CatalogComboBoxViewModel<ContactPerson> ContactPersonPhoneVM
        {
            get
            {
                return
                    new CatalogComboBoxViewModel<ContactPerson>()
                    {
                        DataCatalog =new FullyObservableCollection<ContactPerson>(Model.ContactPerson.Where(a=>a.Phone!="")),
                        Header = "Телефон",
                        EmptyText = "Укажите кто отвез..",
                        SelectedData = SelectedContactPerson,
                        DisplayMemberPathName = "Phone"
                    };
            }
        }
        */
        /*
        public CatalogComboBoxViewModel<ContactPerson> ContactPersonVM
        {
            get
            {
                return
                    new CatalogComboBoxViewModel<ContactPerson>()
                    {
                        DataCatalog = Model.ContactPerson,
                        Header = "Контактное лицо",
                        EmptyText = "Укажите контактное лицо..",
                        SelectedData = SelectedContactPerson,
                        DisplayMemberPathName = "FullName",
                        HeaderVisible = false
                    };
            }
        }
        */

    }

    public class AddOrderModel
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public bool NewOrder = false;
        public AddOrderModel(OrderFlight order)
        {
            //Order = (OrderFlight)order.Clone();
            Order = order;
            OrderDishez = new FullyObservableCollection<DishPackageFlightOrder>(order.DishPackages);

            EventsInit();
        }

        public AddOrderModel()
        {
            NewOrder = true;
            Order = new OrderFlight()
            {

                CreatedBy = Authorization.CurentUser,
                OrderStatus = OrderStatus.InWork,

            };
            OrderDishez = new FullyObservableCollection<DishPackageFlightOrder>();
            EventsInit();
            Order.DeliveryDate = new DateTime(DateTime.Now.AddDays(1).Date.Ticks, DateTimeKind.Utc);
            Order.ExportTime = new DateTime(DateTime.Now.AddDays(1).Date.Ticks, DateTimeKind.Utc);
            Order.CreationDate = new DateTime(DateTime.Now.Ticks, DateTimeKind.Utc);
            Order.ReadyTime = new DateTime(DateTime.Now.AddDays(1).Date.Ticks, DateTimeKind.Utc);
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

        public OrderFlight Order { set; get; }

        public delegate void OrderChangedHandler();
        public event OrderChangedHandler OrderChanged;


        public string SelectedContactPersonName { set; get; }
        public string SelectedContactPersonPhone { set; get; }
        public string SelectedContactPersonEmail { set; get; }

        public decimal OrderSumm
        {
            get
            {
                return OrderDishez.Where(x => !x.Deleted).Sum(a => a.TotalSumm);
            }
        }
        public decimal OrderTotalSumm
        {
            get
            {
                return OrderDishez.Where(x => !x.Deleted).Sum(a => a.TotalSumm) * (1 + Order.ExtraCharge / 100) - OrderDiscount;
            }
        }
        public decimal OrderDiscount
        {
            get
            {
                return Order.DiscountSumm;
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

        ContactPerson addContactPercon()
        {
            var cP = new AlohaService.ServiceDataContracts.ContactPerson()
            {
                Email = SelectedContactPersonEmail.Trim(),
                Phone = SelectedContactPersonPhone.Trim(),
                FirstName = new string(SelectedContactPersonName.Trim().TakeWhile(a => a != ' ').ToArray()),
                SecondName = new string(SelectedContactPersonName.SkipWhile(a => a != ' ').ToArray()).Trim()
            };


            var aCPVM = new AddContactPersonViewModel(cP);
            var addContactPersonWnd = new UI.CtrlAddContactPerson();
            addContactPersonWnd.DataContext = aCPVM;
            aCPVM.CloseAction = new Action(addContactPersonWnd.Close);
            UI.UIModify.ShowDialogWnd(addContactPersonWnd);
            if (aCPVM.Result) { return cP; }
            return null;

        }




        public bool SaveOrder()
        {
            logger.Debug($"SaveOrder ToFly {Order.Id}");
            var addRes = true;


            try
            {
                if (Order.AirCompany == null)
                {
                    UI.UIModify.ShowAlert("Нельзя сохранить заказ без указания авиакомпании");
                    return false;
                }

                if (Order.FlightNumber == null)
                {
                    UI.UIModify.ShowAlert("Нельзя сохранить заказ без указания номера борта");
                    return false;
                }

                if (Order.ContactPerson == null)
                {
                    addRes = false;
                    ContactPerson cP = addContactPercon();
                    if (cP != null) { Order.ContactPerson = cP; addRes = true; }
                    else
                    {
                        UI.UIModify.ShowAlert("Нельзя сохранить заказ без указания контактного лица");
                    }
                }


                Order.DishPackages = OrderDishez.ToList();
                Order.IsSHSent = true;
                bool addToBaseRes = false;
                if (addRes)
                {
                    if (Order.Id == 0)
                    {
                        addToBaseRes = Models.AirOrdersModelSingleton.Instance.AddOrder(Order, OrderDishez.ToList());
                    }
                    else
                    {

                        addToBaseRes = Models.AirOrdersModelSingleton.Instance.UpdateOrder(Order);
                    }
                    try
                    {
                        //Это чтобы по ToFly весь заказ выходил. Эвелина просила.
                        foreach (var d in Order.DishPackages)
                        {
                            d.Printed = false;
                        }
                        /*
                        if (Order.DishPackages != null && Order.DishPackages.Any(a => !a.Printed))
                        {
                        //Печатаем
                            List<string> outErr = new List<string>();
                            if (!PrintRecieps.PrintOnWinPrinter.PrintOrderToGoToKitchen(Order, out outErr))
                            {
                                foreach (string s in outErr)
                                {
                                    UI.UIModify.ShowAlert(s);
                                }
                            }
                            else
                            {
                                foreach (var d in Order.DishPackages)
                                {
                                    d.Printed = true;
                                }
                            }

                        }
                        */
                    }
                    catch
                    { }
                }

                if (addToBaseRes)
                {
                    //SH.SHWrapper.CreateSalesInvoice(Order);

                    if (Order.OrderStatus != OrderStatus.InWork)
                    {

                        var CreateSHres = SH.SHWrapper.CreateSalesInvoiceSync(Order, out string err);
                        if (!CreateSHres)
                        {
                            UI.UIModify.ShowAlert($"Ошибка создания документа расхода. " + Environment.NewLine + err);
                            //UI.UIModify.ShowAlert($"{err + Environment.NewLine} Накладная будет создана при появлении связи со StoreHouse");
                            Models.AirOrdersModelSingleton.Instance.UpdateOrder(Order);
                        }
                    }
                    return true;
                }


                return false;
            }
            catch (Exception e)
            {
                logger.Error($"SaveOrder {Order.Id} Mess: {e.Message}");
                return false;
            }
            finally
            {

            }
        }

        public string OpenDishName { set; get; }
        public Dish SelectedOpenDish { set; get; } = null;

        public void AddDishToOrder()
        {
            try
            {


                if ((AddToOrderDish == null) || (AddToOrderDish.Dish == null) || (AddToOrderDish.Dish.Barcode == 0)) return;
                logger.Debug("AddDish " + AddToOrderDish.Dish.Barcode);
                bool res = true;
                if (DataCatalogsSingleton.Instance.OpenDishezBarCodes.Contains(AddToOrderDish.Dish.Barcode))
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
                    OrderDishez.Add((DishPackageFlightOrder)AddToOrderDish.Clone());
                }
            }
            catch (Exception e)
            {
                logger.Debug("AddDish err " + e.Message);
            }
        }




        public void RemoveDishFromOrder()
        {
            if (RemoveToOrderDish != null)
            {
                if (Order.OrderStatus == OrderStatus.InWork)
                {
                    foreach (var ord in OrderDishez.Where(a => a.PositionInOrder > RemoveToOrderDish.PositionInOrder))
                    {
                        ord.PositionInOrder--;
                    }
                    OrderDishez.Remove(RemoveToOrderDish);

                }
                else
                {
                    if (WndDeleteDishModel.ShowWndDeleteDish(RemoveToOrderDish))
                    {
                        if (RemoveToOrderDish.Deleted)
                        {
                            bool printDeleted = true;
                            var resPrinted = new List<string>();
                            printDeleted = PrintRecieps.PrintOnWinPrinter.PrintOrderToGoToKitchen(Order, out resPrinted, new List<IDishPackageLabel>() { RemoveToOrderDish });
                        }
                    }
                }

                //RemoveToOrderDish.Deleted = true;
                /*
                foreach (var ord in OrderDishez.Where(a => a.PositionInOrder > RemoveToOrderDish.PositionInOrder))
                {
                    ord.PositionInOrder--;
                }
                OrderDishez.Remove(RemoveToOrderDish);
                */
            }
        }

        public FullyObservableCollection<DishPackageFlightOrder> OrderDishez { set; get; }




        public FullyObservableCollection<AirCompany> AirCompanyes
        {
            get
            {
                return DataExtension.DataCatalogsSingleton.Instance.AirCompanies;
            }
        }

        public FullyObservableCollection<DeliveryPlace> DeliveryPlaces
        {
            get
            {
                return DataExtension.DataCatalogsSingleton.Instance.DeliveryPlaces;
            }
        }

        public FullyObservableCollection<Driver> DeliveryPerson
        {
            get
            {
                return DataExtension.DataCatalogsSingleton.Instance.Drivers;
            }
        }
        public FullyObservableCollection<ContactPerson> ContactPerson
        {
            get
            {
                return DataExtension.DataCatalogsSingleton.Instance.ContactPerson;
            }
        }



        //public DishPackageFlightOrder SelectedForAddDish {set;get;}


        public DishPackageFlightOrder AddToOrderDish { set; get; } = new DishPackageFlightOrder();
        public DishPackageFlightOrder RemoveToOrderDish { set; get; } = new DishPackageFlightOrder();

        public FullyObservableCollection<Dish> DishCollection
        {
            get
            {
                return DataExtension.DataCatalogsSingleton.Instance.ActiveDishesToFly;
            }
        }
    }
}

