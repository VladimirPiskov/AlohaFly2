using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace StoreHouseConnect
{
    [DataContract]
    public class TTreeItem
    {
        [DataMember]
        public int Rid { set; get; }
        [DataMember]
        public string Name { set; get; }
        [DataMember]
        public int Parent { set; get; }
    }
}
