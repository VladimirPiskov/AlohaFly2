using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Telerik.Windows.Controls;

namespace AlohaFly.Models
{
    public class MainUIModel
    {

        public ObservableCollection<RadPane> Panes { set; get; } = new ObservableCollection<RadPane>();

        public MainUIModel()
        {

        }

        public event EventHandler BusyStarted;
        public event EventHandler BusyStoped;
        public event EventHandler<string> BusyMessageSended;

        protected virtual void OnBusyStarted()
        {
            BusyStarted?.Invoke(this, new EventArgs());
        }
        protected virtual void OnBusyStoped()
        {
            BusyStoped?.Invoke(this, new EventArgs());
        }
        protected virtual void OnBusyMessageSended(string msg)
        {
            BusyMessageSended?.Invoke(this, msg);
        }

        int _busyCount = 0;
        int busyCount
        {
            set
            {
                if (_busyCount != value)
                {
                    _busyCount = value;
                    if (_busyCount <= 0)
                    {
                        _busyCount = 0;
                        OnBusyStoped();
                    }
                    else
                    {
                        OnBusyStarted();
                    }

                }
            }
            get
            {
                return _busyCount;
            }
        }

        public void StartBusy()
        {

            busyCount++;
        }

        public void StopBusy()
        {
            busyCount--;
        }

        public void SendBusyContent(string msg)
        {
            OnBusyMessageSended(msg);
        }




        public void RemovePane(UserControl uIElement)
        {
            if (Panes.Where(a => a.Content == uIElement).Count() > 0)
            {
                Panes.Remove(Panes.Single(a => a.Content == uIElement));
            }
        }



        void ShowPane(RadPane r)
        {
            r.IsHidden = false;
            r.IsActive = true;
            r.MoveToDocumentHost();
        }

        public ObservableCollection<Interface.INeedSave> OpenVeiwModels
        {
            get
            {
                return new ObservableCollection<Interface.INeedSave>(Panes.Select(a => (Interface.INeedSave)((UserControl)a.Content).DataContext).ToList());
            }
        }



        public void RadDocking_PreviewClose(object sender, Telerik.Windows.Controls.Docking.StateChangeEventArgs e)
        {
            if (e.Panes != null && e.Panes.Count() > 0 && e.Panes.First() is RadPane)
            {
                var r = (e.Panes.First() as RadPane);
                //var vm = (Models)(r.Content as UserControl).DataContext;

                if ((r.Content as UserControl).DataContext is Models.ViewModelPane vm)
                {
                    e.Handled = !vm.SaveChanesAsk();
                }

                if ((r.Content as UserControl).DataContext is Models.ViewModelPaneReactiveObject vm2)
                {
                    e.Handled = !vm2.SaveChanesAsk();
                }

            }
        }


        public bool AddAirOrderPaneOpen()
        {
            //return true;
            return Panes.Any(a => (a.IsVisible && (a.Content as UI.CtrlAddOrder) != null));

        }


        public bool AddToGoOrderPaneOpen()
        {
            //return true;
            return Panes.Any(a => (a.IsVisible && (a.Content as UI.CtrlAddToGoOrder) != null));

        }


        public void AfterSaveNewToGoOrder()
        {
            try

            {
                if (Panes.Any(a => (a.IsVisible && (a.Content is UI.CtrlToGoOrders))))
                {
                    var pane = Panes.FirstOrDefault(a => (a.IsVisible && (a.Content is UI.CtrlToGoOrders)));
                    ((pane.Content as UI.CtrlToGoOrders).DataContext as ToGoOrdersViewModel).AfterAddOrder();
                    ShowPane(pane);
                }
            }
            catch
            { }
        }

        public void AddPane(UserControl UIElement)
        {

            if (UIElement.DataContext is Models.ViewModelPane vm)
            {
                if (Panes.Where(a => a.Header?.ToString().Replace("*", "") == vm.Header.Replace("*", "") && a.IsVisible).Count() > 0)
                {
                    ShowPane(Panes.Single(a => a.Header?.ToString().Replace("*", "") == vm.Header.Replace("*", "") && a.IsVisible));
                }
                else
                {

                    var pane = new RadPane
                    {
                        Content = UIElement,
                    };
                    vm.CloseAction = new Action(() =>
                    {
                        pane.IsHidden = true;
                    });
                    pane.DataContext = vm;
                    pane.IsVisibleChanged += (sender, e) =>
                    {
                    };

                    Binding binding = new Binding();
                    binding.Path = new PropertyPath("Header"); // свойство элемента-источника
                    pane.SetBinding(RadPane.HeaderProperty, binding); // установка привязки для элемента-приемника
                    Panes.Add(pane);
                }
            }
            else if (UIElement.DataContext is Models.ViewModelPaneReactiveObject vm2)
            {

                if (Panes.Where(a => a.Header?.ToString().Replace("*", "") == vm2.Header.Replace("*", "") && a.IsVisible).Count() > 0)
                {
                    ShowPane(Panes.Single(a => a.Header?.ToString().Replace("*", "") == vm2.Header.Replace("*", "") && a.IsVisible));
                }
                else
                {

                    var pane = new RadPane
                    {
                        Content = UIElement,
                    };
                    vm2.CloseAction = new Action(() =>
                    {
                        pane.IsHidden = true;
                    });
                    pane.DataContext = vm2;
                    pane.IsVisibleChanged += (sender, e) =>
                    {
                    };

                    Binding binding = new Binding();
                    binding.Path = new PropertyPath("Header"); // свойство элемента-источника
                    pane.SetBinding(RadPane.HeaderProperty, binding); // установка привязки для элемента-приемника
                    Panes.Add(pane);
                }
            }
        }



        private MenuItemsCollection _items;
        public MenuItemsCollection MenuItems
        {
            get
            {
                if (_items == null)
                {
                    GenerateMenuItems();
                }
                return _items;
            }
        }

        private Dictionary<string, List<MenuItemCatalog>> MainMenu;

        private void GenerateMenuItems()
        {

            MainMenu = new Dictionary<string, List<MenuItemCatalog>>()
            {
                {"Аналитика", Analytics.MenuItems.ItemsCatalog},
                { "Справочники", CatalogSelector.ItemsCatalog},
                {"Заказы", CatalogSelector.OrderItemsCatalog},
                {"Отчеты", CatalogSelector.ReportsCatalog},
                {"Импорт", CatalogSelector.ImportItemsCatalog},
                {"Пользователи", CatalogSelector.UserItemsCatalog},


            };


            _items = new MenuItemsCollection();
            foreach (var n in MainMenu)
            {
                var menuItem = new MenuItem(n.Key);
                foreach (var itm in n.Value)
                {
                    if (itm.IsVisible)
                    {
                        menuItem.Items.Add(itm);
                    }
                }
                if (menuItem.Items.Count > 0)
                {
                    _items.Add(menuItem);
                }
            }







            /*
            var Cats = new MenuItem("Справочники");
            foreach (var itm in CatalogSelector.ItemsCatalog)
            {
                if (itm.IsVisible)
                {
                    Cats.Items.Add(itm);
                }
            }
            if (Cats.Items.Count > 0)
            {
                _items.Add(Cats);
            }
            

            var pOrders = new MenuItem("Заказы");
            foreach (var itm in CatalogSelector.OrderItemsCatalog)
            {
                if (itm.IsVisible)
                {
                    pOrders.Items.Add(itm);
                }
            }
            if (pOrders.Items.Count > 0)
            {
                _items.Add(pOrders);
            }

            var pReps = new MenuItem("Отчеты");
            foreach (var itm in CatalogSelector.ReportsCatalog)
            {
                if (itm.IsVisible)
                {
                    pReps.Items.Add(itm);
                }
            }
            if (pReps.Items.Count > 0)
            {
                _items.Add(pReps);
            }

            var pUser = new MenuItem("Пользователи");
            foreach (var itm in CatalogSelector.UserItemsCatalog)
            {
                if (itm.IsVisible)
                {
                    pUser.Items.Add(itm);
                }
            }
            if (pUser.Items.Count > 0)
            {
                _items.Add(pUser);
            }

    */


        }

    }

    public class MainUIViewModel : ViewModelBase
    {
        public MainUIViewModel(MainUIModel _model)
        {
            model = _model;
            model.BusyStarted += new EventHandler((sender, e) =>
             {
                 IsBusy = true;
             });
            model.BusyStoped += new EventHandler((sender, e) =>
            {
                IsBusy = false;
            });
            model.BusyMessageSended += new EventHandler<string>((sender, s) =>
                  {
                      InvokeOnUIThread(() =>
                      {
                          this.BusyContent = s;
                      });
                  });
        }

        public MainUIModel model;


        private bool isBusy;
        private string busyContent = "Инициализация";
        public bool IsBusy
        {
            get { return this.isBusy; }
            set
            {
                if (this.isBusy != value)
                {
                    this.isBusy = value;
                    this.OnPropertyChanged(() => this.IsBusy);
                }
            }
        }

        public string BusyContent
        {
            get { return this.busyContent; }
            set
            {
                if (this.busyContent != value)
                {
                    this.busyContent = value;
                    this.OnPropertyChanged(() => this.BusyContent);
                }
            }
        }


        public ObservableCollection<RadPane> Panes
        {
            get
            {
                return model.Panes;
            }

            set
            {
                if (model.Panes != value)
                {
                    model.Panes = value;
                    this.OnPropertyChanged(() => this.Panes);
                }
            }
        }

        public MenuItemsCollection MenuItems
        {
            get
            {
                return model.MenuItems;
            }
        }

    }

}
