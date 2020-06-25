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
    public class OrderCustomerAddressService
    {
        private AlohaDb db;
        protected ILog log;

        public OrderCustomerAddressService(AlohaDb databaseContext)
        {
            db = databaseContext;
            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }

        public OperationResult CreateAddress(ServiceDataContracts.OrderCustomerAddress address)
        {
            try
            {
                var dbContext = new AlohaDb();

                var c = new Entities.OrderCustomerAddress
                {
                    Address = address.Address,
                    Comment = address.Comment,
                    IsActive = address.IsActive,
                    IsPrimary = address.IsPrimary,
                    MapUrl = address.MapUrl,
                    OldId = address.OldId,
                    OrderCustomerId = address.OrderCustomerId,
                    SubWay = address.SubWay,
                    ZoneId = address.ZoneId
                };

                c.UpdatedDate = DateTime.Now;
                c.LastUpdatedSession = address.LastUpdatedSession;
                dbContext.OrderCustomerAddresses.Add(c);
                dbContext.SaveChanges();

                return new OperationResult
                {
                    Success = true,
                    CreatedObjectId = c.Id
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

        public OperationResultValue<List<ServiceDataContracts.OrderCustomerAddress>> GetAddressList()
        {
            try
            {
                var dbContext = new AlohaDb();
                var custsAddr = dbContext.OrderCustomers.Where(a => a.IsActive).SelectMany(a => a.Addresses).Select(a => a.Id).ToList();
                var res = dbContext.OrderCustomerAddresses.Where(a=>a.IsActive && custsAddr.Contains(a.Id)).ToList();
                var result =Mapper.Map<List<Entities.OrderCustomerAddress>, List<ServiceDataContracts.OrderCustomerAddress>>(res);
                
                return new OperationResultValue<List<ServiceDataContracts.OrderCustomerAddress>>
                {
                    Success = true,
                    Result = result
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResultValue<List<ServiceDataContracts.OrderCustomerAddress>>
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }
        public OperationResultValue<ServiceDataContracts.OrderCustomerAddress> GetOrderCustomerAddress(long orderCustomerId)
        {
            var result = new OperationResultValue<ServiceDataContracts.OrderCustomerAddress>();
            try
            {
                var orderCustomerAddress = db.OrderCustomerAddresses.Where(a => a.IsActive).FirstOrDefault(oc => oc.Id == orderCustomerId);
                if (orderCustomerAddress != null)
                {
                    result.Result = Mapper.Map<Entities.OrderCustomerAddress, ServiceDataContracts.OrderCustomerAddress>(orderCustomerAddress);
                    result.Success = true;
                }
            }
            catch (Exception e)
            {
                result.Success = false;
                result.ErrorMessage = e.Message;
            }
            return result;
        }
        public OperationResult UpdateAddress(ServiceDataContracts.OrderCustomerAddress address)
        {
            var c = db.OrderCustomerAddresses.FirstOrDefault(cr => cr.Id == address.Id);

            if (c == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "address Not Found." };
            }


            c.Address = address.Address;
            c.Comment = address.Comment;
            c.IsActive = address.IsActive;
            c.IsPrimary = address.IsPrimary;
            c.MapUrl = address.MapUrl;
            c.OldId = address.OldId;
            c.OrderCustomerId = address.OrderCustomerId;
            c.SubWay = address.SubWay;
            c.ZoneId = address.ZoneId;

            c.UpdatedDate = DateTime.Now;
            c.LastUpdatedSession = address.LastUpdatedSession;

            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResult DeleteAddress(long addressId)
        {
            var address = db.OrderCustomerAddresses.FirstOrDefault(c => c.Id == addressId);
            
            if (address == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "Curier Not Found." };
            }
            address.IsActive = false;
            //address.OrderCustomerId = 0;
            db.OrderCustomerAddresses.Remove(address);
            db.SaveChanges();

            return new OperationResult { Success = true };
        }
        
    }
}
