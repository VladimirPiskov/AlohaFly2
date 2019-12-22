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
    public class CurierService
    {
        private AlohaDb db;
        protected ILog log;

        public CurierService(AlohaDb databaseContext)
        {
            db = databaseContext;
            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }

        public OperationResult CreateCurier(ServiceDataContracts.Curier curier)
        {
            try
            {
                var dbContext = new AlohaDb();

                var c = new Entities.Curier();
                c.FullName = curier.FullName;
                c.Phone = curier.Phone;
                c.IsActive = curier.IsActive;

                dbContext.Curiers.Add(c);
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

        public OperationResultValue<List<ServiceDataContracts.Curier>> GetCurierList()
        {
            try
            {
                var dbContext = new AlohaDb();

                var result = dbContext.Curiers.Select(cp => new ServiceDataContracts.Curier
                {
                    Id = cp.Id,
                    FullName = cp.FullName,
                    Phone = cp.Phone,
                    IsActive = cp.IsActive
                }).ToList();
                return new OperationResultValue<List<ServiceDataContracts.Curier>>
                {
                    Success = true,
                    Result = result
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResultValue<List<ServiceDataContracts.Curier>>
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResultValue<ServiceDataContracts.Curier> GetCurier(long curierId)
        {
            var c = db.Curiers.FirstOrDefault(cur => cur.Id == curierId);
            var result = new OperationResultValue<ServiceDataContracts.Curier>();
            result.Success = true;
            result.Result = new ServiceDataContracts.Curier();
            result.Result.FullName = c.FullName;
            result.Result.Id = c.Id;
            result.Result.Phone = c.Phone;
            result.Result.IsActive = c.IsActive;

            return result;
        }

        public OperationResult UpdateCurier(ServiceDataContracts.Curier curier)
        {
            var c = db.Curiers.FirstOrDefault(cr => cr.Id == curier.Id);

            if (c == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "Curier Not Found." };
            }

            c.FullName = curier.FullName;
            c.Phone = curier.Phone;
            c.IsActive = curier.IsActive;

            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResult DeleteCurier(long curierId)
        {
            var curier = db.Curiers.FirstOrDefault(c => c.Id == curierId);
            if (curier == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "Curier Not Found." };
            }

            db.Curiers.Remove(curier);
            db.SaveChanges();

            return new OperationResult { Success = true };
        }

    }
}