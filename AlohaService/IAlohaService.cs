using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using AlohaService.ServiceDataContracts;
using AlohaService.ServiceDataContracts.ExternalContracts;

namespace AlohaService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IAlohaService
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);


        #region User

        [OperationContract]
        OperationResult CreateUser(UserInfo userInfo);

        [OperationContract]
        OperationResultValue<User> GetUser(long userId);

        [OperationContract]
        OperationResult UpdateUser(User user);

        [OperationContract]
        OperationResult DeleteUser(long userId);

        [OperationContract]
        OperationResultValue<User> LoginUser(string loginOrEmail, string password);

        [OperationContract]
        OperationResultValue<List<User>> GetUserList();
        
        #endregion User


        #region Air Company

        [OperationContract]
        OperationResult CreateAirCompany(AirCompany airCompany);

        [OperationContract]
        OperationResultValue<AirCompany> GetAirCompany(long airCompanyId);

        [OperationContract]
        OperationResult UpdateAirCompany(AirCompany airCompany);

        [OperationContract]
        OperationResult DeleteAirCompany(long airCompanyId);

        [OperationContract]
        OperationResultValue<List<AirCompany>> GetAirCompanyList();

        [OperationContract]
        OperationResultValue<List<string>> GetFlightCodes(long airCompanyId, string startsWith);

        #endregion Air Company


        #region Contact Person

        [OperationContract]
        OperationResult CreateContactPerson(ContactPerson contactPerson);

        [OperationContract]
        OperationResultValue<ContactPerson> GetContactPerson(long contactPersonId);

        [OperationContract]
        OperationResult UpdateContactPerson(ContactPerson contactPerson);

        [OperationContract]
        OperationResult DeleteContactPerson(long contactPersonId);

        [OperationContract]
        OperationResultValue<List<ContactPerson>> GetContactPersonList();

        #endregion Contact Person


        #region Curier

        [OperationContract]
        OperationResult CreateCurier(Curier curier);

        [OperationContract]
        OperationResultValue<Curier> GetCurier(long curierId);

        [OperationContract]
        OperationResult UpdateCurier(Curier curier);

        [OperationContract]
        OperationResult DeleteCurier(long curierId);

        [OperationContract]
        OperationResultValue<List<Curier>> GetCurierList();

        #endregion Curier


        #region ItemLabelInfo

        [OperationContract]
        OperationResult CreateItemLabelInfo(ItemLabelInfo itemLabelInfo);

        [OperationContract]
        OperationResultValue<ItemLabelInfo> GetItemLabelInfo(long itemLabelInfoId);

        [OperationContract]
        OperationResult UpdateItemLabelInfo(ItemLabelInfo itemLabelInfo);

        [OperationContract]
        OperationResult DeleteItemLabelInfo(long itemLabelInfoId);

        [OperationContract]
        OperationResultValue<List<ItemLabelInfo>> GetItemLabelInfoList();

        #endregion ItemLabelInfo


        #region Delivery Person

        [OperationContract]
        OperationResult CreateDeliveryPerson(DeliveryPerson deliveryPerson);

        [OperationContract]
        OperationResultValue<DeliveryPerson> GetDeliveryPerson(long deliveryPersonId);

        [OperationContract]
        OperationResult UpdateDeliveryPerson(DeliveryPerson deliveryPersonId);

        [OperationContract]
        OperationResult DeleteDeliveryPerson(long deliveryPerson);

        [OperationContract]
        OperationResultValue<List<DeliveryPerson>> GetDeliveryPersonList();

        #endregion Delivery Person


        #region Delivery Place

        [OperationContract]
        OperationResult CreateDeliveryPlace(DeliveryPlace deliveryPlace);

        [OperationContract]
        OperationResultValue<DeliveryPlace> GetDeliveryPlace(long deliveryPlaceId);

        [OperationContract]
        OperationResult UpdateDeliveryPlace(DeliveryPlace deliveryPlace);

        [OperationContract]
        OperationResult DeleteDeliveryPlace(long deliveryPlaceId);

        [OperationContract]
        OperationResultValue<List<DeliveryPlace>> GetDeliveryPlaceList();

        #endregion Delivery Place








        #region Delivery Place

        [OperationContract]
        OperationResult CreateMarketingChannel(MarketingChannel MarketingChannel);

        [OperationContract]
        OperationResultValue<MarketingChannel> GetMarketingChannel(long MarketingChannelId);

        [OperationContract]
        OperationResult UpdateMarketingChannel(MarketingChannel MarketingChannel);

        [OperationContract]
        OperationResult DeleteMarketingChannel(long MarketingChannelId);

        [OperationContract]
        OperationResultValue<List<MarketingChannel>> GetMarketingChannelList();

        #endregion Delivery Place








        #region Dish

        [OperationContract]
        OperationResult CreateDish(Dish dish);

        [OperationContract]
        OperationResultValue<Dish> GetDish(long dishId);

        [OperationContract]
        OperationResult UpdateDish(Dish dish);

        [OperationContract]
        OperationResult DeleteDish(long dishId);

        [OperationContract]
        OperationResultValue<List<Dish>> GetDishList();

        [OperationContract]
        OperationResultValue<List<Dish>> GetDishListIntelliSense(string startsWith);

        [OperationContract]
        OperationResultValue<List<Dish>> GetDishPage(DishFilter filter, PageInfo page);

        [OperationContract]
        OperationResult SetExternalLink(long dishId, int marketingChanel, long externalId);

        #endregion Dish


        #region Driver

        [OperationContract]
        OperationResult CreateDriver(Driver driver);

        [OperationContract]
        OperationResultValue<Driver> GetDriver(long driverId);

        [OperationContract]
        OperationResult UpdateDriver(Driver driver);

        [OperationContract]
        OperationResult DeleteDriver(long driverId);

        [OperationContract]
        OperationResultValue<List<Driver>> GetDriverList();

        #endregion Driver


        #region Order Flight DishPackage

        [OperationContract]
        OperationResult CreateDishPackageFlightOrder(DishPackageFlightOrder dishPackageFlightOrder);

        [OperationContract]
        OperationResultValue<DishPackageFlightOrder> GetDishPackageFlightOrder(long dishPackageFlightOrderId);

        [OperationContract]
        OperationResult UpdateDishPackageFlightOrder(DishPackageFlightOrder dishPackageFlightOrder);

        [OperationContract]
        OperationResult DeleteDishPackageFlightOrder(long dishPackageFlightOrderId);

        [OperationContract]
        OperationResultValue<List<DishPackageFlightOrder>> GetDishPackageFlightOrderList(long orderFlightId);


        

        #endregion Order Flight DishPackage


        #region Order ToGo DishPackage

        [OperationContract]
        OperationResult CreateDishPackageToGoOrder(DishPackageToGoOrder dishPackageToGoOrder);

        [OperationContract]
        OperationResultValue<DishPackageToGoOrder> GetDishPackageToGoOrder(long dishPackageToGoOrderId);

        [OperationContract]
        OperationResult UpdateDishPackageToGoOrder(DishPackageToGoOrder dishPackageToGoOrder);

        [OperationContract]
        OperationResult DeleteDishPackageToGoOrder(long dishPackageToGoOrderId);

        [OperationContract]
        OperationResultValue<List<DishPackageToGoOrder>> GetDishPackageToGoOrderList(long orderToGoId);

        #endregion FlightOrder DishPackage


        #region Order Customer

        [OperationContract]
        OperationResult CreateOrderCustomer(OrderCustomer orderCustomer);
        /*
        [OperationContract]
        OperationResultValue<OrderCustomer> GetOrderCustomer(long orderCustomerId);
        */
        [OperationContract]
        OperationResult UpdateOrderCustomer(OrderCustomer orderCustomer);

        [OperationContract]
        OperationResult DeleteOrderCustomer(long orderCustomerId);

        /*
        [OperationContract]
        OperationResultValue<List<OrderCustomer>> GetOrderCustomerList();
        */

        [OperationContract]
        OperationResultValue<OrderCustomer> GetOrderCustomer2(long orderCustomerId);
        [OperationContract]
        OperationResultValue<List<OrderCustomer>> GetOrderCustomerList2();

        [OperationContract]
        OperationResult MergeCustomers(ServiceDataContracts.OrderCustomer orderCustomer1, ServiceDataContracts.OrderCustomer orderCustomer2);

        #endregion Order Customer




        #region Order CustomerAddress

        [OperationContract]
        OperationResult CreateOrderCustomerAddress(OrderCustomerAddress orderCustomerAddress);

        [OperationContract]
        OperationResultValue<List<OrderCustomerAddress>> GetOrderCustomerAddressList();

        [OperationContract]
        OperationResult UpdateOrderCustomerAddress(OrderCustomerAddress orderCustomerAddress);

        [OperationContract]
        OperationResult DeleteOrderCustomerAddress(long orderCustomerAddressId);

        [OperationContract]
        OperationResultValue<ServiceDataContracts.OrderCustomerAddress> GetOrderCustomerAddress(long orderCustomerAddressId);

        #endregion Order CustomerAddress 

        #region Order CustomerPhone

        [OperationContract]
        OperationResult CreateOrderCustomerPhone(OrderCustomerPhone orderCustomerPhone);

        [OperationContract]
        OperationResultValue<List<OrderCustomerPhone>> GetOrderCustomerPhoneList();

        [OperationContract]
        OperationResult UpdateOrderCustomerPhone(OrderCustomerPhone orderCustomerPhone);

        [OperationContract]
        OperationResult DeleteOrderCustomerPhone(long orderCustomerPhoneId);

        [OperationContract]
        OperationResultValue<ServiceDataContracts.OrderCustomerPhone> GetOrderCustomerPhone(long orderCustomerPhoneId);

        #endregion Order CustomerPhone 

        #region Order Flight

        [OperationContract]
        OperationResult CreateOrderFlight(OrderFlight orderFlight);

        [OperationContract]
        OperationResult CreateOrderFlightWithPackage(ServiceDataContracts.OrderFlight orderFlight);


        [OperationContract]
        bool InsertOrderFlightFromAloha(OrderFlight orderFlight);

        [OperationContract]
        OperationResultValue<OrderFlight> GetOrderFlight(long orderFlightId);

        /*
        [OperationContract]
        OperationResultValue<OrderFlight> GetOrderFlightByCode(long orderCode);
        */
        [OperationContract]
        OperationResultValue<OrderFlight> UpdateOrderFlight(OrderFlight orderFlight, long userId);

        [OperationContract]
        OperationResult UpdateOrderFlight2(OrderFlight orderFlight, long userId);

        [OperationContract]
        OperationResult DeleteOrderFlight(long orderFlightId);

        [OperationContract]
        OperationResultValue<List<OrderFlight>> GetOrderFlightList(OrderFlightFilter filter, PageInfo page);

        [OperationContract]
        OperationResultValue<List<OrderFlight>> GetOrderFlightListNeedToFR();
        [OperationContract]
        OperationResult SetToFlySHValue(bool value, long orderId);



        [OperationContract]
        byte[] ExportToExcel(long orderId);

        #endregion Order Flight



        #region Order To Go

        [OperationContract]
        OperationResult CreateOrderToGo(OrderToGo orderToGo);

        [OperationContract]
        OperationResultValue<OrderToGo> GetOrderToGo(long orderToGoId);

        [OperationContract]
        OperationResult UpdateOrderToGo(OrderToGo orderToGo);

        [OperationContract]
        OperationResult DeleteOrderToGo(long orderToGoId);

        [OperationContract]
        OperationResultValue<List<OrderToGo>> GetOrderToGoList(OrderToGoFilter filter, PageInfo page);

        [OperationContract]
        OperationResultValue<List<OrderToGo>> GetOrderToGoListNeedToFR();

        [OperationContract]
        OperationResult CreateOrderToGoWithPackage(ServiceDataContracts.OrderToGo orderToGo);

        [OperationContract]
        OperationResult SetToGoSHValue(bool value, long orderId);

        #endregion Order To Go

        #region User Access / User Groups
        [OperationContract]
        OperationResult CreateUserGroup(UserGroup userGroup);

        [OperationContract]
        OperationResult DeleteUserGroup(long userGroupId);

        [OperationContract]
        OperationResultValue<List<UserGroup>> GetUserGroupList();

        [OperationContract]
        OperationResult CreateUserFunc(UserFunc userFunc);


        [OperationContract]
        OperationResultValue<UserFunc> GetUserFunc(long userFuncId);

        [OperationContract]
        OperationResult UpdateUserFunc(UserFunc userFunc);

        [OperationContract]
        OperationResult DeleteUserFunc(long userFuncId);

        [OperationContract]
        OperationResultValue<List<UserFunc>> GetUserFuncList();

        [OperationContract]
        OperationResult AddUserToGroup(long userId, long usrGroupId);

        [OperationContract]
        OperationResult RemoveUserFromGroup(long userId, long usrGroupId);

        [OperationContract]
        OperationResultValue<List<UserGroup>> GetAllUserGroups(long userId);

        [OperationContract]
        OperationResultValue<List<UserGroupAccess>> GetAllGroupFuncs(long groupId);

        [OperationContract]
        OperationResult AddFuncToGroup(long funcId, long usrGroupId, FuncAccessType accessType);

        [OperationContract]
        OperationResult RemoveFuncFromGroup(long funcId, long usrGroupId);

        [OperationContract]
        OperationResult RemoveAllFuncsFromDb();

        [OperationContract]
        OperationResult RemoveAllUserGroupsFromDb();

        #endregion UsrAccess / User Groups

        #region Discounts

        [OperationContract]
        OperationResult CreateDiscountRange(DiscountRange discountRange);

        [OperationContract]
        OperationResultValue<DiscountRange> GetDiscountRange(long discountRangeId);

        [OperationContract]
        OperationResult UpdateDiscountRange(DiscountRange discountRange);

        [OperationContract]
        OperationResult DeleteDiscountRange(long discountRangeId);


        [OperationContract]
        OperationResultValue<List<DiscountRange>> GetDiscountRangesList();

        [OperationContract]
        OperationResult CreateDiscount(Discount discount);

        [OperationContract]
        OperationResultValue<Discount> GetDiscount(long discountId);

        [OperationContract]
        OperationResult UpdateDiscount(Discount discount);

        [OperationContract]
        OperationResult DeleteDiscount(long discountId);

        [OperationContract]
        OperationResultValue<List<Discount>> GetDiscountList();

        [OperationContract]
        OperationResult AddRangeToDiscount(long discountId, long discountRangeId);

        [OperationContract]
        OperationResult RemoveRangeFromDiscount(long discountId, long discountRangeId);

        #endregion Discounts

        #region Alerts

        [OperationContract]
        OperationResult CreateAlert(Alert alert);

        [OperationContract]
        OperationResultValue<Alert> GetAlert(long alertId);

        [OperationContract]
        OperationResult UpdateAlert(Alert alert);

        [OperationContract]
        OperationResult DeleteAlert(long alertId);

        [OperationContract]
        OperationResultValue<List<Alert>> GetAlertList();

        [OperationContract]
        OperationResultValue<List<Alert>> GetAlertListByDateRange(DateTime from, DateTime to);

        #endregion Alerts

        #region Payments

        [OperationContract]
        OperationResult CreatePayment(Payment payment);

        [OperationContract]
        OperationResultValue<Payment> GetPayment(long paymentId);

        [OperationContract]
        OperationResult UpdatePayment(Payment payment);

        [OperationContract]
        OperationResultValue<List<Payment>> GetPaymentList();

        [OperationContract]
        OperationResult DeletePayment(long paymentId);

        #endregion Payments



        #region PaymentsGroup

        [OperationContract]
        OperationResult CreatePaymentGroup(PaymentGroup payment);

        [OperationContract]
        OperationResultValue<PaymentGroup> GetPaymentGroup(long paymentId);

        [OperationContract]
        OperationResult UpdatePaymentGroup(PaymentGroup payment);

        [OperationContract]
        OperationResultValue<List<PaymentGroup>> GetPaymentGroupList();

        [OperationContract]
        OperationResult DeletePaymentGroup(long paymentGroupId);

        #endregion PaymentsGroup

        #region DishKitchenGroup

        [OperationContract]
        OperationResult CreateDishKitchenGroup(DishKitchenGroup dishKitchenGroup);

        [OperationContract]
        OperationResultValue<DishKitchenGroup> GetDishKitchenGroup(long dishKitchenGroupId);

        [OperationContract]
        OperationResult UpdateDishKitchenGroup(DishKitchenGroup dishKitchenGroup);

        [OperationContract]
        OperationResult DeleteDishKitchenGroup(long dishKitchenGroupId);

        [OperationContract]
        OperationResultValue<List<DishKitchenGroup>> GetDishKitсhenGroupsList();

        #endregion DishKitchenGroup

        #region DishLogicGroup

        [OperationContract]
        OperationResult CreateDishLogicGroup(DishLogicGroup dishLogicGroup);

        [OperationContract]
        OperationResultValue<DishLogicGroup> GetDishLogicGroup(long dishLogicGroupId);

        [OperationContract]
        OperationResult UpdateDishLogicGroup(DishLogicGroup dishLogicGroup);

        [OperationContract]
        OperationResult DeleteDishLogicGroup(long dishLogicGroupId);

        [OperationContract]
        OperationResultValue<List<DishLogicGroup>> GetDishLogicGroupsList();

        #endregion DishLogicGroup

        #region reports
        [OperationContract]
        OperationResultValue<decimal> GetSumByDates(DateTime dateFrom, DateTime dateTo, OrderStatus status);
        #endregion

        #region Logs
        [OperationContract]
        OperationResultValue<List<ServiceDataContracts.LogItem>> GetLogItems(DateTime dateFrom, DateTime dateTo);

        #endregion


        #region Updater
        [OperationContract]
        OperationResultValue<UpdateResult> GetUpdatesForSession(DateTime lastUpdateTime, DateTime DataTime, Guid session);
        [OperationContract]
        OperationResultValue<UpdateResult> GetServerTime();

        [OperationContract]
        OperationResultValue<UpdateResult> GetUpdatesForSessionTest();

        #endregion



        #region Analitic
        [OperationContract]
        OperationResultValue<AnalitikData> GetLTVValues(DateTime sDate, DateTime eDate);

        [OperationContract]
        OperationResultValue<AnalitikData> GetLTVValues2(DateTime sDate, DateTime eDate);

        [OperationContract]
        OperationResultValue<List<AnalitikOrderData>> GetAllToGoOrdersData(DateTime sDate, DateTime eDate);
        #endregion


        #region External

        [OperationContract]
        OperationResult ExternalCreateSiteToGoOrder(ExternalToGoOrder order);
        [OperationContract]
        OperationResult ExternalCreateDeleveryClubToGoOrder(ExternalToGoOrder order);
        [OperationContract]
        OperationResult ExternalCreateYandexToGoOrder(ExternalToGoOrder order);


        [OperationContract]
        OperationResultValue<List<long>> ExternalGetSiteOrderList();
        [OperationContract]
        OperationResultValue<List<long>> ExternalGetDeleveryClubOrderList();
        [OperationContract]
        OperationResultValue<List<long>> ExternalGetYandexOrderList();

        



        #endregion





    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
