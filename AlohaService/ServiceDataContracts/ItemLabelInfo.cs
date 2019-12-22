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
    public class ItemLabelInfo : INotifyPropertyChanged
    {
        [DataMember]
        [Display(Name = "Id")]
        public long Id { get; set; }

        [DataMember]
        [Display(Name="ID родительского блюда",AutoGenerateField =false)]
        public long ParenItemId { get; set; }

        [DataMember]
        [Display(Name ="Порядковый номер на листе")]
        public long SerialNumber { get; set; }

        [DataMember]
        [Display(Name = "Русскоязычное наименование")]
        public string  NameRus { get; set; }    

        [DataMember]
        [Display(Name = "Англоязычное наименование")]
        public string NameEng { get; set; }

        [DataMember]
        [Display(Name = "Сообщение")]
        public string  Message { get; set; }

        [DataMember]
        [Display(AutoGenerateField = false)]
        public virtual Dish Dish { get; set; }

        #region INotifyPropertyChanged we do use Foody https://github.com/Fody/PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

    }
}