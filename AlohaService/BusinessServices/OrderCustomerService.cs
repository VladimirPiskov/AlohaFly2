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
    public class OrderCustomerService
    {
        private AlohaDb db;
        protected ILog log;

        public OrderCustomerService(AlohaDb databaseContext)
        {
            db = databaseContext;
            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }

        public OperationResult CreateOrderCustomer(ServiceDataContracts.OrderCustomer orderCustomer)
        {
            try
            {
                var oc = new Entities.OrderCustomer();
                oc.OldId = orderCustomer.OldId;
                oc.Name = orderCustomer.Name;
                oc.IsActive = orderCustomer.IsActive;
                oc.Comments = orderCustomer.Comments;
                oc.Email = orderCustomer.Email;
                oc.MiddleName = orderCustomer.MiddleName;
                oc.SecondName = orderCustomer.SecondName;
                oc.DiscountPercent = orderCustomer.DiscountPercent;

                oc.UpdatedDate = DateTime.Now;
                oc.LastUpdatedSession = orderCustomer.LastUpdatedSession;
                db.OrderCustomers.Add(oc);
                db.SaveChanges();

                
                db.SaveChanges();

                return new OperationResult
                {
                    Success = true,
                    CreatedObjectId = oc.Id
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

        public OperationResultValue<ServiceDataContracts.OrderCustomer> GetOrderCustomer(long orderCustomerId)
        {
            var orderCustomer = db.OrderCustomers.FirstOrDefault(oc => oc.Id == orderCustomerId);
            var result = new OperationResultValue<ServiceDataContracts.OrderCustomer>();
            result.Success = true;
            result.Result = new ServiceDataContracts.OrderCustomer();
            result.Result.Id = orderCustomer.Id;
            result.Result.Name = orderCustomer.Name;
            result.Result.IsActive = orderCustomer.IsActive;

            result.Result.Comments = orderCustomer.Comments;
            result.Result.Email = orderCustomer.Email;
            result.Result.MiddleName = orderCustomer.MiddleName;
            result.Result.SecondName = orderCustomer.SecondName;
            result.Result.DiscountPercent = orderCustomer.DiscountPercent;



            result.Result.Addresses = orderCustomer.Addresses.Where(a => a.IsActive).Select(a => new ServiceDataContracts.OrderCustomerAddress
            {
                Address = a.Address,
                Id = a.Id,
                IsActive = a.IsActive,
                IsPrimary = a.IsPrimary,
                OrderCustomerId = a.OrderCustomerId,

                  MapUrl = a.MapUrl,
                SubWay = a.SubWay,
                Comment = a.Comment,
                ZoneId = a.ZoneId,
                OldId = a.OldId,

            }).ToList();

            result.Result.Phones = orderCustomer.Phones.Where(a => a.IsActive).Select(p => new ServiceDataContracts.OrderCustomerPhone
            {
                Id = p.Id,
                IsActive = p.IsActive,
                IsPrimary = p.IsPrimary,
                OrderCustomerId = p.OrderCustomerId,
                Phone = p.Phone
            }).ToList();

            return result;
        }


        public OperationResultValue<ServiceDataContracts.OrderCustomer> GetOrderCustomer2(long orderCustomerId)
        {
            var orderCustomer = db.OrderCustomers.FirstOrDefault(oc => oc.Id == orderCustomerId);
            var result = 
                new OperationResultValue<ServiceDataContracts.OrderCustomer>();
            result.Success = true;
            result.Result= Mapper.Map<Entities.OrderCustomer, ServiceDataContracts.OrderCustomer>(orderCustomer);
            var infoEnt = db.OrderCustomerInfo.FirstOrDefault(a => a.OrderCustomerId == orderCustomer.Id);
            if (infoEnt != null)
            {
                result.Result.OrderCustomerInfo = Mapper.Map<Entities.OrderCustomerInfo, ServiceDataContracts.OrderCustomerInfo>(infoEnt);
            }
            
                return result;
        }

        public OperationResult UpdateOrderCustomer(ServiceDataContracts.OrderCustomer orderCustomer)
        {
            var order = db.OrderCustomers.FirstOrDefault(oc => oc.Id == orderCustomer.Id);

            if (order == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "OrderCustomer Not Found." };
            }
          
            order.Name = orderCustomer.Name;
            order.IsActive = orderCustomer.IsActive;
            order.OldId = orderCustomer.OldId;
            order.Comments = orderCustomer.Comments;
            order.Email = orderCustomer.Email;
            order.MiddleName = orderCustomer.MiddleName;
            order.SecondName = orderCustomer.SecondName;
            order.DiscountPercent = orderCustomer.DiscountPercent;
            order.UpdatedDate = DateTime.Now;
            order.LastUpdatedSession = orderCustomer.LastUpdatedSession;
            db.SaveChanges();
            return new OperationResult { Success = true, CreatedObjectId=order.Id };
        }

        public OperationResult DeleteOrderCustomer(long orderCustomerId)
        {
            var order = db.OrderCustomers.FirstOrDefault(oc => oc.Id == orderCustomerId);
            if (order == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "OrderCustomer Not Found." };
            }
            order.IsActive = false;
            foreach (var ph in db.OrderCustomerAddresses.Where(a => a.OrderCustomerId == order.Id))
            {
                ph.IsActive = false;
            }
            foreach (var ph in db.OrderCustomerPhones.Where(a => a.OrderCustomerId == order.Id))
            {
                ph.IsActive = false;
            }
            //db.OrderCustomers.Remove(order);

            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResultValue<List<ServiceDataContracts.OrderCustomer>> GetOrderCustomerList()
        {
            try
            {
                var result = db.OrderCustomers.Select(il => new ServiceDataContracts.OrderCustomer
                {
                    Id = il.Id,
                    OldId = il.OldId,
                    Name = il.Name,
                    IsActive = il.IsActive,
                    Comments = il.Comments,
                    Email = il.Email,
                    SecondName= il.SecondName,
                    MiddleName = il.MiddleName,

                    DiscountPercent= il.DiscountPercent,


                    Addresses = il.Addresses.Where(a=>a.IsActive).Select(a => new ServiceDataContracts.OrderCustomerAddress
                    {
                        Address = a.Address,
                        Id = a.Id,
                        IsActive = a.IsActive,
                        IsPrimary = a.IsPrimary,
                        OrderCustomerId = a.OrderCustomerId,
                          MapUrl = a.MapUrl,
                        SubWay = a.SubWay,
                        Comment = a.Comment,
                        ZoneId = a.ZoneId,
                        OldId=a.OldId
                    }).ToList(),

               Phones = il.Phones.Where(a => a.IsActive).Select(p => new ServiceDataContracts.OrderCustomerPhone
                {
                    Id = p.Id,
                    IsActive = p.IsActive,
                    IsPrimary = p.IsPrimary,
                    OrderCustomerId = p.OrderCustomerId,
                    Phone = p.Phone
                }).ToList()
            }).ToList();
                return new OperationResultValue<List<ServiceDataContracts.OrderCustomer>>
                {
                    Success = true,
                    Result = result
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResultValue<List<ServiceDataContracts.OrderCustomer>>
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }


        public OperationResultValue<List<ServiceDataContracts.OrderCustomer>> GetOrderCustomerList2()
        {
            try
            {
                log.Debug("GetOrderCustomerList2");
                var res = db.OrderCustomers.Where(a => a.IsActive).ToList();
                log.Debug($"GetOrderCustomerList2 res:{res.Count }");
                var result = Mapper.Map<List<Entities.OrderCustomer>, List<ServiceDataContracts.OrderCustomer>>(res);
                foreach (var r in result)
                {
                    var infoEnt = db.OrderCustomerInfo.FirstOrDefault(a => a.OrderCustomerId == r.Id);
                    if (infoEnt != null)
                    {
                        r.OrderCustomerInfo = Mapper.Map<Entities.OrderCustomerInfo, ServiceDataContracts.OrderCustomerInfo>(infoEnt);
                    }
                }
                return new OperationResultValue<List<ServiceDataContracts.OrderCustomer>>
                {
                    Success = true,
                    Result = result
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResultValue<List<ServiceDataContracts.OrderCustomer>>
                {
                    
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }
    }
}