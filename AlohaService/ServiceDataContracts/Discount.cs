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

    public class Discount:INotifyPropertyChanged
    {
        [DataMember]
        [Display(Name = "Id")]
        public long Id { get; set; }

        [DataMember]
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [DataMember]
        [Display(Name = "Для ToGo")]
        public bool ToGo { get; set; } = false;

        [DataMember]
        [Display(Name = "Список диапазонов")]
        public List<DiscountRange> Ranges { get; set; }


        [DataMember]
        public Guid LastUpdatedSession { get; set; }



        public event PropertyChangedEventHandler PropertyChanged;
    }
}