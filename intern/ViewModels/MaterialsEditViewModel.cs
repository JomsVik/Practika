using intern.Data;
using intern.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

namespace intern.ViewModels
{
    public class MaterialEditViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationDbContext _db;
        private readonly int _materialId;
        private readonly int _productId;

        public ObservableCollection<MaterialType> MaterialTypes { get; } = new();

        private MaterialType? _selectedMaterialType;
        public MaterialType? SelectedMaterialType
        {
            get => _selectedMaterialType;
            set { _selectedMaterialType = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> Units { get; } = new()
        {
            "шт", "м", "м2", "л", "кг", "упак"
        };

        private string? _selectedUnit;
        public string? SelectedUnit
        {
            get => _selectedUnit;
            set { _selectedUnit = value; OnPropertyChanged(); }
        }

        public string Name { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public decimal StockQuantity { get; set; }
        public decimal MinStock { get; set; }
        public decimal PackageQuantity { get; set; }
        public decimal QuantityPerProduct { get; set; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public MaterialEditViewModel(ApplicationDbContext db, int materialId, int productId)
        {
            _db = db;
            _materialId = materialId;
            _productId = productId;

            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => Close(false));

            SelectedUnit = Units.Count > 0 ? Units[0] : null;

            LoadMaterialTypes();
            LoadData(productId, materialId);
        }

        private async void LoadMaterialTypes()
        {
            var types = await _db.MaterialTypes.ToListAsync();
            MaterialTypes.Clear();
            foreach (var t in types)
                MaterialTypes.Add(t);
        }

        private async void LoadData(int productId, int materialId)
        {
            var material = await _db.Materials.FindAsync(materialId);
            if (material == null) return;

            Name = material.Name;
            Cost = material.Cost;
            StockQuantity = material.StockQuantity;
            MinStock = material.MinStock;
            PackageQuantity = material.PackageQuantity;

            if (!string.IsNullOrWhiteSpace(material.Unit))
            {
                if (!Units.Contains(material.Unit))
                    Units.Add(material.Unit);
                SelectedUnit = material.Unit;
            }

            SelectedMaterialType = await _db.MaterialTypes.FindAsync(material.MaterialTypeId);

            var pm = await _db.ProductMaterials.FindAsync(productId, materialId);
            if (pm != null)
                QuantityPerProduct = (decimal)pm.QuantityPerProduct;
            else
                QuantityPerProduct = 1;

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Cost));
            OnPropertyChanged(nameof(StockQuantity));
            OnPropertyChanged(nameof(MinStock));
            OnPropertyChanged(nameof(PackageQuantity));
            OnPropertyChanged(nameof(SelectedUnit));
            OnPropertyChanged(nameof(SelectedMaterialType));
            OnPropertyChanged(nameof(QuantityPerProduct));
        }

        private async void Save()
        {
            if (SelectedMaterialType == null ||
                string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(SelectedUnit) ||
                Cost <= 0 ||
                StockQuantity < 0 ||
                MinStock < 0 ||
                PackageQuantity <= 0 ||
                QuantityPerProduct <= 0)
            {
                MessageBox.Show("Заполните все поля корректно");
                return;
            }

            Cost = decimal.Round(Cost, 2);
            StockQuantity = decimal.Round(StockQuantity, 2);
            MinStock = decimal.Round(MinStock, 2);
            PackageQuantity = decimal.Round(PackageQuantity, 2);
            QuantityPerProduct = decimal.Round(QuantityPerProduct, 3);

            var material = await _db.Materials.FindAsync(_materialId);
            if (material == null) return;

            material.Name = Name.Trim();
            material.MaterialTypeId = SelectedMaterialType.Id;
            material.Unit = SelectedUnit!;
            material.Cost = Cost;
            material.StockQuantity = StockQuantity;
            material.MinStock = MinStock;
            material.PackageQuantity = PackageQuantity;

            var pm = await _db.ProductMaterials.FindAsync(_productId, _materialId);
            if (pm == null)
            {
                pm = new ProductMaterial
                {
                    ProductId = _productId,
                    MaterialId = _materialId,
                    QuantityPerProduct = (double)QuantityPerProduct
                };
                _db.ProductMaterials.Add(pm);
            }
            else
            {
                pm.QuantityPerProduct = (double)QuantityPerProduct;
            }

            await _db.SaveChangesAsync();
            Close(true);
        }

        private void Close(bool dialogResult)
        {
            if (Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this) is Window window)
            {
                window.DialogResult = dialogResult;
                window.Close();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
