using AlohaFly.DataExtension;
using AlohaFly.Models;
using AlohaService.ServiceDataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Windows.Controls;

namespace AlohaFly.Analytics
{
    public class ServerDataSingleton : ViewModelBase
    {

        private ServerDataSingleton()
        {

        }

        static ServerDataSingleton instance;
        public static ServerDataSingleton Instance
        {

            get
            {
                if (instance == null)
                {
                    instance = new ServerDataSingleton();
                }
                return instance;
            }
        }

        private DateTime periodStartDate;
        public DateTime PeriodStartDate
        {
            get { return this.periodStartDate; }

            set
            {
                if (this.periodStartDate != value)
                {
                    this.periodStartDate = value;
                    this.OnPropertyChanged(nameof(PeriodStartDate));

                }
            }
        }

        private DateTime periodEndDate;
        public DateTime PeriodEndDate
        {
            get { return this.periodEndDate; }

            set
            {
                if (this.periodEndDate != value)
                {
                    this.periodEndDate = value;
                    this.OnPropertyChanged(nameof(PeriodEndDate));

                }
            }
        }

        public void CheckStardDate(DateTime startDate)
        {
            DataCatalogsSingleton.Instance.OrdersFlightData.ChangeStartDate(startDate);
        }

        public List<RevenueInfo> GetRevenueSaleGastroInfos(OrderStatus orderStatus, DateTime startDate, DateTime endDate)
        {
            var tmp = new List<RevenueInfo>();
            
            for (var dt = startDate; dt < endDate; dt = dt.AddDays(1))
            {
                List<OrderFlight> ord = DataCatalogsSingleton.Instance.OrdersFlightData.Data.Where(
                    a => a.DeliveryDate >= dt &&
                    a.DeliveryDate < dt.AddDays(1) &&
                    !DBProvider.SharAirs.Contains((int)a.AirCompanyId) &&

                    a.AirCompany != null &&
                    a.AirCompanyId == MainClass.AirGastroFoodId &&
                    a.AirCompany.PaymentType != null &&
                    a.AirCompany.PaymentType.PaymentGroup != null &&
                    //a.AirCompany.PaymentType.PaymentGroup.Sale &&

                    orderStatus.HasFlag(a.OrderStatus)).ToList();

                var r = new RevenueInfo(dt, ord.Sum(a => a.OrderTotalSumm), ord.Count(), ord.Sum(a => a.DiscountSumm));

                tmp.Add(r);

            }
            return tmp;

        }

        public List<RevenueInfo> GetRevenueSaleAirInfos(OrderStatus orderStatus, DateTime startDate, DateTime endDate)
        {
            var tmp = new List<RevenueInfo>();
            for (var dt = startDate; dt < endDate; dt = dt.AddDays(1))
            {
                List<OrderFlight> ord = DataCatalogsSingleton.Instance.OrdersFlightData.Data.Where(
                    a => a.DeliveryDate >= dt &&
                    a.DeliveryDate < dt.AddDays(1) &&
                    !DBProvider.SharAirs.Contains((int)a.AirCompanyId) &&

                    a.AirCompany != null &&
                    a.AirCompanyId != MainClass.AirGastroFoodId &&
                    a.AirCompany.PaymentType != null &&
                    a.AirCompany.PaymentType.PaymentGroup != null &&
                    a.AirCompany.PaymentType.PaymentGroup.Sale &&

                    orderStatus.HasFlag(a.OrderStatus)).ToList();

                var r = new RevenueInfo(dt, ord.Sum(a => a.OrderTotalSumm), ord.Count(), ord.Sum(a => a.DiscountSumm));

                tmp.Add(r);

            }
            return tmp;

        }
        

        public List<RevenueInfo> GetRevenueSpisAirInfos(OrderStatus orderStatus, DateTime startDate, DateTime endDate)
        {
            var tmp = new List<RevenueInfo>();
            for (var dt = startDate; dt < endDate; dt = dt.AddDays(1))
            {
                List<OrderFlight> ord = DataCatalogsSingleton.Instance.OrdersFlightData.Data.Where(
                    a => a.DeliveryDate >= dt &&
                    a.DeliveryDate < dt.AddDays(1) &&
                    !DBProvider.SharAirs.Contains((int)a.AirCompanyId) &&
                    a.AirCompanyId != MainClass.AirGastroFoodId &&
                    a.AirCompany != null &&
                    a.AirCompany.PaymentType != null &&
                    a.AirCompany.PaymentType.PaymentGroup != null &&
                    !a.AirCompany.PaymentType.PaymentGroup.Sale &&

                    orderStatus.HasFlag(a.OrderStatus)).ToList();

                var r = new RevenueInfo(dt, ord.Sum(a => a.OrderTotalSumm), ord.Count(), ord.Sum(a => a.DiscountSumm));

                tmp.Add(r);

            }
            return tmp;

        }


        public List<OrderToGo> GetAllToGoOrders(OrderStatus orderStatus, DateTime startDate, DateTime endDate)
        {

            DataCatalogsSingleton.Instance.OrdersToGoData.ChangeStartDate(startDate);

            var Orderstmp = new List<OrderToGo>();

            for (var dt = startDate; dt < endDate; dt = dt.AddDays(1))
            {

                List<OrderToGo> ord = DataCatalogsSingleton.Instance.OrdersToGoData.Data.Where(
                     a => a.DeliveryDate >= dt &&
                     a.DeliveryDate < dt.AddDays(1) &&

                     orderStatus.HasFlag(a.OrderStatus)).ToList();
                Orderstmp.AddRange(ord);
            }
            return Orderstmp;


        }


        public List<OrderFlight> GetAllAirOrdersWithShar(OrderStatus orderStatus, DateTime startDate, DateTime endDate)
        {
            var Orderstmp = new List<OrderFlight>();
            for (var dt = startDate; dt < endDate; dt = dt.AddDays(1))
            {
                List<OrderFlight> ord = DataCatalogsSingleton.Instance.OrdersFlightData.Data.Where(
                     a => a.DeliveryDate >= dt &&
                     a.DeliveryDate < dt.AddDays(1) &&
                     a.AirCompanyId != MainClass.AirGastroFoodId &&
                     //!DBProvider.SharAirs.Contains((int)a.AirCompanyId) &&
                     orderStatus.HasFlag(a.OrderStatus)).ToList();
                Orderstmp.AddRange(ord);
            }
            return Orderstmp;
        }


        public List<OrderFlight> GetAllAirOrders(OrderStatus orderStatus, DateTime startDate, DateTime endDate)
        {
            var Orderstmp = new List<OrderFlight>();
            for (var dt = startDate; dt < endDate; dt = dt.AddDays(1))
            {
                List<OrderFlight> ord = DataCatalogsSingleton.Instance.OrdersFlightData.Data.Where(
                     a => a.DeliveryDate >= dt &&
                     a.DeliveryDate < dt.AddDays(1) &&
                     !DBProvider.SharAirs.Contains((int)a.AirCompanyId) &&
                     a.AirCompanyId != MainClass.AirGastroFoodId &&
                     orderStatus.HasFlag(a.OrderStatus)).ToList();
                Orderstmp.AddRange(ord);
            }
            return Orderstmp;
        }



        public List<OrderFlight> GetAllSaleSharOrders(OrderStatus orderStatus, DateTime startDate, DateTime endDate)
        {
            var Orderstmp = new List<OrderFlight>();
            for (var dt = startDate; dt < endDate; dt = dt.AddDays(1))
            {
                List<OrderFlight> ord = DataCatalogsSingleton.Instance.OrdersFlightData.Data.Where(
                     a => a.DeliveryDate >= dt &&
                     a.DeliveryDate < dt.AddDays(1) &&
                     DBProvider.SharAirs.Contains((int)a.AirCompanyId) &&
                     a.AirCompany != null &&
                    a.AirCompany.PaymentType != null &&
                    a.AirCompany.PaymentType.PaymentGroup != null &&
                    a.AirCompany.PaymentType.PaymentGroup.Sale &&

                     orderStatus.HasFlag(a.OrderStatus)).ToList();
                Orderstmp.AddRange(ord);
            }
            return Orderstmp;
        }



        public List<OrderFlight> GetAllSpisSharOrders(OrderStatus orderStatus, DateTime startDate, DateTime endDate)
        {
            var Orderstmp = new List<OrderFlight>();
            for (var dt = startDate; dt < endDate; dt = dt.AddDays(1))
            {
                List<OrderFlight> ord = DataCatalogsSingleton.Instance.OrdersFlightData.Data.Where(
                     a => a.DeliveryDate >= dt &&
                     a.DeliveryDate < dt.AddDays(1) &&
                     DBProvider.SharAirs.Contains((int)a.AirCompanyId) &&
                     a.AirCompany != null &&
                    a.AirCompany.PaymentType != null &&
                    a.AirCompany.PaymentType.PaymentGroup != null &&
                    !a.AirCompany.PaymentType.PaymentGroup.Sale &&

                     orderStatus.HasFlag(a.OrderStatus)).ToList();
                Orderstmp.AddRange(ord);
            }
            return Orderstmp;
        }



        public List<AirCompanyRevenueInfo> GetCompanyRevenueInfos(OrderStatus orderStatus, DateTime startDate, DateTime endDate)
        {
            var tmp = new List<AirCompanyRevenueInfo>();
            var Orderstmp = new List<OrderFlight>();

            for (var dt = startDate; dt < endDate; dt = dt.AddDays(1))
            {
                List<OrderFlight> ord = DataCatalogsSingleton.Instance.OrdersFlightData.Data.Where(
                     a => a.DeliveryDate >= dt &&
                     a.DeliveryDate < dt.AddDays(1) &&


                      !DBProvider.SharAirs.Contains((int)a.AirCompanyId) &&

                    a.AirCompany != null &&
                    a.AirCompanyId != MainClass.AirGastroFoodId &&
                    a.AirCompany.PaymentType != null &&
                    a.AirCompany.PaymentType.PaymentGroup != null &&
                    a.AirCompany.PaymentType.PaymentGroup.Sale &&

                     orderStatus.HasFlag(a.OrderStatus)).ToList();
                Orderstmp.AddRange(ord);
            }
            foreach (long compN in Orderstmp.Select(a => a.AirCompanyId).Distinct())
            {
                tmp.Add(new AirCompanyRevenueInfo()
                {
                    Cname = DataExtension.DataCatalogsSingleton.Instance.AirCompanyData.Data.SingleOrDefault(a => a.Id == compN)?.Name,
                    Revenue = Orderstmp.Where(a => a.AirCompanyId == compN).Sum(a => a.OrderTotalSumm),
                });
            }

            return tmp.OrderByDescending(a => a.Revenue).ToList();

        }

        public void UpdateData()
        {

        }
    }
}
