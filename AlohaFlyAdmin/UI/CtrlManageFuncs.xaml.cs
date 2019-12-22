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
    public partial class CtrlManageFuncs : UserControl
    {
        public CtrlManageFuncs()
        {
            InitializeComponent();
        }


        private FullyObservableCollection<UserFunc> _downloadsCollection;
        public ObservableCollection<UserFunc> DownloadsCollection
        {
            get { return this._downloadsCollection; }
        }
        
        public void UpdateData()
        {
            _downloadsCollection = new FullyObservableCollection<UserFunc> (AlohaFly.DBProvider.GetAllFuncs());
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
            AlohaFly.DBProvider.CreateUserFunc(tbNewFuncsName.Text);
            UpdateData();
        }

        private void CreateFuncs()
        {

            List<string> funcs = new List<string>()
            {
                "Заказ самолеты",
                "Заказ с собой",
                "Справочник авиакомпаний",
                "Справочник блюд",
                "Справочник мест доставки",
                "Справочник водителей",
                "Справочник контактных лиц",
                "Справочник кухонных групп",
                "Справочник логических групп",
                "Редактирование наклеек",
                "Смена своего пароля",



            };
            List<string> ss = AlohaFly.DBProvider.GetAllFuncs().Select(a => a.Name).ToList();
            foreach (var f in funcs)
            {
                if (!ss.Contains(f))
                {
                    AlohaFly.DBProvider.CreateUserFunc(f);
                }
            }
            UpdateData();
            /*
            public const long Access_OrderFly = 21;
        public const long Access_OrderToGo = 22;

        public const long Access_CatalogAirCompany = 31;
        public const long Access_CatalogDish = 32;
        public const long Access_CatalogDeliveryPlace = 33;
        public const long Access_CatalogDriver = 34;
        public const long Access_CatalogContactPerson = 35;
        public const long Access_CatalogKitchenGroup = 36;
        public const long Access_CatalogLogicGroup = 37;

        public const long Access_DishLabels = 50;

        public const long Access_OrdersToFly = 100;
        public const long Access_AddOrdersToFly = 101;

        public const long Access_Reports_Rep1 = 200;
        public const long Access_Reports_Rep2 = 201;
        */
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

        private void btnCreateFuncs_Click(object sender, RoutedEventArgs e)
        {
            CreateFuncs();
        }
    }
}
