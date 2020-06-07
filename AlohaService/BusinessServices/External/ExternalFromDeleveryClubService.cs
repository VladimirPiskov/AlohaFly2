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
        protected override long GetDishIdFromBarcode(int barcode,out string name, out bool succeessful)
        {
            log.Error($"ExternalFromDeleveryClubService.GetDishIdFromBarcode()");
            name = "";
            if (db.DishExternalLinks.Any(a => a.MarketingChanelId ==marketingChanelId && a.ExternalId==barcode))
            {
                succeessful = true;
                var dId = db.DishExternalLinks.First(a => a.MarketingChanelId == marketingChanelId && a.ExternalId == barcode).Id;
                name = db.Dish.SingleOrDefault(a => a.Id == dId).Name;
                return dId;
            }
            succeessful = false;
            return db.Dish.First(a => a.Barcode == -1).Id;

        }
    }
    

    
}