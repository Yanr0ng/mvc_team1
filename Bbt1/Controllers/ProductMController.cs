using Bbt1.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Data.Entity;    //lambda轉為string
using System.Web.Mvc;
using System.IO;
using System.Net;

namespace Bbt1.Controllers
{
    public class ProductMController : DefaultController
    {
        MvcDataBaseEntities db = new MvcDataBaseEntities();

        private readonly string ConnString;
        private readonly SqlConnection conn;
        public ProductMController()
        {
            if (string.IsNullOrEmpty(ConnString))
            {
                ConnString = ConfigurationManager.ConnectionStrings["MvcDataBase"].ConnectionString;
            }
            conn = new SqlConnection(ConnString);
        }

        // GET: ProductM 所有產品
        public ActionResult Index()
        {
            var products = db.Product.ToList().OrderByDescending(x => x.p_lauchdate).OrderBy(x => x.p_status);
            return View(products);
        }

        // POST: Products/Create     //--------------modal or new page?
        public ActionResult Create()
        {
            ViewBag.c_id = new SelectList(db.Category, "c_id", "c_name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "p_id,p_name,p_unitprice,c_id")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.p_lauchdate = DateTime.Now;
                product.p_status = "0";
                db.Product.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.c_id = new SelectList(db.Category, "c_id", "c_name", product.c_id);
            return View(product);
        }

        //編輯產品// POST: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.c_id = new SelectList(db.Category, "c_id", "c_name", product.c_id);
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "p_id,p_name,p_unitprice,p_lauchdate,c_id,p_status")] Product product, int? id, string p_status, string p_name, decimal p_unitprice)
        {
            if (ModelState.IsValid)
            {
                var p = db.Product.Where(m => m.p_id == id).FirstOrDefault();
                p.c_id = p.c_id;
                p.p_status = p_status;
                p.p_name = p_name;
                p.p_unitprice = p_unitprice;
                p.p_lauchdate = p.p_lauchdate;
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.c_id = new SelectList(db.Category, "c_id", "c_name", product.c_id);
            return View(product);
        }

        //單-產品資訊
        public ActionResult ProductDetail(int? id)
        {
            var product_detail = db.Product_Detail.Include(p => p.Product).Where(x => x.p_id == id).ToList();
            var aa = db.Product.Where(m => m.p_id == id).FirstOrDefault();
            ViewBag.p_name = aa.p_name;
            ViewBag.pd_id = id;
            return View(product_detail);

        }

        //新增該產品的其他產品資訊
        public ActionResult CreateNewDetail(int? id)
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateNewDetail(int? id, [Bind(Include = "pd_id,pd_color,pd_stock")]Product_Detail pd)
        {
            if (ModelState.IsValid)
            {
                pd.p_id = id;
                pd.pd_onorder = 0;
                db.Product_Detail.Add(pd);
                db.SaveChanges();
                return RedirectToAction("ProductDetail", new { id });
            }

            ViewBag.p_id = new SelectList(db.Product, "p_id", "p_name");
            //ViewBag.c_id = new SelectList(db.Category, "c_id", "c_name");
            return View();
        }

        //產品圖片
        public ActionResult ProductPicture(int? id)
        {
            var pic = db.Product_Picture.Where(x => x.p_id == id).ToList();
            Session["product_pic"] = id;
            return View(pic);
        }

        //新增產品圖片
        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            var id = int.Parse(Session["product_pic"].ToString());

            if (file != null)
            {
                string pic = System.IO.Path.GetFileName(file.FileName);
                string path = "/Assets/Images/product_item_img/" + pic;
                //string time = DateTime.Now.ToString("yyyyMMdd_HHmmssms");
                //string picc = pic_part[0] + "_" + time + "." + pic_part[1];
                //string path = System.IO.Path.Combine(Server.MapPath("/Assets/images"), pic);
                //file.SaveAs(path);
                //using (MemoryStream ms = new MemoryStream())
                //{
                //    file.InputStream.CopyTo(ms);
                //    byte[] array = ms.GetBuffer();
                //}

                Product_Picture pp = new Product_Picture();
                pp.pp_path = path;
                pp.p_id = id;
                db.Product_Picture.Add(pp);
                db.SaveChanges();
            }
            return RedirectToAction("ProductPicture", "ProductM", new { id });
        }

        //----------------------------
        public ActionResult ProductDetailPopup(int? id)
        {
            var product_detail = db.Product_Detail.Include(p => p.Product).Where(x => x.p_id == id).ToList();
            return View(product_detail);
        }

        //產品特色
        public ActionResult ProductFeature(int? id)
        {
            var feature = db.Product_Feature.Where(x => x.p_id == id).ToList();
            ViewBag.feature_pname = db.Product.Where(x => x.p_id == id).FirstOrDefault().p_name;
            ViewBag.feature_pid = db.Product.Where(x => x.p_id == id).FirstOrDefault().p_id;
            return View(feature);
        }

        //新增產品特色
        public ActionResult AddFeature(int? id)
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddFeature(int? id, [Bind(Include = "pf_id,p_id,pf_description,pf_note")] Product_Feature pf)
        {
            if (ModelState.IsValid)
            {
                pf.p_id = id;
                db.Product_Feature.Add(pf);
                db.SaveChanges();
                return RedirectToAction("ProductFeature", "ProductM", new { id = id });
            }
            return View(pf);
        }

        // GET: ProductClassifies
        //<span class="float-sm-right text-primary">
        public ActionResult ProductClassifies(int? id)
        {
            var classify = db.Classify.Include(c => c.Product).Where(x => x.p_id == id).ToList();
            ViewBag.p_name = db.Product.Where(x => x.p_id == id).FirstOrDefault().p_name;
            ViewBag.p_id = db.Product.Where(x => x.p_id == id).FirstOrDefault().p_id;
            return View(classify);
        }

        //Classifies/Create
        public ActionResult CreateClassifies(int? id)
        {
            //ViewBag.p_id = new SelectList(db.Product, "p_id", "p_name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateClassifies(int? id, [Bind(Include = "cl_id,cl_specs,cl_content,p_id")] Classify classify)
        {
            if (ModelState.IsValid)
            {
                classify.p_id = id;
                db.Classify.Add(classify);
                db.SaveChanges();
                return RedirectToAction("ProductClassifies", new { id = id });
            }
            return View(classify);
        }

        //折扣DISCOUNT
        public ActionResult Discount()
        {
            var discount = db.Discount.OrderByDescending(m => m.d_startdate).ToList();
            return View(discount);
        }

        //Create Discount
        public ActionResult CreateDiscount()
        {
            ViewBag.c_id = new SelectList(db.Category, "c_id", "c_name");
            return View();
        }
        [HttpPost]
        public ActionResult CreateDiscount([Bind(Include = "d_activity,d_startdate,d_enddate,c_name,d_discount,c_id,c_name")]Discount discount)
        {
            if (ModelState.IsValid)
            {
                db.Discount.Add(discount);
                db.SaveChanges();
                return RedirectToAction("Discount", "ProductM");
            }
            return View(discount);
        }

        //--------------------刪除----------------------
        //刪除產品 實際上是p_status變更為8
        public ActionResult DeleteProduct(int? id)
        {
            var product = db.Product.Where(x => x.p_id == id).FirstOrDefault();
            product.p_status = "8";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //刪除產品詳細
        public ActionResult DeleteProductDetail(int? id)
        {
            var pd = db.Product_Detail.Where(x => x.pd_id == id).FirstOrDefault();
            var pid = pd.p_id;
            db.Product_Detail.Remove(pd);
            db.SaveChanges();
            return RedirectToAction("ProductDetail", new { id = pid });
        }

        //刪除產品圖片
        public ActionResult DeleteProductPicture(int? id)
        {
            var pp = db.Product_Picture.Where(x => x.pp_id == id).FirstOrDefault();
            var pid = pp.p_id;
            db.Product_Picture.Remove(pp);
            db.SaveChanges();
            return RedirectToAction("ProductPicture", new { id = pid });
        }

        //刪除產品特色
        public ActionResult DeleteProductFeature(int? id)
        {
            var pf = db.Product_Feature.Where(x => x.pf_id == id).FirstOrDefault();
            var pid = pf.p_id;
            db.Product_Feature.Remove(pf);
            db.SaveChanges();
            return RedirectToAction("ProductFeature", new { id = pid });
        }

        //刪除產品規格
        public ActionResult DeleteProductClassifies(int? id)
        {
            var pc = db.Classify.Where(x => x.cl_id == id).FirstOrDefault();
            var pid = pc.p_id;
            db.Classify.Remove(pc);
            db.SaveChanges();
            return RedirectToAction("ProductClassifies", new { id = pid });
        }
        
        //刪除折扣活動
        public ActionResult DeleteDiscount(int? id)
        {
            var discount = db.Discount.Where(x => x.d_id == id).FirstOrDefault();
            db.Discount.Remove(discount);
            db.SaveChanges();
            return RedirectToAction("Discount");
        }
    }
}