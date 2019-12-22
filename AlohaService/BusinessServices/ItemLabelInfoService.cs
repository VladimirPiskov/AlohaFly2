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
    public class ItemLabelInfoService
    {
        private AlohaDb db;
        protected ILog log;

        public ItemLabelInfoService(AlohaDb databaseContext)
        {
            db = databaseContext;
            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }

        public OperationResult CreateItemLabelInfo(ServiceDataContracts.ItemLabelInfo itemlabelInfo)
        {
            try
            {
                var ili = new Entities.ItemLabelInfo();
                ili.Message = itemlabelInfo.Message;
                ili.NameEng = itemlabelInfo.NameEng;
                ili.NameRus = itemlabelInfo.NameRus;
                ili.ParenItemId = itemlabelInfo.ParenItemId;
                ili.SerialNumber = itemlabelInfo.SerialNumber;

                db.ItemLabelInfos.Add(ili);
                db.SaveChanges();

                return new OperationResult
                {
                    Success = true,
                    CreatedObjectId = ili.Id
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

        public OperationResultValue<ServiceDataContracts.ItemLabelInfo> GetItemLabelInfo(long itemLabelInfoId)
        {
            var label = db.ItemLabelInfos.FirstOrDefault(il => il.Id == itemLabelInfoId);
            var result = new OperationResultValue<ServiceDataContracts.ItemLabelInfo>();
            result.Success = true;
            result.Result = new ServiceDataContracts.ItemLabelInfo();
            result.Result.Id = label.Id;
            result.Result.Message = label.Message;
            result.Result.NameEng = label.NameEng;
            result.Result.NameRus = label.NameRus;
            result.Result.ParenItemId = label.ParenItemId;
            result.Result.SerialNumber = label.SerialNumber;

            result.Result.Dish = new ServiceDataContracts.Dish
            {
                Id = label.Dish.Id,
                Name = label.Dish.Name,
                LabelEnglishName = label.Dish.LabelEnglishName,
                LabelRussianName = label.Dish.LabelRussianName,
                NeedPrintInMenu = label.Dish.NeedPrintInMenu,
                SHId = label.Dish.SHId,
                RussianName = label.Dish.RussianName,
                EnglishName = label.Dish.EnglishName,
                Barcode = label.Dish.Barcode,
                PriceForFlight = label.Dish.PriceForFlight,
                PriceForDelivery = label.Dish.PriceForDelivery,
                IsAlcohol = label.Dish.IsAlcohol,
                IsActive = label.Dish.IsActive,
                IsTemporary = label.Dish.IsTemporary
            };

            return result;
        }

        public OperationResult UpdateItemLabelInfo(ServiceDataContracts.ItemLabelInfo itemlabelInfo)
        {
            var label = db.ItemLabelInfos.FirstOrDefault(il => il.Id == itemlabelInfo.Id);

            if (label == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "ItemLabelInfo Not Found." };
            }

            label.Message = itemlabelInfo.Message;
            label.NameEng = itemlabelInfo.NameEng;
            label.NameRus = itemlabelInfo.NameRus;
            label.ParenItemId = itemlabelInfo.ParenItemId;
            label.SerialNumber = itemlabelInfo.SerialNumber;

            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResult DeleteItemLabelInfo(long itemLabelInfoId)
        {
            var label = db.ItemLabelInfos.FirstOrDefault(d => d.Id == itemLabelInfoId);
            if (label == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "ItemLabelInfo Not Found." };
            }

            db.ItemLabelInfos.Remove(label);
            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResultValue<List<ServiceDataContracts.ItemLabelInfo>> GetItemLabelInfoList()
        {
            try
            {
                var result = db.ItemLabelInfos.Select(il => new ServiceDataContracts.ItemLabelInfo
                {
                    Id = il.Id,
                    Message = il.Message,
                    NameEng = il.NameEng,
                    NameRus = il.NameRus,
                    ParenItemId = il.ParenItemId,
                    SerialNumber = il.SerialNumber
                }).ToList();
                return new OperationResultValue<List<ServiceDataContracts.ItemLabelInfo>>
                {
                    Success = true,
                    Result = result
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResultValue<List<ServiceDataContracts.ItemLabelInfo>>
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }
    }
}