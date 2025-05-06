using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLi_WebDienThoai.Models;

namespace QuanLi_WebDienThoai.Controllers
{
    public class HomeController : Controller
    {
        Models.CSDLDataContext db = new Models.CSDLDataContext();
        CSDLDataContext data = new CSDLDataContext();

        public ActionResult Index()
        {
            var listsanpham = db.San_Phams.Select(sp => new Models.SanPhamModel
            {
                SP = sp,
                HinhAnh = db.Images.Where(i => i.SanPham_id == sp.id).Select(i => i.Name).FirstOrDefault()
            }).ToList();
            return View(listsanpham);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult DanhMuc()
        {
            var list_loaisanpham_slidebar = db.San_Phams
                .GroupBy(sp => sp.Loai_San_Pham_id)
                .Select(group => new Models.LoaiSanPhamModel
                {
                    SP = group.FirstOrDefault(),
                    NameLoaiSP = db.Loai_San_Phams
                        .Where(lsp => lsp.id == group.Key)
                        .Select(lsp => lsp.Name)
                        .SingleOrDefault(),
                    ThuongHieuList = group
                        .Select(sp => sp.ThuongHieu)
                        .Distinct()
                        .ToList()
                }).ToList();

            return PartialView(list_loaisanpham_slidebar);
        }

        public ActionResult SanPham_DanhMuc(int MaLoai)
        {
            var listSP_DM = db.San_Phams.Where(sp => sp.Loai_San_Pham_id == MaLoai).Select(sp => new Models.SanPhamModel
            {
                SP = sp,
                HinhAnh = db.Images.Where(i => i.SanPham_id == sp.id).Select(i => i.Name).FirstOrDefault()
            }).ToList();
            return View("Index", listSP_DM);
        }

        public ActionResult SanPham_ThuongHieu(string thuonghieu, int MaLoai)
        {
            var listSP_TH = db.San_Phams.Where(sp => sp.ThuongHieu == thuonghieu && sp.Loai_San_Pham_id == MaLoai).Select(sp => new Models.SanPhamModel
            {
                SP = sp,
                HinhAnh = db.Images.Where(i => i.SanPham_id == sp.id).Select(i => i.Name).FirstOrDefault()
            }).ToList();
            return View("Index", listSP_TH);
        }


        public ActionResult ChiTiet(int id)
        {
            San_Pham sp = data.San_Phams.FirstOrDefault(t => t.id == id);

            if (sp == null)
            {
                return HttpNotFound();
            }

            List<Image> ima = data.Images.Where(t => t.SanPham_id == id).ToList();
            ViewBag.lstIma = ima;

            var listdanhgia = (
                from dg in db.DanhGias
                join ac in db.Users on dg.User_id equals ac.id
                where dg.SanPham_id == id
                select new Models.DanhGiaSP
                {
                    DanhGiaID = dg.id,
                    USERID = dg.User_id,
                    SoSao = dg.Vote,
                    NoiDung = dg.NoiDung,
                    SanPhamId = dg.id,
                    FullName = ac.Name
                }).ToList();

            var viewModel = new Models.SanPham_DanhGia
            {
                SanPham = sp,
                DanhGias = listdanhgia
            };

            return View(viewModel);  // Truyền ViewModel vào View
        }

        public ActionResult TimKiem(string query)
        {
            ViewBag.NoiDungTimKiem = query;
            if (string.IsNullOrEmpty(query))
                return RedirectToAction("Index", "Home");
            else
            {
                var searchResults = db.San_Phams
                    .Where(sp => sp.Name.Contains(query) || sp.MoTa.Contains(query) || sp.GioiThieu.Contains(query))
                    .Select(sp => new Models.SanPhamModel
                    {
                        SP = sp,
                        HinhAnh = db.Images.Where(i => i.SanPham_id == sp.id).Select(i => i.Name).FirstOrDefault()
                    }).ToList();
                return View(searchResults);
            }
        }
    }
}
