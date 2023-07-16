using MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.ViewModel.ProductManage
{
    public class ProductManageBO : EcommerceASP.Models.Product
    {
        public string ProductCategoryInfo { get; set; }
        public string ProductCategoryDetailInfo { get; set; }
        public string SlugOld { get; set; }
        public string ImageOld { get; set; }
        public string ImageType { get; set; }
        public HttpPostedFileBase UploadImage { get; set; }
    }
    public class SearchFormViewModel
    {
        public string NameProduct { get; set; }
        public string Slug { get; set; }
        public int ProductCategoryId { get; set; }
        public int ProductCategoryDetailId { get; set; }
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
        public IPagedList<ProductManageBO> Items { get; set; }
    }
}