using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLi_WebDienThoai.Models;

namespace QuanLi_WebDienThoai.Controllers
{
    public class DatHangController : Controller
    {
        Models.CSDLDataContext db = new Models.CSDLDataContext();
        CSDLDataContext data = new CSDLDataContext();

        public ActionResult HienThiAnh(int id)
        {
            List<Image> ds = data.Images.Where(t => t.SanPham_id == id).Take(2).ToList();
            ViewBag.Anh1 = ds[0].Name;
            ViewBag.Anh2 = ds[1].Name;
            return PartialView();
        }
        public ActionResult HienThiAnh2(int id)
        {
            List<Image> ds = data.Images.Where(t => t.SanPham_id == id).Take(2).ToList();
            ViewBag.Anh1 = ds[0].Name;
            ViewBag.Anh2 = ds[1].Name;
            return PartialView();
        }

        public ActionResult ThemGioHang(int id, int SoLuong) //id sản phẩm
        {
            List<ChiTiet_DonHang> lct = data.ChiTiet_DonHangs.Where(t => t.SanPham_id == id).ToList();

            User kh = Session["kh"] as User;
            if (kh == null)
                return RedirectToAction("Index", "Users");
            DonHang dt = data.DonHangs.Where(t => t.User_id == kh.id && t.TrangThai == 10).FirstOrDefault();//trạng thái =5 tức là trạng thái chờ
            if (dt == null)
            {
                dt = new DonHang();
                dt.TrangThai=10;
                dt.User_id = kh.id;//
                data.DonHangs.InsertOnSubmit(dt);
                data.SubmitChanges();
            }
            ChiTiet_DonHang ct = lct.Find(t => t.SanPham_id == id && t.DonHang_id == dt.id);
            if (ct == null)
            {
                ChiTiet_DonHang ct2 = new ChiTiet_DonHang();
                data.ChiTiet_DonHangs.InsertOnSubmit(ct2);
                data.SubmitChanges();
                ChiTiet_DonHang cto = data.ChiTiet_DonHangs.Where(t => t.id == ct2.id).First();
                cto.SoLuong = SoLuong;
                cto.DonHang_id = dt.id;
                cto.SanPham_id = id;
                data.SubmitChanges();
            }
            else
            {
                ct.SoLuong = SoLuong;
                data.SubmitChanges();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Them1GioHang(int id) //id sản phẩm
        {
            int SoLuong = 1;
            List<ChiTiet_DonHang> lct = data.ChiTiet_DonHangs.Where(t => t.SanPham_id == id).ToList();

            User kh = Session["kh"] as User;
            if (kh == null)
                return RedirectToAction("Index", "Users");
            DonHang dt = data.DonHangs.Where(t => t.User_id == kh.id && t.TrangThai == 10).FirstOrDefault();//trạng thái =5 tức là trạng thái chờ
            if (dt == null)
            {
                dt = new DonHang();
                dt.TrangThai = 10;
                dt.User_id = kh.id;//
                data.DonHangs.InsertOnSubmit(dt);
                data.SubmitChanges();
            }
            ChiTiet_DonHang ct = lct.Find(t => t.SanPham_id == id && t.DonHang_id == dt.id);
            if (ct == null)
            {
                ChiTiet_DonHang ct2 = new ChiTiet_DonHang();
                data.ChiTiet_DonHangs.InsertOnSubmit(ct2);
                data.SubmitChanges();
                ChiTiet_DonHang cto = data.ChiTiet_DonHangs.Where(t => t.id == ct2.id).First();
                cto.SoLuong = SoLuong;
                cto.DonHang_id = dt.id;
                cto.SanPham_id = id;
                data.SubmitChanges();
            }
            else
            {
                ct.SoLuong = SoLuong;
                data.SubmitChanges();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult XemGioHang()
        {
            User kh = (User)Session["kh"];
            if (kh == null)
                return RedirectToAction("Index", "Users");
            DonHang dt = data.DonHangs.Where(t => t.TrangThai == 10 && t.User_id == kh.id).FirstOrDefault();
            if (dt == null)
            {
                List<ChiTiet_DonHang> c = new List<ChiTiet_DonHang>();
                c = null;
                return View(c);
            }
            List<ChiTiet_DonHang> ct = data.ChiTiet_DonHangs.Where(t => t.DonHang_id == dt.id).ToList();
            foreach (var item in ct)
            {
                if (item.San_Pham.SoLuongCon == 0)
                {
                    data.ChiTiet_DonHangs.DeleteOnSubmit(item);
                }
            }
            data.SubmitChanges();
            List<ChiTiet_DonHang> ct2 = data.ChiTiet_DonHangs.Where(t => t.DonHang_id == dt.id).ToList();
            ViewBag.to = dt.id;
            return View(ct2);
        }

        [HttpPost]
        public ActionResult SuaChiTietDH(int id, int SoLuong)
        {
            ChiTiet_DonHang ct = data.ChiTiet_DonHangs.Where(t => t.id == id).FirstOrDefault();
            if (ct != null)
            {
                ct.SoLuong = SoLuong;
                data.SubmitChanges();
            }
            return RedirectToAction("XemGioHang");
        }

        public ActionResult DeleteChiTietDH(int id)
        {
            ChiTiet_DonHang ct = data.ChiTiet_DonHangs.Where(t => t.id == id).FirstOrDefault();
            data.ChiTiet_DonHangs.DeleteOnSubmit(ct);
            data.SubmitChanges();
            return RedirectToAction("XemGioHang");
        }

        public ActionResult ChiTietDonHang(int id)
        {
            DonHang dt = data.DonHangs.Where(t => t.id == id).First();
            List<ChiTiet_DonHang> ct = data.ChiTiet_DonHangs.Where(t => t.DonHang_id == id).ToList();
            ViewBag.ct = ct;
            return View(dt);
        }
        [HttpPost]
        public ActionResult ChiTietDonHang(int id, FormCollection f)
        {
            DonHang dt = data.DonHangs.Where(t => t.id == id).First();
            dt.TrangThai = 0;
            List<ChiTiet_DonHang> ct = data.ChiTiet_DonHangs.Where(t => t.DonHang_id == dt.id).ToList();
            foreach (var item in ct)
            {
                San_Pham to = data.San_Phams.First(t => t.id == item.SanPham_id);
                to.SoLuongCon = to.SoLuongCon - item.SoLuong;

            }
            data.SubmitChanges();
            dt.GhiChu = f["GhiChu"];

            data.SubmitChanges();
            //return RedirectToAction("Index", "Home");
            return RedirectToAction("XacNhanThanhToan", new { id = dt.id, tongtien = f["tongtien"] });
            /////////////////////////////// Thay Index bằng thanh toán để đi tới trang thanh toán
        }

        public ActionResult XacNhanThanhToan(int id, float tongtien)
        {
            ViewBag.id = id;
            ViewBag.tongtien = tongtien;
            return View();
        }
        [HttpPost]
        public ActionResult XacNhanThanhToan(FormCollection c)
        {
            if (ModelState.IsValid)
            {
                DonHang dt = data.DonHangs.FirstOrDefault(t => t.id == int.Parse(c["id"]));
                List<ChiTiet_DonHang> ct = data.ChiTiet_DonHangs.Where(t => t.DonHang_id == int.Parse(c["id"])).ToList();
                ThanhToan tt = new ThanhToan
                {
                    DonHang_id = dt.id,
                    NgayThanhToan = DateTime.Now,
                    TongTien = float.Parse(c["tongtien"])
                };
                data.ThanhToans.InsertOnSubmit(tt);
                data.SubmitChanges();
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult ThanhToanKhiNhanHang(int id, float tongtien)
        {
            ViewBag.tt = tongtien;
            return PartialView();
        }
        public ActionResult ThanhToanTinDung(int id, float tongtien)
        {
            ViewBag.tt = tongtien;
            return PartialView();
        }
        public ActionResult ThanhToanNganHang(int id, float tongtien)
        {
            ViewBag.tt = tongtien;
            return PartialView();
        }
        public ActionResult ThanhToanMoMo(int id, float tongtien)
        {
            ViewBag.tt = tongtien;
            return PartialView();
        }
        public ActionResult SoLuongGioHang()
        {
            User kh = (User)Session["kh"];
            List<ChiTiet_DonHang> c;
            if (kh != null)
            {
                DonHang dt = data.DonHangs.Where(t => t.TrangThai == 10 && t.User_id == kh.id).FirstOrDefault();
                if (dt == null)
                {
                    c = null;
                    return PartialView(c);
                }
                c = data.ChiTiet_DonHangs.Where(t => t.DonHang_id == dt.id).ToList();
                return PartialView(c);
            }
            c = null;
            return PartialView(c);
        }

        public ActionResult KhuyenMai_SP(int id)
        {
            List<ChiTiet_KhuyenMai> ds = data.ChiTiet_KhuyenMais.Where(t => t.SanPham_id == id).ToList();
            float km = 0;
            foreach(var i in ds)
            {
                km += Convert.ToSingle(i.KhuyenMai.Gia);
            }
            San_Pham sp = data.San_Phams.FirstOrDefault(t => t.id == id);
            float gia = (float)(sp.Gia - sp.Gia * km / 100);
            ViewBag.tt = gia;
            return PartialView();
        }
    }
}