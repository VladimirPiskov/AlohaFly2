using AlohaFly.Models;
using AlohaFly.Models.ToGoClient;
using AlohaFly.Utils;
using AlohaService.ServiceDataContracts;
using AutoMapper;
using DynamicData;
using NLog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace AlohaFly.DataExtension
{
    


    public sealed class DataCatalogsSingleton
    {
        Logger _logger = LogManager.GetCurrentClassLogger();
        private DataCatalogsSingleton()
        {
            InitMapers();
            /*
            Dishes.CollectionChanged += (sender, eventArgs) =>
             {
                 NotifyPropertyChanged("Dishes");
             };
             */
        }

        #region ToGoClients

        public FullyObservableDBData<User> ManagerData = new FullyObservableDBData<User>();
        public FullyObservableDBData<ContactPerson> ContactPersonData = new FullyObservableDBData<ContactPerson>();
        public FullyObservableDBData<DishLogicGroup> DishLogicGroupData = new FullyObservableDBData<DishLogicGroup>();
        public FullyObservableDBData<DishKitchenGroup> DishKitchenGroupData = new FullyObservableDBData<DishKitchenGroup>();
        public FullyObservableDBData<PaymentGroup> PaymentGroupData = new FullyObservableDBData<PaymentGroup>();
        public FullyObservableDBData<Payment> PaymentData = new FullyObservableDBData<Payment>();
        public FullyObservableDBData<Discount> DiscountData = new FullyObservableDBData<Discount>();
        public FullyObservableDBData<Driver> DriverData = new FullyObservableDBData<Driver>();
        public FullyObservableDBData<DeliveryPlace> DeliveryPlaceData = new FullyObservableDBData<DeliveryPlace>();
        public FullyObservableDBData<AirCompany> AirCompanyData = new FullyObservableDBData<AirCompany>();
        public FullyObservableDBData<Dish> DishData = new FullyObservableDBData<Dish>();
        public FullyObservableDBData<ItemLabelInfo> ItemLabelInfoData = new FullyObservableDBData<ItemLabelInfo>();
        public FullyObservableDBData<MarketingChannel> MarketingChannelData = new FullyObservableDBData<MarketingChannel>();
        public FullyObservableDBData<OrderCustomerAddress> OrderCustomerAddressData = new FullyObservableDBData<OrderCustomerAddress>();
        public FullyObservableDBData<OrderCustomerPhone> OrderCustomerPhoneData = new FullyObservableDBData<OrderCustomerPhone>();
        public FullyObservableDBData<OrderCustomer> OrderCustomerData = new FullyObservableDBData<OrderCustomer>();

        public FullyObservableDBData<OrderToGo> OrdersToGoData = new FullyObservableDBData<OrderToGo>();
        public FullyObservableDBData<OrderFlight> OrdersFlightData = new FullyObservableDBData<OrderFlight>();


        DateTime GetMonth(DateTime dt) { return new DateTime(dt.Year, dt.Month, 1); }

        void RefreshDynamicData()
        {

            StartDt = GetMonth(DateTime.Now.AddDays(-2));
            EndDt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            ManagerData.Fill(a => a.Id);
            ContactPersonData.Fill(a => a.Id);
            DishLogicGroupData.Fill(a => a.Id);
            DishKitchenGroupData.Fill(a => a.Id);
            PaymentGroupData.Fill(a => a.Id);
            PaymentData.Fill(a => a.Id);
            DiscountData.Fill(a => a.Id);
            DriverData.Fill(a => a.Id);
            DeliveryPlaceData.Fill(a => a.Id);
            AirCompanyData.Fill(a => a.Id);
            
            DishData.Fill(a => a.Id);
            ItemLabelInfoData.Fill(a => a.Id);
            MarketingChannelData.Fill(a => a.Id);
            OrderCustomerData.Fill(a => a.Id);
            OrderCustomerAddressData.Fill(a => a.Id);
            OrderCustomerPhoneData.Fill(a => a.Id);
            OrdersToGoData.Fill(a => a.Id, GetMonth(DateTime.Now.AddDays(-2)));
            OrdersFlightData.Fill(a => a.Id, GetMonth(DateTime.Now.AddDays(-2)));
            DishFilter = new DishFilter();
            PaymentFilter = new PaymentFilter();
            AirCompanyFilter =new AirCompanyFilter();


            Calc.CalkDiscounts(OrdersFlightData.Data.ToList());

        }

        public AirCompanyFilter AirCompanyFilter;
        public  DishFilter DishFilter;
        public PaymentFilter PaymentFilter;

        public DateTime StartDt { get; private set; }
        public DateTime EndDt { get; private set; }
        public void ChangeOrderDateRange(DateTime dt1, DateTime dt2)
        {
            Action<string> ev=null;
            TaskWithEvent task = new TaskWithEvent(() =>
            {

                StartDt = dt1;
                EndDt = dt2;

                int daysLimit = -15;
                DateTime dtLimit = OrdersFlightData.startDate.GetValueOrDefault().AddDays(daysLimit);



                for (DateTime dt = new DateTime(Math.Max(dt1.Ticks, dtLimit.Ticks)); dt >= dt1; dt = new DateTime(Math.Max(dt1.Ticks, dt.AddDays(daysLimit).Ticks)))
                {
                    OrdersToGoData.ChangeStartDate(dt);
                    
                    OrdersFlightData.ChangeStartDate(dt);
                    
                    if (dt == dt1) break;
                    if (ev != null)
                    {
                        ev("dt");
                    }
                }
                ToGoOrdersModelSingleton.Instance.UpdateDateRange(dt1, dt2);
                    AirOrdersModelSingleton.Instance.UpdateDateRange(dt1, dt2);
            }, ev);

            MainClass.DoWithBusy(task);
            RaiseChangeOrdersDateRangeEvent();
        }


            

        public delegate void ChangeOrdersDateRangeEventHandler(object sender, EventArgs e);

        public event ChangeOrdersDateRangeEventHandler ChangeOrdersDateRangeEvent;
        
        void RaiseChangeOrdersDateRangeEvent()
        {
            // Raise the event in a thread-safe manner using the ?. operator.
            ChangeOrdersDateRangeEvent?.Invoke(this, new EventArgs());
        }

        void InitMapers()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<OrderCustomer, ToGoClientViewModel>()
                //.ForMember(a => a.PhonesVM, m => m.MapFrom(src => src.Phones))
                .ForMember(a => a.AddressesVM, m => m.MapFrom(src => src.Addresses));
                cfg.CreateMap<ToGoClientViewModel, OrderCustomer>();
                cfg.CreateMap<ToGoClientViewModel, ToGoClientViewModel>();
                //.ForMember(a => a.PhonesVM, m => m.Ignore());
                cfg.CreateMap<OrderCustomerPhone, OrderCustomerPhone>();
                cfg.CreateMap<OrderCustomerAddress, OrderCustomerAddress>();
            });
        }
        
        #endregion

        static DataCatalogsSingleton instance;
        public static DataCatalogsSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataCatalogsSingleton();
                }
                return instance;
            }
        }

        public event EventHandler<string> DataCatalogMessage;
        void OnDataCatalogMessage(string msg)
        {
            try
            {
                DataCatalogMessage?.Invoke(this, msg);
            }
            catch (Exception e)
            {
                _logger.Debug($"Error {e.Message} ");
            }
        }
        public List<long> OpenDishezBarCodes = new List<long>() { 1697, 1698, 1718, 1719, 446, 447 };

        public List<long> OpenDishezToGoBarCodes = new List<long>() { 17 };


        /*
        FullyObservableCollection<Dish> dishes = new FullyObservableCollection<Dish>();
        public FullyObservableCollection<Dish> Dishes
        {
            get
            {
                return dishes;
            }
            set
            {
                dishes = value;
                NotifyPropertyChanged("Dishes");
            }
        }
        */

            /*
        public bool AddLabelInfo(long ParenDishId)
        {
            ItemLabelInfo l = new ItemLabelInfo() { ParenItemId = ParenDishId, SerialNumber = ItemLabelsInfo.Where(a => a.ParenItemId == ParenDishId).Count() + 1 };


            var res = DBDataExtractor<ItemLabelInfo>.AddItem(DBProvider.Client.CreateItemLabelInfo, l);
            if (res > 0)
            {
                l.Id = res;
                ItemLabelsInfo.Add(l);
                Dishes.SingleOrDefault(a => a.Id == ParenDishId).LabelsCount++;
                return true;
            }
            return false;
        }
        */
        /*
        public bool RemoveLabelInfo(ItemLabelInfo l)
        {
            var res = DBDataExtractor<ItemLabelInfo>.DeleteItem(DBProvider.Client.DeleteItemLabelInfo, l.Id);
            if (res)
            {
                ItemLabelsInfo.Remove(l);
                Dishes.SingleOrDefault(a => a.Id == l.ParenItemId).LabelsCount--;
                return true;
            }
            return false;
        }

        public bool UpdateLabelInfo(ItemLabelInfo l)
        {
            return DBDataExtractor<ItemLabelInfo>.EditItem(DBProvider.Client.UpdateItemLabelInfo, l);
        }

            */
            /*
        FullyObservableCollection<ItemLabelInfo> itemLabelInfo = new FullyObservableCollection<ItemLabelInfo>();
        public FullyObservableCollection<ItemLabelInfo> ItemLabelsInfo
        {
            get
            {
                return itemLabelInfo;
            }
            set
            {
                itemLabelInfo = value;
                NotifyPropertyChanged("ItemLabelsInfo");
            }
        }
        */

            /*

        public FullyObservableCollection<Dish> ActiveDishesAll
        {
            get
            {
                return new FullyObservableCollection<Dish>(DishData.Where(a => a.IsActive & !a.IsTemporary));
            }

        }
        public FullyObservableCollection<Dish> ActiveDishesToGo
        {
            get
            {
                return new FullyObservableCollection<Dish>(dishes.Where(a => a.IsActive & !a.IsTemporary && a.IsToGo));
            }

        }

        public FullyObservableCollection<Dish> ActiveDishesToFly
        {
            get
            {
                return new FullyObservableCollection<Dish>(dishes.Where(a => a.IsActive & !a.IsTemporary && !a.IsToGo));
            }

        }
        */

            /*
        FullyObservableCollection<AirCompany> airCompanies;
        public FullyObservableCollection<AirCompany> AirCompanies
        {
            get
            {
                return airCompanies;
            }
            set
            {
                airCompanies = value;
                NotifyPropertyChanged("AirCompanies");
            }

        }

        FullyObservableCollection<AirCompany> allAirCompanies;
        public FullyObservableCollection<AirCompany> AllAirCompanies
        {
            get
            {
                return allAirCompanies;
            }
            set
            {
                allAirCompanies = value;
                NotifyPropertyChanged("AllAirCompanies");
            }

        }
        */
        /*
        FullyObservableCollection<Discount> mdiscounts;
        public FullyObservableCollection<Discount> mDiscounts
        {
            get
            {
                return mdiscounts;
            }
            set
            {
                mdiscounts = value;
                NotifyPropertyChanged("mDiscount");

            }

        }
        */
        /*
        FullyObservableCollection<DeliveryPlace> deliveryPlaces = new FullyObservableCollection<DeliveryPlace>();
        public FullyObservableCollection<DeliveryPlace> DeliveryPlaces
        {
            get
            {
                return deliveryPlaces;
            }
            set
            {
                deliveryPlaces = value;
            }

        }


        FullyObservableCollection<MarketingChannel> marketingChannels;
        public FullyObservableCollection<MarketingChannel> MarketingChannels
        {
            get
            {
                return marketingChannels;
            }
            set
            {
                marketingChannels = value;
            }

        }

        FullyObservableCollection<Driver> drivers = new FullyObservableCollection<Driver>();
        public FullyObservableCollection<Driver> Drivers
        {
            get
            {
                return drivers;
            }
            set
            {
                drivers = value;
            }

        }

            */

        /*

        public bool AddOpenDish(Dish d)
        {
            bool res = DBProvider.AddDish(d);
            if (res)
            {
                Dishes.Add(d);
                //NotifyPropertyChanged("GetOpenDishes");
            }
            return res;
        }
        */
        /*
        public FullyObservableCollection<Dish> GetOpenDishes(long OwnerBc)
        {
            return new FullyObservableCollection<Dish>(dishes.Where(a => a.IsActive && a.IsTemporary && a.Barcode == OwnerBc));
        }

        public FullyObservableCollection<Dish> GetOpenDishes()
        {
            return new FullyObservableCollection<Dish>(dishes.Where(a => a.IsActive && a.IsTemporary));
        }
        */


            /*

        public event PropertyChangedEventHandler PropertyChanged;
        void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool AddContactPerson(ContactPerson CP)
        {
            var res = DBDataExtractor<ContactPerson>.AddItem(DBProvider.Client.CreateContactPerson, CP);
            if (res > 0)
            {
                CP.Id = res;
                ContactPerson.Add(CP);

            }
            return res > 0;

        }

        public SourceCache<Payment, long> PaymentsSourceCache = new SourceCache<Payment, long>(t => t.Id);
        public Payment GetPayment(long id)
        {
            if (DataCatalogsSingleton.Instance.PaymentsSourceCache.Lookup(id).HasValue)
            {
                return DataCatalogsSingleton.Instance.PaymentsSourceCache.Lookup(id).Value;
            }
            else
            {
                return null;
            }
        }


        public FullyObservableCollection<Payment> Payments { set; get; }
        public FullyObservableCollection<PaymentGroup> PaymentGroups { set; get; }




        //public FullyObservableCollection<Discount> Discounts { set; get; }

        private FullyObservableCollection<ContactPerson> contactPerson;
        public FullyObservableCollection<ContactPerson> ContactPerson
        {
            set
            {
                contactPerson = value;
            }
            get
            {
                return contactPerson;
            }

        }


*/



        /*
        public FullyObservableCollection<DishLogicGroup> DishLogicGroup { set; get; }
        public FullyObservableCollection<DishKitchenGroup> DishKitchenGroup { set; get; }

            */

        /*   
        FullyObservableCollection<User> managerOperator;
        public FullyObservableCollection<User> ManagerOperator
        {
            get
            {
                return managerOperator;
            }

        }
        */

        public dynamic GetBindingIdCollectionName(string propInfo)
        {
            if (propInfo == "PaymentType")
            {
                return "PaymentId";
            }
            if (propInfo == "DiscountType")
            {
                return "DiscountId";
            }

            return propInfo + "Id";
        }
        
        public dynamic GetCatalogData(Type T)
        {
            if (T == typeof(DishLogicGroup))
            {
                return DataCatalogsSingleton.Instance.DishLogicGroupData.Data;
            }

            if (T == typeof(DishKitchenGroup))
            {
                return DataCatalogsSingleton.Instance.DishKitchenGroupData.Data;
            }

            if (T == typeof(Payment))
            {
                return DataCatalogsSingleton.Instance.PaymentData.Data;
            }

            if (T == typeof(Discount))
            {
                return DataCatalogsSingleton.Instance.DiscountData.Data;
            }
            return null;
        }
        


        public void DataCatalogsFill()
        {
            try
            {
                _logger.Debug("DataCatalogsFill start");
                
                 //OperationResultValue<UpdateResult> res = DBProvider.Client.GetUpdatesForSession("");
                //var res2 = res.Result.UpdatedData[0] as OrderToGo;
                
                OnDataCatalogMessage("Загружаю список операторов");
                //managerOperator = DBDataExtractor<User>.GetDataList(DBProvider.Client.GetUserList) == null ? new FullyObservableCollection<User>() : new FullyObservableCollection<User>(DBDataExtractor<User>.GetDataList(DBProvider.Client.GetUserList));
               // managerOperator = DBDataExtractor<User>.GetDataList(DBProvider.Client.GetUserList);

                OnDataCatalogMessage("Загружаю контакты");
                //ContactPerson = DBDataExtractor<ContactPerson>.GetDataList(DBProvider.Client.GetContactPersonList);
                /*
                ToGoCustomers = DBDataExtractor<OrderCustomer>.GetDataList(DBProvider.Client.GetOrderCustomerList);
                ToGoCustomersAddresses = new FullyObservableCollection<OrderCustomerAddress>();
                foreach (var cust in ToGoCustomers)
                {
                    foreach (var addr in cust.Addresses)
                    {
                        ToGoCustomersAddresses.Add(addr);
                    }
                }
                */
                OnDataCatalogMessage("Загружаю наклейки");
                /*
                ItemLabelsInfo = DBDataExtractor<ItemLabelInfo>.GetDataList(DBProvider.Client.GetItemLabelInfoList);
                DishLogicGroup = DBDataExtractor<DishLogicGroup>.GetDataList(DBProvider.Client.GetDishLogicGroupsList);
                DishKitchenGroup = DBDataExtractor<DishKitchenGroup>.GetDataList(DBProvider.Client.GetDishKitсhenGroupsList);
                */

                OnDataCatalogMessage("Загружаю авиакомпании");
               // PaymentGroups = DBDataExtractor<PaymentGroup>.GetDataList(DBProvider.Client.GetPaymentGroupList);
              //  Payments = DBDataExtractor<Payment>.GetDataList(DBProvider.Client.GetPaymentList);
              /*
                foreach (var a in Payments)
                {
                    if (a.PaymentGroupId != 0)
                    {
                        a.PaymentGroup = PaymentGroups.SingleOrDefault(b => b.Id == a.PaymentGroupId);
                    }

                    PaymentsSourceCache.AddOrUpdate(a);
                }
                */
              //  Discounts = DBDataExtractor<Discount>.GetDataList(DBProvider.Client.GetDiscountList);

                /*
                AllAirCompanies = new FullyObservableCollection<AirCompany>(DBDataExtractor<AirCompany>
                    .GetDataList(DBProvider.Client.GetAirCompanyList)
                    //.Where(a => !DBProvider.SharAirs.Contains(a.Id) || ((Authorization.CurentUser != null) && ((Authorization.CurentUser.UserName == "sh.user") || (Authorization.IsDirector))))
                    .OrderBy(a => a.Name));



                foreach (var a in AllAirCompanies)
                {
                    if (a.PaymentId != null)
                    {
                        a.PaymentType = Payments.SingleOrDefault(b => b.Id == a.PaymentId);
                    }
                    if (a.DiscountId != null)
                    {
                        a.DiscountType = Discounts.SingleOrDefault(b => b.Id == a.DiscountId);
                    }
                }
                AirCompanies = new FullyObservableCollection<AirCompany>(AllAirCompanies.Where(a => a.IsActive && (!DBProvider.SharAirs.Contains(a.Id) || ((Authorization.CurentUser != null) && ((Authorization.CurentUser.UserName == "sh.user") || (Authorization.IsDirector))))));
                //AirCompanies = DBDataExtractor<AirCompany>.GetDataList(DBProvider.Client.GetAirCompanyList);
                //DeliveryPlaces = DBDataExtractor<DeliveryPlace>.GetDataList(DBProvider.Client.GetDeliveryPlaceList) == null ? new FullyObservableCollection<DeliveryPlace>() : new FullyObservableCollection<DeliveryPlace>(DBDataExtractor<DeliveryPlace>.GetDataList(DBProvider.Client.GetDeliveryPlaceList));
                */
                /*
                DeliveryPlaces = DBDataExtractor<DeliveryPlace>.GetDataList(DBProvider.Client.GetDeliveryPlaceList);

                Drivers = DBDataExtractor<Driver>.GetDataList(DBProvider.Client.GetDriverList) == null ? new FullyObservableCollection<Driver>() : new FullyObservableCollection<Driver>(DBDataExtractor<Driver>.GetDataList(DBProvider.Client.GetDriverList));
                mDiscounts = DBDataExtractor<Discount>.GetDataList(DBProvider.Client.GetDiscountList) == null ? new FullyObservableCollection<Discount>() : new FullyObservableCollection<Discount>(DBDataExtractor<Discount>.GetDataList(DBProvider.Client.GetDiscountList));
                */
                OnDataCatalogMessage("Загружаю блюда");
                /*
                Dishes = DBDataExtractor<Dish>.GetDataList(DBProvider.Client.GetDishList);
                foreach (var d in Dishes)
                {
                    if (d.DishKitсhenGroupId > 0)
                    { try { d.DishKitсhenGroup = DishKitchenGroup.Single(a => a.Id == d.DishKitсhenGroupId); } catch { } }
                    if (d.DishLogicGroupId > 0)
                    { try { d.DishLogicGroup = DishLogicGroup.Single(a => a.Id == d.DishLogicGroupId); } catch { } }

                }
                */
                OnDataCatalogMessage("Загружаю cписок клиентов");

//                marketingChannels = DBDataExtractor<MarketingChannel>.GetDataList(DBProvider.Client.GetMarketingChannelList);


                //ItemLabelsInfo.ItemPropertyChanged += ItemLabelsInfo_ItemPropertyChanged;

                RefreshDynamicData();

                _logger.Debug("DataCatalogsFill end");
            }
            catch (Exception e)
            {
                _logger.Debug($"DataCatalogsFill error {e.Message}");
                MainClass.SetNeedExit("Ошибка загрузки справочников " + e.Message);
            }

            /*
            foreach (var d in DBDataExtractor<Dish>.GetDataList(DBProvider.Client.GetDishList) == null ? new FullyObservableCollection<Dish>() : new FullyObservableCollection<Dish>(DBDataExtractor<Dish>.GetDataList(DBProvider.Client.GetDishList)))
            {
                dishes.Add(d);
            }
            */


        }
        /*
        private void ItemLabelsInfo_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            UpdateLabelInfo(ItemLabelsInfo[e.CollectionIndex]);
        }
        */
        /*
        private void FillAllDataAsync()
        {

            Task fillData = new Task(() => DataCatalogsFill());
            fillData.Start();
        }
        */
    }

    /*
    public class DataCatalogHelper
    {
        public DataCatalogHelper()
        { }
        public dynamic GetCatalogData(Type T)
        {
            if (T == typeof(DishLogicGroup))
            {
                return DataCatalogsSingleton.Instance.DishLogicGroup ;
            }

            if (T == typeof(DishKitchenGroup))
            {
                return DataCatalogsSingleton.Instance.DishKitchenGroup ;
            }

            return new FullyObservableCollection<T>;
        }
    }

    public class DataCatalogHelper<T>
        where T : INotifyPropertyChanged
    {
        public DataCatalogHelper()
            {}
        public FullyObservableCollection<T> GetCatalogData()
        {
            if (typeof(T) == typeof(DishLogicGroup))
            {
                return DataCatalogsSingleton.Instance.DishLogicGroup as FullyObservableCollection<T>;
            }

            if (typeof(T) == typeof(DishKitchenGroup))
            {
                return DataCatalogsSingleton.Instance.DishKitchenGroup as FullyObservableCollection<T>;
            }

            return new FullyObservableCollection<T>;
        }
    }
    */

    public class AirCompanyFilter
    {
        [Reactive] public FullyObservableCollection<AirCompany> CurentComps { set; get; }
        FullyObservableDBDataSubsriber<AirCompany, AirCompany> curentCompsConnector = new FullyObservableDBDataSubsriber<AirCompany, AirCompany>(a => a.Id);

        public AirCompanyFilter()
        {
            CurentComps = new FullyObservableCollection<AirCompany>();
            curentCompsConnector.Select(a => a.IsActive && (!DBProvider.SharAirs.Contains(a.Id) || ((Authorization.CurentUser != null) && ((Authorization.CurentUser.UserName == "sh.user") || (Authorization.IsDirector)))))
                            .OrderBy(a => a.Name)
                            .Subsribe(DataCatalogsSingleton.Instance.AirCompanyData, CurentComps);
        }
    }


        public class OpenDishFactory
    {
        [Reactive] public FullyObservableCollection<Dish> OpenDishes { set; get; }
        FullyObservableDBDataSubsriber<Dish, Dish> openDishesConnector = new FullyObservableDBDataSubsriber<Dish, Dish>(a => a.Id);
        public OpenDishFactory(long ownerBarcode)
        {
            OwnerBarcode = ownerBarcode;
            OpenDishes = new FullyObservableCollection<Dish>();
            openDishesConnector.Select(a => a.IsActive && a.IsTemporary &&a.Barcode == OwnerBarcode)
                            .OrderBy(a => a.Barcode)
                            .Subsribe(DataCatalogsSingleton.Instance.DishData, OpenDishes);
        }
        public long OwnerBarcode; 
    }


    public class PaymentFilter
    {
        [Reactive] public FullyObservableCollection<Payment> SpisPaymnets { set; get; }
        FullyObservableDBDataSubsriber<Payment, Payment> spConnector = new FullyObservableDBDataSubsriber<Payment, Payment>(a => a.Id);

        [Reactive] public FullyObservableCollection<Payment> ToGoSpisPaymnets { set; get; }
        FullyObservableDBDataSubsriber<Payment, Payment> togospConnector = new FullyObservableDBDataSubsriber<Payment, Payment>(a => a.Id);

        public PaymentFilter()
        {
            SpisPaymnets = new FullyObservableCollection<Payment>();
            spConnector.Select(x => x.IsActive && x.PaymentGroup != null && !x.PaymentGroup.Sale && (!x.ToGo))
            .Subsribe(DataCatalogsSingleton.Instance.PaymentData, SpisPaymnets);

            ToGoSpisPaymnets = new FullyObservableCollection<Payment>();
            togospConnector.Select(x => x.IsActive && x.PaymentGroup != null && !x.PaymentGroup.Sale && (x.ToGo))
            .Subsribe(DataCatalogsSingleton.Instance.PaymentData, ToGoSpisPaymnets);
        }

    }


        public class DishFilter
    {
        [Reactive] public FullyObservableCollection<Dish> OpenDishes { set; get; }
        FullyObservableDBDataSubsriber<Dish, Dish> openDishesConnector = new FullyObservableDBDataSubsriber<Dish, Dish>(a => a.Id);

        [Reactive] public FullyObservableCollection<Dish> ActiveDishesAll { set; get; }
        FullyObservableDBDataSubsriber<Dish, Dish> activeDishesConnector = new FullyObservableDBDataSubsriber<Dish, Dish>(a => a.Id);
        [Reactive] public FullyObservableCollection<Dish> ActiveDishesToGo { set; get; }
        FullyObservableDBDataSubsriber<Dish, Dish> activeDishesToGoConnector = new FullyObservableDBDataSubsriber<Dish, Dish>(a => a.Id);
        [Reactive] public FullyObservableCollection<Dish> ActiveDishesToFly { set; get; }
        FullyObservableDBDataSubsriber<Dish, Dish> activeDishesToFlyConnector = new FullyObservableDBDataSubsriber<Dish, Dish>(a => a.Id);

        [Reactive] public FullyObservableCollection<Dish> AllDishesToGo { set; get; }
        FullyObservableDBDataSubsriber<Dish, Dish> allDishesToGoConnector = new FullyObservableDBDataSubsriber<Dish, Dish>(a => a.Id);
        [Reactive] public FullyObservableCollection<Dish> AllDishesToFly { set; get; }
        FullyObservableDBDataSubsriber<Dish, Dish> allDishesToFlyConnector = new FullyObservableDBDataSubsriber<Dish, Dish>(a => a.Id);



        public DishFilter()
        {
            OpenDishes = new FullyObservableCollection<Dish>();
            openDishesConnector.Select(a => a.IsActive && a.IsTemporary)
                            .OrderBy(a => a.Barcode)
                            .Subsribe(DataCatalogsSingleton.Instance.DishData, OpenDishes);

            ActiveDishesAll = new FullyObservableCollection<Dish>();
            activeDishesConnector.Select(a => a.IsActive && !a.IsTemporary)
                            .OrderBy(a => a.Barcode)
                            .Subsribe(DataCatalogsSingleton.Instance.DishData, ActiveDishesAll);
            
            ActiveDishesToGo = new FullyObservableCollection<Dish>();
            activeDishesToGoConnector.Select(a => a.IsActive && !a.IsTemporary && a.IsToGo)
                            .OrderBy(a => a.Barcode)
                            .Subsribe(DataCatalogsSingleton.Instance.DishData, ActiveDishesToGo);

            ActiveDishesToFly = new FullyObservableCollection<Dish>();
            activeDishesToFlyConnector.Select(a => a.IsActive && !a.IsTemporary && !a.IsToGo)
                            .OrderBy(a => a.Barcode)
                            .Subsribe(DataCatalogsSingleton.Instance.DishData, ActiveDishesToFly);
            
            AllDishesToGo = new FullyObservableCollection<Dish>();
            allDishesToGoConnector.Select(a => a.IsToGo && !a.IsTemporary)
                            .OrderBy(a => a.Barcode)
                            .Subsribe(DataCatalogsSingleton.Instance.DishData, AllDishesToGo);

            AllDishesToFly = new FullyObservableCollection<Dish>();
            allDishesToFlyConnector.Select(a =>!a.IsToGo && !a.IsTemporary)
                            .OrderBy(a => a.Barcode)
                            .Subsribe(DataCatalogsSingleton.Instance.DishData, AllDishesToFly);


        }

        private List<OpenDishFactory> OpenDishFactories = new List<OpenDishFactory>();

        public OpenDishFactory GetOpenDishes(long ownerBarCode)
        {
            if (!OpenDishFactories.Any(a => a.OwnerBarcode == ownerBarCode))
            {
                OpenDishFactories.Add(new OpenDishFactory(ownerBarCode));
            }

            return OpenDishFactories.FirstOrDefault(a => a.OwnerBarcode == ownerBarCode);
        }


    }

        public interface IAlertsShower
    {
        void InitAlerts(List<Alert> alerts);
    }

}
