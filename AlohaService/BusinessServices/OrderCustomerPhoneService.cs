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
    public class OrderCustomerPhoneService
    {
        private AlohaDb db;
        protected ILog log;

        public OrderCustomerPhoneService(AlohaDb databaseContext)
        {
            db = databaseContext;
            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }

        public OperationResult CreatePhone(ServiceDataContracts.OrderCustomerPhone phone)
        {
            try
            {
                var dbContext = new AlohaDb();

                var c = Mapper.Map<ServiceDataContracts.OrderCustomerPhone, Entities.OrderCustomerPhone>(phone);
                c.UpdatedDate = DateTime.Now;
                
                dbContext.OrderCustomerPhones.Add(c);
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
                    ErrorMessage = e.Message +(e.InnerException?.InnerException.Message ?? "")
                };
            }
        }


        public OperationResultValue<ServiceDataContracts.OrderCustomerPhone> GetOrderCustomerPhone(long orderCustomerId)
        {
            var result = new OperationResultValue<ServiceDataContracts.OrderCustomerPhone>();
            try
            {
                var orderCustomerPhone = db.OrderCustomerPhones.FirstOrDefault(oc => oc.Id == orderCustomerId);
                if (orderCustomerPhone != null)
                {
                    result.Result = Mapper.Map<Entities.OrderCustomerPhone, ServiceDataContracts.OrderCustomerPhone>(orderCustomerPhone);
                    result.Success = true;
                }
                
                
            }
            catch(Exception e)
            {
                result.Success = false;
                result.ErrorMessage = e.Message;
            }
            return result;
        }

        public OperationResultValue<List<ServiceDataContracts.OrderCustomerPhone>> GetPhoneList()
        {
            try
            {
                var dbContext = new AlohaDb();
                var custsPh = dbContext.OrderCustomers.Where(a => a.IsActive).SelectMany(a=>a.Phones).Select(a=>a.Id).ToList();
                var res = dbContext.OrderCustomerPhones.Where(a => a.IsActive && custsPh.Contains( a.Id)).ToList();
                
                var result =  Mapper.Map<List<Entities.OrderCustomerPhone>, List<ServiceDataContracts.OrderCustomerPhone>>(res);
                return new OperationResultValue<List<ServiceDataContracts.OrderCustomerPhone>>
                {
                    Success = true,
                    Result = result
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResultValue<List<ServiceDataContracts.OrderCustomerPhone>>
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }
        public OperationResult UpdatePhone(ServiceDataContracts.OrderCustomerPhone phone)
        {
            var c = db.OrderCustomerPhones.FirstOrDefault(cr => cr.Id == phone.Id);

            if (c == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "phone Not Found." };
            }


            c.Phone = phone.Phone;
            
            c.IsActive = phone.IsActive;
            c.IsPrimary = phone.IsPrimary;
            c.OrderCustomerId = phone.OrderCustomerId;
            c.UpdatedDate = DateTime.Now;
            c.LastUpdatedSession = phone.LastUpdatedSession;
            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResult DeletePhone(long phoneId)
        {
            var phone = db.OrderCustomerPhones.FirstOrDefault(c => c.Id == phoneId);
            //phone.OrderCustomerId = 0;
            phone.IsActive = false;
            /*
            if (phone == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "Curier Not Found." };
            }

            db.OrderCustomerPhones.Remove(phone);
    */

    db.SaveChanges();

            return new OperationResult { Success = true };
        }

    }
}
