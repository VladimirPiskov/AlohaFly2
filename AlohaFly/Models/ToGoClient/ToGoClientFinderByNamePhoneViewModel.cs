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
    public class ToGoClientFinderByNamePhoneViewModel : ToGoClientFinderBaseViewModel
    {

        public ToGoClientFinderByNamePhoneViewModel()
        {
                    WatermarkContent  = "Введите ФИО или телефон для поиска клиента";

            prData = DataCatalogsSingleton.Instance.OrderCustomerPhoneData.Data.Where(a => a.IsPrimary && a.Phone!= null).Select(a => new FindData()
            {
                Data = a.Phone,
                Id = a.Id,
                OrderCustomer = DataCatalogsSingleton.Instance.OrderCustomerData.Data.FirstOrDefault(b => b.Id == a.OrderCustomerId)
            }).Where(a => a.OrderCustomer != null).ToList();

            custData = DataCatalogsSingleton.Instance.OrderCustomerData.Data.Where(a => !string.IsNullOrEmpty(a.FullName)).Select(a => new FindData()
            {
                Data = a.FullName.ToLower().Replace('ё', 'е'),
                Id = a.Id,
                OrderCustomer = a
            }).ToList();

            nprData = DataCatalogsSingleton.Instance.OrderCustomerPhoneData.Data.Where(a => !a.IsPrimary && a.Phone != null).Select(a => new FindData()
            {
                Data = a.Phone,
                Id = a.Id,
                OrderCustomer = DataCatalogsSingleton.Instance.OrderCustomerData.Data.FirstOrDefault(b => b.Id == a.OrderCustomerId)
            }).Where(a => a.OrderCustomer != null).ToList();
        }

        List<FindData> prData = new List<FindData>();
        List<FindData> custData = new List<FindData>();

        List<FindData> nprData = new List<FindData>();


        protected override List<ToGoClientFinderItemViewModel> GetFindResults(string arg)
        {
            var res2 = new List<ToGoClientFinderItemViewModel>();

            

            List<OrderCustomer> res = new List<OrderCustomer>();
            /*
            var PrPhones = DataCatalogsSingleton.Instance.OrderCustomerPhoneData.Data.Where(a => a.IsPrimary && a.Phone != null && a.Phone.Contains(arg));
            if (PrPhones != null && PrPhones.Any())
            {
                res.AddRange(DataCatalogsSingleton.Instance.OrderCustomerData.Data.Where(a => PrPhones.Select(b => b.OrderCustomerId).Contains(a.Id)));
            }
            res.AddRange(DataCatalogsSingleton.Instance.OrderCustomerData.Data.Where(a => !string.IsNullOrEmpty(a.FullName) && a.FullName.ToLower().Contains(arg.ToLower())));

            var AllPhones = DataCatalogsSingleton.Instance.OrderCustomerPhoneData.Data.Where(a => !a.IsPrimary && a.Phone != null && a.Phone.Contains(arg));
            if (AllPhones != null && PrPhones.Any())
            {
                res.AddRange(DataCatalogsSingleton.Instance.OrderCustomerData.Data.Where(a => AllPhones.Select(b => b.OrderCustomerId).Contains(a.Id)));
            }
            */
            string arg2 = arg.ToLower().Replace('ё', 'е');

            res.AddRange(prData.Where(a => a.Data.Contains(arg2)).Select(a => a.OrderCustomer));
            res.AddRange(custData.Where(a => a.Data.Contains(arg2)).Select(a => a.OrderCustomer));

            res.AddRange(nprData.Where(a => a.Data.Contains(arg2)).Select(a => a.OrderCustomer));


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
                    string.Join(", ", DataCatalogsSingleton.Instance.OrderCustomerPhoneData.Data
                    .Where(b => b.OrderCustomerId == a.Id && b.Phone != null && (b.Phone.Contains(arg) || (b.IsPrimary)  ))
                    .Select(b => b.Phone).ToArray())
                    , arg, fontSize2),

                }).ToList();
            }
            return res2;
        }


    }
    

}
