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
    public enum FuncAccessType
    {
        [EnumMember(Value = "0")]
        View = 0,
        [EnumMember(Value = "1")]
        Edit = 1
    }
}