using System;
using System.Collections.Generic;
using System.Text;

namespace intern.Model
{
    public class ProductType
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public double Coefficient { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
