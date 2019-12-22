using AlohaFly.Models;
using AlohaFly.Models.ToGoClient;
using AlohaFly.Utils;
using AlohaService.ServiceDataContracts;
using AutoMapper;
using DynamicData;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace AlohaFly.DataExtension
{
    /*
        public class DateTimeTypeConverter : ITypeConverter<List<OrderCustomerPhone>, ToGoClientPhonesViewModel>
        {
            public ToGoClientPhonesViewModel Convert(List<OrderCustomerPhone> source, ToGoClientPhonesViewModel destination, ResolutionContext context)
            {
                SourceCache<OrderCustomer, long> phonesSource = new SourceCache<OrderCustomer, long>(cust => cust.Id);
                source

                return System.Convert.ToDateTime(source);
            }
        }
        */



    public sealed class DataCatalogsSingleton : INotifyPropertyChanged
    {
        Logger _logger = LogManager.GetCurrentClassLogger();
        /*
        private static readonly Lazy<DataCatalogsSingleton> instanceHolder =
            new Lazy<DataCatalogsSingleton>(() => new DataCatalogsSingleton());
            */
        private DataCatalogsSingleton()
        {
            // DataCatalogsFill();
            InitMapers();
            Dishes.CollectionChanged += (sender, eventArgs) =>
             {
                 NotifyPropertyChanged("Dishes");
             };


            //BindCollections();
        }

        #region ToGoClients

        
        public FullyObservableDBData<OrderCustomerAddress> OrderCustomerAddressData = new FullyObservableDBData<OrderCustomerAddress>();
        public FullyObservableDBData<OrderCustomerPhone> OrderCustomerPhoneData = new FullyObservableDBData<OrderCustomerPhone>();
        public FullyObservableDBData<OrderCustomer> OrderCustomerData = new FullyObservableDBData<OrderCustomer>();

        public FullyObservableDBData<OrderToGo> OrdersToGoData = new FullyObservableDBData<OrderToGo>();

        DateTime GetMonth(DateTime dt) { return new DateTime(dt.Year, dt.Month, 1); }

        void RefreshDynamicData()
        {
           // RealTimeUpdaterSingleton.Instance.Init(DateTime.Now.AddMonths(-2));
            OrderCustomerData.Fill(a => a.Id);
            OrderCustomerAddressData.Fill(a => a.Id);
            OrderCustomerPhoneData.Fill(a => a.Id);
            OrdersToGoData.Fill(a => a.Id, GetMonth(DateTime.Now.AddDays(-2)));
            //RealTimeUpdaterSingleton.Instance.StartQueue();
        }


        public void ChangeOrderDateRange(DateTime dt1, DateTime dt2)
        {
            
            OrdersToGoData.ChangeStartDate(dt1);
            ToGoOrdersModelSingleton.Instance.UpdateDateRange(dt1, dt2);

        }


        /*
        void InitDynamicData()
        {
            FullyObservableDBData<OrderCustomer>.Instance.Fill(a => a.Id);

        }
        */
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
        /*
        public void BindCollections()
        {
            Mapper.Initialize(cfg => {
                cfg.CreateMap<List<OrderCustomerPhone>, ToGoClientPhonesViewModel>().;
                cfg.CreateMap<OrderCustomer, ToGoClientViewModel>()
                .ForMember(a => a.PhonesVM, m => m.MapFrom(src => new ToGoClientViewModel(

                       new Func<List<OrderCustomerPhone>, ReadOnlyObservableCollection<ToGoClientPhoneViewModel>>(a => { return new ReadOnlyObservableCollection<ToGoClientPhoneViewModel>()});

                }
                    
                    
                    ));

                cfg.CreateMap<OrderCustomerAddress, ToGoClientAddressViewModel>();
            });
            var cancellation = toGoClientsSource
                .Connect()
                .Transform(orderCustomer => Mapper.Map<ToGoClientViewModel>(orderCustomer))
                .Sort(SortExpressionComparer<ToGoClientViewModel>.Descending(toGoClientViewModel => toGoClientViewModel.Name))
                .ObserveOnDispatcher()
                .Bind(out ToGoClients)
                .DisposeMany()
                .Subscribe();
        }

       

            */

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
        public Models.CatalogModel<ContactPerson> ContactPersonCatalogModel = new Models.CatalogModel<ContactPerson>(
            new Models.EditCatalogDataFuncs<ContactPerson>()
            {
                AddItemFunc = DBProvider.Client.CreateContactPerson,
                EditItemFunc = DBProvider.Client.UpdateContactPerson,
                CancelAddItemFunc = DBProvider.Client.DeleteContactPerson,
                GetAllDataFunc = DBProvider.Client.GetContactPersonList,
                //AllDataList = Instance.ContactPerson
            }
            );


            */

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




        public FullyObservableCollection<Dish> ActiveDishesAll
        {
            get
            {
                return new FullyObservableCollection<Dish>(dishes.Where(a => a.IsActive & !a.IsTemporary));
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

        /*
        FullyObservableCollection<DeliveryPerson> deliveryPerson;
        public FullyObservableCollection<DeliveryPerson> DeliveryPerson
        {
            get
            {
                return deliveryPerson;
            }

        }
        */

        public bool AddOpenDish(Dish d)
        {
            /*
            var od = new Dish()
            {
                Barcode = barcode,
                Name = Name,
                RussianName = Name,
                EnglishName = EnglishName,
                IsTemporary = true,
                PriceForFlight = Price,
                IsActive = true 
            };
            */
            bool res = DBProvider.AddDish(d);
            if (res)
            {
                Dishes.Add(d);
                //NotifyPropertyChanged("GetOpenDishes");
            }
            return res;
        }


        public FullyObservableCollection<Dish> GetOpenDishes(long OwnerBc)
        {
            return new FullyObservableCollection<Dish>(dishes.Where(a => a.IsActive && a.IsTemporary && a.Barcode == OwnerBc));
        }

        public FullyObservableCollection<Dish> GetOpenDishes()
        {
            return new FullyObservableCollection<Dish>(dishes.Where(a => a.IsActive && a.IsTemporary));
        }

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


        public FullyObservableCollection<Discount> Discounts { set; get; }

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

        /*
        public bool AddOrUpdateToGoCustomer(OrderCustomer customer)
        {
            if (customer.Id == 0)
            {
                var res = DBDataExtractor<OrderCustomer>.AddItem(DBProvider.Client.CreateOrderCustomer, customer);
                if (res != -1)
                {
                    customer.Id = res;
                    ToGoCustomers.Add(customer);
                    if (customer.Addresses != null)
                    {
                        foreach (var addr in customer.Addresses.Where(a => a.NeedUpdate || a.Id == 0))
                        {
                            if (addr.Id == 0)
                            {
                                addr.OrderCustomerId = customer.Id;
                                var id = DBDataExtractor<OrderCustomerAddress>.AddItem(DBProvider.Client.CreateOrderCustomerAddress, addr);

                            }
                            else
                            {
                                DBDataExtractor<OrderCustomerAddress>.EditItem(DBProvider.Client.UpdateOrderCustomerAddress, addr);
                            }
                        }


                    }

                    if (customer.Phones != null)
                    {
                        foreach (var addr in customer.Phones.Where(a => a.NeedUpdate || a.Id == 0))
                        {
                            if (addr.Id == 0)
                            {
                                addr.OrderCustomerId = customer.Id;
                                var id = DBDataExtractor<OrderCustomerPhone>.AddItem(DBProvider.Client.CreateOrderCustomerPhone, addr);

                            }
                            else
                            {
                                DBDataExtractor<OrderCustomerPhone>.EditItem(DBProvider.Client.UpdateOrderCustomerPhone, addr);
                            }
                        }


                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                foreach (var addr in customer.Addresses.Where(a => a.NeedUpdate || a.Id == 0))
                {
                    if (addr.Id == 0)
                    {
                        addr.OrderCustomerId = customer.Id;
                        var id = DBDataExtractor<OrderCustomerAddress>.AddItem(DBProvider.Client.CreateOrderCustomerAddress, addr);

                        addr.Id = id;
                    }
                    else
                    {
                        DBDataExtractor<OrderCustomerAddress>.EditItem(DBProvider.Client.UpdateOrderCustomerAddress, addr);
                    }
                }
                foreach (var addr in customer.Phones.Where(a => a.NeedUpdate || a.Id == 0))
                {
                    if (addr.Id == 0)
                    {
                        addr.OrderCustomerId = customer.Id;
                        var id = DBDataExtractor<OrderCustomerPhone>.AddItem(DBProvider.Client.CreateOrderCustomerPhone, addr);

                        addr.Id = id;
                    }
                    else
                    {
                        DBDataExtractor<OrderCustomerPhone>.EditItem(DBProvider.Client.UpdateOrderCustomerPhone, addr);
                    }
                }



                return DBDataExtractor<OrderCustomer>.EditItem(DBProvider.Client.UpdateOrderCustomer, customer);
            }
        }

        public long AddToGoCustomerAddress(OrderCustomer orderCustomer, OrderCustomerAddress addr)
        {


            addr.OrderCustomerId = orderCustomer.Id;
            var id = DBDataExtractor<OrderCustomerAddress>.AddItem(DBProvider.Client.CreateOrderCustomerAddress, addr);
            if (id > 0)
            {
                addr.Id = id;

                orderCustomer.AddAddress(addr);
                foreach (var adr in orderCustomer.Addresses.Where(a => a.NeedUpdate && a.Id != id))
                {
                    DBDataExtractor<OrderCustomerAddress>.EditItem(DBProvider.Client.UpdateOrderCustomerAddress, adr);
                }

                ToGoCustomersAddresses.Add(addr);

            }

            return id;
        }

        public FullyObservableCollection<OrderCustomerAddress> ToGoCustomersAddresses { set; get; }


        public FullyObservableCollection<OrderCustomerPhone> ToGoCustomersPhones { set; get; }

        private FullyObservableCollection<OrderCustomer> toGoCustomers;
        public FullyObservableCollection<OrderCustomer> ToGoCustomers

        {
            set
            {
                toGoCustomers = value;
            }
            get
            {
                return toGoCustomers;
            }

        }

        /*
        private FullyObservableCollection<OrderCustomer> toGoCustomersAddresses;
        public FullyObservableCollection<OrderCustomer> ToGoCustomersAddresses
        {
            set
            {
                toGoCustomersAddresses = value;
            }
            get
            {
                return toGoCustomersAddresses;
            }

        }
        */






        public FullyObservableCollection<DishLogicGroup> DishLogicGroup { set; get; }
        public FullyObservableCollection<DishKitchenGroup> DishKitchenGroup { set; get; }




        FullyObservableCollection<User> managerOperator;
        public FullyObservableCollection<User> ManagerOperator
        {
            get
            {
                return managerOperator;
            }

        }
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
                return DataCatalogsSingleton.Instance.DishLogicGroup;
            }

            if (T == typeof(DishKitchenGroup))
            {
                return DataCatalogsSingleton.Instance.DishKitchenGroup;
            }

            if (T == typeof(Payment))
            {
                return DataCatalogsSingleton.Instance.Payments;
            }

            if (T == typeof(Discount))
            {
                return DataCatalogsSingleton.Instance.Discounts;
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
                managerOperator = DBDataExtractor<User>.GetDataList(DBProvider.Client.GetUserList);

                OnDataCatalogMessage("Загружаю контакты");
                ContactPerson = DBDataExtractor<ContactPerson>.GetDataList(DBProvider.Client.GetContactPersonList);
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
                //ItemLabelsInfo = DBDataExtractor<ItemLabelInfo>.GetDataList(DBProvider.Client.GetItemLabelInfoList) == null ? new FullyObservableCollection<ItemLabelInfo>() : new FullyObservableCollection<ItemLabelInfo>(DBDataExtractor<ItemLabelInfo>.GetDataList(DBProvider.Client.GetItemLabelInfoList));
                ItemLabelsInfo = DBDataExtractor<ItemLabelInfo>.GetDataList(DBProvider.Client.GetItemLabelInfoList);
                DishLogicGroup = DBDataExtractor<DishLogicGroup>.GetDataList(DBProvider.Client.GetDishLogicGroupsList);
                DishKitchenGroup = DBDataExtractor<DishKitchenGroup>.GetDataList(DBProvider.Client.GetDishKitсhenGroupsList);

                OnDataCatalogMessage("Загружаю авиакомпании");
                PaymentGroups = DBDataExtractor<PaymentGroup>.GetDataList(DBProvider.Client.GetPaymentGroupList);
                Payments = DBDataExtractor<Payment>.GetDataList(DBProvider.Client.GetPaymentList);
                foreach (var a in Payments)
                {
                    if (a.PaymentGroupId != 0)
                    {
                        a.PaymentGroup = PaymentGroups.SingleOrDefault(b => b.Id == a.PaymentGroupId);
                    }

                    PaymentsSourceCache.AddOrUpdate(a);
                }
                Discounts = DBDataExtractor<Discount>.GetDataList(DBProvider.Client.GetDiscountList);
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

                DeliveryPlaces = DBDataExtractor<DeliveryPlace>.GetDataList(DBProvider.Client.GetDeliveryPlaceList);

                Drivers = DBDataExtractor<Driver>.GetDataList(DBProvider.Client.GetDriverList) == null ? new FullyObservableCollection<Driver>() : new FullyObservableCollection<Driver>(DBDataExtractor<Driver>.GetDataList(DBProvider.Client.GetDriverList));
                mDiscounts = DBDataExtractor<Discount>.GetDataList(DBProvider.Client.GetDiscountList) == null ? new FullyObservableCollection<Discount>() : new FullyObservableCollection<Discount>(DBDataExtractor<Discount>.GetDataList(DBProvider.Client.GetDiscountList));

                OnDataCatalogMessage("Загружаю блюда");
                Dishes = DBDataExtractor<Dish>.GetDataList(DBProvider.Client.GetDishList);
                foreach (var d in Dishes)
                {
                    if (d.DishKitсhenGroupId > 0)
                    { try { d.DishKitсhenGroup = DishKitchenGroup.Single(a => a.Id == d.DishKitсhenGroupId); } catch { } }
                    if (d.DishLogicGroupId > 0)
                    { try { d.DishLogicGroup = DishLogicGroup.Single(a => a.Id == d.DishLogicGroupId); } catch { } }
                    d.LabelsCount = ItemLabelsInfo.Where(a => a.ParenItemId == d.Id).Count();
                }

                OnDataCatalogMessage("Загружаю cписок клиентов");

                marketingChannels = DBDataExtractor<MarketingChannel>.GetDataList(DBProvider.Client.GetMarketingChannelList);


                ItemLabelsInfo.ItemPropertyChanged += ItemLabelsInfo_ItemPropertyChanged;

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

        private void ItemLabelsInfo_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            UpdateLabelInfo(ItemLabelsInfo[e.CollectionIndex]);
        }
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

    public interface IAlertsShower
    {
        void InitAlerts(List<Alert> alerts);
    }

}
