using AlohaService.Helpers;
using AlohaService.Interfaces;
using AlohaService.ServiceDataContracts;
using AutoMapper;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AlohaService.BusinessServices
{
    public class UpdaterService
    {
        private AlohaDb db;
        protected ILog log;

        public UpdaterService(AlohaDb databaseContext)
        {
            db = databaseContext;
            LogHelper.Configure();
            log = LogHelper.GetLogger();
        }

        public OperationResultValue<UpdateResult> GetServerTime()
        {
            var res = new OperationResultValue<UpdateResult>();
            res.Result = new UpdateResult();
            res.Result.UpdatesTime = DateTime.Now;
            return res;
        }


        public OperationResultValue<UpdateResult> GetUpdatesForSessionTest()
        {
            var res = new OperationResultValue<UpdateResult>();
            res.Result = new UpdateResult();
            try
            {
                //log.Debug($"GetUpdatesForSession sessionId: {sessionId}; lastUpdateTime:{lastUpdateTime.ToShortDateString()}");
                var result = db.OrderToGo.Where(o => o.Id == 13280).ToList();
                List<ServiceDataContracts.OrderToGo> resultOut = Mapper.Map<List<Entities.OrderToGo>, List<ServiceDataContracts.OrderToGo>>(result);

                log.Debug($"resultOut resultOut[0].DishPackages:{resultOut[0].DishPackages == null}");
                log.Debug($"resultOut resultOut[0].DishPackages.Count():{resultOut[0].DishPackages.Count()}");
                log.Debug($"resultOut resultOut[0].DishPackages.Count():{resultOut[0].DishPackages[0].DishName}");
                log.Debug($"resultOut resultOut[0].DishPackages[0].Dish==null:{resultOut[0].DishPackages[0].Dish == null}");
                log.Debug($"resultOut resultOut[0].DishPackages[0].OrderToGo==null:{resultOut[0].DishPackages[0].OrderToGo == null}");

                res.Result.OrderToGos = resultOut;
                /*
                log.Debug($"ExportTime:{resultOut[0].ExportTime.ToShortDateString()}");
                log.Debug($"ReadyTime:{resultOut[0].ReadyTime.ToShortDateString()}");
                log.Debug($"DeliveryDate:{resultOut[0].DeliveryDate.ToShortDateString()}");
                log.Debug($"LastUpdatedSession:{resultOut[0].LastUpdatedSession}");
                */
                //res.Result.OrderToGos = new List<OrderToGo>();
                //res.Result.OrderToGos.Add(new OrderToGo() {DishPackages = new List<DishPackageToGoOrder>() {new DishPackageToGoOrder () } });
                //resultOut.ForEach(a => { a.DishPackages = new List<DishPackageToGoOrder>(); res.Result.OrderToGos.Add(a); });
                //resultOut.ForEach(a => { a.DishPackages = new List<DishPackageToGoOrder>(); res.Result.OrderToGos.Add(a); });
                log.Debug($"GetUpdatesForSession res.Result.OrderToGos :{res.Result.OrderToGos.Count()}");
                res.Success = true;

            }
            catch (Exception e)
            {
                log.Error("GetUpdatesForSession error ", e);
                res.Success = false;
                res.ErrorMessage = e.Message;
            }

            return res;
        }

        public List<TDestination> GetUpdatedData<TSource, TDestination>(DbSet<TSource> table,DateTime lastUpdateTime, Guid session,DateTime? startData=null)
            where TSource : class,Interfaces.IRealTimeUpdater,new()
            where TDestination: class,new()
        {
            TSource t = new TSource();
            try
            {
                
                TDestination d = new TDestination();

                var qRes = table.Where(o => o.UpdatedDate != null && o.UpdatedDate > lastUpdateTime && o.LastUpdatedSession != session);
                if (startData != null )
                {
                    if (t is Entities.OrderToGo)
                    {
                        IQueryable<Entities.OrderToGo> qResType = (qRes as IQueryable<Entities.OrderToGo>).Where(o => o.DeliveryDate >= startData);
                        qRes = qResType as IQueryable<TSource>;
                    }
                    if (t is Entities.OrderFlight)
                    {
                        IQueryable<Entities.OrderFlight> qResType = (qRes as IQueryable<Entities.OrderFlight>).Where(o => o.DeliveryDate >= startData);
                        qRes = qResType as IQueryable<TSource>;
                    }
                }

                 var res = qRes.ToList();

                log.Debug($"GetUpdatedData TSource: {t.GetType().ToString()}; resCount:{res?.Count()} ");
                var m = Mapper.Map<List<TSource>, List<TDestination>>(res);
                
                if (m != null)
                {
                    log.Debug($"GetUpdatedData TDestination: {m.GetType().ToString()}; resCount:{m?.Count()} ");
                    foreach (var el in m)
                    {
                        if (el == null)
                        {
                            log.Debug($"Next element is null");
                        }
                        log.Debug($"Next element:");
                        foreach (var p in el.GetType().GetProperties())
                        {
                            log.Debug($"prop: {p.Name}; val: {p.GetValue(el)?.ToString()}");
                        }
                    }
                }

                    return m;
            }
            catch(Exception e)
            {
                log.Debug($"Error GetUpdatedData TSource: {t.GetType().ToString()}; mess:{e.Message} ");
                return new List<TDestination>();
            }
        }

        public OperationResultValue<UpdateResult> GetUpdatesForSession(DateTime lastUpdateTime, DateTime DataTime, Guid session)
            {

            var res = new OperationResultValue<UpdateResult>();
            res.Result = new UpdateResult();
            try
            {
                //log.Debug($"GetUpdatesForSession sessionId: {sessionId}; lastUpdateTime:{lastUpdateTime.ToShortDateString()}");
                
                DateTime updTime = DateTime.Now;
                res.Result.UpdatesTime = updTime;
                log.Debug($"GetUpdatesForSession lastUpdateTime:{lastUpdateTime.ToString()}; DataTime:{DataTime.ToString()}; session:{session}");
                try
                {



                    /*
                    var resultAdr = db.OrderCustomerAddresses.Where(o => o.UpdatedDate != null && o.UpdatedDate > lastUpdateTime && o.LastUpdatedSession != session).ToList();
                    res.Result.OrderCustomerAddresss = Mapper.Map<List<Entities.OrderCustomerAddress>, List<ServiceDataContracts.OrderCustomerAddress>>(resultAdr);
                    */


                    res.Result.Users = GetUpdatedData<Entities.User, ServiceDataContracts.User>(db.Users, lastUpdateTime, session);
                    res.Result.ContactPerson = GetUpdatedData<Entities.ContactPerson, ServiceDataContracts.ContactPerson>(db.ContactPersons, lastUpdateTime, session);
                    res.Result.DishLogicGroup = GetUpdatedData<Entities.DishLogicGroup, ServiceDataContracts.DishLogicGroup>(db.DishLogicGroups, lastUpdateTime, session);
                    res.Result.DishKitchenGroup = GetUpdatedData<Entities.DishKitchenGroup, ServiceDataContracts.DishKitchenGroup>(db.DishKitchenGroups, lastUpdateTime, session);
                    res.Result.PaymentGroup = GetUpdatedData<Entities.PaymentGroup, ServiceDataContracts.PaymentGroup>(db.PaymentGroups, lastUpdateTime, session);
                    res.Result.Payment = GetUpdatedData<Entities.Payment, ServiceDataContracts.Payment>(db.Payments, lastUpdateTime, session);
                    res.Result.Discount = GetUpdatedData<Entities.Discount, ServiceDataContracts.Discount>(db.Discounts, lastUpdateTime, session);
                    res.Result.Driver = GetUpdatedData<Entities.Driver, ServiceDataContracts.Driver>(db.Driver, lastUpdateTime, session);
                    res.Result.DeliveryPlace = GetUpdatedData<Entities.DeliveryPlace, ServiceDataContracts.DeliveryPlace>(db.DeliveryPlace, lastUpdateTime, session);
                    res.Result.AirCompany = GetUpdatedData<Entities.AirCompany, ServiceDataContracts.AirCompany>(db.AirCompanies, lastUpdateTime, session);
                    res.Result.Dish = GetUpdatedData<Entities.Dish, ServiceDataContracts.Dish>(db.Dish, lastUpdateTime, session);

                    res.Result.ItemLabelInfo = GetUpdatedData<Entities.ItemLabelInfo, ServiceDataContracts.ItemLabelInfo>(db.ItemLabelInfos, lastUpdateTime, session);
                    res.Result.MarketingChannel = GetUpdatedData<Entities.MarketingChannel, ServiceDataContracts.MarketingChannel>(db.MarketingChannel, lastUpdateTime, session);



                    res.Result.OrderCustomerAddresss = GetUpdatedData<Entities.OrderCustomerAddress, ServiceDataContracts.OrderCustomerAddress>(db.OrderCustomerAddresses, lastUpdateTime,session);

                    var resultPh = db.OrderCustomerPhones.Where(o => o.UpdatedDate != null && o.UpdatedDate > lastUpdateTime && o.LastUpdatedSession != session).ToList();
                    res.Result.OrderCustomerPhones = Mapper.Map<List<Entities.OrderCustomerPhone>, List<ServiceDataContracts.OrderCustomerPhone>>(resultPh);


                    var resultOc = db.OrderCustomers.Where(o => o.UpdatedDate != null && o.UpdatedDate > lastUpdateTime && o.LastUpdatedSession != session).ToList();
                    res.Result.OrderCustomers = Mapper.Map<List<Entities.OrderCustomer>, List<ServiceDataContracts.OrderCustomer>>(resultOc);


                    res.Result.OrderToGos = GetUpdatedData<Entities.OrderToGo, ServiceDataContracts.OrderToGo>(db.OrderToGo, lastUpdateTime, session, DataTime);
                    res.Result.OrderFlight = GetUpdatedData<Entities.OrderFlight, ServiceDataContracts.OrderFlight>(db.OrderFlight, lastUpdateTime, session, DataTime);


                    /*
                    var result = db.OrderToGo.Where(o => o.DeliveryDate >= DataTime && o.UpdatedDate != null && o.UpdatedDate > lastUpdateTime && o.LastUpdatedSession!=session).ToList();
                    List<ServiceDataContracts.OrderToGo> resultOut = Mapper.Map<List<Entities.OrderToGo>, List<ServiceDataContracts.OrderToGo>>(result);
                    res.Result.OrderToGos = resultOut;
                    var resultOF = db.OrderFlight.Where(o => o.DeliveryDate >= DataTime && o.UpdatedDate != null && o.UpdatedDate > lastUpdateTime && o.LastUpdatedSession != session).ToList();
                    res.Result.OrderFlight = Mapper.Map<List<Entities.OrderFlight>, List<ServiceDataContracts.OrderFlight>>(resultOF);
                    */



                    log.Debug($"GetUpdatesForSession res.Result.OrderToGos :{res.Result.OrderToGos.Count()}");

                    res.Success = true;
                }
                catch (Exception e)
                {
                    log.Error($"GetOrderToGoUpdates error {e.Message}");
                }
            }
            catch (Exception e)
            {
                log.Error("GetUpdatesForSession error ", e );
                res.Success = false;
                res.ErrorMessage = e.Message;
            }

            return res;
        }


    }
}