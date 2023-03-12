using EcommerceASP.Queries;
using EcommerceASP.ViewModel.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcommerceASP.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
        public ActionResult Index()
        {
            ContactBO contactBO = new ContactBO();
            return View(contactBO);
        }
        public ActionResult Update(ContactBO objRequest)
        {
            var data = ContactQuery.Update(objRequest);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}