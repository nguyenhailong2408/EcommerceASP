using EcommerceASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.ViewModel.Category
{
    public class ProductCategoryBO: ProductCategory
    {
        public ProductCategoryBO()
        {
            lstProductCategoryDetail = new List<ProductCategoryDetailBO>();
        }
        public List<ProductCategoryDetailBO> lstProductCategoryDetail { get; set; }
    }
}