using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace StoreHouseConnect
{
    public class TExpCtgs
    {
        [DataMember]
        public int Rid { set; get; }
        [DataMember]
        public string Name { set; get; }
    }
}
