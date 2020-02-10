using AlohaFly.DataExtension;
using AlohaFly.Utils;
using AlohaService.Interfaces;
using AlohaService.ServiceDataContracts;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace AlohaFly.Models
{
    class WndDeleteDishModel : ReactiveObject
    {
        /*
        private readonly ReadOnlyObservableCollection<Payment> _spisPayments;
        
        */
        IDeletedDish dish;
         public FullyObservableCollection<Payment> SpisPayments { set; get; }



        public WndDeleteDishModel(IDeletedDish _dish)
        {
            dish = _dish;

            if (dish is DishPackageFlightOrder)
            {
                SpisPayments =  DataCatalogsSingleton.Instance.PaymentFilter.SpisPaymnets;
            }
            else
            {
                SpisPayments = DataCatalogsSingleton.Instance.PaymentFilter.ToGoSpisPaymnets;
            }

            /*
            //_spisPayments = new ReadOnlyObservableCollection<Payment> (DataCatalogsSingleton.Instance.PaymentsSourceCache.Items.AsObservableChangeSet());
            DataCatalogsSingleton.Instance.PaymentsSourceCache.Connect()
            .Filter(x => x.IsActive && x.PaymentGroup != null && !x.PaymentGroup.Sale && (x.ToGo ^ dish is DishPackageFlightOrder))
            //.ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _spisPayments)
            .Subscribe();
            */

            if (dish != null && dish.Deleted)
            {
                if (dish.SpisPaymentId != 0)
                {
                    SelectedSpis = DataCatalogsSingleton.Instance.PaymentData.Data.SingleOrDefault(x => x.Id == dish.SpisPaymentId);
                }
                DeletedComment = dish.Comment;
                Spis = (dish.DeletedStatus == 1);
                NoSpis = (dish.DeletedStatus == 2);
            }


            NoDeleteCommand = new DelegateCommand((_) =>
            {
                if (dish != null)
                {
                    dish.Deleted = false;
                    dish.Comment = "";
                    dish.DeletedStatus = 0;
                    Result = true;
                    dish.SpisPaymentId = 0;
                    dish.UpDateSpisPayment();
                }
                CloseAction();
            });


            OkCommand = new DelegateCommand((_) =>
        {

            if (Spis && (SelectedSpis == null || SelectedSpis.Id == 0))
            {
                UI.UIModify.ShowAlert("Необходимо указать причину списания!");
                return;
            }

            if (dish != null)
            {
                dish.Deleted = true;
                dish.Comment = DeletedComment;
                dish.DeletedStatus = deletedStatus;
                if (SelectedSpis != null)
                {
                    dish.SpisPaymentId = SelectedSpis.Id;
                    dish.UpDateSpisPayment();
                }
                Result = true;
            }
            CloseAction();
        });

            CancelCommand = new DelegateCommand((_) =>
            {

                CloseAction();
            });
        }
        public bool Result = false;

        [Reactive] public string DeletedComment { get; set; } = "";
        int deletedStatus
        {
            get
            {
                if (Spis) return 1;
                else return 2;
            }
        }

        public string BtnOkName
        {
            get
            {
                if (dish == null || !dish.Deleted)
                {
                    return "Удалить";
                }
                return "Восстановить";
            }
        }

        [Reactive] public bool Spis { get; set; } = true;
        [Reactive] public bool NoSpis { get; set; } = false;
        [Reactive] public Payment SelectedSpis { get; set; }


        public string WndPromt { get { return $"Для удаления блюда {dish.DishName} укажите причину удаления и тип списания"; } }


        public static bool ShowWndDeleteDish(IDeletedDish dish)
        {
            var wndDeleteDishModel = new WndDeleteDishModel(dish);
            var wnd = new UI.WndDeleteDish()
            {
                DataContext = wndDeleteDishModel,
                Owner = Application.Current.MainWindow
            };
            wnd.ShowDialog();

            return wndDeleteDishModel.Result;
        }
        public Action CloseAction { get; set; }
        public ICommand OkCommand { get; set; }
        public ICommand NoDeleteCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        //public ICommand CancelCommand { get; set; }




    }
}
