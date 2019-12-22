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
    public class DishPackageFlightOrderService
    {
        private AlohaDb db;
        protected ILog log;

        public DishPackageFlightOrderService(AlohaDb databaseContext)
        {
            db = databaseContext;
            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }

        public OperationResult CreateDishPackageFlightOrder(ServiceDataContracts.DishPackageFlightOrder dishPackageFlightOrder)
        {
            try
            {
                log.Debug($"CreateDishPackageFlightOrder {dishPackageFlightOrder.DishId} {dishPackageFlightOrder.Code?.ToString()}");

                if ((dishPackageFlightOrder.DishId == 0) && (dishPackageFlightOrder.Code != null) && (dishPackageFlightOrder.Code != 0))
                {
                    if (db.Dish.Any(a => a.Barcode == dishPackageFlightOrder.Code))
                    {
                        dishPackageFlightOrder.DishId = db.Dish.FirstOrDefault(a => a.Barcode == (long)dishPackageFlightOrder.Code && a.IsTemporary == false).Id;
                    }
                    else
                    {
                        var d = new Entities.Dish()
                        {
                            Barcode = (long)dishPackageFlightOrder.Code,
                            EnglishName = "",
                            IsActive = true,
                            IsAlcohol = false,
                            IsTemporary = false,
                            LabelEnglishName = "",
                            LabelRussianName = dishPackageFlightOrder.DishName,
                            Name = dishPackageFlightOrder.DishName,
                            NeedPrintInMenu = true,
                            PriceForFlight = dishPackageFlightOrder.TotalPrice,
                            RussianName= dishPackageFlightOrder.DishName,



                        };
                        db.Dish.Add(d);
                        db.SaveChanges();
                        dishPackageFlightOrder.DishId = db.Dish.FirstOrDefault(a => a.Barcode == (long)dishPackageFlightOrder.Code && a.IsTemporary == false).Id;
                    }

                }
                log.Debug($"CreateDishPackageFlightOrder dp.DishId : {dishPackageFlightOrder.DishId} ");
                /*
                var dp = new Entities.DishPackageFlightOrder();
                dp.Amount = dishPackageFlightOrder.Amount;
                dp.Comment = dishPackageFlightOrder.Comment;
                dp.DishName = dishPackageFlightOrder.DishName;
                dp.OrderFlightId = dishPackageFlightOrder.OrderFlightId;
                dp.PassageNumber = dishPackageFlightOrder.PassageNumber;
                dp.TotalPrice = dishPackageFlightOrder.TotalPrice;
                dp.PositionInOrder = dishPackageFlightOrder.PositionInOrder;
                dp.Code = dishPackageFlightOrder.Code;
                */

                var dp = Mapper.Map<ServiceDataContracts.DishPackageFlightOrder, Entities.DishPackageFlightOrder>(dishPackageFlightOrder);

                db.DishPackagesFlightOrder.Add(dp);
                db.SaveChanges();

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

        public OperationResultValue<ServiceDataContracts.DishPackageFlightOrder> GetDishPackageFlightOrder(long dishPackageFlightOrderId)
        {
            var dp = db.DishPackagesFlightOrder.FirstOrDefault(d => d.Id == dishPackageFlightOrderId);

            var result = new OperationResultValue<ServiceDataContracts.DishPackageFlightOrder>();
            result.Success = true;
            /*
            result.Result = new ServiceDataContracts.DishPackageFlightOrder();
            result.Result.Amount = dp.Amount;
            result.Result.Comment = dp.Comment;

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
            result.Result.Dish.EnglishName = dp.Dish.EnglishName;
            result.Result.Dish.RussianName = dp.Dish.RussianName;
            result.Result.Dish.SHId = dp.Dish.SHId;
            result.Result.Dish.NeedPrintInMenu = dp.Dish.NeedPrintInMenu;

            result.Result.PositionInOrder = dp.PositionInOrder;

            result.Result.DishId = dp.DishId;
            result.Result.DishName = dp.DishName;
            result.Result.Id = dp.Id;

            //result.Result.OrderFlight = new ServiceDataContracts.OrderFlight();
            //result.Result.OrderFlight.AirCompany = new ServiceDataContracts.AirCompany();
            //result.Result.OrderFlight.AirCompany.Address = dp.OrderFlight.AirCompany.Address;
            //result.Result.OrderFlight.AirCompany.Code = dp.OrderFlight.AirCompany.Code;
            //result.Result.OrderFlight.AirCompany.DefaultPaymentType = (DefaultPaymentType)dp.OrderFlight.AirCompany.DefaultPaymentType;
            //result.Result.OrderFlight.AirCompany.DiscountType = (DiscountType)dp.OrderFlight.AirCompany.DiscountType;
            //result.Result.OrderFlight.AirCompany.FullName = dp.OrderFlight.AirCompany.FullName;
            //result.Result.OrderFlight.AirCompany.IataCode = dp.OrderFlight.AirCompany.IataCode;
            //result.Result.OrderFlight.AirCompany.Id = dp.OrderFlight.AirCompany.Id;
            //result.Result.OrderFlight.AirCompany.IkaoCode = dp.OrderFlight.AirCompany.IkaoCode;
            //result.Result.OrderFlight.AirCompany.Inn = dp.OrderFlight.AirCompany.Inn;
            //result.Result.OrderFlight.AirCompany.IsActive = dp.OrderFlight.AirCompany.IsActive;
            //result.Result.OrderFlight.AirCompany.Name = dp.OrderFlight.AirCompany.Name;
            //result.Result.OrderFlight.AirCompany.RussianCode = dp.OrderFlight.AirCompany.RussianCode;

            //result.Result.OrderFlight.AirCompanyId = dp.OrderFlight.AirCompanyId;
            //result.Result.OrderFlight.Comment = dp.OrderFlight.Comment;
            //result.Result.OrderFlight.ContactPerson = new ServiceDataContracts.ContactPerson();
            //result.Result.OrderFlight.ContactPerson.FullName = dp.OrderFlight.

            result.Result.PassageNumber = dp.PassageNumber;
            result.Result.OrderFlightId = dp.OrderFlightId;
            result.Result.TotalPrice = dp.TotalPrice;

            result.Result.Code = dp.Code;
            */

            result.Result = Mapper.Map<Entities.DishPackageFlightOrder, ServiceDataContracts.DishPackageFlightOrder>(dp);


            return result;
        }

        public OperationResult UpdateDishPackageFlightOrder(ServiceDataContracts.DishPackageFlightOrder dishPackageFlightOrder)
        {
            log.Info("UpdateDishPackageFlightOrder Id: " + dishPackageFlightOrder.Id);
            //dishPackageFlightOrder.Dish = null;
            //dishPackageFlightOrder.OrderFlight = null;
            var dp = db.DishPackagesFlightOrder.FirstOrDefault(p => p.Id == dishPackageFlightOrder.Id);

            if (dp == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "DishPackagesFlightOrder Not Found." };
            }
            Mapper.Map(dishPackageFlightOrder, dp);
            log.Info($" dishPackageFlightOrder.Deleted: {dishPackageFlightOrder.Deleted} dp {dp.Deleted}");
            /*
            dp.Amount = dishPackageFlightOrder.Amount;
            dp.Comment = dishPackageFlightOrder.Comment;
            dp.DishId = dishPackageFlightOrder.DishId;
            dp.DishName = dishPackageFlightOrder.DishName;
            dp.OrderFlightId = dishPackageFlightOrder.OrderFlightId;
            dp.PassageNumber = dishPackageFlightOrder.PassageNumber;
            dp.TotalPrice = dishPackageFlightOrder.TotalPrice;
            dp.PositionInOrder = dishPackageFlightOrder.PositionInOrder;

            dp.Code = dishPackageFlightOrder.Code;
            */


            db.SaveChanges();
            log.Info("UpdateDishPackageFlightOrder Ok Id: " + dishPackageFlightOrder.Id);

            return new OperationResult { Success = true };
        }

        public OperationResult DeleteDishPackageFlightOrder(long dishPackageFlightOrderId)
        {
            var dp = db.DishPackagesFlightOrder.FirstOrDefault(p => p.Id == dishPackageFlightOrderId);
            if (dp == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "DishPackageFlightOrder Place Not Found." };
            }

            db.DishPackagesFlightOrder.Remove(dp);
            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResultValue<List<ServiceDataContracts.DishPackageFlightOrder>> GetDishPackageFlightOrderList(long orderFlightId)
        {
            try
            {
                var dbContext = new AlohaDb();

                var result = dbContext.DishPackagesFlightOrder.Where(p => p.OrderFlightId == orderFlightId).Select(dp =>
             Mapper.Map<Entities.DishPackageFlightOrder, ServiceDataContracts.DishPackageFlightOrder>(dp)
            
                /*
                new ServiceDataContracts.DishPackageFlightOrder
                {
                    Id = dp.Id,
                    Amount = dp.Amount,
                    Comment = dp.Comment,
                    DishId = dp.DishId,
                    DishName = dp.DishName,
                    OrderFlightId = dp.OrderFlightId,
                    PassageNumber = dp.PassageNumber,
                    TotalPrice = dp.TotalPrice,
                    PositionInOrder = dp.PositionInOrder,
                    Code = dp.Code,

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
                        LabelEnglishName = dp.Dish.LabelEnglishName,
                        LabelRussianName = dp.Dish.LabelRussianName,
                        SHId = dp.Dish.SHId,
                        Name = dp.Dish.Name,
                        NeedPrintInMenu = dp.Dish.NeedPrintInMenu
                    }

                }
    */
    
    ).ToList();
                return new OperationResultValue<List<ServiceDataContracts.DishPackageFlightOrder>>
                {
                    Success = true,
                    Result = result
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResultValue<List<ServiceDataContracts.DishPackageFlightOrder>>
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }
    }
}