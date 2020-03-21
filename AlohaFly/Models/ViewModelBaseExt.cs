using ReactiveUI;
using System;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace AlohaFly.Models
{
    public class ViewModelPane : ViewModelBase
    {
        /*
        public void SetChanged(bool changed)
        {
            Changed = changed;
        }
        */


        public ViewModelPane()
        {
            SaveOrderCommand = new DelegateCommand(_ =>
            {
                if (SaveChanesFunction != null)
                {
                    if (SaveChanesFunction()) CloseAction();
                }
            });
        }
        bool changed = false;
        public bool Changed
        {
            set
            {
                if (changed != value)
                {
                    changed = value;
                    RaisePropertyChanged("Changed");
                    RaisePropertyChanged("Header");
                }
            }
            get
            {
                return changed;
            }
        }


        string header = "";
        public string Header
        {
            set
            {
                if (header != value)
                {
                    header = value;
                    RaisePropertyChanged("Header");
                }
            }
            get
            {
                return header + (Changed ? "*" : "");
            }
        }

        public Action CloseAction { get; set; }
        protected Action ReturnChangesAction { get; set; }
        protected Func<bool> SaveChanesFunction;
        protected string SaveChanesQuestion;


        protected ICommand SaveOrderCommand;

        public bool SaveChanesAsk() //false - отменить закрытие
        {
            if (SaveChanesFunction != null)
            {
                if (Changed)
                {
                    if (WndConfirmCloseModel.ShowWndConfirmCloseModel(SaveChanesQuestion, SaveChanesFunction))
                    {
                        return false;
                        //CloseAction?.Invoke();
                    }
                    else
                    {

                    }
                }
                else
                {
                    //return true;
                    //CloseAction?.Invoke();
                }
            }
            else
            {

                //CloseAction?.Invoke();
            }
            //return true;
            return true;
        }
    }



    public class ViewModelPaneReactiveObject : ReactiveObject, IDisposable
    {
        /*
        public void SetChanged(bool changed)
        {
            Changed = changed;
        }
        */

        protected IDisposable _cleanUp;

        public ViewModelPaneReactiveObject()
        {
            SaveOrderCommand = new DelegateCommand(_ =>
            {
                if (SaveChanesFunction != null)
                {
                    if (SaveChanesFunction()) CloseAction();
                }
            });
        }


        bool changed = false;
        public bool mChanged
        {
            set
            {
                if (changed != value)
                {
                    changed = value;
                }
            }
            get
            {
                return changed;
            }
        }


        string header = "";
        public string Header
        {
            set
            {
                if (header != value)
                {
                    header = value;
                    // RaisePropertyChanged("Header");
                }
            }
            get
            {
                return header + (mChanged ? "*" : "");
            }
        }

        public Action CloseAction { get; set; }
        protected Action ReturnChangesAction { get; set; }
        protected Func<bool> SaveChanesFunction;
        protected string SaveChanesQuestion;


        protected ICommand SaveOrderCommand;

        public bool SaveChanesAsk() //false - отменить закрытие
        {
            if (SaveChanesFunction != null)
            {
                if (mChanged)
                {
                    if (WndConfirmCloseModel.ShowWndConfirmCloseModel(SaveChanesQuestion, SaveChanesFunction))
                    {
                        return false;
                        //CloseAction?.Invoke();
                    }
                    else
                    {

                    }
                }
                else
                {
                    //return true;
                    //CloseAction?.Invoke();
                }
            }
            else
            {

                //CloseAction?.Invoke();
            }
            //return true;
            return true;
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}
