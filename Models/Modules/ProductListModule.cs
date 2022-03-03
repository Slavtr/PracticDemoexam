﻿using PracticDemoexam.Models.PartialModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PracticDemoexam.Models.Modules
{
    public class ProductListModule : INotifyPropertyChanged
    {
        public ProductListModule(IEnumerable<DataModel.Product> products, IEnumerable<DataModel.ProductType> productTypes, IEnumerable<DataModel.Material> materials, Func<int> save, Action<ProductVM> addProduct)
        {
            LoadProducts(products);
            LoadProductTypes(productTypes);
            LoadCommands();
            LoadMaterials(materials);
            SaveAll = save;
            AddProduct = addProduct;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Functions

        private readonly Func<int> SaveAll;
        private readonly Action<ProductVM> AddProduct;

        #endregion

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
        private bool LoadMaterials(IEnumerable<DataModel.Material> materials)
        {
            try
            {
                foreach (DataModel.Material m in materials)
                {
                    Materials.Add(m);
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
            PageLeftCommand = new RoutedCommand("PageLeft", GetType());
            PageLeftCommandBinding = new CommandBinding(PageLeftCommand, PageScrollMethod, CanExecutePageScroll);
            PageRightCommand = new RoutedCommand("PageRight", GetType());
            PageRightCommandBinding = new CommandBinding(PageRightCommand, PageScrollMethod, CanExecutePageScroll);

            ChangeManyCostsCommand = new RoutedCommand("ChangeManyCosts", GetType());
            ChangeManyCostsCommandBinding = new CommandBinding(ChangeManyCostsCommand, ChangeManyCosts, CanExecuteChangeManyCosts);

            ChangeCostCommand = new RoutedCommand("ChangeCost", GetType());
            ChangeCostCommandBinding = new CommandBinding(ChangeCostCommand, ChangeCost);

            RedactItemCommand = new RoutedCommand("RedactItem", GetType());
            RedactItemCommandBinding = new CommandBinding(RedactItemCommand, RedactItem);

            CancelRedactItemCommand = new RoutedCommand("CancelRedactItem", GetType());
            CancelRedactItemCommandBinding = new CommandBinding(CancelRedactItemCommand, RedactItem);

            ItemRedactCommand = new RoutedCommand("ItemRedact", GetType());
            ItemRedactCommandBinding = new CommandBinding(ItemRedactCommand, RedactItemWindowOpen, CanExecuteRedactItem);

            AddProductCommand = new RoutedCommand("AddProduct", GetType());
            AddProductCommandBinding = new CommandBinding(AddProductCommand, AddProductExecute);

            ProductPageCommandBindingCollection.Add(ClearFilterCommandBinding);
            ProductPageCommandBindingCollection.Add(PageLeftCommandBinding);
            ProductPageCommandBindingCollection.Add(PageRightCommandBinding);
            ProductPageCommandBindingCollection.Add(ChangeManyCostsCommandBinding);
            ProductPageCommandBindingCollection.Add(ItemRedactCommandBinding);
            ProductPageCommandBindingCollection.Add(AddProductCommandBinding);

            ProductChangeCostCommandBindingCollection.Add(ChangeCostCommandBinding);

            ProductRedactCommandBindingCollection.Add(RedactItemCommandBinding);
            ProductRedactCommandBindingCollection.Add(CancelRedactItemCommandBinding);
            return true;
        }

        #endregion

        #region CommandBindingCollections

        public CommandBindingCollection ProductPageCommandBindingCollection { get; set; } = new CommandBindingCollection();

        public CommandBindingCollection ProductChangeCostCommandBindingCollection { get; set; } = new CommandBindingCollection();

        public CommandBindingCollection ProductRedactCommandBindingCollection { get; set; } = new CommandBindingCollection();

        #endregion

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
                        PoginationPages[i - 1].Value.Add(value[k]);
                        k++;
                    }
                }
                Pages.Clear();
                for (int i = 1; i < _countPages; i++)
                {
                    Pages.Add(i.ToString());
                }

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
                _currentPage = Convert.ToInt32(value);
                OnPropertyChanged("Products");
                OnPropertyChanged("CurrentPage");
            }
        }

        public List<string> Pages { get; set; } = new List<string>();

        public RoutedCommand PageRightCommand { get; private set; }
        public CommandBinding PageRightCommandBinding { get; private set; }

        public RoutedCommand PageLeftCommand { get; private set; }
        public CommandBinding PageLeftCommandBinding { get; private set; }

        private void PageScrollMethod(object sender, ExecutedRoutedEventArgs e)
        {
            switch ((e.Command as RoutedCommand).Name)
            {
                case "PageLeft":
                    _currentPage -= 1;
                    if (_currentPage < Convert.ToInt32(Pages[0]))
                    {
                        _currentPage = Convert.ToInt32(Pages[Pages.Count - 1]);
                    }
                    break;
                case "PageRight":
                    _currentPage += 1;
                    if (_currentPage > Convert.ToInt32(Pages[Pages.Count - 1]))
                    {
                        _currentPage = Convert.ToInt32(Pages[0]);
                    }
                    break;
            }
            OnPropertyChanged("Products");
            OnPropertyChanged("CurrentPage");
        }
        private void CanExecutePageScroll(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Pages.Count > 1)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        #endregion

        #region ContextMenu

        public Window Window { get; set; } = new Window();

        public ObservableCollection<DataModel.Material> Materials { get; set; } = new ObservableCollection<DataModel.Material>();
        public DataModel.Material CurrentMaterial { get; set; }

        private ObservableCollection<ProductVM> SelectedItems
        {
            get
            {
                ObservableCollection<ProductVM> ret = new ObservableCollection<ProductVM>(Products.Where(x => x.IsSelected == true));
                return ret;
            }
        }
        public ProductVM SelectedItem
        {
            get
            {
                if(SelectedItems.Count > 1)
                {
                    return null;
                }
                else
                {
                    return SelectedItems.First();
                }
            }
            set
            {
                value.IsSelected = true;
                foreach(ProductVM pvm in SelectedItems)
                {
                    pvm.IsSelected = false;
                }
                SelectedItems.Clear();
                SelectedItems.Add(value);
            }
        }

        public RoutedCommand ItemRedactCommand { get; private set; }
        public CommandBinding ItemRedactCommandBinding { get; private set; }

        private void RedactItemWindowOpen(object sender, ExecutedRoutedEventArgs e)
        {
            var window = new Windows.RedactWindow()
            {
                Owner = Window
            };
            window.ShowDialog();
            SelectedItem.BeginEdit();
        }
        private void CanExecuteRedactItem(object sender, CanExecuteRoutedEventArgs e)
        {
            if (SelectedItems.Count == 1)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        public RoutedCommand RedactItemCommand { get; private set; }
        public CommandBinding RedactItemCommandBinding { get; private set; }

        public RoutedCommand CancelRedactItemCommand { get; private set; }
        public CommandBinding CancelRedactItemCommandBinding { get; private set; }

        private void RedactItem(object sender, ExecutedRoutedEventArgs e)
        {
            switch((e.Command as RoutedCommand).Name)
            {
                case "RedactItem":
                    SaveAll();
                    (sender as Window).Hide();
                    OnPropertyChanged("Products");
                    break;
                case "CancelRedactItem":
                    SelectedItem.CancelEdit();
                    break;
            }
        }

        public RoutedCommand ChangeManyCostsCommand { get; private set; }
        public CommandBinding ChangeManyCostsCommandBinding { get; private set; }

        private void ChangeManyCosts(object sender, ExecutedRoutedEventArgs e)
        {
            var window = new Windows.ChangeCostWindow
            {
                Owner = Window
            };
            window.ShowDialog();
        }
        private void CanExecuteChangeManyCosts(object sender, CanExecuteRoutedEventArgs e)
        {
            if (SelectedItems.Count > 1)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        public bool WayOfChangeCost { get; set; }
        public string CostChangeNumber
        {
            get;
            set;
        }

        public RoutedCommand ChangeCostCommand { get; private set; }
        public CommandBinding ChangeCostCommandBinding { get; private set; }

        private void ChangeCost(object sender, ExecutedRoutedEventArgs e)
        {
            if (int.TryParse(CostChangeNumber, out int nomber))
            {
                foreach (ProductVM pvm in SelectedItems)
                {
                    if (WayOfChangeCost)
                    {
                        pvm.Cost += nomber;
                    }
                    else
                    {
                        pvm.Cost = nomber;
                    }
                }
            }
            (sender as Window).Hide();
            WayOfChangeCost = false;
            CostChangeNumber = string.Empty;
            SaveAll();
            OnPropertyChanged("Products");
        }

        public RoutedCommand AddProductCommand { get; private set; }
        public CommandBinding AddProductCommandBinding { get; private set; }

        private void AddProductExecute(object sender, ExecutedRoutedEventArgs e)
        {
            SelectedItem = new ProductVM();
            AddProduct.Invoke(SelectedItem);
            var window = new Windows.RedactWindow()
            {
                Owner = Window
            };
            window.ShowDialog();
            SelectedItem.BeginEdit();
        }

        public RoutedCommand AddMaterialCommand { get; private set; }
        public CommandBinding AddMaterialCommandBinding { get; private set; }

        public RoutedCommand DeleteMaterialCommand { get; private set; }
        public CommandBinding DeleteMaterialCommandBinding { get; private set; }

        private void CanExecuteAddMaterialCommand(object sender, ExecutedRoutedEventArgs e)
        {

        }

        #endregion

    }
}
