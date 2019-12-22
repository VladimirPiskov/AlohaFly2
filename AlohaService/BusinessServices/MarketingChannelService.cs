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
    public class MarketingChannelService
    {
        private AlohaDb db;
        protected ILog log;

        public MarketingChannelService(AlohaDb databaseContext)
        {
            db = databaseContext;
            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }

        public OperationResult CreateMarketingChannel(ServiceDataContracts.MarketingChannel MarketingChannel)
        {
            try
            {
                var dbContext = new AlohaDb();

                var dp = new Entities.MarketingChannel();
                dp.Name = MarketingChannel.Name;
                
                dp.IsActive = MarketingChannel.IsActive;
                dbContext.MarketingChannel.Add(dp);
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

        public OperationResultValue<ServiceDataContracts.MarketingChannel> GetMarketingChannel(long MarketingChannelId)
        {
            var MarketingChannel = db.MarketingChannel.FirstOrDefault(dp => dp.Id == MarketingChannelId);

            var result = new OperationResultValue<ServiceDataContracts.MarketingChannel>();
            result.Success = true;
            result.Result = new ServiceDataContracts.MarketingChannel();
            result.Result.Name = MarketingChannel.Name;
            result.Result.Id = MarketingChannel.Id;
            
            result.Result.IsActive = MarketingChannel.IsActive;

            return result;
        }

        public OperationResult UpdateMarketingChannel(ServiceDataContracts.MarketingChannel MarketingChannel)
        {
            var dp = db.MarketingChannel.FirstOrDefault(p => p.Id == MarketingChannel.Id);

            if (dp == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "MarketingChannel Not Found." };
            }

            dp.Name = MarketingChannel.Name;
            
            dp.IsActive = MarketingChannel.IsActive;

            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResult DeleteMarketingChannel(long MarketingChannelId)
        {
            var dp = db.MarketingChannel.FirstOrDefault(p => p.Id == MarketingChannelId);
            if (dp == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "MarketingChannelNot Found." };
            }

            db.MarketingChannel.Remove(dp);
            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public OperationResultValue<List<ServiceDataContracts.MarketingChannel>> GetMarketingChannelList()
        {
            try
            {
                var dbContext = new AlohaDb();

                var result = dbContext.MarketingChannel.Select(dp => new ServiceDataContracts.MarketingChannel
                {
                    Id = dp.Id,
                    Name = dp.Name,
                   
                    IsActive = dp.IsActive
                }).ToList();
                return new OperationResultValue<List<ServiceDataContracts.MarketingChannel>>
                {
                    Success = true,
                    Result = result
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResultValue<List<ServiceDataContracts.MarketingChannel>>
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }
    }
}