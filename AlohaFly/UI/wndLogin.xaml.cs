using System.Windows;
using System.Windows.Input;

namespace AlohaFly.UI
{
    /// <summary>
    /// Логика взаимодействия для WndLogin.xaml
    /// </summary>
    public partial class wndLogin : Window
    {
        public bool Result = false;
        public wndLogin()
        {
            InitializeComponent();
            tbLogin.Focus();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {

            //Result = true;
            Result = Authorization.DoAuthorization(tbLogin.Text, tbPass.Password);
            if (Result)
            {
                this.Close();
            }
            else
            {
                tbInfo.Text = "Пароль ошибочен... либо Вас еще не существует";
            }
        }

        private void tbPass_TextInput(object sender, TextCompositionEventArgs e)
        {
            tbInfo.Text = "";
        }
    }
}
