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
    public class OperationResult
    {
        [DataMember]
        public bool Success
        {
            get; set;
        }

        [DataMember]
        public long CreatedObjectId
        {
            get; set;
        }

        [DataMember]
        public string ErrorMessage
        {
            get; set;
        }
    }
}