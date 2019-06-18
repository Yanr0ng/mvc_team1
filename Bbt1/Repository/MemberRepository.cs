using Bbt1.ViewModel;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bbt1.Repository
{

    public class MemberRepository : BaseRepository
    {
        public IEnumerable<MemberViewModel> memberlist;
        public IEnumerable<MemberViewModel> SelectAll()
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

                memberlist = conn.Query<MemberViewModel>(sql);
            }            
            return memberlist;
        }        
    }
}