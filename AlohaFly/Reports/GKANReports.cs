using AlohaFly.Models;
using Microsoft.Office.Interop.Excel;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlohaFly.Reports
{
    public class GKANReports
    {

        private GKANReports()
        {
        }
        Logger logger = LogManager.GetCurrentClassLogger();

        //public static GKANReports instanse;
        public static GKANReports Instanse
        {
            get
            {
                return new GKANReports();
            }
        }


        Application app;
        Workbook Wb;
        Worksheet Ws;
        private void OpenXls()
        {
            app = new Microsoft.Office.Interop.Excel.Application();
            Wb = app.Workbooks.Add(true);
            Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            app.Visible = false;
        }



        public void ShowCommonReport(DateTime sDt, DateTime eDt)
        {
            try
            {
                eDt = eDt.AddDays(1);
                OpenXls();
                //ShowCatrepToFly(sDt, eDt);
                var dataCats = GetToFlyCats(sDt, eDt);
                ShowCatReport("To Fly отчет по категориям", dataCats);

                Ws = Wb.Worksheets.Add();
                ShowPaymentReport("To Fly отчет по валютам", dataCats);

                Ws = Wb.Worksheets.Add();
                var dataCatsToGo = GetToGoCats(sDt, eDt);
                ShowCatReport("To Go отчет по категориям", dataCatsToGo);

                Ws = Wb.Worksheets.Add();
                ShowPaymentReport("To Go отчет по валютам", dataCatsToGo);
                var dataCatsSVO = new GKANReportCats() { Dt1 = sDt, Dt2 = eDt }; ;
                try
                {
                    Ws = Wb.Worksheets.Add();
                    dataCatsSVO = GetSVOCats(sDt, eDt);
                    ShowCatReport("Шереметьево отчет по категориям", dataCatsSVO);

                    Ws = Wb.Worksheets.Add();
                    ShowPaymentReport("Шереметьево отчет по валютам", dataCatsSVO);
                }
                catch (Exception e)
                {

                }
                Ws = Wb.Worksheets.Add();
                var dataCatsR = GetRemovedCats(sDt, eDt);
                ShowRemovedReport("Отчет по отказам", dataCatsR, sDt, eDt);

                Ws = Wb.Worksheets.Add();
                var dataCatsD = GetToDiscountCats(sDt, eDt);
                ShowDiscountReport("Отчет по скидкам", dataCatsD, sDt, eDt);


                Ws = Wb.Worksheets.Add();
                ShowCommonReport(dataCatsToGo, dataCats, dataCatsSVO);


                app.Visible = true;
            }
            catch (Exception e)
            {
                logger.Error("Error ShowCommonReport " + e.Message);
            }
        }




        private void ShowCatrepToFly(DateTime sDt, DateTime eDt)
        {
            try
            {
                //OpenXls();
                var dataCats = GetToGoCats(sDt, eDt);
                ShowCatReport("To Fly отчет по категориям", dataCats);

            }
            catch (Exception e)
            {
                logger.Error("Error ShowCatrepToFly " + e.Message);
            }
        }


        private GKANReportCats GetSVOCats(DateTime sDt, DateTime eDt)
        {
            try
            {
                var data = AirOrdersModelSingleton.Instance.SVOorders.Where(a => a.OrderStatus != AlohaService.ServiceDataContracts.OrderStatus.Cancelled && a.DeliveryDate >= sDt && a.DeliveryDate < eDt);
                var dataCats = new GKANReportCats();
                foreach (var catP in DataExtension.DataCatalogsSingleton.Instance.AirCompanyData.Data.Where(x => x.IsActive && DBProvider.SharAirs.Contains(x.Id)))
                {
                    var dataCat = new GKANReportCat()
                    {
                        Admin = !catP.PaymentType.PaymentGroup.Sale,
                        PaymentCat = true,
                        CatName = catP.Name,

                        CatSumm = data.Where(x => x.AirCompany.PaymentType != null && x.AirCompany.Id == catP.Id).Sum(x => x.OrderTotalSumm),
                        //data.Sum(x => x.GetSpisDishesOfPaimentId(catP.Id).Sum(d => d.TotalSumm)),

                        Catcount = data
                        .Where(x => x.AirCompany.PaymentType != null && x.AirCompany.Id == catP.Id)
                        .Count()
                    };


                    dataCats.Cats.Add(dataCat);
                }
              
                foreach (var lCat in DataExtension.DataCatalogsSingleton.Instance.DishLogicGroupData.Data.Where(x => x.IsActive))
                {
                    var dataCat = new GKANReportCat()
                    {
                        Admin = false,
                        PaymentCat = false,
                        CatName = lCat.Name,
                        CatSumm = data.Where(x => x.AirCompany.PaymentType != null && x.AirCompany.PaymentType.PaymentGroup.Sale).Sum(x => x.GetNoSpisDishesOfCat(lCat.Id).Sum(d => d.TotalSumm)),
                        Catcount = data.Where(x => x.AirCompany.PaymentType != null && x.AirCompany.PaymentType.PaymentGroup.Sale).Sum(x => x.GetNoSpisDishesOfCat(lCat.Id).Count()),

                    };
                    if (lCat.Id == 7)//Доп услуги - наценка
                    {
                        dataCat.CatSumm += data.Sum(x => x.ExtraChargeSumm);
                        dataCat.Catcount += data.Where(x => x.ExtraChargeSumm > 0).Count();
                    }
                    dataCats.Cats.Add(dataCat);
                }
                var dataCatW = new GKANReportCat()
                {
                    Admin = false,
                    PaymentCat = false,
                    CatName = "Без категории",
                    CatSumm = data.Where(x => x.AirCompany.PaymentType != null && x.AirCompany.PaymentType.PaymentGroup.Sale).Sum(x => x.GetNoSpisDishesOfCat(null).Sum(d => d.TotalSumm)),
                    Catcount = data.Where(x => x.AirCompany.PaymentType != null && x.AirCompany.PaymentType.PaymentGroup.Sale).Sum(x => x.GetNoSpisDishesOfCat(null).Count()),

                };
                dataCats.Cats.Add(dataCatW);
                return dataCats;
            }
            catch (Exception e)
            {
                logger.Error($"Error GetSVOCats  err: {e.Message}");
                return new GKANReportCats();
            }


        }



        private GKANReportCats GetToDiscountCats(DateTime sDt, DateTime eDt)
        {
            var dataToGo = ToGoOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != AlohaService.ServiceDataContracts.OrderStatus.Cancelled && a.DeliveryDate >= sDt && a.DeliveryDate < eDt);

            var data = AirOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != AlohaService.ServiceDataContracts.OrderStatus.Cancelled && a.DeliveryDate >= sDt && a.DeliveryDate < eDt);
            var dataCats = new GKANReportCats() { Dt1 = sDt, Dt2 = eDt }; ;

            foreach (var d in dataToGo.Where(a => a.DiscountPercent > 0).Select(a => a.DiscountPercent).Distinct())
            {
                dataCats.Cats.Add(new GKANReportCat()
                {
                    Id = 0,
                    CatName = $"Скидка {d}%",
                    CatSumm = dataToGo.Where(a => a.DiscountPercent == d).Sum(a => a.DiscountSumm),
                    Catcount = dataToGo.Where(a => a.DiscountPercent == d).Count()
                });
            }
            dataCats.Cats.Add(new GKANReportCat()
            {
                Id = 1,
                CatName = $"Скидка на компанию",
                CatSumm = data.Sum(a => a.DiscountSumm),
                Catcount = data.Where(a => a.DiscountSumm > 0).Count()
            });

            dataCats.Cats.Add(new GKANReportCat()
            {
                Id = 2,
                CatName = $"Наценка на компанию",
                CatSumm = data.Sum(a => a.ExtraChargeSumm),
                Catcount = data.Where(a => a.ExtraChargeSumm > 0).Count()
            });

            return dataCats;
        }

        private GKANReportCats GetToFlyCats(DateTime sDt, DateTime eDt)
        {
            var data = AirOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != AlohaService.ServiceDataContracts.OrderStatus.Cancelled && a.DeliveryDate >= sDt && a.DeliveryDate < eDt);
            var dataCats = new GKANReportCats() { Dt1 = sDt,Dt2=eDt };
            foreach (var catP in DataExtension.DataCatalogsSingleton.Instance.PaymentData.Data.Where(x => !x.ToGo && x.IsActive && x.PaymentGroup != null))
            {
                var dataCat = new GKANReportCat()
                {
                    Admin = !catP.PaymentGroup.Sale,
                    PaymentCat = true,
                    CatName = catP.Name,

                    CatSumm = data.Where(x => x.AirCompany.PaymentType != null && x.AirCompany.PaymentId == catP.Id).Sum(x => x.OrderTotalSumm) +
                    data.Sum(x => x.GetSpisDishesOfPaimentId(catP.Id).Sum(d => d.TotalSumm) * (100 - x.DiscountPercent) / 100),

                    Catcount = data
                    .Where(x => x.AirCompany.PaymentType != null && x.AirCompany.PaymentId == catP.Id)
                    .Count()

                };


                dataCats.Cats.Add(dataCat);
            }

            var dataCatd = new GKANReportCat()
            {
                Admin = false,
                PaymentCat = false,
                CatName = "Удаленные блюда",
                CatSumm = data.Sum(x => x.GetSpisDishesOfPaimentId(0).Sum(d => d.TotalSumm)),
                Id = 1

            };
            dataCats.Cats.Add(dataCatd);

            foreach (var lCat in DataExtension.DataCatalogsSingleton.Instance.DishLogicGroupData.Data.Where(x => x.IsActive))
            {
                var dataCat = new GKANReportCat()
                {
                    Admin = false,
                    PaymentCat = false,
                    CatName = lCat.Name,
                    CatSumm = data.Where(x => x.AirCompany.PaymentType != null && x.AirCompany.PaymentType.PaymentGroup.Sale).Sum(x => x.GetNoSpisDishesOfCat(lCat.Id).Sum(d => d.TotalSumm) *
                    (x.OrderSumm == 0 ? 1 : (x.OrderTotalSumm / x.OrderSumm))
                    ),
                    Catcount = data.Where(x => x.AirCompany.PaymentType != null && x.AirCompany.PaymentType.PaymentGroup.Sale).Sum(x => x.GetNoSpisDishesOfCat(lCat.Id).Count()),

                };
                if (lCat.Id == 7)//Доп услуги - наценка
                {
                    dataCat.CatSumm += data.Where(x => (bool)x.AirCompany?.PaymentType?.PaymentGroup?.Sale).Sum(x => x.ExtraChargeSumm) - 0;
                    dataCat.Catcount += data.Where(x => x.ExtraChargeSumm > 0 && (bool)x.AirCompany?.PaymentType?.PaymentGroup?.Sale).Count();
                }
                dataCats.Cats.Add(dataCat);
            }

            var dataCatW = new GKANReportCat()
            {
                Admin = false,
                PaymentCat = false,
                CatName = "Без категории",
                CatSumm = data.Where(x => x.AirCompany.PaymentType != null && x.AirCompany.PaymentType.PaymentGroup.Sale).Sum(x => x.GetNoSpisDishesOfCat(null).Sum(d => d.TotalSumm) *
                (x.OrderSumm == 0 ? 1 : (x.OrderTotalSumm / x.OrderSumm))
                ),
                Catcount = data.Where(x => x.AirCompany.PaymentType != null && x.AirCompany.PaymentType.PaymentGroup.Sale).Sum(x => x.GetNoSpisDishesOfCat(null).Count()),

            };
            dataCats.Cats.Add(dataCatW);
            return dataCats;
        }
        private GKANReportCats GetRemovedCats(DateTime sDt, DateTime eDt)
        {
            try
            {
                var dataToGo = ToGoOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != AlohaService.ServiceDataContracts.OrderStatus.Cancelled && a.DeliveryDate >= sDt && a.DeliveryDate < eDt).ToList();
                var dataToFly = AirOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != AlohaService.ServiceDataContracts.OrderStatus.Cancelled && a.DeliveryDate >= sDt && a.DeliveryDate < eDt).ToList();
                dataToFly.AddRange(AirOrdersModelSingleton.Instance.SVOorders.Where(a => a.OrderStatus != AlohaService.ServiceDataContracts.OrderStatus.Cancelled && a.DeliveryDate >= sDt && a.DeliveryDate < eDt).ToList());
                var dataCats = new GKANReportCats() { Dt1 = sDt, Dt2 = eDt }; ;
                foreach (var catP in DataExtension.DataCatalogsSingleton.Instance.PaymentData.Data.Where(x => x.IsActive && x.PaymentGroup != null && !x.PaymentGroup.Sale))
                {
                    var cat = new GKANReportRemovedCat()
                    {
                        CatName = catP.Name,
                        IsOrder = true
                    };
                    dataCats.RemovedCats.Add(cat);

                    foreach (var ord in dataToGo.Where(a => a.PaymentId == catP.Id))
                    {
                        foreach (var d in ord.DishPackagesNoSpis)
                        {
                            if (!cat.Dishes.Any(a => a.BarCode == d.Dish.Barcode))
                            {
                                var dc = new GKANReportRemovedCatDish()
                                {
                                    BarCode = d.Dish.Barcode,
                                    Name = d.DishName,
                                    Count = d.Amount,
                                    Summ = d.TotalSumm * (ord.OrderDishesSumm == 0 ? 1 : ((ord.OrderDishesSumm - ord.DiscountSumm) / ord.OrderDishesSumm))
                                };
                                cat.Dishes.Add(dc);
                            }
                            else
                            {
                                var dc = cat.Dishes.SingleOrDefault(a => a.BarCode == d.Dish.Barcode);
                                dc.Count += d.Amount;
                                dc.Summ += d.TotalSumm * (ord.OrderDishesSumm == 0 ? 1 : ((ord.OrderDishesSumm - ord.DiscountSumm) / ord.OrderDishesSumm));

                            }
                        }

                    }

                    foreach (var ord in dataToFly.Where(a => a.AirCompany.PaymentId == catP.Id))
                    {
                        foreach (var d in ord.DishPackagesNoSpis)
                        {
                            if (!cat.Dishes.Any(a => a.BarCode == d.Dish.Barcode))
                            {
                                var dc = new GKANReportRemovedCatDish()
                                {
                                    Name = d.DishName,
                                    BarCode = d.Dish.Barcode,
                                    Count = d.Amount,
                                    Summ = d.TotalSumm * (ord.OrderDishesSumm == 0 ? 1 : (ord.OrderTotalSumm / ord.OrderDishesSumm))
                                };
                                cat.Dishes.Add(dc);
                            }
                            else
                            {
                                var dc = cat.Dishes.SingleOrDefault(a => a.BarCode == d.Dish.Barcode);
                                dc.Count += d.Amount;
                                dc.Summ += d.TotalSumm * (ord.OrderDishesSumm == 0 ? 1 : (ord.OrderTotalSumm / ord.OrderDishesSumm));

                            }
                        }
                    }

                }


                var catSpis = new GKANReportRemovedCat()
                {
                    CatName = "Отказ со списанием самолеты",
                    IsOrder = false
                };
                var catNoSpis = new GKANReportRemovedCat()
                {
                    CatName = "Отказ без списания самолеты",
                    IsOrder = false
                };
                var toGocatSpis = new GKANReportRemovedCat()
                {
                    CatName = "Отказ со списанием to go",
                    IsOrder = false
                };
                var toGocatNoSpis = new GKANReportRemovedCat()
                {
                    CatName = "Отказ без списаниея to go",
                    IsOrder = false
                };

                dataCats.RemovedCats.Add(catSpis);
                dataCats.RemovedCats.Add(catNoSpis);
                dataCats.RemovedCats.Add(toGocatSpis);
                dataCats.RemovedCats.Add(toGocatNoSpis);

                foreach (var ord in dataToFly)
                {
                    foreach (var d in ord.DishPackagesSpis)
                    {
                        var dc = new GKANReportRemovedCatDish()
                        {
                            Name = d.DishName,
                            BarCode = d.Dish.Barcode,
                            Count = d.Amount,
                            Summ = d.TotalSumm * (ord.OrderDishesSumm == 0 ? 1 : ((ord.OrderDishesSumm - ord.DiscountSumm) / ord.OrderDishesSumm)),
                            Comment = d.SpisPayment?.Name,
                            OrderId = ord.Id
                        };

                        if (d.DeletedStatus == 1)
                        {
                            catSpis.Dishes.Add(dc);
                        }
                        else if (d.DeletedStatus == 2)
                        {
                            catNoSpis.Dishes.Add(dc);
                        }
                    }

                }

                foreach (var ord in dataToGo)
                {
                    foreach (var d in ord.DishPackagesSpis)
                    {
                        var dc = new GKANReportRemovedCatDish()
                        {
                            Name = d.DishName,
                            BarCode = d.Dish.Barcode,
                            Count = d.Amount,
                            Summ = d.TotalSumm * (ord.OrderDishesSumm == 0 ? 1 : ((ord.OrderDishesSumm - ord.DiscountSumm) / ord.OrderDishesSumm)),
                            Comment = d.SpisPayment?.Name,
                            OrderId = ord.Id
                        };

                        if (d.DeletedStatus == 1)
                        {
                            toGocatSpis.Dishes.Add(dc);
                        }
                        else if (d.DeletedStatus == 2)
                        {
                            toGocatNoSpis.Dishes.Add(dc);
                        }
                    }

                }

                return dataCats;
            }

            catch (Exception e)
            {
                logger.Error($"Error GetRemovedCats  err: {e.Message}");
                return new GKANReportCats();
            }
        }

        //private void AddDish

        private GKANReportCats GetToGoCats(DateTime sDt, DateTime eDt)
        {
            var data = ToGoOrdersModelSingleton.Instance.Orders.Where(a => a.OrderStatus != AlohaService.ServiceDataContracts.OrderStatus.Cancelled && a.DeliveryDate >= sDt && a.DeliveryDate < eDt);
            var dataCats = new GKANReportCats() { Dt1 = sDt, Dt2 = eDt }; ;
            foreach (var catP in DataExtension.DataCatalogsSingleton.Instance.PaymentData.Data.Where(x => x.ToGo && x.IsActive && x.PaymentGroup != null))
            {
                var dataCat = new GKANReportCat()
                {
                    Admin = !catP.PaymentGroup.Sale,
                    PaymentCat = true,
                    CatName = catP.Name,

                    CatSumm = data.Where(x => x.PaymentType != null && x.PaymentId == catP.Id).Sum(x => x.OrderTotalSumm) +
                    data.Sum(x => x.GetSpisDishesOfPaimentId(catP.Id).Sum(d => d.TotalSumm)),

                    Catcount = data
                    .Where(x => x.PaymentType != null && x.PaymentId == catP.Id)
                    .Count()

                };


                dataCats.Cats.Add(dataCat);
            }

            var dataCatd = new GKANReportCat()
            {
                Admin = false,
                PaymentCat = false,
                CatName = "Удаленные блюда",
                CatSumm = data.Sum(x => x.GetSpisDishesOfPaimentId(0).Sum(d => d.TotalSumm)),
                Id = 1

            };
            dataCats.Cats.Add(dataCatd);

            foreach (var lCat in DataExtension.DataCatalogsSingleton.Instance.DishLogicGroupData.Data.Where(x => x.IsActive))
            {
                var dataCat = new GKANReportCat()
                {
                    Admin = false,
                    PaymentCat = false,
                    CatName = lCat.Name,
                    CatSumm = data.Where(x => x.PaymentType != null && x.PaymentType.PaymentGroup.Sale).Sum(x => (x.GetNoSpisDishesOfCat(lCat.Id).Sum(d => d.TotalSumm) *
                    (x.OrderDishesSumm == 0 ? 1 : ((x.OrderDishesSumm - x.DiscountSumm) / x.OrderDishesSumm))
                    )),
                    Catcount = data.Where(x => x.PaymentType != null && x.PaymentType.PaymentGroup.Sale).Sum(x => x.GetNoSpisDishesOfCat(lCat.Id).Count()),

                };
                if (lCat.Id == 7)//Доп услуги - доставка
                {
                    dataCat.CatSumm += data.Where(x => x.PaymentType != null && x.PaymentType.PaymentGroup.Sale).Sum(x => x.DeliveryPrice);
                    dataCat.Catcount += data.Where(x => x.PaymentType != null && x.PaymentType.PaymentGroup.Sale).Where(x => x.DeliveryPrice > 0).Count();
                }




                dataCats.Cats.Add(dataCat);
            }

            var dataCatW = new GKANReportCat()
            {
                Admin = false,
                PaymentCat = false,
                CatName = "Без категории",
                CatSumm = data.Where(x => x.PaymentType != null && x.PaymentType.PaymentGroup.Sale).Sum(x => x.GetNoSpisDishesOfCat(null).Sum(d => d.TotalSumm) *
                (x.OrderDishesSumm == 0 ? 1 : ((x.OrderDishesSumm - x.DiscountSumm) / x.OrderDishesSumm))
                   ),
                Catcount = data.Where(x => x.PaymentType != null && x.PaymentType.PaymentGroup.Sale).Sum(x => x.GetNoSpisDishesOfCat(null).Count()),

            };
            dataCats.Cats.Add(dataCatW);

            return dataCats;
        }

        private void ShowCommonReport(GKANReportCats dataCatsToGo, GKANReportCats dataCatsToFly, GKANReportCats dataCatsSVO)
        {

            try
            {
                Ws.Name = "Общий отчет";
                Ws.get_Range("A1:M1000").Font.Name = "Antica";
                Ws.get_Range("A1:M7").Font.Name = "Helica";
                Ws.get_Range("A1:M1000").Font.Size = 10;

                Ws.Cells[2, 2] = "Общий отчет".ToUpper();
                Ws.get_Range("B2:B2").Font.Size = 14;
                Ws.get_Range("B2:B2").Font.Bold = true;

                Ws.Cells[2, 3] = $"{dataCatsToFly.Dt1.ToString("dd/MM")} - {dataCatsToFly.Dt2.ToString("dd/MM/yyyy")}";
                Ws.get_Range("C2:C2").Font.Size = 12;
                Ws.get_Range("C2:C2").Font.Bold = false;
                Ws.get_Range("C2:C2").HorizontalAlignment = XlHAlign.xlHAlignCenter; 

                int row = 4;
                ((Range)Ws.Rows[row]).Font.Bold = true;
                Ws.Cells[row, 2] = "Категория";
                Ws.Cells[row, 3] = "Валюта";
                Ws.Cells[row, 4] = "Кол-во";
                Ws.Cells[row++, 5] = "Сумма";

                Ws.Cells[row, 2] = "Администрация To Fly";
                Ws.Cells[row, 2].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                Ws.Cells[row, 2].VerticalAlignment = XlHAlign.xlHAlignCenter;
                Ws.Range[Ws.Cells[row, 2], Ws.Cells[row + dataCatsToFly.Cats.Where(x => x.Admin && x.PaymentCat).Count(), 2]].Cells.Merge(Type.Missing);

                foreach (var cat in dataCatsToFly.Cats.Where(x => x.Admin && x.PaymentCat))
                {
                    cat.Write(row++, 3, Ws);
                }

                Ws.Cells[row, 3] = "Всего администрация To Fly";
                Ws.Cells[row, 4] = dataCatsToFly.AllAdminCount;
                Ws.Cells[row, 5] = dataCatsToFly.AllSummAdmin;
                ((Range)Ws.Rows[row]).Font.Bold = true;
                row++;

                Ws.Cells[row, 2] = "Администрация To GO";
                Ws.Cells[row, 2].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                Ws.Cells[row, 2].VerticalAlignment = XlHAlign.xlHAlignCenter;
                Ws.Range[Ws.Cells[row, 2], Ws.Cells[row + dataCatsToGo.Cats.Where(x => x.Admin && x.PaymentCat).Count(), 2]].Cells.Merge(Type.Missing);
                foreach (var cat in dataCatsToGo.Cats.Where(x => x.Admin && x.PaymentCat))
                {
                    cat.Write(row++, 3, Ws);
                }
                Ws.Cells[row, 3] = "Всего администрация To GO";
                Ws.Cells[row, 4] = dataCatsToGo.AllAdminCount;
                Ws.Cells[row, 5] = dataCatsToGo.AllSummAdmin;
                ((Range)Ws.Rows[row]).Font.Bold = true;
                row++;


                Ws.Cells[row, 2] = "Администрация SVO";
                Ws.Cells[row, 2].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                Ws.Cells[row, 2].VerticalAlignment = XlHAlign.xlHAlignCenter;
                Ws.Range[Ws.Cells[row, 2], Ws.Cells[row + dataCatsSVO.Cats.Where(x => x.Admin && x.PaymentCat).Count(), 2]].Cells.Merge(Type.Missing);
                foreach (var cat in dataCatsSVO.Cats.Where(x => x.Admin && x.PaymentCat))
                {
                    cat.Write(row++, 3, Ws);
                }
                Ws.Cells[row, 3] = "Всего администрация SVO";
                Ws.Cells[row, 4] = dataCatsSVO.AllAdminCount;
                Ws.Cells[row, 5] = dataCatsSVO.AllSummAdmin;
                ((Range)Ws.Rows[row]).Font.Bold = true;
                row++;
                Ws.Cells[row, 3] = "ИТОГО администрация";
                Ws.Cells[row, 4] = dataCatsSVO.AllAdminCount + dataCatsToGo.AllAdminCount + dataCatsToFly.AllAdminCount;
                Ws.Cells[row, 5] = dataCatsSVO.AllSummAdmin + dataCatsToGo.AllSummAdmin + dataCatsToFly.AllSummAdmin;
                ((Range)Ws.Rows[row]).Font.Bold = true;
                ((Range)Ws.Rows[row]).Font.Size = 12;

                row++;

                Ws.Cells[row, 2] = "To Fly";
                Ws.Cells[row, 2].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                Ws.Cells[row, 2].VerticalAlignment = XlHAlign.xlHAlignCenter;
                Ws.Range[Ws.Cells[row, 2], Ws.Cells[row + dataCatsToFly.Cats.Where(x => !x.Admin && x.PaymentCat).Count(), 2]].Cells.Merge(Type.Missing);

                foreach (var cat in dataCatsToFly.Cats.Where(x => !x.Admin && x.PaymentCat))
                {
                    cat.Write(row++, 3, Ws);
                }

                Ws.Cells[row, 3] = "Выручка To Fly";
                Ws.Cells[row, 4] = dataCatsToFly.AllCountPaymentsNonAdmin;
                Ws.Cells[row, 5] = dataCatsToFly.AllSummPaymentsNonAdmin;
                ((Range)Ws.Rows[row]).Font.Bold = true;
                row++;

                Ws.Cells[row, 2] = "To GO";
                Ws.Cells[row, 2].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                Ws.Cells[row, 2].VerticalAlignment = XlHAlign.xlHAlignCenter;
                Ws.Range[Ws.Cells[row, 2], Ws.Cells[row + dataCatsToGo.Cats.Where(x => !x.Admin && x.PaymentCat).Count(), 2]].Cells.Merge(Type.Missing);
                foreach (var cat in dataCatsToGo.Cats.Where(x => !x.Admin && x.PaymentCat))
                {
                    cat.Write(row++, 3, Ws);
                }
                Ws.Cells[row, 3] = "Выручка To GO";
                Ws.Cells[row, 4] = dataCatsToGo.AllCountPaymentsNonAdmin;
                Ws.Cells[row, 5] = dataCatsToGo.AllSummPaymentsNonAdmin;
                ((Range)Ws.Rows[row]).Font.Bold = true;
                row++;


                Ws.Cells[row, 2] = "SVO";
                Ws.Cells[row, 2].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                Ws.Cells[row, 2].VerticalAlignment = XlHAlign.xlHAlignCenter;
                Ws.Range[Ws.Cells[row, 2], Ws.Cells[row + dataCatsSVO.Cats.Where(x => !x.Admin && x.PaymentCat).Count(), 2]].Cells.Merge(Type.Missing);
                foreach (var cat in dataCatsSVO.Cats.Where(x => !x.Admin && x.PaymentCat))
                {
                    cat.Write(row++, 3, Ws);
                }
                Ws.Cells[row, 3] = "Выручка SVO";
                Ws.Cells[row, 4] = dataCatsSVO.AllCountPaymentsNonAdmin;
                Ws.Cells[row, 5] = dataCatsSVO.AllSummPaymentsNonAdmin;
                ((Range)Ws.Rows[row]).Font.Bold = true;
                row++;
                Ws.Cells[row, 3] = "ИТОГО Выручка";
                Ws.Cells[row, 5] = dataCatsSVO.AllSummPaymentsNonAdmin + dataCatsToGo.AllSummPaymentsNonAdmin + dataCatsToFly.AllSummPaymentsNonAdmin;
                ((Range)Ws.Rows[row]).Font.Bold = true;
                ((Range)Ws.Rows[row]).Font.Size = 14;
                row++;
                ((Range)Ws.Rows[row]).Font.Bold = true;
                Ws.Cells[row++, 2] = "Категории";
                ((Range)Ws.Rows[row]).Font.Bold = true;
                Ws.Cells[row, 2] = "Наименование";
                Ws.Cells[row, 3] = "To Fly";
                Ws.Cells[row, 4] = "To Go";
                Ws.Cells[row, 5] = "SVO";
                Ws.Cells[row++, 6] = "Итого";


                var Cats = dataCatsSVO.Cats.Where(a => !a.PaymentCat).Select(b => b.CatName)
                    .Union(dataCatsToFly.Cats.Where(a => !a.PaymentCat).Select(b => b.CatName))
                    .Union(dataCatsToGo.Cats.Where(a => !a.PaymentCat).Select(b => b.CatName))
                    .Distinct();

                foreach (var cname in Cats)
                {
                    Ws.Cells[row, 2] = cname;
                    Ws.Cells[row, 3] = dataCatsToFly.Cats.Any(a => a.CatName == cname) ? dataCatsToFly.Cats.FirstOrDefault(a => a.CatName == cname).CatSumm : 0;
                    Ws.Cells[row, 4] = dataCatsToGo.Cats.Any(a => a.CatName == cname) ? dataCatsToGo.Cats.FirstOrDefault(a => a.CatName == cname).CatSumm : 0;
                    Ws.Cells[row, 5] = dataCatsSVO.Cats.Any(a => a.CatName == cname) ? dataCatsSVO.Cats.FirstOrDefault(a => a.CatName == cname).CatSumm : 0;
                    Ws.Cells[row, 6] = (dataCatsSVO.Cats.Any(a => a.CatName == cname) ? dataCatsSVO.Cats.FirstOrDefault(a => a.CatName == cname).CatSumm : 0) +
                        (dataCatsToFly.Cats.Any(a => a.CatName == cname) ? dataCatsToFly.Cats.FirstOrDefault(a => a.CatName == cname).CatSumm : 0) +
                        (dataCatsToGo.Cats.Any(a => a.CatName == cname) ? dataCatsToGo.Cats.FirstOrDefault(a => a.CatName == cname).CatSumm : 0);

                    row++;
                }

               
                Ws.Columns[2].AutoFit();
                Ws.Columns[3].AutoFit();
                Ws.Columns[4].ColumnWidth = 10;

                Ws.Columns[5].ColumnWidth = 20;
                Ws.Columns[6].ColumnWidth = 20;


                ((Range)Ws.Range[Ws.Cells[4, 2], Ws.Cells[row, 5]]).Cells.Borders.LineStyle = XlLineStyle.xlContinuous;

            }
            catch (Exception e)
            {
                logger.Error($"Error ShowCatReport  err: {e.Message}");
            }
        }

        private void ShowCatReport(string name, GKANReportCats dataCats)
        {

            try
            {
                Ws.Name = name;
                Ws.get_Range("A1:M1000").Font.Name = "Antica";
                Ws.get_Range("A1:M7").Font.Name = "Helica";
                Ws.get_Range("A1:M1000").Font.Size = 10;

                Ws.Cells[2, 2] = name;
                Ws.get_Range("B2:B2").Font.Size = 14;
                Ws.get_Range("B2:B2").Font.Bold = true;

                Ws.Cells[2, 3] = $"{dataCats.Dt1.ToString("dd/MM")} - {dataCats.Dt2.ToString("dd/MM/yyyy")}";
                Ws.get_Range("C2:C2").Font.Size = 12;
                Ws.get_Range("C2:C2").Font.Bold = false;
                Ws.get_Range("C2:C2").HorizontalAlignment = XlHAlign.xlHAlignCenter; ;
                

                int row = 4;
                Ws.Cells[row, 3] = "Чеков";
                Ws.Cells[row++, 4] = "Сумма";

                Ws.Cells[row, 2] = "Администрация";
                Ws.Cells[row, 3] = dataCats.AllAdminCount;
                Ws.Cells[row++, 4] = dataCats.AllSummAdmin;
                foreach (var cat in dataCats.Cats.Where(x => !x.Admin && x.PaymentCat))
                {
                    cat.Write(row++, 2, Ws);
                }

            ((Range)Ws.Rows[row]).Font.Bold = true;
                Ws.Cells[row, 2] = "Итого";
                Ws.Cells[row, 3] = dataCats.Cats.Where(x => x.PaymentCat).Sum(a => a.Catcount);
                Ws.Cells[row++, 4] = dataCats.Cats.Where(x => x.PaymentCat).Sum(a => a.CatSumm);
                /*
                Ws.Cells[row, 2] = "Удаленные блюда";
                Ws.Cells[row, 3] = dataCats.Cats.SingleOrDefault(x => x.Id==1).Catcount;
                Ws.Cells[row++, 4] = dataCats.Cats.SingleOrDefault(x => x.Id == 1).CatSumm;
                */
                foreach (var cat in dataCats.Cats.Where(x => !x.PaymentCat))
                {
                    cat.Write(row++, 2, Ws);
                }

                Ws.Columns[2].AutoFit();
                Ws.Columns[3].ColumnWidth = 20;

                Ws.Columns[4].ColumnWidth = 20;


                ((Range)Ws.Range[Ws.Cells[4, 2], Ws.Cells[row, 4]]).Cells.Borders.LineStyle = XlLineStyle.xlContinuous;

            }
            catch (Exception e)
            {
                logger.Error($"Error ShowCatReport Name: {name}; err: {e.Message}");
            }
        }


        private void ShowPaymentReport(string name, GKANReportCats dataCats)
        {
            try
            {
                Ws.Name = name;
                Ws.get_Range("A1:M1000").Font.Name = "Antica";
                Ws.get_Range("A1:M7").Font.Name = "Helica";
                Ws.get_Range("A1:M1000").Font.Size = 10;

                Ws.Cells[2, 2] = name;
                Ws.get_Range("B2:B2").Font.Size = 14;
                Ws.get_Range("B2:B2").Font.Bold = true;

                int row = 4;
                Ws.Cells[row, 2] = "Тип валюты";
                Ws.Cells[row++, 3] = "Валюта";
                Ws.Range[Ws.Cells[row, 2], Ws.Cells[row + dataCats.Cats.Where(x => x.Admin && x.PaymentCat).Count(), 2]].Cells.Merge(Type.Missing);

                Ws.Cells[row, 2] = "Администрация";
                Ws.Cells[row, 2].HorizontalAlignment = XlHAlign.xlHAlignCenter;
                Ws.Cells[row, 2].VerticalAlignment = XlHAlign.xlHAlignCenter;

                foreach (var cat in dataCats.Cats.Where(x => x.Admin && x.PaymentCat))
                {
                    Ws.Cells[row, 3] = cat.CatName;
                    Ws.Cells[row++, 4] = cat.CatSumm;
                }
            ((Range)Ws.Rows[row]).Font.Bold = true;
                Ws.Cells[row, 3] = "Итого";
                Ws.Cells[row++, 4] = dataCats.Cats.Where(x => x.Admin && x.PaymentCat).Sum(a => a.CatSumm);

                foreach (var cat in dataCats.Cats.Where(x => !x.Admin && x.PaymentCat))
                {
                    Ws.Cells[row, 2] = cat.CatName;
                    Ws.Cells[row, 3] = cat.CatName;
                    Ws.Cells[row++, 4] = cat.CatSumm;
                }
            ((Range)Ws.Rows[row]).Font.Bold = true;
                Ws.Cells[row, 2] = "Итого";
                Ws.Cells[row, 4] = dataCats.Cats.Where(x => x.PaymentCat).Sum(a => a.CatSumm);

                Ws.Columns[2].AutoFit();
                Ws.Columns[3].AutoFit();

                Ws.Columns[4].ColumnWidth = 20;

                ((Range)Ws.Range[Ws.Cells[4, 2], Ws.Cells[row, 4]]).Cells.Borders.LineStyle = XlLineStyle.xlContinuous;

            }
            catch (Exception e)
            {
                logger.Error($"Error ShowPaymentReport Name: {name}; err: {e.Message}");
            }

        }

        private void WriteStandartHeader(string name, DateTime sDt, DateTime eDt)
        {
            Ws.Name = name;
            Ws.get_Range("A1:M1000").Font.Name = "Antica";
            Ws.get_Range("A1:M7").Font.Name = "Helica";
            Ws.get_Range("A1:M1000").Font.Size = 10;

            Ws.Cells[1, 5] = "UCS R-Keeper Reports";
            Ws.Cells[1, 6] = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            (Ws.Cells[1, 7] as Range).Font.Italic = true;
            (Ws.Cells[1, 7] as Range).Font.Size = 10;
            (Ws.Cells[1, 7] as Range).Font.Underline = true;
            Ws.Cells[1, 7] = "Галерея";



            (Ws.Cells[3, 6] as Range).Font.Bold = false;
            (Ws.Cells[3, 6] as Range).Font.Italic = true;
            (Ws.Cells[3, 6] as Range).Font.Size = 16;
            (Ws.Cells[3, 6] as Range).Font.Name = "Helica";

            Ws.Cells[3, 6] = name;
            Ws.Cells[5, 4] = "период:";
            (Ws.Cells[5, 6] as Range).Font.Bold = true;
            (Ws.Cells[5, 6] as Range).Font.Underline = true;
            (Ws.Cells[5, 6] as Range).Font.Size = 10;
            (Ws.Cells[5, 6] as Range).Font.Name = "Helica";
            Ws.Cells[5, 6] = sDt.ToString("dd.MM.yyyy") + " - " + eDt.ToString("dd.MM.yyyy");
            Ws.Cells[5, 7] = "вкл.";


            Ws.get_Range("A7:P8").Font.Bold = true;
            Ws.get_Range("A7:P8").HorizontalAlignment = XlHAlign.xlHAlignRight;
            (Ws.Cells[7, 1] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;
            (Ws.Cells[7, 2] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;
        }


        private void ShowDiscountReport(string name, GKANReportCats dataCats, DateTime sDt, DateTime eDt)
        {
            try
            {
                WriteStandartHeader(name, sDt, eDt);
                int row = 8;
                Ws.Cells[row, 3] = "название";
                Ws.Cells[row, 5] = "количество";
                Ws.Cells[row, 7] = "сумма";
                row += 2;
                foreach (var d in dataCats.Cats.Where(a => a.Id == 0))
                {
                    Ws.Cells[row, 2] = "toGo";
                    Ws.Cells[row, 3] = d.CatName;
                    Ws.Cells[row, 5] = d.Catcount;
                    Ws.Cells[row, 7] = d.CatSumm;
                    row += 1;

                }
                var d1 = dataCats.Cats.SingleOrDefault(a => a.Id == 1);
                Ws.Cells[row, 2] = "toFly";
                Ws.Cells[row, 3] = d1.CatName;
                Ws.Cells[row, 5] = d1.Catcount;
                Ws.Cells[row, 7] = d1.CatSumm;
                row += 1;
                (Ws.Rows[row] as Range).Font.Bold = true;
                Ws.Cells[row, 3] = "Всего скидки";
                Ws.Cells[row, 5] = dataCats.Cats.Where(a => a.Id < 2).Sum(a => a.Catcount);
                Ws.Cells[row, 7] = dataCats.Cats.Where(a => a.Id < 2).Sum(a => a.CatSumm); ;
                row += 1;
                d1 = dataCats.Cats.SingleOrDefault(a => a.Id == 2);
                Ws.Cells[row, 3] = d1.CatName;
                Ws.Cells[row, 5] = d1.Catcount;
                Ws.Cells[row, 7] = d1.CatSumm;
                row += 1;
                (Ws.Rows[row] as Range).Font.Bold = true;
                Ws.Cells[row, 3] = "Всего наценки";
                Ws.Cells[row, 5] = d1.Catcount;
                Ws.Cells[row, 7] = d1.CatSumm;
                row += 1;
                Ws.Columns[3].AutoFit();

            }
            catch
            {

            }
        }

        private void ShowRemovedReport(string name, GKANReportCats dataCats, DateTime sDt, DateTime eDt)
        {
            try
            {
                WriteStandartHeader(name, sDt, eDt);
                int row = 8;
                Ws.Cells[row, 4] = "код";
                Ws.Cells[row, 5] = "название";
                Ws.Cells[row, 7] = "количество";
                Ws.Cells[row, 9] = "сумма";
                Ws.Cells[row, 11] = "ср. цена";
                Ws.Cells[row, 13] = "номер заказа";
                Ws.Cells[row, 15] = "причина удаления ";
                row += 2;

                foreach (var cat in dataCats.RemovedCats.Where(x => x.IsOrder))
                {
                    Ws.Cells[row, 4] = cat.CatName;
                    (Ws.Cells[row, 4] as Range).Font.Bold = true;
                    (Ws.Cells[row, 4] as Range).Font.Underline = true;
                    row += 2;
                    foreach (var d in cat.Dishes)
                    {
                        Ws.Cells[row, 4] = d.BarCode;
                        Ws.Cells[row, 5] = d.Name;
                        Ws.Cells[row, 7] = d.Count;
                        Ws.Cells[row, 9] = d.Summ;
                        Ws.Cells[row++, 11] = d.AvgPrice;
                    }
                    row += 1;
                    (Ws.Cells[row, 4] as Range).Font.Bold = true;
                    Ws.Cells[row, 4] = "Всего";

                    (Ws.Rows[row] as Range).Font.Bold = true;
                    Ws.Cells[row, 5] = $"({cat.CatName})";
                    Ws.Cells[row, 7] = cat.Dishes.Sum(x => x.Count);
                    Ws.Cells[row++, 9] = cat.Dishes.Sum(x => x.Summ);
                    row += 2;
                }
                row += 2;
                foreach (var cat in dataCats.RemovedCats.Where(x => !x.IsOrder))
                {
                    Ws.Cells[row, 4] = cat.CatName;
                    (Ws.Cells[row, 4] as Range).Font.Bold = true;
                    (Ws.Cells[row, 4] as Range).Font.Underline = true;
                    row += 2;
                    foreach (var d in cat.Dishes)
                    {
                        Ws.Cells[row, 4] = d.BarCode;
                        Ws.Cells[row, 5] = d.Name;
                        Ws.Cells[row, 7] = d.Count;
                        Ws.Cells[row, 9] = d.Summ;
                        Ws.Cells[row, 11] = d.AvgPrice;
                        Ws.Cells[row, 13] = d.OrderId;
                        Ws.Cells[row, 15] = d.Comment;
                        row++;
                    }


                    row += 1;
                    (Ws.Cells[row, 4] as Range).Font.Bold = true;
                    Ws.Cells[row, 4] = "Всего";

                    (Ws.Rows[row] as Range).Font.Bold = true;
                    Ws.Cells[row, 5] = $"({cat.CatName})";
                    Ws.Cells[row, 7] = cat.Dishes.Sum(x => x.Count);
                    Ws.Cells[row++, 9] = cat.Dishes.Sum(x => x.Summ);
                    row += 2;
                }

                Ws.Columns[5].AutoFit();
            }

            catch (Exception e)
            {
                logger.Error($"Error ShowRemovedReport  err: {e.Message}");
            }
        }

        private void RunRep()
        {

        }

    }


    public class GKANReportCats
    {
        public GKANReportCats() { }

        public DateTime Dt1;
        public DateTime Dt2;

        public List<GKANReportCat> Cats = new List<GKANReportCat>();
        public List<GKANReportRemovedCat> RemovedCats = new List<GKANReportRemovedCat>();


        public decimal AllSumm
        {
            get
            {
                return Cats.Sum(x => x.CatSumm);
            }
        }

        public decimal AllSummAdmin
        {
            get
            {
                return Cats.Where(x => x.Admin).Sum(x => x.CatSumm);
            }
        }
        public decimal AllAdminCount
        {
            get
            {
                return Cats.Where(x => x.Admin).Sum(x => x.Catcount);
            }
        }


        public decimal AllSummPaymentsNonAdmin
        {
            get
            {
                return Cats.Where(x => !x.Admin && x.PaymentCat).Sum(x => x.CatSumm);
            }
        }
        public decimal AllCountPaymentsNonAdmin
        {
            get
            {
                return Cats.Where(x => !x.Admin && x.PaymentCat).Sum(x => x.Catcount); ;
            }
        }


        public decimal AllSummPayment
        {
            get
            {
                return Cats.Where(x => x.PaymentCat).Sum(x => x.CatSumm);
            }
        }
    }

    public class GKANReportCat
    {
        public GKANReportCat()
        { }
        public string CatName { set; get; }
        public decimal CatSumm { set; get; }
        public decimal Catcount { set; get; }
        public bool Admin { set; get; }
        public bool PaymentCat { set; get; }
        public int Id { set; get; }

        public void Write(int row, int startColumn, Worksheet ws)
        {
            ws.Cells[row, startColumn++] = CatName;
            ws.Cells[row, startColumn++] = Catcount;
            ws.Cells[row, startColumn] = CatSumm;

        }
    }
    public class GKANReportRemovedCat
    {
        public GKANReportRemovedCat()
        { }
        public string CatName { set; get; }
        public decimal CatSumm { set; get; }
        public decimal Catcount { set; get; }
        public bool IsOrder { set; get; }
        /*
        public bool PaymentCat { set; get; }
        public int Id { set; get; }
        */

        public List<GKANReportRemovedCatDish> Dishes = new List<GKANReportRemovedCatDish>();


        public void Write(int row, int startColumn, Worksheet ws)
        {
            ws.Cells[row, startColumn++] = CatName;
            ws.Cells[row, startColumn++] = Catcount;
            ws.Cells[row, startColumn] = CatSumm;

        }
    }

    public class GKANReportRemovedCatDish
    {
        public GKANReportRemovedCatDish()
        { }
        public string Name { set; get; }
        public decimal Summ { set; get; }
        public decimal Count { set; get; }
        public decimal AvgPrice
        {
            get
            {
                return Summ / Count;
            }
        }
        public long OrderId { set; get; }
        public long BarCode { set; get; }
        public string Comment { set; get; }



        //public bool IsOrder { set; get; }
        /*
        public bool PaymentCat { set; get; }
        public int Id { set; get; }
        */



        public void Write(int row, int startColumn, Worksheet ws)
        {
            /*
            ws.Cells[row, startColumn++] = CatName;
            ws.Cells[row, startColumn++] = Catcount;
            ws.Cells[row, startColumn] = CatSumm;
            */

        }
    }
}
