using EcommerceASP.Queries;
using EcommerceASP.ViewModel.ComponentTypeManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcommerceASP.Controllers.Admin
{
    public class ComponentTypeManageController : Controller
    {
        // GET: ComponentTypeManage
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(SearchFormViewModel objRequest)
        {
            var model = ComponentTypeManageQuery.GetListData(objRequest);
            return PartialView("Component/_ListData", model);
        }

        public ActionResult Dialog(int? id)
        {
            var model = ComponentTypeManageQuery.GetDataUpdate(id);
            return PartialView("Component/_DialogUpdate", model);
        }

        [ValidateInput(false)]
        public ActionResult Update(ComponentTypeManageBO objRequest)
        {
            var model = ComponentTypeManageQuery.Update(objRequest);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int? id)
        {
            var model = ComponentTypeManageQuery.Delete(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}