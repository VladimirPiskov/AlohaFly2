using AlohaService.Interfaces;
using AlohaService.ServiceDataContracts;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;

namespace AlohaFly.PrintRecieps
{
    public static class PrintOnWinPrinter
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        static public bool PrintOrderToGoToKitchen(IOrderLabel order, out List<string> resDescr, List<IDishPackageLabel> deletedDishes = null)
        {
            logger.Debug($"PrintOrderToGoToKitchen Order: {order?.Id}");

            bool res = true;
            resDescr = new List<string>();
            List<FiscalCheckVisualString> strs;
            if (deletedDishes == null)
            {
                strs = GetFStrings(order, 0);
            }
            else
            {
                strs = GetFStringsForDelete(order, deletedDishes);
            }



            if (!PrintDoc2(strs, 1, Properties.Settings.Default.HotColdPrinterName))
            {
                resDescr.Add("Ошибка при печати на принтере горячих блюд");
                res = false;
            }
            if (!PrintDoc2(strs, 1, Properties.Settings.Default.HotColdPrinterName))
            {
                resDescr.Add("Ошибка при печати на принтере горячих блюд");
                res = false;
            }
            if (!PrintDoc2(strs, 1, Properties.Settings.Default.HotColdPrinterName))
            {
                resDescr.Add("Ошибка при печати на принтере горячих блюд");
                res = false;
            }

            if (!PrintDoc2(strs, 1, Properties.Settings.Default.ConditerPrinterName))
            {
                resDescr.Add("Ошибка при печати на принтере кондитера");
                res = false;

            }
            if (!PrintDoc2(strs, 1, Properties.Settings.Default.UpackPrinterName))
            {
                resDescr.Add("Ошибка при печати на принтере упаковки");
                res = false;

            }
            
            logger.Debug($"ToGo PrintOrderToGoToKitchen Order: {order?.Id} end");
            return res;
        }



        static List<FiscalCheckVisualString> GetFStrings(IOrderLabel order, int grId)
        {
            logger.Debug($"GetFStrings Order: {order?.Id}");

            var res = new List<FiscalCheckVisualString>();
            res.Add(new FiscalCheckVisualString("   ", true, true));
            if (order is OrderToGo)
            {
                res.Add(new FiscalCheckVisualString($"Заказ ToGo № {order.Id}", true, true));

                if (order.DishPackagesForLab.Any(a => a.Printed))
                {
                    res.Add(new FiscalCheckVisualString($"Дополнение", true, false));
                }


            }

            if (order is OrderFlight)
            {
                res.Add(new FiscalCheckVisualString($"Заказ ToFly № {order.Id}", true, true));
                if (order.DishPackagesForLab.Any(a => a.Printed))
                {
                    res.Add(new FiscalCheckVisualString($"Дополнение", true, false));
                }
            }

            if (order is OrderFlight)
            {
                res.Add(new FiscalCheckVisualString(order.ReadyTime.ToString("HH:mm dd/MM/yy"), true, false));
            }
            else
            {
                res.Add(new FiscalCheckVisualString(order.DeliveryDate.ToString("HH:mm dd/MM/yy"), true, false));
            }
            res.Add(new FiscalCheckVisualString("   ", true, false));



            if (order is OrderToGo)
            {
                var ordTG = (OrderToGo)order;
                if (ordTG.MarketingChannel != null)
                {
                    res.Add(new FiscalCheckVisualString($"Канал продаж:", true));
                    res.Add(new FiscalCheckVisualString(ordTG.MarketingChannel.Name, false));
                }
            }

            if (!(order is OrderFlight) && order.CommentKitchen != null && order.CommentKitchen.Trim().Length > 0)
            {
                res.Add(new FiscalCheckVisualString($"Комментарий к заказу", true));
                res.Add(new FiscalCheckVisualString(order.CommentKitchen, false));
            }
            res.Add(new FiscalCheckVisualString("   ", true, true));

            if (grId == 0)
            {
                foreach (var dp in order.DishPackagesForLab.Where(a => !a.Printed))
                {

                    res.Add(new FiscalCheckVisualString(dp.Dish.Name, dp.Amount.ToString("0.#"), false));

                    if (!(order is OrderFlight))
                    {
                        if (dp.Comment != null && dp.Comment.Trim() != "")
                        {
                            res.Add(new FiscalCheckVisualString($"   {dp.Comment}", false));
                        }
                    }

                }
                res.Add(new FiscalCheckVisualString("   ", true, true));
                res.Add(new FiscalCheckVisualString("   ", true, true));
                res.Add(new FiscalCheckVisualString("___________________________", true, false));

                logger.Debug($"GetFStrings Order: {order?.Id} Return:");
                foreach (var f in res)
                {
                    logger.Debug(f.strLeft + " " + f.strRight);
                }
                logger.Debug($"GetFStrings Order: {order?.Id} Return end:");
                return res;

            }
            return null;
        }



        static List<FiscalCheckVisualString> GetFStringsForDelete(IOrderLabel order, List<IDishPackageLabel> dishes)
        {
            if ((dishes == null) || (dishes.Count < 1)) return null;
            logger.Debug($"GetFStringsForDelete: {dishes.First().Dish?.Name}");

            var res = new List<FiscalCheckVisualString>();
            res.Add(new FiscalCheckVisualString("   ", true, true));

            if (order is OrderToGo)
            {
                res.Add(new FiscalCheckVisualString($"Заказ ToGo № {order.Id}", true, true));
                res.Add(new FiscalCheckVisualString($"УДАЛЕНИЕ ПОЗИЦИЙ!!!", true, false));

            }

            if (order is OrderFlight)
            {
                res.Add(new FiscalCheckVisualString($"Заказ ToFly № {order.Id}", true, true));
                res.Add(new FiscalCheckVisualString($"УДАЛЕНИЕ ПОЗИЦИЙ!!!", true, false));

            }

            if (order is OrderFlight)
            {
                res.Add(new FiscalCheckVisualString(order.ReadyTime.ToString("HH:mm dd/MM/yy"), true, false));
            }
            else
            {
                res.Add(new FiscalCheckVisualString(order.DeliveryDate.ToString("HH:mm dd/MM/yy"), true, false));
            }
            res.Add(new FiscalCheckVisualString("   ", true, false));



            if (order is OrderToGo)
            {
                var ordTG = (OrderToGo)order;
                if (ordTG.MarketingChannel != null)
                {
                    res.Add(new FiscalCheckVisualString($"Канал продаж:", true));
                    res.Add(new FiscalCheckVisualString(ordTG.MarketingChannel.Name, false));
                }
            }

            if (!(order is OrderFlight) && order.CommentKitchen != null && order.CommentKitchen.Trim().Length > 0)
            {
                res.Add(new FiscalCheckVisualString($"Комментарий к заказу", true));
                res.Add(new FiscalCheckVisualString(order.CommentKitchen, false));
            }
            res.Add(new FiscalCheckVisualString("   ", true, true));

            //  if (grId == 0)
            {
                foreach (var dp in dishes)
                {

                    res.Add(new FiscalCheckVisualString(dp.Dish.Name, dp.Amount.ToString("0.#"), false));
                    if (dp.DeletedStatus == 1)
                    {
                        res.Add(new FiscalCheckVisualString($"Удаление со списанием"));
                        res.Add(new FiscalCheckVisualString($"Причина: {dp.SpisPayment?.Name}"));
                    }
                    else
                    {
                        res.Add(new FiscalCheckVisualString($"Удаление без списания"));
                    }


                    if (!(order is OrderFlight))
                    {
                        if (dp.Comment != null && dp.Comment.Trim() != "")
                        {
                            res.Add(new FiscalCheckVisualString($"   {dp.Comment}", false));
                        }
                    }

                }
                res.Add(new FiscalCheckVisualString("   ", true, true));
                res.Add(new FiscalCheckVisualString("   ", true, true));
                res.Add(new FiscalCheckVisualString("___________________________", true, false));

                logger.Debug($"GetFStrings Order: {order?.Id} Return:");
                foreach (var f in res)
                {
                    logger.Debug(f.strLeft + " " + f.strRight);
                }
                logger.Debug($"GetFStrings Order: {order?.Id} Return end:");
                return res;

            }

        }

        static private bool PrintDoc2(List<FiscalCheckVisualString> Args, int TryCount, string printerName)
        {
            int W = 268;

            ctrlCheckVisual vis = new ctrlCheckVisual();
            logger.Error($"PrintDoc2 printerName: {printerName}");
            if (Args != null)
            {
                // BitmapImage QrImg = FiscalCheckCreator.CreateQRBitmap(Args.QRAsStr, 130, 130);
                vis.CreateCheck(Args);
                vis.Visibility = Visibility.Visible;
                // string PrName = @"Predchek4";
                //string PrName = iniFile.FRSPrinterName;
                try
                {
                    //  Utils.ToCardLog("PrintDoc PrName " + PrName);
                    PrintDialog Pd = new PrintDialog();
                    Pd.PageRangeSelection = PageRangeSelection.AllPages;
                    PrintServer Ps = new PrintServer();
                    PrintQueue PQ = new PrintQueue(Ps, printerName);
                    Pd.PrintQueue = PQ;
                    // Pd.ShowDialog();
                    PrintTicket Pt = Pd.PrintTicket;
                    Pt.PageMediaSize = new PageMediaSize(W, 11349);
                    Pt.PageBorderless = PageBorderless.Borderless;
                    Pt.PageResolution = new PageResolution(203, 203);
                    Pt.PageScalingFactor = 1;
                    Pt.TrueTypeFontMode = TrueTypeFontMode.DownloadAsRasterFont;

                    Size pageSize = new Size(W - 10, Pd.PrintableAreaHeight);
                    //pageSize = new Size(W, H);
                    ((UserControl)vis).Measure(pageSize);
                    ((UserControl)vis).Arrange(new Rect(0, 0, W - 10, ((UserControl)vis).Height));
                    //((UserControl)vis).Arrange(new Rect(0, 0, W, H));

                    Pd.PrintVisual(vis, "Hello");
                    Ps.Dispose();
                    Pd.PrintQueue.Dispose();

                    Pd = null;

                }
                catch (Exception e)
                {
                    //   Utils.ToCardLog("PrintDoc Error " + e.Message);
                    logger.Error("PrintDoc Error " + e.Message);
                    if (TryCount < 5)
                    {
                        logger.Error("Try again " + TryCount);
                        System.Threading.Thread.Sleep(300);
                        GC.Collect();
                        return PrintDoc2(Args, TryCount + 1, printerName);
                    }
                    return false;
                }

            }
            else
            {
                logger.Debug("PrintDoc2 Args==null");
            }
            vis = null;
            GC.Collect();
            logger.Debug("PrintDoc2 Ok");
            return true;

        }
    }
}
