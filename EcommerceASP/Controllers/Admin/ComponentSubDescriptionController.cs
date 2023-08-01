using EcommerceASP.Queries;
using EcommerceASP.ViewModel.ComponentSubDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcommerceASP.Controllers.Admin
{
    public class ComponentSubDescriptionController : Controller
    {
        // GET: ComponentSubDescription
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList(SearchFormViewModel objRequest)
        {
            var model = ComponentSubDescriptionQuery.GetListData(objRequest);
            return PartialView("Component/_ListData", model);
        }
    }
}