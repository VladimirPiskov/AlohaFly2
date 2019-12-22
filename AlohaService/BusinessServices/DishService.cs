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
                /*
                var d = new Entities.Dish();
                d.Barcode = dish.Barcode;
                d.EnglishName = dish.EnglishName;
                d.IsActive = dish.IsActive;
                d.IsTemporary = dish.IsTemporary;
                d.IsToGo = dish.IsToGo;
                d.IsAlcohol = dish.IsAlcohol;
                d.PriceForDelivery = dish.PriceForDelivery;
                d.PriceForFlight = dish.PriceForFlight;
                d.RussianName = dish.RussianName;
                d.DishKitсhenGroupId = dish.DishKitсhenGroupId;
                d.DishLogicGroupId = dish.DishLogicGroupId;
                d.ToFlyLabelSeriesCount = dish.ToFlyLabelSeriesCount;
                d.ToGoLabelSeriesCount = dish.ToGoLabelSeriesCount;
                d.SHId = dish.SHId;
                d.SHIdNewBase = dish.SHIdNewBase;
                d.Name = dish.Name;
                d.LabelEnglishName = dish.LabelEnglishName;
                d.LabelRussianName = dish.LabelRussianName;
                d.NeedPrintInMenu = dish.NeedPrintInMenu;
                */
                var d = Mapper.Map<ServiceDataContracts.Dish, Entities.Dish>(dish);

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
            var result = new OperationResultValue<ServiceDataContracts.Dish>();
            result.Success = true;
            result.Result = new ServiceDataContracts.Dish();
            result.Result.Barcode = dish.Barcode;
            result.Result.EnglishName = dish.EnglishName;
            result.Result.Id = dish.Id;
            result.Result.IsActive = dish.IsActive;
            result.Result.IsTemporary = dish.IsTemporary;
            result.Result.IsToGo = dish.IsToGo;
            result.Result.IsAlcohol = dish.IsAlcohol;
            result.Result.PriceForDelivery = dish.PriceForDelivery;
            result.Result.PriceForFlight = dish.PriceForFlight;
            result.Result.LabelEnglishName = dish.LabelEnglishName;
            result.Result.LabelRussianName = dish.LabelRussianName;
            result.Result.RussianName = dish.RussianName;
            result.Result.Name = dish.Name;
            result.Result.SHId = dish.SHId;
            result.Result.SHIdNewBase = dish.SHIdNewBase;
            result.Result.NeedPrintInMenu = dish.NeedPrintInMenu;
            result.Result.ToGoLabelSeriesCount = dish.ToGoLabelSeriesCount;
            result.Result.ToFlyLabelSeriesCount = dish.ToFlyLabelSeriesCount;
            result.Result.DishLogicGroupId = dish.DishLogicGroupId;
            result.Result.DishKitсhenGroupId = dish.DishKitсhenGroupId;


            /*
            result.Result.DishLogicGroup = dish.DishLogicGroup == null ? null : new ServiceDataContracts.DishLogicGroup
            {
                Id = dish.DishLogicGroup.Id,
                IsActive = dish.DishLogicGroup.IsActive,
                Name = dish.DishLogicGroup.Name,
                PositionForPrint = dish.DishLogicGroup.PositionForPrint
            };
            */

            /*
            result.Result.DishKitсhenGroup = dish.DishKitсhenGroup == null ? null : new ServiceDataContracts.DishKitchenGroup
            {
                Id = dish.DishKitсhenGroup.Id,
                IsActive = dish.DishKitсhenGroup.IsActive,
                Name = dish.DishKitсhenGroup.Name,
                EnglishName = dish.DishKitсhenGroup.EnglishName,
                PositionForPrint = dish.DishKitсhenGroup.PositionForPrint
            };
            */
            return result;
        }

        public OperationResult UpdateDish(ServiceDataContracts.Dish dish)
        {
            try
            {
                var ud = db.Dish.First(d => d.Id == dish.Id);
                Mapper.Map(dish, ud);

                /*
                ud.Barcode = dish.Barcode;
                ud.EnglishName = dish.EnglishName;
                ud.IsActive = dish.IsActive;
                ud.IsTemporary = dish.IsTemporary;
                ud.IsToGo = dish.IsToGo;
                ud.IsShar = dish.IsShar;
                ud.IsAlcohol = dish.IsAlcohol;
                ud.PriceForDelivery = dish.PriceForDelivery;
                ud.PriceForFlight = dish.PriceForFlight;
                ud.RussianName = dish.RussianName;
                ud.LabelEnglishName = dish.LabelEnglishName;
                ud.LabelRussianName = dish.LabelRussianName;
                ud.DishKitсhenGroupId = dish.DishKitсhenGroupId;
                ud.DishLogicGroupId = dish.DishLogicGroupId;
                ud.SHId = dish.SHId;
                ud.Name = dish.Name;
                ud.SHIdNewBase = dish.SHIdNewBase;
                ud.NeedPrintInMenu = dish.NeedPrintInMenu;
                ud.ToFlyLabelSeriesCount = dish.ToFlyLabelSeriesCount;
                ud.ToGoLabelSeriesCount = dish.ToGoLabelSeriesCount;
                */

                if (ud == null)
                {
                    return new OperationResult { Success = false, ErrorMessage = "Dish Not Found." };
                }

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

                //var result = list.Select(dish => new ServiceDataContracts.Dish
                //{
                //    Id = dish.Id,
                //    Barcode = dish.Barcode,
                //    EnglishName = dish.EnglishName,
                //    Name = dish.Name,
                //    SHId = dish.SHId,
                //    NeedPrintInMenu = dish.NeedPrintInMenu,
                //    IsActive = dish.IsActive,
                //    IsTemporary = dish.IsTemporary,
                //    IsAlcohol = dish.IsAlcohol,
                //    IsToGo = dish.IsToGo,
                //    PriceForDelivery = dish.PriceForDelivery,
                //    PriceForFlight = dish.PriceForFlight,
                //    RussianName = dish.RussianName,
                //    LabelEnglishName = dish.LabelEnglishName,
                //    LabelRussianName = dish.LabelRussianName,
                //    DishKitсhenGroupId = dish.DishKitсhenGroupId,
                //    SHIdNewBase = dish.SHIdNewBase,
                //    /*
                //    DishKitсhenGroup = dish.DishKitсhenGroup == null ? null : new ServiceDataContracts.DishKitchenGroup
                //    {
                //        Id = dish.DishKitсhenGroup.Id,
                //        IsActive = dish.DishKitсhenGroup.IsActive,
                //        Name = dish.DishKitсhenGroup.Name,
                //        EnglishName = dish.DishKitсhenGroup.EnglishName,
                //        PositionForPrint = dish.DishKitсhenGroup.PositionForPrint
                //    },
                //    */
                //    DishLogicGroupId = dish.DishLogicGroupId,
                //    /*
                //    DishLogicGroup = dish.DishLogicGroup == null ? null : new ServiceDataContracts.DishLogicGroup
                //    {
                //        Id = dish.DishLogicGroup.Id,
                //        IsActive = dish.DishLogicGroup.IsActive,
                //        Name = dish.DishLogicGroup.Name,
                //        PositionForPrint = dish.DishLogicGroup.PositionForPrint
                //    },
                //    */
                //    ToFlyLabelSeriesCount = dish.ToFlyLabelSeriesCount,
                //    ToGoLabelSeriesCount = dish.ToGoLabelSeriesCount
                //}
                //).ToList();


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
                /*
                DishKitсhenGroup = dish.DishKitсhenGroup == null ? null : new ServiceDataContracts.DishKitchenGroup
                {
                    Id = dish.DishKitсhenGroup.Id,
                    IsActive = dish.DishKitсhenGroup.IsActive,
                    Name = dish.DishKitсhenGroup.Name,
                    EnglishName = dish.DishKitсhenGroup.EnglishName,
                    PositionForPrint = dish.DishKitсhenGroup.PositionForPrint
                },
                */
                DishLogicGroupId = dish.DishLogicGroupId,
                /*
                DishLogicGroup = dish.DishLogicGroup == null ? null : new ServiceDataContracts.DishLogicGroup
                {
                    Id = dish.DishLogicGroup.Id,
                    IsActive = dish.DishLogicGroup.IsActive,
                    Name = dish.DishLogicGroup.Name,
                    PositionForPrint = dish.DishLogicGroup.PositionForPrint
                },
                */
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

            //if (string.IsNullOrEmpty(filter.RussianNameLike))
            //{
            //    list = db.Dish.Where(d => d.PriceForFlight <= filter.FlightPriceEnd &&
            //    d.PriceForFlight >= filter.FlightPriceStart)
            //    //.OrderBy(d => d.RussianName)
            //    .OrderBy(d => d.Id)
            //    .Skip(page.Skip)
            //    .Take(page.Take).ToList();
            //}
            //else
            //{
            //    list = db.Dish.Where(d => d.RussianName.Contains(filter.RussianNameLike) && 
            //    d.PriceForFlight <= filter.FlightPriceEnd &&
            //    d.PriceForFlight >= filter.FlightPriceStart)
            //    .OrderBy(d => d.RussianName)
            //    .Skip(page.Skip)
            //    .Take(page.Take).ToList();
            //}



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
                /*
                DishKitсhenGroup = dish.DishKitсhenGroup == null? null: new ServiceDataContracts.DishKitchenGroup
                {
                    Id = dish.DishKitсhenGroup.Id,
                    IsActive = dish.DishKitсhenGroup.IsActive,
                    Name = dish.DishKitсhenGroup.Name,
                    EnglishName = dish.DishKitсhenGroup.EnglishName,
                    PositionForPrint = dish.DishKitсhenGroup.PositionForPrint
                },
                */
                DishLogicGroupId = dish.DishLogicGroupId,
                /*
                DishLogicGroup = dish.DishLogicGroup == null? null: new ServiceDataContracts.DishLogicGroup
                {
                    Id = dish.DishLogicGroup.Id,
                    IsActive = dish.DishLogicGroup.IsActive,
                    Name = dish.DishLogicGroup.Name,
                    PositionForPrint = dish.DishLogicGroup.PositionForPrint
                },
                */
                ToFlyLabelSeriesCount = dish.ToFlyLabelSeriesCount,
                ToGoLabelSeriesCount = dish.ToGoLabelSeriesCount
            }
            ).ToList();
            return new OperationResultValue<List<ServiceDataContracts.Dish>> { Success = true, Result = result };
        }
    }
}