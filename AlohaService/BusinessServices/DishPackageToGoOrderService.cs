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
    public class DishPackageToGoOrderService
    {
        private AlohaDb db;
        protected ILog log;

        public DishPackageToGoOrderService(AlohaDb databaseContext)
        {
            db = databaseContext;
            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }


        public OperationResult CreateDishPackageToGoOrder(ServiceDataContracts.DishPackageToGoOrder dishPackageToGoOrder)
        {
            try
            {
                log.Error("CreateDishPackageToGoOrder " );

                var dp= Mapper.Map<ServiceDataContracts.DishPackageToGoOrder, Entities.DishPackageToGoOrder>(dishPackageToGoOrder);
                db.DishPackagesToGoOrder.Add(dp);
                db.SaveChanges();

                return new OperationResult
                {
                    Success = true,
                    CreatedObjectId = dp.Id
                };
            }
            catch (Exception e)
            {
                log.Error("CreateDishPackageToGoOrder Error", e);
                return new OperationResult
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResultValue<ServiceDataContracts.DishPackageToGoOrder> GetDishPackageToGoOrder(long dishPackageToGoOrderId)
        {
            var dp = db.DishPackagesToGoOrder.FirstOrDefault(d => d.Id == dishPackageToGoOrderId);

            
            var result = new OperationResultValue<ServiceDataContracts.DishPackageToGoOrder>();
            result.Success = true;
            
            /*
            result.Result = new ServiceDataContracts.DishPackageToGoOrder();

            result.Result.Id = dp.Id;
            result.Result.Amount = dp.Amount;
            result.Result.Comment = dp.Comment;
            result.Result.DishId = dp.DishId;
            result.Result.DishName = dp.DishName;
            result.Result.OrderToGoId = dp.OrderToGoId;
            result.Result.TotalPrice = dp.TotalPrice;
            result.Result.PositionInOrder = dp.PositionInOrder;

            result.Result.Dish = new ServiceDataContracts.Dish();
            result.Result.Dish.Barcode = dp.Dish.Barcode;
            result.Result.Dish.EnglishName = dp.Dish.EnglishName;
            result.Result.Dish.Id = dp.Dish.Id;
            result.Result.Dish.IsActive = dp.Dish.IsActive;
            result.Result.Dish.IsTemporary = dp.Dish.IsTemporary;
            result.Result.Dish.IsAlcohol = dp.Dish.IsAlcohol;
            result.Result.Dish.PriceForDelivery = dp.Dish.PriceForDelivery;
            result.Result.Dish.PriceForFlight = dp.Dish.PriceForFlight;
            result.Result.Dish.RussianName = dp.Dish.RussianName;

            result.Result.Dish.Name = dp.Dish.Name;

            result.Result.Dish.LabelEnglishName = dp.Dish.LabelEnglishName;
            result.Result.Dish.LabelRussianName = dp.Dish.LabelRussianName;

            result.Result.Dish.SHId = dp.Dish.SHId;

            result.Result.Dish.NeedPrintInMenu = dp.Dish.NeedPrintInMenu;

            result.Result.Code = dp.Code;
            */
            result.Result = Mapper.Map<Entities.DishPackageToGoOrder, ServiceDataContracts.DishPackageToGoOrder>(dp);

            return result;
        }

        public OperationResult UpdateDishPackageToGoOrder(ServiceDataContracts.DishPackageToGoOrder dishPackageToGoOrder)
        {
            var dp = db.DishPackagesToGoOrder.FirstOrDefault(p => p.Id == dishPackageToGoOrder.Id);

            if (dp == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "DishPackageToGoOrder Not Found." };
            }
            Mapper.Map(dishPackageToGoOrder, dp);
            /*
            dp.Amount = dishPackageToGoOrder.Amount;
            dp.Comment = dishPackageToGoOrder.Comment;
            dp.DishId = dishPackageToGoOrder.DishId;
            dp.DishName = dishPackageToGoOrder.DishName;
            dp.OrderToGoId = dishPackageToGoOrder.OrderToGoId;
            dp.TotalPrice = dishPackageToGoOrder.TotalPrice;
            dp.PositionInOrder = dishPackageToGoOrder.PositionInOrder;
            dp.Code = dishPackageToGoOrder.Code;
            */
            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResult DeleteDishPackageToGoOrder(long dishPackageToGoOrderId)
        {
            var dp = db.DishPackagesToGoOrder.FirstOrDefault(p => p.Id == dishPackageToGoOrderId);
            if (dp == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "DishPackageToGoOrderId Place Not Found." };
            }

            db.DishPackagesToGoOrder.Remove(dp);
            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResultValue<List<ServiceDataContracts.DishPackageToGoOrder>> GetDishPackageToGoOrderList(long orderToGoId)
        {
            try
            {
                var dbContext = new AlohaDb();

                var result = dbContext.DishPackagesToGoOrder.Where(p => p.OrderToGoId == orderToGoId).Select(
                    
                    dp =>

                          Mapper.Map<Entities.DishPackageToGoOrder, ServiceDataContracts.DishPackageToGoOrder>(dp)

                    /*
                    new ServiceDataContracts.DishPackageToGoOrder
                {
                    Id = dp.Id,
                    Amount = dp.Amount,
                    Comment = dp.Comment,
                    DishId = dp.DishId,
                    DishName = dp.DishName,
                    OrderToGoId = dp.OrderToGoId,
                    TotalPrice = dp.TotalPrice,
                    PositionInOrder = dp.PositionInOrder,

                    Dish = new ServiceDataContracts.Dish()
                    {
                        Barcode = dp.Dish.Barcode,
                        EnglishName = dp.Dish.EnglishName,
                        Id = dp.Dish.Id,
                        IsActive = dp.Dish.IsActive,
                        IsTemporary = dp.Dish.IsTemporary,
                        IsAlcohol = dp.Dish.IsAlcohol,
                        PriceForDelivery = dp.Dish.PriceForDelivery,
                        PriceForFlight = dp.Dish.PriceForFlight,
                        RussianName = dp.Dish.RussianName,
                        Name = dp.Dish.Name,
                        LabelEnglishName = dp.Dish.LabelEnglishName,
                        LabelRussianName = dp.Dish.LabelRussianName,
                        SHId = dp.Dish.SHId,
                        NeedPrintInMenu = dp.Dish.NeedPrintInMenu
                    },
                    Code = dp.Code
                }
    */            
    ).ToList();
                return new OperationResultValue<List<ServiceDataContracts.DishPackageToGoOrder>>
                {
                    Success = true,
                    Result = result
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResultValue<List<ServiceDataContracts.DishPackageToGoOrder>>
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }
    }
}