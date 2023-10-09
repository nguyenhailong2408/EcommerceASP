using EcommerceASP.ViewModel.Category;
using EcommerceASP.ViewModel.Component;
using EcommerceASP.ViewModel.Project;
using EcommerceASP.ViewModel.Topic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.ViewModel
{
    public class HomeBO
    {
        public HomeBO()
        {
            this.lstComponent = new List<ComponentBO>();
            this.lstTopicDetail = new List<TopicDetailBO>();
            this.lstProject = new List<ProjectBO>();
        }
        public List<ComponentBO> lstComponent { get; set; }
        public List<TopicDetailBO> lstTopicDetail { get; set; }
        public List<ProjectBO> lstProject { get; set; }
    }
}