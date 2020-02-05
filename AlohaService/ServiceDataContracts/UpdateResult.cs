using AlohaService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace AlohaService.ServiceDataContracts
{
    [DataContract]
    public class UpdateResult
    {
        [DataMember]
        public Guid SessionId { set; get; }



        [DataMember]
        public List<User> Users { set; get; }
        [DataMember]
        public List<ContactPerson> ContactPerson { set; get; }
        [DataMember]
        public List<DishLogicGroup> DishLogicGroup { set; get; }
        [DataMember]
        public List<DishKitchenGroup> DishKitchenGroup { set; get; }
        [DataMember]
        public List<PaymentGroup> PaymentGroup { set; get; }
        [DataMember]
        public List<Payment> Payment { set; get; }
        [DataMember]
        public List<Discount> Discount { set; get; }
        [DataMember]
        public List<Driver> Driver { set; get; }
        [DataMember]
        public List<DeliveryPlace> DeliveryPlace { set; get; }
        [DataMember]
        public List<AirCompany> AirCompany { set; get; }
        [DataMember]
        public List<Dish> Dish { set; get; }
        [DataMember]
        public List<ItemLabelInfo> ItemLabelInfo { set; get; }
        [DataMember]
        public List<MarketingChannel> MarketingChannel { set; get; }
        [DataMember]
        public List<OrderCustomerAddress> OrderCustomerAddresss { set; get; }
        [DataMember]
        public List<OrderCustomerPhone> OrderCustomerPhones { set; get; }
        [DataMember]
        public List<OrderCustomer> OrderCustomers { set; get; }

        [DataMember]
        public List<OrderToGo> OrderToGos { set; get; }
        [DataMember]
        public List<OrderFlight> OrderFlight { set; get; }

        [DataMember]
        public DateTime UpdatesTime { set; get; }
    }
}   