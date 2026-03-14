using ShoeStore.Models;
using System.Collections.Generic;
using System.Linq;

namespace ShoeStore.Services
{
    public class CartService
    {
        private List<CartItem> _items = new List<CartItem>();

        public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

        public void AddToCart(Product product, int quantity = 1)
        {
            var existingItem = _items.FirstOrDefault(x => x.ProductId == product.Id);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                _items.Add(new CartItem
                {
                    ProductId = product.Id,
                    Article = product.Article,
                    Name = product.Name,
                    Price = product.Price,
                    Discount = product.Discount,
                    Quantity = quantity,
                    ImagePath = product.ImagePath
                });
            }
        }

        public void RemoveFromCart(int productId)
        {
            var item = _items.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                _items.Remove(item);
            }
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var item = _items.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    _items.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
            }
        }

        public void ClearCart()
        {
            _items.Clear();
        }

        public decimal TotalAmount => _items.Sum(x => x.TotalPrice);

        public int TotalItems => _items.Sum(x => x.Quantity);
    }
}