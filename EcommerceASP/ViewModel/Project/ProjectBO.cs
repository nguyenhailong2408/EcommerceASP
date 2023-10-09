using EcommerceASP.ViewModel.ProjectImageManage;
using MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.ViewModel.Project
{
    public class ProjectBO: Models.Project
    {
        public ProjectBO()
        {
            this.lstProjectImages = new List<ProjectImageBO>();
            this.lstProjectRelated = new List<ProjectRelatedBO>();
        }
        public string ParentSlugName { get; set; }
        public string ParentSlug { get; set; }
        public string SlugName { get; set; }
        public int PageId { get; set; }
        public bool IsChild { get; set; }
        public int? ParentId { get; set; }
        public List<ProjectImageBO> lstProjectImages { get; set; }
        public List<ProjectRelatedBO> lstProjectRelated { get; set; }
    }

    public class ProjectRelatedBO: Models.Project
    {

    }

    public class ProjectViewBO
    {
        public ProjectViewBO()
        {
            this.PageCurrent = 1;
            this.PageSize = 9;
        }
        public string ParentSlugName { get; set; }
        public string ParentSlug { get; set; }
        public string SlugName { get; set; }
        public string Slug { get; set; }
        public int PageId { get; set; }
        public bool IsChild { get; set; }
        public int? ParentId { get; set; }
        public string Title { get; set; }
        public string ThumbnailImage { get; set; }
        public int? PageSize { get; set; }
        public int? PageCurrent { get; set; }
        public IPagedList<ProjectBO> lstProject { get; set; }
    }
}