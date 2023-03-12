using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.ViewModel.Contact
{
    public class ContactBO: EcommerceASP.Models.Contact
    {
        public string ParentSlugName { get; set; }
        public string ParentSlug { get; set; }
        public string SlugName { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public int PageId { get; set; }
        public bool IsChild { get; set; }
    }
}