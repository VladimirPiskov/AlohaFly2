using AlohaService.ServiceDataContracts;
using System;

namespace AlohaFly.Analytics
{
    public class RevenueInfo
    {
        public RevenueInfo(DateTime date, decimal revenue, int ordersCount, decimal discount)
        {
            Date = date;
            Revenue = revenue;
            OrdersCount = ordersCount;
            Discount = discount;
        }


        public DateTime Date;
        public bool Alert;
        public OrderStatus OrderStatus;
        public decimal Revenue;
        public decimal Discount;
        public decimal OrdersCount;
    }
    public class AirCompanyRevenueInfo
    {
        public AirCompanyRevenueInfo()
        { }

        public string Cname { set; get; }
        public decimal Revenue { set; get; }
    }



}
