using Npgsql;
using ShoeStore.Models;
using ShoeStore.Services;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ShoeStore.Views
{
    public partial class ProductCatalog : Page
    {
        private DatabaseContext _context;

        public ProductCatalog(AuthService authService)
        {
            InitializeComponent();
            _context = new DatabaseContext();
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
                    string query = "SELECT id, article, name, price, category, quantity FROM products ORDER BY name";

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
                                Price = reader.GetDecimal(3),
                                Category = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                Quantity = reader.GetInt32(5)
                            });
                        }
                    }
                }

                listProducts.ItemsSource = products;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }
    }
}