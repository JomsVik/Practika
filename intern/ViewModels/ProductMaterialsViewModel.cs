using intern.Data;
using intern.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace intern.ViewModels
{
    public class ProductMaterialsViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationDbContext _db;
        private readonly int _productId;
        private readonly Func<Task>? _onMaterialsChanged;
        public string Header { get; }
        public ObservableCollection<ProductMaterialRow> Materials { get; } = new();

        private ProductMaterialRow? _selectedMaterial;
        public ProductMaterialRow? SelectedMaterial
        {
            get => _selectedMaterial;
            set { _selectedMaterial = value; OnPropertyChanged(); }
        }

        public ICommand EditMaterialCommand { get; }

        public ProductMaterialsViewModel(ApplicationDbContext db, int productId, string productName, Func<Task>? onMaterialsChanged = null)
        {
            _db = db;
            _productId = productId;
            _onMaterialsChanged = onMaterialsChanged;

            Header = $"Материалы для: {productName}";

            EditMaterialCommand = new RelayCommand(async _ => await EditMaterial(),
                                              _ => SelectedMaterial != null);

        _ = LoadMaterialsAsync();
        }

        private async Task LoadMaterialsAsync()
        {
            Materials.Clear();

            var rows = await _db.ProductMaterials
                .Include(pm => pm.Material)
                    .ThenInclude(m => m.MaterialType)
                .AsNoTracking()
                .ToListAsync();

            foreach (var pm in rows)
            {
                if (pm.ProductId != _productId)
                    continue;

                Materials.Add(new ProductMaterialRow
                {
                    ProductId = pm.ProductId,
                    MaterialId = pm.MaterialId,
                    MaterialName = pm.Material.Name,
                    MaterialTypeName = pm.Material.MaterialType.Name,
                    QuantityPerProduct = (decimal)pm.QuantityPerProduct,
                    Unit = pm.Material.Unit
                });
            }
        }

        private async Task EditMaterial()
        {
            if (SelectedMaterial == null)
                return;

            var vm = new MaterialEditViewModel(_db, SelectedMaterial.MaterialId, SelectedMaterial.ProductId);
            var window = new MaterialsEditWindow { DataContext = vm };

            if (window.ShowDialog() == true) {
                await LoadMaterialsAsync();
                if (_onMaterialsChanged != null) {
                    await _onMaterialsChanged();
                }
            }
        }

        public class ProductMaterialRow
        {
            public int ProductId { get; set; }
            public int MaterialId { get; set; }
            public string MaterialName { get; set; } = "";
            public string MaterialTypeName { get; set; } = "";
            public decimal QuantityPerProduct { get; set; }
            public string Unit { get; set; } = "";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
