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

        public ActionResult GetDataSubDescription(int? ComponentID)
        {
            var model = ComponentPageSlugQuery.GetDataSubDescription(ComponentID);
            return PartialView("Component/_ListDataSubDescription", model);
        }

        public ActionResult DialogSubDescription(int? id, int? componentId, string strPageSlug)
        {
            var model = ComponentPageSlugQuery.GetDataUpdateSubDescription(id,componentId,strPageSlug);
            return PartialView("Component/_DialogUpdateSubDescription", model);
        }

        [ValidateInput(false)]
        public ActionResult UpdateSubDescription(ComponentSubDescriptionBO objRequest)
        {
            var model = ComponentPageSlugQuery.UpdateSubDescription(objRequest);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}