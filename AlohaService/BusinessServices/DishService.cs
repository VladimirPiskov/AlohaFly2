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
    public class DishService
    {
        private AlohaDb db;
        protected ILog log;

        public DishService(AlohaDb databaseContext)
        {
            db = databaseContext;
            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }

        public OperationResult CreateDish(ServiceDataContracts.Dish dish)
        {
            try
            {
                
                var d = Mapper.Map<ServiceDataContracts.Dish, Entities.Dish>(dish);
                d.UpdatedDate = DateTime.Now;
                db.Dish.Add(d);
                db.SaveChanges();

                return new OperationResult
                {
                    Success = true,
                    CreatedObjectId = d.Id
                };
            }
            catch (Exception e)
            {
                log.Error("Error CreateDish", e);
                return new OperationResult
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResultValue<ServiceDataContracts.Dish> GetDish(long dishId)
        {
            var dish = db.Dish.FirstOrDefault(d => d.Id == dishId);

            var d2 = Mapper.Map<Entities.Dish, ServiceDataContracts.Dish>(dish);

            var result = new OperationResultValue<ServiceDataContracts.Dish>();
            result.Success = true;
            result.Result = d2;
            

          
            return result;
        }


        public OperationResult SetExternalLink(long dishId, int marketingChanel,long externalId)
        {
            var res = new OperationResult() { Success=true};
            try

            {
                DishExternalLinks dl = new DishExternalLinks();

                if (db.DishExternalLinks.Any(a => a.MarketingChanelId == marketingChanel && a.ExternalId == externalId))
                {
                    dl = db.DishExternalLinks.FirstOrDefault(a => a.MarketingChanelId == marketingChanel && a.ExternalId == externalId);
                }
                else
                
                {
                    dl.ExternalId = externalId;
                    dl.MarketingChanelId = marketingChanel;
                    db.DishExternalLinks.Add(dl);
                }
                dl.DishId = dishId;

                db.SaveChanges();
            }
            catch(Exception e)
            {
                res.ErrorMessage = e.Message;
                res.Success = false;
            }
            return res;

        }

            public OperationResult UpdateDish(ServiceDataContracts.Dish dish)
        {
            try
            {
                var ud = db.Dish.First(d => d.Id == dish.Id);
                Mapper.Map(dish, ud);
                

                if (ud == null)
                {
                    return new OperationResult { Success = false, ErrorMessage = "Dish Not Found." };
                }
                ud.UpdatedDate = DateTime.Now;
                db.SaveChanges();

                return new OperationResult { Success = true };
            }
            catch(Exception e)
            {
                log.Error("Error UpdateDish", e);
                return new OperationResult
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResult DeleteDish(long dishId)
        {
            var dish = db.Dish.FirstOrDefault(в => в.Id == dishId);
            if (dish == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "Dish Not Found." };
            }

            db.Dish.Remove(dish);
            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResultValue<List<ServiceDataContracts.Dish>> GetDishList()
        {
            try
            {
                var list = db.Dish.ToList();
                var result = Mapper.Map<List<Entities.Dish>, List<ServiceDataContracts.Dish>>(list);

               


                return new OperationResultValue<List<ServiceDataContracts.Dish>> { Success = true, Result = result };
            }
            catch(Exception e)
            {
                log.Error("Error GetDishList", e);

                return new OperationResultValue<List<ServiceDataContracts.Dish>>
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResultValue<List<ServiceDataContracts.Dish>> GetDishListIntelliSense(string startsWith)
        {
            var result = new List<ServiceDataContracts.Dish>();

            if (string.IsNullOrEmpty(startsWith))
            {
                return new OperationResultValue<List<ServiceDataContracts.Dish>> { Success = true, Result = result };
            }

            var list = db.Dish.Where(d => d.RussianName.StartsWith(startsWith)).OrderBy(d => d.RussianName);

            result = list.Select(dish => new ServiceDataContracts.Dish
            {
                Id = dish.Id,
                Barcode = dish.Barcode,
                EnglishName = dish.EnglishName,
                SHId = dish.SHId,
                Name = dish.Name,
                NeedPrintInMenu = dish.NeedPrintInMenu,
                IsActive = dish.IsActive,
                IsTemporary = dish.IsTemporary,
                IsToGo = dish.IsToGo,
                IsAlcohol = dish.IsAlcohol,
                PriceForDelivery = dish.PriceForDelivery,
                PriceForFlight = dish.PriceForFlight,
                RussianName = dish.RussianName,
                LabelEnglishName = dish.LabelEnglishName,
                LabelRussianName = dish.LabelRussianName,
                DishKitсhenGroupId = dish.DishKitсhenGroupId,
                
                DishLogicGroupId = dish.DishLogicGroupId,
               
                ToFlyLabelSeriesCount = dish.ToFlyLabelSeriesCount,
                ToGoLabelSeriesCount = dish.ToGoLabelSeriesCount
            }
            ).ToList();
            return new OperationResultValue<List<ServiceDataContracts.Dish>> { Success = true, Result = result };
        }

        public OperationResultValue<List<ServiceDataContracts.Dish>> GetDishPage(DishFilter filter, PageInfo page)
        {
            var result = new List<ServiceDataContracts.Dish>();
            var list = new List<Entities.Dish>();

            var querableList = db.Dish.Where(dish => true);

            if (!string.IsNullOrEmpty(filter.RussianNameLike))
            {
                querableList = querableList.Where(d => d.RussianName.Contains(filter.RussianNameLike));
            }

            if(filter.FlightPriceEnd != null)
            {
                querableList = querableList.Where(d => d.PriceForFlight <= filter.FlightPriceEnd);
            }

            if (filter.FlightPriceStart != null)
            {
                querableList = querableList.Where(d => d.PriceForFlight >= filter.FlightPriceStart);
            }

            if (filter.FlightPriceStart != null)
            {
                querableList = querableList.Where(d => d.PriceForFlight >= filter.FlightPriceStart);
            }

            if (filter.IsActive != null)
            {
                querableList = querableList.Where(d => d.IsActive == filter.IsActive);
            }

            if (filter.IsTemporary != null)
            {
                querableList = querableList.Where(d => d.IsActive == filter.IsTemporary);
            }

            if (filter.IsAlcohol != null)
            {
                querableList = querableList.Where(d => d.IsAlcohol == filter.IsAlcohol);
            }

            list = querableList.OrderBy(d => d.RussianName)
                    .Skip(page.Skip)
                    .Take(page.Take).ToList();

            



            result = list.Select(dish => new ServiceDataContracts.Dish
            {
                Id = dish.Id,
                Barcode = dish.Barcode,
                EnglishName = dish.EnglishName,
                SHId = dish.SHId,
                Name = dish.Name,
                NeedPrintInMenu = dish.NeedPrintInMenu,
                IsActive = dish.IsActive,
                IsTemporary = dish.IsTemporary,
                IsToGo = dish.IsToGo,
                IsAlcohol = dish.IsAlcohol,
                PriceForDelivery = dish.PriceForDelivery,
                PriceForFlight = dish.PriceForFlight,
                RussianName = dish.RussianName,
                LabelEnglishName = dish.LabelEnglishName,
                LabelRussianName = dish.LabelRussianName,
                DishKitсhenGroupId = dish.DishKitсhenGroupId,
                
                DishLogicGroupId = dish.DishLogicGroupId,
                
                ToFlyLabelSeriesCount = dish.ToFlyLabelSeriesCount,
                ToGoLabelSeriesCount = dish.ToGoLabelSeriesCount
            }
            ).ToList();
            return new OperationResultValue<List<ServiceDataContracts.Dish>> { Success = true, Result = result };
        }
    }
}