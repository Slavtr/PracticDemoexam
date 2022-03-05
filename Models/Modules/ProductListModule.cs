using Microsoft.Win32;
using PracticDemoexam.Models.PartialModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace PracticDemoexam.Models.Modules
{
    public class ProductListModule : INotifyPropertyChanged
    {
        public ProductListModule(
            IEnumerable<DataModel.Product> products,
            IEnumerable<DataModel.ProductType> productTypes,
            IEnumerable<DataModel.Material> materials,
            Func<int> save,
            Action<ProductVM> addProduct,
            Action<ProductVM> removeProduct)
        {
            LoadProducts(products);
            LoadProductTypes(productTypes);
            LoadCommands();
            LoadMaterials(materials);
            SaveAll = save;
            AddProduct = addProduct;
            RemoveProduct = removeProduct;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Functions

        private readonly Func<int> SaveAll;
        private readonly Action<ProductVM> AddProduct;
        private readonly Action<ProductVM> RemoveProduct;

        #endregion

        #region LoadThings

        public bool LoadProducts(IEnumerable<DataModel.Product> products)
        {
            try
            {
                UnchangedProducts.Clear();
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
                    _unchangedMaterials.Add(m);
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

            AddMaterialCommand = new RoutedCommand("AddMaterial", GetType());
            AddMaterialCommandBinding = new CommandBinding(AddMaterialCommand, ExecuteAddMaterialCommand, CanExecuteAddMaterialCommand);

            DeleteMaterialCommand = new RoutedCommand("DeleteMaterial", GetType());
            DeleteMaterialCommandBinding = new CommandBinding(DeleteMaterialCommand, ExecuteDeleteMaterialCommand, CanExecuteDeleteMaterialCommand);

            DeleteProductCommand = new RoutedCommand("DeleteProduct", GetType());
            DeleteProductCommandBinding = new CommandBinding(DeleteProductCommand, ExecuteDeleteProductCommand, CanExecuteDeleteProductCommand);

            SetImageCommand = new RoutedCommand("SetImage", GetType());
            SetImageCommandBinding = new CommandBinding(SetImageCommand, ExecuteSetImageCommand);

            ProductPageCommandBindingCollection.Add(ClearFilterCommandBinding);
            ProductPageCommandBindingCollection.Add(PageLeftCommandBinding);
            ProductPageCommandBindingCollection.Add(PageRightCommandBinding);
            ProductPageCommandBindingCollection.Add(ChangeManyCostsCommandBinding);
            ProductPageCommandBindingCollection.Add(ItemRedactCommandBinding);
            ProductPageCommandBindingCollection.Add(AddProductCommandBinding);

            ProductChangeCostCommandBindingCollection.Add(ChangeCostCommandBinding);

            ProductRedactCommandBindingCollection.Add(RedactItemCommandBinding);
            ProductRedactCommandBindingCollection.Add(CancelRedactItemCommandBinding);
            ProductRedactCommandBindingCollection.Add(AddMaterialCommandBinding);
            ProductRedactCommandBindingCollection.Add(DeleteMaterialCommandBinding);
            ProductRedactCommandBindingCollection.Add(DeleteProductCommandBinding);
            ProductRedactCommandBindingCollection.Add(SetImageCommandBinding);
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
                    var productVMs = WholeProducts.Where(x => x.Product.Title.Contains(value)).ToList();
                    WholeProducts = new ObservableCollection<ProductVM>(productVMs);
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
                                var productVMs = UnchangedProducts.OrderByDescending(x => x.Product.Title).ToList();
                                WholeProducts = new ObservableCollection<ProductVM>(productVMs);
                            }
                            else
                            {
                                var productVMs = UnchangedProducts.OrderBy(x => x.Product.Title).ToList();
                                WholeProducts = new ObservableCollection<ProductVM>(productVMs);
                            }
                            break;
                        case "Номер цеха":
                            if (_curSort == value)
                            {
                                var productVMs = UnchangedProducts.OrderByDescending(x => x.Product.ProductionWorkshopNumber).ToList();
                                WholeProducts = new ObservableCollection<ProductVM>(productVMs);
                            }
                            else
                            {
                                var productVMs = UnchangedProducts.OrderBy(x => x.Product.ProductionWorkshopNumber).ToList();
                                WholeProducts = new ObservableCollection<ProductVM>(productVMs);
                            }
                            break;
                        case "Мин. стоимость":
                            if (_curSort == value)
                            {
                                var productVMs = UnchangedProducts.OrderByDescending(x => x.Product.MinCostForAgent).ToList();
                                WholeProducts = new ObservableCollection<ProductVM>(productVMs);
                            }
                            else
                            {
                                var productVMs = UnchangedProducts.OrderBy(x => x.Product.MinCostForAgent).ToList();
                                WholeProducts = new ObservableCollection<ProductVM>(productVMs);
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
                    WholeProducts = new ObservableCollection<ProductVM>(productVMs);
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

        private ObservableCollection<KeyValuePair<int, ObservableCollection<ProductVM>>> PoginationPages = new ObservableCollection<KeyValuePair<int, ObservableCollection<ProductVM>>>();
        public ObservableCollection<ProductVM> Products
        {
            get
            {
                if (_currentPage != 0)
                {
                    return PoginationPages.First(x => x.Key == _currentPage).Value;
                }
                else
                {
                    return PoginationPages.First().Value;
                }
            }
            set
            {
                WholeProducts = value;
            }
        }
        private ObservableCollection<ProductVM> _wholeProducts;
        private ObservableCollection<ProductVM> WholeProducts
        {
            get
            {
                return _wholeProducts;
            }
            set
            {
                _wholeProducts = value;
                double d = value.Count;
                _countPages =  Convert.ToInt32(Math.Ceiling(d / 20)) + 1;
                if (_countPages == 0) _countPages = 1;
                _currentPage = 1;
                int k = 0;
                PoginationPages.Clear();
                for (int i = 1; i <= _countPages; i++)
                {
                    PoginationPages.Add(new KeyValuePair<int, ObservableCollection<ProductVM>>(i, new ObservableCollection<ProductVM>()));
                    int c = value.Count - k;
                    for (int j = 0; j < (c < 20 ? c : 20); j++)
                    {
                        PoginationPages[i - 1].Value.Add(value[k]);
                        if (k == value.Count) continue;
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
        private readonly ObservableCollection<ProductVM> UnchangedProducts = new ObservableCollection<ProductVM>();

        private int _countPages;
        private int _currentPage = 1;
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
                OnPropertyChanged("Pages");
            }
        }

        public ObservableCollection<string> Pages { get; set; } = new ObservableCollection<string>();

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
        private ObservableCollection<DataModel.Material> _unchangedMaterials = new ObservableCollection<DataModel.Material>();
        public DataModel.Material AddMaterial { get; set; }
        public DataModel.ProductMaterial DeleteMaterial { get; set; }
        private string _materialFilter;
        public string TextMaterial
        {
            get
            {
                return _materialFilter;
            }
            set
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                {
                    Materials = _unchangedMaterials;
                }
                else
                {
                    Materials = new ObservableCollection<DataModel.Material>(_unchangedMaterials.Where(x => x.Title.Contains(value)));
                }
                _materialFilter = value;
                OnPropertyChanged("Materials");
            }
        }

        private ObservableCollection<ProductVM> SelectedItems
        {
            get
            {
                ObservableCollection<ProductVM> ret = new ObservableCollection<ProductVM>(WholeProducts.Where(x => x.IsSelected == true));
                return ret;
            }
        }
        public ProductVM SelectedItem
        {
            get
            {
                if (SelectedItems.Count > 1 && SelectedItems.Count != 0)
                {
                    return SelectedItems.Last();
                }
                else
                {
                    return SelectedItems.First();
                }
            }
        }
        public string SelectedItemArticleNumber
        {
            get
            {
                if (string.IsNullOrEmpty(SelectedItem.Product.ArticleNumber) || string.IsNullOrWhiteSpace(SelectedItem.Product.ArticleNumber))
                {
                    return "Артикул";
                }
                else
                {
                    return SelectedItem.Product.ArticleNumber;
                }
            }
            set
            {
                var listArticles = Products.Where(x => x.Product.ArticleNumber.Contains(value)).ToList();
                if (listArticles.Count == 1 && listArticles.First() == SelectedItem || listArticles.Count == 0)
                {
                    if(value.Length >= 10)
                    {
                        value = value.Substring(0, 10);
                    }
                    SelectedItem.Product.ArticleNumber = value;
                }
                else
                {
                    MessageBox.Show("Такой артикул уже есть");
                }
                OnPropertyChanged("SelectedItemArticleNumber");
                OnPropertyChanged("SelectedItem");
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
            SelectedItem.BeginEdit();
            window.ShowDialog();
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
            switch ((e.Command as RoutedCommand).Name)
            {
                case "RedactItem":
                    SaveAll();
                    (sender as Window).Hide();
                    OnPropertyChanged("Products");
                    break;
                case "CancelRedactItem":
                    SelectedItem.CancelEdit();
                    (sender as Window).Hide();
                    OnPropertyChanged("Products");
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

        public bool WayOfChangeCost { get; set; } = true;
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
            var product = new ProductVM();
            product.IsSelected = true;
            foreach (ProductVM pvm in UnchangedProducts)
            {
                pvm.IsSelected = false;
            }
            product.Product.ID = WholeProducts.Max(x => x.Product.ID) + 1;
            UnchangedProducts.Add(product);
            WholeProducts = UnchangedProducts;
            AddProduct.Invoke(product);
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

        private void CanExecuteAddMaterialCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (AddMaterial != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void CanExecuteDeleteMaterialCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (DeleteMaterial != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void ExecuteAddMaterialCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var material = new DataModel.ProductMaterial(AddMaterial, SelectedItem.Product, 0);
            var listMat = SelectedItem.ListMaterials;
            listMat.Add(material);
            SelectedItem.ListMaterials = listMat;
            OnPropertyChanged("SelectedItem.ListMaterials");
        }
        private void ExecuteDeleteMaterialCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var listMat = SelectedItem.ListMaterials;
            listMat.Remove(SelectedItem.ListMaterials.First(x => x == DeleteMaterial));
            SelectedItem.ListMaterials = listMat;
        }

        public RoutedCommand DeleteProductCommand { get; private set; }
        public CommandBinding DeleteProductCommandBinding { get; private set; }

        private void ExecuteDeleteProductCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var removedProduct = SelectedItem;
            Products.Remove(removedProduct);
            RemoveProduct(removedProduct);
            (sender as Window).Hide();
            WayOfChangeCost = false;
            CostChangeNumber = string.Empty;
            SaveAll();
            OnPropertyChanged("Products");
        }
        private void CanExecuteDeleteProductCommand(object sender, CanExecuteRoutedEventArgs e)
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

        #endregion

        #region Images

        public RoutedCommand SetImageCommand { get; private set; }
        public CommandBinding SetImageCommandBinding { get; private set; }

        private void ExecuteSetImageCommand(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Images (.png, .jpg)|*.png;*.jpg",
                Multiselect = false
            };
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                string name = dlg.FileName.Split('\\').Last();
                string newName = "Resources/products/" + name;
                File.Copy(dlg.FileName, newName, true);

                SelectedItem.MainImage = new BitmapImage(new Uri(newName.Replace("Resources", "").Replace("/", "\\"), UriKind.Relative));
                OnPropertyChanged("Products");
            }
        }

        #endregion

    }
}
