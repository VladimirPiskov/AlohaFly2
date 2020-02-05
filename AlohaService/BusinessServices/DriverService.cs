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
    public class DriverService
    {
        private AlohaDb db;
        protected ILog log;

        public DriverService(AlohaDb databaseContext)
        {
            db = databaseContext;
            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }

        public OperationResult CreateDriver(ServiceDataContracts.Driver driver)
        {
            try
            {
                var d = new Entities.Driver();
                d.FullName = driver.FullName;
                d.Phone = driver.Phone;
                d.IsActive = driver.IsActive;
                d.UpdatedDate = DateTime.Now;
                d.LastUpdatedSession = driver.LastUpdatedSession;

                db.Driver.Add(d);
                db.SaveChanges();

                return new OperationResult
                {
                    Success = true,
                    CreatedObjectId = d.Id
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

        public OperationResultValue<ServiceDataContracts.Driver> GetDriver(long driverId)
        {
            var driver = db.Driver.FirstOrDefault(d => d.Id == driverId);
            var result = new OperationResultValue<ServiceDataContracts.Driver>();
            result.Success = true;
            result.Result = new ServiceDataContracts.Driver();
            result.Result.FullName = driver.FullName;
            result.Result.Id = driver.Id;
            result.Result.Phone = driver.Phone;
            result.Result.IsActive = driver.IsActive;
            return result;
        }

        public OperationResult UpdateDriver(ServiceDataContracts.Driver driver)
        {
            var dr = db.Driver.FirstOrDefault(d => d.Id == driver.Id);

            if (dr == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "Driver Not Found." };
            }

            dr.FullName = driver.FullName;
            dr.Phone = driver.Phone;
            dr.IsActive = driver.IsActive;

            dr.UpdatedDate = DateTime.Now;
            dr.LastUpdatedSession = driver.LastUpdatedSession;

            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResult DeleteDriver(long driverId)
        {
            var driver = db.Driver.FirstOrDefault(d => d.Id == driverId);
            if (driver == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "Driver Not Found." };
            }

            db.Driver.Remove(driver);
            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResultValue<List<ServiceDataContracts.Driver>> GetDriverList()
        {
            try
            {
                var result = db.Driver.Select(cp => new ServiceDataContracts.Driver
                {
                    Id = cp.Id,
                    FullName = cp.FullName,
                    Phone = cp.Phone,
                    IsActive = cp.IsActive
                }).ToList();
                return new OperationResultValue<List<ServiceDataContracts.Driver>>
                {
                    Success = true,
                    Result = result
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResultValue<List<ServiceDataContracts.Driver>>
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }
    }
}