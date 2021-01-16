using AlohaFly.DataExtension;
using AlohaService.ServiceDataContracts;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace AlohaFly.Models.ToGoClient
{
    public abstract class ToGoClientFinderBaseViewModel : ReactiveObject
    {

        public ToGoClientFinderBaseViewModel()
        {
            this.WhenAnyValue(a => a.FindString).Subscribe(_ => Task.Run(()=> UpdateFindResults()));
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
            if (FindString == null || FindString.Trim().Length < 4)
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
