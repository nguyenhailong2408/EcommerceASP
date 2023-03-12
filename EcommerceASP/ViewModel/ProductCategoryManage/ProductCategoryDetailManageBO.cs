using MvcPaging;

namespace EcommerceASP.ViewModel.ProductCategoryDetailManage
{
    public class ProductCategoryDetailManageBO : Models.ProductCategoryDetail
    {
        public int PageId { get; set; }
        public string PageName { get; set; }
        public string SlugOld { get; set; }
        public string ProductCategoryInfo { get; set; }
        public string ParentName { get; set; }
        public string CreatedByName { get; set; }
    }

    public class SearchFormViewModel
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public int ParentId { get; set; }
        public int ProductCategoryID { get; set; }

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
        public IPagedList<ProductCategoryDetailManageBO> Items { get; set; }
    }
}