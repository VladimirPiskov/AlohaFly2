using AlohaFly.Utils;
using AlohaService.Interfaces;
using AlohaService.ServiceDataContracts;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;


namespace AlohaFly.Models
{

    public class AddLabelsViewModel : ViewModelPane
    {
        IOrderLabel CurentOrder;
        public AddLabelsViewModel(IOrderLabel order)
        {
            CurentOrder = order;
            DishPackages = new FullyObservableCollection<IDishPackageLabel>();
            foreach (var dp in order.DishPackagesForLab)
            {
                dp.PrintLabel = DataExtension.DataCatalogsSingleton.Instance.ItemLabelsInfo.Where(a => a.ParenItemId == dp.DishId).Count() > 0;
                dp.LabelSeriesCount = dp.LabelsCount;
                DishPackages.Add(dp);

            }


            order.PropertyChanged += Order_PropertyChanged;
            OrderDish = DishPackages.First();
            AllDishGridVis = Visibility.Collapsed;
            OrderDishGridVis = Visibility.Visible;
            Init();
        }

        /*
        public override void Dispose(bool disposing)
        {

            base.Dispose(disposing);
        }
        */

        private void Order_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == "DishPackages")
                {
                    DishPackages.Clear();
                    foreach (var dp in ((IOrderLabel)sender).DishPackagesForLab)
                    {
                        dp.PrintLabel = DataExtension.DataCatalogsSingleton.Instance.ItemLabelsInfo.Where(a => a.ParenItemId == dp.DishId).Count() > 0;
                        dp.LabelSeriesCount = dp.LabelsCount;
                        DishPackages.Add(dp);

                    }
                    OrderDish = DishPackages.First();
                }
            }
            catch (Exception ee)
            {

            }
        }

        public AddLabelsViewModel(Dish dish, bool _openItemsOnly = false)
        {
            openItemsOnly = _openItemsOnly;
            LabelDish = dish;
            AllDishGridVis = Visibility.Visible;
            OrderDishGridVis = Visibility.Collapsed;
            Init();

        }

        void Init()
        {
            ItemsSource.CurrentChanged += ItemsSource_CurrentChanged1; ;
            DataExtension.DataCatalogsSingleton.Instance.ItemLabelsInfo.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler((_, __) =>
            {
                UpdateLabels();
                RaisePropertyChanged("ItemsSource");
            });

            DataExtension.DataCatalogsSingleton.Instance.Dishes.ItemPropertyChanged += new EventHandler<ItemPropertyChangedEventArgs>((sender, e) =>
            {
                RaisePropertyChanged("DishName");
                RaisePropertyChanged("CurentDishMaxSerCount");
                RaisePropertyChanged("ItemsSource");
                RaisePropertyChanged("OrdersDishes");
                RaisePropertyChanged("LabelRussianName");
                RaisePropertyChanged("LabelEnglishName");

            });

            AddLabelCommand = new DelegateCommand(_ =>
            {
                if (DataExtension.DataCatalogsSingleton.Instance.AddLabelInfo(LabelDish.Id))
                {
                    ItemsSource.MoveCurrentToLast();
                }

            });
            RemoveLabelCommand = new DelegateCommand(_ =>
            {
                if (ItemsSource.CurrentItem != null)
                {
                    DataExtension.DataCatalogsSingleton.Instance.RemoveLabelInfo((ItemLabelInfo)ItemsSource.CurrentItem);

                    ItemsSource.MoveCurrentToLast();
                }
            });

            PrintCommand = new DelegateCommand(_ =>
            {
                if (OrderDishGridVis == Visibility.Visible)
                {
                    var pvm = new LabelsPrint.LabelPapersVisualViewModel(CurentOrder);
                    pvm.ShowMe();
                }
                else
                {
                    var pvm = new LabelsPrint.LabelPapersVisualViewModel(LabelDish);
                    pvm.ShowMe();
                }
            });


            /*
            PrintCommand = new DelegateCommand(_ =>
            {
                Print.OrderPrintInfo oi = new Print.OrderPrintInfo()
                {
                    BoardName = CurentOrder.FlightNumber,
                    OrderNumber = CurentOrder.Id,
                    OrderTime = CurentOrder.DeliveryDate,
                    OrderType = 0,
                    PrepearingTime = CurentOrder.DeliveryDate,
                    Labels = new List<Print.LabelPrintInfo>()

                };
                //int num = 0;


                foreach (var d in CurentOrder.DishPackages.OrderBy(a=>a.PositionInOrder))
                {
                    if (!d.PrintLabel) { continue; }
                    //foreach(var l in d.DishId)
                    for (int sNum = 0; sNum < d.LabelSeriesCount; sNum++)
                    {
                        int nCount = DataExtension.DataCatalogsSingleton.Instance.ItemLabelsInfo.Where(a => a.ParenItemId == d.DishId).Count();
                        foreach (var l in DataExtension.DataCatalogsSingleton.Instance.ItemLabelsInfo.Where(a => a.ParenItemId == d.DishId).OrderBy(a => a.SerialNumber))
                        {
                            oi.Labels.Add(
                            new Print.LabelPrintInfo()
                            {
                                BarCode = d.Dish.Barcode,
                                Comment = l.Message,
                                ItemName1 = (d.Dish.RussianName != null) ? d.Dish.RussianName : "",
                                ItemName2 = (d.Dish.EnglishName != null) ? d.Dish.EnglishName : "",
                                Order = l.SerialNumber,
                                SubItemmName2 = (l.NameEng != null) ? l.NameEng : "",
                                SubItemName1 = (l.NameRus != null) ? l.NameRus : "",
                                CountStr = nCount.ToString()

                            }
                            );
                        }
                    }
                }

                Print.Print_label p = new Print.Print_label();
                p.Print(oi, true);
            });
            */
        }


        FullyObservableCollection<IDishPackageLabel> DishPackages { set; get; }


        Visibility allDishGridVis { set; get; }
        public Visibility AllDishGridVis
        {
            set
            {

                allDishGridVis = value;
                RaisePropertyChanged("AllDishGridVis");
            }
            get

            {
                return allDishGridVis;

            }
        }

        Visibility orderDishGridVis { set; get; }
        public Visibility OrderDishGridVis
        {
            set
            {

                orderDishGridVis = value;
                RaisePropertyChanged("OrderDishGridVis");
            }
            get

            {
                return orderDishGridVis;

            }
        }


        ICollectionView _orderDishes;
        public ICollectionView OrdersDishes
        {
            get
            {
                if (DishPackages != null)
                {
                    if (_orderDishes == null)
                    {
                        QueryableCollectionView collectionViewSource = new QueryableCollectionView(DishPackages);
                        _orderDishes = collectionViewSource;
                        _orderDishes.MoveCurrentToFirst();
                    }
                }
                return _orderDishes;
            }
        }

        private void ItemsSource_CurrentChanged1(object sender, EventArgs e)
        {
            RaisePropertyChanged("RemoveLabelCommandEnable");
        }

        private void ItemsSource_CurrentChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ItemLabelsInfo_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public ICommand AddLabelCommand { get; set; }
        public ICommand RemoveLabelCommand { get; set; }
        public ICommand PrintCommand { get; set; }


        public bool RemoveLabelCommandEnable
        {
            get
            {
                return (ItemsSource.CurrentItem != null);
            }
        }



        void UpdateLabels()
        {

            long Did = 0;
            /*
            if (AllDishGridVis == Visibility.Visible) {
                Did = LabelDish.Id;
            }
            else
            {
                Did = OrderDish.DishId;
            }
            */
            Did = LabelDish.Id;
            Labels.Clear();
            if (Labels != null)
            {

                foreach (var l in DataExtension.DataCatalogsSingleton.Instance.ItemLabelsInfo.Where(a => a.ParenItemId == Did).OrderBy(a => a.Id))
                {
                    Labels.Add(l);
                }
            }
        }

        FullyObservableCollection<ItemLabelInfo> labels = new FullyObservableCollection<ItemLabelInfo>();
        public FullyObservableCollection<ItemLabelInfo> Labels
        {

            get
            {



                return labels;
            }
        }

        Dish labelDish;
        public Dish LabelDish
        {
            set
            {
                if (value == null)
                {
                    labelDish = null;
                    return;
                }
                if ((labelDish == null) || (value.Id != labelDish.Id))
                {
                    labelDish = value;
                    UpdateLabels();
                    RaisePropertyChanged("LabelDish");
                    RaisePropertyChanged("DishName");
                    RaisePropertyChanged("CurentDishMaxSerCount");
                    RaisePropertyChanged("ItemsSource");
                    RaisePropertyChanged("LabelRussianName");
                    RaisePropertyChanged("LabelEnglishName");
                }
            }
            get
            {
                return labelDish;
            }

        }


        IDishPackageLabel orderDish;
        public IDishPackageLabel OrderDish
        {
            set
            {
                if (value == null)
                {
                    orderDish = null;
                    LabelDish = null;
                    return;
                }
                if ((orderDish == null) || (value.DishId != labelDish.Id))
                {
                    orderDish = value;
                    LabelDish = DataExtension.DataCatalogsSingleton.Instance.Dishes.Single(a => a.Id == orderDish.DishId);
                    UpdateLabels();
                    RaisePropertyChanged("OrderDish");
                    RaisePropertyChanged("LabelDish");
                    RaisePropertyChanged("DishName");
                    RaisePropertyChanged("CurentDishMaxSerCount");
                    RaisePropertyChanged("ItemsSource");
                }
            }
            get
            {
                return orderDish;
            }

        }

        public string DishName
        {
            get
            {
                return LabelDish.Name;
            }
        }


        //public string labelRussianName;
        public string LabelRussianName
        {
            get
            {
                return LabelDish?.LabelRussianName;
            }
            set
            {
                if (LabelDish != null && LabelDish?.LabelRussianName != value)
                {
                    LabelDish.LabelRussianName = value;
                    RaisePropertyChanged("LabelRussianName");
                }
            }
        }

        public string LabelEnglishName
        {
            get
            {
                return LabelDish?.LabelEnglishName;
            }
            set
            {
                if (LabelDish != null && LabelDish?.LabelEnglishName != value)
                {
                    LabelDish.LabelEnglishName = value;
                    RaisePropertyChanged("LabelEnglishName");
                }
            }
        }

        bool curentDishEdited = false;
        public int CurentDishMaxSerCount
        {
            get
            {
                if (LabelDish == null) return 0;
                return LabelDish.ToFlyLabelSeriesCount;
            }
            set
            {
                if ((LabelDish != null) && (LabelDish.ToFlyLabelSeriesCount != value))
                {
                    LabelDish.ToFlyLabelSeriesCount = value;
                    DBDataExtractor<Dish>.EditItem(DBProvider.Client.UpdateDish, LabelDish);

                    RaisePropertyChanged("CurentDishMaxSerCount");

                    if ((OrdersDishes != null) && (OrdersDishes.CurrentItem != null))
                    {
                        var dp = (IDishPackageLabel)(OrdersDishes.CurrentItem);
                        dp.LabelSeriesCount = dp.LabelsCount;
                    }
                    curentDishEdited = true;
                }
            }
        }

        bool openItemsOnly = false;

        ICollectionView _dishitemsSource;
        public ICollectionView DishItemsSource
        {
            get
            {
                if (_dishitemsSource == null)
                {
                    if (openItemsOnly)
                    {
                        QueryableCollectionView collectionViewSource = new QueryableCollectionView(DataExtension.DataCatalogsSingleton.Instance.GetOpenDishes());
                        _dishitemsSource = collectionViewSource;
                        _dishitemsSource.MoveCurrentToFirst();
                    }
                    else
                    {
                        QueryableCollectionView collectionViewSource = new QueryableCollectionView(DataExtension.DataCatalogsSingleton.Instance.ActiveDishesAll);
                        _dishitemsSource = collectionViewSource;
                        _dishitemsSource.MoveCurrentToFirst();
                    }
                }

                return _dishitemsSource;
            }
        }

        ICollectionView _itemsSource;
        public ICollectionView ItemsSource
        {
            get
            {
                if (_itemsSource == null)
                {
                    QueryableCollectionView collectionViewSource = new QueryableCollectionView(Labels);
                    _itemsSource = collectionViewSource;
                    _itemsSource.MoveCurrentToFirst();
                }

                return _itemsSource;
            }
        }

    }

    /*
    public static class DishExt
    {


        public static int GetLabelsCount(this DishPackageFlightOrder d)
        {
            //  if (labelSeriesCount != -1) return labelSeriesCount;
            if (d.Dish == null) return 0;
            // if (!d.PrintLabel) return 0;
            return (int)Math.Ceiling(d.Amount / (decimal)d.Dish.ToFlyLabelSeriesCount);

            //return DataExtension.DataCatalogsSingleton.Instance.ItemLabelsInfo.Where(a => a.ParenItemId == Id).Count();
        }
    }
    */
}
