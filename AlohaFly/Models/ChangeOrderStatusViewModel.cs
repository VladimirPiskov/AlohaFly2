using AlohaService.ServiceDataContracts;
using System;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace AlohaFly.Models
{
    class ChangeOrderStatusViewModel : ViewModelBase
    {
        OrderFlight Order;
        public ChangeOrderStatusViewModel(OrderFlight order)
        {
            Order = order;
            OkCommand = new DelegateCommand((_) =>
            {
                Order.SendBy = Authorization.CurentUser;
                Order.OrderStatus = GetOrderStatus();

                //DBDataExtractor<OrderFlight>.EditItem(DBProvider.Client.UpdateOrderFlight, Order);
                CloseAction();
            });
            CancelCommand = new DelegateCommand((_) => { CloseAction(); });
            Text = $"Смена статуса заказа № {Order.Id}";
            StatusInWork = Order.OrderStatus == OrderStatus.InWork;
            StatusCancelled = Order.OrderStatus == OrderStatus.Cancelled;

            StatusSent = Order.OrderStatus == OrderStatus.Sent;
            StatusCancelledWithRemains = Order.OrderStatus == OrderStatus.CancelledWithRemains;

        }

        public Action CloseAction { get; set; }
        public ICommand OkCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public string Text { set; get; }

        public bool StatusInWork { set; get; } = false;
        public bool StatusCancelled { set; get; } = false;

        public bool StatusSent { set; get; } = false;
        public bool StatusCancelledWithRemains { set; get; } = false;

        OrderStatus GetOrderStatus()
        {
            if (StatusInWork) return OrderStatus.InWork;
            if (StatusCancelled) return OrderStatus.Cancelled;

            if (StatusSent) return OrderStatus.Sent;
            if (StatusCancelledWithRemains) return OrderStatus.CancelledWithRemains;
            return OrderStatus.Closed;
        }



    }
}
