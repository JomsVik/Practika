using Npgsql;
using ShoeStore.Models;
using ShoeStore.Services;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShoeStore.Views
{
    public partial class ProductCatalog : Page
    {
        private DatabaseContext _context;
        private AuthService _authService;
        private CartService _cartService;
        private List<Product> _products;

        public ProductCatalog(AuthService authService, CartService cartService)
        {
            InitializeComponent();
            _context = new DatabaseContext();
            _authService = authService;
            _cartService = cartService;
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                _products = new List<Product>();

                using (var conn = _context.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        SELECT id, article, name, unit, price, supplier, 
                               manufacturer, category, discount, quantity, 
                               description, image_url 
                        FROM products 
                        ORDER BY name";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _products.Add(new Product
                            {
                                Id = reader.GetInt32(0),
                                Article = reader.GetString(1),
                                Name = reader.GetString(2),
                                Unit = reader.GetString(3),
                                Price = reader.GetDecimal(4),
                                Supplier = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                Manufacturer = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                Category = reader.IsDBNull(7) ? "" : reader.GetString(7),
                                Discount = reader.IsDBNull(8) ? 0 : reader.GetInt32(8),
                                Quantity = reader.GetInt32(9),
                                Description = reader.IsDBNull(10) ? "" : reader.GetString(10),
                                ImageUrl = reader.IsDBNull(11) ? "" : reader.GetString(11)
                            });
                        }
                    }
                }

                itemsControl.ItemsSource = _products;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}");
            }
        }

        public void RefreshProducts()
        {
            LoadProducts();
        }

        private void Product_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border == null) return;

            int productId = (int)border.Tag;
            var product = _products.Find(p => p.Id == productId);

            if (product != null)
            {
                var detailWindow = new ProductDetailWindow(product, _authService, _cartService);
                detailWindow.Owner = Window.GetWindow(this);
                detailWindow.ShowDialog();
            }
        }
    }
}