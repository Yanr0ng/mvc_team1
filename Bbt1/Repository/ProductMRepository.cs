using Bbt1.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Bbt1.Repository
{
    public class ProductMRepository:BaseRepository
    {
        public IEnumerable<Product> GetAllProduct()
        {
            return db.Product;
        }

        public IEnumerable<Category> GetAllCate()
        {
            return db.Category;
        }
        public void Create(Product product)
        {
            product.p_lauchdate = DateTime.Now;
            product.p_status = "0";
            db.Product.Add(product);
            db.SaveChanges();
        }

        public Product Find(int? id)
        {
            return db.Product.Find(id);
        }

        public void EditProduct(int? id, string p_status, string p_name, decimal p_unitprice)
        {
            var p = db.Product.Where(m => m.p_id == id).FirstOrDefault();
            p.c_id = p.c_id;
            p.p_status = p_status;
            p.p_name = p_name;
            p.p_unitprice = p_unitprice;
            p.p_lauchdate = p.p_lauchdate;
            db.Entry(p).State = EntityState.Modified;
            db.SaveChanges();
        }

        public IEnumerable<Product_Detail> GetAllPD(int? id)
        {
            return db.Product_Detail.Include(p => p.Product).Where(x => x.p_id == id);
        }

        public void CreatePD(int? id, Product_Detail pd)
        {
            pd.p_id = id;
            pd.pd_onorder = 0;
            db.Product_Detail.Add(pd);
            db.SaveChanges();
        }

        public IEnumerable<Product_Picture> GetAllPP()
        {
            return db.Product_Picture;
        }
        
        public void CreatePP(int id, string path)
        {
            Product_Picture pp = new Product_Picture
            {
                pp_path = path,
                p_id = id
            };
            db.Product_Picture.Add(pp);
            db.SaveChanges();
        }

        public IEnumerable<Product_Feature> GetAllPF()
        {
            return db.Product_Feature;
        }

        public void CreatePF(int? id, Product_Feature pf)
        {
            pf.p_id = id;
            db.Product_Feature.Add(pf);
            db.SaveChanges();
        }

        public IEnumerable<Classify> GetAllClassify()
        {
            return db.Classify.Include(c => c.Product);
        }

        public void CreateClassify(int? id,Classify classify)
        {
            classify.p_id = id;
            db.Classify.Add(classify);
            db.SaveChanges();
        }
        public IEnumerable<Discount> GetAllDiscount()
        {
            return db.Discount;
        }

        public void CreateDiscount(Discount discount)
        {
            db.Discount.Add(discount);
            db.SaveChanges();
        }

        public void ChangeStatus(int? id)
        {
            var product = db.Product.Where(x => x.p_id == id).FirstOrDefault();
            product.p_status = "8";
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
        }

        public int? DeletePD(int? id)
        {
            var pd = db.Product_Detail.Where(x => x.pd_id == id).FirstOrDefault();
            var pid = pd.p_id;
            db.Product_Detail.Remove(pd);
            db.SaveChanges();
            return pid;
        }

        public int? DeletePP(int? id)
        {
            var pp = db.Product_Picture.Where(x => x.pp_id == id).FirstOrDefault();
            var pid = pp.p_id;
            db.Product_Picture.Remove(pp);
            db.SaveChanges();
            return pid;
        }

        public int? DeletePF(int? id)
        {
            var pf = db.Product_Feature.Where(x => x.pf_id == id).FirstOrDefault();
            var pid = pf.p_id;
            db.Product_Feature.Remove(pf);
            db.SaveChanges();
            return pid;
        }
        public int? DeletePC(int? id)
        {
            var pc = db.Classify.Where(x => x.cl_id == id).FirstOrDefault();
            var pid = pc.p_id;
            db.Classify.Remove(pc);
            db.SaveChanges();
            return pid;
        }

        public void DeleteDiscount(int? id)
        {
            var discount = db.Discount.Where(x => x.d_id == id).FirstOrDefault();
            db.Discount.Remove(discount);
            db.SaveChanges();
        }
    }
}