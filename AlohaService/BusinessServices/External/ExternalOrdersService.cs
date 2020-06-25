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

namespace AlohaService.BusinessServices.External
{
    public abstract class ExternalOrdersService
    {
        protected AlohaDb db;
        protected ILog log;

        public  ExternalOrdersService(AlohaDb databaseContext)
        {
            db = databaseContext;
            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }

        public void CreateYandexToGoOrder()
        {

        }

        protected long marketingChanelId { set; get; }
        public OperationResultValue<List<long>> GetExternalOrderList()
        {
            var res = new OperationResultValue<List<long>>();
            try
            {
                res.Result = db.OrderToGo.Where(a=>a.MarketingChannelId == marketingChanelId).Select(a => a.ExternalId ).ToList();
                res.Success = true;
            }
            catch (Exception e)
            {
                log.Error($"CreateSiteToGoOrder {e.Message}");
                res.Success = false;
                res.ErrorMessage = e.Message;
    
            }
            return res;
        }
        protected abstract void SetMarketingChanelAttributes(OrderToGo orderToGo);
        

            public OperationResult CreateToGoOrder(ExternalToGoOrder order)
        {
            OperationResult res = new OperationResult()
            {
                Success = false
            };
           // try
            {
                log.Debug($"CreateSiteToGoOrder {order.ExternalId}, ExtStrId {order.ExternalStringId} Addr: {order.Client.Address}");
                
                if (order.Dishes == null)
                {
                    log.Error($"No one dish");
                    res.ErrorMessage = "No one dish";
                    return res;
                }
                CreateUnknownDish();
                DateTime testDT = new DateTime(2020, 05, 14);

                if (db.OrderToGo.Any(a => a.MarketingChannelId == marketingChanelId && ((order.ExternalId != 0 && a.ExternalId == order.ExternalId))))
                {
                    log.Error($"Order exists ExternalId");
                    res.ErrorMessage = "Order exists";
                    res.Success = true;
                    return res;
                }


                if (db.OrderToGo.Any(a=>a.MarketingChannelId==marketingChanelId && 
                (order.ExternalStringId!=null && order.ExternalStringId.Trim() != "" && a.ExternalStringId == order.ExternalStringId)))

                
                
                {
                    log.Error($"Order exists by ExternalStringId");
                    res.ErrorMessage = "Order exists";
                    res.Success = true;
                    return res;
                }

                var orderToGo = ConvertOrderFromExternal(order);
                SetMarketingChanelAttributes(orderToGo);
               // orderToGo.MarketingChannelId = 3;
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
            /*
            catch (Exception e)
            {
                log.Error($"CreateSiteToGoOrder {e.Message}");
                res.ErrorMessage = e.Message;
            }
            */
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
                log.Debug($"Find  client by phone: {orderCustomer.Id} {orderCustomer.Name}");
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
                    address.Address = client.Address;
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
                    Phone= client.Phone,
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

        protected virtual long GetDishIdFromBarcode(int barcode, out string name, out bool succeessful)
        {
            name = "";
            if (db.Dish.Any(a => a.Barcode == barcode && a.IsActive))
            {
                succeessful = true;
                name = db.Dish.First(a => a.Barcode == barcode && a.IsActive).Name;
                return db.Dish.First(a => a.Barcode == barcode && a.IsActive).Id;
            }
            succeessful = false;
            return db.Dish.First(a => a.Barcode == -1).Id;
        }

        private Entities.DishPackageToGoOrder GetDPFromExternalDishDP(ExternalDishPackage dp)
        {
            
            var dId = GetDishIdFromBarcode(dp.Id, out string name, out bool sucss);
            log.Debug($"GetDPFromExternalDishDP dp.Id: {dp.Id}; dId:{dId}; sucss:{sucss}");
            int bc = sucss ? dp.Id : -1;
            var dpEnt = new Entities.DishPackageToGoOrder()
            {
                Amount = dp.Count,
                Code = bc,
                DishId = dId,
                Comment = dp.Comment,
                Deleted = false,
                DishName = name,
                TotalPrice = dp.Price,
                ExternalCode = dp.Id
            };
            if (sucss)
            {
                dpEnt.DishName = name;
            }
            if ((marketingChanelId == 2) && (!sucss))
            {
                dpEnt.DishName = dp.Name;
            }


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
                ReadyTime = order.DeliveryDate,
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
                ExternalId = order.ExternalId,
                ExternalStringId = order.ExternalStringId,
                LastUpdatedSession = Guid.NewGuid(),

            };

            var clientId = GetOrCreateClient(order.Client, out long addressId);
            orderToGo.OrderCustomerId = clientId;
            orderToGo.AddressId = addressId;




            return orderToGo;
        }

        private void CreateUnknownDish()
        {
            try

            {
                if (!db.Dish.Any(a => a.Barcode == -1))
                {
                    var d = new Entities.Dish()
                    {
                        Barcode = -1,
                        Name = "Неизвестное блюдо",
                        IsActive = true,
                        IsTemporary = false,
                        IsShar = false,
                        IsAlcohol = false,
                        IsToGo = true,
                        LabelEnglishName = "",
                    LabelRussianName="",
                        LastUpdatedSession = Guid.NewGuid(),
                        UpdatedDate = DateTime.Now,

                    };
                    db.Dish.Add(d);
                    db.SaveChanges();
                }
            }
            catch(Exception e)
            {
                log.Error($"CreateSiteToGoOrder {e.Message} {e.InnerException?.Message}");

            }
        }
    }
}