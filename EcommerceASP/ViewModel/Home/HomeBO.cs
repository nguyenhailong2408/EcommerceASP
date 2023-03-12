using EcommerceASP.ViewModel.Category;
using EcommerceASP.ViewModel.Component;
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
        }
        public List<ComponentBO> lstComponent { get; set; }
    }
}