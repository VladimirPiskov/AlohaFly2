using AlohaFly.DataExtension;
using AlohaService.ServiceDataContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;


namespace AlohaFly.Models
{
    class AirCompaniesOrdersViewModel : ViewModelPane
    {
        public AirCompaniesOrdersViewModel()
        {
            GoToOrdersCommand = new DelegateCommand((_) =>
            {
                UI.UIModify.ShowInConstruction();
            });

            CloseOrdersCommand = new DelegateCommand((_) =>
            {
                MainClass.CloseChecksByAirCompAsk(CurentAirCompanyOrders.AirCompany, CurentAirCompanyOrders.Orders.ToList());
            });

            CloseOrderCommand = new DelegateCommand(_ =>
            {
                if (CurentOrder != null)
                {
                    MainClass.CloseSingleCheck(CurentOrder);
                    RaisePropertyChanged("CanEditCurentOrder");
                    RaisePropertyChanged("CanCloseCurentOrder");
                }
            });

            PrintReestr = new DelegateCommand((_) =>
            {
                (new Reports.ExcelReports()).OrdersToExcelByComp(CurentAirCompanyOrders, DataCatalogsSingleton.Instance.StartDt, DataCatalogsSingleton.Instance.EndDt);
            });
            PrintReestrAll = new DelegateCommand((_) =>
            {
                (new Reports.ExcelReports()).AllOrdersToExcelByComps(Models.AirOrdersModelSingleton.Instance.AirCompanyOrders.ToList(), DataCatalogsSingleton.Instance.StartDt, DataCatalogsSingleton.Instance.EndDt);
            });
          //  AirOrdersModelSingleton.Instance.orders.CollectionChanged += Orders_CollectionChanged;
        }

        /*
        private void Orders_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // RaisePropertyChanged("_airCompanies");
            QueryableCollectionView collectionViewSource = new QueryableCollectionView(AirOrdersModelSingleton.Instance.AirCompanyOrders.OrderBy(a => a.Name));
            _airCompanies = collectionViewSource;

            RaisePropertyChanged("AirCompanies");
        }
        */
        public Models.ChangeOrderRangeViewModel changeOrderRangeViewModel { set; get; } = new ChangeOrderRangeViewModel();

        public ICommand GoToOrdersCommand { get; set; }
        public ICommand CloseOrdersCommand { get; set; }
        public ICommand CloseOrderCommand { get; set; }
        public ICommand PrintReestr { get; set; }
        public ICommand PrintReestrAll { get; set; }


        public AirCompanyOrders CurentAirCompanyOrders
        {
            get { return (AirCompanyOrders)AirCompanies.CurrentItem; }
        }



        public OrderFlight curentOrder { set; get; }
        public OrderFlight CurentOrder
        {
            set
            {

                if (curentOrder != value)
                {
                    curentOrder = value;

                    RaisePropertyChanged("CurentOrder");
                    RaisePropertyChanged("OrderHeader");
                    RaisePropertyChanged("CanEditCurentOrder");
                    RaisePropertyChanged("CanCloseCurentOrder");
                }
            }
            get
            {
                return curentOrder;
            }
        }

        public bool CanCloseCurentOrder
        {
            get
            {
                return CurentOrder == null ? false : CurentOrder.OrderStatus == OrderStatus.Sent;
            }
        }


        private List<RadMenuItem> _printInvoiceitems;
        public List<RadMenuItem> PrintInvoiceItems
        {
            get
            {
                if (_printInvoiceitems == null)
                {
                    _printInvoiceitems = new List<RadMenuItem>() {
                        new RadMenuItem()
                        {
                            Command = new DelegateCommand((_) => {
                               Reports.PDFExport.AirCopmInvoicesToPDFRus(CurentAirCompanyOrders.Orders.ToList(),
                                   AlohaService.ExcelExport.ExportHelper.ExportToExcelWorkbookRussian);
                                }),
                            Header = "На русском языке"
                        },

                        new RadMenuItem()
                        {
                            Command = new DelegateCommand((_) => {
                                Reports.PDFExport.AirCopmInvoicesToPDFRus(CurentAirCompanyOrders.Orders.ToList(),
                                   AlohaService.ExcelExport.ExportHelper.ExportToExcelWorkbookEnglish);
                            }),
                            Header = "На английском языке"
                        },
                         new RadMenuItem()
                        {
                            Command = new DelegateCommand((_) => {
                               Reports.PDFExport.AirCopmInvoicesToPDFRus(CurentAirCompanyOrders.Orders.ToList(),
                                   AlohaService.ExcelExport.ExportHelper.ExportToExcelWorkbookRussian,true);
                                }),
                            Header = "На русском языке со скидкой"
                        },
                           new RadMenuItem()
                        {
                            Command = new DelegateCommand((_) => {
                                Reports.PDFExport.AirCopmInvoicesToPDFRus(CurentAirCompanyOrders.Orders.ToList(),
                                   AlohaService.ExcelExport.ExportHelper.ExportToExcelWorkbookEnglish,true);
                            }),
                            Header = "На английском языке со скидкой"
                        },
                        };

                }
                return _printInvoiceitems;
            }
        }

        ICollectionView _airCompanies;
        public ICollectionView AirCompanies
        {
            get
            {
                if (_airCompanies == null)
                {
                    QueryableCollectionView collectionViewSource = new QueryableCollectionView(AirOrdersModelSingleton.Instance.AirCompanyOrders.OrderBy(a => a.Name));
                    _airCompanies = collectionViewSource;
                    _airCompanies.CurrentChanged += new EventHandler((_, __) =>
                    {
                        RaisePropertyChanged("SelectedCompOrders");
                    });
                    _airCompanies.MoveCurrentToFirst();
                }

                return _airCompanies;
            }
        }


        public ICollectionView SelectedCompOrders
        {
            get
            {
                if (CurentAirCompanyOrders?.Orders == null) return null;

                return new QueryableCollectionView(CurentAirCompanyOrders.Orders);
            }
        }


    }

    public class AirCompanyOrders : ViewModelBase
    {

        public AirCompanyOrders(long AirId)
        {
            Id = AirId;
            //AirOrdersModelSingleton.Instance.orders.CollectionChanged += Orders_CollectionChanged;
        }

        public AirCompany AirCompany
        {
            get
            {
                return DataExtension.DataCatalogsSingleton.Instance.AirCompanyData.Data.SingleOrDefault(a => a.Id == Id);
            }
        }
        /*
        private void Orders_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("Orders");
        }
        */
        [Display(AutoGenerateField = false)]
        public IEnumerable<OrderFlight> Orders
        {
            get { return AirOrdersModelSingleton.Instance.Orders.Where(a => a.AirCompanyId == Id); }
        }


        public long Id { set; get; }
        public string Name
        {
            get
            {
                return DataExtension.DataCatalogsSingleton.Instance.AirCompanyData.Data.SingleOrDefault(a => a.Id == Id).Name;
            }
        }
        public int OrdersCount
        {
            get { return Orders.Count(); }
        }
        public decimal OrdersSumm
        {
            get { return Orders.Sum(a => a.OrderSumm); }
        }
        public decimal OrdersDiscount
        {
            get { return Orders.Sum(a => a.DiscountSumm); }
        }
        public decimal OrdersDiscountPercent
        {
            get { return Orders.Sum(a => a.OrderDishesSumm) == 0 ? 0 : Orders.Sum(a => a.DiscountSumm) / Orders.Sum(a => a.OrderDishesSumm); }
        }

        public decimal OrdersTotalSumm
        {
            get { return Orders.Sum(a => a.OrderTotalSumm); }
        }


        public bool Closed { get { return Orders.Where(a => (a.OrderStatus != OrderStatus.Closed) && (a.OrderStatus != OrderStatus.CancelledWithRemains)).Count() == 0; } }


    }



}
