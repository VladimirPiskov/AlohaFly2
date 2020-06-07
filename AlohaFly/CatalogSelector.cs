using AlohaFly.DataExtension;
using AlohaFly.Models;
using AlohaFly.Utils;
using AlohaService.ServiceDataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace AlohaFly
{
    public static class CatalogSelector
    {


        static List<MenuItemCatalog> _importItemsCatalog;
        public static List<MenuItemCatalog> ImportItemsCatalog
        {
            get
            {
                if (_importItemsCatalog == null)
                {
                    _importItemsCatalog = new List<MenuItemCatalog>()
                    {
                        new MenuItemCatalog("ToFly из RKeeper", new Import.DataImportReaderFromExcel(), AccessTypeConst.Access_ImportToFlyRkeeper,false),
                        new MenuItemCatalog("Обновить гастроцены", Utils.EditGastroPrices.Edit, AccessTypeConst.Access_ImportToFlyRkeeper,false),
                        new MenuItemCatalog("Обновить блюда ToFly ", new Import.PricesImportToFlyFromExcel(), AccessTypeConst.Access_ImportToFlyRkeeper,false),
                        new MenuItemCatalog("Обновить блюда ToGo ", new Import.PricesImportToGoFromExcel(), AccessTypeConst.Access_ImportToFlyRkeeper,false),
                        new MenuItemCatalog("Синхрогизировать цены в SH", Utils.SincSHPrices.Sinc, AccessTypeConst.Access_ImportToFlyRkeeper,false),
                        //new MenuItemCatalog("8to+7", Utils.EditCustomers.MergeCustomers, AccessTypeConst.Access_ImportToFlyRkeeper,false),

                    };
                }
                return _importItemsCatalog;
            }
        }


        static List<MenuItemCatalog> _itemsCatalog;
        public static List<MenuItemCatalog> ItemsCatalog
        {
            get
            {
                if (_itemsCatalog == null)
                {
                    _itemsCatalog = new List<MenuItemCatalog>()
                    {
                        new MenuItemCatalog ("Авиакомпании",ShowAirCompanyCatalog, AccessTypeConst.Access_CatalogAirCompany,false),
                        new MenuItemCatalog ("Блюда все",ShowItemsCatalog, AccessTypeConst.Access_CatalogDish,false),
                         new MenuItemCatalog ("Блюда ToFly",ShowItemsCatalogToFly, AccessTypeConst.Access_CatalogDish,false),
                         new MenuItemCatalog ("Блюда ToGo",ShowItemsCatalogToGo, AccessTypeConst.Access_CatalogDish,false),
                         new MenuItemCatalog ("Блюда Временные",ShowItemsCatalogTemp, AccessTypeConst.Access_CatalogDish,false),
                         new MenuItemCatalog ("Места доставки",ShowDeliveryPlaceCatalog, AccessTypeConst.Access_CatalogDeliveryPlace,false),
                         new MenuItemCatalog ("Водители",ShowDriverCatalog, AccessTypeConst.Access_CatalogDriver,false),
                         new MenuItemCatalog ("Стюардессы",ShowContactPersons, AccessTypeConst.Access_CatalogContactPerson,false),
                         new MenuItemCatalog ("Наклейки",ShowItemLabelsInfo, AccessTypeConst.Access_DishLabels,false),
                         new MenuItemCatalog ("Наклейки открытые",ShowOpenItemLabelsInfo, AccessTypeConst.Access_DishLabels,false),
                         new MenuItemCatalog ("Группы блюд",ShowLogicDishGroups, AccessTypeConst.Access_CatalogLogicGroup,false),
                         new MenuItemCatalog ("Кухонные группы блюд",ShowKitchenDishGroups, AccessTypeConst.Access_CatalogKitchenGroup,false),
                         //new MenuItemCatalog ("Клиенты ToGo",ShowToGoCustomers, AccessTypeConst.Access_ToGoClients,false),
                         new MenuItemCatalog ("Клиенты ToGo",ShowToGoCustomers2, AccessTypeConst.Access_ToGoClients,false),
                         new MenuItemCatalog ("Каналы маркетинга",ShowMarketingChanelsCatalog, AccessTypeConst.Access_MarketingChanels,false),
                         new MenuItemCatalog ("Оплаты",ShowPaymentsCatalog, AccessTypeConst.Access_MarketingChanels,false),
                         new MenuItemCatalog ("Группы оплат",ShowPaymentGroupsCatalog, AccessTypeConst.Access_MarketingChanels,false),

                    };
                }
                return _itemsCatalog;

            }
        }


        static List<MenuItemCatalog> _userItemsCatalog;
        public static List<MenuItemCatalog> UserItemsCatalog
        {
            get
            {
                if (_userItemsCatalog == null)
                {
                    _userItemsCatalog = new List<MenuItemCatalog>()
                    {
                        new MenuItemCatalog ("Сменить мой пароль..",ShowChangePass, AccessTypeConst.Access_ChangeMyPass,false),

                    };
                }
                return _userItemsCatalog;

            }
        }

        static List<MenuItemCatalog> _orderItemsCatalog;
        public static List<MenuItemCatalog> OrderItemsCatalog
        {
            get
            {
                if (_orderItemsCatalog == null)
                {
                    _orderItemsCatalog = new List<MenuItemCatalog>()
                    {
                        new MenuItemCatalog ("Заказы ToFly",ShowOrdersToFly, AccessTypeConst.Access_OrdersToFly,false),


                        new MenuItemCatalog ("Новый заказ ToFly",ShowAddOrdersToFly, AccessTypeConst.Access_OrdersToFly,false),
                        new MenuItemCatalog ("Новый заказ ToGo",ShowAddOrdersToGo, AccessTypeConst.Access_OrdersToGo,false),
                        new MenuItemCatalog ("Заказы по авиакомпаниям",ShowAirCompaneisOrdersToFly, AccessTypeConst.Access_OrdersToFlyAirComps,false),
                        new MenuItemCatalog ("Не списаные заказы",ShowOrdersNonSH, AccessTypeConst.Access_OrdersToFly,false),
                    };

                    if (Authorization.CurentUser.UserName != "sh.user")
                    {
                        _orderItemsCatalog.Add(new MenuItemCatalog("Заказы ToGo", ShowOrdersToGo, AccessTypeConst.Access_OrdersToGo, false));
                    }
                }
                return _orderItemsCatalog;

            }
        }

        static List<MenuItemCatalog> _reportsCatalog;
        public static List<MenuItemCatalog> ReportsCatalog
        {
            get
            {
                if (_reportsCatalog == null)
                {
                    _reportsCatalog = new List<MenuItemCatalog>()
                    {
                        new MenuItemCatalog ("Печать по авиакомпаниям",PrintRep1, AccessTypeConst.Access_Reports_Rep1,false),
                        /*
                        new MenuItemCatalog ("Расход блюд",new Action(()=>{ Task.Run(()=>
                            new Reports.ExcelReports().DBFGetRashDishez(DataCatalogsSingleton.Instance.StartDt, DataCatalogsSingleton.Instance.EndDt)); })
                        
                        , AccessTypeConst.Access_Reports_Rep1,false),
                        

                        new MenuItemCatalog("Расход блюд по категориям", new Action(() => { Task.Run(() =>
                               new Reports.ExcelReports().DBFGetRashDishezOnCat(DataCatalogsSingleton.Instance.StartDt, DataCatalogsSingleton.Instance.EndDt)); })
                        , AccessTypeConst.Access_Reports_Rep1, false),
                        

                        new MenuItemCatalog("Расход блюд по группам станций", new Action(() => { Task.Run(() =>
                               new Reports.ExcelReports().DBFGetRashDishezOnStationGroup(DataCatalogsSingleton.Instance.StartDt, DataCatalogsSingleton.Instance.EndDt)); })
                        , AccessTypeConst.Access_Reports_Rep1, false),
                        
                        new MenuItemCatalog("Общая сумма", new Action(() => { Task.Run(() =>
                               new Reports.ExcelReports().DBFGetSales(DataCatalogsSingleton.Instance.StartDt, DataCatalogsSingleton.Instance.EndDt)); })
                        , AccessTypeConst.Access_Reports_Rep1, false),
                        */
                        new MenuItemCatalog("Выручка станций по дням", new Action(() => { Task.Run(() =>
                               new Reports.ExcelReports().DBFGetSalesByDays(DataCatalogsSingleton.Instance.StartDt, DataCatalogsSingleton.Instance.EndDt)); })
                        , AccessTypeConst.Access_Reports_Rep1, false),

                        new MenuItemCatalog("Выручка по категориям", new Action(() => { Task.Run(() =>
                               new Reports.ExcelReports().DBFGetSalesByCats(DataCatalogsSingleton.Instance.StartDt, DataCatalogsSingleton.Instance.EndDt)); })
                        , AccessTypeConst.Access_Reports_Rep1, false),
                        new MenuItemCatalog("Отчет по блюдам", new Action(() => { Task.Run(() =>
                               new Reports.ExcelReports().DBFGetDishezByPayment(DataCatalogsSingleton.Instance.StartDt, DataCatalogsSingleton.Instance.EndDt)); })
                        , AccessTypeConst.Access_Reports_Rep1, false),
                        new MenuItemCatalog("Отчет по компаниям", new Action(() => { Task.Run(() =>
                               new Reports.ExcelReports().DBFGetPaymentsSumm(DataCatalogsSingleton.Instance.StartDt, DataCatalogsSingleton.Instance.EndDt)); })
                        , AccessTypeConst.Access_Reports_Rep1, false),

                        /*
                        new MenuItemCatalog("Отчет по оплатам", new Action(() => { Task.Run(() =>
                               new Reports.ExcelReports().DBFGetPaymentsSumm(DataCatalogsSingleton.Instance.StartDt, DataCatalogsSingleton.Instance.EndDt)); })
                        , AccessTypeConst.Access_Reports_Rep1, false),
                        */
                        /*
                         new MenuItemCatalog ("ToGo Отчет по оплатам",new Action(()=>{
                            new Reports.ExcelReports().DBFGetPaymentsSummToGo(DataCatalogsSingleton.Instance.StartDt, DataCatalogsSingleton.Instance.EndDt);})
                        , AccessTypeConst.Access_Reports_Rep1,false),
                        */

                        new MenuItemCatalog("Общая выручка new", new Action(() => { Task.Run(() =>
                               new Reports.ExcelReports().DBFGetSales2(DataCatalogsSingleton.Instance.StartDt, DataCatalogsSingleton.Instance.EndDt)); })
                        , AccessTypeConst.Access_Reports_Rep1, false),
                         new MenuItemCatalog("Баланс new", new Action(() => { Task.Run(() =>
                               new Reports.ExcelReports().DBFGetBalance(DataCatalogsSingleton.Instance.StartDt, DataCatalogsSingleton.Instance.EndDt)); })
                        , AccessTypeConst.Access_Reports_Rep1, false),

                         new MenuItemCatalog("Отчет по отказам new", new Action(() => { Task.Run(() =>
                               new Reports.ExcelReports().DBFGetVoids2(DataCatalogsSingleton.Instance.StartDt, DataCatalogsSingleton.Instance.EndDt)); })
                        , AccessTypeConst.Access_Reports_Rep1, false),

                         new MenuItemCatalog("Отчет по открытым блюдам", new Action(() => { Task.Run(() =>
                               new Reports.ExcelReports().DBFGetRashOpenDishezOnCat(DataCatalogsSingleton.Instance.StartDt, DataCatalogsSingleton.Instance.EndDt)); })
                        , AccessTypeConst.Access_Reports_Rep1, false),

                         new MenuItemCatalog("Выручка по дням", new Action(() => { Task.Run(() =>
                               new Reports.ExcelReports().SaleByDayAndDepReport(DataCatalogsSingleton.Instance.StartDt, DataCatalogsSingleton.Instance.EndDt)); })
                        , AccessTypeConst.Access_Reports_Rep1, false),

                         new MenuItemCatalog("Общий отчет", new Action(() => { Task.Run(() =>
                               Reports.GKANReports.Instanse.ShowCommonReport(DataCatalogsSingleton.Instance.StartDt, DataCatalogsSingleton.Instance.EndDt)); })
                        , AccessTypeConst.Access_Reports_Rep1, false),
                         new MenuItemCatalog("Клиенты ToGo", new Action(() => { Task.Run(() =>
                               Reports.ToGoReports.Instanse.ShowClientsReport(DataCatalogsSingleton.Instance.StartDt, DataCatalogsSingleton.Instance.EndDt)); })
                        , AccessTypeConst.Access_Reports_Rep1, false),

                    };
                }
                return _reportsCatalog;

            }
        }


        public static void PrintRep1()
        {
            var rep = new Reports.ExcelReports();
            rep.AllOrdersToExcelByComps(AirOrdersModelSingleton.Instance.AirCompanyOrders.ToList(),
                DataCatalogsSingleton.Instance.StartDt, DataCatalogsSingleton.Instance.EndDt);
        }

        public static void ShowContactPersons()
        {
            var _model = new Models.CatalogModel<ContactPerson>(
                DataCatalogsSingleton.Instance.ContactPersonData
                /*
            new Models.EditCatalogDataFuncs<ContactPerson>()
            {
                AddItemFunc = DBProvider.Client.CreateContactPerson,
                EditItemFunc = DBProvider.Client.UpdateContactPerson,
                CancelAddItemFunc = DBProvider.Client.DeleteContactPerson,
                AllDataList = DataExtension.DataCatalogsSingleton.Instance.ContactPerson
            }
            */
            );
            var vm = new Models.CatalogViewModel<ContactPerson>(_model);
            vm.CanDeleteItem = false;
            ShowCatItem(vm, "Справочник стюардесс");
        }

        public static void ShowLogicDishGroups()
        {
            var _model = new Models.CatalogModel<DishLogicGroup>(
                DataCatalogsSingleton.Instance.DishLogicGroupData
            /*
        new Models.EditCatalogDataFuncs<DishLogicGroup>()
        {
            AddItemFunc = DBProvider.Client.CreateDishLogicGroup,
            EditItemFunc = DBProvider.Client.UpdateDishLogicGroup,
            CancelAddItemFunc = DBProvider.Client.DeleteDishLogicGroup,
            AllDataList = DataExtension.DataCatalogsSingleton.Instance.DishLogicGroup
        }
        */
            );
            var vm = new Models.CatalogViewModel<DishLogicGroup>(_model);
            vm.CanDeleteItem = false;
            ShowCatItem(vm, "Справочник логических групп блюд");
        }


        public static void ShowKitchenDishGroups()
        {
            var _model = new Models.CatalogModel<DishKitchenGroup>(
              DataCatalogsSingleton.Instance.DishKitchenGroupData
            /*
        new Models.EditCatalogDataFuncs<DishKitchenGroup>()
        {
            AddItemFunc = DBProvider.Client.CreateDishKitchenGroup,
            EditItemFunc = DBProvider.Client.UpdateDishKitchenGroup,
            CancelAddItemFunc = DBProvider.Client.DeleteDishKitchenGroup,
            AllDataList = DataExtension.DataCatalogsSingleton.Instance.DishKitchenGroup
        }
        */
            );
            var vm = new Models.CatalogViewModel<DishKitchenGroup>(_model);
            vm.CanDeleteItem = false;
            ShowCatItem(vm, "Справочник кухонных групп");
        }

        /*
        public static void ShowToGoCustomers()
        {
            var vm = new Models.ToGoClientCatalogViewModel();
            vm.Header = "Клиенты ToGo";
            var ctrlItemlabels = new UI.CtrlToGoClientCatalog
            {
                DataContext = vm
            };
            MainClass.ShowUC(ctrlItemlabels);

        }
        */
        public static void ShowToGoCustomers2()
        {
            var vm = new Models.ToGoClient.ToGoClientsViewModel();
            vm.Header = "Клиенты ToGo2";
            var ctrlItemlabels = new UI.ToGo.CtrlToGoClientCatalog2
            {
                DataContext = vm
            };
            MainClass.ShowUC(ctrlItemlabels);

        }

        public static void ShowItemLabelsInfo()
        {

            var vm = new Models.AddLabelsViewModel(DataExtension.DataCatalogsSingleton.Instance.DishData.Data.First());
            vm.Header = "Редактирование наклеек";
            var ctrlItemlabels = new UI.ctrlItemLabels();
            ctrlItemlabels.DataContext = vm;

            MainClass.ShowUC(ctrlItemlabels);

        }


        public static void ShowOpenItemLabelsInfo()
        {

            var vm = new Models.AddLabelsViewModel(DataExtension.DataCatalogsSingleton.Instance.DishFilter.OpenDishes.First(), true);
            vm.Header = "Редактирование открытых наклеек";
            var ctrlItemlabels = new UI.ctrlItemLabels();
            ctrlItemlabels.DataContext = vm;

            MainClass.ShowUC(ctrlItemlabels);

        }


        public static void ShowAirCompanyCatalog()
        {

            var _model = new Models.CatalogModel<AirCompany>(DataCatalogsSingleton.Instance.AirCompanyData);
            var ItemsCatalog = new UI.CtrlCatalogView2();
            var ItemsCatalogVm = new Models.CatalogViewModel<AirCompany>(_model) { Header = "Справочник авиакомпаний" };
            ItemsCatalogVm.CanDeleteItem = false;
            ItemsCatalog.DataContext = ItemsCatalogVm;
            MainClass.ShowUC(ItemsCatalog);



            /*
            var _model = new Models.CatalogModel<AirCompany>(
            new Models.EditCatalogDataFuncs<AirCompany>()
            {
                AddItemFunc = DBProvider.Client.CreateAirCompany,
                EditItemFunc = DBProvider.Client.UpdateAirCompany,
                CancelAddItemFunc = DBProvider.Client.DeleteAirCompany,
                AllDataList = DataExtension.DataCatalogsSingleton.Instance.AllAirCompan
            }
            );
            var vm = new Models.CatalogViewModel<AirCompany>(_model);
            vm.CanDeleteItem = false;
            ShowCatItem(vm, "Справочник авиакомпаний");
            */




        }

        public static void ShowDriverCatalog()
        {
            var _model = new Models.CatalogModel<Driver>(DataCatalogsSingleton.Instance.DriverData);
            var ItemsCatalog = new UI.CtrlCatalogView2();
            var ItemsCatalogVm = new Models.CatalogViewModel<Driver>(_model) { Header = "Справочник водителей" };
            ItemsCatalogVm.CanDeleteItem = false;
            ItemsCatalog.DataContext = ItemsCatalogVm;
            MainClass.ShowUC(ItemsCatalog);


            /*
            var _model = new Models.CatalogModel<Driver>(
            new Models.EditCatalogDataFuncs<Driver>()
            {
                AddItemFunc = DBProvider.Client.CreateDriver,
                EditItemFunc = DBProvider.Client.UpdateDriver,
                CancelAddItemFunc = DBProvider.Client.DeleteDriver,
                AllDataList = DataExtension.DataCatalogsSingleton.Instance.Drivers
            }
            );

            var vm = new Models.CatalogViewModel<Driver>(_model);
            vm.CanDeleteItem = false;
            ShowCatItem(vm, "Справочник водителей");
            */
        }

        public static void ShowMarketingChanelsCatalog()
        {
            var _model = new Models.CatalogModel<MarketingChannel>(DataCatalogsSingleton.Instance.MarketingChannelData);
            var ItemsCatalog = new UI.CtrlCatalogView2();
            var ItemsCatalogVm = new Models.CatalogViewModel<MarketingChannel>(_model) { Header = "Справочник каналов маркетинга" };
            ItemsCatalogVm.CanDeleteItem = false;
            ItemsCatalog.DataContext = ItemsCatalogVm;
            MainClass.ShowUC(ItemsCatalog);
            /*
            var _model = new Models.CatalogModel<MarketingChannel>(
            new Models.EditCatalogDataFuncs<MarketingChannel>()
            {
                AddItemFunc = DBProvider.Client.CreateMarketingChannel,
                EditItemFunc = DBProvider.Client.UpdateMarketingChannel,
                CancelAddItemFunc = DBProvider.Client.DeleteMarketingChannel,
                AllDataList = DataExtension.DataCatalogsSingleton.Instance.MarketingChannels
            }
            );

            var vm = new Models.CatalogViewModel<MarketingChannel>(_model);
            vm.CanDeleteItem = false;
            ShowCatItem(vm, "Справочник каналов маркетинга");
            */
        }

        public static void ShowPaymentsCatalog()
        {
            var _model = new Models.CatalogModel<Payment>(DataCatalogsSingleton.Instance.PaymentData);
            var ItemsCatalog = new UI.CtrlCatalogView2();
            var ItemsCatalogVm = new Models.CatalogViewModel<Payment>(_model) { Header = "Справочник видов оплат" };
            ItemsCatalogVm.CanDeleteItem = false;
            ItemsCatalog.DataContext = ItemsCatalogVm;
            MainClass.ShowUC(ItemsCatalog);

            /*
            var _model = new Models.CatalogModel<Payment>(
            new Models.EditCatalogDataFuncs<Payment>()
            {
                AddItemFunc = DBProvider.Client.CreatePayment,
                EditItemFunc = DBProvider.Client.UpdatePayment,
                CancelAddItemFunc = DBProvider.Client.DeletePayment,
                AllDataList = DataExtension.DataCatalogsSingleton.Instance.Payments
            }
            );

            var vm = new Models.CatalogViewModel<Payment>(_model);
            vm.CanDeleteItem = false;
            ShowCatItem(vm, "Справочник видов оплат");
            */
        }


        public static void ShowPaymentGroupsCatalog()
        {

            var _model = new Models.CatalogModel<PaymentGroup>(DataCatalogsSingleton.Instance.PaymentGroupData);
            var ItemsCatalog = new UI.CtrlCatalogView2();
            var ItemsCatalogVm = new Models.CatalogViewModel<PaymentGroup>(_model) { Header = "Справочник групп оплат" };
            ItemsCatalogVm.CanDeleteItem = false;
            ItemsCatalog.DataContext = ItemsCatalogVm;
            MainClass.ShowUC(ItemsCatalog);

            /*
            var _model = new Models.CatalogModel<PaymentGroup>(
            new Models.EditCatalogDataFuncs<PaymentGroup>()
            {
                AddItemFunc = DBProvider.Client.CreatePaymentGroup,
                EditItemFunc = DBProvider.Client.UpdatePaymentGroup,
                CancelAddItemFunc = DBProvider.Client.DeletePaymentGroup,
                AllDataList = DataExtension.DataCatalogsSingleton.Instance.PaymentGroups
            }
            );

            var vm = new Models.CatalogViewModel<PaymentGroup>(_model);
            vm.CanDeleteItem = false;
            ShowCatItem(vm, "Справочник групп оплат");
            */
        }


        public static void ShowCatItem(dynamic vM, string name)
        {
            (vM as ViewModelPane).Header = name;
            var ItemsCatalog = new UI.CtrlCatalogView2() { };
            ItemsCatalog.DataContext = vM;
            MainClass.ShowUC(ItemsCatalog);
        }

        private static void ShowItemsCatalog(string name, FullyObservableCollection<Dish> data)
        {
            /*
            var GetDataFuncs = new Models.EditCatalogDataFuncs<Dish>()
            {
                AddItemFunc = DBProvider.Client.CreateDish,
                EditItemFunc = DBProvider.Client.UpdateDish,
                CancelAddItemFunc = DBProvider.Client.DeleteDish,
                AllDataList = data
            };
            */

            var _model = new Models.CatalogModel<Dish>(DataCatalogsSingleton.Instance.DishData, data);

            var ItemsCatalog = new UI.CtrlCatalogView2();
            var ItemsCatalogVm = new Models.CatalogViewModel<Dish>(_model) { Header = name };

            ItemsCatalogVm.CanDeleteItem = false;
            ItemsCatalog.DataContext = ItemsCatalogVm;


            MainClass.ShowUC(ItemsCatalog);

        }
        private static void ShowItemsCatalog()
        {

            ShowItemsCatalog("Справочник блюд всех", new FullyObservableCollection<Dish>(DataCatalogsSingleton.Instance.DishData.Data));


            /*
            var _model = new Models.CatalogModel<Dish>(DataCatalogsSingleton.Instance.DishData);
            var ItemsCatalog = new UI.CtrlCatalogView2();
            var ItemsCatalogVm = new Models.CatalogViewModel<Dish>(_model) { Header = "Справочник блюд всех" };
            ItemsCatalogVm.CanDeleteItem = false;
            ItemsCatalog.DataContext = ItemsCatalogVm;
            MainClass.ShowUC(ItemsCatalog);
            */
        }

        public static void ShowItemsCatalogToFly()
        {
            ShowItemsCatalog("Справочник блюд ToFly", new FullyObservableCollection<Dish>(DataCatalogsSingleton.Instance.DishFilter.AllDishesToFly));
        }

        public static void ShowItemsCatalogToGo()
        {
            ShowItemsCatalog("Справочник блюд ToGo", new FullyObservableCollection<Dish>(DataCatalogsSingleton.Instance.DishFilter.AllDishesToGo));
        }

        public static void ShowItemsCatalogTemp()
        {
            ShowItemsCatalog("Справочник блюд временных", new FullyObservableCollection<Dish>(DataCatalogsSingleton.Instance.DishFilter.OpenDishes));
        }


        public static void ShowDeliveryPlaceCatalog()
        {

            var _model = new Models.CatalogModel<DeliveryPlace>(DataCatalogsSingleton.Instance.DeliveryPlaceData);
            var ItemsCatalog = new UI.CtrlCatalogView2();
            var ItemsCatalogVm = new Models.CatalogViewModel<DeliveryPlace>(_model) { Header = "Справочник мест доставки" };
            ItemsCatalogVm.CanDeleteItem = false;
            ItemsCatalog.DataContext = ItemsCatalogVm;
            MainClass.ShowUC(ItemsCatalog);

            /*
            var _model = new Models.CatalogModel<DeliveryPlace>(
                new Models.EditCatalogDataFuncs<DeliveryPlace>()
                {
                    AddItemFunc = DBProvider.Client.CreateDeliveryPlace,
                    EditItemFunc = DBProvider.Client.UpdateDeliveryPlace,
                    CancelAddItemFunc = DBProvider.Client.DeleteDeliveryPlace,
                    //GetId = (Air) => { return Air.Id; },
                    AllDataList = DataExtension.DataCatalogsSingleton.Instance.DeliveryPlaces
                });
            var ItemsCatalog = new UI.CtrlCatalogView2();
            ItemsCatalog.DataContext = new Models.CatalogViewModel<DeliveryPlace>(_model) { Header = "Справочник мест доставки" };
            MainClass.ShowUC(ItemsCatalog);
            */
        }


        public static void ShowOrdersNonSH()
        {
            var ctrl = UI.UIModify.GetCtrlOrdersNonSH();
            MainClass.ShowUC(ctrl);
        }


        public static void ShowAirCompaneisOrdersToFly()
        {
            var ctrl = UI.UIModify.GetCtrlOrdersFlightByAirCompaneis();
            MainClass.ShowUC(ctrl);
        }
        public static void ShowOrdersToFly()
        {
            var ctrl = UI.UIModify.GetCtrlOrdersFlight();
            MainClass.ShowUC(ctrl);
        }
        public static void ShowOrdersToGo()
        {
            var ctrl = UI.UIModify.GetCtrlOrdersToGo();
            MainClass.ShowUC(ctrl);
        }
        public static void ShowAddOrdersToFly()
        {
            var ctrl = UI.UIModify.GetCtrlAddOrder();
            MainClass.ShowUC(ctrl);
        }

        public static void ShowAddOrdersToGo()
        {
            var ctrl = UI.UIModify.GetCtrlAddToGoOrder();
            MainClass.ShowUC(ctrl);
        }



        public static void ShowChangePass()
        {
            var wnd = new UI.WndChangePass();
            //wnd.ShowDialog();
            UI.UIModify.ShowDialogWnd(wnd);


        }

    }

    public class MenuItemCatalog : Models.MenuItem
    {
        public MenuItemCatalog(string name, Action showAction, long accessId, bool canDelete) : base(name)
        {
            AccessId = accessId;
            CanDelete = canDelete;
            IsVisible = Authorization.GetAccessType(AccessId) != Utils.FuncAccessTypeEnum.Disable;
            Command = new DelegateCommand((_) => { showAction(); });
        }


        public MenuItemCatalog(string name, Interface.IMenuItem menuItem, long accessId, bool canDelete) : base(name)
        {
            AccessId = accessId;
            CanDelete = canDelete;
            IsVisible = Authorization.GetAccessType(AccessId) != Utils.FuncAccessTypeEnum.Disable;
            Command = menuItem.MenuAction();
        }

        public long AccessId { set; get; }
        public bool CanDelete { set; get; }

        public bool IsVisible { set; get; }
    }

}
