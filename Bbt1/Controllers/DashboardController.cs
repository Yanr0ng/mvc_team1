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

namespace Bbt1.Controllers
{
    public class DashboardController : Controller
    {
        MvcDataBaseEntities db = new MvcDataBaseEntities();
        private readonly string ConnString;
        private readonly SqlConnection conn;
        public DashboardController()
        {

            if (string.IsNullOrEmpty(ConnString))
            {
                ConnString = ConfigurationManager.ConnectionStrings["MvcDataBase"].ConnectionString;
            }
            conn = new SqlConnection(ConnString);
            
        }


        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            Session["ordernum"] = db.Order.Count(x => x.o_status == "0");
            
            ViewData["Order"] = db.Order.Count();     //--->訂單總數量
            ViewData["Members"] = db.Member.Count();  //--->會員總數量
            using (conn)
            {
                //cat sell
                string sql = "select c.c_name, sum(od.od_quantity) as sum " +
                    "from Category c " +
                    "inner join Product p on p.c_id = c.c_id " +
                    "inner join Product_Detail pd on pd.p_id = p.p_id " +
                    "inner join Order_Detail od on od.pd_id = pd.pd_id " +
                    "group by c.c_name " +
                    "order by sum desc";
                //hot 3 item
                string sql2 = "select top(3) p.p_name,sum(od_quantity) as sum " +
                    "from Order_Detail od " +
                    "Inner join Product_Detail pd on od.pd_id = pd.pd_id " +
                    "inner join Product p on p.p_id = pd.p_id " +
                    "inner join Category c on c.c_id = p.c_id " +
                    "group by p.p_name order by sum desc";
                //daily earning
                string sql3 = "select convert(date, o.o_date), " +
                    "cast(sum(od_price * od_quantity ) as int) as sum " +
                    "from Order_Detail od " +
                    "inner join [dbo].[Order] o on o.o_id = od.o_id " +
                    "where convert(date, o.o_date) = convert(date, GETDATE() ) " +
                    "group by convert(date, o.o_date)";

                //monthly earning
                string sql4 = "select  convert(date,o.o_date), " +
                    "cast (sum(od_price*od_quantity) as int) as sum " +
                    "from Order_Detail od " +
                    "inner join [dbo].[Order] o on o.o_id = od.o_id " +
                    "where MONTH(o.o_date) = MONTH(GETDATE()) " +
                    "group by convert(date, o.o_date)";

                string sql5 = "select top 7 convert(date, o.o_date) as day, " +
                    "sum(od.od_quantity * od.od_price) as sum " +
                    "from Order_Detail od " +
                    "inner join [dbo].[Order] o on o.o_id = od.o_id " +
                    "group by convert(date, o.o_date) " +
                    "order by convert(date, o.o_date) desc";
                
                var hotcat = conn.Query(sql).ToList();
                var hotitem = conn.Query(sql2).ToList();
                var dailyearning = conn.Query(sql3).FirstOrDefault();
                var monthlyearning = conn.Query(sql4).ToList();
                var sevendays = conn.Query(sql5).OrderBy(x=>x.day).ToList();
                ViewBag.hot3 = hotcat;
                ViewBag.hot3item = hotitem;
                ViewBag.dailyearning = dailyearning;
                ViewBag.monthlyearning = monthlyearning;
                ViewBag.sevendays = sevendays;

            }
            return View();
        }
    }
}