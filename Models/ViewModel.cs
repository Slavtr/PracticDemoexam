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
            ProductListModule = new Modules.ProductListModule(_entities.Product, _entities.ProductType, _entities.Material, _entities.SaveChanges, AddProduct);
        }

        private void AddProduct(ProductVM product)
        {
            _entities.Product.Add(product.Product);
        }

        private DataModel.Entities _entities = new DataModel.Entities();

        public System.Windows.Controls.Frame MainFrame { get; set; }

        public List<System.Windows.Controls.Page> Pages { get; private set; } = new List<System.Windows.Controls.Page>();

        public Modules.ProductListModule ProductListModule { get; set; }
    }
}
