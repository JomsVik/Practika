using intern.Data;
using intern.Model;
using intern.Services;
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
    public class MaterialCalculatorViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationDbContext _db;
        private readonly MaterialCalculationService _service;
        private readonly int _productId;

        public ObservableCollection<ProductType> ProductTypes { get; } = new();
        public ObservableCollection<Material> Materials { get; } = new();

        private ProductType? _selectedProductType;
        public ProductType? SelectedProductType
        {
            get => _selectedProductType;
            set { _selectedProductType = value; OnPropertyChanged(); }
        }

     

        private Material? _selectedMaterial;
        public Material? SelectedMaterial
        {
            get => _selectedMaterial;
            set
            {
                _selectedMaterial = value;
                OnPropertyChanged();

                StockQuantity = _selectedMaterial?.StockQuantity ?? 0m;
                Unit = _selectedMaterial?.Unit ?? "";
                OnPropertyChanged(nameof(StockQuantity));
                OnPropertyChanged(nameof(Unit));
            }
        }

        public decimal StockQuantity { get; private set; } 
        public string Unit { get; private set; } = "";


        public int ProductCount { get; set; }
        public double Parameter1 { get; set; }
        public double Parameter2 { get; set; }

        private string _resultText = "";
        public string ResultText
        {
            get => _resultText;
            set { _resultText = value; OnPropertyChanged(); }
        }

        public ICommand CalculateCommand { get; }
        public ICommand CloseCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public MaterialCalculatorViewModel(ApplicationDbContext db, int productId)
        {
            _db = db;
            _service = new MaterialCalculationService(db);
            _productId = productId;

            CalculateCommand = new RelayCommand(_ => Calculate());
            CloseCommand = new RelayCommand(_ => Close(false));

            LoadLookups();
        }

        private async void LoadLookups()
        {
            ProductTypes.Clear();
            foreach (var pt in await _db.ProductTypes.ToListAsync())
                ProductTypes.Add(pt);

            // 1) Сначала подтягиваем продукт и выставляем его тип (чтобы расчёт был "для выбранного товара")
            var product = await _db.Products
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(p => p.Id == _productId);

            if (product != null)
                SelectedProductType = product.ProductType;
            else
                SelectedProductType = ProductTypes.Count > 0 ? ProductTypes[0] : null;

            // 2) Материалы только те, что привязаны к этому продукту через ProductMaterials
            Materials.Clear();

            var materials = await _db.ProductMaterials
                .Where(pm => pm.ProductId == _productId)
                .Include(pm => pm.Material)
                    .ThenInclude(m => m.MaterialType) // чтобы service мог брать DefectPercent из MaterialType
                .Select(pm => pm.Material)
                .Distinct()
                .ToListAsync();

            foreach (var m in materials)
                Materials.Add(m);

            SelectedMaterial = Materials.Count > 0 ? Materials[0] : null;

        }

        private void Calculate()
        {
            if (SelectedProductType == null || SelectedMaterial == null)
            {
                MessageBox.Show("Не выбран тип продукта или материал");
                return;
            }

            int result = _service.CalculateRequiredMaterialForMaterial(
                SelectedProductType.Id,
                SelectedMaterial.Id,
                ProductCount,
                Parameter1,
                Parameter2);

            ResultText = result switch
            {
                < 0 => "Ошибка: все параметры должны быть > 0.",
                0 => "Материала достаточно, докупать не нужно.",
                _ => $"Нужно докупить: {result} {Unit}"
            };
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
