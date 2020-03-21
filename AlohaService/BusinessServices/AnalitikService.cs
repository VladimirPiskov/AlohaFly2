using AlohaService.Helpers;
using AlohaService.ServiceDataContracts;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlohaService.BusinessServices
{
    public class AnalitikService
    {
            private AlohaDb db;
            protected ILog log;

            public AnalitikService(AlohaDb databaseContext)
            {
                db = databaseContext;
                LogHelper.Configure();
                log = LogHelper.GetLogger();
            }


        public OperationResultValue<AnalitikData> GetLTVValues2(DateTime sDate, DateTime eDate)
        {
            var res = new OperationResultValue<AnalitikData>()
            {
                Result = new AnalitikData() { sDate = sDate, eDate = eDate },
                Success = false
            };

            try
            {

                var customers = db.OrderToGo.Where(a => a.DeliveryDate >= sDate && a.DeliveryDate <= eDate && a.OrderStatus == 16
                && a.MarketingChannelId != 5) //Яндекс еда          
                    .GroupBy(a => a.OrderCustomerId)
                    .Select(a => new { custId = a.Key, c = a.Count() })
                    .Where(a => a.c > 1).Select(a => a.custId);


                for (var d = sDate; d < eDate; d = d.AddMonths(1))
                {
                    DateTime ed = d.AddMonths(1);
                    var ords = db.OrderToGo.Where(a => a.DeliveryDate >= sDate && a.DeliveryDate < ed && customers.Contains(a.OrderCustomerId));

                    var d1 = ords.Sum(a => a.DishPackages.Sum(b => b.TotalPrice * b.Amount) * (1 - (a.DiscountPercent / 100)));
                    var d2 = ords.Select(a => a.OrderCustomerId).Distinct().Count();

                    res.Result.Data.Add(
                    new AnalitikDataRecord()
                    {
                        Date = d,
                        Value = d2>0?d1/d2:0,
                        Data1 = d1,
                        Data2 = d2
                    }

                    );
                }
            }
            catch (Exception e)
            {
                res.ErrorMessage = e.Message;
            }
            res.Success = true;
            return res;
        }

        public OperationResultValue<AnalitikData> GetLTVValues(DateTime sDate, DateTime eDate)
        {
            var res = new OperationResultValue<AnalitikData>()
            {
                Result = new AnalitikData() { sDate = sDate, eDate = eDate },
                Success = false
            };

            try
            {

                var customers = db.OrderToGo.Where(a => a.DeliveryDate >= sDate && a.DeliveryDate <= eDate && a.OrderStatus == 16
                && a.MarketingChannelId != 5) //Яндекс еда          
                    .GroupBy(a => a.OrderCustomerId)
                    .Select(a => new { custId = a.Key,  c = a.Count() })
                    .Where(a => a.c > 1 ).Select(a => a.custId);


                for (var d = sDate; d < eDate; d = d.AddMonths(1))
                {
                    DateTime ed = d.AddMonths(1);
                    var ords = db.OrderToGo.Where(a => a.DeliveryDate >= d && a.DeliveryDate < ed 
                    && a.OrderStatus == 16
                && a.MarketingChannelId != 5
                
                && customers.Contains(a.OrderCustomerId));

                    var d1 = ords.Sum(a => a.DishPackages.Sum(b => b.TotalPrice * b.Amount) * (1 - (a.DiscountPercent / 100)));
                    var d2 = ords.Select(a => a.OrderCustomerId).Distinct().Count();

                    res.Result.Data.Add(
                    new AnalitikDataRecord()
                    {
                        Date = d,
                        Value = d2 > 0 ? d1 / d2 : 0,
                        Data1 = d1,
                        Data2 = d2
                    }

                    );
                }
            }
            catch(Exception e)
            {
                res.ErrorMessage = e.Message;
            }
            res.Success = true;
            return res;
        }
        public OperationResultValue<List<AnalitikOrderData>> GetAllToGoOrdersData(DateTime sDate, DateTime eDate)
        {
            var res = new OperationResultValue<List<AnalitikOrderData>>()
            {
                Result = new List<AnalitikOrderData>() ,
                Success = false
            };
            try
            {
                var customers = db.OrderToGo.Where(a => a.DeliveryDate >= sDate && a.DeliveryDate <= eDate && a.OrderStatus == 16 && a.OrderCustomerId!=null
                && db.Payments.Where(b => b.PaymentGroup.Sale).Any(b => b.Id == a.PaymentId) // Только продажи
                && a.MarketingChannelId != 5) //Яндекс еда          
                    .GroupBy(a => a.OrderCustomerId)
                    .Select(a => new { custId = a.Key, c = a.Count() })
                    .Where(a => a.c > 1).Select(a => a.custId);

                res.Result = db.OrderToGo.Where(a => a.DeliveryDate >= sDate && a.DeliveryDate < eDate
                    && a.OrderStatus == 16
                && a.MarketingChannelId != 5 //Яндекс еда          
                && a.OrderCustomerId!= 50 //AKINGUMP
                && db.Payments.Where(b=>b.PaymentGroup.Sale).Any(b=>b.Id==a.PaymentId) // Только продажи
                && customers.Contains(a.OrderCustomerId))
                    .Select(a => new AnalitikOrderData()
                    {
                        CustomerID = a.OrderCustomerId,
                        DeleveryDate = a.DeliveryDate,
                        MarketingChanel = a.MarketingChannelId,
                        PaymentId = a.PaymentId,
                        Summ = a.DishPackages.Any() ? a.DishPackages.Sum(b => b.TotalPrice * b.Amount) * (1 - (a.DiscountPercent / 100)):0
                    })
                    .ToList();

                res.Success = true;
            }
            catch (Exception e)
            {
                res.ErrorMessage = e.Message;
            }
            
            return res;
        }

    }
}