using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bbt1.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        //public ActionResult Index()
        //{
        //    return View();
        //}
        public ActionResult Index(string ReturnUrl)
        {
            if (Session["UserName"] != null)
            {
                return RedirectToAction("Dashboard", "Dashboard");
            }
            ViewBag.Url = ReturnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Index(string name, string password, string returnUrl)
        {
            /*
            新增驗證使用者名稱密碼程式碼
            */
            if (name == "admin" && password == "admin")
            {
                Session["UserName"] = "admin";
                //Session["UserName"] = name;
                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Dashboard", "Dashboard");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
           
        }
        //// POST: /Account/LogOff
        //[HttpPost]
        public ActionResult LogOff()
        {
            Session["UserName"] = null;
            return RedirectToAction("Index");
        }
    }
}