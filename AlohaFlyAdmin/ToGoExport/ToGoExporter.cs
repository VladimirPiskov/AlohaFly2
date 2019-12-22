using AlohaService.ServiceDataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace AlohaFlyAdmin.ToGoExport
{
    class ToGoExporter
    {
        static Logger _logger = LogManager.GetCurrentClassLogger();
        static DBToGoDataContext db
        {
            get
            {
                //string constr = @"Data Source=s2010;Initial Catalog=togo;User ID=v.piskov;Password=Eit160t";
                string constr = @"Data Source=zeus;Initial Catalog=G2GoDelivery;User ID=Aloha;Password=WMZjjJyJHeZD";
                return new DBToGoDataContext(constr);
            }

        }

        public static void RunExport()
        {
            //ExportDishes2();
            //DeleteNonActiveDishes();
            //ExportCustomers();
            ExportCustomersPhones();
            ExportOrders();
           
        }


        static OrderStatus GetStatus(int St)
        {
            if (St == 1)
            {
                return OrderStatus.InWork;
            }
            if (St == 2)
            {
                return OrderStatus.Sent;
            }
            if (St == 3)
            {
                return OrderStatus.Cancelled;
            }
            return OrderStatus.InWork;
        }

        static int GetOpenId(string Name)
        {
            if (Name.Trim() == "Надя Парасинина")
            {
                return 1;
            }
            if (Name.Trim() == "Саша Авдонин")
            {
                return 4;
            }
            if (Name.Trim() == "Руссу Игорь")
            {
                return 3;
            }
            if (Name.Trim() == "Саша Процюк")
            {
                return 5;
            }
            if (Name.Trim() == "Аня Жгун")
            {
                return 6;
            }



            return 1;
        }

        static List<Driver> drivers;
        static long GetDriverId(string Name)
        {
            if (drivers == null)
            {
                drivers = AlohaFly.DBProvider.Client.GetDriverList().Result.ToList();
            }
            if (!drivers.Any(a => a.FullName == Name))
            {
                Driver d = new Driver()
                {
                    FullName = Name,
                    IsActive = true,
                    Phone = ""
                };
                AlohaFly.DBProvider.Client.CreateDriver(d);
                drivers = AlohaFly.DBProvider.Client.GetDriverList().Result.ToList();
            }
            return drivers.FirstOrDefault(a => a.FullName == Name).Id;
        }

        static void ExportOrders()
        {
            _logger.Debug("ExportOrders " );
            DateTime dtstart = new DateTime(2019, 01, 31);
            var existsD = AlohaFly.DBProvider.Client.GetDishList().Result.Where(a => a.IsToGo);
            
            var existsOrders = AlohaFly.DBProvider.Client.GetOrderToGoList(new OrderToGoFilter()
            {
                DeliveryDateStart = dtstart.AddDays(-5),
                DeliveryDateEnd = DateTime.Now.AddDays(5)
            }, new PageInfo()
            {
                Skip = 0,
                Take = 1000
            }
               ).Result.Select(a => a.OldId).ToList();

            var custs = AlohaFly.DBProvider.Client.GetOrderCustomerList().Result;
            var oldOrders = db.Orders.Where(a => a.DateDelivery >= dtstart && !existsOrders.Contains(a.ID));

            List<OrderCustomerAddress> addresses = new List<OrderCustomerAddress>();
            foreach (var addrList in custs.Select(a => a.Addresses))
            {
                addresses.AddRange(addrList);


            }
            foreach (var ord in oldOrders)
            {
                try
                {
                    _logger.Debug("ToGoOrder " + ord.ID);
                    OrderToGo ordNew = new OrderToGo();
                    ordNew.OldId = ord.ID;
                    ordNew.AddressId = addresses.Single(a => a.OldId == ord.AddressID).Id;
                    ordNew.PhoneNumber = db.Tel.Single(a => a.ID == ord.TelID).Number;
                  //  ordNew.Comment = ord.CommentCourier;
                    ordNew.CommentKitchen = ord.CommentKitchen;
                    ordNew.CreatedById = GetOpenId(ord.Operator);
                    _logger.Debug("ordNew.CreatedById " );
                    ordNew.CreationDate = ord.CreateDate + ord.CreateTime;
                    ordNew.DeliveryDate = ord.DateDelivery.GetValueOrDefault();// + ord.TimeDelivery.GetValueOrDefault();
                    ordNew.DiscountPercent = 0;
                    ordNew.IsSHSent = false;
                    ordNew.MarketingChannelId = ord.SaleChannelID;
                    ordNew.ReadyTime = ord.DateReady.GetValueOrDefault();//+ord.TimeReady
                    _logger.Debug("ordNew.ReadyTime");
                    ordNew.OrderCustomerId = custs.Single(a => a.OldId == ord.ClientID).Id;
                    _logger.Debug("ordNew.OrderCustomerId ");
                    ordNew.DriverId = GetDriverId(ord.Courier);
                    _logger.Debug("ordNew.DriverId ");
                    ordNew.OrderStatus = GetStatus(ord.Status.GetValueOrDefault());
                    _logger.Debug("ordNew.OrderStatus ");
                    ordNew.Summ = ord.Summ.GetValueOrDefault();
                    _logger.Debug("ordNew.Summ ");
                    ordNew.DeliveryPrice = ord.DeliveryPrice.GetValueOrDefault();
                    var packs = new List<DishPackageToGoOrder>();
                    foreach (var d in db.OrderContent.Where(a => a.OrderID == ord.ID))
                    {
                        _logger.Debug("d.Name; 0");
                        var nd = new DishPackageToGoOrder();
                        nd.DishName = d.Name;
                        nd.Amount = d.Amount.GetValueOrDefault();
                        nd.Comment = d.WarmUP;
                        nd.TotalPrice = d.Price.GetValueOrDefault();
                        nd.Code = d.Code;
                        if (existsD.Any(a => a.Barcode == d.Code))
                        {
                            nd.DishId = existsD.FirstOrDefault(a => a.Barcode == d.Code).Id;
                        }
                        else
                        {
                            nd.DishId = existsD.FirstOrDefault(a => a.Barcode == 17).Id;
                        }
                        _logger.Debug("d.Name; 1");
                        packs.Add(nd);



                    }

                    var res = AlohaFly.DBProvider.Client.CreateOrderToGo(ordNew);
                    if (!res.Success)
                    {
                        _logger.Debug($"ExportOrders error ");
                    }

                    //  var id = res.CreatedObjectId;

                    foreach (var d in packs)
                    {

                        d.OrderToGoId = res.CreatedObjectId;
                        OperationResult res2 = AlohaFly.DBProvider.Client.CreateDishPackageToGoOrder(d);
                        d.Id = res2.CreatedObjectId;
                    }

                    //ordNew.
                }
                catch(Exception e)
                {
                    _logger.Debug("Error ToGoOrder " + e.Message);
                }
            }

        }



        static void DeleteNonActiveDishes()
        {
            _logger.Debug($"DeleteNonActiveDishes ");
            var existsD = AlohaFly.DBProvider.Client.GetDishList().Result.Where(a => a.IsToGo);
            var dItems = db.Menu.Where(a => a.Active);
            //var clsList = new List<OrderCustomer>();
            List<long> actCodes = new List<long>();
            foreach (var d in dItems)
            {
                try
                {
                    actCodes.Add(Convert.ToInt32(d.Code));
                }
                catch
                {
                    _logger.Debug($"DeleteNonActiveDishes error convert {d.Code}");
                }
            }

            foreach (var d in existsD)
            {
                if (!actCodes.Contains(d.Barcode))
                {
                    var res = AlohaFly.DBProvider.Client.DeleteDish(d.Id);
                    if (!res.Success)
                    {
                        _logger.Debug($"DeleteNonActiveDishes error DeleteDish {d.Id} {res.ErrorMessage}");
                    }

                }

            }
        }

        static void ExportDishes2()
        {
            try
            {
                _logger.Debug($"ExportDishes2 ");
                var existsD = AlohaFly.DBProvider.Client.GetDishList().Result.Where(a => a.IsToGo);
                var dItems = db.Menu.Where(a => a.Active);
                _logger.Debug($"dItems count {dItems.Count()}");
                //var clsList = new List<OrderCustomer>();
                foreach (var d in dItems)
                {
                    int code = Convert.ToInt32(d.Code);
                    //  long shId = db.RKeeperMenu.Where(a => a.Code.GetValueOrDefault() == code).First().Sifr.GetValueOrDefault();
                    StickCatalog stick = null;
                    if (!existsD.Any(a => a.Barcode == code))
                    {
                        _logger.Debug($"ExportDishes2 not exist {code}");

                    }
                    else
                    {
                        var Oldd = existsD.FirstOrDefault(a => a.Barcode == code);
                        Oldd.PriceForDelivery = d.Cost;
                        Oldd.PriceForFlight = 0;

                        var res = AlohaFly.DBProvider.Client.UpdateDish(Oldd);
                        if (!res.Success)
                        {
                            _logger.Debug($"Error ExportDishes2 UpdateDish {res.ErrorMessage}");
                            continue;
                        }
                        if (db.StickCatalog.Any(a => a.Code == code.ToString()))
                        {
                            stick = db.StickCatalog.Where(a => a.Code == code.ToString()).FirstOrDefault();
                            Oldd.LabelRussianName = stick.NameRU;
                            Oldd.LabelEnglishName = stick.NameENG;
                            res = AlohaFly.DBProvider.Client.UpdateDish(Oldd);
                            if (!res.Success)
                            {
                                _logger.Debug($"Error ExportDishes2 UpdateDish {res.ErrorMessage}");
                                continue;
                            }
                            var id = Oldd.Id;
                            foreach (var sd in db.StickContent.Where(a => a.StickID == stick.ID))
                            {
                                var lbl = new ItemLabelInfo()
                                {
                                    ParenItemId = id,
                                    NameEng = sd.PartNameENG,
                                    NameRus = sd.PartNameRU,
                                    Message = sd.WarmUp,
                                    SerialNumber = sd.PartNumb
                                };
                                var res2 = AlohaFly.DBProvider.Client.CreateItemLabelInfo(lbl);
                                if (!res2.Success)
                                {
                                    _logger.Debug($"Error ExportDishes2 CreateItemLabelInfo {res2.ErrorMessage}");
                                    continue;
                                }
                            }

                        }
                    }
                }

            }
            catch(Exception e)
            {
                _logger.Debug($"ExportDishes2 error {e.Message}");
            }
        }

        static void ExportDishes()
        {
            var existsD = AlohaFly.DBProvider.Client.GetDishList().Result.Where(a => a.IsToGo);
            var dItems = db.Menu.Where(a => a.Active);
            //var clsList = new List<OrderCustomer>();
            foreach (var d in dItems)
            {
                int code = Convert.ToInt32(d.Code);
                long shId = db.RKeeperMenu.Where(a => a.Code.GetValueOrDefault() == code).First().Sifr.GetValueOrDefault();
                StickCatalog stick = null;
                if (db.StickCatalog.Any(a => a.Code == code.ToString()))
                {
                    stick = db.StickCatalog.Where(a => a.Code == code.ToString()).FirstOrDefault();
                }
                Dish nd = new Dish()
                {
                    Barcode = code,
                    IsTemporary = false,
                    IsToGo = true,
                    EnglishName = "",
                    IsAlcohol = false,
                    IsActive = true,
                    LabelRussianName = d.Name,
                    LabelEnglishName = "",
                    Name = d.Name,
                    LabelsCount = 1,
                    NeedPrintInMenu = true,
                    PriceForDelivery = d.Cost,
                    PriceForFlight = 0,
                    SHId = shId,
                    ToFlyLabelSeriesCount = 1,
                    ToGoLabelSeriesCount = 1,

                };
                if (stick != null)
                {
                    nd.LabelEnglishName = stick.NameENG;
                    nd.LabelRussianName = stick.NameRU;
                }
                var res = AlohaFly.DBProvider.Client.CreateDish(nd);
                if (!res.Success)
                {
                    string s = res.ErrorMessage;
                    continue;
                }
                if (stick != null)
                {
                    var id = res.CreatedObjectId;
                    foreach (var sd in db.StickContent.Where(a => a.StickID == stick.ID))
                    {
                        var lbl = new ItemLabelInfo()
                        {
                            ParenItemId = id,
                            NameEng = sd.PartNameENG,
                            NameRus = sd.PartNameRU,
                            Message = sd.WarmUp,
                            SerialNumber = sd.PartNumb
                        };
                        var res2 = AlohaFly.DBProvider.Client.CreateItemLabelInfo(lbl);
                        if (!res2.Success)
                        {
                            string s = res.ErrorMessage;
                            continue;
                        }
                    }
                }
            }
        }
        static void ExportCustomers()
        {
            _logger.Debug("ExportCustomers ");
            try
            {




                DateTime dtstart = new DateTime(2019, 01, 01);
                //var existsD = AlohaFly.DBProvider.Client.GetDishList().Result.Where(a => a.IsToGo);
                var oldOrdersCustIds = db.Orders.Where(a => a.DateDelivery >= dtstart).Select(a => a.ClientID).ToList();

                var existsClients = AlohaFly.DBProvider.Client.GetOrderCustomerList().Result;
                var cls = db.Client.Where(a => oldOrdersCustIds.Contains(a.ID));
                var clsList = new List<OrderCustomer>();
                foreach (var cl in cls)
                {
                    try
                    {
                        if (existsClients.Any(a => a.OldId == cl.ID)) { continue; }
                        var orderCustomer = new OrderCustomer();
                        orderCustomer.OldId = cl.ID;
                        orderCustomer.Name = cl.Name;
                        orderCustomer.SecondName = cl.FamilyName;
                        orderCustomer.IsActive = true;
                        orderCustomer.Comments = cl.Comment;
                        orderCustomer.DiscountPercent = 0;
                        orderCustomer.Email = cl.Email;

                        orderCustomer.Phones = new List<OrderCustomerPhone>();
                        foreach (var tel in db.Tel.Where(a => a.ClientID == cl.ID))
                        {
                            OrderCustomerPhone ph = new OrderCustomerPhone()
                            {
                                IsActive = tel.Active,
                                IsPrimary = (tel.ID == db.Tel.Where(a => a.ClientID == cl.ID).Select(a => a.ID).Max()),
                                Phone = tel.Number

                            };
                            orderCustomer.Phones.Add(ph);
                        }
                        orderCustomer.Addresses = new List<OrderCustomerAddress>();
                        {
                            foreach (var addr in db.Address.Where(a => a.ClientID == cl.ID))
                            {
                                OrderCustomerAddress ph = new OrderCustomerAddress()
                                {
                                    IsActive = addr.Active.GetValueOrDefault(true),
                                    IsPrimary = (addr.ID == db.Address.Where(a => a.ClientID == cl.ID).Select(a => a.ID).Max()),
                                    Address = addr.Address1,
                                    Comment = addr.Comment,
                                    MapUrl = addr.MapURL,
                                    SubWay = addr.Subway,
                                    ZoneId = addr.Zone.GetValueOrDefault(1),
                                    OldId = addr.ID

                                };
                                orderCustomer.Addresses.Add(ph);
                            }
                        }
                        //clsList.Add(orderCustomer);
                        var res = AlohaFly.DBProvider.Client.CreateOrderCustomer(orderCustomer);
                        if (!res.Success)
                        {
                            string err = res.ErrorMessage;
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.Debug("error  ExportCustomers " + e.Message);
                    }
                }
            }
            catch(Exception exc)
            {
                _logger.Debug("Error2  ExportCustomers " + exc.Message);
            }
        }


        static void ExportCustomersPhones()
        {
            _logger.Debug("ExportCustomersPhones ");
            try
            {




                DateTime dtstart = new DateTime(2019, 01, 01);
                //var existsD = AlohaFly.DBProvider.Client.GetDishList().Result.Where(a => a.IsToGo);
                var oldOrdersCustIds = db.Orders.Where(a => a.DateDelivery >= dtstart).Select(a => a.ClientID).ToList();

                var existsClients = AlohaFly.DBProvider.Client.GetOrderCustomerList().Result;



                var cls = db.Client.Where(a => oldOrdersCustIds.Contains(a.ID));
                var clsList = new List<OrderCustomer>();



                List<OrderCustomerAddress> addresses = new List<OrderCustomerAddress>();
                foreach (var addrList in existsClients.Select(a => a.Addresses))
                {
                    addresses.AddRange(addrList);


                }

                List<OrderCustomerPhone> tels = new List<OrderCustomerPhone>();
                foreach (var phones in existsClients.Select(a => a.Phones))
                {
                    tels.AddRange(phones);


                }

                foreach (var cl in cls)
                {
                    try
                    {

                        //orderCustomer.Addresses = new List<OrderCustomerAddress>();
                        {
                            foreach (var addr in db.Address.Where(a => a.ClientID == cl.ID))
                            {
                                if (!addresses.Any(a => a.OldId == addr.ID))
                                {
                                    OrderCustomerAddress ph = new OrderCustomerAddress()
                                    {
                                        IsActive = addr.Active.GetValueOrDefault(true),
                                        IsPrimary = (addr.ID == db.Address.Where(a => a.ClientID == cl.ID).Select(a => a.ID).Max()),
                                        Address = addr.Address1,
                                        Comment = addr.Comment,
                                        MapUrl = addr.MapURL,
                                        SubWay = addr.Subway,
                                        ZoneId = addr.Zone.GetValueOrDefault(1),
                                        OldId = addr.ID,
                                        OrderCustomerId = existsClients.FirstOrDefault(a => a.OldId == cl.ID).Id,
                                    };
                                    var res = AlohaFly.DBProvider.Client.CreateOrderCustomerAddress(ph);
                                    _logger.Debug("Add address addr.ID");
                                    if (!res.Success)
                                    {
                                        string err = res.ErrorMessage;
                                    }
                                }
                            }
                        }

                        // if (existsClients.Any(a => a.OldId == cl.ID)) { continue; }
                        /*
                         var orderCustomer = new OrderCustomer();
                         orderCustomer.OldId = cl.ID;
                         orderCustomer.Name = cl.Name;
                         orderCustomer.SecondName = cl.FamilyName;
                         orderCustomer.IsActive = true;
                         orderCustomer.Comments = cl.Comment;
                         orderCustomer.DiscountPercent = 0;
                         orderCustomer.Email = cl.Email;

                         orderCustomer.Phones = new List<OrderCustomerPhone>();
                         foreach (var tel in db.Tel.Where(a => a.ClientID == cl.ID))
                         {
                             OrderCustomerPhone ph = new OrderCustomerPhone()
                             {
                                 IsActive = tel.Active,
                                 IsPrimary = (tel.ID == db.Tel.Where(a => a.ClientID == cl.ID).Select(a => a.ID).Max()),
                                 Phone = tel.Number

                             };
                             orderCustomer.Phones.Add(ph);
                         }
                         orderCustomer.Addresses = new List<OrderCustomerAddress>();
                         {
                             foreach (var addr in db.Address.Where(a => a.ClientID == cl.ID))
                             {
                                 OrderCustomerAddress ph = new OrderCustomerAddress()
                                 {
                                     IsActive = addr.Active.GetValueOrDefault(true),
                                     IsPrimary = (addr.ID == db.Address.Where(a => a.ClientID == cl.ID).Select(a => a.ID).Max()),
                                     Address = addr.Address1,
                                     Comment = addr.Comment,
                                     MapUrl = addr.MapURL,
                                     SubWay = addr.Subway,
                                     ZoneId = addr.Zone.GetValueOrDefault(1),
                                     OldId = addr.ID

                                 };
                                 orderCustomer.Addresses.Add(ph);
                             }
                         }
                         //clsList.Add(orderCustomer);
                         var res = AlohaFly.DBProvider.Client.CreateOrderCustomer(orderCustomer);
                         if (!res.Success)
                         {
                             string err = res.ErrorMessage;
                         }

     */
                    }
                    catch (Exception e)
                    {
                        _logger.Debug("error  ExportCustomersPhones " + e.Message);
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.Debug("Error2  ExportCustomersPhones " + exc.Message);
            }
        }




        static void ExportCustomer(int OldId)
        {
            var existsClients = AlohaFly.DBProvider.Client.GetOrderCustomerList().Result;
            var cls = db.Client;
            //var clsList = new List<OrderCustomer>();
            var cl = db.Client.SingleOrDefault(a => a.ID == OldId);

            if (existsClients.Any(a => a.OldId == cl.ID)) { return; }
            var orderCustomer = new OrderCustomer();
            orderCustomer.OldId = cl.ID;
            orderCustomer.Name = cl.Name;
            orderCustomer.SecondName = cl.FamilyName;
            orderCustomer.IsActive = true;
            orderCustomer.Comments = cl.Comment;
            orderCustomer.DiscountPercent = 0;
            orderCustomer.Email = cl.Email;

            orderCustomer.Phones = new List<OrderCustomerPhone>();
            foreach (var tel in db.Tel.Where(a => a.ID == cl.ID))
            {
                OrderCustomerPhone ph = new OrderCustomerPhone()
                {
                    IsActive = tel.Active,
                    IsPrimary = (tel.ID == db.Tel.Where(a => a.ID == cl.ID).Select(a => a.ID).Max()),
                    Phone = tel.Number

                };
                orderCustomer.Phones.Add(ph);
            }
            orderCustomer.Addresses = new List<OrderCustomerAddress>();
            {
                foreach (var addr in db.Address.Where(a => a.ID == cl.ID))
                {
                    OrderCustomerAddress ph = new OrderCustomerAddress()
                    {
                        IsActive = addr.Active.GetValueOrDefault(true),
                        IsPrimary = (addr.ID == db.Address.Where(a => a.ID == cl.ID).Select(a => a.ID).Max()),
                        Address = addr.Address1,
                        Comment = addr.Comment,
                        MapUrl = addr.MapURL,
                        SubWay = addr.Subway,
                        ZoneId = addr.Zone.GetValueOrDefault(1),
                        OldId = addr.ID

                    };
                    orderCustomer.Addresses.Add(ph);
                }
            }
            //clsList.Add(orderCustomer);
            var res = AlohaFly.DBProvider.Client.CreateOrderCustomer(orderCustomer);
            if (!res.Success)
            {
                string err = res.ErrorMessage;
            }
        }

    }
}
