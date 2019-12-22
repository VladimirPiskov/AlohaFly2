using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlohaFlyAdmin
{
    public static class MainClass
    {
        public static void InitUserFuncs()
        {
            var Funcs = AlohaFly.DBProvider.GetAllFuncs();

        }


    }
}
