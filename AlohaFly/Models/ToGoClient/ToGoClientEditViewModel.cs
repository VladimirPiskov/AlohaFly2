using AlohaFly.DataExtension;
using AlohaFly.Utils;
using AlohaService.ServiceDataContracts;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;


namespace AlohaFly.Models.ToGoClient
{
    public class ToGoClientEditViewModel : ReactiveObject
    {
        public OrderCustomer Client { set; get; }
        public ToGoClientEditViewModel(OrderCustomer client, bool isNew = false)
        {
            EditablePhones = new FullyObservableCollection<OrderCustomerPhone>();
            EditableAddresses = new FullyObservableCollection<OrderCustomerAddress>();

            if (!isNew)
            {
                if (client != null)
                {
                    Client = (OrderCustomer)client.Clone();
                    OriginalPhones = DataCatalogsSingleton.Instance.OrderCustomerPhoneData.Data.Where(a => a.OrderCustomerId == Client.Id).ToList();
                    OriginalAddresses = DataCatalogsSingleton.Instance.OrderCustomerAddressData.Data.Where(a => a.OrderCustomerId == Client.Id).ToList();

                    foreach (var ph in OriginalPhones)
                    {
                        EditablePhones.Add((OrderCustomerPhone)ph.Clone());
                    }
                    foreach (var ph in OriginalAddresses)
                    {
                        EditableAddresses.Add((OrderCustomerAddress)ph.Clone());
                    }

                }
            }
            else
            {
                Client = new OrderCustomer() { IsActive = true };
                OriginalPhones = new List<OrderCustomerPhone>();
                EditablePhones.Add(new OrderCustomerPhone() { IsPrimary = true, IsActive = true });
                OriginalAddresses = new List<OrderCustomerAddress>();
                EditableAddresses.Add(new OrderCustomerAddress() { IsPrimary = true, IsActive = true });
            }
            EditablePhones.ItemPropertyChanged += EditablePhones_ItemPropertyChanged;
            EditableAddresses.ItemPropertyChanged += EditablePhones_ItemPropertyChanged;

            this.WhenAnyValue(a => a.Client.CashBack).Subscribe(_ =>
            {
                if (Client != null)
                {
                    if (Client.CashBack && Client.CashBackPercent == 0)
                    {
                        Client.CashBackPercent = 10;
                        Client.CashBackStartDate = DateTime.Now.Date;
                    }
                }
            }
            );
        }


        public class ExitedEventArgs : EventArgs
        {
            public ExitedEventArgs(bool saved, OrderCustomer client)
            {
                Saved = saved;
                Client = client;
            }
            public bool Saved { set; get; }
            public OrderCustomer Client { set; get; }
        }
        public delegate void ExitedEventHandler(object sender, ExitedEventArgs e);
        public event ExitedEventHandler Exited;

        protected virtual void OnExited(ExitedEventArgs e)
        {

            ExitedEventHandler handler = Exited;
            handler?.Invoke(this, e);
        }

        private void EditablePhones_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.CollectionIndex >= 0)
            {
                //var itemSender = ((FullyObservableCollection<>)sender)[e.CollectionIndex];
                if (e.PropertyName == "IsPrimary")
                {
                    if (e.ItemSender is AlohaService.Interfaces.IPrimaryUnik un)
                    {
                        if (un.IsPrimary)
                        {
                            foreach (var itm in (sender as IList))
                            {
                                if (!un.Equals(itm))
                                {
                                    ((AlohaService.Interfaces.IPrimaryUnik)itm).IsPrimary = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        public ICommand AddPhoneCommand
        {
            get
            {
                return new DelegateCommand(_ =>
                {

                    var ph = new OrderCustomerPhone()
                    {
                        OrderCustomerId = Client.Id,
                        IsActive = true
                    };

                    EditablePhones.Insert(0, ph);
                    SelectedPhone = ph;
                    ph.IsFocused = true;
                    ph.IsPrimary = true;
                }
            );
            }
        }
        


            public ICommand ShowMapCommand
        {
            get
            {
                return new DelegateCommand(id =>
                {

                    var addr = EditableAddresses.FirstOrDefault(a => a.Id == (Convert.ToInt64(id)));
                    UI.UIModify.ShowWndMap(addr);
                });
            }
        }
        public ICommand RemovePhoneCommand
        {
            get
            {
                return new DelegateCommand(_ =>
                {
                    if (SelectedPhone == null) return;
                    int newInd = 0;
                    int ind = EditablePhones.IndexOf(SelectedPhone);
                    if (ind > 0)
                    {
                        newInd = ind - 1;
                    }
                    bool isPr = SelectedPhone.IsPrimary;
                    EditablePhones.Remove(SelectedPhone);
                    if (EditablePhones.Count > 0)
                    {
                        SelectedPhone = EditablePhones[newInd];
                        if (isPr) SelectedPhone.IsPrimary = true;
                    }
                });
            }
        }
        public ICommand SaveEditCommand
        {
            get
            {
                return new DelegateCommand(_ =>
                {

                    if (Save())
                    {
                        OnExited(new ExitedEventArgs(true, Client));
                    }

                }
            );
            }
        }

        public ICommand CancelEditCommand
        {
            get
            {
                return new DelegateCommand(_ =>
                {

                    OnExited(new ExitedEventArgs(false, Client));
                }
            );
            }
        }

        public ICommand AddAddressCommand
        {
            get
            {
                return new DelegateCommand(_ =>
                {

                    var ph = new OrderCustomerAddress()
                    {
                        OrderCustomerId = Client.Id,
                        IsActive = true
                    };

                    EditableAddresses.Insert(0, ph);
                    SelectedAddress = ph;
                    ph.IsFocused = true;
                    ph.IsPrimary = true;

                }
            );
            }
        }
        public ICommand RemoveAddressCommand
        {
            get
            {
                return new DelegateCommand(_ =>
                {
                    if (SelectedAddress == null) return;
                    int newInd = 0;
                    int ind = EditableAddresses.IndexOf(SelectedAddress);
                    if (ind > 0)
                    {
                        newInd = ind - 1;
                    }
                    bool isPr = SelectedAddress.IsPrimary;
                    EditableAddresses.Remove(SelectedAddress);
                    if (EditableAddresses.Count > 0)
                    {
                        SelectedAddress = EditableAddresses[newInd];
                        if (isPr) SelectedAddress.IsPrimary = true;
                    }
                });
            }
        }

        [Reactive] public OrderCustomerPhone SelectedPhone { get; set; }
        [Reactive] public OrderCustomerAddress SelectedAddress { get; set; }
        [Reactive] public FullyObservableCollection<OrderCustomerPhone> EditablePhones { get; set; }
        [Reactive] public FullyObservableCollection<OrderCustomerAddress> EditableAddresses { get; set; }

        public List<OrderCustomerPhone> OriginalPhones { get; set; }
        public List<OrderCustomerAddress> OriginalAddresses { get; set; }

        public bool Save()
        {





            //Client.Phones = DataCatalogsSingleton.Instance.OrderCustomerPhoneData.Data.Where(a => a.OrderCustomerId == Client.Id).ToList();
            // Client.Addresses = DataCatalogsSingleton.Instance.OrderCustomerAddressData.Data.Where(a => a.OrderCustomerId == Client.Id).ToList();
            var res = DataCatalogsSingleton.Instance.OrderCustomerData.EndEdit(Client);

            if (!res.Succeess)
            {

                //id = res.UpdatedItem.
                var prmres = UI.UIModify.ShowPromt(
                    $"Ошибка сохранения изменений в карточке клиента. {Environment.NewLine} Все равно закрыть карточку (Изменения НЕ СОХРАНЯТСЯ)? {Environment.NewLine} " +
                    $"Подробности: {Environment.NewLine} " +
                    $"{res.ErrorMessage}", confirm: true);
                return prmres.DialogResult.Value;
            }
            else
            {
                Client.Id = res.UpdatedItem.Id;
            }

            var phonesForDelete = OriginalPhones.Where(a => !EditablePhones.Any(b => a.Id == b.Id)).ToList();
            var phonesForAddOrUpdate = EditablePhones.Where(a => a.Id == 0 || (OriginalPhones.Any(b => a.Id == b.Id) && !a.PhoneEquals(OriginalPhones.FirstOrDefault(c => a.Id == c.Id)))).ToList();
            phonesForAddOrUpdate.ForEach(a => a.OrderCustomerId = Client.Id);
            var lRes = DataCatalogsSingleton.Instance.OrderCustomerPhoneData.EndEditMany(phonesForDelete, phonesForAddOrUpdate);
            if (!lRes.Succeess)
            {
                var prmres = UI.UIModify.ShowPromt(
                    $"Ошибка сохранения изменений в карточке клиента. {Environment.NewLine} Все равно закрыть карточку (Изменения НЕ СОХРАНЯТСЯ)? {Environment.NewLine} " +
                    $"Подробности: {Environment.NewLine} " +
                    $"{lRes.ErrorMessage}", confirm: true);
                return prmres.DialogResult.Value;
            }
            var addressesForDelete = OriginalAddresses.Where(a => !EditableAddresses.Any(b => a.Id == b.Id)).ToList();
            var addressesForAddOrUpdate = EditableAddresses.Where(a => a.Id == 0 || (OriginalAddresses.Any(b => a.Id == b.Id) && !a.AddressEquals(OriginalAddresses.FirstOrDefault(b => a.Id == b.Id)))).ToList();
            addressesForAddOrUpdate.ForEach(a => a.OrderCustomerId = Client.Id);
            var lRes2 = DataCatalogsSingleton.Instance.OrderCustomerAddressData.EndEditMany(addressesForDelete, addressesForAddOrUpdate);
            if (!lRes2.Succeess)
            {
                var prmres = UI.UIModify.ShowPromt(
                    $"Ошибка сохранения изменений в карточке клиента. {Environment.NewLine} Все равно закрыть карточку (Изменения НЕ СОХРАНЯТСЯ)? {Environment.NewLine} " +
                    $"Подробности: {Environment.NewLine} " +
                    $"{lRes2.ErrorMessage}", confirm: true);
                return prmres.DialogResult.Value;
            }
            return true;
        }


    }
}
