using System.Windows;
using ShoeStore.Services;

namespace ShoeStore.Views
{
    public partial class LoginWindow : Window
    {
        private AuthService _authService;

        public LoginWindow()
        {
            InitializeComponent();
            _authService = new AuthService();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                txtStatus.Text = "Введите логин и пароль";
                return;
            }

            var user = _authService.Login(login, password);

            if (user != null)
            {
                var mainWindow = new MainWindow(_authService);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                txtStatus.Text = "Неверный логин или пароль";
                txtPassword.Password = "";
            }
        }

        private void btnGuest_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow(_authService);
            mainWindow.Show();
            this.Close();
        }

        private void RegisterLink_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }
    }
}