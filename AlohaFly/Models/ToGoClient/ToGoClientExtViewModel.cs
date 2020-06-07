using AlohaService.ServiceDataContracts;
using ReactiveUI;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;

namespace AlohaFly.Models.ToGoClient
{
    class ToGoClientExtViewModel : ToGoClientViewModel
    {
        public ToGoClientExtViewModel()
        {

        }

        public ToGoClientExtViewModel(OrderCustomer orderCustomer) : base(orderCustomer)
        {

            _commentsVis = this.WhenAnyValue(a => a.OrderCustomer.Comments)
                .Select(a => !string.IsNullOrWhiteSpace(a) ? Visibility.Visible : Visibility.Collapsed)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.CommentsVis);

            _emailVis = this.WhenAnyValue(a => a.OrderCustomer.Email)
              .Select(a => !string.IsNullOrWhiteSpace(a) ? Visibility.Visible : Visibility.Collapsed)
              .ObserveOn(RxApp.MainThreadScheduler)
              .ToProperty(this, x => x.EmailVis);


            _discountPercentVis = this.WhenAnyValue(a => a.OrderCustomer.DiscountPercent)
            .Select(a => a != 0 ? Visibility.Visible : Visibility.Collapsed)
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.DiscountPercentVis);

            string n = OrderCustomer.FullName;

            _cashBakTxt=this.WhenAnyValue(a => a.OrderCustomer.CashBack, b => b.OrderCustomer.CashBackPercent, c => c.OrderCustomer.CashBackStartDate, d=>d.OrderCustomer.OrderCustomerInfo.CashBackSumm)
            .Select((a) =>OrderCustomer.CashBackStr
            )
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.TxtCashBack);

        }


        private readonly ObservableAsPropertyHelper<string> _cashBakTxt;
        public string TxtCashBack => _cashBakTxt.Value;


        private readonly ObservableAsPropertyHelper<Visibility> _emailVis;
        public Visibility EmailVis => _emailVis.Value;


        private readonly ObservableAsPropertyHelper<Visibility> _commentsVis;
        public Visibility CommentsVis => _commentsVis.Value;

        private readonly ObservableAsPropertyHelper<Visibility> _discountPercentVis;
        public Visibility DiscountPercentVis => _discountPercentVis.Value;

    }
}
