using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlohaService.ServiceDataContracts;
using AlohaService.Entities;
using log4net;
using AlohaService.Helpers;
using AlohaService.ServiceDataContracts.ExternalContracts;
using OrderToGo = AlohaService.Entities.OrderToGo;
using OrderCustomerAddress = AlohaService.Entities.OrderCustomerAddress;
using OrderCustomer = AlohaService.Entities.OrderCustomer;

namespace AlohaService.BusinessServices
{
    public class ExternalOrdersService
    {
        private AlohaDb db;
        protected ILog log;

        public ExternalOrdersService(AlohaDb databaseContext)
        {
            db = databaseContext;
            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }

        public void CreateYandexToGoOrder()
        {

        }

        public OperationResult CreateSiteToGoOrder(ExternalToGoOrder order)
        {
            OperationResult res = new OperationResult()
            {
                Success = false
            };
            try
            {
                log.Debug($"CreateSiteToGoOrder {order.ExternalId}");
                if (order.Dishes == null)
                {
                    log.Error($"No one dish");
                    res.ErrorMessage = "No one dish";
                    return res;
                }
                if (db.OrderToGo.Any(a=>a.MarketingChannelId==3 && a.OldId==order.ExternalId))
                {
                    log.Error($"Order exists");
                    res.ErrorMessage = "Order exists";
                    res.Success = true;
                    return res;
                }

                var orderToGo = ConvertOrderFromExternal(order);
                orderToGo.MarketingChannelId = 3;
                db.OrderToGo.Add(orderToGo);
                db.SaveChanges();
                log.Debug($"CreatedSiteToGoOrder Id: {orderToGo.Id}");

                int num = 1;
                foreach (var dp in order.Dishes)
                {
                    var dpent = GetDPFromExternalDishDP(dp);
                    dpent.PositionInOrder = num;
                    dpent.OrderToGoId = orderToGo.Id;
                    db.DishPackagesToGoOrder.Add(dpent);
                    db.SaveChanges();
                    num++;
                }
                log.Debug($"Dishes saved");
                res.CreatedObjectId = orderToGo.Id;
                res.Success = true;
            }
            catch (Exception e)
            {
                log.Error($"CreateSiteToGoOrder {e.Message}");
                res.ErrorMessage = e.Message;
            }

            return res;
        }



        private long GetOrCreateClient(ExternalClient client, out long addressId)
        {
            var updatedGuid = Guid.NewGuid();
            log.Debug($"CreateSiteToGoOrder ");
            addressId = 0;
            if (db.OrderCustomerPhones.Any(a => a.Phone == client.Phone && a.OrderCustomer.IsActive))
            {

                var orderCustomer = db.OrderCustomerPhones.First(a => a.Phone == client.Phone).OrderCustomer;
                log.Debug($"Find phone in client: {orderCustomer.Id} {orderCustomer.Name}");
                if (!orderCustomer.IsActive)
                {
                    log.Debug($"Do client active");
                    orderCustomer.IsActive = true;
                    orderCustomer.UpdatedDate = DateTime.Now;
                    orderCustomer.LastUpdatedSession = updatedGuid;
                    db.SaveChanges();
                }
                var phone = orderCustomer.Phones.First(a => a.Phone == client.Phone);
                if (!phone.IsActive)
                {
                    log.Debug($"Do phone active");
                    phone.IsActive = true;
                    phone.UpdatedDate = DateTime.Now;
                    phone.LastUpdatedSession = updatedGuid;
                    db.SaveChanges();
                }

                if (orderCustomer.Addresses.Any(a => a.Address == client.Address))
                {
                    var address = orderCustomer.Addresses.First(a => a.Address == client.Address);
                    addressId = address.Id;
                    log.Debug($"Find addres in client addId: {address.Id}");
                    if (!address.IsActive)
                    {
                        log.Debug($"Do address active");
                        address.IsActive = true;
                        address.UpdatedDate = DateTime.Now;
                        address.LastUpdatedSession = updatedGuid;
                        db.SaveChanges();
                        
                    }

                }
                else
                {
                    log.Debug($"Need create address in client ");
                    var address = GetAddressFromExternal(client);
                    address.OrderCustomerId = orderCustomer.Id;
                    address.UpdatedDate = DateTime.Now;
                    address.LastUpdatedSession = updatedGuid;

                    db.OrderCustomerAddresses.Add(address);
                    //orderCustomer.Addresses.Add(address);
                    db.SaveChanges();
                    log.Debug($"Address created id {address.Id}");
                    addressId = address.Id;
                }


                return orderCustomer.Id;
            }
            else
            {
                log.Debug($"Need create client ");
                var orderCustomer = new OrderCustomer()
                {
                    CashBack = false,
                    Email = client.Emale,
                    IsActive = true,
                    Name = client.Name,
                 
                    UpdatedDate = DateTime.Now,
                    LastUpdatedSession = updatedGuid
                };

                db.OrderCustomers.Add(orderCustomer);
                db.SaveChanges();
                log.Debug($"Client saved id:{orderCustomer.Id}");

                var address = GetAddressFromExternal(client);
                address.OrderCustomerId = orderCustomer.Id;
                address.IsPrimary = true;
                address.UpdatedDate = DateTime.Now;
                address.LastUpdatedSession = updatedGuid;

                db.OrderCustomerAddresses.Add(address);
                db.SaveChanges();
                log.Debug($"Address created id {address.Id}");
                addressId = address.Id;

                var phone = new Entities.OrderCustomerPhone()
                {
                    IsActive = true,
                    IsPrimary=true,
                    UpdatedDate = DateTime.Now,
                    LastUpdatedSession = updatedGuid
                };
                phone.OrderCustomerId = orderCustomer.Id;
                db.OrderCustomerPhones.Add(phone);
                db.SaveChanges();
                log.Debug($"Phone created id {phone.Id}");
                return orderCustomer.Id;

            }

        }

        private OrderCustomerAddress GetAddressFromExternal(ExternalClient externalClient)
        {
            var address = new OrderCustomerAddress()
            {
                Address = externalClient.Address,
                IsActive = true,
                IsPrimary = false,

            };
            return address;
        }

        private long GetDishIdFromBarcode(int barcode, out bool succeessful)
        {
            if (db.Dish.Any(a => a.Barcode == barcode))
            {
                succeessful = true;
                return db.Dish.First(a => a.Barcode == barcode).Id;
            }
            succeessful = false;
            return db.Dish.First(a => a.Barcode == -1).Id;
        }

        private Entities.DishPackageToGoOrder GetDPFromExternalDishDP(ExternalDishPackage dp)
        {
            var dId = GetDishIdFromBarcode(dp.Id, out bool sucss);
            int bc = sucss ? dp.Id : -1;
            var dpEnt = new Entities.DishPackageToGoOrder()
            {
                Amount = dp.Count,
                Code = bc,
                DishId = dId,
                Comment = dp.Comment,
                Deleted = false,
                DishName = dp.Name,
                TotalPrice = dp.Price
            };
            return dpEnt;
        }

        private OrderToGo ConvertOrderFromExternal(ExternalToGoOrder order)
        {
            var orderToGo = new OrderToGo()
            {
                Closed = false,
                Comment = order.Comment,
                CreatedById = 2,
                CommentKitchen = "",
                CreationDate = DateTime.Now,
                DeliveryDate = order.DeliveryDate,
                DeliveryPrice = order.DeliveryPrice,
                DiscountPercent = 0,
                DriverId = null,
                ExportTime = DateTime.Now,
                FRPrinted = false,
                IsSHSent = false,
                MarketingChannelId = 0,
                NeedPrintFR = false,
                NeedPrintPrecheck = false,
                NumberOfBoxes = 1,
                OrderComment = order.Comment,
                OrderStatus = 0,
                PaymentId = null,
                PhoneNumber = order.Client.Phone,
                Summ = order.Summ,
                UpdatedDate = DateTime.Now,
                PreCheckPrinted = false,
                OldId = order.ExternalId,
               LastUpdatedSession = Guid.NewGuid(),

            };

            var clientId = GetOrCreateClient(order.Client, out long addressId);
            orderToGo.OrderCustomerId = clientId;
            orderToGo.AddressId = addressId;




            return orderToGo;
        }
    }
}