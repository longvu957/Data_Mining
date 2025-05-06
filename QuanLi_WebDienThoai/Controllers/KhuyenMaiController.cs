using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLi_WebDienThoai.Models;

namespace QuanLi_WebDienThoai.Controllers
{
    public class KhuyenMaiController : Controller
    {
        CSDLDataContext data = new CSDLDataContext();
        // GET: KhuyenMai
        public ActionResult QuanLiKhuyenMai()
        {
            //List<ChiTiet_KhuyenMai> ds = data.ChiTiet_KhuyenMais.ToList();
            var ds = data.ChiTiet_KhuyenMais
                 // Bao gồm bảng KhuyenMai
                .ToList();
            return View(ds);
        }
        public ActionResult ThemKhuyenMai()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemKhuyenMai(KhuyenMai model)
        {
            data.KhuyenMais.InsertOnSubmit(model);
            data.SubmitChanges();
            return RedirectToAction("Index","Admins");
        }

        public ActionResult ThemChiTietKM()
        {
            ViewBag.SanPham_id = new SelectList(data.San_Phams.OrderBy(t => t.id), "id", "Name");
            ViewBag.KhuyenMai_id = new SelectList(data.KhuyenMais.OrderBy(t => t.id), "id", "Name");
            return View(new ChiTiet_KhuyenMai());
        }    
        [HttpPost]
        public ActionResult ThemChiTietKM(ChiTiet_KhuyenMai model)
        {
            data.ChiTiet_KhuyenMais.InsertOnSubmit(model);
            data.SubmitChanges();
            return RedirectToAction("Index", "Admins");
        }
    }
}