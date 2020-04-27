using AlohaService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlohaService.BusinessServices.External
{
    public class ExternalFromSiteOrdersService : ExternalOrdersService
    {
        public ExternalFromSiteOrdersService(AlohaDb databaseContext):base(databaseContext)
        {
            marketingChanelId = 3;
        }
        protected override void SetMarketingChanelAttributes(OrderToGo orderToGo)
        {
            orderToGo.MarketingChannelId = marketingChanelId;
        }
    }
}