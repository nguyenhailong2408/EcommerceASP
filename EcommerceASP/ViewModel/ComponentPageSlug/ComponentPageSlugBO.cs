using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.ViewModel.ComponentPageSlug
{
    public class ComponentPageSlugBO : Models.ComponentPageSlug
    {
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
}