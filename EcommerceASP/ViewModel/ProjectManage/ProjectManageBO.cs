using EcommerceASP.Models;
using EcommerceASP.ViewModel.ComponentTypeManage;
using EcommerceASP.ViewModel.ProjectImageManage;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.ViewModel.ProjectManage
{
    public class ProjectManageBO: Models.Project
    {
        public ProjectManageBO()
        {
            this.lstProjectImages = new List<ProjectImageBO>();
        }
        public string ImageOld { get; set; }
        public HttpPostedFileBase UploadImage { get; set; }
        public string CreatedByName { get; set; }
        public List<HttpPostedFileBase> UploadMultiImage { get; set; }
        public List<ProjectImageBO> lstProjectImages { get; set; }
    }
    public class SearchFormViewModel
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Customer { get; set; }
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
        public IPagedList<ProjectManageBO> Items { get; set; }
    }
}