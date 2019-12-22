using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace StoreHouseConnect
{
    public class TInvoice
    {
        [DataMember]
        public string RID { set; get; }
        [DataMember]
        public DateTime Date { set; get; }
        [DataMember]
        public int RidProvider { set; get; }
        [DataMember]
        public string NameProvider { set; get; }
        [DataMember]
        public List<TGoods> ListGoods { set; get; }
    }
}



