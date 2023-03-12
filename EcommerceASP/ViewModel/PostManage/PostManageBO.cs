using MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.ViewModel.PostManage
{
    public class PostManageBO: EcommerceASP.Models.Topic
    {
        public string CreatedByName { get; set; }
        public string CategoryInfo { get; set; }
    }
    public class SearchFormViewModel
    {
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
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
        public IPagedList<PostManageBO> Items { get; set; }
    }
}