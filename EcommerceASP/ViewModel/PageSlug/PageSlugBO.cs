using MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.ViewModel.PageSlug
{
    public class PageSlugBO: EcommerceASP.Models.PageSlug
    {
        public string PageName { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string AdminSlug { get; set; }
        public bool IsAdminPage { get; set; }
    }
    public class SearchFormViewModel
    {
        public int PageId { get; set; }
        public string PageSlug { get; set; }
        public SearchFormViewModel()
        {
            this.PageCurrent = 1;
        }

        public int? PageCurrent { get; set; }
    }
    public class ListViewModel
    {
        public ListViewModel()
        {
            this.PageSize = 10;
        }
        public int? PageSize { get; set; }
        public IPagedList<PageSlugBO> Items { get; set; }
    }
}