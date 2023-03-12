using EcommerceASP.Queries;
using EcommerceASP.ViewModel.PostDetailManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcommerceASP.Controllers.Admin
{
    [Authorize]
    public class PostDetailManageController : Controller
    {
        // GET: PostDetail
        public ActionResult Index()
        {
            var search = new SearchFormViewModel();
            return View(search);
        }
        public ActionResult GetList(SearchFormViewModel objRequest)
        {
            var model = TopicDetailQuery.GetListData(objRequest);
            return PartialView("Component/_ListData", model);
        }
        public ActionResult Dialog(int? id)
        {
            var model = TopicDetailQuery.GetDataUpdate(id);
            return PartialView("Component/_DialogUpdate", model);
        }

        [ValidateInput(false)]
        public ActionResult Update(PostDetailManageBO objRequest)
        {
            var model = TopicDetailQuery.Update(objRequest);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int? id)
        {
            var model = TopicDetailQuery.Delete(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}