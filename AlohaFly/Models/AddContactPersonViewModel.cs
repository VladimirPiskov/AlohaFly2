using AlohaService.ServiceDataContracts;
using System;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace AlohaFly.Models
{
    public class AddContactPersonViewModel : ViewModelPane
    {
        public AddContactPersonViewModel()
        {

        }

        public AddContactPersonViewModel(ContactPerson cp)
        {
            CP = cp;
            OkCommand = new DelegateCommand(_ =>
            {


                Result = DataExtension.DataCatalogsSingleton.Instance.ContactPersonData.EndEdit(CP).Succeess;
                //long Id  =DBDataExtractor<ContactPerson>.AddItem(DBProvider.Client.CreateContactPerson, CP);

                if (Result)
                {

                    UI.UIModify.ShowAlert($"Контакт {cp.FullName} добавлен в базу.");
                }
                else
                {
                    UI.UIModify.ShowAlert($"Ошибка при добавлении контакта {cp.FullName}.");
                }
                CloseAction();
            }
            );
            CancelCommand = new DelegateCommand(_ =>
            {

                CloseAction();
            }
            );

            ChangeCommand = new DelegateCommand(_ =>
            {
                var s = CP.FirstName;
                CP.FirstName = CP.SecondName;
                CP.SecondName = s;
            });

        }
        public Action CloseAction { set; get; }
        public bool Result { set; get; } = false;
        public ICommand OkCommand { set; get; }
        public ICommand CancelCommand { set; get; }
        public ICommand ChangeCommand { set; get; }

        public ContactPerson CP { set; get; }


    }
}
