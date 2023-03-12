using EcommerceASP.Constaint;
using EcommerceASP.Models;
using EcommerceASP.Queries;
using EcommerceASP.ViewModel.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcommerceASP.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var data = HomeQuery.GetComponent();
            return View(data);
        }
        public ActionResult MenuCategory()
        {
            var data = HomeQuery.GetCategory();
            return PartialView("Components/_Menu", data);
        }

        public ActionResult GetComponent(ComponentBO Component)
        {
            var data = HomeQuery.GetDataComponent(Component);
            switch (Component.ComponentTypeId)
            {
                case (int)EnumComponentType.SlideShow:
                    return PartialView("Components/_SlideMain", data);

                case (int)EnumComponentType.Banner:
                    return PartialView("Components/_Banner_Option_1");

                case (int)EnumComponentType.Product:
                    return PartialView("Components/_Module_Product", data);

                case (int)EnumComponentType.TopicThreeCollumn:
                    return PartialView("Components/_Topic_Splide", data);

                case (int)EnumComponentType.TopicFiveCollumn:
                    return PartialView("Components/_Topic_Splide", data);

                default:
                    return PartialView("Components/_Topic_Splide", null);
            }
        }

        
    }
}