using AlohaService.Helpers;
using AlohaService.Interfaces;
using AlohaService.ServiceDataContracts;
using AutoMapper;
using log4net;
using System;
using System.Collections.Generic;
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
            res.Result.UpdatesTime =  DateTime.Now;
            return res;
        }


        public OperationResultValue<UpdateResult> GetUpdatesForSessionTest()
        {
            var res = new OperationResultValue<UpdateResult>();
            res.Result = new UpdateResult();
            try
            {
                //log.Debug($"GetUpdatesForSession sessionId: {sessionId}; lastUpdateTime:{lastUpdateTime.ToShortDateString()}");
                var result = db.OrderToGo.Where(o => o.Id==13280).ToList();
                List<ServiceDataContracts.OrderToGo> resultOut = Mapper.Map<List<Entities.OrderToGo>, List<ServiceDataContracts.OrderToGo>>(result);
                
                log.Debug($"resultOut resultOut[0].DishPackages:{resultOut[0].DishPackages==null}");
                log.Debug($"resultOut resultOut[0].DishPackages.Count():{resultOut[0].DishPackages.Count()}");
                log.Debug($"resultOut resultOut[0].DishPackages.Count():{resultOut[0].DishPackages[0].DishName}");
                log.Debug($"resultOut resultOut[0].DishPackages[0].Dish==null:{resultOut[0].DishPackages[0].Dish==null}");
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

                    var resultAdr = db.OrderCustomerAddresses.Where(o => o.UpdatedDate != null && o.UpdatedDate > lastUpdateTime && o.LastUpdatedSession != session).ToList();
                    res.Result.OrderCustomerAddresss = Mapper.Map<List<Entities.OrderCustomerAddress>, List<ServiceDataContracts.OrderCustomerAddress>>(resultAdr);

                    var resultPh = db.OrderCustomerPhones.Where(o => o.UpdatedDate != null && o.UpdatedDate > lastUpdateTime && o.LastUpdatedSession != session).ToList();
                    res.Result.OrderCustomerPhones = Mapper.Map<List<Entities.OrderCustomerPhone>, List<ServiceDataContracts.OrderCustomerPhone>>(resultPh);


                    var resultOc = db.OrderCustomers.Where(o => o.UpdatedDate != null && o.UpdatedDate > lastUpdateTime && o.LastUpdatedSession != session).ToList();
                    res.Result.OrderCustomers = Mapper.Map<List<Entities.OrderCustomer>, List<ServiceDataContracts.OrderCustomer>>(resultOc);

                    var result = db.OrderToGo.Where(o => o.DeliveryDate >= DataTime && o.UpdatedDate != null && o.UpdatedDate > lastUpdateTime && o.LastUpdatedSession!=session).ToList();
                    List<ServiceDataContracts.OrderToGo> resultOut = Mapper.Map<List<Entities.OrderToGo>, List<ServiceDataContracts.OrderToGo>>(result);
                    res.Result.OrderToGos = resultOut;
                    //resultOut.ForEach(a => res.Result.OrderToGos.Add(a));
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