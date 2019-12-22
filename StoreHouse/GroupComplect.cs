using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace StoreHouseConnect
{
    [DataContract]
    public class TGroupComplect
    {
        [DataMember]
        public int RID { set; get; }
        [DataMember]
        public int Code { set; get; }
        [DataMember]
        public string Name { set; get; }
    }
}
