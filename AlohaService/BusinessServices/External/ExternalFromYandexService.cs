using AlohaService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlohaService.BusinessServices.External
{
    public class ExternalFromYandexService : ExternalOrdersService
    {
        public ExternalFromYandexService(AlohaDb databaseContext) : base(databaseContext)
        {
            marketingChanelId = 5;
        }
        protected override void SetMarketingChanelAttributes(OrderToGo orderToGo)
        {
            orderToGo.MarketingChannelId = marketingChanelId;
        }
    }
    

    
}