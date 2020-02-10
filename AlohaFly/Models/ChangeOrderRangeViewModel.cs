using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Telerik.Windows.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using AlohaFly.DataExtension;

namespace AlohaFly.Models
{
    class ChangeOrderRangeViewModel : ViewModelPaneReactiveObject
    {



        public ChangeOrderRangeViewModel()
        {

            DataCatalogsSingleton.Instance.ChangeOrdersDateRangeEvent += Instance_ChangeOrdersDateRangeEvent;
            this.WhenAnyValue(a => a.StartDt, b => b.StopDt).Subscribe(_ => DateChanged());
            StartDt = DataCatalogsSingleton.Instance.StartDt;
            StopDt = DataCatalogsSingleton.Instance.EndDt;
            DatePanelVis = Visibility.Collapsed;

            TodayCommand = new DelegateCommand(_ =>
            {
                DatePanelVis = Visibility.Collapsed;

                DataCatalogsSingleton.Instance.ChangeOrderDateRange(DateTime.Now.Date, DateTime.Now.Date.AddDays(1));
                //AirOrdersModelSingleton.Instance.SetNewOrdersRange(DateTime.Now.Date, DateTime.Now.Date.AddDays(1));
                //ToGoOrdersModelSingleton.Instance.SetNewOrdersRange(DateTime.Now.Date, DateTime.Now.Date.AddDays(1));

            });
            ThisMonthCommand = new DelegateCommand(_ =>
            {
                DatePanelVis = Visibility.Collapsed;

                var dt = DateTime.Now;
                DataCatalogsSingleton.Instance.ChangeOrderDateRange(new DateTime(dt.Year, dt.Month, 1), new DateTime(dt.Year, dt.Month, 1).AddMonths(1).AddDays(-1));
                //AirOrdersModelSingleton.Instance.SetNewOrdersRange();
              // ToGoOrdersModelSingleton.Instance.SetNewOrdersRange();

            });
            LastMonthCommand = new DelegateCommand(_ =>
            {
                DatePanelVis = Visibility.Collapsed;
                var dt = DateTime.Now;
                DataCatalogsSingleton.Instance.ChangeOrderDateRange(new DateTime(dt.Year, dt.Month, 1).AddMonths(-1), new DateTime(dt.Year, dt.Month, 1).AddDays(-1));

                //AirOrdersModelSingleton.Instance.SetNewOrdersRange(-1);
                // ToGoOrdersModelSingleton.Instance.SetNewOrdersRange(-1);
            });
            FreeRangeCommand = new DelegateCommand(_ =>
            {
                DatePanelVis = Visibility.Visible;
                
                

            });
            FreeRangeOkCommand = new DelegateCommand(_ =>
            {
                DatePanelVis = Visibility.Collapsed;
                DataCatalogsSingleton.Instance.ChangeOrderDateRange(StartDt, StopDt);
                //  ToGoOrdersModelSingleton.Instance.SetNewOrdersRange(StartDt, StopDt);

            });
            //AirOrdersModelSingleton.Instance.orders.CollectionChanged += Orders_CollectionChanged;
        }

        private void Instance_ChangeOrdersDateRangeEvent(object sender, EventArgs e)
        {
            StartDt = DataCatalogsSingleton.Instance.StartDt;
            StopDt = DataCatalogsSingleton.Instance.EndDt;
        }

        /*
        private void Orders_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("CurentRangeDatesStr");
            RaisePropertyChanged("OrderRangeSumm");
        }
        */
        public ICommand TodayCommand { get; set; }
        public ICommand ThisMonthCommand { get; set; }
        public ICommand LastMonthCommand { get; set; }
        public ICommand FreeRangeCommand { get; set; }
        public ICommand FreeRangeOkCommand { get; set; }

        [Reactive] public DateTime StartDt { set; get; }
        [Reactive] public DateTime StopDt { set; get; }


        

        [Reactive] public Visibility DatePanelVis { set; get; }
        
        /*
        public bool ThisMonthisChecked
        {
            get
            {
                return AirOrdersModelSingleton.Instance.ThisMonth;
            }
        }
        public bool LastMonthisChecked
        {
            get
            {
                return AirOrdersModelSingleton.Instance.LastMonth;
            }
        }
        */
        private void DateChanged()
        {
            CurentRangeDatesStr = GetCurentRangeDatesStr();
            OrderRangeSumm = GetOrderRangeSumm();
        }

        [Reactive] public string CurentRangeDatesStr { set; get; }
        [Reactive] public string OrderRangeSumm { set; get; }
        string GetCurentRangeDatesStr()
        {
            return $"Выбранный диапазон {StartDt.ToString("dd MMMM")} - {StopDt.ToString("dd MMMM yyyy")}"; 
        }
        string GetOrderRangeSumm()
        {
        
            return $"Кол-во заказов {AirOrdersModelSingleton.Instance.Orders.Sum(a => a.OrderTotalSumm).ToString("C", new CultureInfo("ru-RU"))}"; 
        }


    }
    public class IsCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var conVelue = (bool)value;
            return !conVelue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var conVelue = (bool)value;
            return !conVelue;
        }
    }
}
