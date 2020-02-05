using AlohaService.ServiceDataContracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlohaFly
{
    public static class Calc
    {

        public static void CalkDiscounts(List<OrderFlight> orders)
        {
            foreach (int arcId in orders.Where(a => a.OrderStatus != OrderStatus.Closed && (a.AirCompany?.DiscountType == null) && a.AirCompanyId != MainClass.AirAvangardId).Select(a => a.AirCompany.Id).Distinct())
            {
                foreach (var ord in orders.Where(a => a.AirCompany.Id == arcId).OrderBy(a => a.DeliveryDate))
                {
                    if (ord.OrderStatus != OrderStatus.Closed)
                    {
                        ord.DiscountSumm = 0;
                    }
                }
            }

            foreach (int arcId in orders.Where(a => a.OrderStatus != OrderStatus.Closed && (a.AirCompany?.DiscountType != null)).Select(a => a.AirCompany.Id).Distinct())
            {

                if (arcId == MainClass.AirAvangardId)//Это схема Авангарда
                {
                    decimal AvSumm = orders.Where(a => a.AirCompany.Id == arcId).Sum(a => a.OrderSumm);
                    decimal NeedDiscPercent = AvSumm > 800000 ? 0.10M : (AvSumm > 500000 ? 0.08M : 0.05M);
                    decimal NeedDiscSumm = AvSumm * NeedDiscPercent;
                    decimal AlreadyDisc = orders.Where(a => a.AirCompany.Id == arcId && a.OrderStatus == OrderStatus.Closed).Sum(a => a.DiscountSumm);
                    decimal NeedDiscForClosed = orders.Where(a => a.AirCompany.Id == arcId && a.OrderStatus == OrderStatus.Closed).Sum(a => a.OrderSumm) * NeedDiscPercent;
                    decimal NeedMoreDisc = NeedDiscForClosed - AlreadyDisc;

                    long ordForDiscId = 0;
                    if (NeedMoreDisc > 0)
                    {

                        if (orders.Any(a => a.AirCompany.Id == arcId && a.OrderStatus != OrderStatus.Closed && a.OrderStatus != OrderStatus.Cancelled && a.OrderSumm * (1 - NeedDiscPercent) > NeedMoreDisc))
                        {
                            var ordForDisc = orders.Where(a => a.AirCompany.Id == arcId && a.OrderStatus != OrderStatus.Closed && a.OrderStatus != OrderStatus.Cancelled && a.OrderSumm > NeedMoreDisc)
                               .OrderBy(a => a.DeliveryDate).FirstOrDefault();
                            ordForDiscId = ordForDisc.Id;
                            var disc = ordForDisc.OrderSumm * NeedDiscPercent + NeedMoreDisc;
                            disc = Math.Round(disc, 2);
                            if (disc != ordForDisc.DiscountSumm)
                            {
                                ordForDisc.DiscountSumm = disc;
                                DBProvider.Client.UpdateOrderFlight(ordForDisc, Authorization.CurentUser.Id);
                            }


                        }


                    }

                    foreach (var ord in orders.Where(a => a.AirCompany.Id == arcId).OrderBy(a => a.Id))
                    {
                        if (ord.OrderStatus != OrderStatus.Closed && ord.OrderStatus != OrderStatus.Cancelled && ord.Id != ordForDiscId)
                        {
                            var disc = ord.OrderSumm * NeedDiscPercent;

                            disc = Math.Round(disc, 2);
                            if (disc != ord.DiscountSumm)
                            {
                                ord.DiscountSumm = disc;
                                DBProvider.Client.UpdateOrderFlight(ord, Authorization.CurentUser.Id);
                            }
                        }
                    }



                }
                else
                {

                    decimal summ = 0;
                    foreach (var ord in orders.Where(a => a.AirCompany.Id == arcId).OrderBy(a => a.Id))
                    {
                        if (ord.OrderStatus != OrderStatus.Closed)
                        {
                            if (ord.OrderStatus == OrderStatus.Cancelled) { ord.DiscountSumm = 0; }

                            //  var ranges = GetRangeofOrder(summ, summ + ord.OrderSumm, ord.AirCompany.DiscountType.Id);
                            var ranges = GetRangeofOrder(summ, ord.OrderSumm, ord.AirCompany.DiscountType.Id);
                            if (ranges != null)
                            {
                                decimal disc = 0;
                                foreach (var r in ranges)
                                {
                                    if (r.End == null)
                                    {
                                        disc += ((summ + ord.OrderSumm) - Math.Max(r.Start, summ)) * r.DiscountPercent / 100;
                                    }
                                    else
                                    {
                                        disc += (Math.Min((summ + ord.OrderSumm), r.End.Value) - Math.Max(r.Start, summ)) * r.DiscountPercent / 100;
                                    }
                                }
                                disc = Math.Round(disc, 2);
                                if (disc != ord.DiscountSumm)
                                {
                                    ord.DiscountSumm = disc;
                                    DBProvider.Client.UpdateOrderFlight(ord, Authorization.CurentUser.Id);
                                }
                            }

                        }
                        summ += ord.OrderSumm;

                    }
                }
            }
        }

        public static void CalcOrderDisc(OrderFlight ord)
        {
            if (ord == null) return;


            if (ord.OrderSumm == 0) { ord.DiscountSumm = 0; return; };
            if (ord.AirCompany == null) return;
            if (ord.AirCompanyId == MainClass.AirAvangardId) return;
            if (ord.AirCompany.DiscountId == null) return;
            decimal summ = Models.AirOrdersModelSingleton.Instance.GetOrdersOfMonth(ord.DeliveryDate)
                .Where(a => a.AirCompany?.Id == ord.AirCompany?.Id && a.Id < ord.Id)
                .Sum(a => a.OrderSumm);
            if (ord.OrderStatus != OrderStatus.Closed)
            {
                if (ord.OrderStatus == OrderStatus.Cancelled) { ord.DiscountSumm = 0; }

                //  var ranges = GetRangeofOrder(summ, summ + ord.OrderSumm, ord.AirCompany.DiscountType.Id);
                var ranges = GetRangeofOrder(summ, ord.OrderSumm, ord.AirCompany.DiscountType.Id);
                if (ranges != null)
                {
                    decimal disc = 0;
                    foreach (var r in ranges)
                    {
                        if (r.End == null)
                        {
                            disc += ((summ + ord.OrderSumm) - Math.Max(r.Start, summ)) * r.DiscountPercent / 100;
                        }
                        else
                        {
                            disc += (Math.Min((summ + ord.OrderSumm), r.End.Value) - Math.Max(r.Start, summ)) * r.DiscountPercent / 100;
                        }
                    }
                    disc = Math.Round(disc, 2);
                    if (disc != ord.DiscountSumm)
                    {
                        ord.DiscountSumm = disc;
                        //DBProvider.Client.UpdateOrderFlight(ord, Authorization.CurentUser.Id);
                    }
                }
                //summ += ord.OrderSumm;
            }
        }

        static List<DiscountRange> GetRangeofOrder(decimal beforesumm, decimal ordersumm, long discId)
        {
            try
            {
                var ranges = DataExtension.DataCatalogsSingleton.Instance.DiscountData.Data.Single(a => a.Id == discId).Ranges.ToList();
                var res1 = ranges.OrderBy(a => a.Start).Where(a => a.Start <= beforesumm).Last();
                var res2 = ranges.OrderBy(a => a.Start).Where(a => a.Start < beforesumm + ordersumm).Last();
                if ((res1 == null) && (res2 == null)) return null;
                if ((res1 == null) && (res2 != null)) return ranges.OrderBy(a => a.Start).TakeWhile(a => a != res2).ToList();
                return ranges.OrderBy(a => a.Start).SkipWhile(a => a.Start < res1.Start).TakeWhile(a => a.Start <= res2.Start).ToList();
            }
            catch
            {
                return null;
            }

        }

        static decimal GetDiscSumm(AirCompany aircomp, decimal monthSumm)
        {
            if (aircomp.DiscountType == null) return 0;
            decimal res = 0;

            foreach (var r in aircomp.DiscountType.Ranges.OrderBy(a => a.Start))
            {
                if (r.Start > monthSumm) break;
                if (r.End == null)
                {
                    res += (monthSumm - r.Start) * r.DiscountPercent / 100;
                }
                else
                {
                    res += (Math.Min((decimal)r.End, monthSumm) - r.Start) * r.DiscountPercent / 100;
                }
            }
            return res;
        }
    }
}
