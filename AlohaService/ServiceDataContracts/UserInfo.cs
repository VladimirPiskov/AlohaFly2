using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AlohaService.ServiceDataContracts
{
    [DataContract]
    public class UserInfo : INotifyPropertyChanged
    {
        [DataMember]
        [Display(Name = "Внутренний идентификатор")]
        public string UserName { get; set; }

        [DataMember]
        [Display(Name = "Адрес электронной почты")]
        public string Email { get; set; }

        [DataMember]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        [Display(Name = "Статус регистрации")]
        public string RegistrationStatus { get; set; }

        [DataMember]
        [Display(Name = "Секретный вопрос")]
        public string SequrityQuestion { get; set; }

        [DataMember]
        [Display(Name = "Секретный ответ")]
        public string SequrityAnswer { get; set; }

        [DataMember]
        [Display(Name = "Полное имя")]
        public string FullName { get; set; }

        [DataMember]
        [Display(Name = "Активно")]
        public bool IsActive { get; set; }

        [DataMember]
        public UserRole UserRole { get; set; }

        #region INotifyPropertyChanged we do use Foody https://github.com/Fody/PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}