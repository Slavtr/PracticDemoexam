using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PracticDemoexam.Models.PartialModels
{
    public class ProductVM : INotifyPropertyChanged, IEditableObject
    {
        public DataModel.Product Product { get; set; }

        public decimal Cost
        {
            get { return Product.MinCostForAgent; }
            set 
            {
                if (value >= 0)
                {
                    Product.MinCostForAgent = value;
                    OnPropertyChanged("Product");
                    OnPropertyChanged("Cost");
                }
            }
        }

        public string Materials
        {
            get
            {
                string ret = "Материалы: ";
                foreach(DataModel.ProductMaterial mat in Product.ProductMaterial)
                {
                    ret += mat.Material.Title + ", ";
                }
                ret = ret.Trim(new char[]{ ',', ' '});
                return ret;
            }
        }
        public ObservableCollection<DataModel.ProductMaterial> ListMaterials
        {
            get
            {
                return new ObservableCollection<DataModel.ProductMaterial>(Product.ProductMaterial);
            }
            set
            {
                Product.ProductMaterial = value.ToList();
                OnPropertyChanged("ListMaterials");
            }
        }

        private BitmapImage _mainImage;
        public BitmapImage MainImage
        {
            get
            {
                BitmapImage _mainImage = new BitmapImage();
                _mainImage.BeginInit();
                if (string.IsNullOrEmpty(Product.Image) || string.IsNullOrWhiteSpace(Product.Image))
                {
                    _mainImage.UriSource = new Uri("\\Resources\\products\\picture.png", UriKind.RelativeOrAbsolute);
                }
                else
                {
                    _mainImage.UriSource = new Uri(Environment.CurrentDirectory + "\\Resources" + Product.Image, UriKind.Absolute);
                }
                _mainImage.EndInit();
                return _mainImage;
            }
            set
            {
                string path = value.UriSource.OriginalString;
                Product.Image = path;
                _mainImage = value;
                OnPropertyChanged("Product");
                OnPropertyChanged("MainImage");
            }
        }

        public bool SoldInLastMonth
        {
            get
            {
                var lastSale = Product.ProductSale.FirstOrDefault(x => (DateTime.Now - x.SaleDate).TotalDays <= 30);
                if(lastSale == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsSelected { get; set; }

        public ProductVM(DataModel.Product product = null)
        {
            if (product != null)
            {
                Product = product;
            }
            else
            {
                Product = new DataModel.Product();
            }
            OnPropertyChanged("Product");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ProductVM UnchangedProduct;

        public void BeginEdit()
        {
            UnchangedProduct = new ProductVM
            {
                Product = new DataModel.Product
                {
                    ID = Product.ID,
                    ProductTypeID = Product.ProductTypeID,
                    ArticleNumber = Product.ArticleNumber,
                    Description = Product.Description,
                    Image = Product.Image,
                    MinCostForAgent = Product.MinCostForAgent,
                    ProductCostHistory = Product.ProductCostHistory,
                    ProductionPersonCount = Product.ProductionPersonCount,
                    ProductionWorkshopNumber = Product.ProductionWorkshopNumber,
                    ProductMaterial = Product.ProductMaterial,
                    ProductSale = Product.ProductSale,
                    ProductType = Product.ProductType,
                    Title = Product.Title
                },
                IsSelected = this.IsSelected
            };
            OnPropertyChanged("IsIsEdit");
        }

        public void EndEdit()
        {
            UnchangedProduct = null;
            OnPropertyChanged("IsIsEdit");
        }

        public void CancelEdit()
        {
            if (UnchangedProduct == null) return;

            Product = UnchangedProduct.Product;
            IsSelected = UnchangedProduct.IsSelected;
            OnPropertyChanged("IsIsEdit");
        }

        public bool IsInEdit
        {
            get
            {
                if(UnchangedProduct == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
