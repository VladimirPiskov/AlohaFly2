using AlohaService.ServiceDataContracts;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreHouseConnect;


namespace AlohaFly.SH
{

    public class SHDish
    { 
        public int Rid { set; get; }
        public string Name { set; get; }
    }

    public static class SHUtils
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        static TStoreHouse sh;

        private static Dictionary<int, TGoods> dishGroups = new Dictionary<int, TGoods>();

        public static void FillAllSubFolders()
        {
            var cats = sh.GetGoodsTree(out int errCode, out string errMsg);
            foreach (var gtr in cats.ListGoodsTree)
            {
                if (cats==null|| cats.ListGoodsTree == null) continue;
                try
                {
                    var itms = sh.GetGoods(gtr.Rid, out errCode, out errMsg);
                    if (itms == null || itms.ListGoods == null) continue;
                    foreach (var itm in itms.ListGoods)
                    {
                        if (!dishGroups.TryGetValue(itm.Rid, out TGoods g))
                        {
                            dishGroups.Add(itm.Rid, itm);
                        }
                    }
                }
                catch(Exception e)
                {
                    logger.Error($"Error FillAllSubFolders {e.Message}");
                }
             }

        }

        public static bool UpdateSHItems(List<Dish> items, out string ErrMesssage)
        {
            ErrMesssage = "";
            try
            {
                logger.Debug($"UpdateItems");
                if (items == null) return false;
                sh = SHWrapper.ConnectSH();
                if (sh == null)
                {
                    ErrMesssage = "Нет связи с SH";
                    logger.Error($"Error UpdateItems Error {ErrMesssage }");
                    return false;
                }
                FillAllSubFolders();

                //var groups = GetItemFoldersTreeDic();
                //var goods = GetDishesFromSH(groups.Keys.ToList());
                foreach (var itm in items)
                {
                    if (itm.SHId == 0) continue;

                    if (dishGroups.TryGetValue((int)itm.SHIdNewBase, out TGoods shItm))
                    {
                        string prefixDop = itm.DishLogicGroupId == MainClass.DopLogikCatId ? "dop_" : "";
                        
                        var price = itm.IsToGo ? itm.PriceForDelivery : itm.PriceForFlight;
                        if (price != shItm.Price/10000)
                        {
                            try
                            {
                                sh.CloseConection();
                                sh = SHWrapper.ConnectSH();


                                var res = sh.UpdateGoods((int)itm.SHIdNewBase, shItm.Parent, shItm.Name, "Al_" + prefixDop + itm.Barcode.ToString(), (int)itm.Barcode, (int)itm.Barcode, 1, 2, shItm.RidUnit, (double)price, out int ErrCode, out ErrMesssage);
                                if (!res)
                                {
                                    logger.Error($"Error sh.UpdateGoods Barcode:{itm.Barcode}; dName:{shItm.Name}; ErrCode: {ErrCode};  ErrMesssage: {ErrMesssage}");
                                }
                            }
                            catch (Exception e)
                            {
                                logger.Error($"Error UpdateGoods mess {e.Message}");
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                logger.Error($"Error UpdateItems mess {e.Message}");
                ErrMesssage = e.Message;
                return false;
            }
            finally
            {
                if (sh != null)
                {
                    try
                    {
                        sh.CloseConection();

                    }
                    catch (Exception ee) { logger.Error($"UpdateItems error CloseConection Mess: {ee.Message}"); }

                }
            }
        }

    }
}
