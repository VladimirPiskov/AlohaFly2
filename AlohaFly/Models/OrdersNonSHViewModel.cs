using AlohaService.ServiceDataContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;

namespace AlohaFly.Models
{
    class OrdersNonSHViewModel : ViewModelPane
    {
        string ReturnMessage = "";
        public OrdersNonSHViewModel()
        {
            CreateSHIToFlyInvoice = new DelegateCommand(_ =>
            {
                ReturnMessage = "";
                mCreateSHToFlyInvoice((OrderFlight)Orders.CurrentItem);
                RaisePropertyChanged("Orders");
                ShowEndMsg();
            });
            CreateSHIToGoInvoice = new DelegateCommand(_ =>
            {
                ReturnMessage = "";
                mCreateSHToGoInvoice((OrderToGo)OrdersToGo.CurrentItem);
               
                ShowEndMsg();
            });
            CreateAllToGoSHInvoice = new DelegateCommand(_ =>
            {
                ReturnMessage = "";
                SendAllToGo();
                ShowEndMsg();
            });


            


            CreateAllToFlySHInvoice = new DelegateCommand(_ =>
            {
                ReturnMessage = "";
                foreach (var ord in Orders)
                {
                    mCreateSHToFlyInvoice((OrderFlight)ord);
                }
                RaisePropertyChanged("Orders");
                ShowEndMsg();
            });


            CreateAllSHInvoice = new DelegateCommand(_ =>
            {
                ReturnMessage = "";
                CreateAllToGoSHInvoice.Execute(null);
                RaisePropertyChanged("Orders");
                ShowEndMsg();
            });


            CreateAllSHInvoiceByData = new DelegateCommand(_ =>
            {
                ReturnMessage = "";

                var oldOrdeers = new List<long>();
                while (ToGoOrdersModelSingleton.Instance.Orders.Any(a => !oldOrdeers.Contains(a.Id)))
                {
                    var ord = ToGoOrdersModelSingleton.Instance.Orders.FirstOrDefault(a => !oldOrdeers.Contains(a.Id));
                    mCreateSHToGoInvoice((OrderToGo)ord);
                    oldOrdeers.Add(ord.Id);
                }

                foreach (var ord in AirOrdersModelSingleton.Instance.Orders)
                {
                    mCreateSHToFlyInvoice((OrderFlight)ord);
                }
                RaisePropertyChanged("Orders");
            });
        }

       

        private void SendAllToGo()
        {
            var oldOrdeers = new List<long>();
            while (ToGoOrdersModelSingleton.Instance.OrdersNonSH.Any(a => !oldOrdeers.Contains(a.Id)))
            {
                var ord = ToGoOrdersModelSingleton.Instance.OrdersNonSH.FirstOrDefault(a => !oldOrdeers.Contains(a.Id));
                mCreateSHToGoInvoice((OrderToGo)ord);
                oldOrdeers.Add(ord.Id);
            }
        }

        private void ShowEndMsg()
        {
            if (!String.IsNullOrWhiteSpace(ReturnMessage))
            {
                UI.UIModify.ShowAlert(ReturnMessage);
            }
        }

        private void mCreateSHToFlyInvoice(OrderFlight order)
        {
            string ErrMess = "";
            if (SH.SHWrapper.CreateSalesInvoiceSync(order, out ErrMess))
            {

            }
            else
            {
                ReturnMessage += $"Ошибка выгрузки заказа ToFly №{order.Id} {Environment.NewLine} {ErrMess}";
            }
            DBProvider.UpdateOrderFlight(order);
            RaisePropertyChanged("Orders");
        }

        private void mCreateSHToGoInvoice(OrderToGo order)
        {
            string ErrMess = "";
            if (SH.SHWrapper.CreateSalesInvoiceSync(order, out ErrMess))
            {
                DBProvider.Client.SetToGoSHValue(true, order.Id);
                
            }
            else
            {
                ReturnMessage += $"Ошибка выгрузки заказа ToGo №{order.Id} {Environment.NewLine} {ErrMess}";
            }

        }

        public ICommand CreateSHIToFlyInvoice { get; set; }
        public ICommand CreateSHIToGoInvoice { get; set; }
        public ICommand CreateAllToGoSHInvoice { get; set; }
        public ICommand CreateAllToFlySHInvoice { get; set; }

        public ICommand CreateAllSHInvoice { get; set; }

        public ICommand CreateAllSHInvoiceByData { get; set; }


        ICollectionView _orders;
        public ICollectionView Orders
        {
            get
            {
                if (_orders == null)
                {
                    QueryableCollectionView collectionViewSource = new QueryableCollectionView(AirOrdersModelSingleton.Instance.OrdersNonSH);
                    _orders = collectionViewSource;
                      _orders.MoveCurrentToFirst();
                }

                return _orders;
            }
        }

        ICollectionView _orderstoGo;
        public ICollectionView OrdersToGo
        {
            get
            {
                if (_orderstoGo == null)
                {
                    QueryableCollectionView collectionViewSource = new QueryableCollectionView(ToGoOrdersModelSingleton.Instance.OrdersNonSH);
                    _orderstoGo = collectionViewSource;
                      _orderstoGo.MoveCurrentToFirst();
                }

                return _orderstoGo;
            }
        }
    }
}
