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

            if (typeof(T) == (typeof(OrderCustomer)))
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
                preUpdateFunc = itm => { var tItm = itm as OrderCustomer;  tItm.LastUpdatedSession = RealTimeUpdaterSingleton.Instance.Transaction; return tItm as T; };
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
                        ord.CreatedBy = DataExtension.DataCatalogsSingleton.Instance.ManagerOperator.SingleOrDefault(a => a.Id == ord.CreatedById);
                    }
                    if (ord.PaymentId != null)
                    {
                        ord.PaymentType = DataExtension.DataCatalogsSingleton.Instance.Payments.SingleOrDefault(a => a.Id == ord.PaymentId);
                    }

                    if (ord.MarketingChannelId != null)
                    {
                        ord.MarketingChannel = DataExtension.DataCatalogsSingleton.Instance.MarketingChannels.SingleOrDefault(a => a.Id == ord.MarketingChannelId);
                    }

                    if (ord.DriverId != null)
                    {
                        ord.Driver = DataExtension.DataCatalogsSingleton.Instance.Drivers.SingleOrDefault(a => a.Id == ord.DriverId);
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
                            d.Dish = DataExtension.DataCatalogsSingleton.Instance.Dishes.SingleOrDefault(a => a.Id == d.DishId);
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
