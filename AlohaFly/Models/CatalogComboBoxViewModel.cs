using AlohaFly.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace AlohaFly.Models
{
    public class CatalogComboBoxViewModel<T> : ViewModelBase
         where T : INotifyPropertyChanged
    {
        public CatalogComboBoxViewModel()
        {

        }
        public FullyObservableCollection<T> DataCatalog { set; get; }
        public string Header { set; get; }
        public string EmptyText { set; get; }
        public string DisplayMemberPathName { set; get; } = "FullName";
        public ICommand ReturnCommand { get; set; }

        T selectedData;

        public event EventHandler<T> SelectedDataChanged;
        protected virtual void OnSelectedDataChanged(T e)
        {
            SelectedDataChanged?.Invoke(this, e);
        }

        public T SelectedData
        {
            set
            {
                if (value == null || !value.Equals(selectedData))
                {
                    selectedData = value;
                    //OnPropertyChanged("SelectedData");

                    OnSelectedDataChanged(selectedData);
                }
            }

            get
            {
                return selectedData;
            }
        }
        public bool HeaderVisible { set; get; } = true;

        public bool IsFocused { set; get; } = false;

        public System.Windows.Visibility HeaderVisibility
        {
            get
            {
                return HeaderVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }
    }


    public class CatalogComboBoxEnumViewModel<T>

    {
        public CatalogComboBoxEnumViewModel()
        {

        }

        public IEnumerable<T> DataCatalog { set; get; }


        public string Header { set; get; }
        public string EmptyText { set; get; }
        //public string DisplayMemberPathName { set; get; } = "Name";
        public T SelectedData { set; get; }
        public bool HeaderVisible { set; get; } = true;

        public System.Windows.Visibility HeaderVisibility
        {
            get
            {
                return HeaderVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }
    }

}
