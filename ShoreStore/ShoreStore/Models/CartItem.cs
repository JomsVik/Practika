namespace ShoeStore.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string Article { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public int Quantity { get; set; }
        public string ImagePath { get; set; }

        public decimal PriceWithDiscount => Price * (1 - Discount / 100m);
        public decimal TotalPrice => PriceWithDiscount * Quantity;
    }
}