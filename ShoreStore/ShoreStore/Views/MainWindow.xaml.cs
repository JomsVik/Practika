using System.Windows;
using ShoeStore.Services;
using ShoeStore.Views;

namespace ShoeStore.Views
{
    public partial class MainWindow : Window
    {
        private AuthService _authService;
        private CartService _cartService;

        public MainWindow(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;
            _cartService = new CartService();

            if (_authService.CurrentUser != null)
            {
                txtUserInfo.Text = $"{_authService.CurrentUser.FullName} ({_authService.CurrentUser.Role.Name})";
            }

            mnuOrders.Visibility = _authService.IsManager ? Visibility.Visible : Visibility.Collapsed;
            MainFrame.Navigate(new ProductCatalog(_authService, _cartService));
        }

        private void mnuCatalog_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductCatalog(_authService, _cartService));
        }

        private void btnCart_Click(object sender, RoutedEventArgs e)
        {
            var cartWindow = new CartWindow(_cartService, _authService);
            cartWindow.Owner = this;
            cartWindow.ShowDialog();
        }

        private void mnuOrders_Click(object sender, RoutedEventArgs e)
        {
            if (_authService.CurrentUser != null)
            {
                var ordersWindow = new UserOrdersWindow(_authService);
                ordersWindow.Owner = this;
                ordersWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Для просмотра заказов необходимо авторизоваться",
                              "Требуется авторизация",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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