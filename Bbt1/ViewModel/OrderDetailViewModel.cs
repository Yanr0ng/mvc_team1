using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bbt1.ViewModel
{
    public class OrderDetailViewModel
    {
        //o_status,o_receiver,p.p_name,pd.pd_color,od.od_id,od.od_quantity,od.od_price,od.od_discount,Total
        public string o_status { get; set; }
        public string o_receiver { get; set; }
        public string p_name { get; set; }
        public string pd_color { get; set; }
        public int od_id { get; set; }
        public int od_quantity { get; set; }
        public Nullable<float> od_price { get; set; }

        public Nullable<float> od_discount { get; set; }
        
        public Nullable<float> Total { get; set; }
    }
}