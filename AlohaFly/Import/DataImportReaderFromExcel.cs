using AlohaService.ServiceDataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Windows.Controls;

namespace AlohaFly.Import
{
    class DataImportReaderFromExcel : DataImportReader
    {
        public DataImportReaderFromExcel()
        { }

        public override DelegateCommand MenuAction()
        {
            return new DelegateCommand((_) => { ShowDataAsk(); }); ;
        }

        private void ShowDataAsk()
        {
            if (UI.UIModify.ShowOpenFileDialog(out string filePath, "Microsoft Excel((*.xls *) | (*.xlsx *) )", "Укажите файл для импорта"))
            {
                try
                {
                    var ewb = new ExcelWorkBook();
                    var ws = ewb.GetWB(filePath);
                    if (ws == null) return;

                    if (!Parce(ws, out List<OrderFlight> of))
                    {
                        UI.UIModify.ShowAlert($"Ошибка в формате файла {filePath} .");
                    }
                    string res = "";
                    if (of != null && of.Count > 0)
                    {
                        foreach (var order in of)
                        {

                            if (Models.AirOrdersModelSingleton.Instance.AddOrder(order, order.DishPackages))
                            {
                                res += $"Создал заказ №{order.Id} Дата: {order.DeliveryDate} {Environment.NewLine}";
                            }
                            else
                            {
                                res += $"Ошибка создания заказа. Дата: {order.DeliveryDate} {Environment.NewLine}";
                            }
                        }
                        UI.UIModify.ShowAlert(res);
                    }
                    else
                    {
                        UI.UIModify.ShowAlert($"В файле {Environment.NewLine} " +
                            $"{filePath} {Environment.NewLine}" +
                            $"не найдено заказов");
                    }


                }
                catch (Exception e)
                {
                    UI.UIModify.ShowAlert($"Ошибка открытия файла  {Environment.NewLine} " +
                        $"{filePath} {Environment.NewLine}" +
                        $"тектс ошибки {Environment.NewLine}" +
                        $"{e.Message}");
                    return;
                }
            }

        }


        private bool GetDt(string s, out DateTime res)
        {
            res = new DateTime();
            if (s.Length < 11) return false;
            try
            {
                res = new DateTime(Convert.ToInt32(s.Substring(6, 4)), Convert.ToInt32(s.Substring(3, 2)),
                    Convert.ToInt32(s.Substring(0, 2)));


                return true;
            }
            catch
            {
                return false;
            }

        }

        private string GetExcelCellValue(Microsoft.Office.Interop.Excel.Worksheet ws, int row, int column)
        {
            object bcOb = ws.Cells[row, column].Value2;
            if (bcOb == null) return "";
            return ws.Cells[row, column].Value.ToString().Trim();
        }

        private char RepDemSep
        {
            get
            {
                decimal d = 1.1M;
                return d.ToString()[1];
            }
        }

        private decimal GetExcelCellValueDec(Microsoft.Office.Interop.Excel.Worksheet ws, int row, int column)
        {
            string s = ws.Cells[row, column].Value.ToString();

            return Convert.ToDecimal(s.Replace('.', RepDemSep).Replace(',', RepDemSep));
        }


        private int maxExcelRow = 65536;
        private bool Parce(Microsoft.Office.Interop.Excel.Worksheet ws, out List<OrderFlight> orderFlights)
        {
            orderFlights = new List<OrderFlight>();
            try
            {
                int sColumn = 1;
                int row = 3;
                List<DateTime> skipDates = new List<DateTime>();
                while (GetExcelCellValue(ws, row, sColumn) == "Дата")
                {
                    row++;
                    while (GetExcelCellValue(ws, row, sColumn) != "Итого" && row < maxExcelRow)
                    {
                        bool dateParceRes = true;
                        if (!GetDt(GetExcelCellValue(ws, row, sColumn), out DateTime dt)) return false;
                        if (skipDates.Contains(dt)) { dateParceRes = false; }
                        var order = new OrderFlight();
                        if (orderFlights.Any(a => a.DeliveryDate == dt))
                        {
                            order = orderFlights.FirstOrDefault(a => a.DeliveryDate == dt);
                        }
                        else
                        {
                            order = new OrderFlight()
                            {
                                AirCompanyId = 67,
                                AirCompany = DataExtension.DataCatalogsSingleton.Instance.AllAirCompanies.SingleOrDefault(a => a.Id == 67),
                                FlightNumber = dt.ToString("dd.MM.yyyy"),
                                DeliveryPlace = DataExtension.DataCatalogsSingleton.Instance.DeliveryPlaces.SingleOrDefault(a => a.Id == 4),
                                DeliveryDate = dt,
                                CreatedById = Authorization.CurentUser.Id,
                                DishPackages = new List<DishPackageFlightOrder>(),
                                OrderStatus = OrderStatus.InWork
                            };
                        }

                        while (GetExcelCellValue(ws, row, sColumn + 1) != "Итого" && row < maxExcelRow)
                        {
                            if (!dateParceRes) { row++; continue; };

                            try
                            {
                                int rCode = Convert.ToInt32(GetExcelCellValue(ws, row, sColumn + 1));
                                decimal count = GetExcelCellValueDec(ws, row, sColumn + 3);
                                if (!DataExtension.DataCatalogsSingleton.Instance.Dishes.Any(a => a.SHGastroId == rCode))
                                {
                                    string name = GetExcelCellValue(ws, row, sColumn + 2);
                                    var qres = UI.UIModify.ShowPromt($"Нашел неопознанное блюдо {Environment.NewLine}" +
                                        $" Дата {dt.ToString("dd.MM.yyyy")} {Environment.NewLine} " +
                                        $" {rCode}. {name} {Environment.NewLine} " +
                                        $"Продолжить без этого блюда?",
                                        header: "Неопознанное блюдо", confirm: true);

                                    if (!qres.DialogResult.GetValueOrDefault())
                                    {
                                        dateParceRes = false;
                                        skipDates.Add(dt);
                                        continue;
                                    };
                                    row++;
                                    continue;
                                }
                                var dish = DataExtension.DataCatalogsSingleton.Instance.Dishes.FirstOrDefault(a => a.SHGastroId == rCode);
                                var d = new DishPackageFlightOrder()
                                {
                                    DishId = dish.Id,
                                    Amount = count,
                                    Printed = true,
                                    TotalPrice = dish.PriceForFlight,
                                    DishName = dish.Name,
                                    PositionInOrder = order.DishPackages.Count + 1,
                                    Dish = dish
                                };
                                order.DishPackages.Add(d);
                                row++;
                            }
                            catch (Exception e)
                            {
                                var qres = UI.UIModify.ShowPromt($"Ошибка в строке №{row} {Environment.NewLine}" +
                                        $" Дата {dt.ToString("dd.MM.yyyy")} {Environment.NewLine} " +
                                        $" Текст ошибки: {Environment.NewLine}  {e.Message} {Environment.NewLine} " +
                                        $"Продолжить без этой строки?",
                                        header: "Ошибка разбора xlsx", confirm: true);
                                dateParceRes = !qres.DialogResult.GetValueOrDefault();
                                if (!dateParceRes) { skipDates.Add(dt); }
                                continue;


                            }


                        }
                        if (dateParceRes)
                        {
                            if (order.DishPackages.Count > 0)
                            {
                                if (!orderFlights.Any(a => a.DeliveryDate == dt))
                                {
                                    orderFlights.Add(order);
                                }

                            }
                        }
                        row++;

                    }
                    sColumn += 5;
                    row = 3;
                }
                return true;
            }
            catch (Exception e)
            {
                UI.UIModify.ShowAlert($"Ошибка Parce {e.Message}");
                return false;
            }

        }


    }
}
