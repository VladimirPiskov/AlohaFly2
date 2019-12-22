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
    public class Alert : INotifyPropertyChanged
    {
        [DataMember]
        [Display(Name = "№")]
        public long Id { get; set; }

        [DataMember]
        [Display(Name = "Сообщение")]
        public string Message { get; set; }

        [DataMember]
        [Display(Name = "Начало события")]
        public DateTime Start { get; set; }

        [DataMember]
        [Display(Name = "Окончание события")]
        public DateTime End { get; set; }

        [DataMember]
        [Display(Name = "Период напоминания")]
        public int Period { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }



}