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
    public class DeliveryPlaceService
    {
        private AlohaDb db;
        protected ILog log;

        public DeliveryPlaceService(AlohaDb databaseContext)
        {
            db = databaseContext;
            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }

        public OperationResult CreateDeliveryPlace(ServiceDataContracts.DeliveryPlace deliveryPlace)
        {
            try
            {
                var dbContext = new AlohaDb();

                var dp = new Entities.DeliveryPlace();
                dp.Name = deliveryPlace.Name;
                dp.Phone = deliveryPlace.Phone;
                dp.IsActive = deliveryPlace.IsActive;
                dbContext.DeliveryPlace.Add(dp);
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

        public OperationResultValue<ServiceDataContracts.DeliveryPlace> GetDeliveryPlace(long deliveryPlaceId)
        {
            var deliveryPlace = db.DeliveryPlace.FirstOrDefault(dp => dp.Id == deliveryPlaceId);

            var result = new OperationResultValue<ServiceDataContracts.DeliveryPlace>();
            result.Success = true;
            result.Result = new ServiceDataContracts.DeliveryPlace();
            result.Result.Name = deliveryPlace.Name;
            result.Result.Id = deliveryPlace.Id;
            result.Result.Phone = deliveryPlace.Phone;
            result.Result.IsActive = deliveryPlace.IsActive;

            return result;
        }

        public OperationResult UpdateDeliveryPlace(ServiceDataContracts.DeliveryPlace deliveryPlace)
        {
            var dp = db.DeliveryPlace.FirstOrDefault(p => p.Id == deliveryPlace.Id);

            if (dp == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "Delivery Place Not Found." };
            }

            dp.Name = deliveryPlace.Name;
            dp.Phone = deliveryPlace.Phone;
            dp.IsActive = deliveryPlace.IsActive;

            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResult DeleteDeliveryPlace(long deliveryPlaceId)
        {
            var dp = db.DeliveryPlace.FirstOrDefault(p => p.Id == deliveryPlaceId);
            if (dp == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "Delivery Place Not Found." };
            }

            db.DeliveryPlace.Remove(dp);
            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResultValue<List<ServiceDataContracts.DeliveryPlace>> GetDeliveryPlaceList()
        {
            try
            {
                var dbContext = new AlohaDb();

                var result = dbContext.DeliveryPlace.Select(dp => new ServiceDataContracts.DeliveryPlace
                {
                    Id = dp.Id,
                    Name = dp.Name,
                    Phone = dp.Phone,
                    IsActive = dp.IsActive
                }).ToList();
                return new OperationResultValue<List<ServiceDataContracts.DeliveryPlace>>
                {
                    Success = true,
                    Result = result
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResultValue<List<ServiceDataContracts.DeliveryPlace>>
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }
    }
}