using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AlohaService.ServiceDataContracts
{
    [DataContract]
    public class MarketingChannel : INotifyPropertyChanged
    {
        [DataMember]
        [Display(Name = "Внутренний идентификатор")]
        public long Id { get; set; }

        [DataMember]
        [Display(Name = "Наеменование")]
        public string Name { get; set; }


        [DataMember]
        [Display(Name = "Наеменование")]
        public bool IsActive { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}