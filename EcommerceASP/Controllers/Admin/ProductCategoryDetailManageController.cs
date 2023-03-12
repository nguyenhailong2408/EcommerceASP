using EcommerceASP.Queries;
using EcommerceASP.ViewModel.ProductCategoryDetailManage;
using System.Web.Mvc;

namespace EcommerceASP.Controllers.Admin
{
    [Authorize]
    public class ProductCategoryDetailManageController : Controller
    {
        // GET: ProductCategoryDetailManage
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList(SearchFormViewModel objRequest)
        {
            var model = ProductCategoryDetailManageQuery.GetListData(objRequest);
            return PartialView("Component/_ListData", model);
        }

        public ActionResult Dialog(int? id)
        {
            var model = ProductCategoryDetailManageQuery.GetDataUpdate(id);
            return PartialView("Component/_DialogUpdate", model);
        }

        [ValidateInput(false)]
        public ActionResult Update(ProductCategoryDetailManageBO objRequest)
        {
            var model = ProductCategoryDetailManageQuery.Update(objRequest);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int? id)
        {
            var model = ProductCategoryDetailManageQuery.Delete(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}