namespace AlohaFly.Models
{
    /*
    public class AddOrderCustomerViewModel : ViewModelPane
    {
        public AddOrderCustomerViewModel()
        {
        
        }

        public AddOrderCustomerViewModel(OrderCustomer cp)
        {
            CP = cp;
            OkCommand = new DelegateCommand(_ => 
            {


                Result = DataExtension.DataCatalogsSingleton.Instance.AddOrUpdateToGoCustomer(CP);
                //long Id  =DBDataExtractor<ContactPerson>.AddItem(DBProvider.Client.CreateContactPerson, CP);

                if (Result)
                {
                    
                    UI.UIModify.ShowAlert($"Клиент {cp.FullName} добавлен в базу.");
                }
                else
                {
                    UI.UIModify.ShowAlert($"Ошибка при добавлении клиента {cp.FullName}.");
                }
                CloseAction();
            } 
            );
            CancelCommand = new DelegateCommand(_ =>
            {
                
                CloseAction();
            }
            );

            ChangeCommand = new DelegateCommand(_ => {
                var s = CP.Name;
                CP.Name = CP.SecondName;
                CP.SecondName = s;
            });

        }
        public Action CloseAction { set; get; }
        public bool Result { set; get; } = false;
        public ICommand OkCommand { set; get; }
        public ICommand CancelCommand { set; get; }
        public ICommand ChangeCommand { set; get; }

        public OrderCustomer CP { set; get; }


    }
    */
}
