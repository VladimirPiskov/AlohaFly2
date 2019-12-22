using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace AlohaService.ServiceDataContracts
{
    public class LogItem
    {
        [DataMember]
        [Display(Name = "Id события")]
        public long Id { get; set; }

        [DataMember]
        [Display(Name = "Вызыванный метод")]
        public string MethodName { get; set; }

        [DataMember]
        [Display(Name = "Действие")]
        public string ActionName { get; set; }

        [DataMember]
        [Display(Name = "Описание действия")]
        public string ActionDescription { get; set; }

        [DataMember]
        [Display(Name = "Состояние до")]
        public string StateBefore { get; set; }

        [DataMember]
        [Display(Name = "Состояние после")]
        public string StateAfter { get; set; }

        [DataMember]
        [Display(Name = "ID пользователя")]
        public long UserId { get; set; }

        [Display(Name = "Пользователь")]
        [DataMember]
        public virtual User CreatedBy { get; set; }

        [DataMember]
        public DateTime CreationDate { get; set; }
    }
}