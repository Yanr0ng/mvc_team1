using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bbt1.Controllers
{
    public class DefaultController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var userName = Session["UserName"] as String;
            if (String.IsNullOrEmpty(userName))
            {
                //重定向至登入頁面
                filterContext.Result = RedirectToAction("Index", "Login", new { url = Request.RawUrl });
                return;
            }
        }
    }
}