using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PracticDemoexam.Models.DataModel;

namespace PracticDemoexam.Models.DataModel
{
    public partial class ProductMaterial
    {
        public ProductMaterial(Material material, Product product, double count)
        {
            ProductID = product.ID;
            MaterialID = material.ID;
            Material = material;
            Product = product;
            Count = count;
        }
        public ProductMaterial()
        {

        }
    }
}
