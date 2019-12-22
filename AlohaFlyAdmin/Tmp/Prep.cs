using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlohaService.ServiceDataContracts;
using Microsoft.Office.Interop.Excel;


namespace AlohaFlyAdmin.Tmp
{
   public static  class Prep
    {
        public static void Doit()
        {

            Application app;
            Workbook Wb;
            Worksheet Ws;

            app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Wb = app.Workbooks.Open($@"d:\t\g2.xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            int curentBarcode = 9627;
            //42-91
            var bcs = new Dictionary<int, decimal>();
            for (int row = 6; row < 62; row++)
            {
                object bcOb = Ws.Cells[row, 2].Value2;
                if (bcOb == null) continue;
                int bc = 0;

                int.TryParse(Ws.Cells[row, 2].Value2.ToString(), out bc);
                    decimal price = 0;
                string s = Ws.Cells[row, 6].Value2.ToString();

                decimal.TryParse(s.Replace(",",","), out price);
                bcs.Add(bc, price);
                        }

            AlohaFly.DataExtension.DataCatalogsSingleton.Instance.DataCatalogsFill();

            var orders = AlohaFly.DBProvider.GetOrders(new DateTime(2019, 8, 1), new DateTime(2019, 8, 30), out List<OrderFlight> svo);

            AlohaFly.Authorization.CurentUser = new User() {Id=1 };

            foreach (int bc in bcs.Keys)
            {
                var dList = new List<Dish>();
                if (AlohaFly.DataExtension.DataCatalogsSingleton.Instance.Dishes.Where(a => a.Barcode == bc).Count() > 1)
                {
                    dList.Add(AlohaFly.DataExtension.DataCatalogsSingleton.Instance.Dishes.Where(a => a.Barcode == bc).First());
                }

                var d = AlohaFly.DataExtension.DataCatalogsSingleton.Instance.Dishes.SingleOrDefault(a => a.Barcode == bc);
                d.PriceForFlight = bcs[bc];
                AlohaFly.DBProvider.Client.UpdateDish(d);


            }

            foreach (var ord in orders)
            {
                bool needsave = false;
                foreach (var d in ord.DishPackages)
                {
                    if (bcs.ContainsKey((int)d.Dish.Barcode))
                    {
                        d.TotalPrice = bcs[(int)d.Dish.Barcode];
                        needsave = true;
                    }
                    }
                if (needsave) {
                    AlohaFly.DBProvider.UpdateOrderFlight(ord);
                }
            }

        }
    }
}
