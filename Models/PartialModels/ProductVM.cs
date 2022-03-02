using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PracticDemoexam.Models.PartialModels
{
    public class ProductVM : INotifyPropertyChanged
    {
        public DataModel.Product Product { get; set; }

        public decimal Cost
        {
            get { return Product.MinCostForAgent; }
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

        public BitmapImage MainImage
        {
            get
            {
                BitmapImage ret = new BitmapImage();
                ret.BeginInit();
                if (string.IsNullOrEmpty(Product.Image) || string.IsNullOrWhiteSpace(Product.Image))
                {
                    ret.UriSource = new Uri("\\Resources\\products\\picture.png", UriKind.Relative);
                }
                else
                {
                    ret.UriSource = new Uri("\\Resources" + Product.Image, UriKind.Relative);
                }
                ret.EndInit();
                return ret;
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
    }
}
