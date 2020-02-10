using AlohaFly.DataExtension;
using AlohaFly.Utils;
using AlohaService.Interfaces;
using AlohaService.ServiceDataContracts;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;


namespace AlohaFly.Models
{

    public class AddLabelsViewModel  : ViewModelPaneReactiveObject
    {
        IOrderLabel CurentOrder;
        FullyObservableDBDataSubsriber<ItemLabelInfo, ItemLabelInfo> itemLabelInfoConnector = new FullyObservableDBDataSubsriber<ItemLabelInfo, ItemLabelInfo>(a => a.Id);
        FullyObservableCollection<IDishPackageLabel> DishPackages { set; get; }
        public AddLabelsViewModel(IOrderLabel order)
        {
            CurentOrder = order;
            DishPackages = new FullyObservableCollection<IDishPackageLabel>();
            foreach (var dp in order.DishPackagesForLab)
            {
                dp.PrintLabel = DataExtension.DataCatalogsSingleton.Instance.ItemLabelInfoData.Data.Where(a => a.ParenItemId == dp.DishId).Count() > 0;
                dp.LabelSeriesCount = dp.LabelsCount;
                DishPackages.Add(dp);

            }
            order.PropertyChanged += Order_PropertyChanged;
            AllDishGridVis = Visibility.Collapsed;
            OrderDishGridVis = Visibility.Visible;
            CurentDishMaxSerCountVisible = Visibility.Visible;
            Init();
        }

       

        private void Order_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == "DishPackages")
                {
                    DishPackages.Clear();
                    foreach (var dp in ((IOrderLabel)sender).DishPackagesForLab)
                    {
                        dp.PrintLabel = DataExtension.DataCatalogsSingleton.Instance.ItemLabelInfoData.Data.Where(a => a.ParenItemId == dp.DishId).Count() > 0;
                        dp.LabelSeriesCount = dp.LabelsCount;
                        DishPackages.Add(dp);

                    }
                   // OrderDish = DishPackages.First();
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
            CurentDishMaxSerCountVisible = Visibility.Collapsed;
            Init();
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
                        QueryableCollectionView collectionViewSource = new QueryableCollectionView(DataExtension.DataCatalogsSingleton.Instance.DishFilter.OpenDishes);
                        _dishitemsSource = collectionViewSource;
                        _dishitemsSource.MoveCurrentToFirst();
                    }
                    else
                    {
                        QueryableCollectionView collectionViewSource = new QueryableCollectionView(DataExtension.DataCatalogsSingleton.Instance.DishFilter.ActiveDishesAll);
                        _dishitemsSource = collectionViewSource;
                        _dishitemsSource.MoveCurrentToFirst();
                    }
                    _dishitemsSource.CurrentChanging += _dishitemsSource_CurrentChanging;
                    _dishitemsSource.CurrentChanged += _dishitemsSource_CurrentChanged;
                }

                return _dishitemsSource;
            }
        }

        private void _dishitemsSource_CurrentChanged(object sender, EventArgs e)
        {
            if (DishItemsSource.CurrentItem != null)
            {
                LabelDish = (Dish)DishItemsSource.CurrentItem;
            }
        }

        private void _dishitemsSource_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            if (IsBusy) return; //Обход глюка. CurrentChanging генерируется когда меняешь LabelSeriesCount при SaveChanged.
            if (SaveCommandEnable)
            {
                if (UI.UIModify.ShowConfirm($"Сохранить изменения для блюда {LabelDish?.Name}?").DialogResult.GetValueOrDefault())
                {
                    SaveChanged();
                }
                else
                {
                    CancelChanged();
                }
            }

        }

        private void SaveChanged()
        {
            BusyContent = "Сохраняю наклейку";
            IsBusy = true;

            //Иначе Busy  не работает 
            Task.Run(() =>
            {
                foreach (var cD in ChangedLabels.Values)
                {
                    SaveDishLabel(cD);
                }
                if (!ChangedLabels.TryGetValue(CurentRunTimeLabelInfo.ParentItemId, out RunTimeLabelInfo lbl))
                {
                    SaveDishLabel(CurentRunTimeLabelInfo);
                }
                ChangedLabels.Clear();

                SaveCommandEnable = false;
                IsBusy = false;
            }
            );
        }



        private void SaveDishLabel(RunTimeLabelInfo curentData)
        {

            if (curentData == null || curentData.ParentItemId == 0) return;
            var oldLabels = DataExtension.DataCatalogsSingleton.Instance.ItemLabelInfoData.Data.Where(a => a.ParenItemId == curentData.ParentItemId);
            List<ItemLabelInfo> delItems = new List<ItemLabelInfo>();
            if (oldLabels != null && oldLabels.Count() > 0)
            {
                foreach (var l in oldLabels)
                {
                    if (!curentData.Labels.Any(a => a.Id == l.Id))
                    {
                        delItems.Add(l);
                    }
                }
            }
            DataExtension.DataCatalogsSingleton.Instance.ItemLabelInfoData.EndEditMany(delItems, curentData.Labels.ToList());

            if (DataExtension.DataCatalogsSingleton.Instance.DishData.Data.Any(a => a.Id == curentData.ParentItemId))
            {
                var d = DataExtension.DataCatalogsSingleton.Instance.DishData.Data.FirstOrDefault(a => a.Id == curentData.ParentItemId);
                if ((d.LabelEnglishName != curentData.EnglishName) || (d.LabelRussianName != curentData.RussianName))
                {
                    d.LabelEnglishName = curentData.EnglishName;
                    d.LabelRussianName = curentData.RussianName;
                    DataExtension.DataCatalogsSingleton.Instance.DishData.EndEdit(d);
                }
                d.LabelsCount = DataExtension.DataCatalogsSingleton.Instance.ItemLabelInfoData.Data.Where(a => a.ParenItemId == d.Id).Count();
            }
}


        private void CancelChanged()
        {
            SaveCommandEnable = false;
        }

        void Init()
        {

            this.WhenAnyValue(a => a.LabelDish).Subscribe(_ => { UpdateLabels(); });
            //this.WhenAnyValue(a => a.SelectedDishPackage).Subscribe(_ => { if (SelectedDishPackage != null) { LabelDish = SelectedDishPackage.Dish; } });
            this.WhenAnyValue(a => a.CurentLabelInfo).Subscribe(_ => {UpdateCurentLabelInfo(); });
            this.WhenAnyValue(a => a.CurentDishMaxSerCount).Subscribe(_ => {
                if ((LabelDish != null) && (_orderDishes!=null))
                {
                    if (_orderDishes.CurrentItem is DishPackageFlightOrder  dpf)
                    {
                        dpf.Dish.ToFlyLabelSeriesCount = CurentDishMaxSerCount;
                        dpf.LabelSeriesCount = dpf.Dish.ToFlyLabelSeriesCount==0? (int)Math.Ceiling(dpf.Amount) : (int)Math.Ceiling(dpf.Amount / (decimal)dpf.Dish.ToFlyLabelSeriesCount);
                    }
                    else if (_orderDishes.CurrentItem is DishPackageToGoOrder dpt)
                    {
                        dpt.Dish.ToGoLabelSeriesCount = CurentDishMaxSerCount;

                        dpt.LabelSeriesCount = dpt.Dish.ToGoLabelSeriesCount==0? (int)Math.Ceiling(dpt.Amount ): (int)Math.Ceiling(dpt.Amount / (decimal)dpt.Dish.ToGoLabelSeriesCount);
                    }
                    SaveCommandEnable = true;
                }
            
            });


            AddLabelCommand = new DelegateCommand(_ =>
            {
                CurentRunTimeLabelInfo.Labels.Add(new ItemLabelInfo()
                {
                    ParenItemId = LabelDish.Id,
                    SerialNumber = CurentRunTimeLabelInfo.Labels.Count() + 1
                });
                
            });

            RemoveLabelCommand = new DelegateCommand(_ =>
            {
                if (CurentLabelInfo != null)
                {
                    int pos = CurentRunTimeLabelInfo.Labels.IndexOf(CurentLabelInfo);
                    CurentRunTimeLabelInfo.Labels.Remove(CurentLabelInfo);
                    if (CurentRunTimeLabelInfo.Labels.Count > 0)
                    {
                        CurentLabelInfo = CurentRunTimeLabelInfo.Labels[Math.Min(pos, CurentRunTimeLabelInfo.Labels.Count - 1)];
                    }
                    else
                    {
                        CurentLabelInfo = null;
                    }
                }
            });

            PrintCommand = new DelegateCommand(_ =>
            {
                var curLabs = new Dictionary<long, RunTimeLabelInfo>();
               
                if( (ChangedLabels != null) && (LabelDish!=null))
                {
                    curLabs = ChangedLabels.ToDictionary(entry => entry.Key, entry => entry.Value);
                    if (!curLabs.TryGetValue(LabelDish.Id, out var val))
                    {
                        curLabs.Add(LabelDish.Id, CurentRunTimeLabelInfo);
                    }
                }
                if (OrderDishGridVis == Visibility.Visible)
                {
                    //labels = GetLabels();
                    //    var pvm = new LabelsPrint.LabelPapersVisualViewModel(CurentOrder, ChangedLabels);
                    var pvm = new LabelsPrint.LabelPapersVisualViewModel(CurentOrder, curLabs);
                    pvm.ShowMe();
                }
                else
                {
                    var pvm = new LabelsPrint.LabelPapersVisualViewModel(LabelDish, curLabs);
                    //var pvm = new LabelsPrint.LabelPapersVisualViewModel(LabelDish, CurentRunTimeLabelInfo);
                    pvm.ShowMe();
                }
                
            });
            UpdateLabels();
        }


        private List<ItemLabelInfo> GetLabels()
        {
            var tmp = new List<ItemLabelInfo>();
            foreach (var d in CurentOrder.DishPackagesForLab.Where(a => a.PrintLabel))
            {
                if (d.DishId == LabelDish.Id)
                {
                    tmp.AddRange(CurentRunTimeLabelInfo.Labels);
                }
                else
                {
                    if (ChangedLabels.TryGetValue(d.DishId, out RunTimeLabelInfo lbl))
                    {
                        tmp.AddRange(lbl.Labels.ToList());
                    }
                    else
                    {
                        tmp.AddRange(DataCatalogsSingleton.Instance.ItemLabelInfoData.Data.Where(a => a.ParenItemId == d.DishId).OrderBy(a => a.SerialNumber));
                    }   
                }
                }
            return tmp;
        }


        
        [Reactive] public Visibility AllDishGridVis { set; get; }

        [Reactive]
        public Visibility CurentDishMaxSerCountVisible { set; get; }

        [Reactive] public Visibility OrderDishGridVis { set; get; }

       // [Reactive] public IDishPackageLabel SelectedDishPackage { set; get; }


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
                        _orderDishes.CurrentChanged += _orderDishes_CurrentChanged;
                        _orderDishes.MoveCurrentToFirst();
                        if (_orderDishes.CurrentItem != null)
                        {
                            LabelDish = ((IDishPackageLabel)_orderDishes.CurrentItem).Dish;
                        }
                        // _orderDishes.CurrentChanging += _orderDishes_CurrentChanging;



                    }
                }
                return _orderDishes;
            }
        }

        private void _orderDishes_CurrentChanged(object sender, EventArgs e)
        {
            if ((SaveCommandEnable)&&(LabelDish!=null))
            {
                if (!ChangedLabels.TryGetValue(LabelDish.Id, out var val))
                {
                    ChangedLabels.Add(LabelDish.Id, CurentRunTimeLabelInfo);
                }
            }
            if (_orderDishes.CurrentItem != null)
            {
                LabelDish = ((IDishPackageLabel)_orderDishes.CurrentItem).Dish;
            }
        }


        [Reactive] public int CurentDishMaxSerCount { get; set; }
        /*
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
        */
        private void _orderDishes_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            
            if (SaveCommandEnable)
            {
                if (!ChangedLabels.TryGetValue(LabelDish.Id, out var val))
                {
                    ChangedLabels.Add(LabelDish.Id, CurentRunTimeLabelInfo);
                }
            }
            LabelDish = ((IDishPackageLabel)_orderDishes.CurrentItem).Dish;
        }

        public ICommand AddLabelCommand { get; set; }
        public ICommand RemoveLabelCommand { get; set; }
        public ICommand PrintCommand { get; set; }

        public ICommand SaveCommand
        {
            get
            {
                return new DelegateCommand(_ =>
                {

                    SaveChanged();
                }
                );
            }
        }

        [Reactive] public string BusyContent { set; get; } = "Сохраняю заказ";
        [Reactive] public bool IsBusy { set; get; } = false ;
        [Reactive] public bool RemoveLabelCommandEnable { set; get; }

        [Reactive] public Dish LabelDish { set; get; }

        [Reactive] public bool SaveCommandEnable { set; get; } = false;

        [Reactive] public RunTimeLabelInfo CurentRunTimeLabelInfo { set; get; }

        [Reactive] public ItemLabelInfo CurentLabelInfo { set; get; }
        public Dictionary<long, RunTimeLabelInfo> ChangedLabels{ set; get; } = new Dictionary<long, RunTimeLabelInfo>();


        void UpdateCurentLabelInfo()
        {
            RemoveLabelCommandEnable = (CurentLabelInfo != null);
            
        }
        void UpdateLabels()
        {
            if (LabelDish != null)
            {
                
                CurentDishMaxSerCount = LabelDish.ToFlyLabelSeriesCount;

                if (ChangedLabels.TryGetValue(LabelDish.Id, out var val))
                {
                    SaveCommandEnable = true;
                    CurentRunTimeLabelInfo = val;
                    //Labels = val;
                }
                else
                {
                    if (DishPackages == null) { SaveCommandEnable = false; };
                    CurentRunTimeLabelInfo = new RunTimeLabelInfo(LabelDish.Id,DataCatalogsSingleton.Instance.ItemLabelInfoData.Data.Where(a => a.ParenItemId == LabelDish?.Id).OrderBy(a => a.SerialNumber));
                    CurentRunTimeLabelInfo.EnglishName = LabelDish.LabelEnglishName;
                    CurentRunTimeLabelInfo.RussianName = LabelDish.LabelRussianName;
                    //CurentRunTimeLabelInfo.Labels = new FullyObservableCollection<ItemLabelInfo>();
                    CurentRunTimeLabelInfo.PropertyChanged += CurentRunTimeLabelInfo_PropertyChanged;
                }
            }
        }

        private void CurentRunTimeLabelInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveCommandEnable = true;
        }





        /*

        public string DishName
        {
            get
            {
                return LabelDish.Name;
            }
        }

        //public string labelRussianName;
        [Reactive] public string LabelRussianName { set; get; }


        [Reactive] public string LabelEnglishName { set; get; }
        */

        /*
        bool curentDishEdited = false;
        
        */



    }

    public class RunTimeLabelInfo: ReactiveObject
    {
        public RunTimeLabelInfo(long parentItemId, IEnumerable<ItemLabelInfo> labels)
        {
            ParentItemId = parentItemId;
            Labels = new FullyObservableCollection<ItemLabelInfo>(labels);
            Labels.CollectionChanged += Labels_CollectionChanged;
            Labels.ItemPropertyChanged += Labels_ItemPropertyChanged;
            

        }

        public long ParentItemId { set; get; }

        private void Labels_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (blockItemChanged) return;
            this.RaisePropertyChanged();
        }

        bool blockItemChanged = false;
        private void Labels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (Labels != null)
            {
                int c = 1;
                blockItemChanged = true;
                foreach (var l in Labels.OrderBy(a => a.SerialNumber))
                {
                    l.SerialNumber = c;
                    c++;
                }
                blockItemChanged = false;
            }
            
            this.RaisePropertyChanged();
        }


        [Reactive] public FullyObservableCollection<ItemLabelInfo> Labels { set; get; } 
        [Reactive] public string RussianName { set; get; }
        [Reactive] public string EnglishName { set; get; }
    }
}
