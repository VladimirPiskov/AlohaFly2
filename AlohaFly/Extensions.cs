using AlohaFly.DataExtension;
using AlohaService.ServiceDataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace AlohaFly
{
    public static class Extensions
    {
        public static decimal GetOrderDishesSummByAlc(this OrderFlight orderFlight, bool alc)
        {
            if (orderFlight.DishPackages == null) return 0;
            if (orderFlight.OrderStatus == OrderStatus.Cancelled) return 0;
            return orderFlight.DishPackagesNoSpis.Where(a => a.Dish?.IsAlcohol == alc).Sum(a => a.TotalSumm);
        }

        public static decimal GetOrderSummByAlc(this OrderFlight orderFlight, bool alc)
        {
            if (orderFlight.DishPackages == null) return 0;
            if (orderFlight.OrderStatus == OrderStatus.Cancelled) return 0;
            return orderFlight.GetOrderDishesSummByAlc(alc) * (1 + orderFlight.ExtraCharge / 100);
        }

        public static decimal GetOrderTotalSummByAlc(this OrderFlight orderFlight, bool alc)
        {
            if (orderFlight.DishPackages == null) return 0;
            if (orderFlight.OrderStatus == OrderStatus.Cancelled) return 0;
            return orderFlight.GetOrderSummByAlc(alc) - orderFlight.GetDiscountAlc(alc);
        }

        public static void UpDateSpisPayment(this AlohaService.Interfaces.IDeletedDish orderFlightOrder)
        {
            orderFlightOrder.SpisPayment = DataCatalogsSingleton.Instance.GetPayment(orderFlightOrder.SpisPaymentId);


        }

        public static List<AlohaService.Interfaces.IDishPackageLabel> GetSpisDishesOfPaimentId(this AlohaService.Interfaces.IOrderLabel orderFlightOrder, long pId)
        {

            if (orderFlightOrder.DishPackagesSpis == null || !orderFlightOrder.DishPackagesSpis.Any()) { return new List<AlohaService.Interfaces.IDishPackageLabel>(); }
            if (pId == 0)
            {
                return orderFlightOrder.DishPackagesSpis.Where(x => x.Deleted && x.DeletedStatus == 1).ToList();
            }
            return orderFlightOrder.DishPackagesSpis.Where(x => x.Deleted && x.DeletedStatus == 1 && x.SpisPaymentId == pId).ToList();
        }

        public static List<AlohaService.Interfaces.IDishPackageLabel> GetNoSpisDishesOfCat(this AlohaService.Interfaces.IOrderLabel orderFlightOrder, long? cId)
        {
            if (orderFlightOrder.DishPackagesForLab == null || !orderFlightOrder.DishPackagesForLab.Any()) { return new List<AlohaService.Interfaces.IDishPackageLabel>(); }
            return orderFlightOrder.DishPackagesNoSpis.Where(x => x.Dish.DishLogicGroupId == cId).ToList();
        }

        public static decimal GetNoSpisDishesOfCatSum(this AlohaService.ServiceDataContracts.OrderFlight orderFlightOrder, long? cId)
        {

            if (orderFlightOrder.DishPackagesForLab == null || !orderFlightOrder.DishPackagesForLab.Any()) { return 0; }
            return orderFlightOrder.DishPackagesNoSpis.Where(x => x.Dish.DishLogicGroupId == cId).Sum(a => a.TotalSumm) * (orderFlightOrder.OrderSumm == 0 ? 1 : (orderFlightOrder.OrderTotalSumm / orderFlightOrder.OrderSumm));
        }




        public static decimal GetDiscountAlc(this OrderFlight orderFlight, bool alc)
        {
            if (orderFlight.DishPackages == null) return 0;
            if (orderFlight.OrderStatus == OrderStatus.Cancelled) return 0;
            if (orderFlight.OrderDishesSumm == 0) return 0;
            if (!alc)
            {
                if (orderFlight.GetOrderSummByAlc(true) == 0)
                {
                    return orderFlight.DiscountSumm;
                }
                else
                {
                    return Math.Round(orderFlight.DiscountSumm * (orderFlight.GetOrderSummByAlc(false) / orderFlight.OrderDishesSumm));
                }
            }
            else
            {
                return orderFlight.DiscountSumm - orderFlight.GetDiscountAlc(false);
            }

        }

        public static bool IsDop(this Dish d)
        {


            return d.DishLogicGroupId == MainClass.DopLogikCatId;

        }



    }
}
