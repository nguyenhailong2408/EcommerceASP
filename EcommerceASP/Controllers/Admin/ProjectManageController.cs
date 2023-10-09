using EcommerceASP.Queries;
using EcommerceASP.ViewModel.ProjectManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcommerceASP.Controllers.Admin
{
    [Authorize]
    public class ProjectManageController : Controller
    {
        // GET: ProjectManage
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(SearchFormViewModel objRequest)
        {
            var model = ProjectManageQuery.GetListData(objRequest);
            return PartialView("Component/_ListData", model);
        }
        public ActionResult Dialog(int? id)
        {
            var model = ProjectManageQuery.GetDataUpdate(id);
            return PartialView("Component/_DialogUpdate", model);
        }

        [ValidateInput(false)]
        public ActionResult Update(ProjectManageBO objRequest)
        {
            var model = ProjectManageQuery.Update(objRequest);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int? id)
        {
            var model = ProjectManageQuery.Delete(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}