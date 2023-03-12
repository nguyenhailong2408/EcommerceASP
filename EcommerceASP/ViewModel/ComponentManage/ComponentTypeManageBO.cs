using MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.ViewModel.ComponentTypeManage
{
    public class ComponentTypeManageBO: Models.ComponentType
    {
        public string ImageOld { get; set; }
        public HttpPostedFileBase UploadImage { get; set; }
        public string CreatedByName { get; set; }
    }
    public class SearchFormViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
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
        public IPagedList<ComponentTypeManageBO> Items { get; set; }
    }
}