using System;
using System.Collections.Generic;
using System.Text;

namespace intern.Model
{
    public class MaterialType
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public double DefectPercent { get; set; }

        public ICollection<Material> Materials { get; set; } = new List<Material>();
    }
}
