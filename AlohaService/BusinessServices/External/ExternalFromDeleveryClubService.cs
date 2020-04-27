using AlohaService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlohaService.BusinessServices.External
{
    public class ExternalFromDeleveryClubService : ExternalOrdersService
    {
        public ExternalFromDeleveryClubService(AlohaDb databaseContext) : base(databaseContext)
        {
            marketingChanelId = 2;
        }
        protected override void SetMarketingChanelAttributes(OrderToGo orderToGo)
        {
            orderToGo.MarketingChannelId = marketingChanelId;
        }
    }
    

    
}