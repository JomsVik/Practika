using Npgsql;
using ShoeStore.Models;
using ShoeStore.Services;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ShoeStore.Views
{
    public partial class OrderDetailsWindow : Window
    {
        private DatabaseContext _context;
        private AuthService _authService;
        private int _orderId;

        public OrderDetailsWindow(int orderId, AuthService authService)
        {
            InitializeComponent();
            _context = new DatabaseContext();
            _authService = authService;
            _orderId = orderId;

            LoadOrderDetails();
        }

        private void LoadOrderDetails()
        {
            try
            {
                using (var conn = _context.GetConnection())
                {
                    conn.Open();

                    string orderQuery = @"
                        SELECT order_number, order_date, status, pickup_code
                        FROM orders 
                        WHERE id = @orderId";

                    using (var cmd = new NpgsqlCommand(orderQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@orderId", _orderId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtOrderNumber.Text = reader.GetString(0);
                                txtOrderDate.Text = reader.GetDateTime(1).ToString("dd.MM.yyyy HH:mm");
                                txtStatus.Text = reader.GetString(2);
                                txtPickupCode.Text = reader.IsDBNull(3) ? "" : reader.GetString(3);
                            }
                        }
                    }

                    string itemsQuery = @"
                        SELECT p.name, oi.quantity, oi.price_at_moment
                        FROM order_items oi
                        JOIN products p ON oi.product_id = p.id
                        WHERE oi.order_id = @orderId";

                    var items = new List<OrderItemDisplay>();
                    decimal total = 0;

                    using (var cmd = new NpgsqlCommand(itemsQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@orderId", _orderId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new OrderItemDisplay
                                {
                                    ProductName = reader.GetString(0),
                                    Quantity = reader.GetInt32(1),
                                    Price = reader.GetDecimal(2)
                                };
                                items.Add(item);
                                total += item.Total;
                            }
                        }
                    }

                    listItems.ItemsSource = items;
                    txtTotal.Text = total.ToString("N0") + " ₽";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки деталей заказа: {ex.Message}");
            }
        }

        private class OrderItemDisplay
        {
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public decimal Total => Quantity * Price;
        }
    }
}