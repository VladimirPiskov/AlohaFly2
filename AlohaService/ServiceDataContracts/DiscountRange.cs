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
    public class DiscountRange : INotifyPropertyChanged
    {
        [DataMember]
        [Display(Name = "Id")]
        public long Id { get; set; }

        [DataMember]
        [Display(Name = "Начало диапазона")]
        public long Start { get; set; }

        [DataMember]
        [Display(Name = "Конец диапазона (Nulable!)")]
        public long? End { get; set; }

        [DataMember]
        [Display(Name = "Скидка для данного даипазона")]
        public decimal DiscountPercent { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}