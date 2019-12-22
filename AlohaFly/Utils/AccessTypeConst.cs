using System.Collections.Generic;

namespace AlohaFly.Utils
{
    public static class AccessTypeConst
    {
        /*
        public const long Access_OrderFly= 21;
        public const long Access_OrderToGo = 22;

        public const long Access_CatalogAirCompany = 31;
        public const long Access_CatalogDish = 32;
        public const long Access_CatalogDeliveryPlace = 33;
        public const long Access_CatalogDriver = 34;
        public const long Access_CatalogContactPerson = 35;
        public const long Access_CatalogKitchenGroup = 36;
        public const long Access_CatalogLogicGroup = 37;

        public const long Access_DishLabels = 50;

        public const long Access_OrdersToFly = 100;
        public const long Access_AddOrdersToFly = 101;

        public const long Access_Reports_Rep1 = 200;
        public const long Access_Reports_Rep2 = 201;

    */


        public const long Access_OrdersToFly = 1;
        public const long Access_OrdersToGo = 2;

        public const long Access_CatalogAirCompany = 3;
        public const long Access_CatalogDish = 4;
        public const long Access_CatalogDeliveryPlace = 5;
        public const long Access_CatalogDriver = 6;
        public const long Access_CatalogContactPerson = 7;
        public const long Access_CatalogKitchenGroup = 8;
        public const long Access_CatalogLogicGroup = 9;

        public const long Access_DishLabels = 10;
        public const long Access_ChangeMyPass = 11;

        public const long Access_ToGoClients = 12;
        public const long Access_MarketingChanels = 13;


        public const long Access_OrdersToFlyAirComps = 14;
        public const long Access_OrdersSpis = 15;

        public const long Access_ImportToFlyRkeeper = 16;

        public const long Access_Analytics1 = 301;
        public const long Access_Analytics2 = 302;

        //public const long Access_OrdersToFly = 100;
        //public const long Access_OrdersToGo = 103;
        public const long Access_AddOrdersToFly = 101;
        public const long Access_AddOrdersToGo = 102;
        public const long Access_OrdersNonSH = 104;

        public const long Access_Reports_Rep1 = 200;
        public const long Access_Reports_Rep2 = 201;


        public static List<long> NonAdminFuncs = new List<long>() {
            Access_Analytics1,
            Access_Analytics2,
        };
        public static List<long> NonDirectorFuncs = new List<long>() { };
    }

    public enum FuncAccessTypeEnum : int
    {
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Disable = -1,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        View = 0,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Edit = 1,
    }
}
