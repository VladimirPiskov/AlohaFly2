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
    public class DeliveryPlace : INotifyPropertyChanged
    {
        [DataMember]
        [Display(Name = "Id   ")]
        public long Id { get; set; }

        [DataMember]
        [Display(Name = "Наименование      ")]
        public string Name { get; set; }

        [DataMember]
        [Display(Name = "Телефон            ", AutoGenerateField =false)]
        public string Phone { get; set; }

        [DataMember]
        [Display(Name = "Активно")]
        public bool IsActive { get; set; } = true;

        #region INotifyPropertyChanged we do use Foody https://github.com/Fody/PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}