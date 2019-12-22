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
    public class ToGoClientsViewModel : ViewModelPaneReactiveObject
    {
        FullyObservableDBDataSubsriber<OrderCustomer, ToGoClientViewModel> clientsConnector = new FullyObservableDBDataSubsriber<OrderCustomer, ToGoClientViewModel>(a => a.OrderCustomer.Id);
        public ToGoClientsViewModel()
        {

            AddCommand = new DelegateCommand(_ =>
            {
                IsEdit = true;
                EditableSelectedClient = new ToGoClientEditViewModel(null, true);
            }
            );

            EditCommand = new DelegateCommand(_ =>
           {
               IsEdit = true;
               if (SelectedClientVM != null)
               {
                   EditableSelectedClient = new ToGoClientEditViewModel(((ToGoClientViewModel)ToGoClientsCol.CurrentItem).OrderCustomer);
               }
           }
            );

            CancelEditCommand = new DelegateCommand(_ =>
            {
                IsEdit = false;
            }
            );
            EndEditCommand = new DelegateCommand(_ =>
            {

                if (EditableSelectedClient.Save())
                {
                    IsEdit = false;
                    var sel = toGoClients2.FirstOrDefault(a => a.OrderCustomer.Id == EditableSelectedClient.Client.Id);
                    if (sel != null)
                    {
                        ToGoClientsCol.MoveCurrentTo(sel);
                    }
                }

            }
          );
            IsEdit = false;


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

            SelectedClientVM = new ToGoClientExtViewModel(((ToGoClientViewModel)ToGoClientsCol.CurrentItem).OrderCustomer);
        }

        [Reactive] public bool GridViewIsEnabled { get; set; }
        [Reactive] public bool RadGridViewIsFocused { get; set; }

        public ICommand AddCommand { set; get; }
        public ICommand EditCommand { set; get; }
        public ICommand EndEditCommand { set; get; }
        public ICommand CancelEditCommand { get; }



        [Reactive] public ToGoClientEditViewModel EditableSelectedClient { get; set; }

        [Reactive] public ToGoClientViewModel SelectedClientVM { get; set; }



        bool isEdit { get; set; } = true;
        public bool IsEdit
        {
            get
            {
                return isEdit;
            }
            set
            {
                if (value == isEdit) return;
                MainClass.Dispatcher.Invoke(() =>
                {



                    GridViewIsEnabled = !value;
                    ReadOnlyClientVisibility = value ? Visibility.Collapsed : Visibility.Visible;
                    EditClientVisibility = value ? Visibility.Visible : Visibility.Collapsed;
                    isEdit = value;
                    RadGridViewIsFocused = !value;
                });
            }
        }




        [Reactive] public Visibility ReadOnlyClientVisibility { get; set; } = Visibility.Visible;
        [Reactive] public Visibility EditClientVisibility { get; set; } = Visibility.Collapsed;
    }






    public class ToGoClientAddressViewModel : ReactiveObject
    {

        public long Id { get; set; }


        [Reactive] public string Address { get; set; }

        [Reactive] public string MapUrl { get; set; } = "";

        [Reactive] public string SubWay { get; set; } = "";

        [Reactive] public string Comment { get; set; } = "";

        [Reactive] public long ZoneId { get; set; } = 0;

        [Reactive] public bool IsActive { get; set; }

        [Reactive] public bool IsPrimary { get; set; }

        [Reactive] public long OrderCustomerId { get; set; }

        [Reactive] public bool NeedUpdate { get; set; } = false;


        public override string ToString()
        {
            return Address;
        }


    }

    public class ToGoClientPhonesViewModel : ReactiveObject
    {
        public ToGoClientPhonesViewModel(ReadOnlyObservableCollection<ToGoClientPhoneViewModel> phonesVM)
        {
            PhonesVM = phonesVM;
            _primaryPhone = this.WhenAnyValue(x => x.PhonesVM)
                .Select(t => t.SingleOrDefault(ph => ph.IsPrimary))
                .ToProperty(this, nameof(PrimaryPhone));
        }

        public ReadOnlyObservableCollection<ToGoClientPhoneViewModel> PhonesVM { get; }


        private readonly ObservableAsPropertyHelper<ToGoClientPhoneViewModel> _primaryPhone;
        public ToGoClientPhoneViewModel PrimaryPhone => _primaryPhone.Value;

        public override string ToString()
        {
            return String.Join(Environment.NewLine, PhonesVM.Where(a => a.IsActive).OrderBy(a => a.IsPrimary).Select(a => a.Phone));
        }



    }

    public class ToGoClientPhoneViewModel : ReactiveObject
    {

        [Reactive] public long Id { get; set; }

        [Reactive] public string Phone { get; set; }

        [Reactive] public bool IsActive { get; set; }

        [Reactive] public bool IsPrimary { get; set; }

        [Reactive] public long OrderCustomerId { get; set; }

        [Reactive] public bool NeedUpdate { get; set; }

    }

    public class RemoveObjectCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var o = parameter;
        }
        public RemoveObjectCommand()
        {
        }
    }
}
