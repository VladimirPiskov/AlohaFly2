using AlohaService.Helpers;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlohaService.BusinessServices
{
    public class OrderCustomrInfoService
    {
        private AlohaDb db;
        protected ILog log;

        private int deepMonthCount = 3;

        public OrderCustomrInfoService(AlohaDb databaseContext)
        {
            db = databaseContext;
            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }
        public void RecalcCustomerAllInfo()
        {
            log.Debug($"RecalcCustomerAllInfo");
            foreach (var c in db.OrderCustomers)
            {
                RecalcCustomerInfo(c.Id);
            }
        }

            public void RecalcCustomerInfo(long orderCustomerId)
        {
            try
            {

                log.Debug($"RecalcCustomerAllInfo");
                AlohaDb db2 = new AlohaDb();
                Entities.OrderCustomerInfo info;
                if (db2.OrderCustomerInfo.Any(a => a.OrderCustomerId == orderCustomerId))
                {
                    info = db2.OrderCustomerInfo.FirstOrDefault(a => a.OrderCustomerId == orderCustomerId);

                }
                else
                {
                    info = new Entities.OrderCustomerInfo() { OrderCustomerId = orderCustomerId };
                    db2.OrderCustomerInfo.Add(info);
                }
                db2.SaveChanges();
                DateTime dtS = DateTime.Now.AddMonths(-deepMonthCount);
                info.OrderCount = db2.OrderToGo.Where(a => a.DeliveryDate>=dtS && a.OrderStatus>2 && a.OrderCustomerId == orderCustomerId).Count();
                info.MoneyCount = db2.OrderToGo.Where(a => a.DeliveryDate >= dtS && a.OrderStatus >2 && a.OrderCustomerId == orderCustomerId).Sum(a => (a.DishPackages.Sum(b => b.TotalPrice * b.Amount)) * (1 - (a.DiscountPercent / 100)) + a.DeliveryPrice);
                db2.SaveChanges();
            }
            catch(Exception e)
            {
                log.Error($"RecalcCustomerInfo {e.Message}");
                if (e.InnerException != null)
                {
                    log.Error($"RecalcCustomerInfo InnerException  {e.InnerException.Message}");
                }

            }
        }
    }
}