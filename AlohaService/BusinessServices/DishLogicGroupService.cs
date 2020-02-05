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
    public class DishLogicGroupService
    {
        private AlohaDb db;
        protected ILog log;

        public DishLogicGroupService(AlohaDb databaseContext)
        {
            db = databaseContext;
            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }

        public OperationResult CreateDishLogicGroup(ServiceDataContracts.DishLogicGroup group)
        {
            try
            {
                var d = new Entities.DishLogicGroup();
                d.Name = group.Name;
                d.PositionForPrint = group.PositionForPrint;


                d.UpdatedDate = DateTime.Now;
                d.LastUpdatedSession = group.LastUpdatedSession;

                db.DishLogicGroups.Add(d);
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

        public OperationResultValue<ServiceDataContracts.DishLogicGroup> GetDishLogicGroup(long dishLogicGroupId)
        {
            var dish = db.DishLogicGroups.FirstOrDefault(d => d.Id == dishLogicGroupId);
            var result = new OperationResultValue<ServiceDataContracts.DishLogicGroup>();
            result.Success = true;
            result.Result = new ServiceDataContracts.DishLogicGroup();
            result.Result.Id = dish.Id;
            result.Result.IsActive = dish.IsActive;
            result.Result.Name = dish.Name;
            result.Result.PositionForPrint = dish.PositionForPrint;

            return result;
        }

        public OperationResult UpdateDishLogicGroup(ServiceDataContracts.DishLogicGroup dishLogicGroup)
        {
            var ud = db.DishLogicGroups.First(d => d.Id == dishLogicGroup.Id);

            ud.IsActive = dishLogicGroup.IsActive;
            ud.Name = dishLogicGroup.Name;
            ud.PositionForPrint = dishLogicGroup.PositionForPrint;
            ud.UpdatedDate = DateTime.Now;
            ud.LastUpdatedSession = dishLogicGroup.LastUpdatedSession;
            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResult DeleteDishLogicGroup(long dishLogicGroupId)
        {
            var dish = db.DishLogicGroups.FirstOrDefault(в => в.Id == dishLogicGroupId);

            db.DishLogicGroups.Remove(dish);
            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResultValue<List<ServiceDataContracts.DishLogicGroup>> GetDishLogicGroupsList()
        {
            var list = db.DishLogicGroups.ToList();
            var result = list.Select(dish => new ServiceDataContracts.DishLogicGroup
            {
                Id = dish.Id,
                Name = dish.Name,
                IsActive = dish.IsActive,
                PositionForPrint = dish.PositionForPrint
            }
            ).ToList();
            return new OperationResultValue<List<ServiceDataContracts.DishLogicGroup>> { Success = true, Result = result };
        }
    }
}