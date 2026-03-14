using ShoeStore.Models;
using ShoeStore.Services;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ShoeStore.Views
{
    public partial class CartWindow : Window
    {
        private CartService _cartService;
        private AuthService _authService;

        public CartWindow(CartService cartService, AuthService authService)
        {
            InitializeComponent();
            _cartService = cartService;
            _authService = authService;

            RefreshCart();
        }

        private void RefreshCart()
        {
            listCart.ItemsSource = null;
            listCart.ItemsSource = _cartService.Items;
            txtTotal.Text = _cartService.TotalAmount.ToString("N0") + " ₽";

            if (_cartService.TotalItems == 0)
            {
                btnCheckout.IsEnabled = false;
            }
            else
            {
                btnCheckout.IsEnabled = true;
            }
        }

        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int productId = (int)button.Tag;

            var item = _cartService.Items.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                _cartService.UpdateQuantity(productId, item.Quantity + 1);
                RefreshCart();
            }
        }

        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int productId = (int)button.Tag;

            var item = _cartService.Items.FirstOrDefault(x => x.ProductId == productId);
            if (item != null && item.Quantity > 1)
            {
                _cartService.UpdateQuantity(productId, item.Quantity - 1);
                RefreshCart();
            }
        }

        private void RemoveFromCart_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int productId = (int)button.Tag;

            _cartService.RemoveFromCart(productId);
            RefreshCart();
        }

        private void btnCheckout_Click(object sender, RoutedEventArgs e)
        {
            var checkoutWindow = new CheckoutWindow(_cartService, _authService);
            checkoutWindow.Owner = this;
            checkoutWindow.ShowDialog();

            if (checkoutWindow.DialogResult == true)
            {
                _cartService.ClearCart();
                RefreshCart();
                MessageBox.Show("Заказ успешно оформлен!", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}