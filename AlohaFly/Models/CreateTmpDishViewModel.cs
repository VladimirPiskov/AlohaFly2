using AlohaService.ServiceDataContracts;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;

namespace AlohaFly.Models
{
    class CreateTmpDishViewModel : ViewModelBase
    {
        Dish D;
        public bool Result { set; get; } = false;
        public CreateTmpDishViewModel(Dish d)
        {
            D = d;
        }

        public ReadOnlyObservableCollection<Dish> ItemsSource
        {
            get
            {
                return new ReadOnlyObservableCollection<Dish>(new ObservableCollection<Dish>() { D });
            }
        }

    }
}
