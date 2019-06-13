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
    public class MemberController : Controller
    {
        private readonly string ConnString;
        private readonly SqlConnection conn;
        public MemberController()
        {

            if (string.IsNullOrEmpty(ConnString))
            {
                ConnString = ConfigurationManager.ConnectionStrings["MvcDataBase"].ConnectionString;
            }
            conn = new SqlConnection(ConnString);

        }
        // GET: Member
        public ActionResult Index()
        {
            using (conn)
            {

                string sql = "SELECT m_name,m_email,(SELECT m_id FROM Member " +
                    "WHERE m_id = m.m_id) AS Id, (SELECT COUNT(m_id) FROM[dbo].[Order] " +
                    "WHERE m_id = m.m_id and o_status!=8 and o_status!=4) AS Count, " +
                    "SUM(od.od_quantity * od.od_price * (od.od_discount)), " +
                    "(select SUM(od.od_quantity * od.od_price * (od.od_discount)) " +
                    "from [dbo].[Order] o " +
                    "inner JOIN[dbo].[Order_Detail] od on od.o_id = o.o_id " +
                    "where o.o_status!=4 and o_status!=8 and o.m_id=m.m_id) " +
                    "AS Total " +
                    "FROM Member m " +
                    "LEFT OUTER JOIN[dbo].[Order] o on o.m_id = m.m_id " +
                    "LEFT JOIN[dbo].[Order_Detail] od on od.o_id = o.o_id " +
                    "GROUP BY m_name,m_email,m.m_id";

                var order = conn.Query(sql).ToList();
                ViewBag.od = order;
            }
            //List<Order> order;     --->之所以不能用是因為List<!!!> 要新的一個類別才能使用 新的類別要放我們查詢的欄位
            //string sql = "select m_name,m_email,COUNT(o.m_id) as Count from Member m left join [dbo].[Order] o on m.m_id = o.m_id group by m_name,m_email,m.m_id";
            //order = conn.Query<Order>(sql).ToList();
            //ViewBag.od = order;

            return View();
        }
    }
}