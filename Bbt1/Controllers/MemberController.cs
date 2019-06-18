using Bbt1.Repository;
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
        private MemberRepository _Mrepo = new MemberRepository();
        
        // GET: Member
        public ActionResult Index()
        {
            ViewBag.od = _Mrepo.SelectAll().ToList();
            return View();
        }
    }
}