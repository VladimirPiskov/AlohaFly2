using AlohaFly.Models;
using AlohaService.Interfaces;
using AlohaService.ServiceDataContracts;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Telerik.Windows.Controls;

namespace AlohaFly.LabelsPrint
{
    class LabelPapersVisualViewModel : ViewModelPane
    {
        static Logger _logger = LogManager.GetCurrentClassLogger();
        IOrderLabel Order;
        Dish d;
        bool PrintDish = false;
        public LabelPapersVisualViewModel(IOrderLabel order)
        {
            Order = order;
            PapersInit();
            PrintCommand = new DelegateCommand((_) =>
            {
                PrintWinPrinter();
            });
        }

        public LabelPapersVisualViewModel(Dish _d)
        {
            PrintDish = true;
            d = _d;
            PapersInit();
            PrintCommand = new DelegateCommand((_) =>
            {
                PrintWinPrinter();
            });
        }

        public void ShowMe()
        {
            if (Order == null)
            {
                Header = $"Печать наклеек к блюду {d.Name}";
            }
            else
            {
                Header = $"Печать наклеек заказ №{Order.Id}";
            }
            var vis = new CtrlLabelPapersVisual();
            vis.DataContext = this;
            MainClass.ShowUC(vis);
        }


        public ICommand PrintCommand { get; set; }

        void PrintWinPrinter()
        {
            string PrName = "";
            try
            {

                PrintDialog Pd = new PrintDialog();
                if (Pd.ShowDialog().Value)
                {
                    foreach (var p in Papers)
                    {
                        var vis = p.XamlClone<CtrlLabelsPaper>();
                        Pd.PageRangeSelection = PageRangeSelection.AllPages;
                        vis.Height = Pd.PrintableAreaHeight;
                        vis.Width = Pd.PrintableAreaWidth;
                        vis.Padding = new Thickness(0);
                        Pd.PrintVisual(vis, "Hello");

                    }
                }
                Pd.PrintQueue.Dispose();

                Pd = null;
            }
            catch (Exception e)
            {
                _logger.Error($"Error PrintWinPrinter {e.Message}");
            }

            //vis = null;
            GC.Collect();
        }

        void CreateLabelsOfDish(Dish d, DateTime dt)
        {

            int nCount = DataExtension.DataCatalogsSingleton.Instance.ItemLabelsInfo.Where(a => a.ParenItemId == d.Id).Count();
            foreach (var l in DataExtension.DataCatalogsSingleton.Instance.ItemLabelsInfo.Where(a => a.ParenItemId == d.Id).OrderBy(a => a.SerialNumber))
            {
                int logoType = 0;
                if (Order is OrderToGo) { logoType = 1; }
                var lvm = new LabelImageViewModel(d, l, dt, logoType);
                var ctrlImage = new CtrlLabelImage() { DataContext = lvm };
                Labels.Add(ctrlImage);
            }
        }

        void PapersInit()
        {

            Labels = new List<CtrlLabelImage>();
            Papers = new List<CtrlLabelsPaper>();
            if (!PrintDish)
            {
                foreach (var d in Order.DishPackagesForLab.OrderBy(a => a.PositionInOrder))
                {
                    if (!d.PrintLabel) { continue; }
                    //foreach(var l in d.DishId)
                    for (int sNum = 0; sNum < d.LabelSeriesCount; sNum++)
                    {
                        CreateLabelsOfDish(d.Dish, Order.DeliveryDate);
                    }
                }
            }
            else
            {
                CreateLabelsOfDish(d, DateTime.Now);
            }


            if (Labels.Count() == 0) return;
            int CurentLabelNum = 0;
            var paper = new CtrlLabelsPaper();

            paper.DataContext = new LabelPageViewModel(Order, d);
            Papers.Add(paper);
            foreach (var l in Labels)
            {
                if (!paper.AddLabel(l))
                {
                    if (l != Labels.Last())
                    {
                        paper = new CtrlLabelsPaper();
                        paper.DataContext = new LabelPageViewModel(Order, d);
                        Papers.Add(paper);
                    }
                }
                CurentLabelNum++;
            }

        }

        List<CtrlLabelsPaper> Papers { set; get; }
        List<CtrlLabelImage> Labels { set; get; }

        //List<RadBookItem> items;
        List<PageDataItem> items;
        // public List<RadBookItem> Items
        public List<PageDataItem> Items
        {
            get
            {
                if (items == null)
                {
                    items = new List<PageDataItem>();

                    foreach (var p in Papers)
                    {

                        /*
                        RadBookItem r = new RadBookItem()
                        {
                            Cursor = Cursors.Hand,
                            //Background = new SolidColorBrush(Colors.Red),
                            Content = p
                        };
                     */

                        PageDataItem r = new PageDataItem()
                        {
                            PageNumberStr = $"Стр. {Papers.IndexOf(p) + 1} из {Papers.Count}",
                            Item = p,
                        };

                        items.Add(r);
                    }
                }
                return items;
            }

        }


    }
    public class PageDataItem
    {

        public CtrlLabelsPaper Item { set; get; }
        public string PageNumberStr { set; get; }
    }
    public static class ExtensionMethods
    {
        public static T XamlClone<T>(this T original)
          where T : class
        {
            if (original == null)
                return null;

            object clone;
            using (var stream = new MemoryStream())
            {
                XamlWriter.Save(original, stream);
                stream.Seek(0, SeekOrigin.Begin);
                //var tw = new StreamWriter(@"D:\test.xml");
                //XamlWriter.Save(original, tw);
                // tw.Close();


                clone = XamlReader.Load(stream);
            }

            if (clone is T)
                return (T)clone;
            else
                return null;
        }
    }

}
