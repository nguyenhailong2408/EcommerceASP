using MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.ViewModel.PostDetailManage
{
    public class PostDetailManageBO: EcommerceASP.Models.TopicDetail
    {
        public int TopicId { get; set; }
        public string TopicInfo { get; set; }
        public string CreatedByName { get; set; }
        public string ImageOld { get; set; }
        public string ImageType { get; set; }
        public HttpPostedFileBase UploadImage { get; set; }
    }
    public class SearchFormViewModel
    {
        public int TopicId { get; set; }
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
        public IPagedList<PostDetailManageBO> Items { get; set; }
    }
}