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
                    UnchangedProducts.Add(new ProductVM(p));
                }
                WholeProducts = UnchangedProducts;
                OnPropertyChanged("Products");
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
                foreach (DataModel.ProductType pt in productTypes)
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
        public bool LoadCommands()
        {
            ClearFilterCommand = new RoutedCommand("ClearFilter", GetType());
            ClearFilterCommandBinding = new CommandBinding(ClearFilterCommand, ClearFilter, CanExecuteClearFilter);
            CommandBindingCollection.Add(ClearFilterCommandBinding);
            return true;
        }

        #endregion

        public CommandBindingCollection CommandBindingCollection { get; set; } = new CommandBindingCollection();

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region SearchSortFilter

        private string _searchString;
        public string SearchString
        {
            get { return _searchString; }
            set
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                {
                    WholeProducts = UnchangedProducts;
                }
                else
                {
                    var productVMs = Products.Where(x => x.Product.Title.Contains(value)).ToList();
                    WholeProducts = new BindingList<ProductVM>(productVMs);
                }
                OnPropertyChanged("Products");
                OnPropertyChanged("SearchString");
                _searchString = value;
            }
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
                                WholeProducts = new BindingList<ProductVM>(productVMs);
                            }
                            else
                            {
                                var productVMs = Products.OrderBy(x => x.Product.Title).ToList();
                                WholeProducts = new BindingList<ProductVM>(productVMs);
                            }
                            break;
                        case "Номер цеха":
                            if (_curSort == value)
                            {
                                var productVMs = Products.OrderByDescending(x => x.Product.ProductionWorkshopNumber).ToList();
                                WholeProducts = new BindingList<ProductVM>(productVMs);
                            }
                            else
                            {
                                var productVMs = Products.OrderBy(x => x.Product.ProductionWorkshopNumber).ToList();
                                WholeProducts = new BindingList<ProductVM>(productVMs);
                            }
                            break;
                        case "Мин. стоимость":
                            if (_curSort == value)
                            {
                                var productVMs = Products.OrderByDescending(x => x.Product.MinCostForAgent).ToList();
                                WholeProducts = new BindingList<ProductVM>(productVMs);
                            }
                            else
                            {
                                var productVMs = Products.OrderBy(x => x.Product.MinCostForAgent).ToList();
                                WholeProducts = new BindingList<ProductVM>(productVMs);
                            }
                            break;
                    }
                }
                _curSort = value;
                OnPropertyChanged("CurrentSort");
                OnPropertyChanged("Products");
            }
        }

        public BindingList<DataModel.ProductType> ProductTypes { get; set; } = new BindingList<DataModel.ProductType>();
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
                    WholeProducts = new BindingList<ProductVM>(productVMs);
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
            WholeProducts = UnchangedProducts;
            OnPropertyChanged("CurrentType");
            OnPropertyChanged("Products");
        }
        private void CanExecuteClearFilter(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_currType != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        #endregion

        #region Pogination

        private BindingList<KeyValuePair<int, BindingList<ProductVM>>> PoginationPages = new BindingList<KeyValuePair<int, BindingList<ProductVM>>>();
        public BindingList<ProductVM> Products
        {
            get
            {
                return PoginationPages.First(x => x.Key == _currentPage).Value;
            }
            set
            {
                WholeProducts = value;
            }
        }
        private BindingList<ProductVM> _wholeProducts;
        private BindingList<ProductVM> WholeProducts
        {
            get
            {
                return _wholeProducts;
            }
            set
            {
                _wholeProducts = value;
                _countPages = value.Count / 20;
                if (_countPages == 0) _countPages = 1;
                _currentPage = 1;
                int k = 0;
                PoginationPages.Clear();
                for (int i = 1; i <= _countPages; i++)
                {
                    PoginationPages.Add(new KeyValuePair<int, BindingList<ProductVM>>(i, new BindingList<ProductVM>()));
                    for (int j = 0; j < (value.Count < 20 ? value.Count : 20); j++)
                    {
                        PoginationPages[i-1].Value.Add(value[k]);
                        k++;
                    }
                }
                Pages.Clear();
                Pages.Add("<");
                for(int i = 1; i < _countPages; i++)
                {
                    Pages.Add(i.ToString());
                }
                Pages.Add(">");

                _currentPage = PoginationPages.First().Key;
                OnPropertyChanged("Products");
                OnPropertyChanged("Pages");
                OnPropertyChanged("CurrentPage");
            }
        }
        private readonly BindingList<ProductVM> UnchangedProducts = new BindingList<ProductVM>();

        private int _countPages;
        private int _currentPage;
        public string CurrentPage
        {
            get
            {
                return Convert.ToString(_currentPage);
            }
            set
            {
                switch (value)
                {
                    case "<":
                        _currentPage -= 1;
                        if(_currentPage < Convert.ToInt32(Pages[1]))
                        {
                            _currentPage = Convert.ToInt32(Pages[Pages.Count - 2]);
                        }
                        break;
                    case ">":
                        _currentPage += 1;
                        if(_currentPage > Convert.ToInt32(Pages[Pages.Count - 2]))
                        {
                            _currentPage = Convert.ToInt32(Pages[1]);
                        }
                        break;
                    default:
                        _currentPage = Convert.ToInt32(value);
                        break;
                }
                OnPropertyChanged("Products");
                OnPropertyChanged("CurrentPage");
            }
        }

        public List<string> Pages { get; set; } = new List<string>();

        #endregion

    }
}
