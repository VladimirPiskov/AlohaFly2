using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlohaFly.Utils
{
    public static class EditCustomers
    {
        public static void Edit()
        {
            var phones = DataExtension.DataCatalogsSingleton.Instance.OrderCustomerPhoneData.Data;
            foreach (var ph in phones)
            {
                if (ph.Phone == null) continue;

                if (ph.Phone.StartsWith(" ") || ph.Phone.EndsWith(" "))
                {
                    ph.Phone = ph.Phone.Trim();
                    DataExtension.DataCatalogsSingleton.Instance.OrderCustomerPhoneData.EndEdit(ph);
                }

                if (ph.Phone.StartsWith("8"))
                {
                    ph.Phone = "+7" + ph.Phone.Substring(1, ph.Phone.Length - 1);
                    DataExtension.DataCatalogsSingleton.Instance.OrderCustomerPhoneData.EndEdit(ph);
                }

            }

        }

       

        public static void MergeCustomers()
        {
            var custs = DataExtension.DataCatalogsSingleton.Instance.OrderCustomerData.Data;
            var phones = DataExtension.DataCatalogsSingleton.Instance.OrderCustomerPhoneData.Data;
            foreach (var cust in custs)
            {
                if (cust == null) continue;
                
                if (!phones.Any(a => a.IsPrimary && a.OrderCustomerId==cust.Id )) continue;
                var ph = phones.FirstOrDefault(a => a.IsPrimary && a.OrderCustomerId == cust.Id).Phone;
                if (ph == null) continue;
                if (ph.Trim() == "") continue;

                var q = DataExtension.DataCatalogsSingleton.Instance.OrderCustomerData.Data.Where(a => a.Id != cust.Id
                
                && phones.Where(b => b.IsPrimary && b.Phone == ph).Select(c => c.OrderCustomerId).Contains(a.Id));
                
                
                if (q.Any())
                {
                    var res = q.ToList();
                    res.Add(cust);
                    //if (!res.Any(a => a.CashBack))
                    {
                        var ordC = res.Select(a => DataExtension.DataCatalogsSingleton.Instance.OrdersToGoData.Data.Where(b => b.OrderCustomerId == a.Id).Count());
                        var m = ordC.Max();
                        var mainCust = res.FirstOrDefault(a => DataExtension.DataCatalogsSingleton.Instance.OrdersToGoData.Data.Where(b => b.OrderCustomerId == a.Id).Count() == m);
                        foreach (var cudst in res.Where(a => a.Id != mainCust.Id))
                        {
                            var ordrs = DataExtension.DataCatalogsSingleton.Instance.OrdersToGoData.Data.Where(b => b.OrderCustomerId == cudst.Id).ToList();
                            foreach (var ord in ordrs)
                            {
                                ord.OrderCustomerId = mainCust.Id;
                                DataExtension.DataCatalogsSingleton.Instance.OrdersToGoData.EndEdit(ord);
                            }
                            cudst.IsActive = false;
                            DataExtension.DataCatalogsSingleton.Instance.OrderCustomerData.EndEdit(cudst);

                        }
                    }
                    

                }




            }

        }
    }
}
