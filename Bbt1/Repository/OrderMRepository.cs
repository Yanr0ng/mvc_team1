using Bbt1.Models;
using Bbt1.ViewModel;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bbt1.Repository
{
    public class OrderMRepository : BaseRepository
    {
        public IEnumerable<OrderDetailViewModel> order_detail;
        public IEnumerable<OrderDetailViewModel> SelectAll(int? id)
        {
            using (conn)
            {
                string sql = "select o_status,o_receiver,p.p_name,pd.pd_color,od.od_id,od.od_quantity,od.od_price,od.od_discount," +
                    "(od.od_quantity*od.od_price*od.od_discount) as Total " +
                    "from [dbo].[Order_Detail] od " +
                    "inner join [dbo].[Product_Detail] pd on pd.pd_id = od.pd_id " +
                    "inner join [dbo].[Product] p on p.p_id = pd.p_id " +
                    "inner join [dbo].[Order] o on od.o_id = o.o_id " +
                    "where o.o_id=" + id;
                order_detail = conn.Query<OrderDetailViewModel>(sql).ToList();

            }
            return order_detail;
        }
        public int OrderCount()
        {
            return db.Order.Count(x => x.o_status == "0");
        }

        public IEnumerable<Order> GetAllOrder()
        {
            return db.Order;

        }
        public IEnumerable<Member> GetAllMember()
        {
            return db.Member;
        }

        public bool OrderError(int? id)
        {
            var o = db.Order.Where(x => x.o_id == id).FirstOrDefault();
            o.o_status = "8";
            db.SaveChanges();
            return true;
        }

        public int? DeleteOrderDetail(int? id)
        {
            var od = db.Order_Detail.Where(x => x.od_id == id).FirstOrDefault();
            var oid = od.o_id;
            db.Order_Detail.Remove(od);
            db.SaveChanges();
            return oid;
        }
        public Order ChangeStatus(int? id)
        {
            var order = db.Order.Where(m => m.o_id == id).FirstOrDefault();
            order.o_status = 1.ToString();
            order.o_delivedate = DateTime.Now;
            db.SaveChanges();
            return order;
        }
    }
}