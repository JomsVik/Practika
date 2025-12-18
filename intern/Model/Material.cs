using System;
using System.Collections.Generic;
using System.Text;

namespace intern.Model
{
    public class Material
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int MaterialTypeId { get; set; }

        public decimal Cost { get; set; }

        public decimal StockQuantity { get; set; }

        public decimal MinStock { get; set; }

        public decimal PackageQuantity { get; set; }

        public string Unit { get; set; } = null!;

        public MaterialType MaterialType { get; set; } = null!;

        public ICollection<ProductMaterial> ProductMaterials { get; set; } = new List<ProductMaterial>();
    }
}
