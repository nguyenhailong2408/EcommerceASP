using EcommerceASP.Queries;
using EcommerceASP.ViewModel.PostManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcommerceASP.Controllers
{
    [Authorize]
    public class PostManageController : Controller
    {
        // GET: Post
        public ActionResult Index()
        {
            var search = new SearchFormViewModel();
            return View(search);
        }
        public ActionResult GetList(SearchFormViewModel objRequest)
        {
            var model = TopicQuery.GetListData(objRequest);
            return PartialView("Component/_ListData", model);
        }
        public ActionResult Dialog(int? id)
        {
            var model = TopicQuery.GetDataUpdate(id);
            return PartialView("Component/_DialogUpdate", model);
        }

        [ValidateInput(false)]
        public ActionResult Update(PostManageBO objRequest)
        {
            var model = TopicQuery.Update(objRequest);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int? id)
        {
            var model = TopicQuery.DeleteTopic(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}