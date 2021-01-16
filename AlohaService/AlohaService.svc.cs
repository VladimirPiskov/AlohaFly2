using System;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using AlohaService.ServiceDataContracts;
using AlohaService.Entities;
using AlohaService.Helpers;
using AlohaService.BusinessServices;
using log4net;
using AutoMapper;
using AlohaService.ServiceDataContracts.ExternalContracts;
using AlohaService.BusinessServices.External;

namespace AlohaService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class AlohaService : IAlohaService
    {
        protected ILog log;

        private UserService userService;
        private OrderService orderService;
        private AirCompanyService airCompanyService;
        private UserGroupService userGroupService;
        private ContactPersonService contactPersonService;
        private CurierService curierService;
        private DeliveryPersonService deliveryPersonService;
        private DeliveryPlaceService deliveryPlaceService;
        private DishPackageFlightOrderService dishPackageFlightOrderService;
        private DishPackageToGoOrderService dishPackageToGoOrderService;
        private DishService dishService;
        private DriverService driverService;
        private ItemLabelInfoService itemLabelInfoService;
        private OrderCustomerService orderCustomerService;
        private OrderCustomerAddressService orderCustomerAddressService;
        private OrderCustomerPhoneService orderCustomerPhoneService;
        private OrderFlightService orderFlightService;
        private OrderToGoService orderToGoService;
        private DiscountService discountService;
        private AlertService alertService;
        private PaymentService paymentService;
        private PaymentGroupService paymentGroupService;
        private DishLogicGroupService dishLogicGroupService;
        private DishKitchenGroupService dishKitchenGroupService;
        private LogItemService logItemService;

        private MarketingChannelService marketingChannelService;

        private UpdaterService updaterService;
        private AnalitikService analitikService;
        private ExternalFromSiteOrdersService externalFromSiteOrdersService;
        private ExternalFromDeleveryClubService externalFromDeleveryClubService;
        private ExternalFromYandexService externalFromYandexService;

        private static bool MapperInited = false;
        public AlohaService()
        {
            LogHelper.Configure();
            log = LogHelper.GetLogger();
            if (!MapperInited)
            {
                try
                {
                 
                    Mapper.Initialize(cfg =>
                    {
                        //cfg.CreateMap<ServiceDataContracts.Dish, Entities.Dish>().ReverseMap();

                        cfg.CreateMap<ServiceDataContracts.User, Entities.User>()
                        .ForMember(m => m.UserRole, opt => opt.Ignore())
                        .ReverseMap();

                        cfg.CreateMap<ServiceDataContracts.Driver, Entities.Driver>()
                        .ReverseMap();

                        cfg.CreateMap<ServiceDataContracts.ContactPerson, Entities.ContactPerson>()
                        .ReverseMap();

                        cfg.CreateMap<ServiceDataContracts.DishLogicGroup, Entities.DishLogicGroup>()
                        .ReverseMap();
                        cfg.CreateMap<ServiceDataContracts.DishKitchenGroup, Entities.DishKitchenGroup>()
                        .ReverseMap();

                        cfg.CreateMap<ServiceDataContracts.PaymentGroup, Entities.PaymentGroup>()
                        .ReverseMap();

                        cfg.CreateMap<ServiceDataContracts.Payment, Entities.Payment>()
                            .ForMember(m => m.PaymentGroup, opt => opt.Ignore());
                        cfg.CreateMap<Entities.Payment, ServiceDataContracts.Payment>()
                            .ForMember(m => m.PaymentGroup, opt => opt.Ignore());

                        cfg.CreateMap<ServiceDataContracts.Discount, Entities.Discount>()
                       .ReverseMap();

                        cfg.CreateMap<ServiceDataContracts.DeliveryPlace, Entities.DeliveryPlace>()
                       .ReverseMap();

                        cfg.CreateMap<ServiceDataContracts.AirCompany, Entities.AirCompany>()
                            .ForMember(m => m.DiscountType, opt => opt.Ignore())
                            .ForMember(m => m.PaymentType, opt => opt.Ignore());

                        cfg.CreateMap< Entities.AirCompany, ServiceDataContracts.AirCompany>()
                            .ForMember(m => m.DiscountType, opt => opt.Ignore())
                            .ForMember(m => m.PaymentType, opt => opt.Ignore());

                        cfg.CreateMap<ServiceDataContracts.Dish, Entities.Dish>();
                            

                        cfg.CreateMap<Entities.Dish, ServiceDataContracts.Dish>()
                            .ForMember(m => m.DishKitсhenGroup, opt => opt.Ignore())
                            .ForMember(m => m.DishLogicGroup, opt => opt.Ignore());

                        cfg.CreateMap<ServiceDataContracts.ItemLabelInfo, Entities.ItemLabelInfo>()
                            .ForMember(m => m.Dish, opt => opt.Ignore());
                        cfg.CreateMap<Entities.ItemLabelInfo, ServiceDataContracts.ItemLabelInfo>()
                            .ForMember(m => m.Dish, opt => opt.Ignore());

                        cfg.CreateMap<Entities.MarketingChannel, ServiceDataContracts.MarketingChannel>()
                            .ReverseMap();




                        cfg.CreateMap<ServiceDataContracts.DishPackageToGoOrder, Entities.DishPackageToGoOrder>()
                        .ForMember(m => m.Dish, opt => opt.Ignore())
                        .ForMember(m => m.OrderToGo, opt => opt.Ignore());

                        cfg.CreateMap<Entities.DishPackageToGoOrder, ServiceDataContracts.DishPackageToGoOrder>()
                        .ForMember(m => m.Dish, opt => opt.Ignore())
                        .ForMember(m => m.OrderToGo, opt => opt.Ignore());


                        cfg.CreateMap<ServiceDataContracts.DishPackageFlightOrder, Entities.DishPackageFlightOrder>()
                        .ForMember(m => m.Dish, opt => opt.Ignore())
                        .ForMember(m => m.OrderFlight, opt => opt.Ignore());



                        cfg.CreateMap<Entities.DishPackageFlightOrder, ServiceDataContracts.DishPackageFlightOrder>()
                        .ForMember(m => m.Dish, opt => opt.Ignore())
                        .ForMember(m => m.OrderFlight, opt => opt.Ignore());



                        

                        cfg.CreateMap<ServiceDataContracts.OrderFlight, Entities.OrderFlight>()

                        .ForMember(m => m.AirCompany, opt => opt.Ignore())
                        .ForMember(m => m.ContactPerson, opt => opt.Ignore())
                        .ForMember(m => m.CreatedBy, opt => opt.Ignore())
                        .ForMember(m => m.DeliveryPlace, opt => opt.Ignore())
                        .ForMember(m => m.PaymentType, opt => opt.Ignore())
                        .ForMember(m => m.SendBy, opt => opt.Ignore())
                        .ForMember(m => m.WhoDeliveredPersonPerson, opt => opt.Ignore())
                        .ForMember(m => m.DishPackages, opt => opt.MapFrom(a => a.DishPackages.ToList()));




                        cfg.CreateMap<Entities.OrderFlight, ServiceDataContracts.OrderFlight>()

                        .ForMember(m => m.AirCompany, opt => opt.Ignore())
                        .ForMember(m => m.ContactPerson, opt => opt.Ignore())
                        .ForMember(m => m.CreatedBy, opt => opt.Ignore())
                        .ForMember(m => m.DeliveryPlace, opt => opt.Ignore())
                        .ForMember(m => m.PaymentType, opt => opt.Ignore())
                        .ForMember(m => m.SendBy, opt => opt.Ignore())
                        .ForMember(m => m.WhoDeliveredPersonPerson, opt => opt.Ignore())
                        .ForMember(m => m.DishPackages, opt => opt.MapFrom(a => a.DishPackages.ToList()));



                        cfg.CreateMap<ServiceDataContracts.OrderToGo, Entities.OrderToGo>()
                        .ForMember(m => m.DishPackages, opt => opt.MapFrom(a=>a.DishPackages.ToList()))
                        .ReverseMap();


                        cfg.CreateMap<ServiceDataContracts.OrderCustomer, Entities.OrderCustomer>()
                        .ForMember(m => m.Phones, opt => opt.Ignore())
                        .ForMember(m => m.Addresses, opt => opt.Ignore());

                        cfg.CreateMap<Entities.OrderCustomer, ServiceDataContracts.OrderCustomer>()
.ForMember(m => m.Phones, opt => opt.Ignore())
.ForMember(m => m.Addresses, opt => opt.Ignore())
.ForMember(m => m.OrderCustomerInfo, opt => opt.Ignore());

                        cfg.CreateMap<ServiceDataContracts.OrderCustomerPhone, Entities.OrderCustomerPhone>()
                        .ReverseMap();
                        cfg.CreateMap<ServiceDataContracts.OrderCustomerAddress, Entities.OrderCustomerAddress>()
                        .ReverseMap();
                        cfg.CreateMap<ServiceDataContracts.OrderCustomerInfo, Entities.OrderCustomerInfo>()
                        .ReverseMap();
                    });


                    MapperInited = true;
                    log.Debug("Mapper.Initialize ok");
                }
                catch (Exception e)
                {
                    log.Error("Mapper.Initialize error " + e.Message);
                }
            }
            userService = new UserService(new AlohaDb());
            orderService = new OrderService(new AlohaDb());
            airCompanyService = new AirCompanyService(new AlohaDb());
            userGroupService = new UserGroupService(new AlohaDb());
            contactPersonService = new ContactPersonService(new AlohaDb());

            curierService = new CurierService(new AlohaDb());
            deliveryPersonService = new DeliveryPersonService(new AlohaDb());
            deliveryPlaceService = new DeliveryPlaceService(new AlohaDb());
            marketingChannelService = new MarketingChannelService(new AlohaDb());
            dishPackageFlightOrderService = new DishPackageFlightOrderService(new AlohaDb());
            dishPackageToGoOrderService = new DishPackageToGoOrderService(new AlohaDb());
            dishService = new DishService(new AlohaDb());
            driverService = new DriverService(new AlohaDb());
            itemLabelInfoService = new ItemLabelInfoService(new AlohaDb());
            orderCustomerService = new OrderCustomerService(new AlohaDb());
            orderFlightService = new OrderFlightService(new AlohaDb());
            orderToGoService = new OrderToGoService(new AlohaDb());
            discountService = new DiscountService(new AlohaDb());
            alertService = new AlertService(new AlohaDb());
            paymentService = new PaymentService(new AlohaDb());
            paymentGroupService = new PaymentGroupService(new AlohaDb());
            dishLogicGroupService = new DishLogicGroupService(new AlohaDb());
            dishKitchenGroupService = new DishKitchenGroupService(new AlohaDb());
            logItemService = new LogItemService(new AlohaDb());
            orderCustomerAddressService = new OrderCustomerAddressService(new AlohaDb());
            orderCustomerPhoneService = new OrderCustomerPhoneService(new AlohaDb());
            updaterService = new UpdaterService(new AlohaDb());

            analitikService = new AnalitikService(new AlohaDb());
            externalFromSiteOrdersService = new ExternalFromSiteOrdersService(new AlohaDb());
            externalFromDeleveryClubService = new ExternalFromDeleveryClubService(new AlohaDb());
            externalFromYandexService = new ExternalFromYandexService(new AlohaDb());


        }

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        #region User
        public OperationResult CreateUser(UserInfo userInfo)
        {
            return userService.CreateUser(userInfo);
        }

        public OperationResultValue<ServiceDataContracts.User> GetUser(long userId)
        {
            return userService.GetUser(userId);
        }

        public OperationResult UpdateUser(ServiceDataContracts.User user)
        {
            return userService.UpdateUser(user);
        }

        public OperationResult DeleteUser(long userId)
        {
            return userService.DeleteUser(userId);
        }

        public OperationResultValue<ServiceDataContracts.User> LoginUser(string loginOrEmail, string password)
        {
            return userService.LoginUser(loginOrEmail, password);
        }

        public OperationResultValue<List<ServiceDataContracts.User>> GetUserList()
        {
            return userService.GetUserList();
        }
        #endregion User

        #region Air Company
        public OperationResult CreateAirCompany(ServiceDataContracts.AirCompany airCompany)
        {
            log.Info("CreateAirCompany started.");
            var createdAirCompanyId = airCompanyService.CreateAirCompany(airCompany);

            return new OperationResult
            {
                Success = true,
                CreatedObjectId = createdAirCompanyId
            };
        }

        public OperationResultValue<ServiceDataContracts.AirCompany> GetAirCompany(long airCompanyId)
        {
            var airCompany = airCompanyService.GetAirCompany(airCompanyId);

            return new OperationResultValue<ServiceDataContracts.AirCompany>
            {
                Success = true,
                Result = airCompany
            };
        }

        public OperationResult UpdateAirCompany(ServiceDataContracts.AirCompany airCompany)
        {
            airCompanyService.UpdateAirCompany(airCompany);

            return new OperationResult
            {
                Success = true,
                CreatedObjectId = airCompany.Id
            };
        }

        public OperationResult DeleteAirCompany(long airCompanyId)
        {
            airCompanyService.DeleteAirCompany(airCompanyId);

            return new OperationResult
            {
                Success = true,
                CreatedObjectId = airCompanyId
            };
        }

        public OperationResultValue<List<ServiceDataContracts.AirCompany>> GetAirCompanyList()
        {
            var result = airCompanyService.GetAirCompanyList();
            return new OperationResultValue<List<ServiceDataContracts.AirCompany>>
            {
                Success = true,
                Result = result
            };
        }
        #endregion Air Company

        #region User Groups
        public OperationResult CreateUserGroup(ServiceDataContracts.UserGroup userGroup)
        {
            return userGroupService.CreateUserGroup(userGroup);
        }

        public OperationResult DeleteUserGroup(long userGroupId)
        {
            return userGroupService.DeleteUserGroup(userGroupId);
        }

        public OperationResultValue<List<ServiceDataContracts.UserGroup>> GetUserGroupList()
        {
            return userGroupService.GetUserGroupList();
        }

        public OperationResult CreateUserFunc(ServiceDataContracts.UserFunc userFunc)
        {
            return userGroupService.CreateUserFunc(userFunc);
        }

        public OperationResultValue<ServiceDataContracts.UserFunc> GetUserFunc(long userFuncId)
        {
            return userGroupService.GetUserFunc(userFuncId);
        }

        public OperationResult UpdateUserFunc(ServiceDataContracts.UserFunc userFunc)
        {
            return userGroupService.UpdateUserFunc(userFunc);
        }

        public OperationResult DeleteUserFunc(long userFuncId)
        {
            return userGroupService.DeleteUserFunc(userFuncId);
        }

        public OperationResultValue<List<ServiceDataContracts.UserFunc>> GetUserFuncList()
        {
            return userGroupService.GetUserFuncList();
        }

        public OperationResult AddUserToGroup(long userId, long usrGroupId)
        {
            return userGroupService.AddUserToGroup(userId, usrGroupId);
        }

        public OperationResult RemoveUserFromGroup(long userId, long usrGroupId)
        {
            return userGroupService.RemoveUserFromGroup(userId, usrGroupId);
        }

        public OperationResultValue<List<ServiceDataContracts.UserGroup>> GetAllUserGroups(long userId)
        {
            return userGroupService.GetAllUserGroups(userId);
        }

        public OperationResultValue<List<ServiceDataContracts.UserGroupAccess>> GetAllGroupFuncs(long groupId)
        {
            return userGroupService.GetAllGroupFuncs(groupId);
        }

        public OperationResult AddFuncToGroup(long funcId, long usrGroupId, FuncAccessType accessType)
        {
            return userGroupService.AddFuncToGroup(funcId, usrGroupId, accessType);
        }

        public OperationResult RemoveFuncFromGroup(long funcId, long usrGroupId)
        {
            return userGroupService.RemoveFuncFromGroup(funcId, usrGroupId);
        }

        public OperationResult RemoveAllFuncsFromDb()
        {
            return userGroupService.RemoveAllFuncsFromDb();
        }

        public OperationResult RemoveAllUserGroupsFromDb()
        {
            return userGroupService.RemoveAllUserGroupsFromDb();
        }

        #endregion UserGroups

        #region Contact Person

        public OperationResult CreateContactPerson(ServiceDataContracts.ContactPerson contactPerson)
        {
            return contactPersonService.CreateContactPerson(contactPerson);
        }

        public OperationResultValue<ServiceDataContracts.ContactPerson> GetContactPerson(long contactPersonId)
        {
            return contactPersonService.GetContactPerson(contactPersonId);
        }

        public OperationResult UpdateContactPerson(ServiceDataContracts.ContactPerson contactPerson)
        {
            return contactPersonService.UpdateContactPerson(contactPerson);
        }

        public OperationResult DeleteContactPerson(long contactPersonId)
        {
            return contactPersonService.DeleteContactPerson(contactPersonId);
        }

        public OperationResultValue<List<ServiceDataContracts.ContactPerson>> GetContactPersonList()
        {
            return contactPersonService.GetContactPersonList();
        }

        #endregion Contact Person

        #region Curier
        public OperationResult CreateCurier(ServiceDataContracts.Curier curier)
        {
            return curierService.CreateCurier(curier);
        }

        public OperationResultValue<ServiceDataContracts.Curier> GetCurier(long curierId)
        {
            return curierService.GetCurier(curierId);
        }

        public OperationResult UpdateCurier(ServiceDataContracts.Curier curier)
        {
            return curierService.UpdateCurier(curier);
        }

        public OperationResult DeleteCurier(long curierId)
        {
            return curierService.DeleteCurier(curierId);
        }

        public OperationResultValue<List<ServiceDataContracts.Curier>> GetCurierList()
        {
            return curierService.GetCurierList();
        }
        #endregion Curier

        #region ItemLabelInfo
        public OperationResult CreateItemLabelInfo(ServiceDataContracts.ItemLabelInfo itemLabelInfo)
        {
            return itemLabelInfoService.CreateItemLabelInfo(itemLabelInfo);
        }

        public OperationResultValue<ServiceDataContracts.ItemLabelInfo> GetItemLabelInfo(long itemLabelInfoId)
        {
            return itemLabelInfoService.GetItemLabelInfo(itemLabelInfoId);
        }

        public OperationResult UpdateItemLabelInfo(ServiceDataContracts.ItemLabelInfo itemLabelInfo)
        {
            return itemLabelInfoService.UpdateItemLabelInfo(itemLabelInfo);
        }

        public OperationResult DeleteItemLabelInfo(long itemLabelInfoId)
        {
            return itemLabelInfoService.DeleteItemLabelInfo(itemLabelInfoId);
        }

        public OperationResultValue<List<ServiceDataContracts.ItemLabelInfo>> GetItemLabelInfoList()
        {
            return itemLabelInfoService.GetItemLabelInfoList();
        }
        #endregion ItemLabelInfo

        #region Delivery Person

        public OperationResult CreateDeliveryPerson(ServiceDataContracts.DeliveryPerson deliveryPerson)
        {
            return deliveryPersonService.CreateDeliveryPerson(deliveryPerson);
        }

        public OperationResultValue<ServiceDataContracts.DeliveryPerson> GetDeliveryPerson(long deliveryPersonId)
        {
            return deliveryPersonService.GetDeliveryPerson(deliveryPersonId);
        }

        public OperationResult UpdateDeliveryPerson(ServiceDataContracts.DeliveryPerson deliveryPerson)
        {
            return deliveryPersonService.UpdateDeliveryPerson(deliveryPerson);
        }

        public OperationResult DeleteDeliveryPerson(long deliveryPersonId)
        {
            return deliveryPersonService.DeleteDeliveryPerson(deliveryPersonId);
        }

        public OperationResultValue<List<ServiceDataContracts.DeliveryPerson>> GetDeliveryPersonList()
        {
            return deliveryPersonService.GetDeliveryPersonList();
        }

        #endregion Delivery Person

        #region Delivery Place

        public OperationResult CreateDeliveryPlace(ServiceDataContracts.DeliveryPlace deliveryPlace)
        {
            return deliveryPlaceService.CreateDeliveryPlace(deliveryPlace);
        }

        public OperationResultValue<ServiceDataContracts.DeliveryPlace> GetDeliveryPlace(long deliveryPlaceId)
        {
            return deliveryPlaceService.GetDeliveryPlace(deliveryPlaceId);
        }

        public OperationResult UpdateDeliveryPlace(ServiceDataContracts.DeliveryPlace deliveryPlace)
        {
            return deliveryPlaceService.UpdateDeliveryPlace(deliveryPlace);
        }

        public OperationResult DeleteDeliveryPlace(long deliveryPlaceId)
        {
            return deliveryPlaceService.DeleteDeliveryPlace(deliveryPlaceId);
        }

        public OperationResultValue<List<ServiceDataContracts.DeliveryPlace>> GetDeliveryPlaceList()
        {
            return deliveryPlaceService.GetDeliveryPlaceList();
        }

        #endregion Delivery Place

        #region Dish

        public OperationResult CreateDish(ServiceDataContracts.Dish dish)
        {
            return dishService.CreateDish(dish);
        }

        public OperationResultValue<ServiceDataContracts.Dish> GetDish(long dishId)
        {
            return dishService.GetDish(dishId);
        }

        public OperationResult UpdateDish(ServiceDataContracts.Dish dish)
        {
            return dishService.UpdateDish(dish);
        }

        public OperationResult DeleteDish(long dishId)
        {
            return dishService.DeleteDish(dishId);
        }

        public OperationResultValue<List<ServiceDataContracts.Dish>> GetDishList()
        {
            return dishService.GetDishList();
        }

        public OperationResultValue<List<ServiceDataContracts.Dish>> GetDishListIntelliSense(string startsWith)
        {
            return dishService.GetDishListIntelliSense(startsWith);
        }

        public OperationResultValue<List<ServiceDataContracts.Dish>> GetDishPage(DishFilter filter, PageInfo page)
        {
            return dishService.GetDishPage(filter, page);
        }


        public OperationResult SetExternalLink(long dishId, int marketingChanel, long externalId)
        {
            return dishService.SetExternalLink(dishId, marketingChanel, externalId);
        }


        #endregion Dish

        #region Driver

        public OperationResult CreateDriver(ServiceDataContracts.Driver driver)
        {
            return driverService.CreateDriver(driver);
        }

        public OperationResultValue<ServiceDataContracts.Driver> GetDriver(long driverId)
        {
            return driverService.GetDriver(driverId);
        }

        public OperationResult UpdateDriver(ServiceDataContracts.Driver driver)
        {
            return driverService.UpdateDriver(driver);
        }

        public OperationResult DeleteDriver(long driverId)
        {
            return driverService.DeleteDriver(driverId);
        }

        public OperationResultValue<List<ServiceDataContracts.Driver>> GetDriverList()
        {
            return driverService.GetDriverList();
        }

        #endregion Driver

        #region Order Flight DishPackage

        public OperationResult CreateDishPackageFlightOrder(ServiceDataContracts.DishPackageFlightOrder dishPackageFlightOrder)
        {
            return dishPackageFlightOrderService.CreateDishPackageFlightOrder(dishPackageFlightOrder);
        }

        public OperationResultValue<ServiceDataContracts.DishPackageFlightOrder> GetDishPackageFlightOrder(long dishPackageFlightOrderId)
        {
            return dishPackageFlightOrderService.GetDishPackageFlightOrder(dishPackageFlightOrderId);
        }

        public OperationResult UpdateDishPackageFlightOrder(ServiceDataContracts.DishPackageFlightOrder dishPackageFlightOrder)
        {
            return dishPackageFlightOrderService.UpdateDishPackageFlightOrder(dishPackageFlightOrder);
        }

        public OperationResult DeleteDishPackageFlightOrder(long dishPackageFlightOrderId)
        {
            return dishPackageFlightOrderService.DeleteDishPackageFlightOrder(dishPackageFlightOrderId);
        }

        public OperationResultValue<List<ServiceDataContracts.DishPackageFlightOrder>> GetDishPackageFlightOrderList(long orderFlightId)
        {
            return dishPackageFlightOrderService.GetDishPackageFlightOrderList(orderFlightId);
        }

        #endregion Order Flight DishPackage

        #region Order ToGo DishPackage

        public OperationResult CreateDishPackageToGoOrder(ServiceDataContracts.DishPackageToGoOrder dishPackageToGoOrder)
        {
            return dishPackageToGoOrderService.CreateDishPackageToGoOrder(dishPackageToGoOrder);
        }

        public OperationResultValue<ServiceDataContracts.DishPackageToGoOrder> GetDishPackageToGoOrder(long dishPackageToGoOrderId)
        {
            return dishPackageToGoOrderService.GetDishPackageToGoOrder(dishPackageToGoOrderId);
        }

        public OperationResult UpdateDishPackageToGoOrder(ServiceDataContracts.DishPackageToGoOrder dishPackageToGoOrder)
        {
            return dishPackageToGoOrderService.UpdateDishPackageToGoOrder(dishPackageToGoOrder);
        }

        public OperationResult DeleteDishPackageToGoOrder(long dishPackageToGoOrderId)
        {
            return dishPackageToGoOrderService.DeleteDishPackageToGoOrder(dishPackageToGoOrderId);
        }

        public OperationResultValue<List<ServiceDataContracts.DishPackageToGoOrder>> GetDishPackageToGoOrderList(long orderToGoId)
        {
            return dishPackageToGoOrderService.GetDishPackageToGoOrderList(orderToGoId);
        }

        #endregion FlightOrder DishPackage

        #region Order Customer

        public OperationResult CreateOrderCustomer(ServiceDataContracts.OrderCustomer orderCustomer)
        {
            return orderCustomerService.CreateOrderCustomer(orderCustomer);
        }
        /*
        public OperationResultValue<ServiceDataContracts.OrderCustomer> GetOrderCustomer(long orderCustomerId)
        {
            return orderCustomerService.GetOrderCustomer(orderCustomerId);
        }
        */
        public OperationResult UpdateOrderCustomer(ServiceDataContracts.OrderCustomer orderCustomer)
        {
            return orderCustomerService.UpdateOrderCustomer(orderCustomer);
        }

        public OperationResult DeleteOrderCustomer(long orderCustomerId)
        {
            return orderCustomerService.DeleteOrderCustomer(orderCustomerId);
        }
        /*
        public OperationResultValue<List<ServiceDataContracts.OrderCustomer>> GetOrderCustomerList()
        {
            return orderCustomerService.GetOrderCustomerList();
        }
        */
        public OperationResultValue<List<ServiceDataContracts.OrderCustomer>> GetOrderCustomerList2()
        {
            return orderCustomerService.GetOrderCustomerList2();
        }

        public OperationResultValue<ServiceDataContracts.OrderCustomer> GetOrderCustomer2(long orderCustomerId)
        {
            return orderCustomerService.GetOrderCustomer2(orderCustomerId);
        }


        public OperationResult MergeCustomers(ServiceDataContracts.OrderCustomer orderCustomer1, ServiceDataContracts.OrderCustomer orderCustomer2)
        {
            return orderCustomerService.MergeCustomers(orderCustomer1, orderCustomer2);
        }

        #endregion Order Customer

        #region Order Flight

        public bool InsertOrderFlightFromAloha(ServiceDataContracts.OrderFlight orderFlight)
        {
            return orderFlightService.InsertOrderFlightFromAloha(orderFlight);
        }


        public OperationResult CreateOrderFlight(ServiceDataContracts.OrderFlight orderFlight)
        {
            return orderFlightService.CreateOrderFlight(orderFlight);
        }

        public OperationResult CreateOrderFlightWithPackage(ServiceDataContracts.OrderFlight orderFlight)
        {
            return orderFlightService.CreateOrderFlightWithPackage(orderFlight);
        }

        public OperationResultValue<ServiceDataContracts.OrderFlight> GetOrderFlight(long orderFlightId)
        {
            return orderFlightService.GetOrderFlight(orderFlightId);
        }
        /*
        public OperationResultValue<ServiceDataContracts.OrderFlight> GetOrderFlightByCode(long orderCode)
        {
            return orderFlightService.GetOrderFlightByCode(orderCode);
        }
        */
        public OperationResultValue<ServiceDataContracts.OrderFlight> UpdateOrderFlight(ServiceDataContracts.OrderFlight orderFlight, long userId)
        {
            return orderFlightService.UpdateOrderFlight(orderFlight, userId);
        }

        public OperationResult UpdateOrderFlight2(ServiceDataContracts.OrderFlight orderFlight, long userId)
        {
            return orderFlightService.UpdateOrderFlight2(orderFlight, userId);
        }

        public OperationResult DeleteOrderFlight(long orderFlightId)
        {
            return orderFlightService.DeleteOrderFlight(orderFlightId);
        }

        public OperationResultValue<List<ServiceDataContracts.OrderFlight>> GetOrderFlightList(OrderFlightFilter filter, PageInfo page)
        {
            return orderFlightService.GetOrderFlightList(filter, page);
        }

        public OperationResult SetToFlySHValue(bool value, long orderId)
        {
            return orderFlightService.SetSHValue(value, orderId);
        }
        public byte[] ExportToExcel(long orderId)
        {
            return orderFlightService.ExportToExcel(orderId);
        }
        #endregion Order Flight

        #region Order To Go

        public OperationResult CreateOrderToGo(ServiceDataContracts.OrderToGo orderToGo)
        {
            return orderToGoService.CreateOrderToGo(orderToGo);
        }


        public OperationResult CreateOrderToGoWithPackage(ServiceDataContracts.OrderToGo orderToGo)
        {
            return orderToGoService.CreateOrderToGoWithPackage(orderToGo);
        }
        

        public OperationResultValue<ServiceDataContracts.OrderToGo> GetOrderToGo(long orderToGoId)
        {
            return orderToGoService.GetOrderToGo(orderToGoId);
        }

        public OperationResult UpdateOrderToGo(ServiceDataContracts.OrderToGo orderToGo)
        {
            return orderToGoService.UpdateOrderToGo(orderToGo);
        }

        public OperationResult DeleteOrderToGo(long orderToGoId)
        {
            return orderToGoService.DeleteOrderToGo(orderToGoId);
        }

        public OperationResultValue<List<ServiceDataContracts.OrderToGo>> GetOrderToGoList(OrderToGoFilter filter, PageInfo page)
        {
            return orderToGoService.GetOrderToGoList(filter, page);
        }

        
        public OperationResult SetToGoSHValue(bool value, long orderId)
        {
            return orderToGoService.SetSHValue(value, orderId);
        }


        #endregion Order To Go

        #region FlightCode IntelliSence Helper

        public OperationResultValue<List<string>> GetFlightCodes(long airCompanyId, string startsWith)
        {
            return airCompanyService.GetFlightCodes(airCompanyId, startsWith);
        }

        #endregion FlightCode IntelliSence Helper

        #region Discounts


        public OperationResult CreateDiscountRange(ServiceDataContracts.DiscountRange discountRange)
        {
            return new OperationResult
            {
                Success = true,
                CreatedObjectId = discountService.CreateDiscountRange(discountRange)
            };
        }

        public OperationResultValue<ServiceDataContracts.DiscountRange> GetDiscountRange(long discountRangeId)
        {
            var discountRange = discountService.GetDiscountRange(discountRangeId);

            return new OperationResultValue<ServiceDataContracts.DiscountRange>
            {
                Success = true,
                Result = discountRange
            };
        }

        public OperationResult UpdateDiscountRange(ServiceDataContracts.DiscountRange discountRange)
        {
            discountService.UpdateDiscountRange(discountRange);

            return new OperationResult
            {
                Success = true,
                CreatedObjectId = discountRange.Id
            };
        }

        public OperationResult DeleteDiscountRange(long discountRangeId)
        {
            discountService.DeleteDiscountRange(discountRangeId);

            return new OperationResult
            {
                Success = true,
                CreatedObjectId = discountRangeId
            };
        }

        public OperationResultValue<List<ServiceDataContracts.DiscountRange>> GetDiscountRangesList()
        {
            var result = discountService.GetDiscountRangesList();
            return new OperationResultValue<List<ServiceDataContracts.DiscountRange>>
            {
                Success = true,
                Result = result
            };
        }

        public OperationResult CreateDiscount(ServiceDataContracts.Discount discount)
        {
            return new OperationResult
            {
                Success = true,
                CreatedObjectId = discountService.CreateDiscount(discount)
            };
        }

        public OperationResultValue<ServiceDataContracts.Discount> GetDiscount(long discountId)
        {
            var discount = discountService.GetDiscount(discountId);

            return new OperationResultValue<ServiceDataContracts.Discount>
            {
                Success = true,
                Result = discount
            };
        }

        public OperationResult UpdateDiscount(ServiceDataContracts.Discount discount)
        {
            discountService.UpdateDiscount(discount);

            return new OperationResult
            {
                Success = true,
                CreatedObjectId = discount.Id
            };
        }

        public OperationResult DeleteDiscount(long discountId)
        {
            discountService.DeleteDiscount(discountId);

            return new OperationResult
            {
                Success = true,
                CreatedObjectId = discountId
            };
        }

        public OperationResultValue<List<ServiceDataContracts.Discount>> GetDiscountList()
        {
            var result = discountService.GetDiscountList();
            return new OperationResultValue<List<ServiceDataContracts.Discount>>
            {
                Success = true,
                Result = result
            };
        }

        public OperationResult AddRangeToDiscount(long discountId, long discountRangeId)
        {
            discountService.AddRangeToDiscount(discountId, discountRangeId);

            return new OperationResult
            {
                Success = true,
                CreatedObjectId = 0
            };
        }

        public OperationResult RemoveRangeFromDiscount(long discountId, long discountRangeId)
        {
            discountService.RemoveRangeFromDiscount(discountId, discountRangeId);

            return new OperationResult
            {
                Success = true,
                CreatedObjectId = 0
            };
        }

        #endregion Discounts

        #region Alerts

        public OperationResult CreateAlert(ServiceDataContracts.Alert alert)
        {
            return new OperationResult
            {
                CreatedObjectId = alertService.CreateAlert(alert),
                Success = true
            };
        }

        public OperationResultValue<ServiceDataContracts.Alert> GetAlert(long alertId)
        {
            return new OperationResultValue<ServiceDataContracts.Alert>
            {
                Result = alertService.GetAlert(alertId),
                Success = true
            };
        }

        public OperationResult UpdateAlert(ServiceDataContracts.Alert alert)
        {
            alertService.UpdateAlert(alert);

            return new OperationResult
            {
                CreatedObjectId = alert.Id,
                Success = true
            };
        }

        public OperationResult DeleteAlert(long alertId)
        {
            alertService.DeleteAlert(alertId);

            return new OperationResult
            {
                CreatedObjectId = alertId,
                Success = true
            };
        }

        public OperationResultValue<List<ServiceDataContracts.Alert>> GetAlertList()
        {
            return new OperationResultValue<List<ServiceDataContracts.Alert>>
            {
                Result = alertService.GetAlertList(),
                Success = true
            };
        }

        public OperationResultValue<List<ServiceDataContracts.Alert>> GetAlertListByDateRange(DateTime from, DateTime to)
        {
            return new OperationResultValue<List<ServiceDataContracts.Alert>>
            {
                Result = alertService.GetAlertListByDateRange(from, to),
                Success = true
            };
        }

        #endregion Alerts

        #region Payments

        public OperationResult CreatePayment(ServiceDataContracts.Payment payment)
        {
            return paymentService.CreatePayment(payment);
        }

        public OperationResultValue<ServiceDataContracts.Payment> GetPayment(long paymentId)
        {
            return paymentService.GetPayment(paymentId);
        }

        public OperationResult UpdatePayment(ServiceDataContracts.Payment payment)
        {
            return paymentService.UpdatePayment(payment);
        }

        public OperationResultValue<List<ServiceDataContracts.Payment>> GetPaymentList()
        {
            return paymentService.GetPaymentList();
        }

        public OperationResult DeletePayment(long paymentId)
        {
            return paymentService.DeletePayment(paymentId);
        }

        #endregion


        #region PaymentGroups

        public OperationResult CreatePaymentGroup(ServiceDataContracts.PaymentGroup payment)
        {
            return paymentGroupService.CreatePaymentGroup(payment);
        }

        public OperationResultValue<ServiceDataContracts.PaymentGroup> GetPaymentGroup(long paymentId)
        {
            return paymentGroupService.GetPaymentGroup(paymentId);
        }

        public OperationResult UpdatePaymentGroup(ServiceDataContracts.PaymentGroup payment)
        {
            return paymentGroupService.UpdatePaymentGroup(payment);
        }

        public OperationResultValue<List<ServiceDataContracts.PaymentGroup>> GetPaymentGroupList()
        {
            return paymentGroupService.GetPaymentGroupList();
        }

        public OperationResult DeletePaymentGroup(long paymentId)
        {
            return paymentGroupService.DeletePaymentGroup(paymentId);
        }

        #endregion


        #region DishKitchenGroup

        public OperationResult CreateDishKitchenGroup(ServiceDataContracts.DishKitchenGroup dishKitchenGroup)
        {
            return dishKitchenGroupService.CreateDishKitchenGroup(dishKitchenGroup);
        }

        public OperationResultValue<ServiceDataContracts.DishKitchenGroup> GetDishKitchenGroup(long dishKitchenGroupId)
        {
            return dishKitchenGroupService.GetDishKitchenGroup(dishKitchenGroupId);
        }

        public OperationResult UpdateDishKitchenGroup(ServiceDataContracts.DishKitchenGroup dishKitchenGroup)
        {
            return dishKitchenGroupService.UpdateDishKitchenGroup(dishKitchenGroup);
        }

        public OperationResult DeleteDishKitchenGroup(long dishKitchenGroupId)
        {
            return dishKitchenGroupService.DeleteDishKitchenGroup(dishKitchenGroupId);
        }

        public OperationResultValue<List<ServiceDataContracts.DishKitchenGroup>> GetDishKitсhenGroupsList()
        {
            return dishKitchenGroupService.GetDishKitсhenGroupsList();
        }

        #endregion DishKitchenGroup

        #region DishLogicGroup

        public OperationResult CreateDishLogicGroup(ServiceDataContracts.DishLogicGroup dishLogicGroup)
        {
            return dishLogicGroupService.CreateDishLogicGroup(dishLogicGroup);
        }

        public OperationResultValue<ServiceDataContracts.DishLogicGroup> GetDishLogicGroup(long dishLogicGroupId)
        {
            return dishLogicGroupService.GetDishLogicGroup(dishLogicGroupId);
        }

        public OperationResult UpdateDishLogicGroup(ServiceDataContracts.DishLogicGroup dishLogicGroup)
        {
            return dishLogicGroupService.UpdateDishLogicGroup(dishLogicGroup);
        }

        public OperationResult DeleteDishLogicGroup(long dishLogicGroupId)
        {
            return dishLogicGroupService.DeleteDishLogicGroup(dishLogicGroupId);
        }

        public OperationResultValue<List<ServiceDataContracts.DishLogicGroup>> GetDishLogicGroupsList()
        {
            return dishLogicGroupService.GetDishLogicGroupsList();
        }

        #endregion DishLogicGroup

        #region reports
        public OperationResultValue<decimal> GetSumByDates(DateTime dateFrom, DateTime dateTo, OrderStatus status)
        {
            return orderFlightService.GetSumByDates(dateFrom, dateTo, status);
        }
        #endregion

        #region Logs

        public OperationResultValue<List<ServiceDataContracts.LogItem>> GetLogItems(DateTime dateFrom, DateTime dateTo)
        {
            return logItemService.GetLogItems(dateFrom, dateTo);
        }
        #endregion Logs


        public OperationResult CreateMarketingChannel(ServiceDataContracts.MarketingChannel MarketingChannel)
        {
            return marketingChannelService.CreateMarketingChannel(MarketingChannel);
        }

        public OperationResultValue<ServiceDataContracts.MarketingChannel> GetMarketingChannel(long MarketingChannelId)
        {
            return marketingChannelService.GetMarketingChannel(MarketingChannelId);
        }

        public OperationResult UpdateMarketingChannel(ServiceDataContracts.MarketingChannel MarketingChannel)
        {
            return marketingChannelService.UpdateMarketingChannel(MarketingChannel);
        }

        public OperationResult DeleteMarketingChannel(long MarketingChannelId)
        {
            return marketingChannelService.DeleteMarketingChannel(MarketingChannelId);
        }

        public OperationResultValue<List<ServiceDataContracts.MarketingChannel>> GetMarketingChannelList()
        {
            return marketingChannelService.GetMarketingChannelList();
        }

        #region FR
        public OperationResultValue<List<ServiceDataContracts.OrderFlight>> GetOrderFlightListNeedToFR()
        {
            return orderFlightService.GetOrderFlightListNeedToFR();
        }

        public OperationResultValue<List<ServiceDataContracts.OrderToGo>> GetOrderToGoListNeedToFR()
        {
            return orderToGoService.GetOrderToGoListNeedToFR();
        }
        #endregion FR

        #region OrderCustomerAddress
        public OperationResult CreateOrderCustomerAddress(ServiceDataContracts.OrderCustomerAddress orderCustomerAddress)
        {
            return orderCustomerAddressService.CreateAddress(orderCustomerAddress);
        }

        public OperationResultValue<List<ServiceDataContracts.OrderCustomerAddress>> GetOrderCustomerAddressList()
        {
            return orderCustomerAddressService.GetAddressList();
        }

        public OperationResult UpdateOrderCustomerAddress(ServiceDataContracts.OrderCustomerAddress orderCustomerAddress)
        {
            return orderCustomerAddressService.UpdateAddress(orderCustomerAddress);
        }

        public OperationResult DeleteOrderCustomerAddress(long orderCustomerAddressId)
        {
            return orderCustomerAddressService.DeleteAddress(orderCustomerAddressId);
        }

        public OperationResultValue<ServiceDataContracts.OrderCustomerAddress> GetOrderCustomerAddress(long orderCustomerAddessId)
        {
            return orderCustomerAddressService.GetOrderCustomerAddress(orderCustomerAddessId);
        }

        #endregion OrderCustomerAddress

        #region OrderCustomerPhone

        public OperationResult CreateOrderCustomerPhone(ServiceDataContracts.OrderCustomerPhone orderCustomerPhone)
        {
            return orderCustomerPhoneService.CreatePhone(orderCustomerPhone);
        }

        public OperationResultValue<List<ServiceDataContracts.OrderCustomerPhone>> GetOrderCustomerPhoneList()
        {
            return orderCustomerPhoneService.GetPhoneList();
        }

        public OperationResult UpdateOrderCustomerPhone(ServiceDataContracts.OrderCustomerPhone orderCustomerPhone)
        {
            return orderCustomerPhoneService.UpdatePhone(orderCustomerPhone);
        }

        public OperationResult DeleteOrderCustomerPhone(long orderCustomerPhoneId)
        {
            return orderCustomerPhoneService.DeletePhone(orderCustomerPhoneId);
        }

        public OperationResultValue<ServiceDataContracts.OrderCustomerPhone> GetOrderCustomerPhone(long orderCustomerPhoneId)
        {
            return orderCustomerPhoneService.GetOrderCustomerPhone(orderCustomerPhoneId);
        }

        #endregion OrderCustomerPhone



        #region Updater
        
        public OperationResultValue<UpdateResult> GetUpdatesForSession(DateTime lastUpdateTime, DateTime DataTime,Guid session)
        {
            return updaterService.GetUpdatesForSession(lastUpdateTime, DataTime, session);
        }
        public OperationResultValue<UpdateResult> GetServerTime()
        {
            return updaterService.GetServerTime();
        }


        public OperationResultValue<UpdateResult> GetUpdatesForSessionTest()
        {
            return updaterService.GetUpdatesForSessionTest();
        }

        #endregion


        #region Analitic

        public OperationResultValue<AnalitikData> GetLTVValues(DateTime sDate, DateTime eDate)
        {
            return analitikService.GetLTVValues(sDate, eDate);
        }
        public OperationResultValue<AnalitikData> GetLTVValues2(DateTime sDate, DateTime eDate)
        {
            return analitikService.GetLTVValues2(sDate, eDate);
        }


        public OperationResultValue<List<AnalitikOrderData>> GetAllToGoOrdersData(DateTime sDate, DateTime eDate)
        {
            return analitikService.GetAllToGoOrdersData(sDate, eDate);

        }
        #endregion


        #region External

        public OperationResult ExternalCreateSiteToGoOrder(ExternalToGoOrder order)
        {
            return externalFromSiteOrdersService.CreateToGoOrder(order);
        }
        public OperationResult ExternalCreateDeleveryClubToGoOrder(ExternalToGoOrder order)
        {
            //return externalFromDeleveryClubService.CreateToGoOrder(order);
            return new OperationResult()
            {
                Success = true
            };
        }
        public OperationResult ExternalCreateYandexToGoOrder(ExternalToGoOrder order)
        {
            return externalFromYandexService.CreateToGoOrder(order);
        }


        public OperationResultValue<List<long>> ExternalGetSiteOrderList()
        {
            return externalFromSiteOrdersService.GetExternalOrderList();
        }
        public OperationResultValue<List<long>> ExternalGetDeleveryClubOrderList()
        {
            return externalFromDeleveryClubService.GetExternalOrderList();
        }
        public OperationResultValue<List<long>> ExternalGetYandexOrderList()
        {
            return externalFromYandexService.GetExternalOrderList();
        }


        #endregion

    }
}
