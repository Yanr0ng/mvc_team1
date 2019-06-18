using Bbt1.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Bbt1.Repositories;

namespace Bbt1.Controllers
{
    public class DashboardController : Controller
    {
        private DashboardRepository _repo = new DashboardRepository();

        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            Session["ordernum"] = _repo.Ordernum();
            ViewData["Order"] = _repo.OrderCount();     //--->訂單總數量
            ViewData["Members"] = _repo.MemberCount();  //--->會員總數量
            
            ViewBag.hot3 = _repo.CategorySell().ToList();   //每個分類的銷售量
            ViewBag.hot3item = _repo.HotItems().ToList();   //TOP3產品銷售量
            ViewBag.dailyearning = _repo.DailyEarning().ToList();   //日收
            ViewBag.monthlyearning = _repo.MonthlyEarning().ToList();   //月收
            ViewBag.sevendays = _repo.SevenDays().OrderBy(x => x.day).ToList(); //七天收入
            
            return View();
        }
    }
}