using EcommerceASP.ViewModel.Prouduct;
using EcommerceASP.ViewModel.Topic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcommerceASP.ViewModel.Component
{
    public class ComponentBO: EcommerceASP.Models.Component
    {
        public ComponentBO()
        {
            this.lstProduct = new List<EcommerceASP.Models.Product>();
            this.lstTopic = new List<TopicBO>();
        }
        public List<EcommerceASP.Models.Product> lstProduct { get; set; }
        public List<TopicBO> lstTopic { get; set; }
    }

    public class ComponentViewBO
    {
        public ComponentViewBO()
        {
            this.lstDetailComponent = new List<ComponentDetailViewBO>();
            this.lstComponentSubDescription = new List<ComponentSubDescriptionViewBO>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string ParentSlug { get; set; }
        public int Rows { get; set; }
        public int Collumns { get; set; }
        public bool IsSlide { get; set; }
        public List<ComponentDetailViewBO> lstDetailComponent { get; set; }
        public List<ComponentSubDescriptionViewBO> lstComponentSubDescription { get; set; }
    }
    public class ComponentDetailViewBO
    {
        public int ReferenceId { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string ParentSlug { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string FolderImage { get; set; }
        public string ImageName { get; set; }
        public int Priority { get; set; }
        public decimal Price { get; set; }
        public decimal PriceSale { get; set; }
        public DateTime? Created_at { get; set; }
        public string Created_by { get; set; }
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