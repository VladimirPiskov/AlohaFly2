using AlohaFly.DataExtension;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Telerik.Windows.Controls;

namespace AlohaFly.UI
{
    /// <summary>
    /// Логика взаимодействия для WndCreateTmpDish.xaml
    /// </summary>
    public partial class WndCreateTmpDish : RadWindow
    {
        public WndCreateTmpDish()
        {
            InitializeComponent();

        }

        private void RadDataForm_AutoGeneratingField(object sender, Telerik.Windows.Controls.Data.DataForm.AutoGeneratingFieldEventArgs e)
        {
            if (e.PropertyName == "Id")
            {
                e.DataField.IsReadOnly = true;

            }
            if (e.PropertyName.EndsWith("Id") && e.PropertyName != "Id")
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
                        DataMemberBinding = new Binding(e.PropertyName + "Id") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged },
                        ItemsSource = DataCollection,
                    };

                }

            }
        }

        private void mDataForm_EditEnded(object sender, Telerik.Windows.Controls.Data.DataForm.EditEndedEventArgs e)
        {
            model.Result = e.EditAction == Telerik.Windows.Controls.Data.DataForm.EditAction.Commit;
            this.Close();

        }

        Models.CreateTmpDishViewModel model;
        private void RadWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            model = (sender as RadWindow).DataContext as Models.CreateTmpDishViewModel;
        }
    }
}
