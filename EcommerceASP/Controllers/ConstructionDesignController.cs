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
    public class ConstructionDesignController : Controller
    {
        // GET: ConstructionDesign
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetComponentPageSlug(int componentId,int componentTypeId)
        {
            var data = ComponentQuery.GetComponentSubDescription(componentId, componentTypeId);
            //string domainName = ConfigurationManager.AppSettings["DomainName"];
            return PartialView($"../Components/{data.HtmlTemplate}", data);
        }
    }
}