using AlohaService.ServiceDataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlohaFly.Analytics
{
    public class ReportsGeneratorSingleton
    {


        private ReportsGeneratorSingleton()
        {

        }

        static ReportsGeneratorSingleton instance;
        public static ReportsGeneratorSingleton Instance
        {

            get
            {
                if (instance == null)
                {
                    instance = new ReportsGeneratorSingleton();
                }
                return instance;
            }
        }

        public List<SebesReportCatData> GetSebesReportData(DateTime sDt, DateTime eDt, bool toFly=true)
        { 
            var data= DataExtension.DataCatalogsSingleton.Instance.OrdersFlightData.Data.Where(a => a.DeliveryDate >= sDt && a.DeliveryDate < eDt.AddDays(1));
            var dishes = data.SelectMany(a => a.DishPackages).Select(a => a.Dish).Select(a=>a.Id).Distinct();
            var tmp = new List<SebesReportCatData>();
            //SH.SHWrapperExt.ConnectSH();
            foreach (var d in dishes)
            {
                try
                {
                    var dish = DataExtension.DataCatalogsSingleton.Instance.DishData.Data.FirstOrDefault(a => a.Id == d);
                    var sCat = new SebesReportCatData();
                    if (tmp.Any(a => a.Id == dish.DishLogicGroupId))
                    {
                        sCat = tmp.Single(a => a.Id == dish.DishLogicGroupId);
                    }
                    else
                    {
                        sCat.Id = dish.DishLogicGroup.Id;
                        sCat.Name = dish.DishLogicGroup.Name;
                        tmp.Add(sCat);
                    }
                    sCat.Dishes.Add(new SebesReportdishData()
                    {
                        Name = dish.Name,
                        Price = dish.IsToGo ? dish.PriceForDelivery : dish.PriceForFlight,
                        Sebes = (decimal)SH.SHWrapperExt.GetSHSebes((int)dish.SHId, sDt, eDt)
                    }) ;

                }
                catch(Exception e)
                { 

                }

            }
           // SH.SHWrapperExt.Disconnect();
            return tmp;
                


        }


    }

    public class SebesReportData
    { 
        
    
    }

    public class SebesReportCatData
    {
        public long Id { set; get; }
        public string Name { set; get; }

        public List<SebesReportdishData> Dishes { set; get; } = new List<SebesReportdishData>();

    }

    public class SebesReportdishData
    {
        public string Name { set; get; }
        public decimal Price { set; get; }
        public decimal Sebes { set; get; }


    }
}
