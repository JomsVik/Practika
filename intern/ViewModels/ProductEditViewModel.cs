using intern.Data;
using intern.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace intern.ViewModels
{
    public class ProductEditViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationDbContext _db;
        private readonly int? _productId;

        public ObservableCollection<ProductType> ProductTypes { get; } = new();

        private ProductType? _selectedProductType;
        public ProductType? SelectedProductType
        {
            get => _selectedProductType;
            set { _selectedProductType = value; OnPropertyChanged(); }
        }

        public string Article { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal MinPartnerPrice { get; set; }
        public decimal RollWidth { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ProductEditViewModel(ApplicationDbContext db, int? productId)
        {
            _db = db;
            _productId = productId;

            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => Close(false));
            

            LoadProductTypes();
            if (_productId.HasValue)
                LoadProduct(_productId.Value);
        }

        private async void LoadProductTypes()
        {
            var types = await _db.ProductTypes.ToListAsync();
            ProductTypes.Clear();
            foreach (var t in types)
                ProductTypes.Add(t);
        }

        private async void LoadProduct(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return;

            Article = product.Article;
            Name = product.Name;
            MinPartnerPrice = product.MinPartnerPrice;
            RollWidth = product.RollWidth;
            SelectedProductType = await _db.ProductTypes.FindAsync(product.ProductTypeId);

            OnPropertyChanged(nameof(Article));
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(MinPartnerPrice));
            OnPropertyChanged(nameof(RollWidth));
            OnPropertyChanged(nameof(SelectedProductType));
        }

        private async void Save()
        {
            if (SelectedProductType == null ||
                string.IsNullOrWhiteSpace(Article) ||
                string.IsNullOrWhiteSpace(Name) ||
                MinPartnerPrice <= 0 || RollWidth <= 0)
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            MinPartnerPrice = decimal.Round(MinPartnerPrice, 2);
            RollWidth = decimal.Round(RollWidth, 2);

            Product entity;

            if (_productId.HasValue)
            {
                entity = await _db.Products.FindAsync(_productId.Value) ?? new Product();
            }
            else
            {
                entity = new Product();
                _db.Products.Add(entity);
            }

            entity.Article = Article;
            entity.Name = Name;
            entity.MinPartnerPrice = MinPartnerPrice;
            entity.RollWidth = RollWidth;

            
            entity.ProductTypeId = SelectedProductType.Id;

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