using ServiceDll.Models;
using ServiceDll.Realization;
using System.Windows;
using System.Windows.Media;

namespace WpfClientCursova.Windows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (tbEmail.Text != "" || tbPassword.Password != "")
            {
                lbWarning.Content = "";

                AccountApiService service = new AccountApiService();

                // посилаємо модель на сервер
                var responseObj = await service.LoginAsync(new AccountModel
                {
                    Email = tbEmail.Text,
                    Password = tbPassword.Password
                });

                if (responseObj == null)
                {
                    lbWarning.Foreground = Brushes.Red;
                    lbWarning.Content = "Неправильно введені дані";
                }
                else
                {
                    MainWindow window = new MainWindow(responseObj);
                    window.Show();
                    this.Close();
                }
            }
        }

        private void BtnRegistration_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow window = new RegistrationWindow();
            window.ShowDialog();
        }
    }
}
