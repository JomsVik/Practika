using System.Windows;
using ShoeStore.Services;

namespace ShoeStore.Views
{
    public partial class MainWindow : Window
    {
        private AuthService _authService;

        public MainWindow(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;

            if (_authService.CurrentUser != null)
            {
                txtUserInfo.Text = $"{_authService.CurrentUser.FullName} ({_authService.CurrentUser.Role.Name})";
            }

            // Скрываем меню заказов для гостей и клиентов
            mnuOrders.Visibility = _authService.IsManager ? Visibility.Visible : Visibility.Collapsed;

            // Открываем каталог
            MainFrame.Navigate(new ProductCatalog(_authService));
        }

        private void mnuCatalog_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductCatalog(_authService));
        }

        private void mnuOrders_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Модуль заказов в разработке");
        }

        private void mnuLogout_Click(object sender, RoutedEventArgs e)
        {
            _authService.Logout();
            new LoginWindow().Show();
            this.Close();
        }

        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}