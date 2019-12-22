namespace AlohaFly.Models
{
    /*
    public class ToGoClientInfoViewModel : ViewModelBase
    //public class ToGoClientInfoViewModel : ReactiveObject
    {
        public ToGoClientInfoModel Model;
        public ToGoClientInfoViewModel(ToGoClientInfoModel model)
        {
            Model = model;
        }

        public string FirstName
        {
            set
            {
                if (Model.Customer.Name != value)
                {
                    //this.RaiseAndSetIfChanged(ref Model.Customer.Name, value);
                    
                    Model.Customer.Name = value;
                    RaisePropertyChanged("FirstName");
                    
                }
            }
            get
            {
                return Model.Customer.Name;
            }
        }

        public string SecondName
        {
            set
            {
                if (Model.Customer.SecondName != value)
                {
                    Model.Customer.SecondName = value;
                    RaisePropertyChanged("SecondName");
                }
            }
            get
            {
                return Model.Customer.SecondName;
            }
        }


        public string MiddleName
        {
            set
            {
                if (Model.Customer.MiddleName != value)
                {
                    Model.Customer.MiddleName = value;
                    RaisePropertyChanged("MiddleName");
                }
            }
            get
            {
                return Model.Customer.MiddleName;
            }
        }

        public string Email
        {
            set
            {
                if (Model.Customer.Email != value)
                {
                    Model.Customer.Email = value;
                    RaisePropertyChanged("Email");
                }
            }
            get
            {
                return Model.Customer.Email;
            }
        }

        public string Comments
        {
            set
            {
                if (Model.Customer.Comments != value)
                {
                    Model.Customer.Comments = value;
                    RaisePropertyChanged("Comments");
                }
            }
            get
            {
                return Model.Customer.Comments;
            }
        }

    }

    public class ToGoClientInfoModel
    {
        public OrderCustomer Customer;
        public ToGoClientInfoModel(OrderCustomer orderCustomer)
        {
            Customer = orderCustomer;
        }
        public ToGoClientInfoModel()
        {
            Customer = new OrderCustomer();
        }
    }
    */
}
