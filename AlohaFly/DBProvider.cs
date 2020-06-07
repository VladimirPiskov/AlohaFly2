using AlohaFly.Utils;
using AlohaService;
//using AlohaFly.DBService;
using AlohaService.ServiceDataContracts;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace AlohaFly
{
    public static class DBProvider
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        public static IAlohaService GetClient()
        {

            return Client;
        }

        public static string TestStr = "";
        public static IAlohaService Client
        {
            get
            {
                try
                {


#if DEBUG


                   var address = new EndpointAddress(new Uri(Properties.Settings.Default.DBAddressRelease));
                   /*
                    var address = new EndpointAddress(new Uri(Properties.Settings.Default.DBAddress));
                    TestStr = "test";
                    */
#else
                    var address = new EndpointAddress(new Uri(Properties.Settings.Default.DBAddressRelease));
#endif

                    /*
                    var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential)
                    {
                        MaxReceivedMessageSize = 1000000000,
                        SendTimeout = new TimeSpan(0, 10, 0)
                    };
                    */

                    BinaryMessageEncodingBindingElement encoding = new BinaryMessageEncodingBindingElement();
                    HttpsTransportBindingElement transport = new HttpsTransportBindingElement();
                    transport.MaxReceivedMessageSize = (long)Math.Pow(10,9);
                    transport.MaxBufferSize = (int)Math.Pow(10, 9);
                    transport.MaxBufferPoolSize = (long)Math.Pow(10, 9);


                    Binding binding = new CustomBinding(encoding, transport);
                    


                    var channelFactory = new System.ServiceModel.ChannelFactory<AlohaService.IAlohaService>(binding, address);
                    var credintialBehaviour = channelFactory.Endpoint.Behaviors.Find<System.ServiceModel.Description.ClientCredentials>();
                    credintialBehaviour.UserName.UserName = "aloha_user";
                    credintialBehaviour.UserName.Password = "Welcome01";
                    var client = channelFactory.CreateChannel();


                    //var client = new var(binding, address);

                    System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };
                    return client;
                }
                catch (Exception e)
                {

                    DBError(e.Message);
                    return null;
                }
            }
        }




        public static bool ChangePass(User user, string newPass)
        {
            if (user == null) return false;
            logger.Info($"ChangePass {user.FullName}");
            var Client = GetClient();
            if (Client == null) { return false; }

            try
            {
                user.Password = newPass;
                var res = Client.UpdateUser(user);
                if (!res.Success)
                {
                    DBError(res.ErrorMessage);
                    return false;
                }
                //errorMsg = res.ErrorMessage;
                if (res.Success)
                {
                    return true;
                }
                else
                {
                    DBError(res.ErrorMessage);
                    return false;
                }
            }
            catch (Exception e)
            {
                DBError(e.Message);
                return false;
            }
            finally
            {
                //Client.();
            }
        }

        public static bool LoginUser(string login, string password, out AlohaService.ServiceDataContracts.User user, string errorMsg)
        {
            logger.Info($"LoginUser {login}");
            errorMsg = "";
            user = null;
            var Client = GetClient();
            if (Client == null) { return false; }

            try
            {

                OperationResultValue<User> res = Client.LoginUser(login, password);
                if (!res.Success)
                {
                    DBError(res.ErrorMessage);
                    return false;
                }
                errorMsg = res.ErrorMessage;
                user = res.Result;

                return res.Success;
            }
            catch (Exception e)
            {
                //  DBError(e.Message);
                return false;
            }
            finally
            {
                //Client.();
            }
        }

        public static List<User> GetAllUsers()
        {
            var Client = GetClient();
            if (Client == null) { return null; }
            var res = Client.GetUserList();
            if (!res.Success)
            {
                DBError(res.ErrorMessage);
                return null; ;
            }
            return res.Result.ToList();
        }


        public static bool CreateUser(UserInfo user)
        {

            var Client = GetClient();
            if (Client == null) { return false; }
            OperationResult res = Client.CreateUser(user);
            if (!res.Success)
            {
                DBError(res.ErrorMessage);
                return false;
            }
            return res.Success;
        }


        /*
        public static OrderFlight  GetAddOrderFlight(, List<DishPackageFlightOrder> dishes)
        {

        }
        */



        public static bool AddDish(Dish d)
        {
            try
            {
                logger.Debug($"AddDish {d.Barcode} {d.Name} {d.EnglishName}");
                var Client = GetClient();
                if (Client == null) { return false; }
                var res = Client.CreateDish(d);
                d.Id = res.CreatedObjectId;
                logger.Debug($"AddOrderFlight {d.Barcode}; Id={d.Id};  res = {res.Success}; err: {res.ErrorMessage} ");

                return res.Success;
            }
            catch (Exception e)
            {
                DBError($"Error AddDish Mess: {e.Message}");
                return false;
            }
        }

        public static bool AddOrderFlight(OrderFlight order, List<DishPackageFlightOrder> dishes)
        {
            try
            {
                logger.Debug($"AddOrderFlight {order.Id}");
                var Client = GetClient();
                if (Client == null) { return false; }

                if (order.AirCompany != null) order.AirCompanyId = order.AirCompany?.Id;
                if (order.ContactPerson != null) order.ContactPersonId = order.ContactPerson?.Id;
                if (order.WhoDeliveredPersonPerson != null) order.WhoDeliveredPersonPersonId = order.WhoDeliveredPersonPerson?.Id;
                if (order.DeliveryPlace != null) order.DeliveryPlaceId = order.DeliveryPlace?.Id;
                if (order.Driver != null) order.DriverId = order.Driver?.Id;
                if (order.CreatedBy != null) order.CreatedById = order.CreatedBy?.Id;
                if (order.SendBy != null) order.SendById = order.SendBy?.Id;

                var delt = order.DeliveryDate;


                if (order.ContactPerson != null) order.PhoneNumber = order.ContactPerson.Phone;

                foreach (var d in dishes)
                {
                    d.DishId = d.Dish.Id;
                    d.OrderFlight = null;
                }

                OperationResult res = Client.CreateOrderFlight(order);



                if (!res.Success)
                {
                    DBError(res.ErrorMessage);
                    return false;
                }
                order.Id = res.CreatedObjectId;

                foreach (var d in dishes)
                {

                    d.OrderFlightId = res.CreatedObjectId;
                    OperationResult res2 = Client.CreateDishPackageFlightOrder(d);
                    d.Id = res2.CreatedObjectId;
                }

                logger.Debug($"AddOrderFlight {order.Id} res = {res.Success}; err: {res.ErrorMessage} ");

                return res.Success;
            }
            catch (Exception e)
            {
                DBError($"Error AddOrderFlight Mess: {e.Message}");
                return false;
            }
        }



        public static bool AddOrderToGo(OrderToGo order, List<DishPackageToGoOrder> dishes)
        {
            try
            {
                logger.Debug($"AddOrderToGo {order.Id}");
                var Client = GetClient();
                if (Client == null) { return false; }

                order.OrderCustomerId = order.OrderCustomer?.Id;
                order.MarketingChannelId = order.MarketingChannel?.Id;
                order.DriverId = order.Driver?.Id;
                order.CreatedById = order.CreatedBy?.Id;
                order.AddressId = order.Address?.Id;
                foreach (var d in dishes)
                {
                    d.DishId = d.Dish.Id;
                    d.OrderToGo = null;
                }

                OperationResult res = Client.CreateOrderToGo(order);
                if (!res.Success)
                {
                    DBError(res.ErrorMessage);
                    return false;
                }
                order.Id = res.CreatedObjectId;

                foreach (var d in dishes)
                {
                    d.OrderToGoId = res.CreatedObjectId;
                    OperationResult res2 = Client.CreateDishPackageToGoOrder(d);
                    d.Id = res2.CreatedObjectId;
                }
                logger.Debug($"AddOrderToGo {order.Id} res = {res.Success}; err: {res.ErrorMessage} ");
                return res.Success;
            }
            catch (Exception e)
            {
                DBError($"Error AddOrderToGo Mess: {e.Message}");
                return false;
            }
        }
        /*
        public static bool UpdateOrderToGo(OrderToGo order)
        {
            try
            {
                logger.Debug($"UpdateOrderToGo Mess: {order.Id}");
                var Client = GetClient();
                if (Client == null) { return false; }

                order.AddressId = order.Address?.Id;
                order.OrderCustomerId = order.OrderCustomer?.Id;
                order.MarketingChannelId = order.MarketingChannel?.Id;
                order.DriverId = order.Driver?.Id;
                order.CreatedById = order.CreatedBy?.Id;

                foreach (var d in order.DishPackages)
                {
                    d.OrderToGo = null;
                    d.OrderToGoId = order.Id;
                    d.DishId = d.Dish.Id;
                }

                var res = Client.UpdateOrderToGo(order);

                if (!res.Success)
                {
                    DBError(res.ErrorMessage);
                    return false;
                }
                //order = res.Result;

                return res.Success;
            }
            catch (Exception e)
            {
                DBError($"Error UpdateOrderFlight Mess: {e.Message}");
                return false;
            }
        }

        public static bool UpdateOrderFlight(OrderFlight order)
        {
            try
            {
                var Client = GetClient();
                if (Client == null) { return false; }

                order.AirCompanyId = order.AirCompany?.Id;
                order.ContactPersonId = order.ContactPerson?.Id;
                order.WhoDeliveredPersonPersonId = order.WhoDeliveredPersonPerson?.Id;
                order.DeliveryPlaceId = order.DeliveryPlace?.Id;
                order.DriverId = order.Driver?.Id;
                order.CreatedById = order.CreatedBy?.Id;
                order.SendById = order.SendBy?.Id;
                foreach (var d in order.DishPackages)
                {
                    d.OrderFlight = null;
                    d.OrderFlightId = order.Id;
                    d.DishId = d.Dish.Id;
                }
                var res = Client.UpdateOrderFlight(order, Authorization.CurentUser.Id);

                if (!res.Success)
                {
                    DBError(res.ErrorMessage);
                    return false;
                }
                order = res.Result;

                return res.Success;
            }
            catch (Exception e)
            {
                DBError($"Error UpdateOrderFlight Mess: {e.Message}");
                return false;
            }
        }

            */
        public static void UpdateDishList(List<Dish> Dl)
        {
            try
            {
                var Client = GetClient();
                foreach (var d in Dl)
                {
                    var res = Client.UpdateDish(d);
                }
            }
            catch
            { }
        }



        public static List<DishPackageFlightOrder> GetOrdersToFlyDishes(long Id)
        {
            var client = GetClient();
            if (client == null) { return null; }
            var res = client.GetDishPackageFlightOrderList(Id);
            if (!res.Success)
            {
                DBError(res.ErrorMessage);
                return new List<DishPackageFlightOrder>();
            }

            return res.Result.ToList();
        }

        public static List<long> SharAirs = new List<long>() { 41, 44, 45, 46, 47, 48, 49 };
        public static int SharUserId = 10;

        /*
        public static List<OrderFlight> GetOrders(DateTime startDt, DateTime endDt, out List<OrderFlight> SVOOrders)
        {
            SVOOrders = new List<OrderFlight>();
            try
            {
                logger.Debug($"GetOrders startDt {startDt}, endDt {endDt}");

                var client = GetClient();
                if (client == null) { return null; }
                logger.Debug($"GetClient ok");

                var res = client.GetOrderFlightList(
                  new OrderFlightFilter
                  {
                      DeliveryDateStart = new DateTime(startDt.Year, startDt.Month, startDt.Day, 0, 0, 0, DateTimeKind.Utc),
                      DeliveryDateEnd = new DateTime(endDt.Year, endDt.Month, endDt.Day, 23, 59, 59, DateTimeKind.Utc),
                  },
                  new AlohaService.ServiceDataContracts.PageInfo
                  {
                      Skip = 0,
                      Take = 10000
                  }
              );
                if (res == null) { logger.Debug($"res==null"); return new List<OrderFlight>(); ; }
                logger.Debug($"data recived res {res.Success} err: {res.ErrorMessage}");
                if (!res.Success)
                {
                    logger.Debug($"data recived err: {res.ErrorMessage}");
                    DBError(res.ErrorMessage);
                    return new List<OrderFlight>();
                }
                var result = res.Result.Where(a => a.AirCompanyId != null).OrderByDescending(a => a.DeliveryDate).ToList();

                //var result = res.Result;
                foreach (var ord in result)
                {
                    //if (ord.DeliveryPlaceId)

                        ord.ContactPerson = DataExtension.DataCatalogsSingleton.Instance.ContactPerson.SingleOrDefault(a => a.Id == ord.ContactPersonId);
                        ord.AirCompany = DataExtension.DataCatalogsSingleton.Instance.AllAirCompanies.SingleOrDefault(a => a.Id == ord.AirCompanyId);
                        if (ord.DeliveryPlaceId != null)
                        {
                            ord.DeliveryPlace = DataExtension.DataCatalogsSingleton.Instance.DeliveryPlaces.SingleOrDefault(a => a.Id == ord.DeliveryPlaceId);
                        }
                        if (ord.CreatedById != null)
                        {
                            ord.CreatedBy = DataExtension.DataCatalogsSingleton.Instance.ManagerOperator.SingleOrDefault(a => a.Id == ord.CreatedById);
                        }

                        if (ord.SendById != null)
                        {
                            ord.SendBy = DataExtension.DataCatalogsSingleton.Instance.ManagerOperator.SingleOrDefault(a => a.Id == ord.SendById);
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
                }

                SVOOrders = result.Where(a => SharAirs.Contains(a.AirCompanyId.GetValueOrDefault())).ToList();

                if ((Authorization.CurentUser != null) && (Authorization.CurentUser.UserName == "sh.user"))
                {
                    //result = result.Where(a => a.OrderStatus != OrderStatus.Cancelled).ToList();

                    result = result.Where(a => SharAirs.Contains(a.AirCompanyId.GetValueOrDefault())).ToList();

                }
                else
                {
                    if (!Authorization.IsDirector)
                    {
                        result = result.Where(a => a.OrderStatus != OrderStatus.Cancelled).ToList();
                        //var NonAirs = new List<long>() { 41, 44, 45 };
                        result = result.Where(a => !SharAirs.Contains(a.AirCompanyId.GetValueOrDefault())).ToList();
                    }

                }


                logger.Debug($"data recived return result {result.Count()} items ");
                return result;
            }
            catch (Exception e)
            {
                logger.Debug($"GetOrders error {e.Message}");
                return new List<OrderFlight>(); ;
            }
        }

            */


        /*

        public static List<OrderFlight> GetOrdersSVO(DateTime startDt, DateTime endDt)
        {
            try
            {
                logger.Debug($"GetOrders startDt {startDt}, endDt {endDt}");
                var client = GetClient();
                if (client == null) { return null; }
                logger.Debug($"GetClient ok");

                var res = client.GetOrderFlightList(
                  new OrderFlightFilter
                  {
                      DeliveryDateStart = new DateTime(startDt.Year, startDt.Month, startDt.Day, 0, 0, 0, DateTimeKind.Utc),
                      DeliveryDateEnd = new DateTime(endDt.Year, endDt.Month, endDt.Day, 23, 59, 59, DateTimeKind.Utc),

                  },

                  new AlohaService.ServiceDataContracts.PageInfo
                  {
                      Skip = 0,
                      Take = 10000
                  }
              );
                if (res == null) { logger.Debug($"res==null"); return new List<OrderFlight>(); ; }
                logger.Debug($"data recived res {res.Success} err: {res.ErrorMessage}");
                if (!res.Success)
                {
                    logger.Debug($"data recived err: {res.ErrorMessage}");
                    DBError(res.ErrorMessage);
                    return new List<OrderFlight>();
                }
                var result = res.Result.Where(a => a.AirCompanyId != null).OrderByDescending(a => a.DeliveryDate).ToList();


                result = result.Where(a => a.OrderStatus != OrderStatus.Cancelled).ToList();

                result = result.Where(a => SharAirs.Contains(a.AirCompanyId.GetValueOrDefault())).ToList();



                //var result = res.Result;
                foreach (var ord in result)
                {
                    //if (ord.DeliveryPlaceId)

                    ord.ContactPerson = DataExtension.DataCatalogsSingleton.Instance.ContactPerson.SingleOrDefault(a => a.Id == ord.ContactPersonId);
                    ord.AirCompany = DataExtension.DataCatalogsSingleton.Instance.AllAirCompanies.SingleOrDefault(a => a.Id == ord.AirCompanyId);
                    if (ord.DeliveryPlaceId != null)
                    {
                        ord.DeliveryPlace = DataExtension.DataCatalogsSingleton.Instance.DeliveryPlaces.SingleOrDefault(a => a.Id == ord.DeliveryPlaceId);
                    }
                    if (ord.CreatedById != null)
                    {
                        ord.CreatedBy = DataExtension.DataCatalogsSingleton.Instance.ManagerOperator.SingleOrDefault(a => a.Id == ord.CreatedById);
                    }

                    if (ord.SendById != null)
                    {
                        ord.SendBy = DataExtension.DataCatalogsSingleton.Instance.ManagerOperator.SingleOrDefault(a => a.Id == ord.SendById);
                    }


                    if (ord.DishPackages != null)
                    {
                        foreach (var d in ord.DishPackages)
                        {
                            d.Dish = DataExtension.DataCatalogsSingleton.Instance.Dishes.SingleOrDefault(a => a.Id == d.DishId);
                            if (d.TotalPrice != d.Dish.PriceForFlight)
                            {
                                int i = 0;
                                // UI.UIModify.ShowAlert($"{d.TotalPrice}");
                            }
                            d.Printed = true;
                        }
                    }
                }
                logger.Debug($"data recived return result {result.Count()} items ");
                return result;
            }
            catch (Exception e)
            {
                logger.Debug($"GetOrders error {e.Message}");
                return new List<OrderFlight>(); ;
            }
        }

        */

        /*
    public static List<OrderToGo> GetOrderToGoList(DateTime startDt, DateTime endDt)
    {
        try
        {
            logger.Debug($"GetOrderToGoList startDt {startDt}, endDt {endDt}");
            var client = GetClient();
            if (client == null) { return null; }
            logger.Debug($"GetClient ok");

            var res = client.GetOrderToGoList(
              new OrderToGoFilter
              {
                  DeliveryDateStart = new DateTime(startDt.Year, startDt.Month, startDt.Day, 0, 0, 0),
                  DeliveryDateEnd = new DateTime(endDt.Year, endDt.Month, endDt.Day, 23, 59, 59),
              },
              new AlohaService.ServiceDataContracts.PageInfo
              {
                  Skip = 0,
                  Take = 10000
              }
              );
            if (res == null) { logger.Debug($"res==null"); return new List<OrderToGo>(); ; }
            logger.Debug($"data recived res {res.Success} err: {res.ErrorMessage}");
            if (!res.Success)
            {
                logger.Debug($"data recived err: {res.ErrorMessage}");
                DBError(res.ErrorMessage);
                return new List<OrderToGo>();
            }
            var result = res.Result.OrderByDescending(a => a.DeliveryDate).ToList();

            if (!Authorization.IsDirector)
            {
                result = result.Where(a => a.OrderStatus != OrderStatus.Cancelled).ToList();
                var NonAirs = new List<long>() { 41, 44, 45 };
            }

            foreach (var ord in result)
            {
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
            }
            logger.Debug($"data recived return result {result.Count()} items ");
            return result;
        }
        catch (Exception e)
        {
            logger.Debug($"GetOrders error {e.Message}");
            return new List<OrderToGo>(); ;
        }
    }

    */
        /*
        #region DeliveryPerson
        public static List<DeliveryPerson> GetDeliveryPersonList()
        {

            var Client = GetClient();
            if (Client == null) { return null; }
            var res = Client.GetDeliveryPersonList();
            if (!res.Success)
            {
                DBError(res.ErrorMessage);
                return null; ;
            }
            List<DeliveryPerson> aL = res.Result.ToList();
            // aL.Add(AddTestAirC());


            return null;
        }
        
        #endregion
        */

        #region ContactPerson
        public static List<ContactPerson> GetContactPerson()
        {
            /*
            var Client = GetClient();
            if (Client == null) { return null; }
            var res = Client.GetDe();
            if (!res.Success)
            {
                DBError(res.ErrorMessage);
                return null; ;
            }
            List<AirCompany> aL = res.Result.ToList();
            // aL.Add(AddTestAirC());
            */
            //return new List<ContactPerson>() { new ContactPerson() { FullName = "Клава" } };

            return null;
        }
        #endregion

        #region DeliveryPlace
        public static List<DeliveryPlace> GetDeliveryPlaces()
        {
            /*
            var Client = GetClient();
            if (Client == null) { return null; }
            var res = Client.GetDe();
            if (!res.Success)
            {
                DBError(res.ErrorMessage);
                return null; ;
            }
            List<AirCompany> aL = res.Result.ToList();
            // aL.Add(AddTestAirC());
            */

            return null;
        }
        #endregion


        #region AirCompany
        public static List<AirCompany> GetAirCompanyList()
        {
            var Client = GetClient();
            if (Client == null) { return null; }
            var res = Client.GetAirCompanyList();
            if (!res.Success)
            {
                DBError(res.ErrorMessage);
                return null; ;
            }
            List<AirCompany> aL = res.Result.ToList();
            // aL.Add(AddTestAirC());


            return aL;
        }



        private static AirCompany AddTestAirC()
        {
            return new AirCompany()
            {
                Address = "gkfasdf",
                FullName = "DDDD"

            };
        }

        public static long CreateAirCompany(AirCompany cmp)
        {

            var Client = GetClient();
            if (Client == null) { return -1; }
            var cmpInfo = new AirCompany();
            OperationResult res = Client.CreateAirCompany(cmpInfo);
            if (!res.Success)
            {
                DBError(res.ErrorMessage);
                return -1;
            }
            var res2 = Client.GetAirCompany(res.CreatedObjectId);
            if (!res2.Success)
            {
                DBError(res.ErrorMessage);
                return -1;
            }
            cmp.Id = res.CreatedObjectId;
            return res.CreatedObjectId;

        }



        public static bool DeleteAirCompany(long Id)
        {

            var Client = GetClient();
            if (Client == null) { return false; }
            OperationResult res = Client.DeleteAirCompany(Id);
            if (!res.Success)
            {
                DBError(res.ErrorMessage);
                return false;
            }
            return res.Success;
        }


        public static bool UpdateAirCompany(AirCompany cmp)
        {


            var Client = GetClient();
            if (Client == null) { return false; }

            OperationResult res = Client.UpdateAirCompany(cmp);
            if (!res.Success)
            {
                DBError(res.ErrorMessage);
                return false;
            }
            return true;
        }

        #endregion AirCompany


        #region Funcs
        public static List<UserFunc> GetAllFuncs()
        {

            var Client = GetClient();
            if (Client == null) { return null; }
            var res = Client.GetUserFuncList();
            if (!res.Success)
            {
                DBError(res.ErrorMessage);
                return null; ;
            }
            return res.Result.ToList();
        }

        public static bool UpdateUserFunc(long id, string name)
        {
            var Client = GetClient();
            if (Client == null) { return false; }
            var uf = new UserFunc() { Id = id, Name = name };
            OperationResult res = Client.UpdateUserFunc(uf);
            if (!res.Success)
            {
                DBError(res.ErrorMessage);
                return false; ;
            }
            return true;
        }

        public static bool CreateUserFunc(string FName)
        {

            var Client = GetClient();
            if (Client == null) { return false; }
            var uf = new UserFunc() { Name = FName };
            OperationResult res = Client.CreateUserFunc(uf);
            if (!res.Success)
            {
                DBError(res.ErrorMessage);
                return false;
            }
            return res.Success;
        }

        public static bool DeleteUserFunc(long Id)
        {

            var Client = GetClient();
            if (Client == null) { return false; }
            OperationResult res = Client.DeleteUserFunc(Id);
            if (!res.Success)
            {
                DBError(res.ErrorMessage);
                return false;
            }
            return res.Success;
        }
        #endregion Funcs


        public static Dictionary<long, FuncAccessType> GetUserFuncs(long userId)
        {
            var Client = GetClient();
            if (Client == null) { return null; }

            var res = Client.GetAllUserGroups(userId);

            if (!res.Success)
            {
                DBError(res.ErrorMessage);
                return null; ;
            }
            Dictionary<long, FuncAccessType> UserFuncs = new Dictionary<long, FuncAccessType>();
            foreach (var uGroupe in res.Result)
            {
                try
                {

                    var GFuncs = Client.GetAllGroupFuncs(uGroupe.Id);
                    if (GFuncs.Success)
                    {
                        foreach (var uf in GFuncs.Result)
                        {
                            FuncAccessType at;
                            if (UserFuncs.TryGetValue(uf.UserFuncId, out at))
                            {
                                if (at < uf.FuncAccessType)
                                {
                                    UserFuncs[uf.UserFuncId] = uf.FuncAccessType;
                                }
                            }
                            else
                            {
                                UserFuncs.Add(uf.UserFuncId, uf.FuncAccessType);
                            }
                        }

                    }
                }
                catch
                {

                }
            }
            return UserFuncs;
        }

        static void DBError(string Message)
        {
            UI.UIModify.ShowAlert($"Ошибка соединения с базой данных {Environment.NewLine} {Message}");
        }


    }
    public static class DBDataExtractor<T>
        where T : INotifyPropertyChanged
    {
        static Logger logger = LogManager.GetCurrentClassLogger();


        public static bool DeleteItem(Func<long, OperationResult> deleteItemFunc, long arg)
        {
            var res = deleteItemFunc(arg);
            if (!res.Success)
            {
                DBError(res.ErrorMessage);
                return false;
            }
            return true;
        }


        public static bool EditItem(Func<T, OperationResult> editItemFunc, T arg)
        {
            var res = editItemFunc(arg);
            if (!res.Success)
            {
                DBError(res.ErrorMessage);
                return false;
            }

            return true;
        }




        public static long AddItem(Func<T, OperationResult> addItemFunc, T arg)
        {
            var res = addItemFunc(arg);
            if (!res.Success)
            {
                DBError(res.ErrorMessage);
                return -1;
            }
            return res.CreatedObjectId;
        }

        public static T GetDataItem(Func<long, OperationResultValue<T>> getItemFunc, long id)
        {
            try
            {
                var res = getItemFunc(id);
                if (!res.Success)
                {
                    DBError(res.ErrorMessage);
                    return default(T);
                }
                return res.Result;
            }
            catch (Exception e)
            {
                DBError($"Exception GetDataList: {e.Message}");
                throw e;
                //return null;
            }

        }


        public static FullyObservableCollection<T> GetDataList(Func<OperationResultValue<List<T>>> getListFunc)
        {
            try
            {
                var res = getListFunc();
                if (!res.Success)
                {
                    DBError(res.ErrorMessage);
                    return null; ;
                }
                return new FullyObservableCollection<T>(res.Result);
            }
            catch (Exception e)
            {
                DBError($"Exception GetDataList: {e.Message}");
                throw e;
                //return null;
            }

        }

        static void DBError(string Message)
        {
            UI.UIModify.ShowAlert($"Ошибка соединения с базой данных {Environment.NewLine} {Message}");
            logger.Info($"DBError {Message}");
        }
    }

}
