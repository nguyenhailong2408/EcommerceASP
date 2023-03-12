using EcommerceASP.Attribute;
using EcommerceASP.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcommerceASP.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        [Route("san-pham/{PageCurrent}")]
        public ActionResult Index(int PageCurrent)
        {
            var myRoute = Url.RouteUrl(RouteData.Values);
            var data = ProductQuery.GetAllProduct(PageCurrent);
            return View(data);
        }
        
    }
}