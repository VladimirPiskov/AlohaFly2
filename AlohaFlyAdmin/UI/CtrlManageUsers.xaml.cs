using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AlohaFly.Utils;
using AlohaService.ServiceDataContracts;

namespace AlohaFlyAdmin.UI
{
    /// <summary>
    /// Логика взаимодействия для CtrlManageFuncs.xaml
    /// </summary>
    public partial class CtrlManageUsers : UserControl
    {
        public CtrlManageUsers()
        {
            InitializeComponent();
        }


        private FullyObservableCollection<User> _downloadsCollection;
        public ObservableCollection<User> DownloadsCollection
        {
            get { return this._downloadsCollection; }
        }
        
        public void UpdateData()
        {
            _downloadsCollection = new FullyObservableCollection<User> (AlohaFly.DBProvider.GetAllUsers());
            _downloadsCollection.ItemPropertyChanged += _downloadsCollection_ItemPropertyChanged;
            gridFuncs.ItemsSource = DownloadsCollection;
        }

        private void _downloadsCollection_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            long id= ((FullyObservableCollection<UserFunc>)sender)[e.CollectionIndex].Id; 
            string n = ((FullyObservableCollection<UserFunc>)sender)[e.CollectionIndex].Name;
            AlohaFly.DBProvider.UpdateUserFunc(id,n);

        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateData();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //CTmp.CreateEMUser();
            //CTmp.CreateOperatorUser1();
            CTmp.CreateOperatorUser11();

            // CTmp.CreateTUser();
            //CTmp.CreateAlohaSharUser2();
            //CTmp.CreateAlohaSharUser();
            //CTmp.CreateP2User();
            //CTmp.CreatePUser();
            //CTmp.CreateDirectorUser();
            //CTmp.CreateNPUser();
            UpdateData();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (gridFuncs.SelectedItem != null)
            {
                long Id = ((UserFunc)gridFuncs.SelectedItem).Id;
                AlohaFly.DBProvider.DeleteUserFunc(Id);
                UpdateData();
            }
        }

        private void btnOperGr_Click(object sender, RoutedEventArgs e)
        {
            CTmp.CreateOperatorGroup();
        }

        private void btnCreateOpers_Click(object sender, RoutedEventArgs e)
        {
            //CTmp.CreateOperatorUser();
            CTmp.CreateOperatorUser1();
            CTmp.CreateOperatorUser2();
            CTmp.CreateOperatorUser3();
            CTmp.CreateOperatorUser4();
        }
    }
}
