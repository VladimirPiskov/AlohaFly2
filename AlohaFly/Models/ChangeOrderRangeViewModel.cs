using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace AlohaFly.Models
{
    class ChangeOrderRangeViewModel : ViewModelBase
    {
        public ChangeOrderRangeViewModel()
        {
            StartDt = AirOrdersModelSingleton.Instance.StartDt;
            StopDt = AirOrdersModelSingleton.Instance.EndDt;
            TodayCommand = new DelegateCommand(_ =>
            {
                DatePanelVis = Visibility.Collapsed;
                AirOrdersModelSingleton.Instance.SetNewOrdersRange(DateTime.Now.Date, DateTime.Now.Date.AddDays(1));
                //ToGoOrdersModelSingleton.Instance.SetNewOrdersRange(DateTime.Now.Date, DateTime.Now.Date.AddDays(1));

            });
            ThisMonthCommand = new DelegateCommand(_ =>
            {
                DatePanelVis = Visibility.Collapsed;
                AirOrdersModelSingleton.Instance.SetNewOrdersRange();
              // ToGoOrdersModelSingleton.Instance.SetNewOrdersRange();

            });
            LastMonthCommand = new DelegateCommand(_ =>
            {
                DatePanelVis = Visibility.Collapsed;
                AirOrdersModelSingleton.Instance.SetNewOrdersRange(-1);
               // ToGoOrdersModelSingleton.Instance.SetNewOrdersRange(-1);
            });
            FreeRangeCommand = new DelegateCommand(_ =>
            {
                DatePanelVis = Visibility.Visible;
                //AirOrdersModelSingleton.Instance.SetNewOrdersRange(StartDt, StopDt);

            });
            FreeRangeOkCommand = new DelegateCommand(_ =>
            {
                DatePanelVis = Visibility.Collapsed;
                AirOrdersModelSingleton.Instance.SetNewOrdersRange(StartDt, StopDt);
              //  ToGoOrdersModelSingleton.Instance.SetNewOrdersRange(StartDt, StopDt);

            });
            AirOrdersModelSingleton.Instance.orders.CollectionChanged += Orders_CollectionChanged;
        }

        private void Orders_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("CurentRangeDatesStr");
            RaisePropertyChanged("OrderRangeSumm");
        }

        public ICommand TodayCommand { get; set; }
        public ICommand ThisMonthCommand { get; set; }
        public ICommand LastMonthCommand { get; set; }
        public ICommand FreeRangeCommand { get; set; }
        public ICommand FreeRangeOkCommand { get; set; }

        public DateTime StartDt { set; get; }
        public DateTime StopDt { set; get; }


        public Visibility datePanelVis = Visibility.Collapsed;

        public Visibility DatePanelVis
        {
            set
            {
                datePanelVis = value;
                RaisePropertyChanged("DatePanelVis");
            }
            get
            {
                return datePanelVis;
            }
        }

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

        public string CurentRangeDatesStr
        {
            get { return $"Выбранный диапазон {AirOrdersModelSingleton.Instance.StartDt.ToString("dd MMMM")} - {AirOrdersModelSingleton.Instance.EndDt.ToString("dd MMMM yyyy")}"; }
        }
        public string OrderRangeSumm
        {
            //get { return $"Cумма всех заказов {AirOrdersModelSingleton.Instance.Orders.Sum(a=>a.OrderTotalSumm).ToString("C", new CultureInfo("ru-RU"))}"; }
            get { return $"Кол-во заказов {AirOrdersModelSingleton.Instance.Orders.Sum(a => a.OrderTotalSumm).ToString("C", new CultureInfo("ru-RU"))}"; }
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
