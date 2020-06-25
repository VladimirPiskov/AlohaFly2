using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace StoreHouseConnect
{
    public class TExpenceDocument
    {
        [DataMember]
        public string Prefix { set; get; }
        [DataMember]
        public int DocNum { set; get; }
        [DataMember]
        public DateTime Date { set; get; }
        [DataMember]
        public int RidPlace { set; get; }
        [DataMember]
        public int CatExpence { set; get; }
        [DataMember]
        public string Coment { set; get; }
        [DataMember]
        public int Type { set; get; } //0 - учет, 1- спец.учет
        [DataMember]
        public List<TItemDocument> ListItemDocument { set; get; }
    }
}


