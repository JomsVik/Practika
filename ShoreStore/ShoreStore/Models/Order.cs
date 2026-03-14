using System;
using System.Collections.Generic;

namespace ShoeStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int? PickupPointId { get; set; }
        public string PickupCode { get; set; }
        public string Status { get; set; }

        public User User { get; set; }
        public PickupPoint PickupPoint { get; set; }
        public List<OrderItem> OrderItems { get; set; }

        public decimal TotalAmount { get; set; }
    }
}