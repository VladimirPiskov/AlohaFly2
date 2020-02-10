using AlohaService.ServiceDataContracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlohaFly.DataExtension
{

    public class LinkedData<T>
        where T : class
    {
        public Func<DateTime?, DateTime?,OperationResultValue<List<T>>> DBListFunc { set; get; }
        public Func<T, FullyObservableDBDataUpdateResult<T>> DBUpdater { set; get; }
        public Func<T, FullyObservableDBDataUpdateResult<T>> DBDeleter { set; get; }

        public Func<T, T> DBChildrenDataUpdater { set; get; }

        private DateTime? startDate;

        public Action<T> SubClassesUpdater { set; get; }
        public Action<T> SubClassesDeleter { set; get; }
    }
    public class DataDBUpdaterFactory<T>
        where T : class
    {

        public DataDBUpdaterFactory(Func<T, long> _keySelector)
        {
            keySelector = _keySelector;
            InitFunctions();
        }


        public LinkedData<T> GetLinkedFullyObservableDBData()
        {

            var res = new LinkedData<T>();

            res.SubClassesUpdater = (a) => { };
            res.DBChildrenDataUpdater = (a) => postGetFunc(a);

            if (typeof(T) == (typeof(Dish)))
            {
                res.DBListFunc = (_, __) => DBProvider.Client.GetDishList() as OperationResultValue<List<T>>;

            }

            else if (typeof(T) == (typeof(ItemLabelInfo)))
            {
                res.DBListFunc = (_, __) => DBProvider.Client.GetItemLabelInfoList() as OperationResultValue<List<T>>;

            }
            else if (typeof(T) == (typeof(DishKitchenGroup)))
            {
                res.DBListFunc = (_, __) => DBProvider.Client.GetDishKitсhenGroupsList() as OperationResultValue<List<T>>;

            }
            else if (typeof(T) == (typeof(DishLogicGroup)))
            {
                res.DBListFunc = (_, __) => DBProvider.Client.GetDishLogicGroupsList() as OperationResultValue<List<T>>;

            }

            else if (typeof(T) == (typeof(ItemLabelInfo)))
            {
                res.DBListFunc = (_, __) => DBProvider.Client.GetItemLabelInfoList() as OperationResultValue<List<T>>;

            }

            else if (typeof(T) == (typeof(OrderCustomer)))
            {
                res.DBListFunc = (_,__) => DBProvider.Client.GetOrderCustomerList2() as OperationResultValue<List<T>>;

            }
            else if (typeof(T) == (typeof(OrderCustomerAddress)))
            {
                res.DBListFunc = (_, __) => DBProvider.Client.GetOrderCustomerAddressList() as OperationResultValue<List<T>>;

                
            }
            else if (typeof(T) == (typeof(OrderCustomerPhone)))
            {
                res.DBListFunc = (_, __) => DBProvider.Client.GetOrderCustomerPhoneList() as OperationResultValue<List<T>>;
            }
            else if (typeof(T) == (typeof(User)))
            {
                res.DBListFunc = (_, __) => DBProvider.Client.GetUserList() as OperationResultValue<List<T>>;
            }
            else if (typeof(T) == (typeof(Payment)))
            {
                res.DBListFunc = (_, __) => DBProvider.Client.GetPaymentList() as OperationResultValue<List<T>>;
            }
            else if (typeof(T) == (typeof(PaymentGroup)))
            {
                res.DBListFunc = (_, __) => DBProvider.Client.GetPaymentGroupList() as OperationResultValue<List<T>>;
            }
            else if (typeof(T) == (typeof(Discount)))
            {
                res.DBListFunc = (_, __) => DBProvider.Client.GetDiscountList() as OperationResultValue<List<T>>;
            }
            else if (typeof(T) == (typeof(Driver)))
            {
                res.DBListFunc = (_, __) => DBProvider.Client.GetDriverList() as OperationResultValue<List<T>>;
            }
            else if (typeof(T) == (typeof(DeliveryPlace)))
            {
                res.DBListFunc = (_, __) => DBProvider.Client.GetDeliveryPlaceList() as OperationResultValue<List<T>>;
            }
            else if (typeof(T) == (typeof(AirCompany)))
            {
                res.DBListFunc = (_, __) => DBProvider.Client.GetAirCompanyList() as OperationResultValue<List<T>>;
            }
            else if (typeof(T) == (typeof(MarketingChannel)))
            {
                res.DBListFunc = (_, __) => DBProvider.Client.GetMarketingChannelList() as OperationResultValue<List<T>>;
            }
            else if (typeof(T) == (typeof(ContactPerson)))
            {
                res.DBListFunc = (_, __) => DBProvider.Client.GetContactPersonList() as OperationResultValue<List<T>>;
            }
            else if (typeof(T) == (typeof(OrderToGo)))
            {
                res.DBListFunc = (dt1, dt2) =>
                {
                    var res1 = DBProvider.Client.GetOrderToGoList(
                        new OrderToGoFilter() { DeliveryDateStart = dt1, DeliveryDateEnd = dt2 }, 
                        new PageInfo(){
                        Skip = 0,
                        Take = 10000
                    });
                    
                    if (res1.Success)
                    {
                        res1.Result = res1.Result.Select(a => postGetFunc(a as T) as OrderToGo).ToList();
                    }

                    return res1 as OperationResultValue<List<T>>;
                }; 
            }
            else if (typeof(T) == (typeof(OrderFlight)))
            {
                res.DBListFunc = (dt1, dt2) =>
                {
                    var res1 = DBProvider.Client.GetOrderFlightList(
                        new OrderFlightFilter() { DeliveryDateStart = dt1, DeliveryDateEnd = dt2 },
                        new PageInfo()
                        {
                            Skip = 0,
                            Take = 10000
                        });

                    if (res1.Success)
                    {
                        res1.Result = res1.Result.Select(a => postGetFunc(a as T) as OrderFlight).ToList();
                    }

                    return res1 as OperationResultValue<List<T>>;
                };
            }
            else
            {
                throw new ArgumentException("Non supported type");
            }
            res.DBUpdater = GetDBUpdater();

            res.DBDeleter = GetDBDeleter();

            return res;
        }


        private Func<T, long> keySelector;
        Func<T, OperationResult> updateFunc;
        Func<T, T> preUpdateFunc=a=>a;
        Func<T, T> postGetFunc = a => a;
        Func<T, OperationResult> createFunc;
        Func<long, OperationResultValue<T>> getFunc;
        Func<long, OperationResult> deleteFunc;
        Func<OperationResultValue<List<T>>> dBListFunc { set; get; }

        private void InitFunctions()
        {

            if (typeof(T) == (typeof(OrderCustomer)))
            {
                createFunc = itm => { return DBProvider.Client.CreateOrderCustomer(itm as OrderCustomer); };
                updateFunc = itm => { return DBProvider.Client.UpdateOrderCustomer(itm as OrderCustomer); };
                getFunc = itm => { return DBProvider.Client.GetOrderCustomer2(itm) as OperationResultValue<T>; };
                deleteFunc = itm => { return DBProvider.Client.DeleteOrderCustomer(itm) as OperationResult; };
                preUpdateFunc = itm => { var tItm = itm as OrderCustomer; tItm.LastUpdatedSession = RealTimeUpdaterSingleton.Instance.Transaction; return tItm as T; };
            }
            else if (typeof(T) == (typeof(User)))
            {
                createFunc = itm => { return DBProvider.Client.CreateUser(itm as UserInfo); };
                updateFunc = itm => { return DBProvider.Client.UpdateUser(itm as User); };
                getFunc = itm => { return DBProvider.Client.GetUser(itm) as OperationResultValue<T>; };
                deleteFunc = itm => { return DBProvider.Client.DeleteUser(itm) as OperationResult; };
                preUpdateFunc = itm => { var tItm = itm as User; tItm.LastUpdatedSession = RealTimeUpdaterSingleton.Instance.Transaction; return tItm as T; };
            }
            else if (typeof(T) == (typeof(OrderCustomerPhone)))
            {
                createFunc = itm => { return DBProvider.Client.CreateOrderCustomerPhone(itm as OrderCustomerPhone); };
                updateFunc = itm => { return DBProvider.Client.UpdateOrderCustomerPhone(itm as OrderCustomerPhone); };
                getFunc = itm => { return DBProvider.Client.GetOrderCustomerPhone(itm) as OperationResultValue<T>; };
                deleteFunc = itm => { return DBProvider.Client.DeleteOrderCustomerPhone(itm) as OperationResult; };
                preUpdateFunc = itm => { var tItm = itm as OrderCustomerPhone; tItm.LastUpdatedSession = RealTimeUpdaterSingleton.Instance.Transaction; return tItm as T; };
            }
            else if (typeof(T) == (typeof(OrderCustomerAddress)))
            {
                createFunc = itm => { return DBProvider.Client.CreateOrderCustomerAddress(itm as OrderCustomerAddress); };
                updateFunc = itm => { return DBProvider.Client.UpdateOrderCustomerAddress(itm as OrderCustomerAddress); };
                getFunc = itm => { return DBProvider.Client.GetOrderCustomerAddress(itm) as OperationResultValue<T>; };
                deleteFunc = itm => { return DBProvider.Client.DeleteOrderCustomerAddress(itm) as OperationResult; };
                preUpdateFunc = itm => { var tItm = itm as OrderCustomerAddress; tItm.LastUpdatedSession = RealTimeUpdaterSingleton.Instance.Transaction; return tItm as T; };
            }
            else if (typeof(T) == (typeof(ItemLabelInfo)))
            {
                createFunc = itm => { return DBProvider.Client.CreateItemLabelInfo(itm as ItemLabelInfo); };
                updateFunc = itm => { return DBProvider.Client.UpdateItemLabelInfo(itm as ItemLabelInfo); };
                getFunc = itm => { return DBProvider.Client.GetItemLabelInfo(itm) as OperationResultValue<T>; };
                deleteFunc = itm => { return DBProvider.Client.DeleteItemLabelInfo(itm) as OperationResult; };
                preUpdateFunc = itm =>
                {
                    if (itm == null) return itm;
                    var tItm = itm as ItemLabelInfo;
                    if (tItm.ParenItemId == 0 && tItm.Dish != null)
                    {
                        tItm.ParenItemId = tItm.Dish.Id;
                    }
                    tItm.LastUpdatedSession = RealTimeUpdaterSingleton.Instance.Transaction;
                    return tItm as T;
                };

                postGetFunc = itm =>
                {
                    var tItm = itm as ItemLabelInfo;
                    if (tItm == null) return itm;
                    if (DataExtension.DataCatalogsSingleton.Instance.DishData.Data.Any(a => a.Id == tItm.ParenItemId))
                    {
                        var d = DataExtension.DataCatalogsSingleton.Instance.DishData.Data.FirstOrDefault(a => a.Id == tItm.ParenItemId);
                        tItm.Dish = d;
                        // Кол-во наклеек у блюда
                        if (DataExtension.DataCatalogsSingleton.Instance.ItemLabelInfoData != null)
                        {
                            d.LabelsCount = DataExtension.DataCatalogsSingleton.Instance.ItemLabelInfoData.Data.Where(a => a.ParenItemId == d.Id).Count();
                            if (!DataExtension.DataCatalogsSingleton.Instance.ItemLabelInfoData.Data.Any(a => a.Id == tItm.Id))
                            {
                                d.LabelsCount++;
                            }
                        }

                    }


                    return tItm as T;
                };
            }
            else if (typeof(T) == (typeof(Dish)))
            {
                createFunc = itm => { return DBProvider.Client.CreateDish(itm as Dish); };
                updateFunc = itm => { return DBProvider.Client.UpdateDish(itm as Dish); };
                getFunc = itm => { return DBProvider.Client.GetDish(itm) as OperationResultValue<T>; };
                deleteFunc = itm => { return DBProvider.Client.DeleteDish(itm) as OperationResult; };

                preUpdateFunc = itm =>
                {
                    if (itm == null) return itm;
                    var tItm = itm as Dish;
                    if (tItm.DishLogicGroup != null)
                    {
                        tItm.DishLogicGroupId = tItm.DishLogicGroup.Id;
                    }
                    if (tItm.DishKitсhenGroup != null)
                    {
                        tItm.DishKitсhenGroupId = tItm.DishKitсhenGroup.Id;
                    }
                    tItm.LastUpdatedSession = RealTimeUpdaterSingleton.Instance.Transaction;
                    return tItm as T;
                };

                postGetFunc = itm =>
                {
                    var tItm = itm as Dish;
                    if (tItm == null) return itm;

                    if (DataExtension.DataCatalogsSingleton.Instance.DishKitchenGroupData.Data.Any(a => a.Id == tItm.DishKitсhenGroupId))
                    {
                        tItm.DishKitсhenGroup = DataExtension.DataCatalogsSingleton.Instance.DishKitchenGroupData.Data.FirstOrDefault(a => a.Id == tItm.DishKitсhenGroupId);
                    }
                    if (DataExtension.DataCatalogsSingleton.Instance.DishLogicGroupData.Data.Any(a => a.Id == tItm.DishLogicGroupId))
                    {
                        tItm.DishLogicGroup = DataExtension.DataCatalogsSingleton.Instance.DishLogicGroupData.Data.FirstOrDefault(a => a.Id == tItm.DishLogicGroupId);
                    }
                    return tItm as T;
                };

            }
            else if (typeof(T) == (typeof(Payment)))
            {
                createFunc = itm => { return DBProvider.Client.CreatePayment(itm as Payment); };
                updateFunc = itm => { return DBProvider.Client.UpdatePayment(itm as Payment); };
                getFunc = itm => { return DBProvider.Client.GetPayment(itm) as OperationResultValue<T>; };
                deleteFunc = itm => { return DBProvider.Client.DeletePayment(itm) as OperationResult; };
                preUpdateFunc = itm =>
                {
                    var tItm = itm as Payment;
                    if ((tItm.PaymentGroupId == 0) && (tItm.PaymentGroup != null))
                    {
                        tItm.PaymentGroupId = tItm.PaymentGroup.Id;
                    }
                    tItm.PaymentGroup = null;
                    tItm.LastUpdatedSession = RealTimeUpdaterSingleton.Instance.Transaction; 
                    return tItm as T;
                };
                postGetFunc = itm =>
                {
                    Payment ord = itm as Payment;
                    if (ord == null) return itm;
                    ord.PaymentGroup = DataExtension.DataCatalogsSingleton.Instance.PaymentGroupData.Data.SingleOrDefault(a => a.Id == ord.PaymentGroupId);
                    return ord as T;
                };
            }
            else if (typeof(T) == (typeof(AirCompany)))
            {
                createFunc = itm => { return DBProvider.Client.CreateAirCompany(itm as AirCompany); };
                updateFunc = itm => { return DBProvider.Client.UpdateAirCompany(itm as AirCompany); };
                getFunc = itm => { return DBProvider.Client.GetAirCompany(itm) as OperationResultValue<T>; };
                deleteFunc = itm => { return DBProvider.Client.DeleteAirCompany(itm) as OperationResult; };
                preUpdateFunc = itm =>
                {
                    var tItm = itm as AirCompany;
                    if ((tItm.DiscountId == 0) && (tItm.DiscountType != null))
                    {
                        tItm.DiscountId = tItm.DiscountType.Id;
                    }
                    tItm.DiscountType = null;
                    if ((tItm.PaymentId == 0) && (tItm.PaymentType != null))
                    {
                        tItm.PaymentId = tItm.PaymentType.Id;
                    }
                    tItm.DiscountType = null;
                    tItm.LastUpdatedSession = RealTimeUpdaterSingleton.Instance.Transaction; 
                    return tItm as T;
                };
                postGetFunc = itm =>
                {
                    var ord = itm as AirCompany;
                    if (ord == null) return itm;
                    ord.DiscountType  = DataExtension.DataCatalogsSingleton.Instance.DiscountData.Data.SingleOrDefault(a => a.Id == ord.DiscountId);
                    ord.PaymentType = DataExtension.DataCatalogsSingleton.Instance.PaymentData.Data.SingleOrDefault(a => a.Id == ord.PaymentId);
                    return ord as T;
                };
            }
            else if (typeof(T) == (typeof(DishKitchenGroup)))
            {
                createFunc = itm => { return DBProvider.Client.CreateDishKitchenGroup(itm as DishKitchenGroup); };
                updateFunc = itm => { return DBProvider.Client.UpdateDishKitchenGroup(itm as DishKitchenGroup); };
                getFunc = itm => { return DBProvider.Client.GetDishKitchenGroup(itm) as OperationResultValue<T>; };
                deleteFunc = itm => { return DBProvider.Client.DeleteDishKitchenGroup(itm) as OperationResult; };
                preUpdateFunc = itm =>
                {
                    var tItm = itm as DishKitchenGroup;
                    tItm.LastUpdatedSession = RealTimeUpdaterSingleton.Instance.Transaction; 
                    return tItm as T;
                };
            }
            else if (typeof(T) == (typeof(DishLogicGroup)))
            {
                createFunc = itm => { return DBProvider.Client.CreateDishLogicGroup(itm as DishLogicGroup); };
                updateFunc = itm => { return DBProvider.Client.UpdateDishLogicGroup(itm as DishLogicGroup); };
                getFunc = itm => { return DBProvider.Client.GetDishLogicGroup(itm) as OperationResultValue<T>; };
                deleteFunc = itm => { return DBProvider.Client.DeleteDishLogicGroup(itm) as OperationResult; };
                preUpdateFunc = itm =>
                {
                    var tItm = itm as DishLogicGroup;
                    tItm.LastUpdatedSession = RealTimeUpdaterSingleton.Instance.Transaction; 
                    return tItm as T;
                };
            }
            else if (typeof(T) == (typeof(PaymentGroup)))
            {
                createFunc = itm => { return DBProvider.Client.CreatePaymentGroup(itm as PaymentGroup); };
                updateFunc = itm => { return DBProvider.Client.UpdatePaymentGroup(itm as PaymentGroup); };
                getFunc = itm => { return DBProvider.Client.GetPaymentGroup(itm) as OperationResultValue<T>; };
                deleteFunc = itm => { return DBProvider.Client.DeletePaymentGroup(itm) as OperationResult; };
                preUpdateFunc = itm =>
                {
                    var tItm = itm as PaymentGroup;
                    tItm.LastUpdatedSession = RealTimeUpdaterSingleton.Instance.Transaction; 
                    return tItm as T;
                };
            }
            else if (typeof(T) == (typeof(Discount)))
            {
                createFunc = itm => { return DBProvider.Client.CreateDiscount(itm as Discount); };
                updateFunc = itm => { return DBProvider.Client.UpdateDiscount(itm as Discount); };
                getFunc = itm => { return DBProvider.Client.GetPaymentGroup(itm) as OperationResultValue<T>; };
                deleteFunc = itm => { return DBProvider.Client.DeletePaymentGroup(itm) as OperationResult; };
                preUpdateFunc = itm =>
                {
                    var tItm = itm as Discount;
                    tItm.LastUpdatedSession = RealTimeUpdaterSingleton.Instance.Transaction; 
                    return tItm as T;
                };
            }
            else if (typeof(T) == (typeof(DeliveryPlace)))
            {
                createFunc = itm => { return DBProvider.Client.CreateDeliveryPlace(itm as DeliveryPlace); };
                updateFunc = itm => { return DBProvider.Client.UpdateDeliveryPlace(itm as DeliveryPlace); };
                getFunc = itm => { return DBProvider.Client.GetDeliveryPlace(itm) as OperationResultValue<T>; };
                deleteFunc = itm => { return DBProvider.Client.DeleteDeliveryPlace(itm) as OperationResult; };
                preUpdateFunc = itm =>
                {
                    var tItm = itm as DeliveryPlace;
                    tItm.LastUpdatedSession = RealTimeUpdaterSingleton.Instance.Transaction; 
                    return tItm as T;
                };
            }
            else if (typeof(T) == (typeof(ContactPerson)))
            {
                createFunc = itm => { return DBProvider.Client.CreateContactPerson(itm as ContactPerson); };
                updateFunc = itm => { return DBProvider.Client.UpdateContactPerson(itm as ContactPerson); };
                getFunc = itm => { return DBProvider.Client.GetContactPerson(itm) as OperationResultValue<T>; };
                deleteFunc = itm => { return DBProvider.Client.DeleteContactPerson(itm) as OperationResult; };
                preUpdateFunc = itm =>
                {
                    var tItm = itm as ContactPerson;
                    tItm.LastUpdatedSession = RealTimeUpdaterSingleton.Instance.Transaction; 
                    return tItm as T;
                };
            }
            
            else if (typeof(T) == (typeof(MarketingChannel)))
            {
                createFunc = itm => { return DBProvider.Client.CreateMarketingChannel(itm as MarketingChannel); };
                updateFunc = itm => { return DBProvider.Client.UpdateMarketingChannel(itm as MarketingChannel); };
                getFunc = itm => { return DBProvider.Client.GetMarketingChannel(itm) as OperationResultValue<T>; };
                deleteFunc = itm => { return DBProvider.Client.DeleteMarketingChannel(itm) as OperationResult; };
                preUpdateFunc = itm =>
                {
                    var tItm = itm as MarketingChannel;
                    tItm.LastUpdatedSession = RealTimeUpdaterSingleton.Instance.Transaction; 
                    return tItm as T;
                };
            }
            else if (typeof(T) == (typeof(Driver)))
            {
                createFunc = itm => { return DBProvider.Client.CreateDriver(itm as Driver); };
                updateFunc = itm => { return DBProvider.Client.UpdateDriver(itm as Driver); };
                getFunc = itm => { return DBProvider.Client.GetDriver(itm) as OperationResultValue<T>; };
                deleteFunc = itm => { return DBProvider.Client.DeleteDriver(itm) as OperationResult; };
                preUpdateFunc = itm =>
                {
                    var tItm = itm as Driver;
                    tItm.LastUpdatedSession = RealTimeUpdaterSingleton.Instance.Transaction; 
                    return tItm as T;
                };
            }
            else if (typeof(T) == (typeof(OrderToGo)))
            {
                createFunc = itm => { return DBProvider.Client.CreateOrderToGoWithPackage(itm as OrderToGo); };
                updateFunc = itm => { return DBProvider.Client.UpdateOrderToGo(itm as OrderToGo); };
                getFunc = itm => { return DBProvider.Client.GetOrderToGo(itm) as OperationResultValue<T>; };
                deleteFunc = itm => { return DBProvider.Client.DeleteOrderToGo(itm) as OperationResult; };
                preUpdateFunc = itm =>
                {
                    if (itm == null) return itm;
                    OrderToGo order = itm as OrderToGo;
                    order.OrderCustomerId = order.OrderCustomer?.Id;
                    order.MarketingChannelId = order.MarketingChannel?.Id;
                    order.DriverId = order.Driver?.Id;
                    order.CreatedById = order.CreatedBy?.Id;
                    order.AddressId = order.Address?.Id;
                    order.LastUpdatedSession = RealTimeUpdaterSingleton.Instance.Transaction;
                    foreach (var d in order.DishPackages)
                    {
                        d.OrderToGoId = order.Id;
                        d.DishId = d.Dish.Id;
                        d.Dish = null;

                        d.OrderToGo = null;
                    }
                    return order as T;
                };

                postGetFunc = itm =>
                {
                    OrderToGo ord = itm as OrderToGo;
                    if (ord == null) return itm;
                    ord.OrderCustomer = DataExtension.DataCatalogsSingleton.Instance.OrderCustomerData.Data.SingleOrDefault(a => a.Id == ord.OrderCustomerId);
                    if (ord.CreatedById != null)
                    {
                        ord.CreatedBy = DataExtension.DataCatalogsSingleton.Instance.ManagerData.Data.SingleOrDefault(a => a.Id == ord.CreatedById);
                    }
                    if (ord.PaymentId != null)
                    {
                        ord.PaymentType = DataExtension.DataCatalogsSingleton.Instance.PaymentData.Data.SingleOrDefault(a => a.Id == ord.PaymentId);
                    }

                    if (ord.MarketingChannelId != null)
                    {
                        ord.MarketingChannel = DataExtension.DataCatalogsSingleton.Instance.MarketingChannelData.Data.SingleOrDefault(a => a.Id == ord.MarketingChannelId);
                    }

                    if (ord.DriverId != null)
                    {
                        ord.Driver = DataExtension.DataCatalogsSingleton.Instance.DriverData.Data.SingleOrDefault(a => a.Id == ord.DriverId);
                    }
                    if (ord.AddressId != 0)
                    {
                        ord.Address = DataExtension.DataCatalogsSingleton.Instance.OrderCustomerAddressData.Data.SingleOrDefault(a => a.Id == ord.AddressId);
                    }
                    if (ord.DishPackages != null)
                    {
                        foreach (var d in ord.DishPackages)
                        {

                            d.Printed = true;
                            d.Dish = DataExtension.DataCatalogsSingleton.Instance.DishData.Data.SingleOrDefault(a => a.Id == d.DishId);
                            if (d.Deleted && d.DeletedStatus == 1) { d.UpDateSpisPayment(); }
                        }
                    }
                    return ord as T;
                };

            }
            else if (typeof(T) == (typeof(OrderFlight)))
            {
                createFunc = itm => { return DBProvider.Client.CreateOrderFlightWithPackage(itm as OrderFlight); };
                updateFunc = itm => { return DBProvider.Client.UpdateOrderFlight2(itm as OrderFlight, Authorization.CurentUser.Id); };
                getFunc = itm => { return DBProvider.Client.GetOrderFlight(itm) as OperationResultValue<T>; };
                deleteFunc = itm => { return DBProvider.Client.DeleteOrderFlight(itm) as OperationResult; };
                preUpdateFunc = itm =>
                {
                    if (itm == null) return itm;
                    OrderFlight order = itm as OrderFlight;
                    order.AirCompanyId = order.AirCompany?.Id;
                    order.ContactPersonId = order.ContactPerson?.Id;
                    order.WhoDeliveredPersonPersonId = order.WhoDeliveredPersonPerson?.Id;
                    order.DeliveryPlaceId = order.DeliveryPlace?.Id;
                    order.DriverId = order.Driver?.Id;
                    order.CreatedById = order.CreatedBy?.Id;
                    order.SendById = order.SendBy?.Id;

                    order.LastUpdatedSession = RealTimeUpdaterSingleton.Instance.Transaction;
                    foreach (var d in order.DishPackages)
                    {
                        d.OrderFlightId = order.Id;
                        if (d.Dish != null)
                        {
                            d.DishId = d.Dish.Id;
                        }
                        d.Dish = null;
                        d.OrderFlight = null;
                    }
                    return order as T;
                };

                postGetFunc = itm =>
                {
                    OrderFlight ord = itm as OrderFlight;
                    if (ord == null) return itm;
                    if (ord.ContactPersonId != null)
                    {
                        ord.ContactPerson = DataExtension.DataCatalogsSingleton.Instance.ContactPersonData.Data.SingleOrDefault(a => a.Id == ord.ContactPersonId);
                    }
                    ord.AirCompany = DataExtension.DataCatalogsSingleton.Instance.AirCompanyData.Data.SingleOrDefault(a => a.Id == ord.AirCompanyId);
                    if (ord.DeliveryPlaceId != null)
                    {
                        ord.DeliveryPlace = DataExtension.DataCatalogsSingleton.Instance.DeliveryPlaceData.Data.SingleOrDefault(a => a.Id == ord.DeliveryPlaceId);
                    }
                    if (ord.CreatedById != null)
                    {
                        ord.CreatedBy = DataExtension.DataCatalogsSingleton.Instance.ManagerData.Data.SingleOrDefault(a => a.Id == ord.CreatedById);
                    }

                    if (ord.SendById != null)
                    {
                        ord.SendBy = DataExtension.DataCatalogsSingleton.Instance.ManagerData.Data.SingleOrDefault(a => a.Id == ord.SendById);
                    }

                    if (ord.DishPackages != null)
                    {
                        foreach (var d in ord.DishPackages)
                        {
                            d.Dish = DataExtension.DataCatalogsSingleton.Instance.DishData.Data.SingleOrDefault(a => a.Id == d.DishId);

                            d.Printed = true;
                            if (d.Deleted && d.DeletedStatus == 1) { d.UpDateSpisPayment(); }
                        }
                    }
                    return ord as T;
                };

            }
            else

            {
                throw new ArgumentException("Not supported type");
            }
        }

        private Func<T, FullyObservableDBDataUpdateResult<T>> GetDBDeleter()
        {
            return (item) =>
            {
                var result = new FullyObservableDBDataUpdateResult<T>();
                var dbRes = deleteFunc(keySelector(item));
                result.Succeess = dbRes.Success;
                if (!dbRes.Success)
                {
                    result.ErrorMessage = dbRes.ErrorMessage;
                }
                return result;
            };
        }

        private Func<T, FullyObservableDBDataUpdateResult<T>> GetDBUpdater()
        {
            return (item) =>
                        {
                            var result = new FullyObservableDBDataUpdateResult<T>();
                            OperationResult dbRes;
                            long ItemId = keySelector(item);
                            item = preUpdateFunc(item);
                            if (ItemId == 0)
                            {
                                dbRes = createFunc(item);
                                ItemId = dbRes.CreatedObjectId;
                            }
                            else
                            {
                                dbRes = updateFunc(item);
                            }

                            result.Succeess = dbRes.Success;
                            if (dbRes.Success)
                            {
                                var newItmRes = getFunc(ItemId);
                                if (newItmRes.Success)
                                {
                                    result.UpdatedItem = postGetFunc(newItmRes.Result);
                                }
                                else
                                {
                                    result.Succeess = false;
                                    result.ErrorMessage = newItmRes.ErrorMessage;
                                }
                            }
                            else
                            {
                                result.ErrorMessage = dbRes.ErrorMessage;
                            }
                            return result;

                        };

        }



    }
}
