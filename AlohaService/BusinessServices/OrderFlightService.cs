using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlohaService.ServiceDataContracts;
using AlohaService.Entities;
using log4net;
using AlohaService.Helpers;
using Telerik.Windows.Documents.Spreadsheet.Model;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace AlohaService.BusinessServices
{
    public class OrderFlightService
    {
        private AlohaDb db;
        protected ILog log;

        public OrderFlightService(AlohaDb databaseContext)
        {
            db = databaseContext;

            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }

        public OperationResult CreateOrderFlight(ServiceDataContracts.OrderFlight orderFlight)
        {
            try
            {
                var of = new Entities.OrderFlight();
                if (orderFlight.AirCompany != null)
                {
                    of.AirCompanyId = orderFlight.AirCompany?.Id;//.AirCompanyId;
                }
                else
                {
                    of.AirCompanyId = orderFlight.AirCompanyId;
                }

                of.Comment = orderFlight.Comment;
                of.ContactPersonId = orderFlight.ContactPersonId;
                of.DeliveryDate = orderFlight.DeliveryDate;
                of.DeliveryPlaceId = orderFlight.DeliveryPlaceId;
                of.DriverId = orderFlight.DriverId;
                of.ExportTime = orderFlight.ExportTime;
                of.ExtraCharge = orderFlight.ExtraCharge;
                of.FlightNumber = orderFlight.FlightNumber;
                of.NumberOfBoxes = orderFlight.NumberOfBoxes;
                of.OrderComment = orderFlight.OrderComment;
                of.OrderNumber = orderFlight.OrderNumber;
                of.OrderStatus = (int)orderFlight.OrderStatus;
                of.PhoneNumber = orderFlight.PhoneNumber;
                of.ReadyTime = orderFlight.ReadyTime;
                of.WhoDeliveredPersonPersonId = orderFlight.WhoDeliveredPersonPersonId;
                of.DeliveryAddress = orderFlight.DeliveryAddress;

                of.CreationDate = orderFlight.CreationDate == null ? DateTime.Now : orderFlight.CreationDate.Value;
                of.CreatedById = orderFlight.CreatedById;
                of.SendById = orderFlight.SendById;

                of.Code = orderFlight.Code;

                of.PaymentId = orderFlight.PaymentId;

                of.DiscountSumm = orderFlight.DiscountSumm;

                of.Closed = orderFlight.Closed;
                of.NeedPrintFR = orderFlight.NeedPrintFR;
                of.NeedPrintPrecheck = orderFlight.NeedPrintPrecheck;
                of.FRPrinted = orderFlight.FRPrinted;
                of.PreCheckPrinted = orderFlight.PreCheckPrinted;
                of.IsSHSent = orderFlight.IsSHSent;
                of.AlohaGuidId = orderFlight.AlohaGuidId;
                db.OrderFlight.Add(of);
                db.SaveChanges();

                return new OperationResult
                {
                    Success = true,
                    CreatedObjectId = of.Id
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResult
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }
        public OperationResult CreateOrderFlightWithPackage(ServiceDataContracts.OrderFlight orderFlight)
        {
            try
            {
                log.Debug($"CreateOrderToGoWithPackage");
                var otg = Mapper.Map<ServiceDataContracts.OrderFlight, Entities.OrderFlight>(orderFlight);
                otg.UpdatedDate = DateTime.Now;
                otg.LastUpdatedSession = orderFlight.LastUpdatedSession;
                db.OrderFlight.Add(otg);
                db.SaveChanges();

                OrderCustomrInfoService srv = new OrderCustomrInfoService(db);
                //srv.RecalcCustomerInfo(orderToGo.OrderCustomerId.GetValueOrDefault());
                srv.RecalcCustomerAllInfo();
                return new OperationResult
                {
                    Success = true,
                    CreatedObjectId = otg.Id
                };
            }

            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResult
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResultValue<ServiceDataContracts.OrderFlight> GetOrderFlightByCode(long orderCode)
        {
            var result = new OperationResultValue<ServiceDataContracts.OrderFlight>();
            result.Success = true;
            var order = db.OrderFlight.FirstOrDefault(of => of.Code == orderCode);

            if (order == null)
            {
                result.Result = null;
                return result;
            }

            result.Result = new ServiceDataContracts.OrderFlight();
            result.Result.Id = order.Id;
            result.Result.AirCompany = order.AirCompany == null ? null :
                new ServiceDataContracts.AirCompany
                {
                    Id = order.AirCompany.Id,
                    Code = order.AirCompany.Code,
                    Name = order.AirCompany.Name,
                    FullName = order.AirCompany.FullName,
                    Address = order.AirCompany.Address,
                    Inn = order.AirCompany.Inn,
                    IkaoCode = order.AirCompany.IkaoCode,
                    IataCode = order.AirCompany.IataCode,
                    RussianCode = order.AirCompany.RussianCode,
                    IsActive = order.AirCompany.IsActive,
                    SHId = order.AirCompany.SHId,
                    Code1C = order.AirCompany.Code1C,
                    Name1C = order.AirCompany.Name1C,
                    DiscountType = order.AirCompany.DiscountType == null ? null : new ServiceDataContracts.Discount
                    {
                        Id = order.AirCompany.DiscountType.Id,
                        Name = order.AirCompany.DiscountType.Name,
                        //Ranges = order.AirCompany.DiscountType.Ranges.Select(r => new ServiceDataContracts.DiscountRange
                        //{
                        //    DiscountPercent = r.DiscountPercent,
                        //    End = r.End,
                        //    Id = r.Id,
                        //    Start = r.Start
                        //}).ToList()
                    },
                    PaymentId = order.AirCompany.PaymentId,
                    PaymentType = order.AirCompany.PaymentType == null ? null :
                        new ServiceDataContracts.Payment
                        {
                            Code = order.AirCompany.PaymentType.Code,
                            Id = order.AirCompany.PaymentType.Id,
                            FiskalId = order.AirCompany.PaymentType.FiskalId,
                            IsCash = order.AirCompany.PaymentType.IsCash,
                            Name = order.AirCompany.PaymentType.Name
                        }
                };
            result.Result.AirCompanyId = order.AirCompanyId;
            result.Result.Comment = order.Comment;
            result.Result.ContactPerson = order.ContactPerson == null ? null :
                new ServiceDataContracts.ContactPerson
                {
                    Id = order.ContactPerson.Id,
                    FirstName = order.ContactPerson.FirstName,
                    SecondName = order.ContactPerson.SecondName,
                    Phone = order.ContactPerson.Phone
                };
            result.Result.ContactPersonId = order.ContactPersonId;
            result.Result.DeliveryDate = order.DeliveryDate;
            result.Result.DeliveryPlace = order.DeliveryPlace == null ? null :
                new ServiceDataContracts.DeliveryPlace
                {
                    Id = order.DeliveryPlace.Id,
                    Phone = order.DeliveryPlace.Phone,
                    Name = order.DeliveryPlace.Name
                };
            result.Result.DeliveryPlaceId = order.DeliveryPlaceId;
            result.Result.Driver = order.Driver == null ? null :
                new ServiceDataContracts.Driver
                {
                    Id = order.Driver.Id,
                    FullName = order.Driver.FullName,
                    Phone = order.Driver.Phone
                };
            result.Result.DriverId = order.DriverId;
            result.Result.ExportTime = order.ExportTime;
            result.Result.ExtraCharge = order.ExtraCharge;
            result.Result.FlightNumber = order.FlightNumber;
            result.Result.Id = order.Id;
            result.Result.NumberOfBoxes = order.NumberOfBoxes;
            result.Result.OrderComment = order.OrderComment;
            result.Result.OrderNumber = order.OrderNumber;
            result.Result.OrderStatus = (OrderStatus)order.OrderStatus;
            result.Result.PhoneNumber = order.PhoneNumber;
            result.Result.ReadyTime = order.ReadyTime;
            result.Result.DeliveryAddress = order.DeliveryAddress;


            result.Result.CreationDate = order.CreationDate;

            result.Result.CreatedBy = order.CreatedBy == null ? null :
                new ServiceDataContracts.User
                {
                    Id = order.CreatedBy.Id,

                    UserName = order.CreatedBy.UserName,
                    Email = order.CreatedBy.Email,
                    Phone = order.CreatedBy.Phone,

                    RegistrationStatus = order.CreatedBy.RegistrationStatus,
                    SequrityQuestion = order.CreatedBy.SequrityQuestion,
                    SequrityAnswer = order.CreatedBy.SequrityAnswer,
                    FullName = order.CreatedBy.FullName,
                    UserRole = (UserRole)order.CreatedBy.UserRole
                };
            result.Result.CreatedById = order.CreatedById;

            result.Result.SendBy = order.SendBy == null ? null :
                new ServiceDataContracts.User
                {
                    Id = order.SendBy.Id,

                    UserName = order.SendBy.UserName,
                    Email = order.SendBy.Email,
                    Phone = order.SendBy.Phone,

                    RegistrationStatus = order.SendBy.RegistrationStatus,
                    SequrityQuestion = order.SendBy.SequrityQuestion,
                    SequrityAnswer = order.SendBy.SequrityAnswer,
                    FullName = order.SendBy.FullName,
                    UserRole = (UserRole)order.SendBy.UserRole
                };

            result.Result.SendById = order.CreatedById;
            result.Result.Code = order.Code;

            result.Result.PaymentId = order.PaymentId;
            result.Result.PaymentType = order.PaymentType == null ? null :
                new ServiceDataContracts.Payment
                {
                    Code = order.AirCompany.PaymentType.Code,
                    Id = order.AirCompany.PaymentType.Id,
                    FiskalId = order.AirCompany.PaymentType.FiskalId,
                    IsCash = order.AirCompany.PaymentType.IsCash,
                    Name = order.AirCompany.PaymentType.Name
                };

            result.Result.DiscountSumm = order.DiscountSumm;

            result.Result.Closed = order.Closed;
            result.Result.NeedPrintFR = order.NeedPrintFR;
            result.Result.NeedPrintPrecheck = order.NeedPrintPrecheck;
            result.Result.FRPrinted = order.FRPrinted;
            result.Result.PreCheckPrinted = order.PreCheckPrinted;
            result.Result.IsSHSent = order.IsSHSent;


            return result;
        }

        public OperationResultValue<ServiceDataContracts.OrderFlight> GetOrderFlight(long orderFlightId)
        {
            try
            {
                var db = new AlohaDb();
                log.Info("GetOrderFlight start");
                var order = db.OrderFlight.FirstOrDefault(of => of.Id == orderFlightId);

                if (order == null)
                {
                    return new OperationResultValue<ServiceDataContracts.OrderFlight> { Success = false, ErrorMessage = "order Not Found." };
                }

                var result = new OperationResultValue<ServiceDataContracts.OrderFlight>();
                result.Success = true;
                result.Result = new ServiceDataContracts.OrderFlight();
                result.Result.Id = order.Id;
                /*
                result.Result.AirCompany = order.AirCompany == null ? null :
                    new ServiceDataContracts.AirCompany
                    {
                        Id = order.AirCompany.Id,
                        Code = order.AirCompany.Code,
                        Name = order.AirCompany.Name,
                        FullName = order.AirCompany.FullName,
                        Address = order.AirCompany.Address,
                        Inn = order.AirCompany.Inn,
                        IkaoCode = order.AirCompany.IkaoCode,
                        IataCode = order.AirCompany.IataCode,
                        RussianCode = order.AirCompany.RussianCode,
                        IsActive = order.AirCompany.IsActive,
                        SHId = order.AirCompany.SHId,
                        Code1C = order.AirCompany.Code1C,
                        Name1C = order.AirCompany.Name1C,
                        DiscountType = order.AirCompany.DiscountType == null ? null :
                        new ServiceDataContracts.Discount
                        {
                            Id = order.AirCompany.DiscountType.Id,
                            Name = order.AirCompany.DiscountType.Name,
                            //Ranges = order.AirCompany.DiscountType.Ranges.Select(r => new ServiceDataContracts.DiscountRange
                            //{
                            //    DiscountPercent = r.DiscountPercent,
                            //    End = r.End,
                            //    Id = r.Id,
                            //    Start = r.Start
                            //}).ToList()
                        },
                        PaymentId = order.AirCompany.PaymentId,
                        PaymentType = order.AirCompany.PaymentType == null ? null :
                            new ServiceDataContracts.Payment
                            {
                                Code = order.AirCompany.PaymentType.Code,
                                Id = order.AirCompany.PaymentType.Id,
                                FiskalId = order.AirCompany.PaymentType.FiskalId,
                                IsCash = order.AirCompany.PaymentType.IsCash,
                                Name = order.AirCompany.PaymentType.Name
                            }
                    };
                    */
                result.Result.AirCompanyId = order.AirCompanyId;
                result.Result.Comment = order.Comment;
                /*
                result.Result.ContactPerson = order.ContactPerson == null ? null :
                    new ServiceDataContracts.ContactPerson
                    {
                        Id = order.ContactPerson.Id,
                        FirstName = order.ContactPerson.FirstName,
                        SecondName = order.ContactPerson.SecondName,
                        Phone = order.ContactPerson.Phone
                    };
                    */
                result.Result.ContactPersonId = order.ContactPersonId;
                result.Result.DeliveryDate = order.DeliveryDate;
                /*
                result.Result.DeliveryPlace = order.DeliveryPlace == null ? null :
                    new ServiceDataContracts.DeliveryPlace
                    {
                        Id = order.DeliveryPlace.Id,
                        Phone = order.DeliveryPlace.Phone,
                        Name = order.DeliveryPlace.Name
                    };
                    */
                result.Result.DeliveryPlaceId = order.DeliveryPlaceId;
                /*
                result.Result.Driver = order.Driver == null ? null :
                    new ServiceDataContracts.Driver
                    {
                        Id = order.Driver.Id,
                        FullName = order.Driver.FullName,
                        Phone = order.Driver.Phone
                    };
                    */
                result.Result.DriverId = order.DriverId;
                result.Result.ExportTime = order.ExportTime;
                result.Result.ExtraCharge = order.ExtraCharge;
                result.Result.FlightNumber = order.FlightNumber;
                result.Result.Id = order.Id;
                result.Result.NumberOfBoxes = order.NumberOfBoxes;
                result.Result.OrderComment = order.OrderComment;
                result.Result.OrderNumber = order.OrderNumber;
                result.Result.OrderStatus = (OrderStatus)order.OrderStatus;
                result.Result.PhoneNumber = order.PhoneNumber;
                result.Result.ReadyTime = order.ReadyTime;
                result.Result.DeliveryAddress = order.DeliveryAddress;


                result.Result.CreationDate = order.CreationDate;
                /*
                result.Result.CreatedBy = order.CreatedBy == null ? null :
                    new ServiceDataContracts.User
                    {
                        Id = order.CreatedBy.Id,

                        UserName = order.CreatedBy.UserName,
                        Email = order.CreatedBy.Email,
                        Phone = order.CreatedBy.Phone,

                        RegistrationStatus = order.CreatedBy.RegistrationStatus,
                        SequrityQuestion = order.CreatedBy.SequrityQuestion,
                        SequrityAnswer = order.CreatedBy.SequrityAnswer,
                        FullName = order.CreatedBy.FullName,
                        UserRole = (UserRole)order.CreatedBy.UserRole
                    };
                    */
                result.Result.CreatedById = order.CreatedById;

                /*
                result.Result.SendBy = order.SendBy == null ? null :
                new ServiceDataContracts.User
                {
                    Id = order.SendBy.Id,

                    UserName = order.SendBy.UserName,
                    Email = order.SendBy.Email,
                    Phone = order.SendBy.Phone,

                    RegistrationStatus = order.SendBy.RegistrationStatus,
                    SequrityQuestion = order.SendBy.SequrityQuestion,
                    SequrityAnswer = order.SendBy.SequrityAnswer,
                    FullName = order.SendBy.FullName,
                    UserRole = (UserRole)order.SendBy.UserRole
                };
                */
                result.Result.SendById = order.SendById;

                result.Result.Code = order.Code;





                if (order.DishPackages != null)
                {
                    log.Info("order.DishPackages.Count: " + order.DishPackages.Count());
                    result.Result.DishPackages = order.DishPackages.Select(pack =>
                    new ServiceDataContracts.DishPackageFlightOrder
                    {

                        Amount = pack.Amount,
                        Code = pack.Code,
                        Comment = pack.Comment,
                        Deleted = pack.Deleted,
                        DeletedStatus = pack.DeletedStatus,
                        SpisPaymentId = pack.SpisPaymentId,
                        Id = pack.Id,
                        DishId = pack.DishId,
                        DishName = pack.DishName,
                        OrderFlightId = pack.OrderFlightId,
                        TotalPrice = pack.TotalPrice,
                        PositionInOrder = pack.PositionInOrder,
                        PassageNumber = pack.PassageNumber

                    }).ToList();
                }
                result.Result.PaymentId = order.PaymentId;
                result.Result.PaymentType = order.PaymentType == null ? null :
                    new ServiceDataContracts.Payment
                    {
                        Code = order.AirCompany.PaymentType.Code,
                        Id = order.AirCompany.PaymentType.Id,
                        FiskalId = order.AirCompany.PaymentType.FiskalId,
                        IsCash = order.AirCompany.PaymentType.IsCash,
                        Name = order.AirCompany.PaymentType.Name
                    };


                result.Result.DiscountSumm = order.DiscountSumm;

                result.Result.Closed = order.Closed;
                result.Result.NeedPrintFR = order.NeedPrintFR;
                result.Result.NeedPrintPrecheck = order.NeedPrintPrecheck;
                result.Result.FRPrinted = order.FRPrinted;
                result.Result.PreCheckPrinted = order.PreCheckPrinted;
                result.Result.IsSHSent = order.IsSHSent;

                log.Info("GetOrderFlight end");

                return result;
            }
            catch (Exception e)
            {
                log.Error("GetOrderFlight failed.", e);
                return new OperationResultValue<ServiceDataContracts.OrderFlight> { Success = false, ErrorMessage = "GetOrderFlight failed." };
            }
        }

        private string GetOrderFlightString(Entities.OrderFlight orderFlight)
        {
            var result = string.Empty;

            result += "OrderId: " + orderFlight.Id;
            result += Environment.NewLine;

            foreach (var dp in orderFlight.DishPackages)
            {
                result += "Dish Name:" + dp.Dish.Name;
                result += "Amount: " + dp.Amount;
                result += Environment.NewLine;
            }

            return result;
        }

        private string GetOrderFlightString(ServiceDataContracts.OrderFlight orderFlight)
        {
            var result = string.Empty;

            result += "OrderId: " + orderFlight.Id;
            result += Environment.NewLine;

            foreach (var dp in orderFlight.DishPackages)
            {
                result += "Dish Name:" + dp.Dish.Name;
                result += "Amount: " + dp.Amount;
                result += Environment.NewLine;
            }

            return result;
        }

        private void UpdateDishesForOrder(ServiceDataContracts.OrderFlight orderFlight, Entities.OrderFlight originalOrder, long userId=0)
        {
            log.Info("UpdateDishesForOrder");

            // Delete not presented packages
            foreach (var package in originalOrder.DishPackages.ToList())
            {
                if (!orderFlight.DishPackages.Any(dp => dp.Id == package.Id))
                {
                    log.Info("Delete not presented packages");
                    log.Info("Dish Id: " + package.DishId);
                    var packageToDelete = db.DishPackagesFlightOrder.First(p => p.Id == package.Id);
                    db.DishPackagesFlightOrder.Remove(packageToDelete);
                    // db.SaveChanges();
                }
            }

            var ds = new DishPackageFlightOrderService(db);
            // Add New packages
            foreach (var package in orderFlight.DishPackages.ToList())
            {
                if (package.Id == 0)
                {
                    log.Info("Add new packages");
                    log.Info("ADish Id: " + package.DishId);


                    ds.CreateDishPackageFlightOrder(package);

                    /*
                    db.DishPackagesFlightOrder.Add(
                        new Entities.DishPackageFlightOrder
                        {
                            Amount = package.Amount,
                            Code = package.Code,
                            Comment = package.Comment,
                            DishId = package.DishId,
                            DishName = package.DishName,
                            OrderFlightId = orderFlight.Id,
                            PassageNumber = package.PassageNumber,
                            PositionInOrder = package.PositionInOrder,
                            TotalPrice = package.TotalPrice
                        });
                    //db.SaveChanges();
                    */
                }
            }

            // Update presented packages
            foreach (var package in orderFlight.DishPackages.ToList())
            {

                ds.UpdateDishPackageFlightOrder(package);
                /*
                var packageToUpdate = db.DishPackagesFlightOrder.FirstOrDefault(p => p.Id == package.Id);

                if (packageToUpdate != null)
                {
                    log.Info("Update presented");
                    log.Info("Dish Id: " + package.DishId);

                    packageToUpdate.Amount = package.Amount;
                    packageToUpdate.Code = package.Code;
                    packageToUpdate.Comment = package.Comment;
                    packageToUpdate.DishId = package.DishId;
                    packageToUpdate.DishName = package.DishName;
                    packageToUpdate.PassageNumber = package.PassageNumber;
                    packageToUpdate.PositionInOrder = package.PositionInOrder;
                    packageToUpdate.TotalPrice = package.TotalPrice;

                    //db.SaveChanges();
                }
                */
            }
        }


       

        public OperationResult UpdateOrderFlight2(ServiceDataContracts.OrderFlight orderFlight, long userId)
        {
            var res = UpdateOrderFlight(orderFlight, userId);
            OperationResult r = new OperationResult()
            {
                Success = res.Success,
                CreatedObjectId = res.Result.Id,
                ErrorMessage = res.ErrorMessage
            };
            return r;

        }

            public OperationResultValue<ServiceDataContracts.OrderFlight> UpdateOrderFlight(ServiceDataContracts.OrderFlight orderFlight, long userId)
        {
            var user = db.Users.FirstOrDefault(usr => usr.Id == userId);
            if (user == null)
            {
                return new OperationResultValue<ServiceDataContracts.OrderFlight> { Success = false, ErrorMessage = "User Not Found." };
            }

            var order = db.OrderFlight.FirstOrDefault(o => o.Id == orderFlight.Id);

            if (order == null)
            {
                return new OperationResultValue<ServiceDataContracts.OrderFlight> { Success = false, ErrorMessage = "OrderFlight Not Found." };
            }

            order.AirCompanyId = orderFlight.AirCompanyId;
            order.Comment = orderFlight.Comment;
            order.ContactPersonId = orderFlight.ContactPersonId;
            order.DeliveryDate = orderFlight.DeliveryDate;
            order.DeliveryPlaceId = orderFlight.DeliveryPlaceId;
            order.DriverId = orderFlight.DriverId;
            order.ExportTime = orderFlight.ExportTime;
            order.ExtraCharge = orderFlight.ExtraCharge;
            order.FlightNumber = orderFlight.FlightNumber;
            order.NumberOfBoxes = orderFlight.NumberOfBoxes;
            order.OrderComment = orderFlight.OrderNumber;
            order.OrderNumber = orderFlight.OrderNumber;
            order.OrderStatus = (int)orderFlight.OrderStatus;
            order.PhoneNumber = orderFlight.PhoneNumber;
            order.ReadyTime = orderFlight.ReadyTime;
            order.WhoDeliveredPersonPersonId = orderFlight.WhoDeliveredPersonPersonId;
            order.CreatedById = orderFlight.CreatedById;
            order.SendById = orderFlight.SendById;

            order.Code = orderFlight.Code;
            order.DeliveryAddress = orderFlight.DeliveryAddress;

            order.PaymentId = orderFlight.PaymentId;

            if (orderFlight.CreationDate != null)
            {
                order.CreationDate = orderFlight.CreationDate.Value;
            }

            order.DiscountSumm = orderFlight.DiscountSumm;

            order.Closed = orderFlight.Closed;
            order.NeedPrintFR = orderFlight.NeedPrintFR;
            order.NeedPrintPrecheck = orderFlight.NeedPrintPrecheck;
            order.FRPrinted = orderFlight.FRPrinted;
            order.PreCheckPrinted = orderFlight.PreCheckPrinted;
            order.IsSHSent = orderFlight.IsSHSent;

            Entities.LogItem logItem = new Entities.LogItem();

            if (orderFlight.DishPackages != null)
            {
                logItem.ActionDescription = "Обновление заказа";
                logItem.ActionName = "UpdateDishesForOrder";
                logItem.CreationDate = DateTime.Now;
                logItem.MethodName = "UpdateDishesForOrder";
                logItem.UserId = userId;
                logItem.StateBefore = GetOrderFlightString(order);

                UpdateDishesForOrder(orderFlight, order, userId);

                //log.StateAfter = GetOrderFlightString(orderFlight);
            }


            order.UpdatedDate = DateTime.Now;
            order.LastUpdatedSession = orderFlight.LastUpdatedSession;

            db.SaveChanges();

            log.Info("GetOrder Flight after update. Order Flight Id: " + orderFlight.Id);

            var result = GetOrderFlight(orderFlight.Id);

            try
            {
                if (orderFlight.DishPackages != null)
                {
                    logItem.StateAfter = GetOrderFlightString(result.Result);
                    db.LogItems.Add(logItem);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }

            return result;
            //return new OperationResultValue<ServiceDataContracts.OrderFlight>
            //{
            //    Success = true,
            //    Result = null
            //};
        }

        public OperationResult DeleteOrderFlight(long orderFlightId)
        {
            var order = db.OrderFlight.FirstOrDefault(oc => oc.Id == orderFlightId);
            if (order == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "OrderFlight Not Found." };
            }

            db.OrderFlight.Remove(order);
            db.SaveChanges();

            return new OperationResult { Success = true };
        }

        public bool InsertOrderFlightFromAloha(ServiceDataContracts.OrderFlight orderFlight)
        {
            if (orderFlight.AlohaGuidId != null && db.OrderFlight.Any(a => a.AlohaGuidId == orderFlight.AlohaGuidId))
            {
                return true;
            }
            var res = CreateOrderFlight(orderFlight);
            if (res.Success)
            {
                var dishPackageFlightOrderService = new DishPackageFlightOrderService(db);
                foreach (var d in orderFlight.DishPackages)
                {
                    d.OrderFlightId = res.CreatedObjectId;
                    var resd = dishPackageFlightOrderService.CreateDishPackageFlightOrder(d);
                    if (!resd.Success)
                    {
                        DeleteOrderFlight(res.CreatedObjectId);
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }


        public OperationResultValue<List<ServiceDataContracts.OrderFlight>> GetOrderFlightList(OrderFlightFilter filter, PageInfo page)
        {
            log.Info("GetOrderFlightList(OrderFlightFilter filter, PageInfo page)");
            try
            {
                var query = db.OrderFlight.Where(o => true);

                #region Filter

                if (filter.AirCompanyId != null)
                {
                    //log.Info(String.Format("Using filter.AirCompanyId {0}", filter.AirCompanyId));
                    query = query.Where(o => o.AirCompanyId == filter.AirCompanyId);
                }

                if (filter.Code != null)
                {
                    //log.Info(String.Format("Using filter.Code {0}", filter.Code));
                    query = query.Where(o => o.Code == filter.Code);
                }

                if (!string.IsNullOrEmpty(filter.Comment))
                {
                    //log.Info(String.Format("Using filter.Comment {0}", filter.Comment));
                    query = query.Where(o => o.Comment == filter.Comment);
                }

                if (filter.ContactPersonId != null)
                {
                    //log.Info(String.Format("Using filter.ContactPersonId {0}", filter.ContactPersonId));
                    query = query.Where(o => o.ContactPersonId == filter.ContactPersonId);
                }

                if (filter.CreatedById != null)
                {
                    //log.Info(String.Format("Using filter.CreatedById {0}", filter.CreatedById));
                    query = query.Where(o => o.CreatedById == filter.CreatedById);
                }

                if (filter.CreationDateStart != null)
                {
                    //log.Info(String.Format("Using filter.CreationDateStart {0}", filter.CreationDateStart));
                    query = query.Where(o => o.CreationDate >= filter.CreationDateStart);
                }

                if (filter.CreationDateEnd != null)
                {
                    //log.Info(String.Format("Using filter.CreationDateEnd {0}", filter.CreationDateEnd));
                    query = query.Where(o => o.CreationDate <= filter.CreationDateEnd);
                }

                if (filter.DeliveryDateEnd != null)
                {
                    //log.Info(String.Format("Using filter.DeliveryDateEnd {0}", filter.DeliveryDateEnd));
                    query = query.Where(o => o.DeliveryDate <= filter.DeliveryDateEnd);
                }

                if (filter.DeliveryDateStart != null)
                {
                    //log.Info(String.Format("Using filter.AirCompanyId {0}", filter.AirCompanyId));
                    query = query.Where(o => o.DeliveryDate >= filter.DeliveryDateStart);
                }

                if (filter.DeliveryPlaceId != null)
                {
                    //log.Info(String.Format("Using filter.DeliveryPlaceId {0}", filter.DeliveryPlaceId));
                    query = query.Where(o => o.DeliveryPlaceId >= filter.DeliveryPlaceId);
                }

                if (filter.DriverId != null)
                {
                    //log.Info(String.Format("Using filter.DriverId {0}", filter.DriverId));
                    query = query.Where(o => o.DriverId == filter.DriverId);
                }

                if (filter.ExtraCharge != null)
                {
                    //log.Info(String.Format("Using filter.ExtraCharge {0}", filter.ExtraCharge));
                    query = query.Where(o => o.ExtraCharge == filter.ExtraCharge);
                }

                if (filter.ExportTimeEnd != null)
                {
                    //log.Info(String.Format("Using filter.ExportTimeEnd {0}", filter.ExportTimeEnd));
                    query = query.Where(o => o.ExportTime <= filter.ExportTimeEnd);
                }

                if (filter.ExportTimeStart != null)
                {
                    //log.Info(String.Format("Using filter.ExportTimeStart {0}", filter.ExportTimeStart));
                    query = query.Where(o => o.ExportTime <= filter.ExportTimeStart);
                }

                if (filter.ExtraCharge != null)
                {
                    //log.Info(String.Format("Using filter.ExtraCharge {0}", filter.ExtraCharge));
                    query = query.Where(o => o.ExtraCharge <= filter.ExtraCharge);
                }

                if (!string.IsNullOrEmpty(filter.FlightNumber))
                {
                    //log.Info(String.Format("Using filter.FlightNumber {0}", filter.FlightNumber));
                    query = query.Where(o => o.FlightNumber == filter.FlightNumber);
                }

                if (filter.NumberOfBoxes != null)
                {
                    //log.Info(String.Format("Using filter.NumberOfBoxes {0}", filter.NumberOfBoxes));
                    query = query.Where(o => o.NumberOfBoxes == filter.NumberOfBoxes);
                }

                if (!string.IsNullOrEmpty(filter.OrderComment))
                {
                    //log.Info(String.Format("Using filter.OrderComment {0}", filter.OrderComment));
                    query = query.Where(o => o.OrderComment == filter.OrderComment);
                }

                if (!string.IsNullOrEmpty(filter.OrderNumber))
                {
                    //log.Info(String.Format("Using filter.OrderNumber {0}", filter.OrderNumber));
                    query = query.Where(o => o.OrderNumber == filter.OrderNumber);
                }

                if (filter.OrderStatus != null)
                {
                    var statuses = new List<int>();
                    if (filter.OrderStatus.Value.HasFlag(OrderStatus.Cancelled))
                    {
                        statuses.Add((int)OrderStatus.Cancelled);
                    }

                    if (filter.OrderStatus.Value.HasFlag(OrderStatus.CancelledWithRemains))
                    {
                        statuses.Add((int)OrderStatus.CancelledWithRemains);
                    }

                    if (filter.OrderStatus.Value.HasFlag(OrderStatus.Closed))
                    {
                        statuses.Add((int)OrderStatus.Closed);
                    }

                    if (filter.OrderStatus.Value.HasFlag(OrderStatus.InWork))
                    {
                        statuses.Add((int)OrderStatus.InWork);
                    }

                    if (filter.OrderStatus.Value.HasFlag(OrderStatus.Sent))
                    {
                        statuses.Add((int)OrderStatus.Sent);
                    }

                    //log.Info(String.Format("Using filter.OrderStatus {0}", filter.OrderStatus));
                    //query = query.Where(o => o.OrderStatus == (int)filter.OrderStatus);

                    query = query.Where(o => statuses.Contains(o.OrderStatus));
                }

                if (!string.IsNullOrEmpty(filter.PhoneNumber))
                {
                    //log.Info(String.Format("Using filter.PhoneNumber {0}", filter.PhoneNumber));
                    query = query.Where(o => o.PhoneNumber == filter.PhoneNumber);
                }

                if (filter.ReadyTimeEnd != null)
                {
                    //log.Info(String.Format("Using filter.ReadyTimeEnd {0}", filter.ReadyTimeEnd));
                    query = query.Where(o => o.ReadyTime <= filter.ReadyTimeEnd);
                }

                if (filter.ReadyTimeStart != null)
                {
                    //log.Info(String.Format("Using filter.ReadyTimeStart {0}", filter.ReadyTimeStart));
                    query = query.Where(o => o.ReadyTime >= filter.ReadyTimeStart);
                }

                if (filter.WhoDeliveredPersonPersonId != null)
                {
                    //log.Info(String.Format("Using filter.WhoDeliveredPersonPersonId {0}", filter.WhoDeliveredPersonPersonId));
                    query = query.Where(o => o.WhoDeliveredPersonPersonId >= filter.WhoDeliveredPersonPersonId);
                }

                #endregion Filter

                query = query.OrderByDescending(o => o.Id).Skip(page.Skip).Take(page.Take);

                db.Database.Log = s => log.Info(String.Format("SQL: {0}", s));
                //var orders = query.ToList();

                //var result = query.ProjectTo<ServiceDataContracts.OrderFlight>().ToList();

                var result = query.Select(order =>

               new ServiceDataContracts.OrderFlight
               {
                   Id = order.Id,

                   AirCompanyId = order.AirCompanyId,
                   Comment = order.Comment,

                   ContactPersonId = order.ContactPersonId,
                   DeliveryDate = order.DeliveryDate,

                   DeliveryPlaceId = order.DeliveryPlaceId,
                   /*
                   Driver = order.Driver == null ? null :
                       new ServiceDataContracts.Driver
                       {
                           Id = order.Driver.Id,
                           FullName = order.Driver.FullName,
                           Phone = order.Driver.Phone
                       },
                       */
                   DriverId = order.DriverId,
                   ExportTime = order.ExportTime,
                   ExtraCharge = order.ExtraCharge,
                   FlightNumber = order.FlightNumber,
                   NumberOfBoxes = order.NumberOfBoxes,
                   OrderComment = order.OrderComment,
                   OrderNumber = order.OrderNumber,
                   OrderStatus = (OrderStatus)order.OrderStatus,
                   PhoneNumber = order.PhoneNumber,
                   ReadyTime = order.ReadyTime,


                   CreatedById = order.CreatedById,

                   SendById = order.SendById,

                   CreationDate = order.CreationDate,

                   DeliveryAddress = order.DeliveryAddress,

                   Code = order.Code,

                   DishPackages = order.DishPackages.Select(pack => new ServiceDataContracts.DishPackageFlightOrder()
                   {

                       Amount = pack.Amount,
                       Code = pack.Code,
                       Comment = pack.Comment,
                       Deleted = pack.Deleted,
                       DeletedStatus = pack.DeletedStatus,
                       SpisPaymentId = pack.SpisPaymentId,
                       Id = pack.Id,
                       DishId = pack.DishId,
                       DishName = pack.DishName,
                       OrderFlightId = pack.OrderFlightId,
                       TotalPrice = pack.TotalPrice,
                       PositionInOrder = pack.PositionInOrder,
                       PassageNumber = pack.PassageNumber


                   }).ToList(),

                   PaymentId = order.PaymentId,

                   DiscountSumm = order.DiscountSumm,
                   Closed = order.Closed,
                   NeedPrintFR = order.NeedPrintFR,
                   NeedPrintPrecheck = order.NeedPrintPrecheck,
                   FRPrinted = order.FRPrinted,
                   PreCheckPrinted = order.PreCheckPrinted,
                   IsSHSent = order.IsSHSent

               }

).ToList();

                log.Info(String.Format("Result successfully created. Collection count: {0}", result.Count));

                return new OperationResultValue<List<ServiceDataContracts.OrderFlight>>
                {
                    Success = true,
                    Result = result
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResultValue<List<ServiceDataContracts.OrderFlight>>
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public OperationResult SetSHValue(bool value, long orderId)
        {
            try
            {
                var order = db.OrderFlight.SingleOrDefault(a => a.Id == orderId);
                if (order != null)
                {
                    order.IsSHSent = value;
                    return new OperationResult() { Success = true, CreatedObjectId = orderId };
                }
                else
                {
                    return new OperationResult() { Success = false, CreatedObjectId = orderId, ErrorMessage = "NoSuchOrder" };
                }

            }
            catch (Exception e)
            {
                return new OperationResult() { Success = false, CreatedObjectId = orderId, ErrorMessage = e.Message };
            }
        }

        public OperationResultValue<List<ServiceDataContracts.OrderFlight>> GetOrderFlightListNeedToFR()
        {
            log.Info("GetOrderFlightListNeedToFR");
            try
            {
                var query = db.OrderFlight.Where(o => o.NeedPrintFR);


                query = query.OrderByDescending(o => o.Id);

                //db.Database.Log = s => log.Info(String.Format("SQL: {0}", s));


                var result = query.Select(order =>

                    new ServiceDataContracts.OrderFlight
                    {
                        Id = order.Id,

                        AirCompanyId = order.AirCompanyId,
                        Comment = order.Comment,

                        ContactPersonId = order.ContactPersonId,
                        DeliveryDate = order.DeliveryDate,

                        DeliveryPlaceId = order.DeliveryPlaceId,

                        Driver = order.Driver == null ? null :
                            new ServiceDataContracts.Driver
                            {
                                Id = order.Driver.Id,
                                FullName = order.Driver.FullName,
                                Phone = order.Driver.Phone
                            },
                        DriverId = order.DriverId,
                        ExportTime = order.ExportTime,
                        ExtraCharge = order.ExtraCharge,
                        FlightNumber = order.FlightNumber,
                        NumberOfBoxes = order.NumberOfBoxes,
                        OrderComment = order.OrderComment,
                        OrderNumber = order.OrderNumber,
                        OrderStatus = (OrderStatus)order.OrderStatus,
                        PhoneNumber = order.PhoneNumber,
                        ReadyTime = order.ReadyTime,


                        CreatedById = order.CreatedById,

                        SendById = order.SendById,

                        CreationDate = order.CreationDate,

                        DeliveryAddress = order.DeliveryAddress,

                        Code = order.Code,
                        /*
                        DishPackages = order.DishPackages.Where(x=>!x.Deleted).Select(pack =>
                            Mapper.Map<Entities.DishPackageFlightOrder, ServiceDataContracts.DishPackageFlightOrder>(pack)
                        ).ToList(),
                        */



                        DishPackages = order.DishPackages.Where(x => !x.Deleted).Select(pack => new ServiceDataContracts.DishPackageFlightOrder()
                        {

                            Amount = pack.Amount,
                            Code = pack.Code,
                            Comment = pack.Comment,
                            Deleted = pack.Deleted,
                            DeletedStatus = pack.DeletedStatus,
                            SpisPaymentId = pack.SpisPaymentId,
                            Id = pack.Id,
                            DishId = pack.DishId,
                            DishName = pack.DishName,
                            OrderFlightId = pack.OrderFlightId,
                            TotalPrice = pack.TotalPrice,
                            PositionInOrder = pack.PositionInOrder,
                            PassageNumber = pack.PassageNumber,


                        }).ToList(),

                        PaymentId = order.PaymentId,

                        DiscountSumm = order.DiscountSumm,
                        Closed = order.Closed,
                        NeedPrintFR = order.NeedPrintFR,
                        NeedPrintPrecheck = order.NeedPrintPrecheck,
                        FRPrinted = order.FRPrinted,
                        PreCheckPrinted = order.PreCheckPrinted,
                        IsSHSent = order.IsSHSent

                    }).ToList();





                foreach (var or in query)
                {
                    or.NeedPrintFR = false;
                }
                db.SaveChanges();
                log.Info($"GetOrderFlightListNeedToFR returned: {result.Count }");
                return new OperationResultValue<List<ServiceDataContracts.OrderFlight>>
                {
                    Success = true,
                    Result = result
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResultValue<List<ServiceDataContracts.OrderFlight>>
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        public byte[] ExportToExcel(long orderId)
        {
            var order = this.GetOrderFlight(orderId);

            var workbook = new Workbook();
            var worksheet = workbook.Worksheets.Add();
            worksheet.Columns[0].SetWidth(new ColumnWidth(15, false));

            int rowIndex = 0;
            int colIndex = 1;

            var border1 = new CellBorder(CellBorderStyle.Thin, ThemableColor.FromArgb(0, 0, 0, 0));

            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Организация:");

            CellIndex c11 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex c22 = new CellIndex(rowIndex, colIndex);
            worksheet.Cells[c11, c22].Merge();
            colIndex++;


            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Gallery TO FLY");

            CellIndex c111 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex c222 = new CellIndex(rowIndex, colIndex);
            CellIndex c333 = new CellIndex(rowIndex, colIndex + 1);
            CellIndex c444 = new CellIndex(rowIndex, colIndex + 1);
            CellIndex c555 = new CellIndex(rowIndex, colIndex + 1);
            worksheet.Cells[c111, c555].Merge();
            worksheet.Cells[c111].SetBorders(new CellBorders(null, null, null, border1, null, null, null, null));
            worksheet.Cells[c222].SetBorders(new CellBorders(null, null, null, border1, null, null, null, null));
            worksheet.Cells[c333].SetBorders(new CellBorders(null, null, null, border1, null, null, null, null));
            worksheet.Cells[c444].SetBorders(new CellBorders(null, null, null, border1, null, null, null, null));
            worksheet.Cells[c555].SetBorders(new CellBorders(null, null, null, border1, null, null, null, null));

            rowIndex += 2;
            colIndex = 3;

            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex].SetIsBold(true);
            worksheet.Cells[rowIndex, colIndex].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex++].SetValue("INVOICE");
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));

            CellIndex cc1 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex cc2 = new CellIndex(rowIndex, colIndex);
            CellIndex cc3 = new CellIndex(rowIndex, colIndex + 1);
            CellIndex cc4 = new CellIndex(rowIndex, colIndex + 1);
            CellIndex cc5 = new CellIndex(rowIndex, colIndex + 1);
            worksheet.Cells[cc1, cc5].Merge();


            rowIndex += 2;
            colIndex = 1;

            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Document" + Environment.NewLine + "Number");
            worksheet.Cells[rowIndex, colIndex - 1].SetIsWrapped(true);
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));

            CellIndex d1 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex d2 = new CellIndex(rowIndex, colIndex);
            worksheet.Cells[d1, d2].Merge();
            colIndex++;

            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("2FLY" + order.Result.Id + "-" + order.Result.FlightNumber);

            rowIndex += 2;
            colIndex = 1;

            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Date");
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));
            CellIndex dd1 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex dd2 = new CellIndex(rowIndex, colIndex);
            worksheet.Cells[dd1, dd2].Merge();
            colIndex++;

            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue(order.Result.DeliveryDate.ToString("dd.MM.yyyy"));

            rowIndex += 2;
            colIndex = 1;

            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Supplier");
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));
            CellIndex s1 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex s2 = new CellIndex(rowIndex, colIndex);
            worksheet.Cells[s1, s2].Merge();
            colIndex++;

            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Gallery TO FLY");

            rowIndex += 2;
            colIndex = 1;

            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Company");
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));
            CellIndex cp1 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex cp2 = new CellIndex(rowIndex, colIndex);
            worksheet.Cells[cp1, cp2].Merge();
            colIndex++;

            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue(order.Result.AirCompany.Name);

            rowIndex += 2;
            colIndex = 1;

            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Comments:");
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));
            CellIndex cm1 = new CellIndex(rowIndex, colIndex - 1);
            CellIndex cm2 = new CellIndex(rowIndex, colIndex);
            worksheet.Cells[cm1, cm2].Merge();
            colIndex++;

            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(120, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue(String.Format("{0}, {1}, {2}, {3}, {4}",
                order.Result.DeliveryPlace.Name,
                order.Result.DeliveryDate.ToString("dd.MM.yyyy"),
                order.Result.OrderTotalSumm + " руб.",
                order.Result.Comment,
                order.Result.PhoneNumber));

            colIndex = 1;
            rowIndex += 1;

            CellIndex c1 = new CellIndex(rowIndex, colIndex);
            worksheet.Cells[c1].SetBorders(new CellBorders(border1, border1, null, border1, null, null, null, null));

            CellIndex c2 = new CellIndex(rowIndex, colIndex + 1);
            worksheet.Cells[c2].SetBorders(new CellBorders(null, border1, null, border1, null, null, null, null));

            CellIndex c3 = new CellIndex(rowIndex, colIndex + 2);
            worksheet.Cells[c3].SetBorders(new CellBorders(null, border1, null, border1, null, null, null, null));

            CellIndex c4 = new CellIndex(rowIndex, colIndex + 3);

            worksheet.Cells[c1, c4].Merge();

            worksheet.Cells[c1].SetValue("Товар");
            worksheet.Cells[c1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[c1].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[c4].SetBorders(new CellBorders(null, border1, border1, border1, null, null, null, null));


            colIndex = 1;
            rowIndex += 1;

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(50, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("№");
            worksheet.Cells[rowIndex, colIndex - 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex - 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Rows[rowIndex].SetHeight(new RowHeight(35, true));

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(50, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Code");
            worksheet.Cells[rowIndex, colIndex - 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex - 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(230, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Items");
            worksheet.Cells[rowIndex, colIndex - 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex - 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(50, false));
            worksheet.Cells[rowIndex, colIndex++].SetValue("Unit");
            worksheet.Cells[rowIndex, colIndex - 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex, colIndex - 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(50, false));
            worksheet.Cells[rowIndex - 1, colIndex++].SetValue("Qnt");
            worksheet.Cells[rowIndex - 1, colIndex - 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex - 1, colIndex - 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            c1 = new CellIndex(rowIndex, colIndex - 1);
            c2 = new CellIndex(rowIndex - 1, colIndex - 1);
            worksheet.Cells[c1, c2].Merge();
            worksheet.Cells[rowIndex - 1, colIndex - 1].SetBorders(new CellBorders(border1, border1, border1, null, null, null, null, null));

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(50, false));
            worksheet.Cells[rowIndex - 1, colIndex++].SetValue("Price");
            worksheet.Cells[rowIndex - 1, colIndex - 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex - 1, colIndex - 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            c1 = new CellIndex(rowIndex, colIndex - 1);
            c2 = new CellIndex(rowIndex - 1, colIndex - 1);
            worksheet.Cells[c1, c2].Merge();
            worksheet.Cells[rowIndex - 1, colIndex - 1].SetBorders(new CellBorders(border1, border1, border1, null, null, null, null, null));

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Columns[colIndex].SetWidth(new ColumnWidth(50, false));
            worksheet.Cells[rowIndex - 1, colIndex].SetValue("Cost");
            worksheet.Cells[rowIndex - 1, colIndex].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[rowIndex - 1, colIndex].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            c1 = new CellIndex(rowIndex, colIndex);
            c2 = new CellIndex(rowIndex - 1, colIndex);
            worksheet.Cells[c1, c2].Merge();
            worksheet.Cells[rowIndex - 1, colIndex].SetBorders(new CellBorders(border1, border1, border1, null, null, null, null, null));
            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));


            var number = 0;

            decimal totalQnt = 0;
            decimal totalPrice = 0;
            decimal totalSumm = 0;

            foreach (var package in order.Result.DishPackages)
            {
                number++;
                colIndex = 1;
                rowIndex++;

                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue(number.ToString());

                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue(package.Dish.Barcode);

                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue(package.Dish.EnglishName);

                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue("порц");

                totalQnt += package.Amount;
                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue(package.Amount.ToString());

                totalPrice += package.TotalPrice;
                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue(package.TotalPrice.ToString());

                totalSumm += package.TotalSumm;
                worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
                worksheet.Cells[rowIndex, colIndex++].SetValue(package.TotalSumm.ToString());
            }

            rowIndex += 1;

            worksheet.Cells[rowIndex, 1].SetBorders(new CellBorders(border1, border1, null, border1, null, null, null, null));
            worksheet.Cells[rowIndex, 2].SetBorders(new CellBorders(null, border1, null, border1, null, null, null, null));
            worksheet.Cells[rowIndex, 3].SetBorders(new CellBorders(null, border1, null, border1, null, null, null, null));


            colIndex = 4;

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(null, border1, border1, border1, null, null, null, null));
            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Cells[rowIndex, colIndex++].SetValue("TOTAL");

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Cells[rowIndex, colIndex++].SetValue(totalQnt.ToString());

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Cells[rowIndex, colIndex++].SetValue(totalPrice.ToString());

            worksheet.Cells[rowIndex, colIndex].SetBorders(new CellBorders(border1, border1, border1, border1, null, null, null, null));
            worksheet.Cells[rowIndex, colIndex].SetUnderline(UnderlineType.Single);
            worksheet.Cells[rowIndex, colIndex++].SetValue(totalSumm.ToString());

            var formatProvider = new XlsxFormatProvider();
            return formatProvider.Export(workbook);
        }

        public OperationResultValue<decimal> GetSumByDates(DateTime dateFrom, DateTime dateTo, OrderStatus status)
        {
            try
            {
                var orders = this.GetOrderFlightList(
                    new OrderFlightFilter
                    {
                        OrderStatus = status,
                        DeliveryDateStart = dateFrom,
                        DeliveryDateEnd = dateTo
                    }, new PageInfo { Skip = 0, Take = 9999999 }
                    );

                decimal sum = 0;

                foreach (var order in orders.Result)
                {
                    if (order.DishPackages == null)
                    {
                        continue;
                    }
                    if (order.OrderStatus == OrderStatus.Cancelled)
                    {
                        continue;
                    }

                    sum += order.DishPackages.Sum(a => a.TotalSumm) * (1 + order.ExtraCharge / 100);
                }

                return new OperationResultValue<decimal>
                {
                    Result = sum,
                    Success = true
                };
            }
            catch (Exception e)
            {
                log.Error("Error", e);
                return new OperationResultValue<decimal>
                {
                    Success = false,
                    ErrorMessage = e.Message,

                };
            }
        }

    }
}