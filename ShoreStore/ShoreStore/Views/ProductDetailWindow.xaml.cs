using ShoeStore.Models;
using ShoeStore.Services;
using System.Windows;

namespace ShoeStore.Views
{
    public partial class ProductDetailWindow : Window
    {
        private Product _product;
        private AuthService _authService;
        private CartService _cartService;

        public ProductDetailWindow(Product product, AuthService authService, CartService cartService)
        {
            InitializeComponent();
            _product = product;
            _authService = authService;
            _cartService = cartService;

            DataContext = _product;
        }

        private void btnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (_authService.CurrentUser == null)
            {
                MessageBox.Show("Для добавления товаров в корзину необходимо авторизоваться",
                              "Требуется авторизация",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_product.Quantity <= 0)
            {
                MessageBox.Show("Товара нет в наличии", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _cartService.AddToCart(_product);
            MessageBox.Show($"Товар \"{_product.Name}\" добавлен в корзину",
                          "Добавлено", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}