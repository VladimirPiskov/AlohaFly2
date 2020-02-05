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
    public class Payment : INotifyPropertyChanged
    {
        [DataMember]
        [Display(Name = "Id")]
        public long Id { get; set; }

        [DataMember]
        [Display(Name = "Код платежа")]
        public int Code { get; set; }

        [DataMember]
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [DataMember]
        [Display(Name = "Наличные?")]
        public bool IsCash { get; set; }



        

        [DataMember]
        [Display(Name = "Id в ФР")]
        public long FiskalId { get; set; }


        [DataMember]
        public long FRSend { get; set; }

        [DataMember]
        public long PaymentGroupId { get; set; }

        //[DataMember]
        public PaymentGroup PaymentGroup { get; set; }

        [DataMember]
        [Display(Name = "ToGo")]
        public bool ToGo { get; set; }
        [DataMember]
        public long SHId { get; set; }

        [DataMember]
        [Display(Name = "Активно")]
        public bool IsActive { get; set; } = true;


        [DataMember]
        [Display(AutoGenerateField = false)]
        public Guid LastUpdatedSession { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return Name;
        }
    }
}