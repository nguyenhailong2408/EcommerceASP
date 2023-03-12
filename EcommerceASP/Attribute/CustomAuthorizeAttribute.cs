using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcommerceASP.Attribute
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var routeData = httpContext.Request.RequestContext.RouteData;
            var controller = routeData.GetRequiredString("controller");
            var action = routeData.GetRequiredString("action");
            var user = System.Web.HttpContext.Current.User;
            return base.AuthorizeCore(httpContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var context = filterContext.HttpContext;
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            string url = null;
            
            if(string.IsNullOrEmpty(url))
            {
                url = urlHelper.Action("Permission", "Error", new { Area = "" });
            }

            // [NEED_TO_TRANS]Check Ajax error
            if (context.Request.IsAjaxRequest())
            {
                var result = new JavaScriptResult();
                result.Script = "Common.RedirectLoginUrl('" + url + "')";
                filterContext.Result = result;
            }
            else
            {
                filterContext.Result = new RedirectResult(url);
            }
        }
    }
}