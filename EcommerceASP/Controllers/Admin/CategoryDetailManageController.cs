using EcommerceASP.Queries;
using EcommerceASP.ViewModel.CategoryDetailManage;
using System.Web.Mvc;

namespace EcommerceASP.Controllers.Admin
{
    [Authorize]
    public class CategoryDetailManageController : Controller
    {
        // GET: CategoryDetailManage
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList(SearchFormViewModel objRequest)
        {
            var model = CategoryDetailManageQuery.GetListData(objRequest);
            return PartialView("Component/_ListData", model);
        }

        public ActionResult Dialog(int? id)
        {
            var model = CategoryDetailManageQuery.GetDataUpdate(id);
            return PartialView("Component/_DialogUpdate", model);
        }

        //[ValidateInput(false)]
        public ActionResult Update(CategoryDetailManageBO objRequest)
        {
            var model = CategoryDetailManageQuery.Update(objRequest);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int? id)
        {
            var model = CategoryDetailManageQuery.Delete(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}