using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bbt1.ViewModel
{
    public class MemberViewModel
    {
        //m_name,m_email,Count,Total
        public string m_name { get; set; }
        public string m_email { get; set; }

        public Nullable<int> Count { get; set; }
        public Nullable<float> Total { get; set; }
        public List<MemberViewModel> data { get; internal set; }
    }
}