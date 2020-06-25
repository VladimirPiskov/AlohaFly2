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
            log.Debug($"ExternalFromDeleveryClubService.GetDishIdFromBarcode() extern barcode: "+ barcode);
            name = "";
            try
            {
                
                if (db.DishExternalLinks.Any(a => a.MarketingChanelId == marketingChanelId && a.ExternalId == barcode))
                {
                    succeessful = true;
                    var dId = db.DishExternalLinks.First(a => a.MarketingChanelId == marketingChanelId && a.ExternalId == barcode).DishId;
                    name = db.Dish.SingleOrDefault(a => a.Id == dId).Name;
                    log.Debug($"ExternalFromDeleveryClubService.GetDishIdFromBarcode() return " + dId);
                    return dId;
                }
            }
            catch(Exception e)
            {
                log.Error($"ExternalFromDeleveryClubService.GetDishIdFromBarcode() " + e.Message);
            }
            succeessful = false;
            log.Debug($"ExternalFromDeleveryClubService.GetDishIdFromBarcode() return Unknown" );
            return db.Dish.First(a => a.Barcode == -1).Id;


        }
    }
    

    
}