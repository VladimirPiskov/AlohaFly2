using AlohaFly.DataExtension;
using AlohaService.ServiceDataContracts;
using NLog;
using StoreHouseConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlohaFlyAdmin
{
    static class SHNewBaseExport
    {
        static Logger _logger = LogManager.GetCurrentClassLogger();
        public static void Export()
        {
            Connect();
            //GetAllExpCats();
                GetDishesFromSH();



            //  AddGoodsToGo();
            //AddGoodsSVO();
            //  AddGoods(); //Это не делать чтобы не задублировать!!!!

             //GetPlaces();
           // AddAirsCat();
            //UpdateDishesNewShId();
            //CreateGroupsToGo();
            //CreateGroups();
            //PrintNonCatsGoodsToGo();
            //PrintNonCatsGoods();
            //CreateGroups();



            GetGoodsTree();
            //GetUnits();
            //AddGoodsTest();
            //  AddGoodTest();
            //  GetCats1();

            //GetToFlyGoodsTreeDic();

        }
        static int UnitId = 6;
        static int ToFlyFolderId = 5;
        static int SharFolderId = 6;
        static int ToGoFolderId = 7;

        static TStoreHouse sh = null;

        static string Connect()
        {
            try
            {
                //195.13.163.36
                //3456

                sh = new TStoreHouse();

                string err = "";
                int errCode = 0;
                //var conn = sh.ConnectSH("195.13.163.36", 3456, "Администратор", "Ro369MP123",out err);
                var conn = sh.ConnectSH("79.174.68.175", 61333, "Admin", "1333", out errCode, out err);
                return conn ? "Ok" : err;
            }
            catch (Exception e)
            {
                //_logger.Error(e.Message);
                return e.Message;
            }

        }

        private static void CreateGroups()
        {

            var ShGroups = GetToFlyGoodsTreeDic();
            AlohaFly.DataExtension.DataCatalogsSingleton.Instance.DataCatalogsFill();
            var orders = AlohaFly.DBProvider.GetOrders(new DateTime(2019, 1, 1), new DateTime(2019, 3, 1), out List<OrderFlight> SVOOrders);
            var DList = new List<Dish>();
            foreach (var ord in orders)
            {
                foreach (var dp in ord.DishPackages)
                {
                    if ((dp.Dish.DishKitсhenGroup != null) && (dp.Dish.DishKitсhenGroup.SHIdToFly != 8))
                    {
                        if (!DList.Contains(dp.Dish))
                        {
                            DList.Add(dp.Dish);
                        }
                    }
                }
            }
            var grNames = DList.Select(a => a.DishKitсhenGroup).Distinct();


            foreach (var gr in grNames)
            {
                if (!ShGroups.Values.Contains(gr.Name.Trim()))
                {
                    string err = "";
                    int errCode = 0;
                    sh.InsGoodTree(ToFlyFolderId, (ToFlyFolderId * 100 + gr.Id).ToString(), gr.Name.Trim(), out errCode, out err);
                }
                else
                {
                    gr.SHIdToFly = ShGroups.Where(a => a.Value == gr.Name.Trim()).FirstOrDefault().Key;
                    AlohaFly.DBProvider.Client.UpdateDishKitchenGroup(gr);
                }
            }
            /*
                if (!ShGroups.Values.Contains(gr.Trim()))
                {

                }
            */

        }


        private static void CreateGroupsToGo()
        {

            var ShGroups = GetToGoGoodsTreeDic();
            AlohaFly.DataExtension.DataCatalogsSingleton.Instance.DataCatalogsFill();
            var orders = AlohaFly.DBProvider.GetOrderToGoList(new DateTime(2019, 1, 1), new DateTime(2019, 3, 1));
            var DList = new List<Dish>();
            foreach (var ord in orders)
            {
                foreach (var dp in ord.DishPackages)
                {
                    if ((dp.Dish.DishKitсhenGroup != null))
                    {
                        if (!DList.Contains(dp.Dish))
                        {
                            DList.Add(dp.Dish);
                        }
                    }
                }
            }
            var grNames = DList.Select(a => a.DishKitсhenGroup).Distinct();


            foreach (var gr in grNames)
            {
                if (!ShGroups.Values.Contains(gr.Name.Trim()))
                {
                    string err = "";
                    int errCode = 0;
                    sh.InsGoodTree(ToGoFolderId, (ToGoFolderId * 100 + gr.Id).ToString(), gr.Name.Trim(), out errCode, out err);
                }
                else
                {
                    gr.SHIdToGo = ShGroups.Where(a => a.Value == gr.Name.Trim()).FirstOrDefault().Key;
                    AlohaFly.DBProvider.Client.UpdateDishKitchenGroup(gr);
                }
            }
            /*
                if (!ShGroups.Values.Contains(gr.Trim()))
                {

                }
            */

        }


        private static void PrintNonCatsGoodsToGo()
        {
            AlohaFly.DataExtension.DataCatalogsSingleton.Instance.DataCatalogsFill();
            var orders = AlohaFly.DBProvider.GetOrderToGoList(new DateTime(2019, 1, 1), new DateTime(2019, 3, 1));
            var AirOrders = orders;//.Where(a => !AlohaFly.DBProvider.SharAirs.Contains(a.AirCompanyId.GetValueOrDefault()));
            var DList = new List<Dish>();
            foreach (var ord in AirOrders)
            {
                foreach (var dp in ord.DishPackages)
                {
                    if ((dp.Dish.DishKitсhenGroup == null))
                    {
                        if (!DList.Contains(dp.Dish))
                        {
                            DList.Add(dp.Dish);
                        }
                    }
                }
            }
            foreach (var d in DList.OrderBy(a => a.Id))
            {
                _logger.Debug($"Id: {d.Id}; Barcode {d.Barcode}; Name: {d.Name}");
            }


        }

        private static void PrintNonCatsGoods()
        {
            AlohaFly.DataExtension.DataCatalogsSingleton.Instance.DataCatalogsFill();
            var orders = AlohaFly.DBProvider.GetOrders(new DateTime(2019, 2, 1), new DateTime(2019, 4, 1), out List<OrderFlight> SVOOrders);
            var AirOrders = orders.Where(a => !AlohaFly.DBProvider.SharAirs.Contains(a.AirCompanyId.GetValueOrDefault()));
            var DList = new List<Dish>();
            foreach (var ord in AirOrders)
            {
                foreach (var dp in ord.DishPackages)
                {
                    if ((dp.Dish.DishKitсhenGroup == null) && dp.Dish.DishLogicGroupId != 1)
                    {
                        if (!DList.Contains(dp.Dish))
                        {
                            DList.Add(dp.Dish);
                        }
                    }
                }
            }
            foreach (var d in DList.OrderBy(a => a.Id))
            {
                _logger.Debug($"Id: {d.Id}; Barcode {d.Barcode}; Name: {d.Name}");
            }


        }


        private static void AddGoodsToGo()
        {
            var existD = GetToGoExistBarCodes();
            AlohaFly.DataExtension.DataCatalogsSingleton.Instance.DataCatalogsFill();
            var orders = AlohaFly.DBProvider.GetOrderToGoList(new DateTime(2019, 2, 1), new DateTime(2019, 4, 1));
            var DList = new List<Dish>();
            foreach (var ord in orders)
            {
                foreach (var dp in ord.DishPackages)
                {
                    if ((dp.Dish.DishKitсhenGroup != null) && (dp.Dish.DishKitсhenGroup.SHIdToGo != 0))
                    {
                        if ((!existD.Contains((int)dp.Dish.Barcode)) && (!existD.Contains((int)dp.Dish.Barcode + 10000)))
                        {
                            if (!DList.Contains(dp.Dish))
                            {
                                DList.Add(dp.Dish);
                            }
                        }
                    }
                }
            }


            foreach (var d in AlohaFly.DataExtension.DataCatalogsSingleton.Instance.Dishes.Where(a => a.Barcode >= 8470 && a.Barcode < 8550 && a.IsToGo))
            {
                if (!DList.Contains(d) && d.DishKitсhenGroup != null)
                {
                    if ((!existD.Contains((int)d.Barcode)) && (!existD.Contains((int)d.Barcode + 10000)))
                    {
                        DList.Add(d);
                    }
                }
            }


            foreach (var d in DList)
            {
                AddGoodToGo(d);
            }




        }

        private static void AddGoods()
        {
            var existD = GetToFlyExistBarCodes();
            AlohaFly.DataExtension.DataCatalogsSingleton.Instance.DataCatalogsFill();
            var orders = AlohaFly.DBProvider.GetOrders(new DateTime(2019, 2, 1), new DateTime(2019, 4, 1), out List<OrderFlight> SVOOrders);
            var DList = new List<Dish>();

            foreach (var ord in orders)
            {
                foreach (var dp in ord.DishPackages)
                {
                    if ((dp.Dish.DishKitсhenGroup != null) && (dp.Dish.DishLogicGroupId != 1) && (!dp.Dish.IsTemporary)
                        && (dp.Dish.DishKitсhenGroup.SHIdToFly != 0)

                        )
                    {
                        if (!DList.Contains(dp.Dish))
                        {
                            if ((!existD.Contains((int)dp.Dish.Barcode)) && (!existD.Contains((int)dp.Dish.Barcode + 10000)))
                            {
                                DList.Add(dp.Dish);
                            }
                        }
                    }
                }
            }

            foreach (var d in AlohaFly.DataExtension.DataCatalogsSingleton.Instance.Dishes.Where(a => a.Barcode >= 8470 && a.Barcode < 8550 && !a.IsToGo))
            {
                if (!DList.Contains(d) && d.DishKitсhenGroup != null)
                {
                    if ((!existD.Contains((int)d.Barcode)) && (!existD.Contains((int)d.Barcode + 10000)))
                    {
                        DList.Add(d);
                    }
                }
            }


            foreach (var d in DList)
            {
                AddGood(d);
            }




        }


        private static void AddGoodsSVO()
        {
            var existD = GetSVOExistBarCodes();
            AlohaFly.DataExtension.DataCatalogsSingleton.Instance.DataCatalogsFill();
            var orders = AlohaFly.DBProvider.GetOrdersSVO(new DateTime(2019, 2, 1), new DateTime(2019, 4, 1));
            var DList = new List<Dish>();

            foreach (var ord in orders)
            {
                foreach (var dp in ord.DishPackages)
                {
                    if (dp.Dish.DishLogicGroupId == 1)



                    {
                        if (!DList.Contains(dp.Dish))
                        {
                            if ((!existD.Contains((int)dp.Dish.Barcode)) && (!existD.Contains((int)dp.Dish.Barcode + 10000)))
                            {
                                DList.Add(dp.Dish);
                            }
                        }
                    }
                }
            }




            foreach (var d in DList)
            {
                AddGoodSVO(d);
            }




        }

        public static void UpdateDishesNewShId()
        {
            var dd = GetDishesFromSH().Where(a => a.prCode != null);
            AlohaFly.DataExtension.DataCatalogsSingleton.Instance.DataCatalogsFill();
            foreach (var d in AlohaFly.DataExtension.DataCatalogsSingleton.Instance.Dishes.Where(a => !a.IsTemporary))
            {
                if (dd.Any(a => (d.Barcode == GetBarCode(a.prCode)) || (d.Barcode + 10000 == GetBarCode(a.prCode))))
                {
                    var shd = dd.FirstOrDefault(a => (d.Barcode == GetBarCode(a.prCode)) || (d.Barcode + 10000 == GetBarCode(a.prCode)));
                    string s = shd.Name;
                    d.SHIdNewBase = dd.FirstOrDefault(a => (d.Barcode == GetBarCode(a.prCode)) || (d.Barcode + 10000 == GetBarCode(a.prCode))).Rid;
                    AlohaFly.DBProvider.Client.UpdateDish(d);
                }
            }


        }


        private static void GetAllExpCats()
        {
            string errMess;
            int errCode = 0;

            var shExpCats = sh.ExpCtgs(out errCode, out errMess);

            foreach (var cat in shExpCats.ListExpCtgs)
            {
                _logger.Debug($"{cat.Rid} {cat.Name}");
            }
        }

        private static void AddAirsCat()
        {
            string errMess;
            int errCode = 0;

            var shExpCats = sh.ExpCtgs(out errCode, out errMess);

            AlohaFly.DataExtension.DataCatalogsSingleton.Instance.DataCatalogsFill();
            foreach (var airc in AlohaFly.DataExtension.DataCatalogsSingleton.Instance.AirCompanies.Where(a=>a.SHId<80))
            {
                if (!shExpCats.ListExpCtgs.Select(a => a.Name).Contains(airc.Name.Trim()))
                {
                    bool res = sh.AddExpCtgs(airc.Name.Trim(), out errCode, out errMess);
                    shExpCats = sh.ExpCtgs(out errCode, out errMess);
                }
                if (shExpCats.ListExpCtgs.Any(a => a.Name == airc.Name.Trim()))
                {
                    airc.SHId = shExpCats.ListExpCtgs.FirstOrDefault(a => a.Name == airc.Name.Trim()).Rid;
                    AlohaFly.DBProvider.Client.UpdateAirCompany(airc);
                }
            }
            foreach (var p in AlohaFly.DataExtension.DataCatalogsSingleton.Instance.Payments.Where(a => a.ToGo && a.SHId<80))
            {
                if (!shExpCats.ListExpCtgs.Select(a => a.Name).Contains(p.Name))
                {
                    bool res = sh.AddExpCtgs(p.Name, out errCode, out errMess);
                    shExpCats = sh.ExpCtgs(out errCode, out errMess);

                    
                }
                if (shExpCats.ListExpCtgs.Any(a => a.Name == p.Name))
                {
                    p.SHId = shExpCats.ListExpCtgs.FirstOrDefault(a => a.Name == p.Name).Rid;
                    AlohaFly.DBProvider.Client.UpdatePayment(p);
                }
            }
        }

        private static List<TGoods> GetDishesFromSH()
        {
            string errMess;
            int errCode = 0;
            var groups = sh.GetGoodsTree(out errCode, out errMess).ListGoodsTree;
            var tmp = new List<TGoods>();



         // foreach (var grId in groups)
            {
                //var res = sh.GetGoods(grId.Rid, out errCode, out errMess);
                var res = sh.GetGoods(61, out errCode, out errMess);
                if (res.ListGoods.Count > 0)
                {
                    tmp.AddRange(res.ListGoods);
                    foreach (var d in res.ListGoods)
                    {
                        _logger.Debug($"{d.Rid} {d.Name}");
                            }
                }

            }
            return tmp;
        }

        private static void AddGoodsTest()
        {
            AlohaFly.DataExtension.DataCatalogsSingleton.Instance.DataCatalogsFill();

            var d = AlohaFly.DataExtension.DataCatalogsSingleton.Instance.Dishes.SingleOrDefault(a => a.Id == 29);
            AddGood(d);
        }

        //29
        private static void AddGood(Dish dish)
        {
            string err = "";

            int errCode = 0;

            if (!sh.AddGoods((int)dish.DishKitсhenGroup.SHIdToFly, dish.Name + "_ToFly", "Al_" + dish.Barcode.ToString(), (int)dish.Barcode, (int)dish.Barcode, 1, 2, UnitId, (double)dish.PriceForFlight, out errCode, out err))
            {
                int i = 0;
            }


        }

        private static void AddGoodToGo(Dish dish)
        {
            string err = "";
            int errCode = 0;
            if (!sh.AddGoods((int)dish.DishKitсhenGroup.SHIdToGo, dish.Name + "_ToGo", "Al_" + dish.Barcode.ToString(), (int)dish.Barcode, (int)dish.Barcode, 1, 2, UnitId, (double)dish.PriceForDelivery, out errCode, out err))
            {
                if (errCode != 0)

                {
                    int i = 0;
                }

            }


        }

        private static void AddGoodSVO(Dish dish)
        {
            string err = "";
            int errCode = 0;
            if (!sh.AddGoods(SharFolderId, dish.Name, "Al_" + dish.Barcode.ToString(), (int)dish.Barcode, (int)dish.Barcode, 1, 2, UnitId, (double)dish.PriceForFlight, out errCode, out err))
            {
                if (errCode != 0)

                {
                    int i = 0;
                }

            }


        }

        private static void AddGoodTest()
        {
            string err = "";
            int errCode = 0;
            Dish dish = new Dish()
            {
                Barcode = 23,
                PriceForFlight = 1250,
                Name = "Малина 100 гр."
            };

            //sh.AddGoods(8, dish.Name, dish.Barcode.ToString(), (int)dish.Barcode, (int)dish.Barcode, 1, 2, UnitId, (double)dish.PriceForFlight);

            if (!sh.AddGoods(38, dish.Name, "Al" + dish.Barcode.ToString(), (int)dish.Barcode, (int)dish.Barcode, 1, 2, UnitId, (double)dish.PriceForFlight, out errCode, out err))
            {
                int i = 0;
            }

        }



        private static List<string> GetCats1()
        {
            string errMess;
            int errCode = 0;
            var res = new List<string>();
            var un = sh.Categoty1(out errCode, out errMess);
            foreach (var u in un.ListCategory)
            {
                res.Add($"{u.Rid} {u.Name}");
            }
            return res;
        }



        private static List<string> GetPlaces()
        {
            string errMess;
            int errCode = 0;
            var res = new List<string>();
            var un = sh.PlaceImpl(out errCode, out errMess);
            foreach (var u in un.ListPlace)
            {
                res.Add($"{u.Rid} {u.Name}");
            }
            return res;
        }

        private static List<string> GetCats2()
        {
            string errMess;
            int errCode = 0;
            var res = new List<string>();
            var un = sh.Categoty2(out errCode, out errMess);
            foreach (var u in un.ListCategory)
            {
                res.Add($"{u.Rid} {u.Name}");
            }
            return res;
        }

        private static List<string> GetUnits()
        {
            string errMess;
            int errCode = 0;
            var res = new List<string>();
            var un = sh.Units(out errCode, out errMess);
            foreach (var u in un.ListUnit)
            {
                res.Add($"{u.Rid} {u.Name}");
            }
            return res;
        }



        private static List<string> GetGoodsTree()
        {
            string errMess;
            int errCode = 0;
            var res = new List<string>();
            var un = sh.GetGoodsTree(out errCode, out errMess);
            foreach (var u in un.ListGoodsTree)
            {
                //res.Add($"{u.Rid} {u.Name}");

                res.Add($"{u.Rid} {u.Name}");
                _logger.Debug($"Id: {u.Rid}; ParentId: {u.Parent};  {u.Name}");
            }
            return res;
        }


        private static List<int> GetToFlyExistBarCodes()
        {
            var tmp = new List<int>();
            string errMess;
            int errCode = 0;
            foreach (var grId in GetToFlyGoodsTreeDic().Keys)
            {
                var res = sh.GetGoods(grId, out errCode, out errMess);
                tmp.AddRange(res.ListGoods.Select(a => GetBarCode(a.prCode)));

            }
            return tmp;
        }


        private static List<int> GetSVOExistBarCodes()
        {
            var tmp = new List<int>();
            string errMess;
            int errCode = 0;
            foreach (var grId in GetSVOGoodsTreeDic().Keys)
            {
                var res = sh.GetGoods(grId, out errCode, out errMess);
                if (res.ListGoods.Count > 0)
                {
                    tmp.AddRange(res.ListGoods.Select(a => GetBarCode(a.prCode)));
                }

            }
            return tmp;
        }
        private static List<int> GetToGoExistBarCodes()
        {
            var tmp = new List<int>();
            string errMess;
            int errCode = 0;
            foreach (var grId in GetToGoGoodsTreeDic().Keys)
            {
                var res = sh.GetGoods(grId, out errCode, out errMess);
                tmp.AddRange(res.ListGoods.Select(a => GetBarCode(a.prCode)));

            }
            return tmp;
        }

        private static int GetBarCode(string shBarCode)
        {
            if (shBarCode == null) return 0;
            if (shBarCode.Contains("Al_"))
            {
                shBarCode = shBarCode.Substring(3);
            }
            return Convert.ToInt32(shBarCode);
        }

        private static Dictionary<int, string> GetToFlyGoodsTreeDic()
        {
            string errMess;
            int errCode = 0;
            var res = new Dictionary<int, string>();
            var un = sh.GetGoodsTree(out errCode, out errMess);
            foreach (var u in un.ListGoodsTree)
            {
                if (u.Parent == ToFlyFolderId)
                {
                    res.Add(u.Rid, u.Name.Trim());
                }
            }
            return res;
        }
        private static Dictionary<int, string> GetSVOGoodsTreeDic()
        {
            string errMess;
            int errCode = 0;
            var res = new Dictionary<int, string>();
            var un = sh.GetGoodsTree(out errCode, out errMess);
            foreach (var u in un.ListGoodsTree)
            {
                if ((u.Parent == SharFolderId) || (u.Rid == SharFolderId))
                {
                    res.Add(u.Rid, u.Name.Trim());
                }
                //res.Add(u.Rid, u.Name.Trim());
            }

            return res;
        }


        private static Dictionary<int, string> GetToGoGoodsTreeDic()
        {
            string errMess;
            int errCode = 0;
            var res = new Dictionary<int, string>();
            var un = sh.GetGoodsTree(out errCode, out errMess);
            foreach (var u in un.ListGoodsTree)
            {
                if (u.Parent == ToGoFolderId)
                {
                    res.Add(u.Rid, u.Name.Trim());
                }
            }
            return res;
        }

    }
}
