using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace AlohaService.ServiceDataContracts
{
    [DataContract]
    [Flags]
    public enum UserRole
    {
        [EnumMember(Value = "0")]
        Admin = 0,
        [EnumMember(Value = "1")]
        Director = 1,
        [EnumMember(Value = "2")]
        Other = 2
    }
}