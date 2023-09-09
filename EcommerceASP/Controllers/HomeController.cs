using EcommerceASP.Constaint;
using EcommerceASP.Models;
using EcommerceASP.Queries;
using EcommerceASP.ViewModel.Component;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            string domainName = ConfigurationManager.AppSettings["DomainName"];
            if (domainName.Equals(EnumDomainName.thuanphat.ToString()))
            {
                return View(data);
            }
            return View("BeeIndex", data);
            
        }
        public ActionResult MenuCategory()
        {
            string domainName = ConfigurationManager.AppSettings["DomainName"];
            var data = HomeQuery.GetCategory();
            if (domainName.Equals(EnumDomainName.thuanphat.ToString()))
            {
                return PartialView("Components/_Menu", data);
            }
            return PartialView("Components/_MenuBee", data);
        }

        public ActionResult MenuMobileCategory()
        {
            string domainName = ConfigurationManager.AppSettings["DomainName"];
            var data = HomeQuery.GetCategory();
            if (domainName.Equals(EnumDomainName.thuanphat.ToString()))
            {
                return PartialView("Components/_MenuMobile", data);
            }
            return PartialView("Components/BeeDecor/_MenuMobile_Bee", data);
        }

        public ActionResult GetComponent(ComponentBO Component)
        {
            var data = HomeQuery.GetDataComponent(Component);
            string domainName = ConfigurationManager.AppSettings["DomainName"];
            if (domainName.Equals(EnumDomainName.thuanphat.ToString()))
            {
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
            else
            {
                switch (Component.ComponentTypeId)
                {
                    case (int)EnumComponentType.SlideShow:
                        return PartialView("Components/_SlideMain", data);
                    default:
                        return PartialView("Components/_Topic_Splide", null);
                }
            }
        }
    }
}