using AlohaFly.Models;
using AlohaFly.Utils;
using AlohaService.ServiceDataContracts;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlohaFly.Analytics
{
    class LTVViewModel : ViewModelPaneReactiveObject
    {
        public LTVViewModel()
        {
            LoadLTVData();
            StartDt = new DateTime(2019, 1, 1);
            StopDt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            this.WhenAnyValue(a => a.StartDt, b => b.StopDt).Subscribe(_ =>
                 {
                     RefreshCustomerData();
                 });
            CustomersData.ItemPropertyChanged += CustomersData_ItemPropertyChanged;
        }

        private void CustomersData_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UseInReport")
            {
                RefreshLTVSummData();
            }
        }

        private List<AnalitikOrderData> serverData=new List<AnalitikOrderData> ();
        private void LoadLTVData()
        {
            DateTime sDate = new DateTime(2019, 1, 1);
            DateTime eDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var res = DBProvider.Client.GetAllToGoOrdersData(sDate, eDate);
            if (res.Success)
            {
                serverData = res.Result;
            }
        }        

        [Reactive] public DateTime StartDt { set; get;}
        [Reactive] public DateTime StopDt { set; get; }


        private void RefreshCustomerData()
        {
            var tmpCustomers = serverData.Where(a => a.DeleveryDate >= StartDt && a.DeleveryDate < StopDt)
                .Select(a => a.CustomerID)
                .Distinct();
            if (CustomersData == null) { CustomersData = new FullyObservableCollection<LTVCustomerInfo>(); }
            foreach (var custId in tmpCustomers)
            {
                LTVCustomerInfo cust = new LTVCustomerInfo();
                if (CustomersData.Any(a => a.Id == custId))
                {
                    cust = CustomersData.FirstOrDefault(a => a.Id == custId);
                
                }
                else
                {
                    if (DataExtension.DataCatalogsSingleton.Instance.OrderCustomerData.Data.Any(a => a.Id == custId))
                    {
                        var custData = DataExtension.DataCatalogsSingleton.Instance.OrderCustomerData.Data.FirstOrDefault(a => a.Id == custId);

                        cust.Name = $"{custData.FullName}";
                        cust.Id = custData.Id;
                    }
                    else
                    {
                        cust.Name = $"Аноним";
                    }
                    CustomersData.Add(cust);
                    cust.UseInReport = true;
                }
                cust.Summ = serverData.Where(a => a.DeleveryDate >= StartDt && a.DeleveryDate < StopDt && a.CustomerID==custId).Sum(a=>a.Summ);
                

            }



            RefreshLTVSummData();

        }

        private void RefreshLTVSummData()
        {


            var tmpData = serverData.Where(a => a.DeleveryDate >= StartDt && a.DeleveryDate < StopDt && CustomersData.Any(b => b.UseInReport && b.Id == a.CustomerID)).ToList();
            if (LTVSummData == null) { LTVSummData = new FullyObservableCollection<LTVData>(); }
            LTVSummData.Clear();
            
            for (var d = StartDt; d < StopDt; d = d.AddMonths(1))
            {
                DateTime ed = d.AddMonths(1);


                var d1 = tmpData.Where(a => a.DeleveryDate >= d && a.DeleveryDate < ed).Sum(a => a.Summ);
                var d2 = tmpData.Where(a => a.DeleveryDate >= d && a.DeleveryDate < ed).Select(a => a.CustomerID).Distinct().Count();



                LTVSummData.Add(
                new LTVData()
                {
                    Date = d,
                    Value = d2 > 0 ? d1 / d2 : 0,
                    UnikGCount = d2,

                }

                );
            }
            
            

        }

        [Reactive] public FullyObservableCollection<LTVData> LTVSummData { set; get; }
        [Reactive] public FullyObservableCollection<LTVCustomerInfo> CustomersData { set; get; }


       
    }
    public class LTVData : INotifyPropertyChanged
    {
        public LTVData()
        { }
        public DateTime Date { set; get; }
        public decimal Value { set; get; }
        public int UnikGCount { set; get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }


    public class LTVCustomerInfo: INotifyPropertyChanged
    {
        public LTVCustomerInfo()
        { }

        public long Id { set; get; }
        public string  Name { set; get; }
        public decimal Summ { set; get; }
        public bool  UseInReport { set; get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

}
