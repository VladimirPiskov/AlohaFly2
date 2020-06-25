using AlohaFly.DataExtension;
using AlohaFly.Utils;
using AlohaService.ServiceDataContracts;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;

namespace AlohaFly.Models.ToGoClient
{
    public class ToGoMergeCustomers : ViewModelPaneReactiveObject
    {
        FullyObservableDBDataSubsriber<OrderCustomer, ToGoClientViewModel> clientsConnector = new FullyObservableDBDataSubsriber<OrderCustomer, ToGoClientViewModel>(a => a.OrderCustomer.Id);
        public ToGoMergeCustomers(ToGoClientViewModel _parentClient)
        {
            ParentClient = _parentClient;

            OkCommand = new DelegateCommand(_ =>
            {
                if (SelectedClientVM == null)
                {
                    UI.UIModify.ShowAlert("Не выбран второй клиент");
                    return;
                }
                if (SelectedClientVM.OrderCustomer.Id== ParentClient.OrderCustomer.Id)
                {
                    UI.UIModify.ShowAlert("Нельзя объединить с самим собой");
                    return;
                }

                string txt = $"Вы уверены что хотите передать все заказы от {SelectedClientVM.OrderCustomer.Id} " +
                $" {SelectedClientVM.OrderCustomer.Name }({SelectedClientVM.OrderCustomer.GetPrimaryPhone().Phone}) к клиенту" + Environment.NewLine +
                $"{ ParentClient.OrderCustomer.Id} {ParentClient.OrderCustomer.Name }({ParentClient.OrderCustomer.GetPrimaryPhone().Phone})";
                
                if (UI.UIModify.ShowConfirm(txt).DialogResult.GetValueOrDefault() )
                {
                    var res = DBProvider.Client.MergeCustomers(ParentClient.OrderCustomer, SelectedClientVM.OrderCustomer);
                    if (res.Success)
                    {
                        RealTimeUpdaterSingleton.Instance.FastUpdate();
                        DataCatalogsSingleton.Instance.OrderCustomerData.DeleteItem(SelectedClientVM.OrderCustomer);
                    }
                        string rStr = res.Success ? "Объединение прошло успешно" : $"Ошибка объединения {res.ErrorMessage}";
                    
                        UI.UIModify.ShowAlert(rStr);
                    
                    

                }


                CloseAction();
            }
            );

            CancelCommand = new DelegateCommand(_ =>
           {
               CloseAction();
               
           }
            );


            toGoClients2 = new FullyObservableCollection<ToGoClientViewModel>();
            clientsConnector.Subsribe(DataCatalogsSingleton.Instance.OrderCustomerData, toGoClients2, a => new ToGoClientViewModel(a));
        }



        

        private FullyObservableCollection<ToGoClientViewModel> toGoClients2;// = new ObservableCollection<OrderCustomer>();
        ICollectionView _toGoClientsCol;
        public ICollectionView ToGoClientsCol
        {
            get
            {
                {
                    if (_toGoClientsCol == null)
                    {
                        QueryableCollectionView collectionViewSource = new QueryableCollectionView(toGoClients2);
                        _toGoClientsCol = collectionViewSource;
                        SelectedClientVM = (ToGoClientViewModel)ToGoClientsCol.CurrentItem;
                        _toGoClientsCol.CurrentChanged += _toGoClientsCol_CurrentChanged;
                        _toGoClientsCol.MoveCurrentToFirst();
                    }
                }
                return _toGoClientsCol;
            }
        }
        private void _toGoClientsCol_CurrentChanged(object sender, EventArgs e)
        {
            SelectedClientVM?.Dispose();
            if ((ToGoClientViewModel)ToGoClientsCol.CurrentItem != null)
            {
                SelectedClientVM = new ToGoClientExtViewModel(((ToGoClientViewModel)ToGoClientsCol.CurrentItem).OrderCustomer);
                InfoMessage2 = $"{SelectedClientVM.OrderCustomer.Id}. {SelectedClientVM.OrderCustomer.Name}";
            }
        }


        

        public ICommand OkCommand { set; get; }
        public ICommand CancelCommand { set; get; }


         [Reactive] public string InfoMessage2 { set; get; }
        

            [Reactive] public ToGoClientEditViewModel SelectedClient { get; set; }

        [Reactive] public ToGoClientViewModel SelectedClientVM { get; set; }

        [Reactive] public ToGoClientViewModel ParentClient { get; set; }

        public string InfoMessage
        {
            get
            {
                if (ParentClient == null) return "";
                try
                {
                    return $"{ParentClient.OrderCustomer.Id}. {ParentClient.OrderCustomer.Name} ({ParentClient.OrderCustomer.GetPrimaryPhone().Phone})";
                        }
                catch
                {
                    return "Ошибка клиента"; 
                }
            }
        }
    }
}
