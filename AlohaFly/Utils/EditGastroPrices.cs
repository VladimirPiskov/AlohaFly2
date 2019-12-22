using AlohaFly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using System.Text;
using System.Threading.Tasks;

namespace AlohaFly.Utils
{
     public static class EditGastroPrices
    {
        public static void Edit()
        {
            var ords = AirOrdersModelSingleton.Instance.Orders.Where(a => a.AirCompanyId == 67);
            foreach (var ord in ords)
            {
                if (ord.DishPackages.Any(dp => dp.TotalPrice != dp.Dish.PriceForFlight))
                {
                    ord.DishPackages.Where(dp => dp.TotalPrice != dp.Dish.PriceForFlight).ToList().ForEach(dp => dp.TotalPrice = dp.Dish.PriceForFlight);
                    ord.IsSHSent = false;
                    DBProvider.Client.UpdateOrderFlight(ord, Authorization.CurentUser.Id);
                }
            }
        }
    }
}
