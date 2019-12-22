using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlohaService.ServiceDataContracts;
using AlohaService.Entities;
using log4net;
using AlohaService.Helpers;

namespace AlohaService.BusinessServices
{
    public class DeliveryPersonService
    {
        private AlohaDb db;
        protected ILog log;

        public DeliveryPersonService(AlohaDb databaseContext)
        {
            db = databaseContext;
            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }

        public OperationResult CreateDeliveryPerson(ServiceDataContracts.DeliveryPerson deliveryPerson)
        {
            try
            {
                var dbContext = new AlohaDb();

                var dp = new Entities.DeliveryPerson();
                dp.FullName = deliveryPerson.FullName;
                dp.Phone = deliveryPerson.Phone;
                dp.IsActive = deliveryPerson.IsActive;

                dbContext.DeliveryPersons.Add(dp);
                dbContext.SaveChanges();

                return new OperationResult
                {
                    Success = true,
                    CreatedObjectId = dp.Id
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

        public OperationResultValue<ServiceDataContracts.DeliveryPerson> GetDeliveryPerson(long deliveryPersonId)
        {
            var dp = db.DeliveryPersons.FirstOrDefault(per => per.Id == deliveryPersonId);
            var result = new OperationResultValue<ServiceDataContracts.DeliveryPerson>();
            result.Success = true;
            result.Result = new ServiceDataContracts.DeliveryPerson();
            result.Result.FullName = dp.FullName;
            result.Result.Id = dp.Id;
            result.Result.Phone = dp.Phone;
            result.Result.IsActive = dp.IsActive;

            return result;
        }

        public OperationResult UpdateDeliveryPerson(ServiceDataContracts.DeliveryPerson deliveryPerson)
        {
            var dp = db.DeliveryPersons.FirstOrDefault(per => per.Id == deliveryPerson.Id);

            if (dp == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "Delivery Person Not Found." };
            }

            dp.FullName = deliveryPerson.FullName;
            dp.Phone = deliveryPerson.Phone;
            dp.IsActive = deliveryPerson.IsActive;

            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResult DeleteDeliveryPerson(long deliveryPersonId)
        {
            var dp = db.DeliveryPersons.FirstOrDefault(c => c.Id == deliveryPersonId);
            if (dp == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "Delivery Person Not Found." };
            }

            db.DeliveryPersons.Remove(dp);
            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResultValue<List<ServiceDataContracts.DeliveryPerson>> GetDeliveryPersonList()
        {
            try
            {
                var dbContext = new AlohaDb();

                var result = dbContext.DeliveryPersons.Select(dp => new ServiceDataContracts.DeliveryPerson
                {
                    Id = dp.Id,
                    FullName = dp.FullName,
                    Phone = dp.Phone,
                    IsActive = dp.IsActive
                }).ToList();
                return new OperationResultValue<List<ServiceDataContracts.DeliveryPerson>>
                {
                    Success = true,
                    Result = result
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResultValue<List<ServiceDataContracts.DeliveryPerson>>
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }
    }
}