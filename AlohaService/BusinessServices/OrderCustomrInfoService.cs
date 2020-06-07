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

                log.Debug($"RecalcCustomerInfo");
                AlohaDb db2 = new AlohaDb();
                Entities.OrderCustomerInfo info;
                var cust = db2.OrderCustomers.FirstOrDefault(a => a.Id == orderCustomerId);
                log.Debug($"RecalcCustomerAllInfo cust get");
               
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
                log.Debug($"RecalcCustomerAllInfo info get");
                DateTime dtS = DateTime.Now.AddMonths(-deepMonthCount);
                info.OrderCount = db2.OrderToGo.Where(a => a.DeliveryDate>=dtS && a.OrderStatus>2 && a.OrderCustomerId == orderCustomerId).Count();
                log.Debug($"RecalcCustomerAllInfo OrderCount get");
                var ordrs = db2.OrderToGo.Where(a => a.DeliveryDate >= dtS && a.OrderStatus > 2 &&  (a.OrderCustomerId ?? 0)== orderCustomerId && a.DishPackages.Count > 0).ToList();
                log.Debug($"RecalcCustomerAllInfo ordrs get");
                info.MoneyCount = ordrs.Sum(a => (a.DishPackages.Sum(b => b.TotalPrice * b.Amount)) * (1 - (a.DiscountPercent / 100)) + a.DeliveryPrice);
                log.Debug($"RecalcCustomerAllInfo MoneyCount get");
                if (cust.CashBack)
                {
                    log.Debug($"Calc cashback for {cust.Name}");
                    var cashbackMarks = new List<long> { 1, 3, 4 };
                    var saleGroups = db.Payments.Where(b => b.PaymentGroup.Sale).Select(a => a.Id).ToList();
                    decimal? summIn = db2.OrderToGo
                        .Where(a => a.DeliveryDate >= cust.CashBackStartDate && a.OrderStatus > 2 && (a.OrderCustomerId ?? 0) == orderCustomerId
                        && cashbackMarks.Contains(a.MarketingChannelId ?? 0)
                        && saleGroups.Contains(a.PaymentId ?? 0)
                        && a.DishPackages.Count>0)
                        .Sum(a => (a.DishPackages.Where(c => !c.Deleted).Sum(b => b.TotalPrice * b.Amount)) * (1 - (a.DiscountPercent / 100)) + a.DeliveryPrice);
                    log.Debug($"Calc cashback summIn {summIn}");
                    decimal? summOut = db2.OrderToGo
                        .Where(a => a.DeliveryDate >= cust.CashBackStartDate && a.OrderStatus > 2 && (a.OrderCustomerId ?? 0) == orderCustomerId && (a.PaymentId ?? 0 ) == 38  && a.DishPackages.Count > 0)
                        .Sum(a => (a.DishPackages.Sum(b => (decimal?)b.TotalPrice * (decimal?)b.Amount)) * (1 - ((decimal?)a.DiscountPercent / 100)) + (decimal?)a.DeliveryPrice) ;

                    log.Debug($"Calc cashback for {cust.Name} sumIn:{summIn??0}; sumOut:{summOut??0}");

                    info.CashBackSumm = (summIn ??0)*cust.CashBackPercent/100
                        -
                        (summOut??0)
                        ;
                }
                cust.LastUpdatedSession = Guid.NewGuid();
                cust.UpdatedDate = DateTime.Now;

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