using EcommerceASP.Queries;
using EcommerceASP.ViewModel.SlideManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcommerceASP.Controllers
{
    [Authorize]
    public class SlideManageController : Controller
    {
        // GET: Slide
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(SearchFormViewModel objRequest)
        {
            var model = SlideManageQuery.GetListData(objRequest);
            return PartialView("Component/_ListData", model);
        }

        public ActionResult Dialog(int? id)
        {
            var model = SlideManageQuery.GetDataUpdate(id);
            return PartialView("Component/_DialogUpdate", model);
        }

        [ValidateInput(false)]
        public ActionResult Update(SlideManageBO objRequest)
        {
            var model = SlideManageQuery.Update(objRequest);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int? id)
        {
            var model = SlideManageQuery.Delete(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}