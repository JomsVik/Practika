using intern.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace intern.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<MaterialType> MaterialTypes => Set<MaterialType>();
        public DbSet<ProductType> ProductTypes => Set<ProductType>();
        public DbSet<Material> Materials => Set<Material>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductMaterial> ProductMaterials => Set<ProductMaterial>();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<MaterialType>(entity =>
            {
                entity.ToTable("material_type");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name)
                      .HasColumnName("name")
                      .HasMaxLength(255)
                      .IsRequired();
                entity.Property(e => e.DefectPercent)
                      .HasColumnName("defect_percent");
            });


            modelBuilder.Entity<ProductType>(entity =>
            {
                entity.ToTable("product_type");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name)
                      .HasColumnName("name")
                      .HasMaxLength(255)
                      .IsRequired();
                entity.Property(e => e.Coefficient)
                      .HasColumnName("coefficient");
            });


            modelBuilder.Entity<Material>(entity =>
            {
                entity.ToTable("material");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name)
                      .HasColumnName("name")
                      .HasMaxLength(255)
                      .IsRequired();
                entity.Property(e => e.MaterialTypeId)
                      .HasColumnName("material_type_id");
                entity.Property(e => e.Cost)
                      .HasColumnName("cost")
                      .HasColumnType("decimal(18,2)");
                entity.Property(e => e.StockQuantity)
                      .HasColumnName("stock_quantity")
                      .HasColumnType("decimal(18,2)");
                entity.Property(e => e.MinStock)
                      .HasColumnName("min_stock")
                      .HasColumnType("decimal(18,2)");
                entity.Property(e => e.PackageQuantity)
                      .HasColumnName("package_quantity")
                      .HasColumnType("decimal(18,2)");
                entity.Property(e => e.Unit)
                      .HasColumnName("unit")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.HasOne(e => e.MaterialType)
                      .WithMany(mt => mt.Materials)
                      .HasForeignKey(e => e.MaterialTypeId)
                      .HasConstraintName("FK_material_material_type");
            });


            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("product");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ProductTypeId)
                      .HasColumnName("product_type_id");
                entity.Property(e => e.Name)
                      .HasColumnName("name")
                      .HasMaxLength(255)
                      .IsRequired();
                entity.Property(e => e.Article)
                      .HasColumnName("article")
                      .HasMaxLength(50)
                      .IsRequired();
                entity.Property(e => e.MinPartnerPrice)
                      .HasColumnName("min_partner_price")
                      .HasColumnType("decimal(18,2)");
                entity.Property(e => e.RollWidth)
                      .HasColumnName("roll_width")
                      .HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.ProductType)
                      .WithMany(pt => pt.Products)
                      .HasForeignKey(e => e.ProductTypeId)
                      .HasConstraintName("FK_product_product_type");
            });


            modelBuilder.Entity<ProductMaterial>(entity =>
            {
                entity.ToTable("product_material");

                entity.HasKey(e => new { e.ProductId, e.MaterialId })
                      .HasName("PK_product_material");

                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.MaterialId).HasColumnName("material_id");
                entity.Property(e => e.QuantityPerProduct)
                      .HasColumnName("quantity_per_product");

                entity.HasOne(pm => pm.Product)
                      .WithMany(p => p.ProductMaterials)
                      .HasForeignKey(pm => pm.ProductId)
                      .HasConstraintName("FK_product_material_product");

                entity.HasOne(pm => pm.Material)
                      .WithMany(m => m.ProductMaterials)
                      .HasForeignKey(pm => pm.MaterialId)
                      .HasConstraintName("FK_product_material_material");
            });
        }
    }
}
