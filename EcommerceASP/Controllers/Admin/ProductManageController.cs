using EcommerceASP.Queries;
using EcommerceASP.ViewModel.Base;
using EcommerceASP.ViewModel.ProductManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcommerceASP.Controllers.Admin
{
    [Authorize]
    public class ProductManageController : Controller
    {
        // GET: ProductManage
        public ActionResult Index()
        {
            var search = new SearchFormViewModel();
            return View(search);
        }
        public ActionResult GetList(SearchFormViewModel objRequest)
        {
            var model = ProductManageQuery.GetListData(objRequest);
            return PartialView("Component/_ListData", model);
        }
        public ActionResult Dialog(int? id)
        {
            var model = ProductManageQuery.GetDataUpdate(id);
            return PartialView("Component/_DialogUpdate", model);
        }

        [ValidateInput(false)]
        public ActionResult Update(ProductManageBO objRequest)
        {
            var model = ProductManageQuery.Update(objRequest);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int? id)
        {
            var model = ProductManageQuery.DeleteProduct(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}