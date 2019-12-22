using System.Windows.Controls;

namespace AlohaFly.UI
{
    /// <summary>
    /// Логика взаимодействия для CtrlToGoClientInfo.xaml
    /// </summary>
    public partial class CtrlToGoClientInfo : UserControl
    {
        public CtrlToGoClientInfo()
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

            if (!((Models.CatalogViewModel)DataContext).AddItem())
            {
                radDataForm.CancelEdit();
            }
            else
            {
                IsNew = true;
            }
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
                if (IsNew)
                {
                    ((Models.CatalogViewModel)DataContext).CancelAddItem();
                }
            }
            IsNew = false;
            Editing = false;
        }
    }
}
