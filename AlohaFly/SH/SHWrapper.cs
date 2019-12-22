using AlohaService.Interfaces;
using AlohaService.ServiceDataContracts;
using NLog;
using StoreHouseConnect;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlohaFly.SH
{
    public static class SHWrapper
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        static TStoreHouse sh;

        static TStoreHouse ConnectSH()
        {
            try
            {
                var ip = Properties.Settings.Default.SHIP;
                var port = Properties.Settings.Default.SHPort;
                var login = Properties.Settings.Default.SHLogin;
                var pass = Properties.Settings.Default.SHPass;
                logger.Debug($"try connectSH {ip}:{port} login:{login}; pass:{pass}");
                string err = "";
                int errCode = 0;
                var conn = true;
                if (sh == null)
                {
                    sh = new TStoreHouse();

                }
                conn = sh.ConnectSH(ip, port, login, pass, out errCode, out err);


                logger.Debug($"ConnectSH {conn} err: {err}");
                if (conn) return sh;
                return null;
            }
            catch (Exception e)
            {
                logger.Debug($"SH Conect error {e.Message}");
                return null;
            }
        }

        /*
        public static void CreateSalesInvoice(OrderFlight order)
        {
            var t = new Task(

                () =>
                {
                    string ErrMesssage = "";
                    var CreateSHres = CreateSalesInvoiceSync(order, out ErrMesssage);
                    if (!CreateSHres)
                    {
                        UI.UIModify.ShowAlert($"{ErrMesssage + Environment.NewLine} Накладная будет создана при появлении связи со StoreHouse");
                        order.IsSHSent = false;
                        Models.AirOrdersModelSingleton.Instance.UpdateOrder(order);
                    }
                }

            );
            t.Start();
        }
        /*
        static int UnitId = 6;
        static int ToFlyFolderId = 5;
        static int SharFolderId = 6;
        static int ToGoFolderId = 7;
        */



        private static List<TGoods> GetDishesFromSH(List<int> groups)
        {
            string errMess;
            int errCode = 0;
            //var groups = sh.GetGoodsTree(out errCode, out errMess).ListGoodsTree;
            var tmp = new List<TGoods>();
            foreach (var grId in groups)
            {
                var res = sh.GetGoods(grId, out errCode, out errMess);
                if (res.ListGoods.Count > 0)
                {
                    tmp.AddRange(res.ListGoods);
                }
            }
            return tmp;
        }


        private static Dictionary<int, string> GetToFlyGoodsTreeDic()
        {
            string errMess;
            int errCode = 0;
            var res = new Dictionary<int, string>();
            var un = sh.GetGoodsTree(out errCode, out errMess);
            foreach (var u in un.ListGoodsTree)
            {
                if (u.Parent == Properties.Settings.Default.SHToFlyFolderId)
                {
                    logger.Debug($"{u.Rid} {u.Name.Trim()}");
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
                if ((u.Parent == Properties.Settings.Default.SHSharFolderId) || (u.Rid == Properties.Settings.Default.SHSharFolderId))
                {
                    res.Add(u.Rid, u.Name.Trim());
                }
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
                if (u.Parent == Properties.Settings.Default.SHToGoFolderId)
                {
                    res.Add(u.Rid, u.Name.Trim());
                }
            }
            return res;
        }


        const int ShMaxNameLenght = 40;
        private static bool AddNewDishToFly(Dish dish, out string ErrMesssage, out int catId)
        {
            logger.Debug($"AddNewDishToFly dish: {dish.Barcode} {dish.Name}; KithcenCat: {dish.DishKitсhenGroupId}; LogicCat: {dish.DishLogicGroupId}");
            catId = Properties.Settings.Default.SHNoCatFolderToFlyId;
            ErrMesssage = "";
            int errCode = 0;
            var toFlyGroups = GetToFlyGoodsTreeDic().Keys.ToList();
            toFlyGroups.AddRange(GetSVOGoodsTreeDic().Keys.ToList());
            var existD = GetDishesFromSH(toFlyGroups);

            //if (dish.IsTemporary)
            //{
            //    
            //    if (existD.Any(a => GetBarCode(a.prCode) == dish.Barcode && GetBarCode2OpenDish(a.prCode) == dish.Id))
            //    {

            //        dish.SHIdNewBase = existD.FirstOrDefault(a => GetBarCode(a.prCode) == dish.Barcode && GetBarCode2OpenDish(a.prCode) == dish.Id).Rid;
            //        //catId = -1;
            //        DBProvider.Client.UpdateDish(dish);
            //        return true;
            //    }
            //    /*
            //    if (DataExtension.DataCatalogsSingleton.Instance.Dishes.Any(a => a.Barcode == dish.Barcode && !a.IsTemporary))
            //    {
            //        dish.SHIdNewBase = DataExtension.DataCatalogsSingleton.Instance.Dishes.SingleOrDefault(a => a.Barcode == dish.Barcode && !a.IsTemporary).SHIdNewBase;
            //        return true;
            //    }
            //   */
            //}
            //else

            {

                if (existD.Any(a => GetBarCode(a.prCode, dish.IsDop()) == dish.Barcode || GetBarCode(a.prCode, dish.IsDop()) == dish.Barcode + 10000))
                {
                    dish.SHIdNewBase = existD.FirstOrDefault(a => GetBarCode(a.prCode, dish.IsDop()) == dish.Barcode || GetBarCode(a.prCode, dish.IsDop()) == dish.Barcode + 10000).Rid;
                    //catId = -1;
                    DBProvider.Client.UpdateDish(dish);
                    return true;
                }

            }
            try
            {
                bool res = false;
                if (dish.Name == null)
                {
                    logger.Error($"Error dish.Name == null Id: {dish.Id}");
                }

                string dName = dish.Name.Length > ShMaxNameLenght ? dish.Name.Substring(0, ShMaxNameLenght) : dish.Name;
                /*
                if (dish.IsTemporary)
                {
                    catId = Properties.Settings.Default.SHOpenDishFolderToFly;
                    res = sh.AddGoods(catId, dName + "_ToFly", "Al_Op_" + dish.Barcode.ToString() + "_" + dish.Id.ToString(), (int)dish.Barcode, (int)dish.Barcode, 1, 2, Properties.Settings.Default.SHUnitId, (double)dish.PriceForFlight, out errCode, out ErrMesssage);

                }
                
                else
                */
                {
                    string prefixDop = dish.DishLogicGroupId == MainClass.DopLogikCatId ? "dop_" : "";
                    if (dish.DishLogicGroupId == null || (int)dish.DishLogicGroupId != 1)
                    {

                        if (dish.DishKitсhenGroup != null && dish.DishKitсhenGroup.SHIdToFly != 0)
                        {
                            catId = (int)dish.DishKitсhenGroup.SHIdToFly;
                        }


                        res = sh.AddGoods(catId, dName + "_" + prefixDop + "ToFly", "Al_" + prefixDop + dish.Barcode.ToString(), (int)dish.Barcode, (int)dish.Barcode, 1, 2, Properties.Settings.Default.SHUnitId, (double)dish.PriceForFlight, out errCode, out ErrMesssage);
                    }
                    else
                    {
                        catId = Properties.Settings.Default.SHSharFolderId;
                        res = sh.AddGoods(catId, dName + "_ToFly", "Al_" + prefixDop + dish.Barcode.ToString(), (int)dish.Barcode, (int)dish.Barcode, 1, 2, Properties.Settings.Default.SHUnitId, (double)dish.PriceForFlight, out errCode, out ErrMesssage);
                    }
                }

                if (res)
                {
                    int shId = 0;
                    /*
                    if (dish.IsTemporary)
                    {
                        shId = GetOpenDishShId(catId, (int)dish.Barcode, (int)dish.Id);
                    }
                    else
                    */
                    {
                        shId = GetDishShId(catId, (int)dish.Barcode, dish.IsDop());
                    }
                    if (shId != 0)
                    {
                        logger.Debug($"shId = {shId}");
                        dish.SHIdNewBase = shId;
                        DBProvider.Client.UpdateDish(dish);
                        return true;
                    }
                    else
                    {
                        logger.Debug($"TExpenceDocument create Shid=0 return false");
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
            catch (Exception e)
            {
                logger.Error($"AddNewDishToFly error: " + e.Message);
                return false;
            }

            finally
            {
                logger.Debug($"AddNewDishToFly end. Errcode {errCode}; ErrMesssage: {ErrMesssage}");
            }
        }


        private static bool AddNewDishToGo(Dish dish, out string ErrMesssage, out int catId)
        {

            logger.Debug($"AddNewDishToGo dish: {dish.Barcode} {dish.Name}; KithcenCat: {dish.DishKitсhenGroupId}; LogicCat: {dish.DishLogicGroupId}");
            catId = Properties.Settings.Default.SHNoCatFolderToGoId;
            ErrMesssage = "";
            int errCode = 0;
            try
            {

                var toGoGroups = GetToGoGoodsTreeDic().Keys.ToList();

                var existD = GetDishesFromSH(toGoGroups);

                /*
                if (dish.IsTemporary)
                {
                    if (existD.Any(a => GetBarCode(a.prCode) == dish.Barcode && GetBarCode2OpenDish(a.prCode) == dish.Id))
                    {

                        dish.SHIdNewBase = existD.FirstOrDefault(a => GetBarCode(a.prCode) == dish.Barcode && GetBarCode2OpenDish(a.prCode) == dish.Id).Rid;
                        //catId = -1;
                        DBProvider.Client.UpdateDish(dish);
                        return true;
                    }
                }
                else
                */
                {

                    if (existD.Any(a => GetBarCode(a.prCode, dish.IsDop()) == dish.Barcode || GetBarCode(a.prCode, dish.IsDop()) == dish.Barcode + 10000))
                    {
                        dish.SHIdNewBase = existD.FirstOrDefault(a => GetBarCode(a.prCode, dish.IsDop()) == dish.Barcode || GetBarCode(a.prCode, dish.IsDop()) == dish.Barcode + 10000).Rid;
                        //catId = -1;
                        DBProvider.Client.UpdateDish(dish);
                        return true;
                    }


                }

                string dName = dish.Name.Length > ShMaxNameLenght ? dish.Name.Substring(0, ShMaxNameLenght) : dish.Name;

                var res = false;
                /*
                if (dish.IsTemporary)
                {
                    catId = Properties.Settings.Default.SHOpenDishFolderToGo;
                    res = sh.AddGoods(catId, dName + "_ToGo", "Al_Op_" + dish.Barcode.ToString() + "_" + dish.Id.ToString(), (int)dish.Barcode, (int)dish.Barcode, 1, 2, Properties.Settings.Default.SHUnitId, (double)dish.PriceForDelivery, out errCode, out ErrMesssage);
                }
                else
                */
                {
                    string prefixDop = dish.DishLogicGroupId == MainClass.DopLogikCatId ? "dop_" : "";
                    if (dish.DishKitсhenGroup != null && dish.DishKitсhenGroup.SHIdToGo != 0)
                    {
                        catId = (int)dish.DishKitсhenGroup.SHIdToGo;
                    }
                    res = sh.AddGoods(catId, dName + "_" + prefixDop + "ToGo", "Al_" + prefixDop + dish.Barcode.ToString(), (int)dish.Barcode, (int)dish.Barcode, 1, 2, Properties.Settings.Default.SHUnitId, (double)dish.PriceForDelivery, out errCode, out ErrMesssage);
                }
                if (res)
                {
                    int shId = 0;
                    /*
                    if (dish.IsTemporary)
                    {
                        shId = GetOpenDishShId(catId, (int)dish.Barcode, (int)dish.Id);
                    }
                    else
                    */
                    {
                        shId = GetDishShId(catId, (int)dish.Barcode, dish.IsDop());
                    }
                    if (shId != 0)
                    {
                        logger.Debug($"shId = {shId}");
                        dish.SHIdNewBase = shId;
                        DBProvider.Client.UpdateDish(dish);
                        return true;
                    }
                    else
                    {
                        logger.Debug($"TExpenceDocument create Shid=0 return false");
                        return false;
                    }
                }
                else
                {
                    return false;
                }


            }
            catch (Exception e)
            {
                logger.Error($"AddNewDishToGo error: " + e.Message);
                return false;
            }

            finally
            {
                logger.Debug($"AddNewDishToGo end. Errcode {errCode}; ErrMesssage: {ErrMesssage}");
            }


            /*
            int errCode = 0;
            string dName = dish.Name.Length > ShMaxNameLenght ? dish.Name.Substring(0, ShMaxNameLenght) : dish.Name;
            return sh.AddGoods((int)dish.DishKitсhenGroup.SHIdToGo, dName + "_ToGo", "Al_" + dish.Barcode.ToString(), (int)dish.Barcode, (int)dish.Barcode, 1, 2, Properties.Settings.Default.SHUnitId, (double)dish.PriceForDelivery, out errCode, out ErrMesssage);
            */
        }

        private static int GetOpenDishShId(int catId, int barcode, int dId)
        {
            string errMess;
            int errCode = 0;
            int id = 0;
            logger.Debug($"GetDishShId barcode: {barcode} catId: {catId}");
            try
            {
                var res = sh.GetGoods(catId, out errCode, out errMess);
                if (res != null)
                {
                    id = res.ListGoods.SingleOrDefault(a => GetBarCode(a.prCode) == barcode && GetBarCode2OpenDish(a.prCode) == dId).Rid;
                }
            }
            catch (Exception e)
            {
                logger.Error($"GetDishShId error: " + e.Message);
            }

            return id;
        }

        private static int GetDishShId(int catId, int barcode, bool isDop = false)
        {
            string errMess;
            int errCode = 0;
            int id = 0;
            logger.Debug($"GetDishShId barcode: {barcode} catId: {catId}");
            try
            {
                var res = sh.GetGoods(catId, out errCode, out errMess);
                if (res != null)
                {
                    id = res.ListGoods.SingleOrDefault(a => GetBarCode(a.prCode, isDop) == barcode).Rid;
                }
            }
            catch (Exception e)
            {
                logger.Error($"GetDishShId error: " + e.Message);
            }

            return id;
        }

        private static int GetBarCode2OpenDish(string shBarCode)
        {
            if (shBarCode == null) return 0;
            try
            {
                if (shBarCode.Contains("Al_"))
                {
                    if (shBarCode.Contains("Al_Op_"))
                    {
                        shBarCode = shBarCode.Split('_')[3];
                    }

                    else
                    {
                        return 0;
                    }
                }

                return Convert.ToInt32(shBarCode);
            }
            catch
            {
                return 0;
            }
        }
        private static int GetBarCode(string shBarCode, bool dop = false)
        {
            if (shBarCode == null) return 0;
            try
            {
                if (dop)
                {
                    if (shBarCode.Contains("Al_dop_"))
                    {
                        shBarCode = shBarCode.Substring(7);
                    }
                    else
                    {
                        return 0;
                    }
                }

                if (shBarCode.Contains("Al_"))
                {


                    if (shBarCode.Contains("Al_Op_"))
                    {
                        // shBarCode = shBarCode.Split('_')[2];
                        return 0;
                    }
                    else
                    {
                        shBarCode = shBarCode.Substring(3);
                    }

                }

                return Convert.ToInt32(shBarCode);
            }
            catch
            {
                return 0;
            }
        }

        private static bool AddAirCompany(AirCompany airc, out string ErrMesssage)
        {
            try
            {
                logger.Debug($"AddAirCompany Order {airc.Name}");
                ErrMesssage = "";

                var shExpCats = sh.ExpCtgs(out int errCode, out string errMess);
                if (!shExpCats.ListExpCtgs.Select(a => a.Name).Contains(airc.Name.Trim()))
                {
                    bool res = sh.AddExpCtgs(airc.Name.Trim(), out errCode, out errMess);
                    if (!res)
                    {
                        ErrMesssage = "Не могу добавить авиакомпанию" + Environment.NewLine + errMess + Environment.NewLine;
                        return false;

                    }
                    shExpCats = sh.ExpCtgs(out errCode, out errMess);
                }
                if (shExpCats.ListExpCtgs.Any(a => a.Name == airc.Name.Trim()))
                {
                    airc.SHId = shExpCats.ListExpCtgs.FirstOrDefault(a => a.Name == airc.Name.Trim()).Rid;
                    DBProvider.Client.UpdateAirCompany(airc);
                    logger.Debug($"AddAirCompany ok {airc.Name}");
                    return true;

                }
                else
                {
                    ErrMesssage = "Не могу добавить авиакомпанию" + Environment.NewLine;
                    return false;
                }
            }
            catch (Exception e)
            {
                logger.Error($"AddAirCompany error {airc.Name} " + e.Message);
                ErrMesssage = "Ошибка при добавлении авиакомпании" + Environment.NewLine + e.Message + Environment.NewLine;
                return false;
            }
        }

        private static bool AddToGoPayment(Payment p, out string ErrMesssage)
        {
            try
            {
                logger.Debug($"AddToGoPayment Order {p.Name}");
                ErrMesssage = "";

                var shExpCats = sh.ExpCtgs(out int errCode, out string errMess);
                if (!shExpCats.ListExpCtgs.Select(a => a.Name).Contains(p.Name.Trim()))
                {
                    bool res = sh.AddExpCtgs(p.Name.Trim(), out errCode, out errMess);
                    if (!res)
                    {
                        ErrMesssage = "Не могу добавить платеж" + Environment.NewLine + errMess + Environment.NewLine;
                        return false;

                    }
                    shExpCats = sh.ExpCtgs(out errCode, out errMess);
                }
                if (shExpCats.ListExpCtgs.Any(a => a.Name == p.Name.Trim()))
                {
                    p.SHId = shExpCats.ListExpCtgs.FirstOrDefault(a => a.Name == p.Name.Trim()).Rid;
                    DBProvider.Client.UpdatePayment(p);
                    logger.Debug($"AddToGoPayment ok {p.Name}");
                    return true;

                }
                else
                {
                    ErrMesssage = "Не могу добавить платеж" + Environment.NewLine;
                    return false;
                }
            }
            catch (Exception e)
            {
                logger.Error($"AddAirCompany error {p.Name} " + e.Message);
                ErrMesssage = "Ошибка при добавлении платежа" + Environment.NewLine + e.Message + Environment.NewLine;
                return false;
            }
        }



        //public static bool CreateSalesInvoiceSync(OrderFlight order, out string ErrMesssage)
        //{

        //    ErrMesssage = "";
        //    if (order == null) return false;
        //    if (order.OrderStatus == OrderStatus.Cancelled || order.OrderStatus==OrderStatus.InWork)
        //    {
        //        return DeleteSalesInvoice(order);
        //    }



        //    TStoreHouse sh = null;
        //    order.IsSHSent = false;
        //    try
        //    {
        //        logger.Debug($"CreateSalesInvoice Order {order.Id}");
        //        sh = ConnectSH();
        //        if (sh == null) return false;

        //        if (order.AirCompany.SHId == 0)
        //        {
        //            var addAirRes = AddAirCompany(order.AirCompany, out ErrMesssage);
        //            if (!addAirRes)
        //            {
        //                return false;
        //            }
        //        }


        //        TExpenceDocument d = new TExpenceDocument()
        //        {
        //            Prefix = Properties.Settings.Default.SHDocPrefix,
        //            DocNum = (int)order.Id,
        //            RidPlace = Properties.Settings.Default.SHRidPlace,
        //            CatExpence = (int)order.AirCompany.SHId,
        //            Date = order.DeliveryDate,
        //            Coment = $"{order.DeliveryPlace?.Name} {order.FlightNumber} {order.DeliveryDate.ToString("HH:mm")} ",
        //            ListItemDocument = new List<TItemDocument>()
        //        };

        //        if (DBProvider.SharAirs.Contains(order.AirCompany.Id))
        //        {
        //            d.RidPlace = Properties.Settings.Default.SHRidPlaceSVO;
        //            d.Coment = "SVO" + d.Coment;
        //        }

        //        if (order.ContactPerson != null) { d.Coment += order.ContactPerson.FullSearchData; }
        //        logger.Debug($"TExpenceDocument create Prefix: {d.Prefix}; DocNum:{d.DocNum}; RidPlace:{d.RidPlace}; CatExpence:{d.CatExpence}; Date:{d.Date}; Coment:{d.Coment};");
        //        if (order.DishPackages != null)
        //        {
        //            decimal DPSumm = 0;
        //            foreach (var dp in order.DishPackages)
        //            {

        //                if (dp.Dish.SHIdNewBase == 0)
        //                {
        //                    logger.Debug($"dp.Dish.SHIdNewBase == 0");
        //                    int catId = 0;
        //                    if (!AddNewDishToFly(dp.Dish, out ErrMesssage, out catId))
        //                    {
        //                        logger.Debug($"TExpenceDocument create AddNewDishToFly error return false");
        //                        return false;
        //                    }
        //                }

        //                decimal discDownPecent = (order.OrderSumm == 0) ? 0 : order.OrderTotalSumm / order.OrderSumm;
        //                if (order.DiscountSumm == 0) { discDownPecent = 1; }
        //                decimal dSumm = dp.TotalPrice * (discDownPecent) * dp.Amount;
        //                if (dp == order.DishPackages.Last())
        //                {
        //                    dSumm = (order.OrderTotalSumm - DPSumm);
        //                }
        //                else
        //                {
        //                    DPSumm += dSumm;
        //                }
        //                TItemDocument itm = new TItemDocument()
        //                {
        //                    Quantity = (double)dp.Amount,
        //                    Rid = (int)dp.Dish.SHIdNewBase,
        //                    Price = (double)dSumm//* 1000
        //                };
        //                logger.Debug($"TExpenceDocument create itm {dp.DishName}; Price:  {itm.Price}; Quantity: {itm.Quantity}; Rid:{itm.Rid} ");
        //                d.ListItemDocument.Add(itm);
        //            }
        //        }
        //        d.Type = 1;
        //        string err = "";
        //        int errCode = 0;
        //        bool res = sh.ExpenceDocumentCreate(d, out errCode, out ErrMesssage);



        //        if (res)
        //        {
        //            logger.Debug($"CreateSalesToFlyInvoice Ok");

        //        }
        //        else
        //        {
        //            logger.Debug($"CreateSalesToFlyInvoice error Number {order.Id}  err: {ErrMesssage}");
        //        }

        //        order.IsSHSent = res;
        //        return res;

        //    }
        //    catch (Exception e)
        //    {
        //        ErrMesssage = $"Ошибка создания расходной накладной {Environment.NewLine + e.Message}";
        //        logger.Error($"CreateSalesInvoice Oreder {order.Id}. Mess: {e.Message}");
        //        return false;
        //    }
        //    finally
        //    {

        //        if (sh != null)
        //        {
        //            try
        //            {
        //                sh.CloseConection();

        //            }
        //            catch (Exception ee) { logger.Error($"CreateSalesInvoice error CloseConection Mess: {ee.Message}"); }

        //        }

        //    }
        //}

            /*

        public static bool ShSendCheck(IOrderLabel order, out string ErrMesssage)
        {
            ErrMesssage = "";
            var prefix = Properties.Settings.Default.SHDocPrefix + DBProvider.TestStr + (order is OrderToGo ? "ToGo" : "");
            var prefixAnn = prefix + "_Annul";
            TExpenceDocument d = new TExpenceDocument()
            {
                Prefix = prefix,
                DocNum = (int)order.Id,
                Date = order.DeliveryDate,
                Type = 1
            };
            TStoreHouse sh = null;
            sh = ConnectSH();
            if (sh == null) return false;
            //sh.ExpenceDocumentCheck(d,)
                bool res = sh.ExpenceDocumentCheck(d, out int errCode, out string err);
            logger.Debug($"DeleteSalesInvoice {res}; err: {err}");
            if (errCode == -100)
            {
                logger.Debug($"(errCode == -100) ok;");
                res = true;
            }
        }
        */

            public static bool CreateSalesInvoiceSync(IOrderLabel order, out string ErrMesssage)
        {
            ErrMesssage = "";

            if (order == null) return false;

            try
            {


                DeleteSalesInvoice(order);
                if (order.OrderStatus == OrderStatus.Cancelled)
                {
                    order.IsSHSent = true;
                }
                if (order.OrderStatus == OrderStatus.Cancelled || order.OrderStatus == OrderStatus.InWork)
                {
                    return true;
                }


                order.IsSHSent = false;

                TStoreHouse sh = null;
                sh = ConnectSH();
                if (sh == null) return false;



                foreach (var dp in order.DishPackagesForLab.Where(a => a.Dish.SHIdNewBase == 0))
                {
                    if (order is OrderFlight)
                    {
                        if (!AddNewDishToFly(dp.Dish, out ErrMesssage, out int catId))
                        {
                            logger.Debug($"TExpenceDocument create AddNewDishToFly error return false");
                            return false;
                        }
                    }
                    else
                    {
                        if (!AddNewDishToGo(dp.Dish, out ErrMesssage, out int catId))
                        {
                            logger.Debug($"TExpenceDocument create AddNewDishToFly error return false");
                            return false;
                        }
                    }


                }
                var prefix = Properties.Settings.Default.SHDocPrefix + DBProvider.TestStr;
                var docNum = (int)order.Id;
                var ridPlace = 0;
                var comment = "";
                var catExp = 0;
                if (order is OrderFlight orderFlight)
                {
                    //prefix += "ToFly";
                    ridPlace = Properties.Settings.Default.SHRidPlace;
                    comment = $"{orderFlight.DeliveryPlace?.Name} {orderFlight.FlightNumber} {orderFlight.DeliveryDate.ToString("HH:mm")} ";
                    catExp = (int)orderFlight.AirCompany.SHId;
                    if (DBProvider.SharAirs.Contains(orderFlight.AirCompany.Id))
                    {
                        ridPlace = Properties.Settings.Default.SHRidPlaceSVO;
                        comment = "SVO " + comment;
                    }

                    if (orderFlight.AirCompany.SHId == 0)
                    {
                        var addAirRes = AddAirCompany(orderFlight.AirCompany, out ErrMesssage);
                        if (!addAirRes)
                        {
                            return false;
                        }
                    }

                }
                else if (order is OrderToGo orderToGo)
                {

                    if (orderToGo.PaymentType != null && orderToGo.PaymentType.SHId == 0)
                    {
                        var addPRes = AddToGoPayment(orderToGo.PaymentType, out ErrMesssage);
                        if (!addPRes)
                        {
                            return false;
                        }

                    }

                    prefix += "ToGo";
                    ridPlace = Properties.Settings.Default.SHRidPlaceToGo;
                    catExp = (int)(orderToGo.PaymentType?.SHId ?? Properties.Settings.Default.SHToGoNotPaymentCatId); //: (int)orderToGo.PaymentType.SHId
                    comment = orderToGo.OrderComment == null ? "" : orderToGo.OrderComment + (orderToGo.CommentKitchen ?? "");
                }
                bool res = true;

                if (order.DishPackagesNoSpis.Any())
                {
                    res &= CreateSalesInvoiceSyncSale(order, order.DishPackagesNoSpis.ToList(), docNum, catExp, ridPlace, prefix, comment, out string ErrMesssage2);
                    ErrMesssage += ErrMesssage2 + Environment.NewLine;
                }
                if (order.DishPackagesSpis.Where(a => a.DeletedStatus == 1).Any())
                {
                    foreach (var p in order.DishPackagesSpis.Where(a => a.DeletedStatus == 1).Select(x => x.SpisPaymentId).Distinct())
                    {
                        prefix += "_Annul";
                        catExp = (int)DataExtension.DataCatalogsSingleton.Instance.GetPayment(p)?.SHId;

                        res &= CreateSalesInvoiceSyncSale(order, order.DishPackagesSpis.Where(x => x.SpisPaymentId == p).ToList(), docNum, catExp, ridPlace, prefix, comment, out string ErrMesg);
                        ErrMesssage += ErrMesg + Environment.NewLine;
                    }
                }

                order.IsSHSent = res;
                return res;
            }
            catch (Exception e)
            {
                ErrMesssage = $"Ошибка создания расходной накладной {Environment.NewLine + e.Message}";
                logger.Error($"CreateSalesInvoice Oreder {order.Id}. Mess: {e.Message}");
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
                    catch (Exception ee) { logger.Error($"CreateSalesInvoice error CloseConection Mess: {ee.Message}"); }

                }
            }
        }


        public static bool CreateSalesInvoiceSyncSale(IOrderLabel order, List<IDishPackageLabel> dishes, int docNum, int catExp, int ridPlace, string prefix, string comment, out string ErrMesssage)
        {

            try
            {
                ErrMesssage = "";
                //TStoreHouse sh = null;
                logger.Debug($"CreateSalesInvoice Order {order.Id}");
                //sh = ConnectSH();
                //if (sh == null) return false;
                TExpenceDocument d = new TExpenceDocument()
                {
                    Prefix = prefix,
                    DocNum = docNum,
                    RidPlace = ridPlace,
                    CatExpence = catExp,
                    Date = order.DeliveryDate,
                    Coment = comment,
                    ListItemDocument = new List<TItemDocument>()
                };


                logger.Debug($"TExpenceDocument create Prefix: {d.Prefix}; DocNum:{d.DocNum}; RidPlace:{d.RidPlace}; CatExpence:{d.CatExpence}; Date:{d.Date}; Coment:{d.Coment};");
                if (dishes != null)
                {

                    decimal DPSumm = 0;
                    foreach (var dp in dishes.Where(a => a.Amount != 0))
                    {


                        decimal dSumm = dp.TotalPrice * (1 - order.DiscountPercent / 100) * dp.Amount;
                        if (!dp.Deleted)
                        {
                            if (dp == dishes.Last())
                            {
                                dSumm = (order.OrderDishesSumm * (1 - order.DiscountPercent / 100) - DPSumm);
                            }
                            else
                            {
                                DPSumm += dSumm;
                            }
                        }
                        TItemDocument itm = new TItemDocument()
                        {
                            Quantity = (double)dp.Amount,
                            Rid = (int)dp.Dish.SHIdNewBase,
                            Price = (double)dSumm//* 1000
                        };
                        logger.Debug($"TExpenceDocument create itm {dp.Dish.Name}; Price:  {itm.Price}; Quantity: {itm.Quantity}; Rid:{itm.Rid} ");
                        d.ListItemDocument.Add(itm);
                    }
                    if (!dishes.FirstOrDefault().Deleted)
                    {
                        if (order is OrderToGo orderToGo)
                        {
                            if (orderToGo.DeliveryPrice > 0)
                            {
                                TItemDocument itm = new TItemDocument()
                                {
                                    Quantity = 1,
                                    Rid = 3632, // Доставка
                                    Price = (double)orderToGo.DeliveryPrice
                                };
                                logger.Debug($"TExpenceDocument create itm delev Quantity: {itm.Quantity}; Rid:{itm.Rid}");
                                d.ListItemDocument.Add(itm);
                            }
                        }

                        if (order is OrderFlight orderFlight)
                        {

                            if (orderFlight.ExtraChargeSumm > 0)
                            {
                                TItemDocument itm = new TItemDocument()
                                {
                                    Quantity = 1,
                                    Rid = 3631, // Надбавка
                                    Price = (double)orderFlight.ExtraChargeSumm
                                };
                                logger.Debug($"TExpenceDocument create ExtraChargeSumm Quantity: {itm.Quantity}; Rid:{itm.Rid}");
                                d.ListItemDocument.Add(itm);
                            }
                        }
                    }

                }
                d.Type = 1;
                string err = "";
                int errCode = 0;
                bool res = sh.ExpenceDocumentCreate(d, out errCode, out err);
                //bool res = err.ToLower() == "ok";
                if (res)
                {
                    logger.Debug($"CreateSalesToGoInvoice Ok");

                }
                else
                {
                    logger.Debug($"CreateSalesToGoInvoice error Number {order.Id}  err: {err}");
                }

                return res;

            }
            catch (Exception e)
            {
                ErrMesssage = $"Ошибка создания расходной накладной {Environment.NewLine + e.Message}";
                logger.Error($"CreateSalesInvoice Oreder {order.Id}. Mess: {e.Message}");
                return false;
            }
            finally
            {
                /*
                if (sh != null)
                {
                    try
                    {
                        sh.CloseConection();

                    }
                    catch (Exception ee) { logger.Error($"CreateSalesInvoice error CloseConection Mess: {ee.Message}"); }

                }
                */
            }
        }

        public static bool DeleteSalesInvoice(IOrderLabel order)
        {
            if (order == null) return false;
            TStoreHouse sh = null;
            order.IsSHSent = false;
            try
            {
                logger.Debug($"DeleteSalesInvoice Oreder {order.Id}");
                sh = ConnectSH();
                if (sh == null) return false;



                var prefix = Properties.Settings.Default.SHDocPrefix + DBProvider.TestStr + (order is OrderToGo ? "ToGo" : "");
                var prefixAnn = prefix + "_Annul";

                TExpenceDocument d = new TExpenceDocument()
                {
                    Prefix = prefixAnn,
                    DocNum = (int)order.Id,
                    Date = order.DeliveryDate,
                    Type = 1
                };


                logger.Debug($"sh.ExpenceDocumentDelete anul DocNum: {d.DocNum}; Prefix: {d.Prefix}; Date: {d.Date}");
                sh.ExpenceDocumentDelete(d, out int errCodet, out string errt);
                d.Prefix = prefix;


                logger.Debug($"sh.ExpenceDocumentDelete DocNum: {d.DocNum}; Prefix: {d.Prefix}; Date: {d.Date}");
                bool res = sh.ExpenceDocumentDelete(d, out int errCode, out string err);
                logger.Debug($"DeleteSalesInvoice {res}; err: {err}");
                if (errCode == -100)
                {
                    logger.Debug($"(errCode == -100) ok;");
                    res = true;
                }
                order.IsSHSent = res;
                return res;
            }
            catch (Exception e)
            {

                logger.Error($"DeleteSalesInvoice error. Mess: {e.Message}");
                return false;
            }
            finally
            {
                if (sh != null)
                {
                    try { sh.CloseConection(); } catch { }

                }
            }
        }

        /*
        public static bool DeleteSalesInvoice(OrderToGo order)
        {
            if (order == null) return false;
            TStoreHouse sh = null;
            order.IsSHSent = false;
            try
            {
                logger.Debug($"DeleteSalesInvoice Oreder {order.Id}");
                sh = ConnectSH();
                if (sh == null) return false;
                TExpenceDocument d = new TExpenceDocument()
                {
                    Prefix = Properties.Settings.Default.SHDocPrefix+DBProvider.TestStr + "ToGo",
                    DocNum = (int)order.Id,
                    Date = order.DeliveryDate,
                    Type = 1
                };
                string err = "";
                int errCode = 0;
                bool res = sh.ExpenceDocumentDelete(d, out errCode, out err);
                logger.Debug($"DeleteSalesInvoice {res}; err: {err}");
                if (errCode == -100)
                {
                    logger.Debug($"(errCode == -100) ok;");
                    res = true;
                }
                order.IsSHSent = res;
                return res;
            }
            catch (Exception e)
            {

                logger.Error($"DeleteSalesInvoice error. Mess: {e.Message}");
                return false;
            }
            finally
            {
                if (sh != null)
                {
                    try { sh.CloseConection(); } catch { }

                }
            }
        }
        */
    }
}
