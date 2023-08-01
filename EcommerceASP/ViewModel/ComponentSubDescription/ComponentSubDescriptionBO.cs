using MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.ViewModel.ComponentSubDescription
{
    public class ComponentSubDescriptionBO : Models.ComponentSubDescription
    {
        public string CreatedByName { get; set; }
    }
    public class SearchFormViewModel
    {
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
        public IPagedList<ComponentSubDescriptionBO> Items { get; set; }
    }
}