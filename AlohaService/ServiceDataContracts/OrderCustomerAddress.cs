using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

using System.ComponentModel.DataAnnotations;
using AlohaService.Interfaces;

namespace AlohaService.ServiceDataContracts
{
    public class OrderCustomerAddress: INotifyPropertyChanged,ICloneable, IPrimaryUnik, IFocusable
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public long OldId { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public string MapUrl { get; set; } = "";
        [DataMember]
        public string SubWay { get; set; } = "";
        [DataMember]
        public string Comment { get; set; } = "";
        [DataMember]
        public long ZoneId { get; set; } = 0;
        [DataMember]
        public bool IsActive { get; set; }
        [DataMember]
        public bool IsPrimary { get; set; }
        [DataMember]
        public long OrderCustomerId { get; set; }
        [DataMember]
        public Guid LastUpdatedSession { get; set; }
        public bool NeedUpdate { get; set; } = false;
        public bool IsFocused { get; set; } = false;

        public string AddressExt
        {
            get {
                return Address + (!string.IsNullOrWhiteSpace(SubWay) ? $"; Метро {SubWay}" : "") +$"; Зона {ZoneId}"+ (!string.IsNullOrWhiteSpace(Comment) ? $"; ({Comment})" : "");
            }
        }

        public override string ToString()
        {

            return Address + (!string.IsNullOrWhiteSpace(SubWay) ? $"; Метро {SubWay}":"" ) + (!string.IsNullOrWhiteSpace(Comment)? $"; ({Comment})" : "");
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}