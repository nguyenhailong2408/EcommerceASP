using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.ViewModel.Category
{
    public class CategoryBO : EcommerceASP.Models.Category
    {
        public CategoryBO()
        {
            this.lstProductCategory = new List<ProductCategoryBO>();
            this.lstCategoryDetail = new List<CategoryDetailBO>();
        }
        public List<ProductCategoryBO> lstProductCategory { get; set; }
        public List<CategoryDetailBO> lstCategoryDetail { get; set; }
    }
}