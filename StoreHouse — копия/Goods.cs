using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace StoreHouseConnect
{
    [DataContract]
    public class TGoods
    {
        [DataMember]
        public int Rid { set; get; }
        [DataMember]
        public string prCode { set; get; }
        [DataMember]
        public string numCode { set; get; }
        [DataMember]
        public string Name { set; get; }
        [DataMember]
        public int Parent { set; get; }
        [DataMember]
        public int RidUnit { set; get; }
        [DataMember]
        public string Unit { set; get; }
        [DataMember]
        public int Price { set; get; }
        [DataMember]
        public string en { set; get; }
        [DataMember]
        public string ru { set; get; }
        [DataMember]
        public int Type { set; get; }
    }
}

