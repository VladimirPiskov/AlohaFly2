using AlohaFly.DataExtension;
using AlohaFly.Utils;
using AlohaService.ServiceDataContracts;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Controls;
using Telerik.Windows.Data;

namespace AlohaFly.Models
{

    public class CatalogModel
    {

    }


    public class CatalogModel<T> : INotifyPropertyChanged
         where T : INotifyPropertyChanged
    {
        private readonly FullyObservableCollection<T> _catalogData = new FullyObservableCollection<T>();

        public event PropertyChangedEventHandler PropertyChanged;
        private EditCatalogDataFuncs<T> EditFuncs;

        public CatalogModel(EditCatalogDataFuncs<T> editFuncs)
        {
            EditFuncs = editFuncs;
            _catalogData = editFuncs.AllDataList;

        }

        protected void NotifyPropertyChanged(
          string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public FullyObservableCollection<T> CatalogData
        {
            get
            {
                return _catalogData;
            }
        }

        public long AddItem(T item)
        {


            //return DBDataExtractor<T>.AddItem((_) => { return EditFuncs.AddItemFunc(item); }, item);
            var res = DBDataExtractor<T>.AddItem(EditFuncs.AddItemFunc, item);
            if (res != -1) { SetId(item, res); };
            return res;


        }

        public void SetId(dynamic item, long Id)
        {
            item.Id = Id;
            //typeof(T).GetMember("Id")?[0].SetValue(item, Id);

        }

        public bool CommitEditItem(T item)
        {
            return DBDataExtractor<T>.EditItem(EditFuncs.EditItemFunc, item);
        }
        public bool CancelAddItem(long Id)
        {
            return DBDataExtractor<T>.DeleteItem(EditFuncs.CancelAddItemFunc, Id);
        }

        public bool DeleteItem(T item)
        {
            long Id = ((dynamic)item).Id;
            return DBDataExtractor<T>.DeleteItem(EditFuncs.CancelAddItemFunc, Id);

        }

    }

    public class EditCatalogDataFuncs<T>
        where T : INotifyPropertyChanged
    {
        public EditCatalogDataFuncs()
        { }


        public Func<T, OperationResult> AddItemFunc;
        public Func<T, OperationResult> EditItemFunc;
        public Func<long, OperationResult> CancelAddItemFunc;
        //public Func<List<T>> GetAllDataFunc;

        //public Func<OperationResultValue<T[]>> GetAllDataFunc;
        //public Func<T> GetNewItem;
        public FullyObservableCollection<T> AllDataList;
    }

    public abstract class CatalogViewModel : ViewModelPane
    {
        public CatalogViewModel()
        {

        }
        public abstract bool CommitChanges();
        public abstract bool AddItem();
        public abstract bool DeleteItem();
        public abstract bool CancelAddItem();

        /*
        public void CommitChanges()
        {

        }
        */


        /*
        public void CommitChanges()
        {
            _model.CommitEditItem(SelectedItem);
        }
        */
    }

    public class CatalogViewModel<T> : CatalogViewModel
        where T : INotifyPropertyChanged
    {
        readonly CatalogModel<T> _model;

        public CatalogViewModel(CatalogModel<T> model)
        {
            _model = model;
            if (_model.CatalogData != null)
            {
                _model.CatalogData.CollectionChanged += CatalogData_CollectionChanged; ;
            }

        }


        public bool CanDeleteItem { set; get; } = true;
        public bool CanAddItem { set; get; } = true;
        public bool CanEditItem { set; get; } = true;

        public Telerik.Windows.Controls.Data.DataForm.DataFormCommandButtonsVisibility? RadFormBtnVis
        {
            get
            {

                var res =
                        Telerik.Windows.Controls.Data.DataForm.DataFormCommandButtonsVisibility.Navigation |
                        Telerik.Windows.Controls.Data.DataForm.DataFormCommandButtonsVisibility.Commit |
                        Telerik.Windows.Controls.Data.DataForm.DataFormCommandButtonsVisibility.Cancel;

                if (CanDeleteItem)
                {
                    res = res | Telerik.Windows.Controls.Data.DataForm.DataFormCommandButtonsVisibility.Delete;
                }

                if (CanAddItem)
                {
                    res = res | Telerik.Windows.Controls.Data.DataForm.DataFormCommandButtonsVisibility.Add;
                }
                if (CanEditItem)
                {
                    res = res | Telerik.Windows.Controls.Data.DataForm.DataFormCommandButtonsVisibility.Edit;
                }
                return res;

            }
        }

        public override bool CommitChanges()
        {
            return _model.CommitEditItem(SelectedItem);
        }

        private long AddedItemId = -1;
        public override bool AddItem()
        {
            AddedItemId = _model.AddItem(SelectedItem);
            return (AddedItemId != -1);
        }
        public override bool CancelAddItem()
        {
            return _model.CancelAddItem(AddedItemId);
        }

        public override bool DeleteItem()
        {
            return _model.DeleteItem(SelectedItem);

        }


        private void CatalogData_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("ItemsSource");
            //NotifyPropertyChanged("ItemsSource");
        }

        ICollectionView _itemsSource;
        public ICollectionView ItemsSource
        {
            get
            {
                if (_itemsSource == null)
                {
                    QueryableCollectionView collectionViewSource = new QueryableCollectionView(Model.CatalogData);


                    _itemsSource = collectionViewSource;
                    _itemsSource.MoveCurrentToFirst();
                }

                return _itemsSource;
            }
        }

        public T SelectedItem
        {
            get
            {
                return (T)ItemsSource.CurrentItem;
            }
        }


        public CatalogModel<T> Model
        {
            get
            {
                return _model;
            }

        }
        public List<DataGridColumn> DataColumns
        {
            get
            {
                return DataVisualExtension.GetDataColumns();
            }
        }


        /*
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void NotifyPropertyChanged(
            string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        */

    }

    /*
    public class CatalogItemPropViewModel
    {
        public CatalogItemPropViewModel()
        {

        }
        public string Name { set; get; }
        public string Value { set; get; }
    }
    */
}
