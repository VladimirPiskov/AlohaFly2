using AlohaFly.DataExtension;
using AlohaFly.Models;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls.GridView.SearchPanel;

namespace AlohaFly.UI
{
    /// <summary>
    /// Логика взаимодействия для CtrlCatalogView2.xaml
    /// </summary>
    public partial class CtrlCatalogView2 : UserControl

    {
        public CtrlCatalogView2()
        {
            InitializeComponent();

        }

        bool Editing = false;
        bool IsNew = false;
        private void RadGridView_SelectionChanging(object sender, Telerik.Windows.Controls.SelectionChangingEventArgs e)
        {
            if (Editing)
            {
                e.Cancel = true;
            }
        }

        private void RadDataForm_BeginningEdit(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Editing = true;
        }



        private void RadDataForm_AutoGeneratingField(object sender, Telerik.Windows.Controls.Data.DataForm.AutoGeneratingFieldEventArgs e)
        {
            if (e.PropertyName == "Id")
            {
                e.DataField.IsReadOnly = true;

            }

            if ((e.PropertyName.EndsWith("Id") && e.PropertyName != "Id") && (!Properties.Settings.Default.IdFieldsVisible))
            {
                e.DataField.Visibility = Visibility.Collapsed;
            }


            if (typeof(INotifyPropertyChanged).IsAssignableFrom(e.PropertyType))
            {
                dynamic DataCollection = DataCatalogsSingleton.Instance.GetCatalogData(e.PropertyType);
                if (DataCollection != null)
                {

                    e.DataField = new DataFormComboBoxField()
                    {
                        Label = e.DataField.Label,
                        DisplayMemberPath = "Name",
                        SelectedValuePath = "Id",
                        DataMemberBinding = new Binding(DataCatalogsSingleton.Instance.GetBindingIdCollectionName(e.PropertyName)) { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged },
                        ItemsSource = DataCollection,

                    };

                }

            }

        }

        private void RadDataForm_AddedNewItem(object sender, Telerik.Windows.Controls.Data.DataForm.AddedNewItemEventArgs e)
        {
            
            if (!((Models.CatalogViewModel)DataContext).AddItem())
            {
                radDataForm.CancelEdit();
            }
            else
            {
                radDataForm.CancelEdit();
                mainGrid.FilterDescriptors.Clear();
                radDataForm.CurrentItem = GetMaxIdElement();
                //radDataForm.MoveCurrentToLast();
                //radDataForm.ItemsSource
                //radDataForm.CurrentItem
                radDataForm.BeginEdit();
                mainGrid.ScrollIntoViewAsync(mainGrid.CurrentItem, _ => { });
                IsNew = true;
            }
            
        }
        
        private object GetMaxIdElement()
        {
            var la = ((CatalogViewModel)radDataForm.DataContext).LastAddedItem;
            var lId = ((dynamic)la).Id;
            foreach (var itm in radDataForm.ItemsSource)
            {
                var id = ((dynamic)itm).Id;
                if (id == lId)
                {
                    return itm;
                }

            }
            return null;
        }
        
        private void radDataForm_DeletingItem(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !((Models.CatalogViewModel)DataContext).DeleteItem();

        }

        private void radDataForm_EditEnding(object sender, Telerik.Windows.Controls.Data.DataForm.EditEndingEventArgs e)
        {
            if (e.EditAction == Telerik.Windows.Controls.Data.DataForm.EditAction.Commit)
            {
                e.Cancel = !((Models.CatalogViewModel)DataContext).CommitChanges();
            }
            else
            {
                /*
                if (IsNew)
                {
                    ((Models.CatalogViewModel)DataContext).CancelAddItem();
                }
                */
            }
            IsNew = false;
            Editing = false;
        }

        private void RadGridView_AutoGeneratingColumn(object sender, GridViewAutoGeneratingColumnEventArgs e)
        {

            if ((e.ItemPropertyInfo.Name.EndsWith("Id") && e.ItemPropertyInfo.Name != "Id") && (!Properties.Settings.Default.IdFieldsVisible))
            {
                e.Column.IsVisible = false;
            }

            /*
            if (e.Column.Name=="Баркод")
            {
                
            }
            */
            if (typeof(INotifyPropertyChanged).IsAssignableFrom(e.ItemPropertyInfo.PropertyType))
            {

                dynamic DataCollection = DataCatalogsSingleton.Instance.GetCatalogData(e.ItemPropertyInfo.PropertyType);
                if (DataCollection != null)
                {

                    e.Column = new GridViewComboBoxColumn()
                    {
                        IsLightweightModeEnabled = true,
                        Header = e.Column.Header,
                        DisplayMemberPath = "Name",
                        SelectedValueMemberPath = "Id",
                        DataMemberBinding = new Binding(DataCatalogsSingleton.Instance.GetBindingIdCollectionName(e.ItemPropertyInfo.Name)) { Mode = BindingMode.TwoWay },
                        ItemsSource = DataCollection,
                    };

                }
            }
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Reports.ExportProvider.ExportGridToExcel(mainGrid);
        }

        private void radDataForm_AddingNewItem(object sender, Telerik.Windows.Controls.Data.DataForm.AddingNewItemEventArgs e)
        {
            //Очищаем поис
            var clearSearchValue = GridViewSearchPanelCommands.ClearSearchValue as RoutedUICommand;
            clearSearchValue.Execute(null, this.mainGrid.ChildrenOfType<GridViewSearchPanel>().FirstOrDefault());
            /*
            e.Cancel = true;
            if (((Models.CatalogViewModel)DataContext).AddItem())
            {
                mainGrid.ScrollIntoView(mainGrid.SelectedItem);
                radDataForm.BeginEdit();
            }
            */

            /*
            if (!((Models.CatalogViewModel)DataContext).AddItem())
            {
                
                radDataForm.CancelEdit();
            }
            else
            {
                IsNew = true;
            }
            */
        }
    }
}
