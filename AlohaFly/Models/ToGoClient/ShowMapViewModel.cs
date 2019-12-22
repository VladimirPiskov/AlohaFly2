using AlohaFly.DataExtension;
using AlohaService.ServiceDataContracts;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace AlohaFly.Models.ToGoClient
{
    public  class ShowMapViewModel : ReactiveObject
    {
        [Reactive] public string BrowserAddress { set; get; }
        [Reactive] public string Address { set; get; }


        OrderCustomerAddress orderCustomerAddress;
        public ShowMapViewModel(OrderCustomerAddress _orderCustomerAddress)
        {
            if (_orderCustomerAddress == null) { _orderCustomerAddress = new OrderCustomerAddress(); };
            orderCustomerAddress = _orderCustomerAddress;
            Address = orderCustomerAddress.Address;
            NavigateToAddress();
        }
        private void NavigateToAddress()
        {
            
            //if (string.IsNullOrEmpty(orderCustomerAddress.MapUrl))
            {
                orderCustomerAddress.MapUrl = GeneratePrintUrl(Address);
            }
            BrowserAddress = orderCustomerAddress.MapUrl;
        }


        private string GeneratePrintUrl(string address)
        {
            //var b64AddressBytes = System.Text.Encoding.UTF8.GetBytes(address);
            //string b64Address = System.Convert.ToBase64String(b64AddressBytes);
            string res = @"https://yandex.ru/maps/print/?mode=search&text=" + HttpUtility.UrlEncode(address) +@"&z=17";
            return res;
        }

        public ICommand FindAddressCommand
        {
            get
            {
                return new DelegateCommand(_ =>
                {
                    NavigateToAddress();
                }
            );
            }
        }
        /*
        public ICommand SaveCommand
        {
            get
            {
                return new DelegateCommand(_ =>
                {
                    DataCatalogsSingleton.Instance.OrderCustomerAddressData.EndEdit();
                }
            );
            }
        }
        */
    }
}
