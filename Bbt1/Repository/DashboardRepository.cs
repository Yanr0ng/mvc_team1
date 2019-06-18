using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bbt1.ViewModel;
using System.Data.SqlClient;
using Bbt1.Models;
using Bbt1.Repository;

namespace Bbt1.Repositories
{
    public class DashboardRepository:BaseRepository
    {
        public IEnumerable<D_CategorySellViewModel> selllist;
        public IEnumerable<D_HotItemsViewModel> itemslist;
        public IEnumerable<D_DailyEarningViewModel> daily;
        public IEnumerable<D_MonthlyEarningViewModel> monthly;
        public IEnumerable<D_SenvenDayViewModel> days;

        public int MemberCount()
        {
            return db.Member.Count();
        }

        public int OrderCount()
        {
            return db.Order.Count();
        }

        public int Ordernum()
        {
            return db.Order.Count(x => x.o_status == "0");
        }

        //category sell
        public IEnumerable<D_CategorySellViewModel> CategorySell()
        {
            using (conn)
            {
                string sql = "select c.c_name, sum(od.od_quantity) as sum " +
                    "from Category c " +
                    "inner join Product p on p.c_id = c.c_id " +
                    "inner join Product_Detail pd on pd.p_id = p.p_id " +
                    "inner join Order_Detail od on od.pd_id = pd.pd_id " +
                    "group by c.c_name " +
                    "order by sum desc";
                selllist = conn.Query<D_CategorySellViewModel>(sql);
            }
            return selllist;
        }
        
        //hot 3 item
        public IEnumerable<D_HotItemsViewModel> HotItems()
        {
            conn = new SqlConnection(connString);
            using (conn)
            {
                string sql2 = "select top(3) p.p_name,sum(od_quantity) as sum " +
                    "from Order_Detail od " +
                    "Inner join Product_Detail pd on od.pd_id = pd.pd_id " +
                    "inner join Product p on p.p_id = pd.p_id " +
                    "inner join Category c on c.c_id = p.c_id " +
                    "group by p.p_name order by sum desc";
                itemslist = conn.Query<D_HotItemsViewModel>(sql2);
            }
            return itemslist;
        }

        //daily earning
        public IEnumerable<D_DailyEarningViewModel> DailyEarning()
        {
            conn = new SqlConnection(connString);
            using (conn)
            {
                string sql3 = "select convert(date, o.o_date), " +
                   "cast(sum(od_price * od_quantity *od.od_discount) as int) as sum " +
                   "from Order_Detail od " +
                   "inner join [dbo].[Order] o on o.o_id = od.o_id " +
                   "where convert(date, o.o_date) = convert(date, GETDATE() ) " +
                   "group by convert(date, o.o_date)";
                daily = conn.Query<D_DailyEarningViewModel>(sql3);
            }
            return daily;
        }

        //monthly earning
        public IEnumerable<D_MonthlyEarningViewModel> MonthlyEarning()
        {
            conn = new SqlConnection(connString);
            using (conn)
            {
                //monthly earning
                string sql4 = "select  convert(date,o.o_date), " +
                    "cast (sum(od_price*od_quantity*od_discount) as int) as sum " +
                    "from Order_Detail od " +
                    "inner join [dbo].[Order] o on o.o_id = od.o_id " +
                    "where MONTH(o.o_date) = MONTH(GETDATE()) " +
                    "group by convert(date, o.o_date)";

                monthly = conn.Query<D_MonthlyEarningViewModel>(sql4);
            }
            return monthly;
        }

        //seven days
        public IEnumerable<D_SenvenDayViewModel> SevenDays()
        {
            conn = new SqlConnection(connString);
            using (conn)
            {
                
                string sql5 = "select top 7 convert(date, o.o_date) as day, " +
                    "sum(od.od_quantity * od.od_price*od_discount) as sum " +
                    "from Order_Detail od " +
                    "inner join [dbo].[Order] o on o.o_id = od.o_id " +
                    "group by convert(date, o.o_date) " +
                    "order by convert(date, o.o_date) desc";
                days = conn.Query<D_SenvenDayViewModel>(sql5);
            }
            return days;
        }
    }
}