using EcommerceASP.Queries;
using EcommerceASP.ViewModel.CategoryManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcommerceASP.Controllers.Admin
{
    [Authorize]
    public class CategoryManageController : Controller
    {
        // GET: CategoryManage
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(SearchFormViewModel objRequest)
        {
            var model = CategoryManageQuery.GetListData(objRequest);
            return PartialView("Component/_ListData", model);
        }
        public ActionResult Dialog(int? id)
        {
            var model = CategoryManageQuery.GetDataUpdate(id);
            return PartialView("Component/_DialogUpdate", model);
        }

        [ValidateInput(false)]
        public ActionResult Update(CategoryManageBO objRequest)
        {
            var model = CategoryManageQuery.Update(objRequest);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int? id)
        {
            var model = CategoryManageQuery.Delete(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}