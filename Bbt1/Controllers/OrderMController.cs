using Bbt1.Models;
using Dapper;
using Rotativa;
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
            var order = db.Order.Where(x => x.o_status != 8.ToString()).ToList().OrderBy(x => x.o_status).ThenByDescending(x => x.o_delivedate).ThenByDescending(x => x.o_date);



            return View(order);
        }

        //查看訂單詳細
        public ActionResult OrderDetail(int? id)
        {
            var order = db.Order.Where((x)=> x.o_id == id).FirstOrDefault();

            using (conn)
            {
                string sql = "select o_status,o_receiver,p.p_name,pd.pd_color,od.od_id,od.od_quantity,od.od_price,od.od_discount," +
                    "(od.od_quantity*od.od_price*od.od_discount) as Total " +
                    "from [dbo].[Order_Detail] od " +
                    "inner join [dbo].[Product_Detail] pd on pd.pd_id = od.pd_id " +
                    "inner join [dbo].[Product] p on p.p_id = pd.p_id " +
                    "inner join [dbo].[Order] o on od.o_id = o.o_id " +
                    "where o.o_id=" + id;
                var order_detail = conn.Query(sql).ToList();
                ViewBag.order_detail = order_detail;
            }
            ViewBag.order = order;
            return View();
        }

        //出貨按鈕    //寄信給使用者
        public ActionResult Ship(int? id)
        {
            var order = db.Order.Where(m => m.o_id == id).FirstOrDefault();
            order.o_status = 1.ToString();
            order.o_delivedate = DateTime.Now;
            db.SaveChanges();

            var uid = db.Member.Where(x => x.m_id == order.m_id).FirstOrDefault().m_email_id;   //guid產出的不重複代號
            var email = db.Member.Where(x => x.m_id == order.m_id).FirstOrDefault().m_email;    //收信人的email
            string a = Convert.ToString(uid);

            string cont;
            cont = "http://" + Request.Url.Authority + "/Member/result?uid=" + a;  //訂單的網址
            System.Net.Mail.MailMessage MyMail = new System.Net.Mail.MailMessage();//建立MAIL   
            MyMail.From = new System.Net.Mail.MailAddress("gurutw201905@gmail.com", "Guru");//寄信人   
            MyMail.To.Add(new System.Net.Mail.MailAddress(email));//收信人1     
            MyMail.Subject = "商品已出貨成功！";//主題   
            MyMail.Body = "您好\n\n您的訂單已出貨，謝謝你的支持\n\n以下網址能夠返回網站看到此筆出貨的訂單\n\n" + cont + "\n\n 提醒您！請務必在指定時間內完成取貨，逾期未取，商品將會自動退貨！\n\n GuruTW 團隊 敬上";//內容   
            System.Net.Mail.SmtpClient Client = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);//GMAIL主機   
                                                                                                      //System.Net.Mail.SmtpClient Client = new System.Net.Mail.SmtpClient("msa.hinet.net");//hinet主機   
            Client.Credentials = new System.Net.NetworkCredential("gurutw201905@gmail.com", "wearethe@1");//帳密，Hinet不用但須在它的ADLS(區段)裡面   
            Client.EnableSsl = true;//Gmail需啟動SSL，Hinet不用   
            Client.Send(MyMail);//寄出

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

        //訂單轉變PDF檔
        public ActionResult ExportPDF(int id)
        {
            //UrlAsPdf pdf = new UrlAsPdf("https://gurutwadmin.azurewebsites.net/OrderM/OrderDetail/" + id); 
            ////ActionAsPdf pdf = new ActionAsPdf("Dashboard");
            //pdf.FileName = "OrderDetail.pdf";
            //pdf.PageSize = Rotativa.Options.Size.A4;
            //return pdf;

            return new ActionAsPdf("OrderDetail",new { id })
            {
                FileName = Server.MapPath("OrderDetail.pdf"),
                PageSize = Rotativa.Options.Size.A4
            };
        }

    }
}