using AlohaService.ServiceDataContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlohaService.Interfaces
{
    public   interface IOrderLabel 
    {
        long Id { set; get; }
        DateTime DeliveryDate { get; set; }
        DateTime ReadyTime { get; set; }
        List<IDishPackageLabel> DishPackagesForLab { get; }
        string CommentKitchen { get; }
        OrderStatus OrderStatus { get; set; }
        bool IsSHSent { get; set; } 
        decimal DiscountPercent { get; set; }
        decimal OrderDishesSumm { get; }
        List<IDishPackageLabel> DishPackagesNoSpis { get; }
        List<IDishPackageLabel> DishPackagesSpis { get; }
        event PropertyChangedEventHandler PropertyChanged;
    }

    public interface IDishPackageLabel: IDeletedDish, INotifyPropertyChanged
    {
        bool PrintLabel { get; set; }
        int LabelSeriesCount { get; set; }
        long DishId { get; set; }
        Dish Dish { get; set; }
        int LabelsCount { get;}
        long PositionInOrder { get; set; }
        decimal Amount { get; set; }
        //string Comment { get; set; }
        bool Printed { get; set; } 
        decimal TotalPrice { get; set; }
        decimal TotalSumm { get;  }
        //decimal TotalSummWithDiscount { get; }

    }

    public interface IDeletedDish
    {
        bool Deleted { get; set; }
        int DeletedStatus { get; set; }
        string Comment { get; set; }
        string DishName { get; set; }
        long SpisPaymentId { get; set; }
        Payment SpisPayment { get; set; }
    }
    }
