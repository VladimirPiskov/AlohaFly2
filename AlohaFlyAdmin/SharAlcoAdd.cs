using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace AlohaFlyAdmin
{
  public static  class SharAlcoAdd
    {

        public static void AddToCFC()
        {
            var dList = AlohaFly.DBProvider.Client.GetDishList();
            var res=  dList.Result.Where(a=>a.IsShar & a.Barcode >=9600 & a.Barcode < 9800).ToList();



            DishesToAloha.AddToSql(res);


        }

            public static  void DoIt()
        {

            Application app;
            Workbook Wb;
            Worksheet Ws;

            app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Wb = app.Workbooks.Open($@"d:\t\v.xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            Ws = (Microsoft.Office.Interop.Excel.Worksheet)Wb.ActiveSheet;
            int curentBarcode = 9627;
            //42-91
            for (int row = 42; row < 91; row++)
            {
                object range1 = Ws.Cells[row, 3].Value2;
                if (range1 == null) continue;
                    int price = 0;
               // if (Ws.Cells[row, 3].value.IsNullOrEmpty) continue;
                if (int.TryParse(Ws.Cells[row, 3].Value2.ToString(), out price))
                {
                    if (price == 0) continue;
                    string name = Ws.Cells[row, 2].Value2.ToString();
                    AlohaService.ServiceDataContracts.Dish d = new AlohaService.ServiceDataContracts.Dish()
                    {
                        Barcode = curentBarcode,
                        DishKitсhenGroupId = 20,
                        DishLogicGroupId = 9,
                        IsShar = true,
                        IsActive = true,
                        EnglishName = name,
                        LabelRussianName = name,
                        IsAlcohol = true,
                        IsTemporary = false,
                        IsToGo = false,
                        LabelEnglishName = name,
                        Name = name,
                        PriceForFlight = price,
                        PriceForDelivery = 0,


                    };
                  var res=  AlohaFly.DBProvider.Client.CreateDish(d);
                    Ws.Cells[row, 1] = curentBarcode;
                    curentBarcode++;
                }
            }
            

        }
       

    }
}
