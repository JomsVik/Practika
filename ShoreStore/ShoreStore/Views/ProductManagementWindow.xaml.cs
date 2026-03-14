using Npgsql;
using ShoeStore.Models;
using ShoeStore.Services;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ShoeStore.Views
{
    public partial class ProductManagementWindow : Window
    {
        private DatabaseContext _context;
        private AuthService _authService;

        public ProductManagementWindow(AuthService authService)
        {
            InitializeComponent();
            _context = new DatabaseContext();
            _authService = authService;

            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                var products = new List<Product>();

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
                            products.Add(new Product
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

                listProducts.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}");
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new ProductEditWindow(_authService);
            editWindow.Owner = this;

            if (editWindow.ShowDialog() == true)
            {
                LoadProducts();
            }
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            if (button == null) return;

            int productId = (int)button.Tag;

            var editWindow = new ProductEditWindow(_authService, productId);
            editWindow.Owner = this;

            if (editWindow.ShowDialog() == true)
            {
                LoadProducts();
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            if (button == null) return;

            int productId = (int)button.Tag;

            var result = MessageBox.Show("Вы уверены, что хотите удалить этот товар?",
                                        "Подтверждение удаления",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var conn = _context.GetConnection())
                    {
                        conn.Open();

                        string checkQuery = "SELECT COUNT(*) FROM order_items WHERE product_id = @productId";
                        using (var checkCmd = new NpgsqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@productId", productId);
                            int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                            if (count > 0)
                            {
                                MessageBox.Show("Невозможно удалить товар, так как он есть в заказах",
                                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                        }

                        string deleteQuery = "DELETE FROM products WHERE id = @id";
                        using (var deleteCmd = new NpgsqlCommand(deleteQuery, conn))
                        {
                            deleteCmd.Parameters.AddWithValue("@id", productId);
                            deleteCmd.ExecuteNonQuery();
                        }
                    }

                    LoadProducts();
                    MessageBox.Show("Товар успешно удален", "Успех",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                }
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadProducts();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}