using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Print
{
    [DataContract]
    public class LabelPrintInfo
    {
        [DataMember]
        public long Order { set; get; } //Порядковый номер наклейки. Далее все свойства 
        [DataMember]
        public long BarCode { set; get; } // указаны в порядке печати
        [DataMember]
        public string Comment { set; get; }
        [DataMember]
        public string ItemName1 { set; get; }
        [DataMember]
        public string ItemName2 { set; get; }
        [DataMember]
        public string CountStr { set; get; }
        [DataMember]
        public string SubItemName1 { set; get; }
        [DataMember]
        public string SubItemmName2 { set; get; }

    }
}
