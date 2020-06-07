using AlohaService.Interfaces;
using AlohaService.ServiceDataContracts;
using System;
using Telerik.Windows.Controls;

namespace AlohaFly.LabelsPrint
{
    class LabelPageViewModel : ViewModelBase
    {

        IOrderLabel order;
        Dish d;
        public LabelPageViewModel(IOrderLabel _order, Dish _d)
        {
            order = _order;
            d = _d;
        }



        public string OrderInfoStr
        {
            get
            {
                if (order == null)
                {
                    return $"Наклейки к блюду {d.LabelRussianName}";
                }
                else if (order is OrderFlight)
                {
                    return $"Заказ на борт {((OrderFlight)order).FlightNumber} готовность к {((OrderFlight)order).ReadyTime.ToString("dd.MM.yy HH:mm")}";
                }
                else if (order is OrderToGo)
                {
                    return $"Номер заказа {((OrderToGo)order).Id}";
                }
                return "";
            }
        }

    }

    class LabelImageViewModel : ViewModelBase
    {
        Dish d;
        ItemLabelInfo l;
        DateTime dt;
        int LogoType = 0;
        public LabelImageViewModel(Dish _d, ItemLabelInfo _l, DateTime _dt, int logoType = 0)
        {
            d = _d;
            l = _l;
            dt = _dt;
            LogoType = logoType;

        }


        public string LDate
        {
            get
            {
                return dt.ToString("dd.MM.yyyy");
            }
        }

        public string BarCode
        {
            get
            {
                return d.Barcode.ToString();
            }
        }

        public string RussianItem
        {
            get
            {
                return d.LabelRussianName;
            }
        }
        public string EnglishItem
        {
            get
            {
                return d.LabelEnglishName;
            }
        }
        public string BJU1
        {
            get
            {
                return $"Белки:{d.B};Жиры:{d.J}";
            }
        }
        public string BJU2
        {
            get
            {
                return $"Углеводы:{d.U};Ккал:{d.Ccal}";
            }
        }

        public string PartStr
        {
            get
            {
                return $"Part {l.SerialNumber} of {d.LabelsCount}";
            }
        }


        public System.Windows.Visibility ToFlyLogoVisible
        {
            get
            {
                if (LogoType == 0)
                { return System.Windows.Visibility.Visible; }
                else
                {
                    return System.Windows.Visibility.Collapsed;
                }

            }
        }

        public System.Windows.Visibility ToGoLogoVisible
        {
            get
            {
                if (LogoType == 1)
                { return System.Windows.Visibility.Visible; }
                else
                {
                    return System.Windows.Visibility.Collapsed;
                }

            }
        }


        public System.Windows.Visibility CommentStrVisibility
        {
            get
            {
                if (CommentStr?.Trim() == "")
                { return System.Windows.Visibility.Collapsed; }
                else
                {
                    return System.Windows.Visibility.Visible;
                }

            }
        }



        public System.Windows.Visibility BJUVis
        {
            get
            {
                if (d.B==0 && d.Ccal==0 && d.J==0 && d.U==0)
                { return System.Windows.Visibility.Collapsed; }
                else
                {
                    return System.Windows.Visibility.Visible;
                }

            }
        }

        public string CommentStr
        {
            get
            {
                return l.Message;
            }
        }
        public string RussianComp
        {
            get
            {
                return l.NameRus;
            }
        }
        public string EnglishComp
        {
            get
            {
                return l.NameEng;
            }
        }


    }
}
