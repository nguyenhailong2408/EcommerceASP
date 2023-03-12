using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.ViewModel.Component
{
    public class BreadcrumbBO
    {
        public string ParentSlugName { get; set; }
        public string ParentSlug { get; set; }
        public string SlugName { get; set; }
        public string Slug { get; set; }
        public bool IsChild { get; set; }
        public string Title { get; set; }
        public bool IsShowTitle { get; set; }
    }
}