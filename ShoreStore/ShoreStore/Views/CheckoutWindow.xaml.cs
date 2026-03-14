using Npgsql;
using ShoeStore.Models;
using ShoeStore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ShoeStore.Views
{
    public partial class CheckoutWindow : Window
    {
        private CartService _cartService;
        private AuthService _authService;
        private DatabaseContext _context;
        private List<PickupPoint> _pickupPoints;

        public CheckoutWindow(CartService cartService, AuthService authService)
        {
            InitializeComponent();
            _cartService = cartService;
            _authService = authService;
            _context = new DatabaseContext();

            LoadData();
        }

        private void LoadData()
        {
            if (_authService.CurrentUser != null)
            {
                txtFullName.Text = _authService.CurrentUser.FullName;
            }

            LoadPickupPoints();
            RefreshCart();
        }

        private void LoadPickupPoints()
        {
            try
            {
                _pickupPoints = new List<PickupPoint>();

                using (var conn = _context.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT id, address FROM pickup_points ORDER BY address";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _pickupPoints.Add(new PickupPoint
                            {
                                Id = reader.GetInt32(0),
                                Address = reader.GetString(1)
                            });
                        }
                    }
                }

                cmbPickupPoints.ItemsSource = _pickupPoints;
                if (_pickupPoints.Count > 0)
                {
                    cmbPickupPoints.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пунктов выдачи: {ex.Message}");
            }
        }

        private void RefreshCart()
        {
            listOrderItems.ItemsSource = null;
            listOrderItems.ItemsSource = _cartService.Items;
            txtTotal.Text = _cartService.TotalAmount.ToString("N0") + " ₽";
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtStatus.Text = "";

                if (cmbPickupPoints.SelectedItem == null)
                {
                    txtStatus.Text = "Выберите пункт выдачи";
                    return;
                }

                if (_cartService.TotalItems == 0)
                {
                    txtStatus.Text = "Корзина пуста";
                    return;
                }

                using (var conn = _context.GetConnection())
                {
                    conn.Open();

                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string orderNumber = "ORD-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
                            int pickupPointId = ((PickupPoint)cmbPickupPoints.SelectedItem).Id;

                            string insertOrderQuery = @"
                                INSERT INTO orders (order_number, user_id, order_date, 
                                                   pickup_point_id, status, pickup_code)
                                VALUES (@orderNumber, @userId, @orderDate, 
                                       @pickupPointId, @status, @pickupCode)
                                RETURNING id";

                            int orderId;
                            using (var orderCmd = new NpgsqlCommand(insertOrderQuery, conn))
                            {
                                orderCmd.Parameters.AddWithValue("@orderNumber", orderNumber);
                                orderCmd.Parameters.AddWithValue("@userId", _authService.CurrentUser.Id);
                                orderCmd.Parameters.AddWithValue("@orderDate", DateTime.Now);
                                orderCmd.Parameters.AddWithValue("@pickupPointId", pickupPointId);
                                orderCmd.Parameters.AddWithValue("@status", "Новый");
                                orderCmd.Parameters.AddWithValue("@pickupCode", new Random().Next(100, 999).ToString());

                                orderId = Convert.ToInt32(orderCmd.ExecuteScalar());
                            }

                            foreach (var item in _cartService.Items)
                            {
                                string insertItemQuery = @"
                                    INSERT INTO order_items (order_id, product_id, quantity, price_at_moment)
                                    VALUES (@orderId, @productId, @quantity, @price)";

                                using (var itemCmd = new NpgsqlCommand(insertItemQuery, conn))
                                {
                                    itemCmd.Parameters.AddWithValue("@orderId", orderId);
                                    itemCmd.Parameters.AddWithValue("@productId", item.ProductId);
                                    itemCmd.Parameters.AddWithValue("@quantity", item.Quantity);
                                    itemCmd.Parameters.AddWithValue("@price", item.PriceWithDiscount);

                                    itemCmd.ExecuteNonQuery();
                                }

                                string updateStockQuery = @"
                                    UPDATE products 
                                    SET quantity = quantity - @quantity 
                                    WHERE id = @productId AND quantity >= @quantity";

                                using (var stockCmd = new NpgsqlCommand(updateStockQuery, conn))
                                {
                                    stockCmd.Parameters.AddWithValue("@quantity", item.Quantity);
                                    stockCmd.Parameters.AddWithValue("@productId", item.ProductId);

                                    int updated = stockCmd.ExecuteNonQuery();
                                    if (updated == 0)
                                    {
                                        throw new Exception($"Недостаточно товара {item.Name} на складе");
                                    }
                                }
                            }

                            transaction.Commit();

                            MessageBox.Show($"Заказ №{orderNumber} успешно оформлен!\nКод получения: {orderNumber.Split('-').Last()}",
                                          "Заказ оформлен", MessageBoxButton.OK, MessageBoxImage.Information);

                            this.DialogResult = true;
                            this.Close();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            txtStatus.Text = $"Ошибка: {ex.Message}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                txtStatus.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}