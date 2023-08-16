using EcommerceASP.Constaint;
using EcommerceASP.Queries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcommerceASP.Controllers
{
    
    public class RouterController : Controller
    {
        // GET: Router
        public ActionResult Index()
        {
            return View();
        }

        [Route("{strSlug}")]
        public ActionResult Index(string strSlug, int page = 1)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(strSlug))
                {
                    var router = RouterQuery.GetRouterPage(strSlug);
                    if (router == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    if (router.IsAdminPage)
                    {
                        return Redirect(router.AdminSlug);
                    }
                    string domainName = ConfigurationManager.AppSettings["DomainName"];
                    
                    switch (router?.PageId)
                    {
                        case 2:
                            var data2 = ProductQuery.GetProductList(strSlug, page);
                            data2.PageId = router.PageId;
                            return View("../Product/Index", data2);
                        case 3:
                            var data3 = TopicQuery.GetTopic(strSlug, page);
                            data3.PageId = router.PageId;
                            if (domainName.Equals(EnumDomainName.thuanphat.ToString()))
                            {
                                return View("../ConstructionDesign/Index", data3);
                            }
                            return View("../ConstructionDesign/BeeDecor/BeeIndex", data3);
                        case 4:
                            var data4 = TopicQuery.GetTopic(strSlug, page);
                            data4.PageId = router.PageId;
                            if (domainName.Equals(EnumDomainName.thuanphat.ToString()))
                            {
                                return View("../Topic/Index", data4);
                            }
                            return View("../Topic/BeeDecor/BeeIndex", data4);
                        case 8:
                            var data8 = ProductQuery.GetProductDetail(strSlug);
                            data8.PageId = router.PageId;
                            return View("../ProductDetail/Index", data8);
                        case 9:
                            var data9 = TopicQuery.GetTopic(strSlug, page);
                            data9.PageId = router.PageId;
                            if (domainName.Equals(EnumDomainName.thuanphat.ToString()))
                            {
                                return View("../ConstructionDesign/_Detail", data9);
                            }
                            return View("../ConstructionDesign/BeeDecor/_BeeDetail", data9);
                        case 10:
                            var data10 = TopicQuery.GetTopic(strSlug, page);
                            data10.PageId = router.PageId;
                            if (domainName.Equals(EnumDomainName.thuanphat.ToString()))
                            {
                                return View("../Topic/_Detail", data10);
                            }
                            return View("../Topic/BeeDecor/_BeeDetail", data10);
                        case 13:
                            var data13 = ContactQuery.GetContact(strSlug);
                            data13.PageId = router.PageId;
                            if (domainName.Equals(EnumDomainName.thuanphat.ToString()))
                            {
                                return View("../Contact/Index", data13);
                            }
                            return View("../Contact/BeeIndex", data13);
                        case 22:
                            var data22 = TopicQuery.GetComponent(strSlug, page);
                            data22.PageId = router.PageId;
                            return View("../PageComponent/Index", data22);
                        default:
                            return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch(Exception objEx)
            {
                return Redirect("Error/Index");
            }
            
        }
    }
}