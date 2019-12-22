using AlohaFly.DataExtension;
using AlohaFly.Utils;
using AlohaService.ServiceDataContracts;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;

namespace AlohaFly.Models.ToGoClient
{
    public class ToGoClientViewModel : ReactiveObject, ICloneable
    {
        FullyObservableDBDataSubsriber<OrderCustomerPhone, OrderCustomerPhone> phonesConnector = new FullyObservableDBDataSubsriber<OrderCustomerPhone, OrderCustomerPhone>(a => a.Id);
        FullyObservableDBDataSubsriber<OrderCustomerAddress, OrderCustomerAddress> addressConnector = new FullyObservableDBDataSubsriber<OrderCustomerAddress, OrderCustomerAddress>(a => a.Id);

        public ToGoClientViewModel()
        { }

        public ToGoClientViewModel(OrderCustomer orderCustomer)
        {

            OrderCustomer = orderCustomer;

            PhonesVM = new FullyObservableCollection<OrderCustomerPhone>();
            phonesConnector.Select(a => a.OrderCustomerId == OrderCustomer.Id)
                            .OrderBy(a => Convert.ToInt32(!a.IsPrimary))
                            .Subsribe(DataCatalogsSingleton.Instance.OrderCustomerPhoneData, PhonesVM)
                            .SubsribeAction(DataCatalogsSingleton.Instance.OrderCustomerPhoneData, a => SetPrimaryPhone(a), a => a.FirstOrDefault(b => b.IsPrimary));

            AddressesVM = new FullyObservableCollection<OrderCustomerAddress>();
            addressConnector.Select(a => a.OrderCustomerId == OrderCustomer.Id)
                            .OrderBy(a => Convert.ToInt32(!a.IsPrimary))
                            .Subsribe(DataCatalogsSingleton.Instance.OrderCustomerAddressData, AddressesVM)
                            .SubsribeAction(DataCatalogsSingleton.Instance.OrderCustomerAddressData, a => SetPrimaryAddress(a), a => a.FirstOrDefault(b => b.IsPrimary));

          


        }
       
        ~ToGoClientViewModel()
        {
            DataCatalogsSingleton.Instance.OrderCustomerPhoneData.Unsubsribe(PhonesVM);
            DataCatalogsSingleton.Instance.OrderCustomerAddressData.Unsubsribe(AddressesVM);
        }


        public void Dispose()
        {
            DataCatalogsSingleton.Instance.OrderCustomerPhoneData.Unsubsribe(PhonesVM);
            DataCatalogsSingleton.Instance.OrderCustomerAddressData.Unsubsribe(AddressesVM);
        }


        public OrderCustomerInfo ClientInfo
        {
            get
            {
                return OrderCustomer?.OrderCustomerInfo;

            }
        }

        public void SetPrimaryPhone(OrderCustomerPhone value)
        {
            PrimaryPhone = value;
        }
        [Reactive] public OrderCustomerPhone PrimaryPhone { get; set; }



        public void SetPrimaryAddress(OrderCustomerAddress value)
        {
            PrimaryAddress = value;
        }
        [Reactive] public OrderCustomerAddress PrimaryAddress { get; set; }



        public OrderCustomer OrderCustomer { get; set; }




        [Reactive] public FullyObservableCollection<OrderCustomerPhone> PhonesVM { set; get; }

        [Reactive] public FullyObservableCollection<OrderCustomerAddress> AddressesVM { get; set; }


        public object Clone()
        {
            return this.MemberwiseClone();


        }

    }
}
