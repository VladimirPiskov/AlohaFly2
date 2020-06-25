using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace StoreHouseConnect
{
    [DataContract]
    public class TComplect
    {
        [DataMember]
        public int Rid { set; get; }
        [DataMember]
        public string CodeStr { set; get; }
        [DataMember]
        public string CodeNum { set; get; }
        [DataMember]
        public string Name { set; get; }
        [DataMember]
        public int UnitId { set; get; }
        [DataMember]
        public string UnitName { set; get; }
        [DataMember]
        public List<TItemComplect> ItemList { set; get; }
    }
}

