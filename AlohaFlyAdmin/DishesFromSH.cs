using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlohaService.ServiceDataContracts;
using NLog;
using StoreHouseConnect;


namespace AlohaFlyAdmin
{
    public  class DishesFromSH
    {
        static Logger _logger = LogManager.GetCurrentClassLogger();
        static TStoreHouse sh = null;
        public DishesFromSH()
        {
            Connect();
        }
        static string Connect()
        {
            try
            {
                _logger.Debug("Connect");
                sh = new TStoreHouse();
                _logger.Debug("new ok");
                string err = "";
                int errCode = 0;
                //var conn = sh.ConnectSH("195.13.163.36", 3456, "Администратор", "Ro369MP123",out err);
                var conn = sh.ConnectSH("195.13.163.36", 3456, "test", "test",out errCode, out err);
                _logger.Debug($"Conect {conn} err: {err}");
                return conn ? "Ok" : err;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return e.Message;
            }   

        }
        /*
        public void CreateInvoices()
        {
            AlohaFly.DataExtension.DataCatalogsSingleton.Instance.DataCatalogsFill();
            var orders = AlohaFly.DBProvider.GetOrders(new DateTime(2018, 12, 1), new DateTime(2018, 12, 31), out List<OrderFlight> SVOOrders);
            foreach (var ord in orders)
            {
                if (ord.OrderStatus != AlohaService.ServiceDataContracts.OrderStatus.Cancelled)
                {
                    string s = "";
                    var res= AlohaFly.SH.SHWrapper.CreateSalesInvoiceSync(ord,out s);
                    _logger.Debug($"CreateSalesInvoiceSync {ord.Id} {res} {s}");
                }
            }

        }
        */
        public TListPlaceImpl GetPlaces()
        {
            string errMess;
            int errCode = 0;
            var gr = sh.PlaceImpl(out errCode, out errMess);
            return gr;
        }

        public TListExpCtgs GetCats()
        {
            string errMess;
            int errCode = 0;
            var gr = sh.ExpCtgs(out errCode, out errMess);
            return gr;
        }

        public List<TTreeItem> GetGoups(int parentId)
        {
            string errMess;
            int errCode = 0;
            var gr = sh.GetGoodsTree(out errCode, out errMess).ListGoodsTree;
            return gr.Where(a => a.Parent == parentId).ToList();
        }



        public void PrintDishFromSH(bool toGo)
        {
            //Добавляем блюда, если их нет, синхронизируем имя если они есть

            var SHDs = GetDishesFromSH(toGo);
            foreach (var SHD in SHDs)
            {
                _logger.Debug($"GetDish dish rID: {SHD.Rid}, code: {SHD.prCode}, name: {SHD.Name}");
            }


            


        }


        public void AddDishToSQL(bool toGo)
        {
            //Добавляем блюда, если их нет, синхронизируем имя если они есть

            var SHDs = GetDishesFromSH(toGo);
            var SQLDs = AlohaFly.DBProvider.Client.GetDishList().Result;
            if (SQLDs == null) return;
            foreach (var SHD in SHDs)
            {
                _logger.Debug($"GetDish dish rID: {SHD.Rid}, code: {SHD.prCode}, name: {SHD.Name}");
                var d = SHD;
                string s = "";
                try
                {
                    if (SQLDs.Any(a => a.SHId == SHD.Rid))
                    {
                        _logger.Debug($"AddDishToSQL dish exist {SHD.prCode} {SHD.Name}");
                        /*
                        var SQLD = SQLDs.SingleOrDefault(a => a.SHId == SHD.Rid);
                        if (SHD.ru?.Trim() != "")
                        {
                            SQLD.Name = SHD.ru;
                        }
                        if (SHD.en?.Trim() != "")
                        {
                            SQLD.EnglishName = SHD.en;
                            //SQLD.LabelEnglishName = SHD.en;

                        }
                            AlohaFly.DBProvider.Client.UpdateDish(SQLD);
                        */
                    }
                    else
                    {
                        AlohaService.ServiceDataContracts.Dish dd = new AlohaService.ServiceDataContracts.Dish()
                        {
                            Barcode = Convert.ToInt32(SHD.prCode.Replace(".", "")),
                          //  Name = (SHD?.ru.Trim() == "") ? SHD.Name : SHD.ru,
                            //EnglishName = SHD.en,
                            SHId = SHD.Rid,
                            
                            IsActive = true,
                            IsTemporary = false,
                            NeedPrintInMenu = true,
                            IsToGo = toGo,

                        };
                        if (toGo)
                        {
                            dd.PriceForDelivery = SHD.Price / 10000;
                        }
                        else
                        {
                            dd.PriceForFlight = SHD.Price / 10000;
                            }
                        //var bc = 
                        if (SHD.ru == null)
                        {
                            dd.Name = SHD.Name;
                        }
                        else
                        {
                            dd.Name = SHD.ru;
                        }

                        if (SHD.en == null)
                        {
                            dd.EnglishName = "";
                        }
                        else
                        {
                            dd.EnglishName = SHD.en;
                        }


                        //if (dd.EnglishName == null) { dd.EnglishName = ""; }
                        //if (dd.Name == null) { dd.Name = ""; }
                        dd.LabelEnglishName = dd.EnglishName;
                        dd.LabelRussianName = dd.Name;
                      var res=  AlohaFly.DBProvider.Client.CreateDish(dd);
                        if (!res.Success)
                        {
                            s = res.ErrorMessage;
                            string ssD = $"{d.Rid};{d.numCode};{d.prCode};{d.Name};{d.ru};{d.en};{d.Price};{d.RidUnit};{d.Unit};{d.Parent}; ";
                            _logger.Error($"Error AlohaFly.DBProvider.Client.CreateDish D: {ssD} {Environment.NewLine} Error: {s}");
                        }
                        else
                        {
                            _logger.Debug($"AlohaFly.DBProvider.Client.CreateDish AddD: {dd.Barcode} {dd.Name} ");
                        }
                    }
                }
                catch(Exception e)
                {
                    
                    string ssD = $"{d.Rid};{d.numCode};{d.prCode};{d.Name};{d.ru};{d.en};{d.Price};{d.RidUnit};{d.Unit};{d.Parent}; ";
                    _logger.Error($"Error AddDishToSQL D: {ssD} {Environment.NewLine} Error:{e.Message}");
                    s = e.Message;
                }
            }


        }


        public List<TGoods> GetDishesHoz()
        {
            var dd = new List<TGoods>();
            //foreach (var grN in res)
            {
                TGoodsList r = null;
                try
                {
                    string merrMess;
                    int merrCode = 0;
                    // _logger.Debug($"GetGoods Id {grN} ");
                    r = sh.GetGoods(40, out merrCode, out merrMess);
                }
                catch (Exception e)
                {
                    // _logger.Error($"GetGoods Id {grN} Error "+e.Message);
                }
                if (r != null)
                {
                    dd.AddRange(r.ListGoods);
                }
            }
            return dd;
        }

            public List<TGoods> GetDishesFromSH(bool togo)
        {
            string errMess;
            int errCode = 0;
            var gr = sh.GetGoodsTree(out errCode, out errMess).ListGoodsTree;
            List<int> PGroups;
            //List<int> PGroups = new List<int> { 77, 164, 2417 };
            //List<int> PGroups = new List<int> { 77, 2417 };
            if (togo)
            {
                PGroups = new List<int> { 164 };
            }
            else
            {
                PGroups = new List<int> { 77, 2417 };
            }
            List<int> res = new List<int>();
            res.AddRange(PGroups);

            foreach (var id in PGroups)
            {
                var r = GetGroups(id, gr);
                if (r.Count > 0)
                {
                    res.AddRange(r);
                }
            }

            //var grs = GetGroups();
            var dd = new List<TGoods>();
            foreach (var grN in res)
            {
                TGoodsList r = null;
                try
                {
                    string merrMess;
                    int merrCode = 0;
                    // _logger.Debug($"GetGoods Id {grN} ");
                    r = sh.GetGoods(grN, out merrCode, out merrMess);
                }
                catch (Exception e)
                {
                    // _logger.Error($"GetGoods Id {grN} Error "+e.Message);
                }
                if (r != null)
                {
                    dd.AddRange(r.ListGoods);
                }
            }
            return dd;
        }

        public string PrintDishes()
        {
            string s = "";

            string errMess;
            int errCode = 0;
            var gr = sh.GetGoodsTree(out errCode, out errMess).ListGoodsTree;
            
            //List<int> PGroups = new List<int> { 77, 164, 2417 };
            List<int> PGroups = new List<int> { 77,  2417 };
            List<int> res = new List<int>();
            res.AddRange(PGroups);
            /*
            foreach (var id in PGroups)
            {
                var r = GetGroups(id, gr);
                if (r.Count > 0)
                {
                    res.AddRange(r);
                }
            }
            */
            //var grs = GetGroups();
            var dd = GetDishesFromSH(false);
                /*
            foreach (var grN in res)
            {
                TGoodsList r=null;
                try
                {
                   // _logger.Debug($"GetGoods Id {grN} ");
                    r = sh.GetGoods(grN);
                }
                catch(Exception e)
                {
                   // _logger.Error($"GetGoods Id {grN} Error "+e.Message);
                }
                if (r != null)
                {
                    dd.AddRange(r.ListGoods);
                }
            }
            */
            string ss = "";
            foreach (var d in dd)
            {
                ss += $"{d.Rid};{d.numCode};{d.prCode};{d.Name};{d.ru};{d.en};{d.Price};{d.RidUnit};{d.Unit};{d.Parent}" + Environment.NewLine;


            }
            _logger.Debug(ss);
                foreach (var t in gr)
            {
                if (res.Contains(t.Rid))
                {
                    s += $"{t.Rid} {t.Name} Parent: {t.Parent}" + Environment.NewLine;
                }
            }
            return s;
           
        }
        private List<int> GetGroups()
        {
            string errMess;
            int errCode = 0;
            var gr = sh.GetGoodsTree(out errCode, out errMess).ListGoodsTree;
            List<int> PGroups = new List<int> { 77, 164, 2417 };
            List<int> res = new List<int>();
            res.AddRange(PGroups);

            foreach (var id in PGroups)
            {
                var r = GetGroups(id, gr);
                if (r.Count > 0)
                {
                    res.AddRange(r);
                }
            }
            return res;
        }

        private List<int> GetGroups(int gId, List<TTreeItem> AllList)
        {
            //_logger.Debug($"GetGroups {gId}");
            if (AllList.Where(a => a.Parent == gId).Count() == 0) return new List<int>();
            try
            {
                List<int> res = AllList.Where(a => a.Parent == gId).Select(a => a.Rid).ToList();
                if (res == null) return new List<int>();
                List<int> res2 = new List<int>();
                res2.AddRange(res);
                foreach (var chGId in res)
                {
                    if (chGId == gId) continue;
                    //_logger.Debug($"Find {chGId}");
                    var r = GetGroups(chGId, AllList);
                    //_logger.Debug($"Find {chGId} Count {r.Count}");
                    if (r.Count > 0)
                    {
                        res2.AddRange(r);
                    }
                }
                return res2;
            }
            catch (Exception e)
            {
                _logger.Debug($"Error GetGroups {e.Message}");
                return new List<int>();
            }
        }
    }
}
