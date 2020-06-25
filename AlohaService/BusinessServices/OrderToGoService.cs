using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlohaService.ServiceDataContracts;
using AlohaService.Entities;
using log4net;
using AlohaService.Helpers;
using AutoMapper;

namespace AlohaService.BusinessServices
{
    public class OrderToGoService
    {
        private AlohaDb db;
        protected ILog log;

        public OrderToGoService(AlohaDb databaseContext)
        {
            db = databaseContext;
            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }


        public OperationResult CreateOrderToGoWithPackage(ServiceDataContracts.OrderToGo orderToGo)
        {
            try
            {
                log.Debug($"CreateOrderToGoWithPackage");
                var otg = Mapper.Map<ServiceDataContracts.OrderToGo, Entities.OrderToGo>(orderToGo);
                otg.UpdatedDate = DateTime.Now;
                otg.LastUpdatedSession = orderToGo.LastUpdatedSession;
                db.OrderToGo.Add(otg);
                db.SaveChanges();

                OrderCustomrInfoService srv = new OrderCustomrInfoService(db);
                //srv.RecalcCustomerInfo(orderToGo.OrderCustomerId.GetValueOrDefault());
                //srv.RecalcCustomerAllInfo();
                return new OperationResult
                {
                    Success = true,
                    CreatedObjectId = otg.Id
                };
            }
            catch(Exception e)
            {
                log.Error($"CreateOrderToGoWithPackage {e.Message}");
                return new OperationResult
                {
                    Success = false,
                    CreatedObjectId = 0
                };
            }
        }

        public OperationResult CreateOrderToGo(ServiceDataContracts.OrderToGo orderToGo)
        {
            try
            {
                var otg = new Entities.OrderToGo();

                otg.OldId = orderToGo.OldId;

                // otg.Comment = orderToGo.Comment;
                otg.DriverId = orderToGo.DriverId;
                otg.DeliveryDate = orderToGo.DeliveryDate;
                // otg.DeliveryPlaceId = orderToGo.DeliveryPlaceId;
                otg.ExportTime = orderToGo.ExportTime;
                //otg.ExtraCharge = orderToGo.ExtraCharge;
                otg.IsSHSent = orderToGo.IsSHSent;
                otg.CommentKitchen = orderToGo.CommentKitchen;
                otg.OrderComment = orderToGo.OrderComment;
                otg.OrderNumber = orderToGo.OrderNumber;
                otg.OrderStatus = (int)orderToGo.OrderStatus;
                otg.PhoneNumber = orderToGo.PhoneNumber;
                otg.ReadyTime = orderToGo.ReadyTime;
                //otg.WhoDeliveredPersonPersonId = orderToGo.WhoDeliveredPersonPersonId;

                otg.MarketingChannelId = orderToGo.MarketingChannelId;
                otg.OrderCustomerId = orderToGo.OrderCustomerId;

                otg.CreatedById = orderToGo.CreatedById;

                otg.CreationDate = orderToGo.CreationDate == null ? DateTime.Now : orderToGo.CreationDate.Value;
                otg.Summ = orderToGo.Summ;
                otg.DeliveryPrice = orderToGo.DeliveryPrice;

                otg.PaymentId = orderToGo.PaymentId;

                otg.DiscountPercent = orderToGo.DiscountPercent;


                otg.AddressId = orderToGo.AddressId;
                otg.PreCheckPrinted = orderToGo.PreCheckPrinted;
                otg.NeedPrintFR = orderToGo.NeedPrintFR;
                otg.FRPrinted = orderToGo.FRPrinted;
                otg.Closed = orderToGo.Closed;
                otg.NeedPrintPrecheck = orderToGo.NeedPrintPrecheck;


                otg.UpdatedDate = DateTime.Now;
                otg.LastUpdatedSession = orderToGo.LastUpdatedSession;
                db.OrderToGo.Add(otg);
                db.SaveChanges();

                return new OperationResult
                {
                    Success = true,
                    CreatedObjectId = otg.Id
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResult
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResultValue<ServiceDataContracts.OrderToGo> GetOrderToGo(long orderToGoId)
        {
            var order = db.OrderToGo.FirstOrDefault(of => of.Id == orderToGoId);
            var result = new OperationResultValue<ServiceDataContracts.OrderToGo>();
            result.Success = true;
            result.Result = new ServiceDataContracts.OrderToGo();
            result.Result.Id = order.Id;
            result.Result.DeliveryDate = order.DeliveryDate;

            result.Result.ExportTime = order.ExportTime;
            result.Result.IsSHSent = order.IsSHSent;
            result.Result.DriverId = order.DriverId;

            result.Result.MarketingChannelId = order.MarketingChannelId;
            result.Result.CommentKitchen = order.CommentKitchen;
            result.Result.OrderComment = order.OrderComment;
            result.Result.Closed = order.Closed;
            result.Result.OrderCustomerId = order.OrderCustomerId;
            result.Result.OrderNumber = order.OrderNumber;
            result.Result.OrderStatus = (OrderStatus)order.OrderStatus;
            result.Result.PhoneNumber = order.PhoneNumber;
            result.Result.ReadyTime = order.ReadyTime;
            result.Result.CreatedById = order.CreatedById;

            result.Result.CreationDate = order.CreationDate;
            result.Result.Summ = order.Summ;
            result.Result.DeliveryPrice = order.DeliveryPrice;
            result.Result.AddressId = order.AddressId;
            result.Result.PreCheckPrinted = order.PreCheckPrinted;
            result.Result.NeedPrintFR = order.NeedPrintFR;
            result.Result.FRPrinted = order.FRPrinted;
            result.Result.NeedPrintPrecheck = order.NeedPrintPrecheck;
            result.Result.DishPackages = order.DishPackages.Select(pack =>
                            new ServiceDataContracts.DishPackageToGoOrder
                            {
                                Id = pack.Id,
                                Amount = pack.Amount,
                                Comment = pack.Comment,
                                DishId = pack.DishId,
                                DishName = pack.DishName,
                                OrderToGoId = pack.OrderToGoId,
                                TotalPrice = pack.TotalPrice,
                                PositionInOrder = pack.PositionInOrder,
                                Deleted = pack.Deleted,
                                DeletedStatus = pack.DeletedStatus,
                                SpisPaymentId = pack.SpisPaymentId,
                                Code = pack.Code,
                                ExternalCode = pack.ExternalCode
                            }).ToList();
            result.Result.PaymentId = order.PaymentId;
            result.Result.DiscountPercent = order.DiscountPercent;

            return result;
        }





        private void UpdateDishesForOrder(ServiceDataContracts.OrderToGo orderToGo, Entities.OrderToGo originalOrder, long userId)
        {
            log.Info("UpdateDishesForOrder");

            // Delete not presented packages
            foreach (var package in originalOrder.DishPackages.ToList())
            {
                if (!orderToGo.DishPackages.Any(dp => dp.Id == package.Id))
                {
                    log.Info("Delete not presented packages");
                    log.Info("Dish Id: " + package.DishId);
                    var packageToDelete = db.DishPackagesToGoOrder.First(p => p.Id == package.Id);
                    db.DishPackagesToGoOrder.Remove(packageToDelete);
                    // db.SaveChanges();
                }
            }

            var ds = new DishPackageToGoOrderService(db);

            // Add New packages
            foreach (var package in orderToGo.DishPackages.ToList())
            {
                if (package.Id == 0)
                {
                    log.Info("Add new packages");
                    log.Info("ADish Id: " + package.DishId);

                    ds.CreateDishPackageToGoOrder(package);
                }

            }

            // Update presented packages
            foreach (var package in orderToGo.DishPackages.ToList())
            {
                ds.UpdateDishPackageToGoOrder(package);

            }
        }
        public OperationResult SetSHValue(bool value,long orderId)
        {
            try
            {
                var order = db.OrderToGo.SingleOrDefault(a => a.Id == orderId);
                if (order != null)
                {
                    order.IsSHSent = value;
                    db.SaveChanges();
                    return new OperationResult() { Success = true, CreatedObjectId = orderId };
                }
                else
                {
                    return new OperationResult() { Success = false, CreatedObjectId = orderId, ErrorMessage="NoSuchOrder" };
                }
                
            }
            catch(Exception e)
            {
                return new OperationResult() { Success = false, CreatedObjectId = orderId, ErrorMessage = e.Message };
            }
        }



            public OperationResult UpdateOrderToGo(ServiceDataContracts.OrderToGo orderToGo)
        {
            var order = db.OrderToGo.FirstOrDefault(o => o.Id == orderToGo.Id);

            if (order == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "OrderFlight Not Found." };
            }
            order.OldId = orderToGo.OldId;
            order.OrderCustomerId = orderToGo.OrderCustomerId;
            // order.Comment = orderToGo.Comment;
            order.DeliveryDate = orderToGo.DeliveryDate;
            //order.DeliveryPlaceId = orderToGo.DeliveryPlaceId;
            order.DriverId = orderToGo.DriverId;
            order.IsSHSent = orderToGo.IsSHSent;
            order.ExportTime = orderToGo.ExportTime;
            //order.ExtraCharge = orderToGo.ExtraCharge;
            order.CommentKitchen = orderToGo.CommentKitchen;
            order.OrderComment = orderToGo.OrderComment;
            order.OrderNumber = orderToGo.OrderNumber;
            order.OrderStatus = (int)orderToGo.OrderStatus;
            order.PhoneNumber = orderToGo.PhoneNumber;
            order.ReadyTime = orderToGo.ReadyTime;
            //order.WhoDeliveredPersonPersonId = orderToGo.WhoDeliveredPersonPersonId;
            order.MarketingChannelId = orderToGo.MarketingChannelId;
            order.OrderCustomerId = orderToGo.OrderCustomerId;
            order.Closed = orderToGo.Closed;
            order.CreationDate = orderToGo.CreationDate == null ? DateTime.Now : orderToGo.CreationDate.Value;
            order.CreatedById = orderToGo.CreatedById;
            order.Summ = orderToGo.Summ;
            order.DeliveryPrice = orderToGo.DeliveryPrice;
            order.AddressId = orderToGo.AddressId;
            order.PreCheckPrinted = orderToGo.PreCheckPrinted;
            order.NeedPrintFR = orderToGo.NeedPrintFR;
            order.FRPrinted = orderToGo.FRPrinted;
            order.NeedPrintPrecheck = orderToGo.NeedPrintPrecheck;

            order.PaymentId = orderToGo.PaymentId;

            if (orderToGo.CreationDate != null)
            {
                order.CreationDate = orderToGo.CreationDate.Value;
            }

            order.DiscountPercent = orderToGo.DiscountPercent;

            Entities.LogItem logItem = new Entities.LogItem();

            if (orderToGo.DishPackages != null)
            {

                UpdateDishesForOrder(orderToGo, order, 1);
            }
            //order.UpdatedDate = DateTime.Now;

            order.UpdatedDate = DateTime.Now;
            order.LastUpdatedSession = order.LastUpdatedSession;
            db.SaveChanges();

            OrderCustomrInfoService srv = new OrderCustomrInfoService(db);
            srv.RecalcCustomerInfo(orderToGo.OrderCustomerId.GetValueOrDefault());
            //srv.RecalcCustomerAllInfo();
            return new OperationResult { Success = true,CreatedObjectId = order.Id };
        }

        public OperationResult DeleteOrderToGo(long orderToGoId)
        {
            var order = db.OrderToGo.FirstOrDefault(oc => oc.Id == orderToGoId);
            if (order == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "OrderToGo Not Found." };
            }

            db.OrderToGo.Remove(order);
            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        /*
        public OperationResultValue<List<ServiceDataContracts.OrderToGo>> GetOrderUpdatedToGoList(DateTime startDate, Guid transactionId)
        {
            
            var query = db.OrderToGo.Where(o => o.DeliveryDate >= startDate);
            if (db.TransactionTime.Any(a => a.Transaction == transactionId))
            {
                DateTime dt = db.TransactionTime.FirstOrDefault(a => a.Transaction == transactionId).LastUpdatedTime;
                query = query.Where(a=>a.UpdatedDate>=dt);
            }
            var res = query.ToList();

            var c = Mapper.Map<List<Entities.OrderToGo>, List<ServiceDataContracts.OrderToGo>>(res);
            c.ForEach(b=>b.DishPackages = )

            DishPackages = order.DishPackages.Select(pack =>
                                     new ServiceDataContracts.DishPackageToGoOrder
                                     {
                                         Id = pack.Id,
                                         Amount = pack.Amount,
                                         Comment = pack.Comment,
                                         DishId = pack.DishId,
                                         DishName = pack.DishName,
                                         OrderToGoId = pack.OrderToGoId,
                                         TotalPrice = pack.TotalPrice,
                                         PositionInOrder = pack.PositionInOrder,
                                         Deleted = pack.Deleted,
                                         DeletedStatus = pack.DeletedStatus,
                                         SpisPaymentId = pack.SpisPaymentId,
                                         Code = pack.Code

                                     }).ToList(),


        }
        */


        public List<ServiceDataContracts.OrderToGo> GetOrderToGoUpdates(DateTime lastUpdateTime, DateTime DataTime)
        {
            log.Debug($"GetOrderToGoUpdates lastUpdateTime:{lastUpdateTime.ToString()}; DataTime:{DataTime.ToString()}");
            try

            {
                var result = db.OrderToGo.Where(o => o.DeliveryDate >= DataTime && o.UpdatedDate != null && o.UpdatedDate > lastUpdateTime).ToList();
                var outRes = Mapper.Map<List<Entities.OrderToGo>, List<ServiceDataContracts.OrderToGo>>(result);

                return outRes;
            }
            catch (Exception e)
            {
                log.Error($"GetOrderToGoUpdates error {e.Message}");
                return new List<ServiceDataContracts.OrderToGo>();
            }

            /*
            foreach (var r in outRes)
            {
                r.DishPackages = 
            }
            */

        }

        public OperationResultValue<List<ServiceDataContracts.OrderToGo>> GetOrderToGoList(OrderToGoFilter filter, PageInfo page)
        {
            try
            {
                try { log.Debug($"GetOrderToGoList Start: {filter?.DeliveryDateStart} End: {filter?.DeliveryDateEnd}"); } catch { }
                var query = db.OrderToGo.Where(o => true);

                #region Filter





                if (filter.Comment != null)
                {
                    query = query.Where(o => o.Comment == filter.Comment);
                }

                if (filter.CreatedById != null)
                {
                    query = query.Where(o => o.CreatedById == filter.CreatedById);
                }

                if (filter.CreationDateStart != null)
                {
                    query = query.Where(o => o.CreationDate >= filter.CreationDateStart);
                }

                if (filter.CreationDateEnd != null)
                {
                    query = query.Where(o => o.CreationDate <= filter.CreationDateEnd);
                }

                if (filter.DeliveryDateEnd != null)
                {
                    query = query.Where(o => o.DeliveryDate <= filter.DeliveryDateEnd);
                }

                if (filter.DeliveryDateStart != null)
                {
                    query = query.Where(o => o.DeliveryDate >= filter.DeliveryDateStart);
                }
                /*
                if (filter.DeliveryPlaceId != null)
                {
                    query = query.Where(o => o.DeliveryPlaceId >= filter.DeliveryPlaceId);
                }
                */
                if (filter.ExportTimeEnd != null)
                {
                    query = query.Where(o => o.ExportTime <= filter.ExportTimeEnd);
                }

                if (filter.ExportTimeStart != null)
                {
                    query = query.Where(o => o.ExportTime <= filter.ExportTimeStart);
                }



                if (filter.OrderComment != null)
                {
                    query = query.Where(o => o.OrderComment == filter.OrderComment);
                }

                if (filter.OrderNumber != null)
                {
                    query = query.Where(o => o.OrderNumber == filter.OrderNumber);
                }

                if (filter.OrderStatus != null)
                {
                    query = query.Where(o => o.OrderStatus == (int)filter.OrderStatus);
                }

                if (filter.PhoneNumber != null)
                {
                    query = query.Where(o => o.PhoneNumber == filter.PhoneNumber);
                }

                if (filter.ReadyTimeEnd != null)
                {
                    query = query.Where(o => o.ReadyTime <= filter.ReadyTimeEnd);
                }

                if (filter.ReadyTimeStart != null)
                {
                    query = query.Where(o => o.ReadyTime >= filter.ReadyTimeStart);
                }



                #endregion Filter

                query = query.OrderByDescending(o => o.Id).Skip(page.Skip).Take(page.Take);

                var result =
                //query.ToList().Select(order => Mapper.Map<Entities.OrderToGo, ServiceDataContracts.OrderToGo>(order)
                query.Select(order =>
                    new ServiceDataContracts.OrderToGo
                    {
                        Id = order.Id,
                        OldId = order.OldId,
                        //Comment = order.Comment,
                        DeliveryDate = order.DeliveryDate,
                        ExportTime = order.ExportTime,

                        IsSHSent = order.IsSHSent,
                        DriverId = order.DriverId,
                        MarketingChannelId = order.MarketingChannelId,
                        CommentKitchen = order.CommentKitchen,
                        OrderComment = order.OrderComment,
                        Closed = order.Closed,
                        OrderCustomerId = order.OrderCustomerId,
                        OrderNumber = order.OrderNumber,
                        OrderStatus = (OrderStatus)order.OrderStatus,
                        PhoneNumber = order.PhoneNumber,
                        ReadyTime = order.ReadyTime,
                        CreatedById = order.CreatedById,
                        CreationDate = order.CreationDate,
                        Summ = order.Summ,
                        DeliveryPrice = order.DeliveryPrice,

                        AddressId = order.AddressId,
                        PreCheckPrinted = order.PreCheckPrinted,
                        NeedPrintFR = order.NeedPrintFR,
                        FRPrinted = order.FRPrinted,
                        NeedPrintPrecheck = order.NeedPrintPrecheck,


                        //DishPackages = order.DishPackages.Select(pack =>
                        //            Mapper.Map<Entities.DishPackageToGoOrder, ServiceDataContracts.DishPackageToGoOrder>(pack)
                        //).ToList(),



                        DishPackages = order.DishPackages.Select(pack =>
                                     new ServiceDataContracts.DishPackageToGoOrder
                                     {
                                         Id = pack.Id,
                                         Amount = pack.Amount,
                                         Comment = pack.Comment,
                                         DishId = pack.DishId,
                                         DishName = pack.DishName,
                                         OrderToGoId = pack.OrderToGoId,
                                         TotalPrice = pack.TotalPrice,
                                         PositionInOrder = pack.PositionInOrder,
                                         Deleted = pack.Deleted,
                                         DeletedStatus = pack.DeletedStatus,
                                         SpisPaymentId = pack.SpisPaymentId,
                                         Code = pack.Code,
                                         ExternalCode = pack.ExternalCode

                                     }).ToList(),

                        PaymentId = order.PaymentId,
                        DiscountPercent = order.DiscountPercent

                    }

                        ).ToList();

                //result.ForEach(a => { a.DeliveryDate = a.DeliveryDate.ToUniversalTime(); });
                log.Debug($"GetOrderToGoList End");
                return new OperationResultValue<List<ServiceDataContracts.OrderToGo>>
                {
                    Success = true,
                    Result = result
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResultValue<List<ServiceDataContracts.OrderToGo>>
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResultValue<List<ServiceDataContracts.OrderToGo>> GetOrderToGoListNeedToFR()
        {
            try
            {
                //try { log.Debug($"GetOrderToGoList Start: {filter?.DeliveryDateStart} End: {filter?.DeliveryDateEnd}"); } catch { }
                var query = db.OrderToGo.Where(o => o.NeedPrintFR);



                query = query.OrderByDescending(o => o.Id);

                var result = query.Select(order =>
                    new ServiceDataContracts.OrderToGo
                    {
                        Id = order.Id,
                        OldId = order.OldId,
                        // Comment = order.Comment,
                        DeliveryDate = order.DeliveryDate,
                        ExportTime = order.ExportTime,
                        // ExtraCharge = order.ExtraCharge,
                        IsSHSent = order.IsSHSent,
                        DriverId = order.DriverId,
                        MarketingChannelId = order.MarketingChannelId,
                        CommentKitchen = order.CommentKitchen,
                        OrderComment = order.OrderComment,
                        OrderCustomerId = order.OrderCustomerId,
                        OrderNumber = order.OrderNumber,
                        OrderStatus = (OrderStatus)order.OrderStatus,
                        PhoneNumber = order.PhoneNumber,
                        ReadyTime = order.ReadyTime,
                        Closed = order.Closed,
                        CreatedById = order.CreatedById,
                        CreationDate = order.CreationDate,
                        Summ = order.Summ,
                        DeliveryPrice = order.DeliveryPrice,

                        AddressId = order.AddressId,
                        PreCheckPrinted = order.PreCheckPrinted,
                        NeedPrintFR = order.NeedPrintFR,
                        FRPrinted = order.FRPrinted,
                        NeedPrintPrecheck = order.NeedPrintPrecheck,

                        DishPackages = order.DishPackages.Where(x => !x.Deleted).Select(pack =>
                                       new ServiceDataContracts.DishPackageToGoOrder
                                       {
                                           Id = pack.Id,
                                           Amount = pack.Amount,
                                           Comment = pack.Comment,
                                           DishId = pack.DishId,
                                           DishName = pack.DishName,
                                           OrderToGoId = pack.OrderToGoId,
                                           TotalPrice = pack.TotalPrice,
                                           PositionInOrder = pack.PositionInOrder,
                                           Code = pack.Code,
                                           ExternalCode = pack.ExternalCode

                                       }).ToList(),

                        /*
                        DishPackages = order.DishPackages.Where(a=>!a.Deleted).Select(pack =>
                                    Mapper.Map<Entities.DishPackageToGoOrder, ServiceDataContracts.DishPackageToGoOrder>(pack)
                        ).ToList(),
                        
                        */
                        PaymentId = order.PaymentId,

                        DiscountPercent = order.DiscountPercent

                    }).ToList();
                log.Debug($"GetOrderToGoList End");


                foreach (var or in query)
                {
                    or.NeedPrintFR = false;
                }
                db.SaveChanges();

                return new OperationResultValue<List<ServiceDataContracts.OrderToGo>>
                {
                    Success = true,
                    Result = result
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResultValue<List<ServiceDataContracts.OrderToGo>>
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }


    }
}