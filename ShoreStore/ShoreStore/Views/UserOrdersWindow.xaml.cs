using Npgsql;
using ShoeStore.Models;
using ShoeStore.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ShoeStore.Views
{
    public partial class UserOrdersWindow : Window
    {
        private DatabaseContext _context;
        private AuthService _authService;

        public UserOrdersWindow(AuthService authService)
        {
            InitializeComponent();
            _context = new DatabaseContext();
            _authService = authService;

            LoadUserOrders();
        }

        private void LoadUserOrders()
        {
            try
            {
                var orders = new List<Order>();

                using (var conn = _context.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        SELECT id, order_number, order_date, status, pickup_code
                        FROM orders 
                        WHERE user_id = @userId 
                        ORDER BY order_date DESC";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", _authService.CurrentUser.Id);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                orders.Add(new Order
                                {
                                    Id = reader.GetInt32(0),
                                    OrderNumber = reader.GetString(1),
                                    OrderDate = reader.GetDateTime(2),
                                    Status = reader.GetString(3),
                                    PickupCode = reader.IsDBNull(4) ? "" : reader.GetString(4)
                                });
                            }
                        }
                    }

                    foreach (var order in orders)
                    {
                        string itemsQuery = @"
                            SELECT oi.quantity, oi.price_at_moment
                            FROM order_items oi
                            WHERE oi.order_id = @orderId";

                        using (var cmd = new NpgsqlCommand(itemsQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@orderId", order.Id);

                            using (var reader = cmd.ExecuteReader())
                            {
                                decimal total = 0;
                                while (reader.Read())
                                {
                                    total += reader.GetInt32(0) * reader.GetDecimal(1);
                                }
                                order.TotalAmount = total;
                            }
                        }
                    }
                }

                listOrders.ItemsSource = orders;

                if (orders.Count == 0)
                {
                    MessageBox.Show("У вас пока нет заказов", "Информация",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}");
            }
        }

        private void ViewOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            int orderId = (int)button.Tag;

            var detailsWindow = new OrderDetailsWindow(orderId, _authService);
            detailsWindow.Owner = this;
            detailsWindow.ShowDialog();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}