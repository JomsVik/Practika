using intern.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace intern.Services
{
    public class ProductCostService
    {
        private readonly ApplicationDbContext _db;

        public ProductCostService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<decimal> CalculateProductCostAsync(int productId)
        {
            var product = await _db.Products
                .Include(p => p.ProductMaterials)
                    .ThenInclude(pm => pm.Material)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                throw new ArgumentException("Product not found", nameof(productId));

            decimal cost = 0m;

            foreach (var pm in product.ProductMaterials)
            {
                var quantity = (decimal)pm.QuantityPerProduct;
                cost += quantity * pm.Material.Cost;
            }

            cost = Math.Round(cost, 2, MidpointRounding.AwayFromZero);
            if (cost < 0) cost = 0;

            return cost;
        }
    }
}

