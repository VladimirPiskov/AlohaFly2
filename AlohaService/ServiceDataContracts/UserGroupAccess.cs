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
    public class UserGroupAccess : INotifyPropertyChanged
    {
        [DataMember]
        [Display(Name = "Внутренний идентификатор")]
        public long Id { get; set; }

        [DataMember]
        [Display(Name = "Внутренний идентификатор группы")]
        public long UserGroupId { get; set; }

        [DataMember]
        [Display(Name = "Внутренний идентификатор функции")]
        public long UserFuncId { get; set; }

        [DataMember]
        public UserFunc UserFunc { get; set; }

        [DataMember]
        public FuncAccessType FuncAccessType { get; set; }

        #region INotifyPropertyChanged we do use Foody https://github.com/Fody/PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}