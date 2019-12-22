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
    [Flags]
    public enum OrderStatus
    {
        //[Display(Name = "Unknown")]
        //[Description("Unknown")]
        //[EnumMember(Value = "0")]
        //Unknown = 0,


        [Display(Name = "В работе")]
        [Description("В работе")]
        [EnumMember(Value = "1")]
        InWork = 1,

        [Display(Name = "Отменен")]
        [EnumMember(Value = "2")]
        [Description("Отменен")]
        Cancelled = 2,

        [Display(Name = "Отправлен")]
        [EnumMember(Value = "4")]
        [Description("Отправлен")]
        Sent = 4,

        [Display(Name = "Отмена с остатком")]
        [EnumMember(Value = "8")]
        [Description("Отмена с остатком")]
        CancelledWithRemains = 8,

        [Display(Name = "Закрыт")]
        [EnumMember(Value = "16")]
        [Description("Закрыт")]
        Closed = 16
    }
}