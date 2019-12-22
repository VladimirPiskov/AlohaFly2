using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Print
{
    [DataContract]
    public class OrderPrintInfo
    {
        [DataMember]
        public List<LabelPrintInfo> Labels { set; get; } //Список наклеек
        [DataMember]
        public string BoardName { set; get; }
        [DataMember]
        public long OrderNumber { set; get; }
        [DataMember]
        public DateTime PrepearingTime { set; get; }
        [DataMember]
        public DateTime OrderTime { set; get; }
        [DataMember]
        public int OrderType { set; get; } //Тип наклейки 0 (To Fly) или 1(To Go). От //этого зависит лого.

    }
}
