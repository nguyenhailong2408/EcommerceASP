using MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.ViewModel.Topic
{
    public class TopicViewBO
    {
        public TopicViewBO()
        {
            this.PageCurrent = 1;
            this.PageSize = 10;
            this.lstComponentPageSlug = new List<Models.ComponentPageSlug>();
        }
        public string ParentSlugName { get; set; }
        public string ParentSlug { get; set; }
        public string SlugName { get; set; }
        public string Slug { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public int PageId { get; set; }
        public bool IsChild { get; set; }
        public int? ParentId { get; set; }
        public int? PageSize { get; set; }

        public int? PageCurrent { get; set; }
        public IPagedList<TopicDetailBO> lstTopicDetail { get; set; }
        public List<Models.ComponentPageSlug> lstComponentPageSlug { get; set; }
    }

}