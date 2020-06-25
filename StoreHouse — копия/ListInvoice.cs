using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace StoreHouseConnect
{
    public class TListInvoice
    {
        [DataMember]
        public List<TInvoice> ListInvoice { set; get; }
    }
}
