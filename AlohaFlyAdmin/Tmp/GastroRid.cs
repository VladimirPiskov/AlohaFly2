using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlohaFly;
using AlohaFly.Import;

namespace AlohaFlyAdmin
{
    static class GastroRid
    {
        public static void Insert()
        {
            AlohaFly.DataExtension.DataCatalogsSingleton.Instance.DataCatalogsFill();
            var ewb = new ExcelWorkBook();
            var ws = ewb.GetWB(@"e:\t\1.xlsx");
            if (ws == null) return;
            for (int row = 4; row < 73; row++)
            {
                long k =Convert.ToInt64(ws.Cells[row,2].Value2.ToString());
                long a = Convert.ToInt64(ws.Cells[row, 3].Value2.ToString());

                var d = AlohaFly.DataExtension.DataCatalogsSingleton.Instance.DishData.Data.Where(b => b.Barcode == a).FirstOrDefault();
                d.SHGastroId=k;
                AlohaFly.DBProvider.Client.UpdateDish(d);
            }
        }
    }
}
