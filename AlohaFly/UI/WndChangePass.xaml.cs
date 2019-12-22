using System.Windows;
using Telerik.Windows.Controls;

namespace AlohaFly.UI
{
    /// <summary>
    /// Логика взаимодействия для WndChangePass.xaml
    /// </summary>
    public partial class WndChangePass : RadWindow
    {
        public WndChangePass()
        {
            InitializeComponent();
            Header = $"Смена пароля {Authorization.CurentUser.FullName}";
            tbOldPass.Focus();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (tbNewPass.Password != tbNewPass2.Password)
            {
                tbInfo.Text = "Пароли не совпадают";
                return;
            }

            if (!Authorization.DoAuthorization(Authorization.CurentUser.UserName, tbOldPass.Password))
            {
                tbInfo.Text = "Старый пароль не верен";
                return;
            }

            if (DBProvider.ChangePass(Authorization.CurentUser, tbNewPass.Password))
            {
                UI.UIModify.ShowAlert("Пароль изменен");
                this.Close();

            }


        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
