using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace StoreHouseConnect
{
    [DataContract]
    public class TComplectList
    {
        [DataMember]
        public List<TComplect> ListComplect { set; get; }
    }
}
