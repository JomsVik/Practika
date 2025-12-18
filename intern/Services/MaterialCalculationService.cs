using intern.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace intern.Services
{
    public class MaterialCalculationService
    {
        private readonly ApplicationDbContext _db;
        public MaterialCalculationService(ApplicationDbContext db) => _db = db;

        public int CalculateRequiredMaterial(
            int productTypeId,
            int materialTypeId,
            int productCount,
            double parameter1,
            double parameter2,
            double stockQuantity)
        {
            if (productCount <= 0 || parameter1 <= 0 || parameter2 <= 0 || stockQuantity < 0)
                return -1;

            var productType = _db.ProductTypes.Find(productTypeId);
            var materialType = _db.MaterialTypes.Find(materialTypeId);
            if (productType == null || materialType == null)
                return -1;

            double defectPercent = materialType.DefectPercent;
            if (defectPercent < 0 || defectPercent > 1)
                return -1;

            double perProduct = parameter1 * parameter2 * productType.Coefficient;
            if (perProduct <= 0) return -1;

            double total = perProduct * productCount;
            total *= (1.0 + defectPercent);

            double needToBuy = total - stockQuantity;
            if (needToBuy <= 0) return 0;

            return (int)Math.Ceiling(needToBuy);
        }

        public int CalculateRequiredMaterialForMaterial(
            int productTypeId,
            int materialId,
            int productCount,
            double parameter1,
            double parameter2)
        {
            var material = _db.Materials
                .Include(m => m.MaterialType)
                .FirstOrDefault(m => m.Id == materialId);

            if (material == null)
                return -1;

            return CalculateRequiredMaterial(
                productTypeId,
                material.MaterialTypeId,
                productCount,
                parameter1,
                parameter2,
                (double)material.StockQuantity);
        }
    }
}
