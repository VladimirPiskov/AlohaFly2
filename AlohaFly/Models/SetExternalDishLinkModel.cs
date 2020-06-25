using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlohaService.ServiceDataContracts;
using Telerik.Windows.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace AlohaFly.Models
{
    class SetExternalDishLinkModel : ViewModelPaneReactiveObject
    {

        DishPackageToGoOrder myDp;
        public SetExternalDishLinkModel(OrderToGo order, DishPackageToGoOrder dp )
        {

            InfoMessage = dp?.DishName + " " +dp?.TotalPrice +"руб.";

            OkCommand = new DelegateCommand(_ =>
            {
                try

                {
                    if (SelectedClientVM == null)
                    {
                        UI.UIModify.ShowAlert("Не выбрано блюдо");
                        return;
                    }
                    var dish = (Dish)SelectedClientVM;
                    DBProvider.Client.SetExternalLink(dish.Id, (int)order.MarketingChannelId.GetValueOrDefault(), (int)dp.ExternalCode);
                    dp.Code = (int)dish.Barcode;
                    dp.DishName = dish.Name;
                    dp.Dish = dish;
                    dp.DishId = dish.Id;
                }
                catch(Exception e)
                { 
                        

                }
                
                { }
                CloseAction();
            }
            );

            CancelCommand = new DelegateCommand(_ =>
            {
                CloseAction();
            }
            );




        }
        public ICommand OkCommand { set; get; }
        public ICommand CancelCommand { set; get; }

        ICollectionView _toGoClientsCol;
        public ICollectionView ToGoClientsCol
        {
            get
            {
                {
                    if (_toGoClientsCol == null)
                    {
                        QueryableCollectionView collectionViewSource =
                            new QueryableCollectionView(DataExtension.DataCatalogsSingleton.Instance.DishFilter.ActiveDishesToGo);
                        _toGoClientsCol = collectionViewSource;
                        SelectedClientVM = (Dish)ToGoClientsCol.CurrentItem;
                        //_toGoClientsCol.CurrentChanged += _toGoClientsCol_CurrentChanged;
                        _toGoClientsCol.MoveCurrentToFirst();
                    }
                }
                return _toGoClientsCol;
            }
        }
        [Reactive] public Dish SelectedClientVM { get; set; }

        [Reactive] public string InfoMessage { get; set; }




    }
}
