using System;
using System.Collections.Generic;
using System.Text;

namespace intern.Model
{
    public class ProductMaterial
    {
        public int ProductId { get; set; }

        public int MaterialId { get; set; }

        public double QuantityPerProduct { get; set; }

        public Product Product { get; set; } = null!;

        public Material Material { get; set; } = null!;
    }
}
