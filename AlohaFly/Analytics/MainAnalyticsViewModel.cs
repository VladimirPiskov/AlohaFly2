using AlohaFly.Models;
using AlohaService.ServiceDataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Windows.Data;

namespace AlohaFly.Analytics
{
    class MainAnalyticsViewModel : ViewModelPane
    {

        public MainAnalyticsViewModel()
        {
            SelectedStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            SelectedEndDate = DateTime.Now.Date.AddDays(1);
        }

        public DateTime PeriodStart { set; get; } = new DateTime(2018, 11, 1);
        public DateTime PeriodEnd { set; get; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1);
        public DateTime VisiblePeriodStart { set; get; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-4);
        public DateTime VisiblePeriodEnd { set; get; } = DateTime.Now.Date.AddDays(4);
        public TimeSpan MinSelectionRange = new TimeSpan(24, 0, 0);


        private DateTime selectedStartDate;
        public DateTime SelectedStartDate
        {
            get { return this.selectedStartDate; }

            set
            {
                if (this.selectedStartDate != value)
                {
                    this.selectedStartDate = value;
                    this.OnPropertyChanged("SelectedStartDate");
                    UpdateSumms();



                }
            }
        }

        private DateTime selectedEndDate;
        public DateTime SelectedEndDate
        {
            get { return this.selectedEndDate; }

            set
            {
                if (this.selectedEndDate != value)
                {
                    this.selectedEndDate = value;
                    this.OnPropertyChanged("SelectedEndDate");

                    UpdateSumms();


                }
            }
        }

        private void UpdateSumms()
        {

            this.OnPropertyChanged("SaleAirSummStr");
            this.OnPropertyChanged("AvgSaleAirSummStr");
            this.OnPropertyChanged("AlertAirOrdersStr");
            this.OnPropertyChanged("DaysAirOrdersStr");
            this.OnPropertyChanged("CancelledAirOrdersStr");
            this.OnPropertyChanged("NightAirOrdersStr");
            this.OnPropertyChanged("SaleSharAirSummStr");


            this.OnPropertyChanged("SaleToGoSummStr");
            this.OnPropertyChanged("AvgSaleToGoSummStr");
            this.OnPropertyChanged("SaleToGoSummNonClosedStr");
            this.OnPropertyChanged("DaysToGoOrdersStr");
            this.OnPropertyChanged("CancelledToGoOrdersStr");
            this.OnPropertyChanged("NightToGoOrdersStr");
            this.OnPropertyChanged("SaleToGoSummNonSendStr");

            this.OnPropertyChanged("AvgSharSummStr");
            this.OnPropertyChanged("SaleSharSummStr");
            this.OnPropertyChanged("SpisSharSummStr");


            this.OnPropertyChanged("SpisToGoSummStr");
            this.OnPropertyChanged("SpisToFlySummStr");

            this.OnPropertyChanged("ToGoChanelsRevenueInfos");
            this.OnPropertyChanged("CompanyRevenueInfos");
            this.OnPropertyChanged("SharRevenueInfos");
            this.OnPropertyChanged("SpisRevenueInfos");

            this.OnPropertyChanged("DishAInfosToFly");
            this.OnPropertyChanged("DishAInfosToGo");

            this.OnPropertyChanged("GastroSummStr");




        }


        #region Sale Air

        public List<OrderFlight> SaleAirOrders
        {
            get
            {
                return AirOrders.Where(a =>
                     a.AirCompany != null &&
                     a.AirCompanyId != MainClass.AirGastroFoodId &&
                    a.AirCompany.PaymentType != null &&
                    a.AirCompany.PaymentType.PaymentGroup != null &&
                    a.AirCompany.PaymentType.PaymentGroup.Sale
                    ).ToList();
            }
        }

        List<RevenueInfo> SelectedRevenueSaleAirInfos
        {
            get
            {
                return RevenueSaleAirInfos.Where(a => a.Date >= SelectedStartDate && a.Date < SelectedEndDate).ToList();
            }
        }

        public decimal GastroSaleAirSumm
        {
            get
            {
                return GastroSaleAirInfos.Sum(a => a.Revenue);
            }
        }

        public decimal RevenueSaleAirSumm
        {
            get
            {
                return RevenueSaleAirInfos.Sum(a => a.Revenue);
            }
        }
        public decimal SaleAirOrdersCount
        {
            get
            {
                return RevenueSaleAirInfos.Sum(a => a.OrdersCount);
            }
        }
        public decimal SaleGastroOrdersCount
        {
            get
            {
                return GastroSaleAirInfos.Sum(a => a.OrdersCount);
            }
        }

        public string SaleAirSummStr
        {
            get
            {
                return $" {RevenueSaleAirSumm.ToString("C2")} ({SaleAirOrdersCount})";
            }
        }

        public string SaleSharAirSummStr
        {
            get
            {
                return $"{SaleAirOrders.Where(a => a.CreatedById == DBProvider.SharUserId).Sum(a => a.OrderTotalSumm).ToString("C2")} ({SaleAirOrders.Where(a => a.CreatedById == DBProvider.SharUserId).Count()})";
            }
        }

        public string AvgSaleAirSummStr
        {
            get
            {
                decimal avg = SaleAirOrdersCount == 0 ? 0 : RevenueSaleAirSumm / SaleAirOrdersCount;
                return $"{(avg).ToString("C2")}";
            }
        }

        public string AlertAirOrdersStr //Срочные заказы без шереметьева и списаний
        {
            get
            {
                return $"{SaleAirOrders.Where(a => a.ExtraCharge > 0).Sum(a => a.OrderTotalSumm).ToString("C2")} ({SaleAirOrders.Where(a => a.ExtraCharge > 0).Count()})";
            }
        }

        public string CancelledAirOrdersStr //Отмененные заказы без шереметьева и списаний
        {
            get
            {
                return $" {CancelledOrdersList.Where(a => a.OrderStatus == OrderStatus.Cancelled).Sum(a => a.CancelledOrderDishesSumm).ToString("C2")} ({CancelledOrdersList.Where(a => a.OrderStatus == OrderStatus.Cancelled).Count()})";
            }
        }


        public string DaysAirOrdersStr //Дневные заказы без шереметьева и списаний
        {
            get
            {
                return $" {SaleAirOrders.Where(a => a.DeliveryDate.Hour >= 11).Sum(a => a.OrderTotalSumm).ToString("C2")} ({SaleAirOrders.Where(a => a.DeliveryDate.Hour >= 11).Count()})";
            }
        }

        public string NightAirOrdersStr //Ночные заказы без шереметьева и списаний
        {
            get
            {
                return $" {SaleAirOrders.Where(a => a.DeliveryDate.Hour < 11).Sum(a => a.OrderTotalSumm).ToString("C2")} ({SaleAirOrders.Where(a => a.DeliveryDate.Hour < 11).Count()})";
            }
        }

        public string GastroSummStr
        {
            get
            {
                return $" {GastroSaleAirSumm.ToString("C2")} ({SaleGastroOrdersCount})";
            }
        }


        #endregion


        #region Shar

        public List<OrderFlight> SharSaleOrders
        {
            get
            {
                return ServerDataSingleton.Instance.GetAllSaleSharOrders(FilterOrderStatusNonCanc, SelectedStartDate, SelectedEndDate);
            }
        }

        public List<OrderFlight> SharSpisOrders
        {
            get
            {
                return ServerDataSingleton.Instance.GetAllSpisSharOrders(FilterOrderStatusNonCanc, SelectedStartDate, SelectedEndDate);
            }
        }

        public string AvgSharSummStr
        {
            get
            {
                decimal avg = SharSaleOrders.Count == 0 ? 0 : SharSaleOrders.Sum(a => a.OrderTotalSumm) / SharSaleOrders.Count;
                return $"{(avg).ToString("C2")}";
            }
        }
        public string SaleSharSummStr
        {
            get
            {
                return $" {SharSaleOrders.Sum(a => a.OrderTotalSumm).ToString("C2")} ({SharSaleOrders.Count})";
            }
        }
        public string SpisSharSummStr
        {
            get
            {
                return $" {SharSpisOrders.Sum(a => a.OrderTotalSumm).ToString("C2")} ({SharSpisOrders.Count})";
            }
        }

        public List<AirCompanyRevenueInfo> SharRevenueInfos
        {
            get
            {
                return AllSharOrders.Where(a => a.AirCompany != null && a.AirCompany.PaymentType != null && a.AirCompany.PaymentType.PaymentGroup != null)
                    .GroupBy(a => a.AirCompany).OrderBy(a => !a.Key.PaymentType.PaymentGroup.Sale).ThenByDescending(a => a.Key.Name).Select(
                    a => new AirCompanyRevenueInfo()
                    {
                        Cname = a.Key.Name,
                        Revenue = a.Sum(b => b.OrderTotalSumm)

                    }
                    ).ToList();
            }
        }


        public List<OrderFlight> AllSharOrders
        {
            get
            {
                return ServerDataSingleton.Instance.GetAllAirOrdersWithShar(FilterOrderStatusNonCanc, SelectedStartDate, SelectedEndDate).Where(a => a.CreatedById == DBProvider.SharUserId).ToList();
            }
        }



        public List<OrderFlight> AllToFlyOrdersWithShar
        {
            get
            {
                return ServerDataSingleton.Instance.GetAllAirOrdersWithShar(FilterOrderStatusNonCanc, SelectedStartDate, SelectedEndDate).ToList();
            }
        }

        #endregion

        #region Spis
        public List<OrderToGo> ToGoSpisOrders
        {
            get
            {
                return ServerDataSingleton.Instance.GetAllToGoOrders(FilterOrderStatusNonCanc, SelectedStartDate, SelectedEndDate)
                    .Where(a => a.PaymentType != null && (a.PaymentType.PaymentGroup != null && !a.PaymentType.PaymentGroup.Sale)).ToList();
            }
        }
        public List<OrderFlight> SpisAirOrders
        {
            get
            {
                return AirOrders.Where(a =>
                     a.AirCompany != null &&
                    a.AirCompany.PaymentType != null &&
                    a.AirCompany.PaymentType.PaymentGroup != null &&
                    !a.AirCompany.PaymentType.PaymentGroup.Sale
                    ).ToList();
            }
        }


        public string SpisToGoSummStr
        {
            get
            {
                return $" {ToGoSpisOrders.Sum(a => a.OrderTotalSumm).ToString("C2")} ({ToGoSpisOrders.Count})";
            }
        }

        public string SpisToFlySummStr
        {
            get
            {
                return $" {SpisAirOrders.Sum(a => a.OrderTotalSumm).ToString("C2")} ({SpisAirOrders.Count})";
            }
        }



        public List<AirCompanyRevenueInfo> SpisRevenueInfos
        {
            get
            {
                return AllToFlyOrdersWithShar.Where(a => a.AirCompany != null && a.AirCompany.PaymentType != null && a.AirCompany.PaymentType.PaymentGroup != null &&
                !a.AirCompany.PaymentType.PaymentGroup.Sale)
                    .GroupBy(a => a.AirCompany).Select(
                    a => new AirCompanyRevenueInfo()
                    {
                        Cname = a.Key.Name,
                        Revenue = a.Sum(b => b.OrderTotalSumm)

                    }
                    ).Union
                     (ToGoOrders.Where(a => a.PaymentType != null && a.PaymentType.PaymentGroup != null && !a.PaymentType.PaymentGroup.Sale)
                    .GroupBy(a => a.PaymentType).Select(
                    a => new AirCompanyRevenueInfo()
                    {
                        Cname = a.Key.Name,
                        Revenue = a.Sum(b => b.OrderTotalSumm)

                    }
                    )

                    ).ToList();
            }
        }
        #endregion


        #region ToGo
        public List<OrderToGo> ToGoOrders
        {
            get
            {
                return ServerDataSingleton.Instance.GetAllToGoOrders(FilterOrderStatusNonCanc, SelectedStartDate, SelectedEndDate);
            }
        }


        public List<OrderToGo> ToGoSaleOrders
        {
            get
            {
                return ServerDataSingleton.Instance.GetAllToGoOrders(FilterOrderStatusNonCanc, SelectedStartDate, SelectedEndDate)
                    .Where(a => a.PaymentType == null || (a.PaymentType.PaymentGroup != null && a.PaymentType.PaymentGroup.Sale)).ToList();
            }
        }

        public List<OrderToGo> ToGoCancelledOrders
        {
            get
            {
                return ServerDataSingleton.Instance.GetAllToGoOrders(OrderStatus.Cancelled, SelectedStartDate, SelectedEndDate)
                    .Where(a => a.PaymentType == null || (a.PaymentType.PaymentGroup != null && a.PaymentType.PaymentGroup.Sale)).ToList();
            }
        }

        public string AvgToGoAirSummStr
        {
            get
            {
                decimal avg = ToGoSaleOrders.Count == 0 ? 0 : ToGoSaleOrders.Sum(a => a.OrderTotalSumm) / ToGoSaleOrders.Count;
                return $"{(avg).ToString("C2")}";
            }
        }
        public string SaleToGoSummStr
        {
            get
            {
                return $" {ToGoSaleOrders.Sum(a => a.OrderTotalSumm).ToString("C2")} ({ToGoSaleOrders.Count})";
            }
        }

        public string SaleToGoSummNonClosedStr
        {
            get
            {
                return $" {ToGoSaleOrders.Where(a => a.OrderStatus != OrderStatus.Closed && a.OrderStatus != OrderStatus.CancelledWithRemains).Sum(a => a.OrderTotalSumm).ToString("C2")}" +
                    $" ({ToGoSaleOrders.Where(a => a.OrderStatus != OrderStatus.Closed && a.OrderStatus != OrderStatus.CancelledWithRemains).Count()})";
            }
        }

        public string SaleToGoSummNonSendStr
        {
            get
            {
                return $" {ToGoSaleOrders.Where(a => a.OrderStatus == OrderStatus.InWork).Sum(a => a.OrderTotalSumm).ToString("C2")} " +
                    $"({ToGoSaleOrders.Where(a => a.OrderStatus == OrderStatus.InWork).Count()})";
            }
        }

        public string CancelledToGoOrdersStr
        {
            get
            {
                return $" {ToGoCancelledOrders.Sum(a => a.CancelledOrderDishesSumm).ToString("C2")} ({ToGoCancelledOrders.Count()})";
            }
        }


        public string DaysToGoOrdersStr //Дневные заказы без шереметьева и списаний
        {
            get
            {
                return $" {ToGoSaleOrders.Where(a => a.DeliveryDate.Hour >= 11).Sum(a => a.OrderTotalSumm).ToString("C2")} ({ToGoSaleOrders.Where(a => a.DeliveryDate.Hour >= 11).Count()})";
            }
        }

        public string NightToGoOrdersStr //Ночные заказы без шереметьева и списаний
        {
            get
            {
                return $" {ToGoSaleOrders.Where(a => a.DeliveryDate.Hour < 11).Sum(a => a.OrderTotalSumm).ToString("C2")} ({ToGoSaleOrders.Where(a => a.DeliveryDate.Hour < 11).Count()})";
            }
        }


        #endregion





        public OrderStatus FilterOrderStatusNonCanc { set; get; } = OrderStatus.InWork | OrderStatus.Sent | OrderStatus.Closed | OrderStatus.CancelledWithRemains;
        public OrderStatus FilterOrderStatus { set; get; } = OrderStatus.InWork | OrderStatus.Sent | OrderStatus.Closed | OrderStatus.CancelledWithRemains | OrderStatus.Cancelled;

        public List<RevenueInfo> GastroSaleAirInfos
        {
            get
            {
                return ServerDataSingleton.Instance.GetRevenueSaleGastroInfos(FilterOrderStatusNonCanc, SelectedStartDate, SelectedEndDate);
            }
        }

        public List<RevenueInfo> RevenueSaleAirInfos
        {
            get
            {
                return ServerDataSingleton.Instance.GetRevenueSaleAirInfos(FilterOrderStatusNonCanc, SelectedStartDate, SelectedEndDate);
            }
        }

        public List<RevenueInfo> CancelledSaleRevenueInfos
        {
            get
            {
                return ServerDataSingleton.Instance.GetRevenueSaleAirInfos(OrderStatus.Cancelled, SelectedStartDate, SelectedEndDate);
            }
        }



        public List<AirCompanyRevenueInfo> CompanyRevenueInfos
        {
            get
            {
                return ServerDataSingleton.Instance.GetCompanyRevenueInfos(FilterOrderStatusNonCanc, SelectedStartDate, SelectedEndDate);
            }
        }

        public List<AirCompanyRevenueInfo> ToGoChanelsRevenueInfos
        {
            get
            {
                return ToGoSaleOrders.Where(a => a.MarketingChannel != null).GroupBy(a => a.MarketingChannel).Select(
                    a => new AirCompanyRevenueInfo()
                    {
                        Cname = a.Key.Name,
                        Revenue = a.Sum(b => b.OrderTotalSumm)

                    }
                    ).ToList();
            }
        }


        public List<OrderFlight> AirOrders
        {
            get
            {
                return ServerDataSingleton.Instance.GetAllAirOrders(FilterOrderStatusNonCanc, SelectedStartDate, SelectedEndDate);
            }
        }

        /*
        public List<OrderFlight> SharOrders
        {
            get
            {
                return ServerDataSingleton.Instance.GetAllSharOrders(FilterOrderStatusNonCanc, SelectedStartDate, SelectedEndDate);
            }
        }
        */
        public List<OrderFlight> CancelledOrdersList
        {
            get
            {
                return ServerDataSingleton.Instance.GetAllAirOrders(OrderStatus.Cancelled, SelectedStartDate, SelectedEndDate).Where(a =>
                     a.AirCompany != null &&
                     a.AirCompanyId != MainClass.AirGastroFoodId &&
                    a.AirCompany.PaymentType != null &&
                    a.AirCompany.PaymentType.PaymentGroup != null &&
                    a.AirCompany.PaymentType.PaymentGroup.Sale
                    ).ToList();
            }
        }
        /*
        public List<DishAInfo> DishAInfosToGo
        {
            get
            {
                
                var tmp = new List<DishAInfo>();

                if (ToGoSaleOrders.Count() == 0) return tmp;
                foreach (var aor in DataExtension.DataCatalogsSingleton.Instance.Dishes)
                {
                    if (ToGoSaleOrders.Where(a => a.DishPackages.Any(b => b.Dish.Id == aor.Id)).Count() > 0)
                    {
                        var d = new DishAInfo()
                        {
                            Id = aor.Id,
                            Barcode = aor.Barcode,
                            Name = aor.Name,
                            Penetration = ToGoSaleOrders.Where(a => a.DishPackages.Any(b => b.Dish.Id == aor.Id)).Count() * 100 / ToGoSaleOrders.Count()

                        };
                        tmp.Add(d);
                    }
                }

                return tmp.OrderByDescending(a => a.Penetration).ToList();
            
                return null;
                }
        }
        */

        public List<DishAInfo> DishAInfosToFly
        {
            get
            {
                //var tmp = new List<DishAInfo>();
                var dTmp = new Dictionary<long, DishAInfo>();
                foreach (var ord in SaleAirOrders)
                {
                    foreach (var d in ord.DishPackages.Select(a => a.Dish).Distinct())
                    {
                        if (!dTmp.TryGetValue(d.Id, out DishAInfo di))
                        {
                            di = new DishAInfo()
                            {
                                Id = d.Id,
                                Barcode = d.Barcode,
                                Name = d.Name,
                                PenetrationIncludeCount = 1,
                                AllOrdersCount = SaleAirOrders.Count(),
                                Count = ord.DishPackages.Where(a => a.Dish == d).Sum(a => a.Amount)

                            };
                            dTmp.Add(di.Id, di);

                        }
                        else
                        {
                            di.PenetrationIncludeCount++;
                            di.Count += ord.DishPackages.Where(a => a.Dish == d).Sum(a => a.Amount);


                        }
                    }
                }
                return dTmp.Values.OrderByDescending(a => a.Penetration).ToList();
            }
        }


        public List<DishAInfo> DishAInfosToGo
        {
            get
            {
                //var tmp = new List<DishAInfo>();
                var dTmp = new Dictionary<long, DishAInfo>();
                foreach (var ord in ToGoSaleOrders)
                {
                    foreach (var d in ord.DishPackages.Select(a => a.Dish).Distinct())
                    {
                        if (!dTmp.TryGetValue(d.Id, out DishAInfo di))
                        {
                            di = new DishAInfo()
                            {
                                Id = d.Id,
                                Barcode = d.Barcode,
                                Name = d.Name,
                                PenetrationIncludeCount = 1,
                                AllOrdersCount = ToGoSaleOrders.Count(),
                                Count = ord.DishPackages.Where(a => a.Dish == d).Sum(a => a.Amount)
                            };
                            dTmp.Add(di.Id, di);
                        }
                        else
                        {
                            di.PenetrationIncludeCount++;
                            di.Count += ord.DishPackages.Where(a => a.Dish == d).Sum(a => a.Amount);
                        }
                    }
                }
                return dTmp.Values.OrderByDescending(a => a.Penetration).ToList();
            }
        }

        /*
        public List<DishAInfo> DishAInfosToFly
        {
            get
            {
                var tmp = new List<DishAInfo>();

                if (SaleAirOrders.Count() == 0) return tmp;
                foreach (var aor in DataExtension.DataCatalogsSingleton.Instance.Dishes)
                {
                    if (SaleAirOrders.Where(a => a.DishPackages.Any(b => b.Dish.Id == aor.Id)).Count() > 0)
                    {
                        var d = new DishAInfo()
                        {
                            Id = aor.Id,
                            Barcode = aor.Barcode,
                            Name = aor.Name,
                            Penetration = SaleAirOrders.Where(a => a.DishPackages.Any(b => b.Dish.Id == aor.Id)).Count() * 100 / SaleAirOrders.Count()

                        };
                        tmp.Add(d);
                    }
                }
                
                return tmp.OrderByDescending(a=>a.Penetration).ToList();
            }
        }



        */
        /*
        public IQueryable<DishAInfo> DishAInfosToFlyColl
        {
            get
            {
               return new QueryableCollectionView(DishAInfosToFly.OrderBy(a => a.Penetration));
                
            }
        }
        */
    }


    public class DishAInfo
    {
        public long Barcode { set; get; }
        public long Id { set; get; }

        public string Name { set; get; }
        public decimal Penetration
        {
            get
            {
                return AllOrdersCount == 0 ? 0 : PenetrationIncludeCount * 100 / AllOrdersCount;
            }
        }
        public string LogicCat { set; get; }
        public string KitchenCat { set; get; }
        public string LogikCat { set; get; }
        public int PenetrationIncludeCount { set; get; }
        public int AllOrdersCount { set; get; }
        public decimal Count { set; get; }
    }

}
