using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace StoreHouseConnect
{
    [DataContract]
    public class TGoodsTree
    {
        [DataMember]
        public List<TTreeItem> ListGoodsTree { set; get; } 
    }
}
