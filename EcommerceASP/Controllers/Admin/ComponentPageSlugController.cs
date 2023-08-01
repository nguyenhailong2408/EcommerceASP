using EcommerceASP.Queries;
using EcommerceASP.ViewModel.ComponentPageSlug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcommerceASP.Controllers.Admin
{
    public class ComponentPageSlugController : Controller
    {
        // GET: ComponentPageSlug
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList(SearchFormViewModel objRequest)
        {
            var model = ComponentPageSlugQuery.GetListData(objRequest);
            return PartialView("Component/_ListData", model);
        }
        public ActionResult Dialog(int? id)
        {
            var model = ComponentPageSlugQuery.GetDataUpdate(id);
            return PartialView("Component/_DialogUpdate", model);
        }
        public ActionResult DialogSubDescription(int? id)
        {
            var model = ComponentPageSlugQuery.GetDataUpdateSubDescription(id);
            return PartialView("Component/_DialogUpdateSubDescription",model);
        }

        [ValidateInput(false)]
        public ActionResult Update(ComponentPageSlugBO objRequest)
        {
            var model = ComponentPageSlugQuery.Update(objRequest);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int? id)
        {
            var model = ComponentPageSlugQuery.Delete(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}