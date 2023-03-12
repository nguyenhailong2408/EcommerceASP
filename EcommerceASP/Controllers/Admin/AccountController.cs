using EcommerceASP.Queries;
using EcommerceASP.ViewModel.Account;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;

namespace EcommerceASP.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login", "Account");
        }
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User != null && User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    returnUrl = Regex.Replace(returnUrl, "#", "%23", RegexOptions.Multiline);
                    ViewBag.ReturnUrl = returnUrl;
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
                
            return View("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginBO model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = AccountQuery.LoginUser(model);
            if (result.Status)
            {
                FormsAuthentication.SetAuthCookie(model.Username, false);
                return RedirectToAction("Login", "Account", new { returnUrl });
            }
            else
            {
                ModelState.AddModelError("", result.Message);
            }

            return View(model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}