using AlohaService.ServiceDataContracts;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AlohaFly.Analytics
{
    class AirOrdersInfo : Collection<AirOrderInfo>
    {
        public DateTime PeriodStart { set; get; } = new DateTime(2018, 11, 1);
        public DateTime PeriodEnd { set; get; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1);
        public AirOrdersInfo()
        {

            OrderStatus ordS = OrderStatus.CancelledWithRemains | OrderStatus.Closed | OrderStatus.InWork | OrderStatus.Sent;
            foreach (var ord in ServerDataSingleton.Instance.GetAllAirOrders(ordS, PeriodStart, PeriodEnd))
            {
                this.Add(new AirOrderInfo(ord));
            }


        }
    }

    public class AirOrderInfo
    {
        OrderFlight Ord = new OrderFlight();
        public AirOrderInfo(OrderFlight ord)
        {
            Ord = ord;
        }

        [Display(Name = "Номер рейса")]
        public string FlightNumber { get { return Ord.FlightNumber; } }
        [Display(Name = "Авиакомпания")]
        public string AirCompName { get { return DataExtension.DataCatalogsSingleton.Instance.AirCompanyData.Data.SingleOrDefault(a => a.Id == Ord.AirCompanyId).Name; } }
        [Display(Name = "Дата")]
        public DateTime DelDate { get { return Ord.DeliveryDate; } }
        [Display(Name = "Статус заказа")]
        public OrderStatus OrderStatus { get { return Ord.OrderStatus; } }
        [Display(Name = "Контактное лицо")]
        public string ContactName { get { return DataExtension.DataCatalogsSingleton.Instance.ContactPersonData.Data.SingleOrDefault(a => a.Id == Ord.ContactPersonId).FullName; } }
        [Display(Name = "Сумма заказа")]
        public decimal Total { get { return Ord.OrderTotalSumm; } }
        [Display(Name = "Скидка")]
        public decimal Discount { get { return Ord.DiscountSumm; } }
        [Display(Name = "Итого")]
        public decimal Count { get { return 1; } }


    }
}
