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
    public abstract class ToGoClientFinderBaseViewModel : ReactiveObject
    {

        public ToGoClientFinderBaseViewModel()
        {
            this.WhenAnyValue(a => a.FindString).Subscribe(_ => UpdateFindResults());
            ClearCommand = new DelegateCommand(_ =>
            {
                FindString = "";
                FindTextCollection.Clear();
            }
            );

            SelectItemCommand = new DelegateCommand(_ =>
            {
                SetSelectedItem();
            }
            );
        }



        public event ItemSelectedEventHandler ItemSelected;

        protected virtual void OnItemSelected(ItemSelectedEventArgs e)
        {

            ItemSelectedEventHandler handler = ItemSelected;
            handler?.Invoke(this, e);
        }

        private void SetSelectedItem()
        {

            if (SelectedClient != null)
            {
                OnItemSelected(new ItemSelectedEventArgs(SelectedClient?.orderCustomer));
            }
            FindString = "";
            FindTextCollection.Clear();
        }

        protected abstract List<ToGoClientFinderItemViewModel> GetFindResults(string arg);
        /*
        {
      
            var res2 = new List<ToGoClientFinderItemViewModel>();
            

      List<OrderCustomer> res = new List<OrderCustomer>();
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
*/
        protected string GetFindHTML(string soure, string findText, int fontSize)
        {
            string res = $@"<p style=""font-size:{fontSize}px; line-height:0.1"">";
            int pos = soure.IndexOf(findText,StringComparison.OrdinalIgnoreCase);
            int lastpos = 0;
            while (pos >= 0) 
            {
                
                res += soure.Substring(lastpos, pos - lastpos) + $@"<b>{soure.Substring(pos, findText.Length)}</b>";
                lastpos = pos+findText.Length;
                pos = soure.IndexOf(findText, lastpos, StringComparison.OrdinalIgnoreCase);
            }
            res += soure.Substring(lastpos, soure.Length - lastpos);
            res += $@"</p>";
            return res; 
        }

        [Reactive] public bool FPopupIsOpen { get; set; } = false;


        [Reactive] public string WatermarkContent { get; set; } = "Введите текст для поиска клиента";

        public ICommand ClearCommand { get; set; }
        public ICommand SelectItemCommand { get; set; }

        private void UpdateFindResults()
        {
            if (FindString == null || FindString.Trim().Length < 2)
            {
                FPopupIsOpen = false;
                return;
            }
            var res = GetFindResults(FindString);
            if (res.Count == 0)
            {
                InfoText = "Соответствий не найдено";
                InfoTextVisibility = Visibility.Visible;

            }
            else
            {
                InfoTextVisibility = Visibility.Collapsed;
            }

            FindTextCollection = res;
            FPopupIsOpen = true;
        }


        [Reactive] public List<ToGoClientFinderItemViewModel> FindTextCollection { set; get; }

        [Reactive] public ToGoClientFinderItemViewModel SelectedClient { set; get; }

        //private string findString = "";
        [Reactive] public string FindString { set; get; }
        [Reactive] public string InfoText { set; get; }
        [Reactive] public Visibility InfoTextVisibility { set; get; } = Visibility.Collapsed;
    

    }
    public class ToGoClientFinderItemViewModel
    {
        public ToGoClientFinderItemViewModel() { }
        public OrderCustomer orderCustomer { set; get; }
        public string FullName { set; get; }
        public string Phone { set; get; }
        public string EMail { set; get; }


    }

    public delegate void ItemSelectedEventHandler(object sender, ItemSelectedEventArgs e);
    public class ItemSelectedEventArgs : EventArgs
    {
        public ItemSelectedEventArgs(OrderCustomer selectedOrderCustomer) : base()
        {
            SelectedOrderCustomer = selectedOrderCustomer;
        }
        public OrderCustomer SelectedOrderCustomer { set; get; }


    }


}
