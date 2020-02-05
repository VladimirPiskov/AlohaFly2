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
    public class AirCompany: INotifyPropertyChanged
    {
        [DataMember]
        [Display(Name = "№")]
        public long Id { get; set; }

        [DataMember]
        [Display(AutoGenerateField = false)]
        public string Code { get; set; }

        [DataMember]
        [Display(Name = "Наименование                     ")]
        public string Name { get; set; }

        [DataMember]
        [Display(AutoGenerateField = false)]
        public string FullName { get; set; }
        [DataMember]
        [Display(AutoGenerateField = false)]
        public string Address { get; set; }
        [DataMember]
        [Display(Name = "ИНН")]
        public string Inn { get; set; }
        [DataMember]
        [Display(AutoGenerateField = false)]
        public string IkaoCode { get; set; }
        [DataMember]
        [Display(AutoGenerateField = false)]
        public string IataCode { get; set; }
        [DataMember]
        [Display(AutoGenerateField = false)]
        public string RussianCode { get; set; }
        [DataMember]
        [Display(Name = "Активно")]
        public bool IsActive { get; set; } = true;

        [DataMember]
        [Display(Name = "Id Схема скидок")]
        public long? DiscountId { get; set; }

        [DataMember]

        [Display(Name = "Схема скидок")]
        public Discount DiscountType { get; set; }

        [DataMember]
        //[Display(Name = "SHId", AutoGenerateField = false)]
        [Display(Name = "SHId")]
        public long SHId { get; set; }

        [DataMember]
        [Display(Name = "Code1C", AutoGenerateField = false)]
        public string Code1C { get; set; }

        [DataMember]
        [Display(Name = "Name1C")]
        public string Name1C { get; set; }
      
        [DataMember]
        public long? PaymentId { get; set; }

        [DataMember]
        [Display(Name = "Вид платежа")]
        public Payment PaymentType { get; set; }

        [DataMember]
        [Display(AutoGenerateField = false)]
        public Guid LastUpdatedSession { get; set; }



        public event PropertyChangedEventHandler PropertyChanged;

        
    }
}