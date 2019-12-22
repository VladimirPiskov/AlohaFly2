using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AlohaService.ServiceDataContracts
{
    [DataContract]
    public class UserUserGroup : INotifyPropertyChanged
    {
        [DataMember]
        [Display(Name = "Внутренний идентификатор")]
        public long Id { get; set; }

        [DataMember]
        [Display(Name = "Внцтренний идентификатор пользователя")]
        public long UserId { get; set; }

        [DataMember]
        public User User { get; set; }

        [DataMember]
        [Display(Name = "Внутренний идентификатор группы")]
        public long GroupId { get; set; }

        [DataMember]
        public UserGroup UserGroup { get; set; }

        #region INotifyPropertyChanged we do use Foody https://github.com/Fody/PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}