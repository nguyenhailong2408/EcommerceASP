using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.ViewModel.Prouduct
{
    public class ProductDetailViewBO: EcommerceASP.Models.Product
    {
        public string ParentSlugName { get; set; }
        public string ParentSlug { get; set; }
        public string SlugName { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public int? ParentId { get; set; }
        public int PageId { get; set; }
        public bool IsChild { get; set; }
    }
}