using AlohaFly.Models;
using AlohaFly.Models.ToGoClient;
using AlohaFly.Reports;
using AlohaService.Interfaces;
using AlohaService.ServiceDataContracts;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Docking;
using Telerik.Windows.Documents.Spreadsheet.Model;

namespace AlohaFly.UI
{
    public static class UIModify
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        public static UserControl GetCtrlAddOrder(OrderFlight curentOrder)
        {


            var ctrlOrder = new CtrlAddOrder();
            //var flightOrder  = new  Order
            var addOrderModel = new Models.AddOrderModel(curentOrder);
            var addOrderVM = new Models.AddOrderViewModel(addOrderModel) { Header = $"Изменение заказа {curentOrder.Id}" };
            ctrlOrder.DataContext = addOrderVM;
            //  addOrderVM.CloseAction = new Action(() => { MainClass.HideUC(ctrlOrder); });
            ctrlOrder.Tag = $"Изменение заказа {curentOrder.Id}";
            return ctrlOrder;
        }

        public static UserControl GetCtrlAddOrder()
        {
            var ctrlOrder = new CtrlAddOrder();
            var addOrderModel = new Models.AddOrderModel();
            var addOrderVM = new Models.AddOrderViewModel(addOrderModel) { Header = "Добавление заказа ToFly" };
            //addOrderVM.CloseAction = new Action(() => { MainClass.HideUC(ctrlOrder); });
            ctrlOrder.DataContext = addOrderVM;
            //ctrlOrder.Tag = "Добавление заказа ToFly";
            return ctrlOrder;
        }

        public static UserControl GetCtrlAddToGoOrder()
        {
            var ctrlOrder = new CtrlAddToGoOrder();
            var addOrderModel = new Models.AddToGoOrderModel();
            var addOrderVM = new Models.AddToGoOrderViewModel(addOrderModel) { Header = "Добавление заказа ToGo" };
            ctrlOrder.DataContext = addOrderVM;
            return ctrlOrder;
        }

        public static UserControl GetCtrlAddToGoOrder(OrderToGo curentOrder)
        {


            var ctrlOrder = new CtrlAddToGoOrder();
            //var flightOrder  = new  Order
            var addOrderModel = new Models.AddToGoOrderModel(curentOrder);
            var addOrderVM = new Models.AddToGoOrderViewModel(addOrderModel) { Header = $"Изменение заказа {curentOrder.Id}" };
            ctrlOrder.DataContext = addOrderVM;
            //  addOrderVM.CloseAction = new Action(() => { MainClass.HideUC(ctrlOrder); });
            ctrlOrder.Tag = $"Изменение заказа {curentOrder.Id}";
            return ctrlOrder;
        }


        public static UserControl GetCtrlOrdersFlight()
        {
            var ctrlOrder = new CtrlAirOrders();
            var addOrderVM = new Models.AirOrdersViewModel() { Header = "Заказы ToFly" };
            ctrlOrder.DataContext = addOrderVM;
            ctrlOrder.Tag = "Заказы ToFly";
            return ctrlOrder;
        }

        public static UserControl GetCtrlOrdersToGo()
        {
            var ctrlOrder = new CtrlToGoOrders();
            var addOrderVM = new Models.ToGoOrdersViewModel() { Header = "Заказы ToGo" };
            ctrlOrder.DataContext = addOrderVM;
            ctrlOrder.Tag = "Заказы ToFly";
            return ctrlOrder;
        }

        public static UserControl GetCtrlOrdersFlightByAirCompaneis()
        {
            var ctrlOrder = new AirCompaniesOrders();
            var addOrderVM = new Models.AirCompaniesOrdersViewModel() { Header = "Заказы по авиакомпаниям" };
            ctrlOrder.DataContext = addOrderVM;

            return ctrlOrder;
        }

        public static UserControl GetCtrlOrdersNonSH()
        {
            var ctrlOrder = new CtrlOrdersNonSH();
            var addOrderVM = new Models.OrdersNonSHViewModel() { Header = "Не списанные заказы" };
            ctrlOrder.DataContext = addOrderVM;

            return ctrlOrder;
        }


        

        public static bool ShowOpenFileDialog(out string fileName, string filter = "", string caption = "Выберите файл")
        {
            fileName = "";
            try
            {

                RadOpenFileDialog openFileDialog = new RadOpenFileDialog();
                openFileDialog.Owner = MainClass.MainAppwindow;
                //openFileDialog.Filter = filter;

                openFileDialog.ShowDialog();
                fileName = openFileDialog.FileName;

                return openFileDialog.DialogResult == true;
            }
            catch (Exception e)
            {
                logger.Error("Error ShowOpenFileDialog " + e.Message);
                return false;
            }

        }

        //private static WndShowMap mapWnd = new WndShowMap();
        public static void ShowWndMap(OrderCustomerAddress address)
        {
            try
            {
                logger.Debug("ShowWndMap");
                

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    var model = new ShowMapViewModel(address);

                    WndShowMap mapWnd = new WndShowMap
                    {
                        DataContext = model
                    };

                    ShowDialogWnd(mapWnd);
                });
            }
            catch (Exception e)
            {
                logger.Error($"ShowWndChangeOrderStatus {e.Message}");
            }
        }


        public static void ShowWndMergeCustomers(ToGoClientViewModel parentClient)
        {
            try
            {
                logger.Debug("ShowWndMap");

              

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    var model = new ToGoMergeCustomers(parentClient);

                    var mergeWnd = new UI.ToGo.WndMergeCustomers
                    {
                        DataContext = model
                    };

                    ShowDialogWnd(mergeWnd);
                });
            }
            catch (Exception e)
            {
                logger.Error($"ShowWndChangeOrderStatus {e.Message}");
            }
        }



        public static void ShowWndChangeOrderStatus(OrderFlight order)
        {
            try
            {
                logger.Debug("ShowWndChangeOrderStatus");

                if (order == null) { ShowAlert("Нет выделенного заказа для смены статуса"); return; }
                var vm = new ChangeOrderStatusViewModel(order);
                var wnd = new WndChangeOrderStatus();
                wnd.DataContext = vm;
                wnd.ShowDialog();
            }
            catch (Exception e)
            {
                logger.Error($"ShowWndChangeOrderStatus {e.Message}");
            }
        }

        public static void ShowWndPrintLabels(IOrderLabel order)
        {
            try
            {
                logger.Debug("ShowWndPrintLabels");
                if (order == null) { ShowAlert("Нет выделенного заказа для печати"); return; }
                if ((order.DishPackagesForLab == null) || (order.DishPackagesForLab.Count() == 0)) { ShowAlert("Нет блюд в заказе для печати"); return; }
                //var vm = new PrintLabelsViewModel(order);
                var vm = new AddLabelsViewModel(order) { Header = $"Печать наклеек заказ {order.Id}" };
                //var ctrl = new CtrlPrintLabels();
                var ctrl = new ctrlItemLabels();
                ctrl.DataContext = vm;

                MainClass.ShowUC(ctrl);
            }
            catch (Exception e)
            {
                logger.Error($"ShowWndChangeOrderStatus {e.Message}");
            }
        }

        /*
        public static void ShowWndPrintLabels(OrderToGo order)
        {
            
            if (order == null) { ShowAlert("Нет выделенного заказа для печати"); return; }
            if ((order.DishPackages == null) || (order.DishPackages.Count() == 0)) { ShowAlert("Нет блюд в заказе для печати"); return; }
           
            var vm = new AddLabelsViewModel(order as IOrderLabel) { Header = $"Печать наклеек заказ {order.Id}" };
            //var ctrl = new CtrlPrintLabels();
            var ctrl = new ctrlItemLabels();
            ctrl.DataContext = vm;

            MainClass.ShowUC(ctrl);
            
        }
        */
        public static void ShowWndPrintForKitchen(OrderFlight order)
        {
            if (order == null) { ShowAlert("Нет выделенного заказа для печати"); return; }
            if ((order.DishPackagesNoSpis == null) || (order.DishPackagesNoSpis.Count() == 0)) { ShowAlert("Нет блюд в заказе для печати"); return; }

            (new WordReports()).PrintKitchenDocumentToFly(order);

            /*
            var printOrderKitchenToFly = new PrintOrderKitchenToFly(order) { Header = $"Печать заказа для кухни {order.Id}" };
            var ctrl = new CtrlPrintOrder();
            ctrl.Tag = $"Печать заказа для кухни {order.Id}";
            ctrl.DataContext = printOrderKitchenToFly;
            MainClass.ShowUC(ctrl);
            */
        }

        public static void ShowWndPrintExcelDoc(string header, Workbook wb)
        {
            try
            {
                // if (order == null) { ShowAlert("Нет выделенного заказа для печати"); return; }
                //  if ((order.DishPackages == null) || (order.DishPackages.Count() == 0)) { ShowAlert("Нет блюд в заказе для печати"); return; }
                var invoiceGenerateViewModel = new InvoiceGenerateViewModel(
                    wb
                //) {Header = $"Накладная {invoiceGenerateType} к заказу {order.Id}" };
                )
                { Header = header };
                var ctrl = new CtrlInvoiceSpreadSheet();
                ctrl.DataContext = invoiceGenerateViewModel;
                MainClass.ShowUC(ctrl);
            }
            catch
            { }
        }


        public static WindowClosedEventArgs ShowConfirm(string alertText, Action<bool> action = null, string header = "Подтвердите действие")
        {
            WindowClosedEventArgs result = null;
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {

                var dp = new DialogParameters
                {
                    Content = new TextBlock() { Text = alertText, TextWrapping = TextWrapping.Wrap, Width = 350 },
                    Header = header,
                    DialogStartupLocation = WindowStartupLocation.CenterScreen,
                    
                    
                };

                if (!((System.Windows.Application.Current.MainWindow == null) || (System.Windows.Application.Current.MainWindow.Visibility != System.Windows.Visibility.Visible)))
                {
                    dp.Owner = System.Windows.Application.Current.MainWindow;
                }
                if (action == null)
                {
                    dp.Closed = new EventHandler<WindowClosedEventArgs>((s, e) => { result = e; });
                }

                else
                {
                    dp.Closed = new EventHandler<WindowClosedEventArgs>((s, e) => { action(e.DialogResult.Value); });
                }
                dp.OkButtonContent = "Да";
                dp.CancelButtonContent = "Нет";
                RadWindow.Confirm(dp);

            });
            return result;
        }





        public static WindowClosedEventArgs ShowPromt(string promtText, string okButtonContent = "Да", string сancelButtonContent = "Нет", string header = "Подтвердите действие", bool confirm = false)
        {
            WindowClosedEventArgs result = null;
            logger.Debug($"ShowPromt: {promtText}");
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {

                var dp = new DialogParameters
                {
                    Content = new TextBlock() { Text = promtText, TextWrapping = TextWrapping.Wrap, Width = 350 },
                    Header = header,
                    DialogStartupLocation = WindowStartupLocation.CenterScreen
                };

                if (!((System.Windows.Application.Current.MainWindow == null) || (System.Windows.Application.Current.MainWindow.Visibility != System.Windows.Visibility.Visible)))
                {
                    dp.Owner = System.Windows.Application.Current.MainWindow;
                }
                dp.Closed = new EventHandler<WindowClosedEventArgs>((s, e) =>
                {

                    result = e;
                    logger.Debug($"ShowPromt result: {e.DialogResult}");
                });
                dp.OkButtonContent = okButtonContent;
                dp.CancelButtonContent = сancelButtonContent;
                if (confirm)
                {
                    RadWindow.Confirm(dp);
                }
                else
                {
                    RadWindow.Prompt(dp);
                }
            });
            return result;
        }

        /*
        public static void ShowSaveBeforeClosedConfirm(string alertText, Action<bool> action)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {

                var dp = new DialogParameters
                {
                    Content = alertText,
                };

                if (!((System.Windows.Application.Current.MainWindow == null) || (System.Windows.Application.Current.MainWindow.Visibility != System.Windows.Visibility.Visible)))
                {
                    dp.Owner = System.Windows.Application.Current.MainWindow;
                }
                dp.Closed = new EventHandler<WindowClosedEventArgs>((s, e) => { action(e.DialogResult.Value); });
                dp.OkButtonContent = "Да";
                
                dp.CancelButtonContent = "Нет";
                RadWindow.Confirm(dp);

            });
        }
        */

        public static void ShowInConstruction()
        {
            ShowAlert("Данная функция пока в разработке." + Environment.NewLine + "Заработает очень скоро");
        }

        public static void ShowAlert(string alertText)
        {
            logger.Debug($"ShowAlert {alertText}");
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {

                var dp = new DialogParameters
                {
                    Content = alertText,
                    Header = "Внимание!",
                    DialogStartupLocation = WindowStartupLocation.CenterScreen
                };

                if (!((System.Windows.Application.Current.MainWindow == null) || (System.Windows.Application.Current.MainWindow.Visibility != System.Windows.Visibility.Visible)))
                {
                    dp.Owner = System.Windows.Application.Current.MainWindow;
                }
                RadWindow.Alert(dp);

            }
            );
        }

        public static void ShowDialogWnd(RadWindow wnd)
        {
            wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            wnd.Owner = System.Windows.Application.Current.MainWindow;
            //wnd.Show();
            wnd.ShowDialog();
        }
    }

    public class CustomDockingPanesFactory : DockingPanesFactory
    {
        protected override void AddPane(Telerik.Windows.Controls.RadDocking radDocking, Telerik.Windows.Controls.RadPane pane)
        {
            //var tag = pane.Tag.ToString();
            var paneGroup = radDocking.SplitItems.ToList().FirstOrDefault(i => i.Control.Name.Contains("MainRadPaneGroup")) as RadPaneGroup;
            //var paneGroup = radDocking.SplitItems.ToList().FirstOrDefault() as RadPaneGroup;

            if (paneGroup != null)
            {
                paneGroup.Items.Add(pane);
            }
            else
            {
                base.AddPane(radDocking, pane);
            }
        }
    }


    public class OpenItemStyleSelector : StyleSelector
    {
        List<long> GreenBars = new List<long>() { 1697, 1718, 446 };
        List<long> BlueBars = new List<long>() { 1698, 1719, 447 };

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is DishPackageFlightOrder club)
            {
                //DishPackageFlightOrder club = item as DishPackageFlightOrder;

                if (club.Deleted)
                {
                    return DeletedStyle;
                }

                if ((club.Dish != null) && (GreenBars.Contains(club.Dish.Barcode)))
                {
                    return OpenItemStyle1;
                }
                if ((club.Dish != null) && (BlueBars.Contains(club.Dish.Barcode)))
                {
                    return OpenItemStyle2;
                }
               
                if (Authorization.IsDirector)
                {
                    if ((club.Dish != null) && (club.Dish.PriceForFlight != club.TotalPrice))
                    {
                        return ChangePriceStyle;
                    }
                }



            }

            if (item is DishPackageToGoOrder toGoOrder)
            {
                if (toGoOrder.Deleted)
                {
                    return DeletedStyle;
                }
                if ((toGoOrder.Dish != null) && (toGoOrder.Dish.Barcode == -1))
                {
                    return OpenItemStyle3;
                }

            }
            return null;
        }
        public Style OpenItemStyle1 { get; set; }
        public Style OpenItemStyle2 { get; set; }
        public Style OpenItemStyle3 { get; set; }
        public Style DeletedStyle { get; set; }
        public Style ChangePriceStyle { get; set; }

    }


    public class StadiumCapacityStyle : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is OrderFlight)
            {
                OrderFlight club = item as OrderFlight;
                if (club.OrderStatus == OrderStatus.InWork)
                {
                    return WhiteStyle;
                }
                else if (club.OrderStatus == OrderStatus.Sent)
                {
                    return LightBlueStyle;
                }
                else if (club.OrderStatus == OrderStatus.Cancelled)
                {
                    return RedStyle;
                }
                else if (club.OrderStatus == OrderStatus.CancelledWithRemains)
                {
                    return RoseStyle;
                }
                else if (club.OrderStatus == OrderStatus.Closed)
                {
                    return CloseStyle;
                }
                else
                {
                    return WhiteStyle;
                }
            }
            if (item is OrderToGo)
            {
                OrderToGo club = item as OrderToGo;
                if (club.OrderStatus == OrderStatus.InWork)
                {
                    return WhiteStyle;
                }
                else if (club.OrderStatus == OrderStatus.Sent)
                {
                    return LightBlueStyle;
                }
                else if (club.OrderStatus == OrderStatus.Cancelled)
                {
                    return RedStyle;
                }
                else if (club.OrderStatus == OrderStatus.CancelledWithRemains)
                {
                    return RoseStyle;
                }
                else if (club.OrderStatus == OrderStatus.Closed)
                {
                    return CloseStyle;
                }
                else
                {
                    return WhiteStyle;
                }
            }
            return null;
        }


        public Style CloseStyle { get; set; }
        public Style RoseStyle { get; set; }
        public Style RedStyle { get; set; }
        public Style WhiteStyle { get; set; }
        public Style LightBlueStyle { get; set; }
    }

    public class AttachedMouseBinding
    {
        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(AttachedMouseBinding),
            new FrameworkPropertyMetadata(CommandChanged));

        private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InputBinding inputBinding = d as InputBinding;
            ICommand command = e.NewValue as ICommand;
            if (inputBinding != null)
            {
                inputBinding.Command = command;
            }
        }
    }
    public static class FocusExtension
    {
        public static bool GetIsFocused(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFocusedProperty);
        }

        public static void SetIsFocused(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFocusedProperty, value);
        }

        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached(
                "IsFocused", typeof(bool), typeof(FocusExtension),
                new UIPropertyMetadata(false, OnIsFocusedPropertyChanged));

        private static void OnIsFocusedPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {

            var uie = (UIElement)d;
            if ((bool)e.NewValue)
            {
                uie.Focus(); // Don't care about false values.
                d.SetValue(IsFocusedProperty, false);
            }
        }
    }
    public class EnterKeyTraversal
    {
        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }

        static void ue_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var ue = e.OriginalSource as FrameworkElement;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                ue.MoveFocus(new TraversalRequest(System.Windows.Input.FocusNavigationDirection.Next));
            }
        }

        private static void ue_Unloaded(object sender, RoutedEventArgs e)
        {
            var ue = sender as FrameworkElement;
            if (ue == null) return;

            ue.Unloaded -= ue_Unloaded;
            ue.PreviewKeyDown -= ue_PreviewKeyDown;
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool),

            typeof(EnterKeyTraversal), new UIPropertyMetadata(false, IsEnabledChanged));

        static void IsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ue = d as FrameworkElement;
            if (ue == null) return;

            if ((bool)e.NewValue)
            {
                ue.Unloaded += ue_Unloaded;
                ue.PreviewKeyDown += ue_PreviewKeyDown;
            }
            else
            {
                ue.PreviewKeyDown -= ue_PreviewKeyDown;
            }
        }
    }
    public static class SpreadsheetAttachedProperties
    {
        public static readonly DependencyProperty WorkbookProperty =
            DependencyProperty.RegisterAttached("Workbook", typeof(Workbook),
            typeof(SpreadsheetAttachedProperties), new PropertyMetadata(null, OnWorkbookChanged));

        public static Workbook GetWorkbook(DependencyObject obj)
        {
            return (Workbook)obj.GetValue(WorkbookProperty);
        }

        public static void SetWorkbook(DependencyObject obj, Workbook value)
        {
            obj.SetValue(WorkbookProperty, value);
        }

        private static void OnWorkbookChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            RadSpreadsheet spreadsheet = d as RadSpreadsheet;
            if (spreadsheet != null)
            {
                spreadsheet.Workbook = (Workbook)e.NewValue;
            }
        }
    }
}
