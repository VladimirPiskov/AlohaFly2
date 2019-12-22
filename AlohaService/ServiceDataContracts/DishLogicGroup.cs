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
    public class DishLogicGroup : INotifyPropertyChanged
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [DataMember]
        [Display(Name = "Английское наименование")]
        public string EnglishName { get; set; }

        [DataMember]
        [Display(Name = "Позиция при печати")]
        public long PositionForPrint { get; set; }

        [DataMember]
        [Display(Name = "Активно")]
        public bool IsActive { get; set; } = true;

        public override string ToString()
        {
            return Name;
        }

        #region INotifyPropertyChanged we do use Foody https://github.com/Fody/PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}