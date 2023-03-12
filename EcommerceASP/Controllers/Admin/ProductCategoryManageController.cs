using EcommerceASP.Queries;
using EcommerceASP.ViewModel.ProductCategoryManage;
using System.Web.Mvc;

namespace EcommerceASP.Controllers.Admin
{
    [Authorize]
    public class ProductCategoryManageController : Controller
    {
        // GET: ProductCategoryManage
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList(SearchFormViewModel objRequest)
        {
            var model = ProductCategoryManageQuery.GetListData(objRequest);
            return PartialView("Component/_ListData", model);
        }

        public ActionResult Dialog(int? id)
        {
            var model = ProductCategoryManageQuery.GetDataUpdate(id);
            return PartialView("Component/_DialogUpdate", model);
        }

        [ValidateInput(false)]
        public ActionResult Update(ProductCategoryManageBO objRequest)
        {
            var model = ProductCategoryManageQuery.Update(objRequest);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int? id)
        {
            var model = ProductCategoryManageQuery.Delete(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}