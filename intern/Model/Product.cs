using System;
using System.Collections.Generic;
using System.Text;

namespace intern.Model
{
    public class Product
    {
        public int Id { get; set; }

        public int ProductTypeId { get; set; }

        public string Name { get; set; } = null!;

        public string Article { get; set; } = null!;

        public decimal MinPartnerPrice { get; set; }

        public decimal RollWidth { get; set; }

        public ProductType ProductType { get; set; } = null!;

        public ICollection<ProductMaterial> ProductMaterials { get; set; } = new List<ProductMaterial>();
    }
}
