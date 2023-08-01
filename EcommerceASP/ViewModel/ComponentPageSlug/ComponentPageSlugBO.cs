using MvcPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.ViewModel.ComponentPageSlug
{
    public class ComponentPageSlugBO : Models.ComponentPageSlug
    {
        public ComponentPageSlugBO()
        {
            this.lstSubDesc = new List<ComponentSubDescriptionBO>();
        }
        public string CreatedByName { get; set; }
        public List<ComponentSubDescriptionBO> lstSubDesc { get; set; }
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
        public IPagedList<ComponentPageSlugBO> Items { get; set; }
    }

    public class ComponentTemplateViewBO
    {
        public ComponentTemplateViewBO()
        {
            this.lstComponentSubDescription = new List<ComponentSubDescriptionViewBO>();
        }
        public int ComponentTypeId { get; set; }
        public string HtmlTemplate { get; set; }
        public List<ComponentSubDescriptionViewBO> lstComponentSubDescription { get; set; }
    } 
    public class ComponentSubDescriptionViewBO
    {
        public int ComponentId { get; set; }
        public string SubTitle { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
    }
    public class ComponentSubDescriptionBO : Models.ComponentSubDescription
    {
        public string ImageOld { get; set; }
        public HttpPostedFileBase UploadImage { get; set; }
        public string CreatedByName { get; set; }
    }
}