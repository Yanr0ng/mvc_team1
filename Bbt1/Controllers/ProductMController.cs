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
using Bbt1.Repository;

namespace Bbt1.Controllers
{
    public class ProductMController : DefaultController
    {
        private ProductMRepository _repo = new ProductMRepository();


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
            var products = _repo.GetAllProduct().OrderByDescending(x => x.p_lauchdate).OrderBy(x => x.p_status).ToList();
            return View(products);
        }

        // POST: Products/Create     //--------------modal or new page?
        public ActionResult Create()
        {
            ViewBag.c_id = new SelectList(_repo.GetAllCate(), "c_id", "c_name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "p_id,p_name,p_unitprice,c_id")] Product product)
        {
            if (ModelState.IsValid)
            {
                _repo.Create(product);
                return RedirectToAction("Index");
            }
            ViewBag.c_id = new SelectList(_repo.GetAllCate(), "c_id", "c_name", product.c_id);
            return View("Create");
        }

        //編輯產品// POST: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = _repo.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.c_id = new SelectList(_repo.GetAllCate(), "c_id", "c_name", product.c_id);
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "p_id,p_name,p_unitprice,p_lauchdate,c_id,p_status")] Product product, int? id, string p_status, string p_name, decimal p_unitprice)
        {
            if (ModelState.IsValid)
            {
                _repo.EditProduct(id,p_status,p_name,p_unitprice);
                
                return RedirectToAction("Index");
            }
            return View(product);
        }

        //單-產品資訊
        public ActionResult ProductDetail(int? id)
        {
            var product_detail = _repo.GetAllPD(id);//db.Product_Detail.Include(p => p.Product).Where(x => x.p_id == id).ToList();
            var aa = _repo.GetAllProduct().Where(m => m.p_id == id).FirstOrDefault();
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
                _repo.CreatePD(id, pd);
                return RedirectToAction("ProductDetail", new { id });
            }

            ViewBag.p_id = new SelectList(_repo.GetAllProduct(), "p_id", "p_name");
            //ViewBag.c_id = new SelectList(db.Category, "c_id", "c_name");
            return View();
        }

        //產品圖片
        public ActionResult ProductPicture(int? id)
        {
            var pic = _repo.GetAllPP().Where(x => x.p_id == id).ToList();
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

                _repo.CreatePP(id,path);
            }
            return RedirectToAction("ProductPicture", "ProductM", new { id });
        }
        

        //產品特色
        public ActionResult ProductFeature(int? id)
        {
            var feature = _repo.GetAllPF().Where(x => x.p_id == id).ToList();
            ViewBag.feature_pname = _repo.GetAllProduct().Where(x => x.p_id == id).FirstOrDefault().p_name;
            ViewBag.feature_pid = _repo.GetAllProduct().Where(x => x.p_id == id).FirstOrDefault().p_id;
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

                _repo.CreatePF(id,pf);
                return RedirectToAction("ProductFeature", "ProductM", new { id });
            }
            return View(pf);
        }

        // GET: ProductClassifies
        //<span class="float-sm-right text-primary">
        public ActionResult ProductClassifies(int? id)
        {
            var classify = _repo.GetAllClassify().Where(x => x.p_id == id).ToList();
            ViewBag.p_name = _repo.GetAllProduct().Where(x => x.p_id == id).FirstOrDefault().p_name;
            ViewBag.p_id = _repo.GetAllProduct().Where(x => x.p_id == id).FirstOrDefault().p_id;
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
                _repo.CreateClassify(id, classify);
                return RedirectToAction("ProductClassifies", new { id });
            }
            return View(classify);
        }
        
        //折扣DISCOUNT
        public ActionResult Discount()
        {
            var discount = _repo.GetAllDiscount().OrderByDescending(m => m.d_startdate).ToList();
            return View(discount);
        }

        //Create Discount
        public ActionResult CreateDiscount()
        {
            ViewBag.c_id = new SelectList(_repo.GetAllCate(), "c_id", "c_name");
            return View();
        }
        [HttpPost]
        public ActionResult CreateDiscount([Bind(Include = "d_activity,d_startdate,d_enddate,c_name,d_discount,c_id,c_name")]Discount discount)
        {
            if (ModelState.IsValid == false)
            {                
                ViewBag.c_id = new SelectList(_repo.GetAllCate(), "c_id", "c_name");
                return View("CreateDiscount","_LayoutA");
            }
            _repo.CreateDiscount(discount);
            return RedirectToAction("Discount", "ProductM");
        }
        
        //--------------------刪除----------------------
        //刪除產品 實際上是p_status變更為8
        public ActionResult DeleteProduct(int? id)
        {
            _repo.ChangeStatus(id);
            return RedirectToAction("Index");
        }

        //刪除產品詳細
        public ActionResult DeleteProductDetail(int? id)
        {
            var pid= _repo.DeletePD(id);
            return RedirectToAction("ProductDetail", new { id = pid });
        }

        //刪除產品圖片
        public ActionResult DeleteProductPicture(int? id)
        {
            var pid =_repo.DeletePP(id);
            return RedirectToAction("ProductPicture", new { id = pid });
        }

        //刪除產品特色
        public ActionResult DeleteProductFeature(int? id)
        {
            var pid = _repo.DeletePF(id);
            return RedirectToAction("ProductFeature", new { id = pid });
        }

        //刪除產品規格
        public ActionResult DeleteProductClassifies(int? id)
        {
            var pid = _repo.DeletePC(id);
            return RedirectToAction("ProductClassifies", new { id = pid });
        }
        
        //刪除折扣活動
        public ActionResult DeleteDiscount(int? id)
        {
            _repo.DeleteDiscount(id);
            return RedirectToAction("Discount");
        }
    }
}