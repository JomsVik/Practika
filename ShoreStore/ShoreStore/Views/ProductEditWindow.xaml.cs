using Npgsql;
using ShoeStore.Models;
using ShoeStore.Services;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ShoeStore.Views
{
    public partial class ProductEditWindow : Window
    {
        private DatabaseContext _context;
        private AuthService _authService;
        private int? _productId;
        private List<string> _categories;

        public ProductEditWindow(AuthService authService, int? productId = null)
        {
            InitializeComponent();
            _context = new DatabaseContext();
            _authService = authService;
            _productId = productId;

            if (_productId.HasValue)
            {
                txtTitle.Text = "Редактирование товара";
                LoadProduct();
            }

            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                _categories = new List<string>();

                using (var conn = _context.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT DISTINCT category FROM products WHERE category IS NOT NULL ORDER BY category";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _categories.Add(reader.GetString(0));
                        }
                    }
                }

                cmbCategory.ItemsSource = _categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}");
            }
        }

        private void LoadProduct()
        {
            try
            {
                using (var conn = _context.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        SELECT article, name, unit, price, supplier, 
                               manufacturer, category, discount, quantity, 
                               description, image_url 
                        FROM products 
                        WHERE id = @id";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", _productId.Value);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtArticle.Text = reader.GetString(0);
                                txtName.Text = reader.GetString(1);
                                txtUnit.Text = reader.GetString(2);
                                txtPrice.Text = reader.GetDecimal(3).ToString();
                                txtSupplier.Text = reader.IsDBNull(4) ? "" : reader.GetString(4);
                                txtManufacturer.Text = reader.IsDBNull(5) ? "" : reader.GetString(5);
                                cmbCategory.Text = reader.IsDBNull(6) ? "" : reader.GetString(6);
                                txtDiscount.Text = reader.GetInt32(7).ToString();
                                txtQuantity.Text = reader.GetInt32(8).ToString();
                                txtDescription.Text = reader.IsDBNull(9) ? "" : reader.GetString(9);
                                txtImageUrl.Text = reader.IsDBNull(10) ? "" : reader.GetString(10);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товара: {ex.Message}");
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtArticle.Text))
                {
                    MessageBox.Show("Введите артикул", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(txtName.Text))
                {
                    MessageBox.Show("Введите наименование товара", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
                {
                    MessageBox.Show("Введите корректную цену", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(txtDiscount.Text, out int discount) || discount < 0 || discount > 100)
                {
                    MessageBox.Show("Скидка должна быть от 0 до 100", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
                {
                    MessageBox.Show("Введите корректное количество", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var conn = _context.GetConnection())
                {
                    conn.Open();

                    if (_productId.HasValue)
                    {
                        string updateQuery = @"
                            UPDATE products 
                            SET article = @article, name = @name, unit = @unit, 
                                price = @price, supplier = @supplier, manufacturer = @manufacturer,
                                category = @category, discount = @discount, quantity = @quantity,
                                description = @description, image_url = @imageUrl
                            WHERE id = @id";

                        using (var cmd = new NpgsqlCommand(updateQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", _productId.Value);
                            cmd.Parameters.AddWithValue("@article", txtArticle.Text);
                            cmd.Parameters.AddWithValue("@name", txtName.Text);
                            cmd.Parameters.AddWithValue("@unit", txtUnit.Text);
                            cmd.Parameters.AddWithValue("@price", price);
                            cmd.Parameters.AddWithValue("@supplier", string.IsNullOrEmpty(txtSupplier.Text) ? DBNull.Value : (object)txtSupplier.Text);
                            cmd.Parameters.AddWithValue("@manufacturer", string.IsNullOrEmpty(txtManufacturer.Text) ? DBNull.Value : (object)txtManufacturer.Text);
                            cmd.Parameters.AddWithValue("@category", string.IsNullOrEmpty(cmbCategory.Text) ? DBNull.Value : (object)cmbCategory.Text);
                            cmd.Parameters.AddWithValue("@discount", discount);
                            cmd.Parameters.AddWithValue("@quantity", quantity);
                            cmd.Parameters.AddWithValue("@description", string.IsNullOrEmpty(txtDescription.Text) ? DBNull.Value : (object)txtDescription.Text);
                            cmd.Parameters.AddWithValue("@imageUrl", string.IsNullOrEmpty(txtImageUrl.Text) ? DBNull.Value : (object)txtImageUrl.Text);

                            cmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("Товар успешно обновлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        string checkQuery = "SELECT COUNT(*) FROM products WHERE article = @article";
                        using (var checkCmd = new NpgsqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@article", txtArticle.Text);
                            int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                            if (count > 0)
                            {
                                MessageBox.Show("Товар с таким артикулом уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                        }

                        string insertQuery = @"
                            INSERT INTO products (article, name, unit, price, supplier, manufacturer,
                                                  category, discount, quantity, description, image_url)
                            VALUES (@article, @name, @unit, @price, @supplier, @manufacturer,
                                    @category, @discount, @quantity, @description, @imageUrl)";

                        using (var cmd = new NpgsqlCommand(insertQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@article", txtArticle.Text);
                            cmd.Parameters.AddWithValue("@name", txtName.Text);
                            cmd.Parameters.AddWithValue("@unit", txtUnit.Text);
                            cmd.Parameters.AddWithValue("@price", price);
                            cmd.Parameters.AddWithValue("@supplier", string.IsNullOrEmpty(txtSupplier.Text) ? DBNull.Value : (object)txtSupplier.Text);
                            cmd.Parameters.AddWithValue("@manufacturer", string.IsNullOrEmpty(txtManufacturer.Text) ? DBNull.Value : (object)txtManufacturer.Text);
                            cmd.Parameters.AddWithValue("@category", string.IsNullOrEmpty(cmbCategory.Text) ? DBNull.Value : (object)cmbCategory.Text);
                            cmd.Parameters.AddWithValue("@discount", discount);
                            cmd.Parameters.AddWithValue("@quantity", quantity);
                            cmd.Parameters.AddWithValue("@description", string.IsNullOrEmpty(txtDescription.Text) ? DBNull.Value : (object)txtDescription.Text);
                            cmd.Parameters.AddWithValue("@imageUrl", string.IsNullOrEmpty(txtImageUrl.Text) ? DBNull.Value : (object)txtImageUrl.Text);

                            cmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("Товар успешно добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}