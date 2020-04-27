using AlohaFly.DataExtension;
using AlohaService.ServiceDataContracts;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace AlohaFly.Models.ToGoClient
{
    public class ToGoClientFinderByAddressViewModel : ToGoClientFinderBaseViewModel
    {

        public ToGoClientFinderByAddressViewModel()
        {
            WatermarkContent = "Введите адрес для поиска клиента";
        }
                     
        protected override List<ToGoClientFinderItemViewModel> GetFindResults(string arg)
        {
            var res2 = new List<ToGoClientFinderItemViewModel>();


            List<OrderCustomer> res = new List<OrderCustomer>();
            var PrAddresses = DataCatalogsSingleton.Instance.OrderCustomerAddressData.Data.Where(a => a.IsPrimary && a.Address != null && a.Address.ToLower().Replace('ё', 'е').Contains(arg.ToLower().Replace('ё', 'е')));
            if (PrAddresses != null && PrAddresses.Any())
            {
                res.AddRange(DataCatalogsSingleton.Instance.OrderCustomerData.Data.Where(a => PrAddresses.Select(b => b.OrderCustomerId).Contains(a.Id)));
            }
            //res.AddRange(DataCatalogsSingleton.Instance.OrderCustomerData.Data.Where(a => !string.IsNullOrEmpty(a.FullName) && a.FullName.ToLower().Contains(arg.ToLower())));

            var AllAddresses = DataCatalogsSingleton.Instance.OrderCustomerAddressData.Data.Where(a => !a.IsPrimary && a.Address != null && a.Address.ToLower().Replace('ё', 'е').Contains(arg.ToLower().Replace('ё', 'е')));
            if (AllAddresses != null && AllAddresses.Any())
            {
                res.AddRange(DataCatalogsSingleton.Instance.OrderCustomerData.Data.Where(a => AllAddresses.Select(b => b.OrderCustomerId).Contains(a.Id)));
            }
            if (res != null)
            {
                
                int fontSize1 = 18;
                int fontSize2 = 14;
                res2 = res.Distinct().Select(a => new ToGoClientFinderItemViewModel()
                {
                    orderCustomer = a,
                    EMail = a.Email,
                    FullName = GetFindHTML(a.FullName, arg, fontSize1),
                    Phone =
                    GetFindHTML(
                    string.Join(", ", DataCatalogsSingleton.Instance.OrderCustomerAddressData.Data
                    .Where(b => b.OrderCustomerId == a.Id && b.Address != null && (b.Address.Contains(arg) || (b.IsPrimary)  ))
                    .Select(b => b.Address).ToArray())
                    , arg, fontSize2),

                }).ToList();
            }
            return res2;
        }


    }
    

}
