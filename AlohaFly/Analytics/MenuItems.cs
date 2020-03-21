using AlohaFly.Utils;
using System.Collections.Generic;

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
