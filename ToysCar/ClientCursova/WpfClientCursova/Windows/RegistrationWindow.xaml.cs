using ServiceDll.Models;
using ServiceDll.Realization;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfClientCursova.Windows
{
    /// <summary>
    /// Interaction logic for RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }
        private async void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            tbWarningFirstName.Text = "";
            tbWarningLastName.Text = "";
            tbWarningPassword.Text = "";
            tbWarningConfirmPassword.Text = "";
            tbWarningEmail.Text = "";
            tbWarningPhone.Text = "";

            if (tbPassword.Password != tbConfirmPassword.Password)
            {
                tbWarningPassword.Text = "Пароль не співпадає";
                tbWarningConfirmPassword.Text = "Пароль не співпадає";
                return;
            }
            if (tbFirstName.Text == tbLastName.Text && tbFirstName.Text != "")
            {
                tbWarningFirstName.Text = "Поле FirstName та LastName не можуть співпадати";
                tbWarningLastName.Text = "Поле FirstName та LastName не можуть співпадати";
                return;
            }

            // відправляємо модель на сервер
            AccountApiService service = new AccountApiService();

            var errorList = await service.RegistrationAsync(new UserModel
            {
                FirstName = tbFirstName.Text,
                LastName = tbLastName.Text,
                Password = tbPassword.Password,
                Email = tbEmail.Text,
                Phone = tbPhone.Text
            });

            // витягуємо помилки, якщо поля невалідні
            if (errorList != null)
            {
                foreach (var item in errorList)
                {
                    if ("firstName" == item.Key)
                        tbWarningFirstName.Text = item.Value;

                    if ("lastName" == item.Key)
                        tbWarningLastName.Text = item.Value;

                    if ("password" == item.Key)
                        tbWarningPassword.Text = item.Value;

                    if("email" == item.Key)
                        tbWarningEmail.Text = item.Value;

                    if("phone" == item.Key)
                        tbWarningPhone.Text = item.Value;
                }


            }
            // в іншому випадку реєстрація успішна
            else
            {
                tbWarningPhone.Foreground = Brushes.Blue;
                tbWarningPhone.Text = "Реєстрація успішна";

                btnSend.IsEnabled = false;
            }
        }

        private void TbPhone_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (tbPhone.Text.Length <= 1)
            {
                tbPhone.Text = "+3";
                tbPhone.SelectionStart = 2;
                tbPhone.SelectionLength = 0;
            }
        }
    }
}
