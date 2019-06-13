using Bbt1.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bbt1.Controllers
{
    public class OrderMController : Controller
    {
        MvcDataBaseEntities db = new MvcDataBaseEntities();

        private readonly string ConnString;
        private readonly SqlConnection conn;
        public OrderMController()
        {
            if (string.IsNullOrEmpty(ConnString))
            {
                ConnString = ConfigurationManager.ConnectionStrings["MvcDataBase"].ConnectionString;
            }
            conn = new SqlConnection(ConnString);
        }

        // GET: OrderM
        public ActionResult Index()
        {
            var order = db.Order.Where(x => x.o_status != 8.ToString()).ToList().OrderBy(x => x.o_status).ThenByDescending(x => x.o_delivedate);
            return View(order);
        }

        //查看訂單詳細
        public ActionResult OrderDetail(int? id)
        {
            using (conn)
            {
                string sql = "select o_receiver,p.p_name,pd.pd_color,od.od_quantity,od.od_price,od.od_discount," +
                    "(od.od_quantity*od.od_price*od.od_discount) as Total " +
                    "from [dbo].[Order_Detail] od " +
                    "inner join [dbo].[Product_Detail] pd on pd.pd_id = od.pd_id " +
                    "inner join [dbo].[Product] p on p.p_id = pd.p_id " +
                    "inner join [dbo].[Order] o on od.o_id = o.o_id " +
                    "where o.o_id=" + id;
                var order_detail = conn.Query(sql).ToList();
                ViewBag.order_detail = order_detail;
            }
            return View();
        }

        //出貨按鈕
        public ActionResult Ship(int? id)
        {
            var order = db.Order.Where(m => m.o_id == id).FirstOrDefault();
            order.o_status = 1.ToString();
            order.o_delivedate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //刪除一筆訂單 改訂單狀態為8
        public ActionResult DeleteOrder(int? id)
        {
            var o = db.Order.Where(x => x.o_id == id).FirstOrDefault();
            o.o_status = 8.ToString();
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //刪除一筆訂單詳細
        public ActionResult DeleteOrderDetail(int? id)
        {
            var od = db.Order_Detail.Where(x => x.od_id == id).FirstOrDefault();
            var oid = od.o_id;
            db.Order_Detail.Remove(od);
            db.SaveChanges();
            return RedirectToAction("OrderDetail", new { id = oid });
        }


        public ActionResult gggg()
        {
            var user = db.Member.FirstOrDefault((x) => x.m_email == "Kevin@gmail.com")?.m_id;
            var sc = db.Shopping_Cart.Where((x) => x.m_id == user);
            var pd = db.Product_Detail.Where((x) => sc.Any((y) => y.pd_id == x.pd_id));
            var p = db.Product.Where((x) => pd.Any((y) => y.p_id == x.p_id)).ToList();
            return null;
        }
    }
}