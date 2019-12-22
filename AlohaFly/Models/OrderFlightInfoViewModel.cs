using AlohaService.ServiceDataContracts;
using System;
using System.ComponentModel;
using System.Globalization;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;

namespace AlohaFly.Models
{
    public class OrderInfoViewModel : ViewModelBase
    {
        OrderFlight orderFlight;
        OrderToGo orderToGo;
        public OrderInfoViewModel(OrderFlight order)
        {
            orderFlight = order;
        }
        public OrderInfoViewModel(OrderToGo order)
        {
            orderToGo = order;
        }


        public void SetOrder(OrderFlight order)
        {
            orderFlight = order;
            if (orderFlight != null)
            {
                orderFlight.PropertyChanged += Order_PropertyChanged;
            }
            ChangeOrder();
        }

        public void SetOrder(OrderToGo order)
        {
            orderToGo = order;
            if (orderToGo != null)
            {
                orderToGo.PropertyChanged += Order_PropertyChanged;
            }
            ChangeOrder();
        }
        void ChangeOrder()
        {
            RaisePropertyChanged("Order");
            RaisePropertyChanged("SummInfoStr");
            RaisePropertyChanged("OrderDishez");
            RaisePropertyChanged("FirstColumnInfoStr");
        }

        private void Order_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DishPackages")
            {
                RaisePropertyChanged("OrderDishez");
            }
        }

        ICollectionView _orderDishez;
        public ICollectionView OrderDishez
        {
            get
            {
                QueryableCollectionView collectionViewSource = null;
                if (orderFlight != null)
                {
                    collectionViewSource = new QueryableCollectionView(orderFlight?.DishPackages);

                }
                if (orderToGo != null)
                {
                    collectionViewSource = new QueryableCollectionView(orderToGo?.DishPackages);
                }
                if (collectionViewSource != null)
                {
                    _orderDishez = collectionViewSource;
                    _orderDishez.MoveCurrentToFirst();
                }

                return _orderDishez;
            }
        }


        public string SummInfoStr
        {
            get
            {
                string s = "";
                if (orderFlight != null)
                {
                    s = $"Сумма заказа: {orderFlight?.OrderDishesSumm.ToString("c", CultureInfo.GetCultureInfo("ru-Ru"))} {Environment.NewLine}";
                    s += $"Сумма надбавки: {(orderFlight?.OrderDishesSumm * orderFlight?.ExtraCharge / 100).GetValueOrDefault().ToString("c", CultureInfo.GetCultureInfo("ru-Ru"))} {Environment.NewLine}";
                    s += $"Сумма скидки: {orderFlight?.DiscountSumm.ToString("c", CultureInfo.GetCultureInfo("ru-Ru"))} {Environment.NewLine}";
                    s += $"Итого: {orderFlight?.OrderTotalSumm.ToString("c", CultureInfo.GetCultureInfo("ru-Ru")) } ";
                }
                if (orderToGo != null)
                {

                }
                return s;

            }

        }



        public string FirstColumnInfoStr
        {
            get
            {
                string s = "";
                if (orderFlight != null)
                {
                    s = $"Заказ принял: {orderFlight.CreatedBy?.FullName} {Environment.NewLine}";
                    if ((orderFlight.OrderStatus == OrderStatus.Sent) || (orderFlight.OrderStatus == OrderStatus.Closed))
                    {
                        if (orderFlight.SendBy != null) { s += $"Заказ отправил: {orderFlight.SendBy?.FullName} {Environment.NewLine}"; }
                        if (orderFlight.WhoDeliveredPersonPerson != null) { s += $"Заказ отвез: {orderFlight.WhoDeliveredPersonPerson.FullName} {Environment.NewLine}"; }
                        s += $"Количество коробок: {orderFlight.NumberOfBoxes} {Environment.NewLine}";

                    }
                    if (orderFlight.OrderStatus == OrderStatus.CancelledWithRemains)
                    {
                        if (orderFlight.SendBy != null) { s += $"Заказ отменил: {orderFlight.SendBy.FullName} {Environment.NewLine}"; }
                    }
                    if (orderFlight.OrderStatus == OrderStatus.Cancelled)
                    {
                        if (orderFlight.SendBy != null) { s += $"Заказ удалил: {orderFlight.SendBy.FullName} {Environment.NewLine}"; }
                    }
                }
                return s;

            }

        }
    }
}
