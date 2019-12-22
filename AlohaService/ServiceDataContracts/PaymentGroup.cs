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
    public class PaymentGroup : INotifyPropertyChanged
    {
        [DataMember]
        [Display(Name = "Id")]
        public long Id { get; set; }

        [DataMember]
        [Display(Name = "Код платежа")]
        public int Code { get; set; }

        [DataMember]
        [Display(Name = "Наименование")]
        public string Name { get; set; }

       
        [DataMember]
        [Display(Name = "Активно")]
        public bool IsActive { get; set; } = true;

        [DataMember]
        [Display(Name = "Выручка")]
        public bool Sale { get; set; } = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return Name;
        }
    }
}