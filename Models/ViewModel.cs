using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PracticDemoexam.Models.PartialModels;

namespace PracticDemoexam.Models
{
    public class ViewModel
    {
        public ViewModel()
        {
            ProductListModule = new Modules.ProductListModule(_entities.Product, _entities.ProductType, _entities.Material, _entities.SaveChanges, AddProduct, RemoveProduct);
        }

        private void AddProduct(ProductVM product)
        {
            _entities.Product.Add(product.Product);
        }

        private void RemoveProduct(ProductVM product)
        {
            var productSale = _entities.ProductSale.FirstOrDefault(x => x.Product.ID == product.Product.ID);
            if (productSale == null)
            {
                _entities.ProductCostHistory.RemoveRange(_entities.ProductCostHistory.Where(x => x.Product.ID == product.Product.ID));
                _entities.ProductMaterial.RemoveRange(_entities.ProductMaterial.Where(x => x.Product.ID == product.Product.ID));
                _entities.Product.Remove(product.Product);
                _entities.SaveChanges();
            }
        }

        private DataModel.Entities _entities = new DataModel.Entities();

        public System.Windows.Controls.Frame MainFrame { get; set; }

        public List<System.Windows.Controls.Page> Pages { get; private set; } = new List<System.Windows.Controls.Page>();

        public Modules.ProductListModule ProductListModule { get; set; }
    }
}
