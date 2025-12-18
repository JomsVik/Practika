using intern.Data;
using intern.Model;
using intern.Services;
using intern.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace intern.ViewModels
{
    public class ProductListViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationDbContext _db;
        private readonly ProductCostService _costService;

        public ObservableCollection<ProductRowViewModel> Products { get; } = new();

        private string? _statusMessage;
        public string? StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        private ProductRowViewModel? _selectedProduct;
        public ProductRowViewModel? SelectedProduct
        {
            get => _selectedProduct;
            set { _selectedProduct = value; OnPropertyChanged(); }
        }

        public ICommand AddProductCommand { get; }
        public ICommand EditProductCommand { get; }
        public ICommand OpenMaterialsCommand { get; }
        public ICommand OpenMaterialCalculatorCommand { get;  }
        public ProductListViewModel(ApplicationDbContext db)
        {
            _db = db;
            _costService = new ProductCostService(db);
            AddProductCommand = new RelayCommand(_ => AddProduct());
            EditProductCommand = new RelayCommand(
                _ => EditProduct(),
                _ => SelectedProduct != null);

            OpenMaterialsCommand = new RelayCommand(
                _ => OpenMaterials(),
                _ => SelectedProduct != null);

            OpenMaterialCalculatorCommand = new RelayCommand(
                _ => OpenMaterialCalculator(),
                _ => SelectedProduct != null);

            LoadProducts();
        }

        private void AddProduct()
        {
            var vm = new ProductEditViewModel(_db, null);
            var window = new ProductEditWindow { DataContext = vm };

            if (window.ShowDialog() == true)
                LoadProducts();
        }

        private void EditProduct()
        {
            if (SelectedProduct == null) return;

            var vm = new ProductEditViewModel(_db, SelectedProduct.Id);
            var window = new ProductEditWindow { DataContext = vm };

            if (window.ShowDialog() == true)
                LoadProducts();
        }

        private void OpenMaterials() {
            if (SelectedProduct == null) return;
            Func<Task> onChanged = () => RefreshProductCostAsync(SelectedProduct.Id);

            var vm = new ProductMaterialsViewModel(_db, SelectedProduct.Id, SelectedProduct.Name, onChanged);
            var window = new MaterialsWindow { DataContext = vm };

            window.ShowDialog();
        }

        private void OpenMaterialCalculator()
        {
            if (SelectedProduct == null) return;

            var vm = new MaterialCalculatorViewModel(_db, SelectedProduct.Id);
            var window = new MaterialCalculatorWindow { DataContext = vm };
            window.ShowDialog();
        }

        private async void LoadProducts()
        {
            try
            {
                Products.Clear();
                var products = await _db.Products
                    .Include(p => p.ProductType)
                    .ToListAsync();

                foreach (var p in products) {
                    var cost = await _costService.CalculateProductCostAsync(p.Id);
                    Products.Add(new ProductRowViewModel
                    {
                        Id = p.Id,
                        Article = p.Article,
                        ProductTypeName = p.ProductType.Name,
                        Name = p.Name,
                        MinPartnerPrice = p.MinPartnerPrice,
                        RollWidth = p.RollWidth,
                        CalculatedCost = cost
                    });
                }
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
        }

        private async Task RefreshProductCostAsync(int productId)
        {
            try
            {
                var cost = await _costService.CalculateProductCostAsync(productId);

                var row = Products.FirstOrDefault(p => p.Id == productId);
                if (row != null)
                    row.CalculatedCost = cost;
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public class ProductRowViewModel: INotifyPropertyChanged
        {
            public int Id { get; set; }
            public string Article { get; set; } = "";
            public string ProductTypeName { get; set; } = "";
            public string Name { get; set; } = "";
            public decimal MinPartnerPrice { get; set; }
            public decimal RollWidth { get; set; }
            private decimal _calculatedCost;
            public decimal CalculatedCost { 
                get => _calculatedCost; 
                set { _calculatedCost = value; OnPropertyChanged(); }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
            private void OnPropertyChanged([CallerMemberName] string? name = null) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
