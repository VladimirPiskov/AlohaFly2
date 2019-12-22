using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AlohaFly.DataExtension
{
    class RealTimeUpdaterSingleton
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        private RealTimeUpdaterSingleton()
        {

        }

        static RealTimeUpdaterSingleton instance;
        public static RealTimeUpdaterSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RealTimeUpdaterSingleton();
                }
                return instance;
            }
        }

        Thread wThread;
        public void Init()
        {
            //dataDate = _dataDate;
            Transaction = Guid.NewGuid();

            var res = DBProvider.Client.GetServerTime();
            lastUpdatetime = res.Result.UpdatesTime;
            /*
            if (res.Success)
            {
                lastUpdatetime = res.Result.UpdatesTime;
            }
            else
            {
                MainClass.SetNeedExit($"Ошибка доступа к базе {res.ErrorMessage}");
            }

            */

        }

        public void StartQueue()
        {
            wThread = new Thread(UpdateDataQueue);
            wThread.Start();
        }

        public void StopQueue()
        {
            endQueue = true;
        }

        bool endQueue = false;
        int interval = 20;
        public DateTime dataDate

        {
            get {
                return DataCatalogsSingleton.Instance.OrdersToGoData.startDate.GetValueOrDefault();
            }
        }
        DateTime lastUpdatetime;
        public Guid Transaction;
        private void UpdateDataQueue()
        {
            while (!endQueue)
            {
                UpdateData();
                for (int i = 0; i < interval; i++)
                {
                    if (endQueue) { return;}
                    Thread.Sleep(1000);
                }
            }
        }



        public  void UpdateData()
        {
            try
            {
                //var res = DBProvider.Client.GetUpdatesForSessionTest();
                
                var res = DBProvider.Client.GetUpdatesForSession(lastUpdatetime, dataDate, Transaction);
                if (res.Success)
                {
                    lastUpdatetime = res.Result.UpdatesTime;

                   
                    DataCatalogsSingleton.Instance.OrderCustomerAddressData.UpdateItems(res.Result.OrderCustomerAddresss);
                    DataCatalogsSingleton.Instance.OrderCustomerPhoneData.UpdateItems(res.Result.OrderCustomerPhones);
                    DataCatalogsSingleton.Instance.OrderCustomerData.UpdateItems(res.Result.OrderCustomers);
                    DataCatalogsSingleton.Instance.OrdersToGoData.UpdateItems(res.Result.OrderToGos);
                }
                
            }
            catch(Exception e)
            {
                logger.Error($"RealTimeUpdaterSingleton UpdateData {e.Message}");
            }

        }


    }
}
