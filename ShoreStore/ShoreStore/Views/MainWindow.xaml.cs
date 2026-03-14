using System.Windows;
using ShoeStore.Services;

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

            UpdateUserInfo();
            SetupMenuByRole();

            MainFrame.Navigate(new ProductCatalog(_authService, _cartService));
        }

        private void UpdateUserInfo()
        {
            if (_authService.CurrentUser != null)
            {
                txtUserInfo.Text = _authService.CurrentUser.FullName;
                txtRoleInfo.Text = $"({_authService.GetRoleName()})";
            }
            else
            {
                txtUserInfo.Text = "Гость";
                txtRoleInfo.Text = "(просмотр каталога)";
            }
        }

        private void SetupMenuByRole()
        {
            // Показываем раздел администрирования только для администратора
            if (_authService.IsAdmin)
            {
                mnuAdmin.Visibility = Visibility.Visible;
            }
            else
            {
                mnuAdmin.Visibility = Visibility.Collapsed;
            }

            // Показываем заказы для менеджера и администратора
            mnuOrders.Visibility = _authService.IsManager ? Visibility.Visible : Visibility.Collapsed;

            if (_authService.IsGuest)
            {
                btnCart.IsEnabled = false;
                btnCart.ToolTip = "Для добавления товаров в корзину необходимо авторизоваться";
            }
        }

        private void mnuCatalog_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductCatalog(_authService, _cartService));
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

        // ЭТОТ МЕТОД ТЕПЕРЬ РАБОТАЕТ
        private void mnuProducts_Click(object sender, RoutedEventArgs e)
        {
            if (_authService.IsAdmin)
            {
                var adminWindow = new ProductManagementWindow(_authService);
                adminWindow.Owner = this;
                adminWindow.ShowDialog();

                // Обновляем каталог после изменений
                if (MainFrame.Content is ProductCatalog catalog)
                {
                    catalog.RefreshProducts();
                }
            }
            else
            {
                MessageBox.Show("У вас нет прав для управления товарами",
                              "Доступ запрещен",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void mnuAllOrders_Click(object sender, RoutedEventArgs e)
        {
            if (_authService.IsAdmin)
            {
                MessageBox.Show("Модуль управления всеми заказами в разработке",
                              "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void mnuUsers_Click(object sender, RoutedEventArgs e)
        {
            if (_authService.IsAdmin)
            {
                MessageBox.Show("Модуль управления пользователями в разработке",
                              "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnCart_Click(object sender, RoutedEventArgs e)
        {
            if (_authService.CurrentUser == null)
            {
                MessageBox.Show("Для работы с корзиной необходимо авторизоваться",
                              "Требуется авторизация",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var cartWindow = new CartWindow(_cartService, _authService);
            cartWindow.Owner = this;
            cartWindow.ShowDialog();
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