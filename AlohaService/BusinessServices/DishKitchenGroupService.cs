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
    public class DishKitchenGroupService
    {
        private AlohaDb db;
        protected ILog log;

        public DishKitchenGroupService(AlohaDb databaseContext)
        {
            db = databaseContext;
            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }

        public OperationResult CreateDishKitchenGroup(ServiceDataContracts.DishKitchenGroup group)
        {
            try
            {
                var d = new Entities.DishKitchenGroup();
                d.Name = group.Name;
                d.EnglishName = group.EnglishName;
                d.PositionForPrint = group.PositionForPrint;
                d.SHIdSh = group.SHIdSh;
                d.SHIdToFly = group.SHIdToFly;
                d.SHIdToGo = group.SHIdToGo;
                db.DishKitchenGroups.Add(d);
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

        public OperationResultValue<ServiceDataContracts.DishKitchenGroup> GetDishKitchenGroup(long dishKitchenGroupId)
        {
            var dish = db.DishKitchenGroups.FirstOrDefault(d => d.Id == dishKitchenGroupId);
            var result = new OperationResultValue<ServiceDataContracts.DishKitchenGroup>();
            result.Success = true;
            result.Result = new ServiceDataContracts.DishKitchenGroup();
            result.Result.Id = dish.Id;
            result.Result.IsActive = dish.IsActive;
            result.Result.Name = dish.Name;
            result.Result.EnglishName = dish.EnglishName;
            result.Result.PositionForPrint = dish.PositionForPrint;
            result.Result.SHIdSh = dish.SHIdSh;
            result.Result.SHIdToFly = dish.SHIdToFly;
            result.Result.SHIdToGo = dish.SHIdToGo;
            return result;
        }

        public OperationResult UpdateDishKitchenGroup(ServiceDataContracts.DishKitchenGroup dishKitchenGroup)
        {
            var ud = db.DishKitchenGroups.First(d => d.Id == dishKitchenGroup.Id);

            ud.IsActive = dishKitchenGroup.IsActive;
            ud.Name = dishKitchenGroup.Name;
            ud.EnglishName = dishKitchenGroup.EnglishName;
            ud.PositionForPrint = dishKitchenGroup.PositionForPrint;
            ud.SHIdSh = dishKitchenGroup.SHIdSh;
            ud.SHIdToFly = dishKitchenGroup.SHIdToFly;
            ud.SHIdToGo = dishKitchenGroup.SHIdToGo;

            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResult DeleteDishKitchenGroup(long dishKitchenGroupId)
        {
            var dish = db.DishKitchenGroups.FirstOrDefault(в => в.Id == dishKitchenGroupId);

            db.DishKitchenGroups.Remove(dish);
            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResultValue<List<ServiceDataContracts.DishKitchenGroup>> GetDishKitсhenGroupsList()
        {
            var list = db.DishKitchenGroups.ToList();
            var result = list.Select(dish => new ServiceDataContracts.DishKitchenGroup
            {
                Id = dish.Id,
                Name = dish.Name,
                EnglishName = dish.EnglishName,
                IsActive = dish.IsActive,
                PositionForPrint = dish.PositionForPrint,
                SHIdSh = dish.SHIdSh,
                SHIdToFly = dish.SHIdToFly,
               SHIdToGo = dish.SHIdToGo 

            }
            ).ToList();
            return new OperationResultValue<List<ServiceDataContracts.DishKitchenGroup>> { Success = true, Result = result };
        }
    }
}