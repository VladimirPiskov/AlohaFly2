namespace AlohaFly.Models
{
    /*
    class ToGoClientCatalogViewModel : ViewModelPane
    {
        public ToGoClientCatalogViewModel()
        {
            AddPhoneToCustomer = new DelegateCommand(_ =>
            {
                if ((SelectedItem!=null) && (AddPhoneStr.Trim() != ""))
                {
                    if (SelectedItem.Phones == null) { SelectedItem.Phones = new List<OrderCustomerPhone>(); }
                    foreach (var ph in SelectedItem.Phones)
                    {
                        ph.IsPrimary = false;
                    }

                    var nph = new OrderCustomerPhone()
                    {
                        IsActive = true,
                        IsPrimary = true,
                        Phone = AddPhoneStr
                    };

                    SelectedItem.Phones.Insert(0,nph);
                    //SelectedItem.PrimaryPhone = "";
                    CommitChanges();
                    AddPhoneStr = "";
                   
                    SetPhonesCollection();
                    RaisePropertyChanged("PhonesCollection");
                    RaisePropertyChanged("AddPhoneStr");

                    PhonesCollection.MoveCurrentTo(nph);
                    //RaisePropertyChanged("AddPhoneBntEnabled");
                }
            }
            );



            RemovePhoneFromCustomer = new DelegateCommand(_ =>
            {
                if ((SelectedItem != null) && (SelectedPhone!=null))
                {
                    //if (SelectedItem.Phones == null) { SelectedItem.Phones = new List<OrderCustomerPhone>(); }

                    SelectedItem.Phones.Remove(SelectedPhone);
                    
                    if (!SelectedItem.Phones.Any(a => a.IsPrimary) && SelectedItem.Phones.Count>0)
                    {
                        SelectedItem.Phones.First().IsPrimary = true;
                    }
                    //SelectedItem.PrimaryPhone = "";
                    CommitChanges();
                    SetPhonesCollection();
                    RaisePropertyChanged("PhonesCollection");

                    //RaisePropertyChanged("AddPhoneBntEnabled");
                }
            }
            );

            SetPrimaryPhone = new DelegateCommand(_ =>
            {
                if ((SelectedItem != null) && (SelectedPhone != null))
                {
                    
                    foreach (var ph in SelectedItem.Phones)
                    {
                        ph.IsPrimary = false;
                        ph.NeedUpdate = true;
                    }
                    SelectedPhone.IsPrimary = true;
                    CommitChanges();

                   // SelectedItem.PrimaryPhone = "";
                //    SelectedItem.IsActive = !SelectedItem.IsActive;

                    RaisePropertyChanged("PhonesCollection");
                //RaisePropertyChanged("AddPhoneBntEnabled");
                }
            }
           );



            AddAddressToCustomer = new DelegateCommand(_ =>
            {
                if ((SelectedItem != null) && (AddAddressStr.Trim() != ""))
                {
                    if (SelectedItem.Addresses == null) { SelectedItem.Addresses = new List<OrderCustomerAddress>(); }

                 
                    var nph = new OrderCustomerAddress()
                    {
                        IsActive = true,
                        IsPrimary = true,
                        Address = AddAddressStr,
                        NeedUpdate = true,

                    };


                    SelectedItem.AddAddress(nph);
                    //SelectedItem.Addresses.Insert(0, nph);
                    //SelectedItem.PrimaryAddress = "";
                    CommitChanges();
                    //AddAddressStr = "";
                    
                    SetAddresssCollection();
                    AddresssCollection.MoveCurrentTo(nph);
                    RaisePropertyChanged("AddresssCollection");
                    RaisePropertyChanged("AddAddressStr");
                    //RaisePropertyChanged("AddAddressBntEnabled");
                }
            }
           );



            RemoveAddressFromCustomer = new DelegateCommand(_ =>
            {
                if ((SelectedItem != null) && (SelectedAddress != null))
                {
                    //if (SelectedItem.Addresss == null) { SelectedItem.Addresss = new List<OrderCustomerAddress>(); }

                    SelectedItem.Addresses.Remove(SelectedAddress);

                    if (!SelectedItem.Addresses.Any(a => a.IsPrimary) && SelectedItem.Addresses.Count > 0)
                    {
                        SelectedItem.Addresses.First().IsPrimary = true;
                    }
                    //SelectedItem.PrimaryAddress = "";
                    CommitChanges();
                    SetAddresssCollection();
                    RaisePropertyChanged("AddresssCollection");

                    //RaisePropertyChanged("AddAddressBntEnabled");
                }
            }
            );

            SetPrimaryAddress = new DelegateCommand(_ =>
            {
                if ((SelectedItem != null) && (SelectedAddress != null))
                {

                    foreach (var ph in SelectedItem.Addresses)
                    {
                        ph.IsPrimary = false;
                    }
                    SelectedAddress.IsPrimary = true;
                    CommitChanges();

                    //SelectedItem.PrimaryAddress = "";
                    //    SelectedItem.IsActive = !SelectedItem.IsActive;

                    RaisePropertyChanged("AddresssCollection");
                    //RaisePropertyChanged("AddAddressBntEnabled");
                }
            }
           );
        }

        public string AddPhoneStr { set; get; } = "";
        public string AddAddressStr { set; get; } = "";


        ICollectionView _itemsSource;
        public ICollectionView ItemsSource
        {
            get
            {
                if (_itemsSource == null)
                {
                    QueryableCollectionView collectionViewSource = new QueryableCollectionView(DataCatalogsSingleton.Instance.ToGoCustomers);

                    
                    _itemsSource = collectionViewSource;
                    _itemsSource.MoveCurrentToFirst();
                    _itemsSource.CurrentChanged += new EventHandler((_, __) => {
                        // RaisePropertyChanged("SelectedItemViewModel");
                        SetPhonesCollection();
                        SetAddresssCollection();
                        RaisePropertyChanged("PhonesCollection");
                        RaisePropertyChanged("AddresssCollection");
                        RaisePropertyChanged("AddPhoneBntEnabled");
                        RaisePropertyChanged("AdAddressBntEnabled");
                    });
                }

                return _itemsSource;
            }
        }


        public ICommand AddPhoneToCustomer { get; set; }
        public ICommand RemovePhoneFromCustomer { get; set; }
        public ICommand SetPrimaryPhone { get; set; }


        public ICommand AddAddressToCustomer { get; set; }
        public ICommand RemoveAddressFromCustomer { get; set; }
        public ICommand SetPrimaryAddress { get; set; }


        public bool AddItem()
        {
            if (SelectedItem.Addresses == null) { SelectedItem.Addresses = new List<OrderCustomerAddress>(); }
            if (SelectedItem.Phones == null) { SelectedItem.Phones = new List<OrderCustomerPhone>(); }

            return DataCatalogsSingleton.Instance.AddOrUpdateToGoCustomer(SelectedItem);
          
        }
        public bool CancelAddItem()
        {
            //return _model.CancelAddItem(AddedItemId);
            return DBDataExtractor<OrderCustomer>.DeleteItem(DBProvider.Client.DeleteOrderCustomer, SelectedItem.Id);
        }

        public bool DeleteItem()
        {
            //return _model.DeleteItem(SelectedItem);
            return DBDataExtractor<OrderCustomer>.DeleteItem(DBProvider.Client.DeleteOrderCustomer, SelectedItem.Id);

        }

        public bool CommitChanges()
        {
            return DataCatalogsSingleton.Instance.AddOrUpdateToGoCustomer(SelectedItem);
            //_model.CommitEditItem(SelectedItem);
        }


        public OrderCustomer SelectedItem
        {
            get
            {
                if (ItemsSource == null) return null;
                return (OrderCustomer)ItemsSource.CurrentItem;
            }
        }

        public OrderCustomerPhone SelectedPhone
        {
            get
            {
                if (PhonesCollection == null) return null;
                return (OrderCustomerPhone)PhonesCollection.CurrentItem;
            }
        }


        public bool SetPrymaryPhoneBntEnabled
        {
            get
            {

                return ((SelectedItem != null) && (SelectedPhone != null) && (!SelectedPhone.IsPrimary));
                }
        }

        public bool RemovePhoneBntEnabled
        {
            get
            {
                return ((SelectedItem != null) && (PhonesCollection!=null) && (PhonesCollection.CurrentItem != null));
                
            }
        }

        public bool AddPhoneBntEnabled
        {
            get
            {
                return (SelectedItem != null);
                
            }
        }



        public OrderCustomerAddress SelectedAddress
        {
            get
            {
                if (AddresssCollection == null) return null;
                return (OrderCustomerAddress)AddresssCollection.CurrentItem;
            }
        }


        public bool SetPrymaryAddressBntEnabled
        {
            get
            {

                return ((SelectedItem != null) && (SelectedAddress != null) && (!SelectedAddress.IsPrimary));
            }
        }

        public bool RemoveAddressBntEnabled
        {
            get
            {
                return ((SelectedItem != null) && (AddresssCollection != null) && (AddresssCollection.CurrentItem != null));

            }
        }

        public bool AddAddressBntEnabled
        {
            get
            {
                return (SelectedItem != null);

            }
        }



        void SetPhonesCollection()
        {
            if ((SelectedItem == null) || (SelectedItem.Phones == null))
            {
                phonesCollection = null;
                RaisePropertyChanged("SetPrymaryPhoneBntEnabled");
                RaisePropertyChanged("AddPhoneBntEnabled");
                RaisePropertyChanged("RemovePhoneBntEnabled");

            }
            else
            {
                QueryableCollectionView collectionViewSource = new QueryableCollectionView(SelectedItem.Phones);
                phonesCollection = collectionViewSource;
                phonesCollection.CurrentChanged += new EventHandler((_, __) =>
                {
                    // RaisePropertyChanged("SelectedItemViewModel");
                    RaisePropertyChanged("SetPrymaryPhoneBntEnabled");
                    RaisePropertyChanged("AddPhoneBntEnabled");
                    RaisePropertyChanged("RemovePhoneBntEnabled");
                });
            }
        }

        ICollectionView phonesCollection;
        public ICollectionView PhonesCollection
        {
            get
            {
                if (SelectedItem==null)
                {
                    return null;
                }
                
                return phonesCollection;
            }
        }



        void SetAddresssCollection()
        {
            if ((SelectedItem == null) || (SelectedItem.Addresses == null))
            {
                phonesCollection = null;
                RaisePropertyChanged("SetPrymaryAddressBntEnabled");
                RaisePropertyChanged("AddAddressBntEnabled");
                RaisePropertyChanged("RemoveAddressBntEnabled");

            }
            else
            {
                QueryableCollectionView collectionViewSource = new QueryableCollectionView(SelectedItem.Addresses);
                addresssCollection = collectionViewSource;
                addresssCollection.CurrentChanged += new EventHandler((_, __) =>
                {
                    // RaisePropertyChanged("SelectedItemViewModel");
                    RaisePropertyChanged("SetPrymaryAddressBntEnabled");
                    RaisePropertyChanged("AddAddressBntEnabled");
                    RaisePropertyChanged("RemoveAddressBntEnabled");
                });
            }
        }

        ICollectionView addresssCollection;
        public ICollectionView AddresssCollection
        {
            get
            {
                if (SelectedItem == null)
                {
                    return null;
                }

                return addresssCollection;
            }
        }


    }

*/
}
