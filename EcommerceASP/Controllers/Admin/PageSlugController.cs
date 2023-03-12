using EcommerceASP.Queries;
using EcommerceASP.ViewModel.Base;
using EcommerceASP.ViewModel.PageSlug;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcommerceASP.Controllers.Admin
{
    [Authorize]
    public class PageSlugController : Controller
    {
        // GET: PageSlug
        public ActionResult Index()
        {
            var search = new SearchFormViewModel();
            return View(search);
        }
        public ActionResult GetList(SearchFormViewModel objRequest)
        {
            var model = PageSlugQuery.GetListData(objRequest);
            return PartialView("Component/_ListData", model);
        }
        public ActionResult Dialog(int? id)
        {
            var model = PageSlugQuery.GetDataUpdate(id);
            return PartialView("Component/_DialogUpdate", model);
        }
        public ActionResult Update(PageSlugBO objRequest)
        {
            var model = ResponseAPI.GetSuccessResponse("Success",null);
            //model = ResponseAPI.GetFailedResponse("Lỗi rồi nhé");
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}