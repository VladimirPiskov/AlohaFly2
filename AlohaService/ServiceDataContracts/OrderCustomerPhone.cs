using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AlohaService.Interfaces;

namespace AlohaService.ServiceDataContracts
{
    public class OrderCustomerPhone : INotifyPropertyChanged, ICloneable, IPrimaryUnik, IFocusable
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
        [DataMember]
        public bool IsPrimary { get; set; }
        [DataMember]
        public long OrderCustomerId { get; set; }
        [DataMember]
        public Guid LastUpdatedSession { get; set; }
        public bool NeedUpdate { get; set; }
        public bool IsFocused { get; set; } = false;





        public event PropertyChangedEventHandler PropertyChanged;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}