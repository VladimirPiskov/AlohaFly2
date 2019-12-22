using System;
using System.Collections.Generic;

namespace AlohaFly
{
    public static class LabelPrint
    {
    }


    public class OrderPrintInfo
    {
        public List<LabelPrintInfo> Labels { set; get; }
        public string BoardName { set; get; }
        public long OrderNumber { set; get; }
        public DateTime PrepearingTime { set; get; }
        public DateTime OrderTime { set; get; }
        public int OrderType { set; get; } //Тип наклейки 0 (To Fly) или 1(To Go). От этого зависит лого.



    }

    public class LabelPrintInfo
    {
        public long Order { set; get; }
        public long BarCode { set; get; }
        public string Comment { set; get; }
        public string ItemName1 { set; get; }
        public string ItemName2 { set; get; }
        public string CountStr { set; get; }
        public string SubItemName1 { set; get; }
        public string SubItemmName2 { set; get; }


    }
}
