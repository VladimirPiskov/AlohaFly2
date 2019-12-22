using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AlohaFly.UI
{
    /// <summary>
    /// Логика взаимодействия для CtrlToGoClientCatalog.xaml
    /// </summary>
    public partial class CtrlToGoClientCatalog : UserControl
    {
        public CtrlToGoClientCatalog()
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

        private void RadDataForm_AddedNewItem(object sender, Telerik.Windows.Controls.Data.DataForm.AddedNewItemEventArgs e)
        {
            /*
            if (!((Models.ToGoClientCatalogViewModel)DataContext).AddItem())
            {
                radDataForm.CancelEdit();
            }
            else
            {
                IsNew = true;
            }
            */
        }


        private void RadDataForm_AutoGeneratingField(object sender, Telerik.Windows.Controls.Data.DataForm.AutoGeneratingFieldEventArgs e)
        {
            if (e.PropertyName == "Id")
            {
                e.DataField.IsReadOnly = true;

            }
            var disFialds = new List<string>() { "FullName", "Phones", "Addresses" };

            if (disFialds.Contains(e.PropertyName))
            {
                e.DataField.Visibility = Visibility.Collapsed;
            }
        }


        private void radDataForm_DeletingItem(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //e.Cancel = !((Models.ToGoClientCatalogViewModel)DataContext).DeleteItem();

        }

        private void radDataForm_EditEnding(object sender, Telerik.Windows.Controls.Data.DataForm.EditEndingEventArgs e)
        {
            /*
            if (e.EditAction == Telerik.Windows.Controls.Data.DataForm.EditAction.Commit)
            {
                e.Cancel = !((Models.ToGoClientCatalogViewModel)DataContext).CommitChanges();
            }
            else
            {
                if (IsNew)
                {
                    ((Models.ToGoClientCatalogViewModel)DataContext).CancelAddItem();
                }
            }
            IsNew = false;
            Editing = false;
            */
        }
    }
}
