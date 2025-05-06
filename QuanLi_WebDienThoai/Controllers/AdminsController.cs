using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLi_WebDienThoai.Models;

namespace QuanLi_WebDienThoai.Controllers
{
    public class AdminsController : Controller
    {
        CSDLDataContext data = new CSDLDataContext();

        // GET: Admins
        public ActionResult Index()
        {
            var result = (from sp in data.San_Phams
                          join ctdh in data.ChiTiet_DonHangs on sp.id equals ctdh.SanPham_id
                          join dh in data.DonHangs on ctdh.DonHang_id equals dh.id
                          group ctdh by sp into productGroup
                          orderby productGroup.Sum(ctdh => ctdh.SoLuong) descending
                          select new SanPhamWithSales
                          {
                              SanPham = productGroup.Key, // Thông tin sản phẩm
                              SoLuongDaBan =(int)productGroup.Sum(ctdh => ctdh.SoLuong) // Tổng số lượng đã bán
                          })
                          .Take(10) // Lấy top sản phẩm
                          .ToList();
            float TongTien = ((float)data.ThanhToans.Sum(t => t.TongTien).Value);
            ViewBag.TongTien = TongTien;
            ViewBag.TongDonHang = data.DonHangs.Count();//Tổng đơn hàng


            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            // Tính tổng doanh thu tháng hiện tại
            float TongTienThangNay = ((float)data.ThanhToans
                .Where(tt => tt.NgayThanhToan.HasValue &&
                             tt.NgayThanhToan.Value.Month == currentMonth &&
                             tt.NgayThanhToan.Value.Year == currentYear)
                .Sum(tt => tt.TongTien).Value);
            ViewBag.TongTienThangNay = TongTienThangNay;
            return View(result);
        }

        public ActionResult KhuyenMai()
        {
            return View();
        }
        public ActionResult DanhsachPhieuNhap()
        {
            return View();
        }
        public ActionResult PhieuNhap()
        {
            return View();
        }
        //còn action của sản phẩm 
        //action chi tiết sản phẩm
        public ActionResult PhanQuyen()
        {
            return View();
        }

        public ActionResult ThemSanPham()
        {
            // Lấy danh sách loại sản phẩm và tạo SelectList
            ViewBag.id = new SelectList(data.Loai_San_Phams.OrderBy(t => t.id), "id", "Name");

            // Trả về View
            return View(new San_Pham());
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemSanPham(San_Pham model, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                if (files != null)
                {
                    if(files.Count() != 4)
                    {
                        ViewBag.id = new SelectList(data.Loai_San_Phams.OrderBy(t => t.id), "id", "Name");
                        ViewBag.Thongbao = "Phải nhập đủ 4 ảnh";
                        return View(model);
                    }    
                    data.San_Phams.InsertOnSubmit(model);
                    data.SubmitChanges();
                    foreach (var file in files)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            // Đường dẫn lưu file
                            var fileName = Path.GetFileName(file.FileName);
                            var path = Path.Combine(Server.MapPath("~/Content/themes/images/products"), fileName);
                            // Kiểm tra xem tệp đã tồn tại chưa, nếu chưa thì lưu trữ tệp
                            if (!System.IO.File.Exists(path))
                            {
                                file.SaveAs(path);
                                data.Images.InsertOnSubmit(new Image
                                {
                                    SanPham_id = model.id,
                                    Name = fileName
                                });
                            }
                            else
                            {
                                ViewBag.Thongbao = "Một hoặc nhiều hình ảnh đã tồn tại";
                                ViewBag.id = new SelectList(data.Loai_San_Phams.OrderBy(t => t.id), "id", "Name");
                                return View(model);
                            }
                        }
                    }
                    
                }
                else
                {
                  
                }

                data.SubmitChanges();
                return RedirectToAction("ThemSanPham");

            }
            ViewBag.id = new SelectList(data.Loai_San_Phams.OrderBy(t => t.id), "id", "Name");
            return View(model);
        }

        public ActionResult SuaSanPham(int id)
        {
            // Lấy danh sách loại sản phẩm và tạo SelectList
            ViewBag.id = new SelectList(data.Loai_San_Phams.OrderBy(t => t.id), "id", "Name");
            San_Pham ds = data.San_Phams.Where(t => t.id == id).FirstOrDefault();
            return View(ds);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaSanPham(San_Pham model, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                if (files != null)
                {
                    if (files.Count() != 4)
                    {
                        ViewBag.id = new SelectList(data.Loai_San_Phams.OrderBy(t => t.id), "id", "Name");
                        ViewBag.Thongbao = "Phải nhập đủ 4 ảnh";
                        return View(model);
                    }
                    data.San_Phams.InsertOnSubmit(model);
                    data.SubmitChanges();
                    // Xóa các hình ảnh cũ nếu cần
                    var oldImages = data.Images.Where(i => i.SanPham_id == model.id).ToList();
                    data.Images.DeleteAllOnSubmit(oldImages); // Xóa hết hình ảnh cũ
                    foreach (var file in files)
                    {
                        if (file != null)
                        {
                            // Đường dẫn lưu file
                            var fileName = Path.GetFileName(file.FileName);
                            var path = Path.Combine(Server.MapPath("~/Content/themes/images/products"), fileName);
                            // Kiểm tra xem tệp đã tồn tại chưa, nếu chưa thì lưu trữ tệp
                            if (!System.IO.File.Exists(path))
                            {
                                file.SaveAs(path);
                                data.Images.InsertOnSubmit(new Image
                                {
                                    SanPham_id = model.id,
                                    Name = fileName
                                });
                            }
                            else
                            {
                                ViewBag.Thongbao = "Một hoặc nhiều hình ảnh đã tồn tại";
                                ViewBag.id = new SelectList(data.Loai_San_Phams.OrderBy(t => t.id), "id", "Name");
                                return View(model);
                            }
                        }
                    }

                }
                else
                {
                }
                data.SubmitChanges();
                return RedirectToAction("QuanLySanPham");

            }
            ViewBag.Thongbao = "Lỗi";
            ViewBag.id = new SelectList(data.Loai_San_Phams.OrderBy(t => t.id), "id", "Name");
            return View(model);
        }
        public ActionResult Xoa(int id)
        {
            var t = data.San_Phams.FirstOrDefault(a => a.id == id);
            var oldImages = data.Images.Where(i => i.SanPham_id == t.id).ToList();
            data.Images.DeleteAllOnSubmit(oldImages); // Xóa hết hình ảnh cũ
            
            data.San_Phams.DeleteOnSubmit(t);
            data.SubmitChanges();
            return RedirectToAction("QuanLySanPham");

        }

        public ActionResult ChiTietSanPham(int id)
        {
            return View(data.San_Phams.FirstOrDefault(t => t.id == id));
        }
        public ActionResult dsAnh(int id)
        {
            return PartialView(data.Images.Where(t => t.SanPham_id == id).ToList());
        }

        public ActionResult QuanLySanPham()
        {
            List<San_Pham> lst = data.San_Phams.ToList();
            return View(lst);
        }

        public ActionResult DuyetDon(int? page, int trangThai = 0)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var danhSachDonHang = (from dh in data.DonHangs
                                   join u in data.Users on dh.User_id equals u.id
                                   join tt in data.ThanhToans on dh.id equals tt.DonHang_id
                                   where dh.TrangThai == trangThai // Lọc theo trạng thái
                                   select new DonHangModel
                                   {
                                       Id = dh.id,
                                       TenKhachHang = u.Name,
                                       NgayDatHang = tt.NgayThanhToan,
                                       TrangThai = dh.TrangThai,
                                       NgayNhanHang = "Trống",
                                       ThanhToan = "Chưa thanh toán" // Hoặc tính toán từ bảng ThanhToan
                                   }).ToList();

            int totalItems = danhSachDonHang.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var donHangTrangHienTai = danhSachDonHang
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.TrangThai = trangThai;
            return View(danhSachDonHang);
        }
        public ActionResult DuyetDonHang(int maHoaDon)
        {
            var donHang = data.DonHangs.FirstOrDefault(dh => dh.id == maHoaDon);
            donHang.TrangThai = 1;
            data.SubmitChanges();
            return RedirectToAction("DuyetDon");
        }
        public ActionResult HuyDonHang(int maHoaDon)
        {
            var donHang = data.DonHangs.FirstOrDefault(dh => dh.id == maHoaDon);
            donHang.TrangThai = 2;
            data.SubmitChanges();
            return RedirectToAction("DuyetDon");
        }
        public ActionResult DanhGia()
        {
            var danhSachDanhGia = from dg in data.DanhGias
                                  join sp in data.San_Phams on dg.SanPham_id equals sp.id
                                  join u in data.Users on dg.User_id equals u.id
                                  select new DanhGiaModel
                                  {
                                      Id = dg.id,
                                      Vote = (int)dg.Vote,
                                      TenSanPham = sp.Name,
                                      TenNguoiMua = u.Name
                                  };
            return View(danhSachDanhGia);
        }
        public ActionResult ChiTietDanhGia(int id)
        {
            var ds = data.DanhGias.Where(t => t.id == id).FirstOrDefault();
            return View(ds);
        }

        public ActionResult FeedBack()
        {
            List<Feedback> ds = data.Feedbacks.ToList();
            return View(ds);
        }

        public ActionResult QuanLiKhachHang()
        {
            List<User> ds = data.Users.Where(t => t.Role_id == 2).ToList();
            return View(ds);
        }

        public ActionResult XoaKH(int id)
        {
            var t = data.Users.FirstOrDefault(a => a.id == id);
            data.Users.DeleteOnSubmit(t);
            data.SubmitChanges();
            return RedirectToAction("QuanLiKhachHang");
        }

        public ActionResult SuaKH(int id)
        {
            User ds = data.Users.Where(t => t.id == id).FirstOrDefault();
            return View(ds);
        }
        [HttpPost]
        public ActionResult SuaKH(User model)
        {
            if (ModelState.IsValid)
            {
                
                    data.Users.InsertOnSubmit(model);
                    data.SubmitChanges();
                // Xóa các hình ảnh cũ nếu cần
                    return RedirectToAction("QuanLiKhachHang");
            }
            return View(model);
        }
    }
}