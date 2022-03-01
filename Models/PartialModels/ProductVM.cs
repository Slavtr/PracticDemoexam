using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticDemoexam.Models.PartialModels
{
    public class ProductVM
    {
        public DataModel.Product Product { get; set; }

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
        }
    }
}
