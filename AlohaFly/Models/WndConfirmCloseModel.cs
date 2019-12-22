using System;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace AlohaFly.Models
{
    public class WndConfirmCloseModel : ViewModelBase
    {

        public int Result = 0;
        public bool Cancel = true;
        public WndConfirmCloseModel(string ask, Func<bool> okCommand)
        {
            Message = ask;
            OkCommand = new DelegateCommand((_) =>
            {
                Cancel = !okCommand();
                CloseAction();
            });
            NoCommand = new DelegateCommand((_) =>
            {
                Cancel = false;
                CloseAction();
            });
            CancelCommand = new DelegateCommand((_) =>
            {
                CloseAction();
            });
        }

        public static bool ShowWndConfirmCloseModel(string ask, Func<bool> okCommand) //true отменить закрытие
        {
            var wndConfirmCloseModel = new WndConfirmCloseModel(ask, okCommand);
            var wnd = new UI.WndConfirmClose()
            {
                DataContext = wndConfirmCloseModel,
                Owner = Application.Current.MainWindow
            };

            wnd.ShowDialog();
            return wndConfirmCloseModel.Cancel;
        }


        public string Message { set; get; }

        public Action CloseAction { get; set; }
        public ICommand OkCommand { get; set; }
        public ICommand NoCommand { get; set; }
        public ICommand CancelCommand { get; set; }

    }
}
