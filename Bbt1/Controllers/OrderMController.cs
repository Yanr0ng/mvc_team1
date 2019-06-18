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
using PagedList;
using Bbt1.Repository;

namespace Bbt1.Controllers
{
    public class OrderMController : Controller
    {
        private OrderMRepository _OMrepo = new OrderMRepository();

        // GET: OrderM
        public ActionResult Index(string SortOrder, int? page)
        {
            Session["ordernum"] = _OMrepo.OrderCount();           
            ViewBag.name = String.IsNullOrEmpty(SortOrder) ? "name_desc" : "";
            ViewBag.receive = SortOrder == "receive" ? "receive_desc" : "receive";
            ViewBag.address = SortOrder == "address" ? "address_desc" : "address";
            ViewBag.dway = SortOrder == "dway" ? "dway_desc" : "dway";
            ViewBag.pay = SortOrder == "pay" ? "receive_desc" : "pay";
            ViewBag.odate = SortOrder == "odate" ? "odate_desc" : "odate";
            ViewBag.ddate = SortOrder == "ddate" ? "ddate_desc" : "ddate";
            ViewBag.status = SortOrder == "status" ? "status_desc" : "status";
            var order = _OMrepo.GetAllOrder();
            switch (SortOrder)
            {
                case "name_desc":
                    order = order.OrderByDescending(s => s.Member.m_name).ToList();
                    break;
                case "address":
                    order = order.OrderBy(s => s.o_address).ToList();
                    break;
                case "address_desc":
                    order = order.OrderByDescending(s => s.o_address).ToList();
                    break;
                case "receive":
                    order = order.OrderBy(s => s.o_receiver).ToList();
                    break;
                case "receive_desc":
                    order = order.OrderByDescending(s => s.o_receiver).ToList();
                    break;
                case "dway":
                    order = order.OrderBy(s => s.Delive_Way.dw_name).ToList();
                    break;
                case "dway_desc":
                    order = order.OrderByDescending(s => s.Delive_Way.dw_name).ToList();
                    break;
                case "pay":
                    order = order.OrderBy(s => s.Payment.pay_name).ToList();
                    break;
                case "pay_desc":
                    order = order.OrderByDescending(s => s.Payment.pay_name).ToList();
                    break;
                case "odate":
                    order = order.OrderBy(s => s.o_date).ToList();
                    break;
                case "odate_desc":
                    order = order.OrderByDescending(s => s.o_date).ToList();
                    break;
                case "ddate":
                    order = order.OrderBy(s => s.o_delivedate).ToList();
                    break;
                case "ddate_desc":
                    order = order.OrderByDescending(s => s.o_delivedate).ToList();
                    break;
                case "status":
                    order = order.OrderBy(s => s.o_status).ToList();
                    break;
                case "status_desc":
                    order = order.OrderByDescending(s => s.o_status).ToList();
                    break;
                default:
                    order = order.OrderByDescending(s => s.o_date).ToList();
                    break;
            }
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            return View(order.ToPagedList(pageNumber, pageSize));
        }

        //查看訂單詳細
        public ActionResult OrderDetail(int? id)
        {
            var order = _OMrepo.GetAllOrder().Where((x) => x.o_id == id).FirstOrDefault();

            ViewBag.order_detail = _OMrepo.SelectAll(id).ToList();

            ViewBag.order = order;
            return View();
        }

        //出貨按鈕    //寄信給使用者
        public ActionResult Ship(int? id)
        {
            //var order = _OMrepo.GetAllOrder().Where(m => m.o_id == id).FirstOrDefault();
            //order.o_status = 1.ToString();
            //order.o_delivedate = DateTime.Now;
            //db.SaveChanges();
            var order = _OMrepo.ChangeStatus(id);

            var uid = _OMrepo.GetAllMember().Where(x => x.m_id == order.m_id).FirstOrDefault().m_email_id;   //guid產出的不重複代號
            var email = _OMrepo.GetAllMember().Where(x => x.m_id == order.m_id).FirstOrDefault().m_email;    //收信人的email
            string a = Convert.ToString(uid);

            string cont;
            cont = "https://gurutwmvc.azurewebsites.net" + "/Member/result?uid=" + a;  //訂單的網址
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
            _OMrepo.OrderError(id);
            return RedirectToAction("Index");
        }

        //刪除一筆訂單詳細
        public ActionResult DeleteOrderDetail(int? id)
        {
            var oid =_OMrepo.DeleteOrderDetail(id);
            return RedirectToAction("OrderDetail", new { id = oid });
        }

        //訂單轉PDF檔
        public ActionResult ExportPDF(int id)
        {
            return new ActionAsPdf("OrderDetail", new { id })
            {
                FileName = Server.MapPath("OrderDetail.pdf"),
                PageSize = Rotativa.Options.Size.A4
            };
        }

    }
}