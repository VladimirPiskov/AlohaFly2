using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreHouseConnect;
using NLog;

namespace AlohaFly.SH
{
    public static class SHWrapperExt
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        static TStoreHouse sh;
        static public TStoreHouse ConnectSH()
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

        public static void Disconnect()
        {
            sh.CloseConection();
            
        }

        static public double GetSHSebes(int rID,DateTime sDt, DateTime eDt)
        
        {
            try
            {
                logger.Debug($"GetSHSebes {rID}");
                ConnectSH();
                var res =  sh.GetPriceGoods(rID, sDt, eDt, out int errCode, out string errCodeStr);
                if (errCode != 0)
                {
                    logger.Error($"GetSHSebes {rID} Code: {errCode}; {errCodeStr}");
                    return 0;
                }
                Disconnect();
                return res; 

            }
            catch(Exception e)
            {
                logger.Error($"GetSHSebes {rID} : {e.Message}");
                return 0;
            }

        }

    }
}
