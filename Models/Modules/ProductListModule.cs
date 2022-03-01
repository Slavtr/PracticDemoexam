using PracticDemoexam.Models.PartialModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PracticDemoexam.Models.Modules
{
    public class ProductListModule : INotifyPropertyChanged
    {
        public ProductListModule(IEnumerable<DataModel.Product> products, IEnumerable<DataModel.ProductType> productTypes)
        {
            LoadProducts(products);
            LoadProductTypes(productTypes);
            LoadCommands();
        }

        #region LoadThings

        private bool LoadProducts(IEnumerable<DataModel.Product> products)
        {
            try
            {
                foreach (DataModel.Product p in products)
                {
                    Products.Add(new ProductVM(p));
                    UnchangedProducts.Add(new ProductVM(p));
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        private bool LoadProductTypes(IEnumerable<DataModel.ProductType> productTypes)
        {
            try
            {
                foreach(DataModel.ProductType pt in productTypes)
                {
                    ProductTypes.Add(pt);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        private bool LoadCommands()
        {
            try
            {
                ClearFilterCommand = new RoutedCommand("ClearFilter", GetType());
                ClearFilterCommandBinding = new CommandBinding(ClearFilterCommand, ClearFilter, CanExecuteClearFilter);
                CommandBindingCollection.Add(ClearFilterCommandBinding);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        public ObservableCollection<ProductVM> Products { get; set; } = new ObservableCollection<ProductVM>();
        private readonly ObservableCollection<ProductVM> UnchangedProducts = new ObservableCollection<ProductVM>();

        public CommandBindingCollection CommandBindingCollection { get; set; } = new CommandBindingCollection();

        #region SearchSortFilter

        private string _searchString;
        public string SearchString
        {
            get { return _searchString; }
            set
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                {
                    Products.Clear();
                    foreach(ProductVM pvm in UnchangedProducts)
                    {
                        Products.Add(pvm);
                    }
                }
                else
                {
                    var productVMs = Products.Where(x => x.Product.Title.Contains(value)).ToList();
                    Products.Clear();
                    foreach (ProductVM pvm in productVMs)
                    {
                        Products.Add(pvm);
                    }
                }
                OnPropertyChanged("Products");
                OnPropertyChanged("SearchString");
                _searchString = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string[] SortVariants { get; } = { "Сброс", "Наименование", "Номер цеха", "Мин. стоимость" };
        private string _curSort;
        public string CurrentSort
        {
            get { return _curSort; }
            set
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                {
                    Products.Clear();
                    foreach (ProductVM pvm in UnchangedProducts)
                    {
                        Products.Add(pvm);
                    }
                }
                else
                {
                    switch (value)
                    {
                        case "Сброс":
                            SearchString = SearchString;
                            break;
                        case "Наименование":
                            if (_curSort == value)
                            {
                                var productVMs = Products.OrderByDescending(x => x.Product.Title).ToList();
                                Products.Clear();
                                foreach (ProductVM pvm in productVMs)
                                {
                                    Products.Add(pvm);
                                }
                            }
                            else
                            {
                                var productVMs = Products.OrderBy(x => x.Product.Title).ToList();
                                Products.Clear();
                                foreach (ProductVM pvm in productVMs)
                                {
                                    Products.Add(pvm);
                                }
                            }
                            break;
                        case "Номер цеха":
                            if (_curSort == value)
                            {
                                var productVMs = Products.OrderByDescending(x => x.Product.ProductionWorkshopNumber).ToList();
                                Products.Clear();
                                foreach (ProductVM pvm in productVMs)
                                {
                                    Products.Add(pvm);
                                }
                            }
                            else
                            {
                                var productVMs = Products.OrderBy(x => x.Product.ProductionWorkshopNumber).ToList();
                                Products.Clear();
                                foreach (ProductVM pvm in productVMs)
                                {
                                    Products.Add(pvm);
                                }
                            }
                            break;
                        case "Мин. стоимость":
                            if (_curSort == value)
                            {
                                var productVMs = Products.OrderByDescending(x => x.Product.MinCostForAgent).ToList();
                                Products.Clear();
                                foreach (ProductVM pvm in productVMs)
                                {
                                    Products.Add(pvm);
                                }
                            }
                            else
                            {
                                var productVMs = Products.OrderBy(x => x.Product.MinCostForAgent).ToList();
                                Products.Clear();
                                foreach (ProductVM pvm in productVMs)
                                {
                                    Products.Add(pvm);
                                }
                            }
                            break;
                    }
                }
                _curSort = value;
                OnPropertyChanged("CurrentSort");
                OnPropertyChanged("Products"); 
            }
        }

        public ObservableCollection<DataModel.ProductType> ProductTypes { get; set; } = new ObservableCollection<DataModel.ProductType>();
        private DataModel.ProductType _currType;
        public DataModel.ProductType CurrentType
        {
            get { return _currType; }
            set
            {
                if (value != null)
                {
                    _currType = value;
                    var productVMs = UnchangedProducts.Where(x => x.Product.ProductTypeID == _currType.ID).ToList();
                    Products.Clear();
                    foreach(ProductVM pvm in productVMs)
                    {
                        Products.Add(pvm);
                    }
                    OnPropertyChanged("CurrentType");
                    OnPropertyChanged("Products");
                }
            }
        }

        public RoutedCommand ClearFilterCommand { get; private set; }
        public CommandBinding ClearFilterCommandBinding { get; private set; }

        private void ClearFilter(object sender, ExecutedRoutedEventArgs e)
        {
            _currType = null;
            Products.Clear();
            foreach (ProductVM pvm in UnchangedProducts)
            {
                Products.Add(pvm);
            }
            OnPropertyChanged("CurrentType");
            OnPropertyChanged("Products");
        }
        private void CanExecuteClearFilter(object sender, CanExecuteRoutedEventArgs e)
        {
            if(_currType != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        #endregion

    }
}
