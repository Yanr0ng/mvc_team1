using Bbt1.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Bbt1.Repository
{
    public class BaseRepository
    {
        public static string connString;
        public SqlConnection conn;
        public MvcDataBaseEntities db;

        public BaseRepository()
        {
            db = new MvcDataBaseEntities();
            if (string.IsNullOrEmpty(connString))
            {
                connString = ConfigurationManager.ConnectionStrings["MvcDataBase"].ConnectionString;
            }
            if (conn == null)
            {
                conn = new SqlConnection(connString);
            }
        }
    }
}