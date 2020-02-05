using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

using System.ComponentModel;

namespace AlohaService.ServiceDataContracts
{
    [DataContract]
    public class ContactPerson: INotifyPropertyChanged
    {
        [DataMember]
        [Display(Name = "Внутренний идентификатор")]
        public long Id { get; set; }

        [DataMember]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [DataMember]
        [Display(Name = "Фамилия")]
        public string SecondName { get; set; }

        
        [Display(Name = "Имя полностью")]
        public string FullName
        {
            get
            {
                return $"{FirstName} {SecondName}";
            }
        }

        [Display(Name = "Поле поиска" , AutoGenerateField =false)]
        public string FullSearchData
        {
            get
            {
                return $"{FirstName} {SecondName} {Phone} {Email}";
            }
        }

        [DataMember]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [DataMember]
        [Display(Name = "Активно")]
        public bool IsActive { get; set; } = true;

        [DataMember]
        public Guid LastUpdatedSession { get; set; }



        #region INotifyPropertyChanged we do use Foody https://github.com/Fody/PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}