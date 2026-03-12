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
                            var product = new Product
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
                            };

                            products.Add(product);
                        }
                    }
                }

                
                itemsControl.ItemsSource = products;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}");
            }
        }
    }
}