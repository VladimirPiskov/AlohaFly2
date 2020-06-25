using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace StoreHouseConnect
{
    public class TParametrForPrice
    {
        [DataMember]
        public DateTime DTStart { set; get; }
        [DataMember]
        public DateTime DTEnd { set; get; }
        [DataMember]
        public int Rid { set; get; }
    }
}
