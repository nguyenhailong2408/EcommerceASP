using MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.ViewModel.CategoryDetailManage
{
    public class CategoryDetailManageBO : Models.CategoryDetail
    {
        public int? PageId { get; set; }
        public string PageInfo { get; set; }
        public string ParentName { get; set; }
        public string CategoryName { get; set; }
        public string CreatedByName { get; set; }
        public string ImageOld { get; set; }
        public HttpPostedFileBase UploadImage { get; set; }
    }
    public class SearchFormViewModel
    {
        public int CategoryId { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
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
        public IPagedList<CategoryDetailManageBO> Items { get; set; }
    }
}