using AlohaFly.DataExtension;
using AlohaFly.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlohaFly.Analytics
{
    public class MenuItems
    {
        static List<MenuItemCatalog> _itemsCatalog;
        public static List<MenuItemCatalog> ItemsCatalog
        {
            get
            {
                if (_itemsCatalog == null)
                {
                    _itemsCatalog = new List<MenuItemCatalog>()
                    {
                        new MenuItemCatalog ("Выручка",ShowMainAnalytics, AccessTypeConst.Access_Analytics1,false),
                        new MenuItemCatalog ("Выручка по категориям",ShowPivotAir, AccessTypeConst.Access_Analytics2,false),
                        new MenuItemCatalog ("LTV2GO",ShowLTV, AccessTypeConst.Access_Analytics2,false),
                        /*
                        new MenuItemCatalog ("Отчет по себестоимости",new Action(() => { Task.Run(() =>
                             new  Reports.ExcelReports().AnaliticReportCreate(DataCatalogsSingleton.Instance.StartDt, DataCatalogsSingleton.Instance.EndDt)); }), AccessTypeConst.Access_Analytics2,false),
                    
                        */

                        new MenuItemCatalog ("Отчет по себестоимости",new Action(() => { new  Reports.ExcelReports().AnaliticReportCreate(DataCatalogsSingleton.Instance.StartDt, DataCatalogsSingleton.Instance.EndDt); }), AccessTypeConst.Access_Analytics2,false),


                        };
                }
                return _itemsCatalog;

            }
        }
        public static void ShowMainAnalytics()
        {
            var ctrl = new CtrlMainAnalitics()
            {
                DataContext = new MainAnalyticsViewModel()
                {
                    Header = "Выручка"
                }
            };
            MainClass.ShowUC(ctrl);
        }

        public static void ShowLTV()
        {
            var ctrl = new CtrlLTVToGo()
            {
                DataContext = new LTVViewModel()
                {
                    Header = "LTV2GO"
                }
            };
            MainClass.ShowUC(ctrl);
        }

        public static void ShowPivotAir()
        {
            var ctrl = new CtrlPivotGridAirCompanies()
            {
                DataContext = new PivotGridAirCompaniesViewModel()
                {
                    Header = "Выручка по категориям"
                }
            };
            MainClass.ShowUC(ctrl);
        }

    }
}
