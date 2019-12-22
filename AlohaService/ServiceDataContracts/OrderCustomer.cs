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
    public class OrderCustomer : INotifyPropertyChanged
    {
        [DataMember]
        [Display(Name = "Внутренний идентификатор")]
        public long Id { get; set; }

        [DataMember]
        [Display(AutoGenerateField = false)]
        public long OldId { get; set; }

        [DataMember]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        
        [DataMember]
        [Display(Name = "Фамилия")]
        public string SecondName { get; set; }

        [DataMember]
        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }

        [DataMember]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [DataMember]
        [Display(Name = "Комментарий")]
        public string Comments { get; set; }

        [DataMember]
        [Display(Name = "Скидка")]
        public decimal DiscountPercent { get; set; }

        [DataMember]
        public Guid LastUpdatedSession { get; set; }

        
        [Display(AutoGenerateField = false)]
        public string FullName => $"{Name} {SecondName} {MiddleName}";
        
        [DataMember]
        [Display(Name = "Активно")]
        public bool IsActive { get; set; }

        [DataMember]
        public List<OrderCustomerPhone> Phones { get; set; }

        [DataMember]
        public OrderCustomerInfo OrderCustomerInfo { get; set; }

        [DataMember]
        public virtual List<OrderCustomerAddress> Addresses { get; set; }

                public object Clone()
        {
            return this.MemberwiseClone();
        }

        #region INotifyPropertyChanged we do use Foody https://github.com/Fody/PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}