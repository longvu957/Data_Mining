using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using QuanLi_WebDienThoai.Models;

namespace QuanLi_WebDienThoai.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        Models.CSDLDataContext db = new Models.CSDLDataContext();
        CSDLDataContext data = new CSDLDataContext();
        public ActionResult Index()
        {

            return View();
        }
        [HttpPost]


        public ActionResult Index(string Username, string Password)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra người dùng
                var user = data.Users.FirstOrDefault(t => (t.Role_id == 1 || t.Role_id == 2) && (t.Email == Username || t.DienThoai == Username));
                if (user != null)
                {
                    Session["UserName"] = user.Name;
                    Session["UserID"] = user.id;
                    if (Password == user.Password)
                    {
                        TempData["Message"] = "Đăng nhập thành công";
                        if (user.Role_id == 1)
                        {
                            Session["kh"] = user;

                            return RedirectToAction("Index", "Admins");
                        }
                        else if (user.Role_id == 2)
                        {
                            Session["kh"] = user;

                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ViewBag.LoginQL = "Đăng nhập không thành công. Vui lòng kiểm tra lại tên đăng nhập và mật khẩu.";
                    }
                }
                else
                {
                    ViewBag.LoginQL = "Tài khoản không tồn tại ";
                }
            }
            return View();
        }


        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(string UserName, string Email, string SDT, string Password, string Gender, string DiaChi)
        {
            if (ModelState.IsValid)
            {
                if (!Regex.IsMatch(SDT, @"^\d{10}$"))
                {
                    ViewBag.LoginQL = "Số điện thoại phải có đúng 10 chữ số.";
                    return View();
                }
                var UserExists = db.Users.Any(t => t.Name == UserName || t.Email == Email);
                if (UserExists)
                {
                    ViewBag.LoginQL = "Tên người dùng hoặc email đã tồn tại. Vui lòng chọn tên khác.";
                    return View();
                }
                var New = new User
                {
                    Name = UserName,
                    Email = Email,
                    DienThoai = SDT,
                    Password = Password,
                    GioiTinh = Gender,
                    DiaChi = DiaChi,
                    Role_id = 2


                };
                // Lưu người dùng mới vào cơ sở dữ liệu
                data.Users.InsertOnSubmit(New);

                TempData["MessageRegister"] = "Đăng ký thành công";
                data.SubmitChanges();
                return RedirectToAction("Index", "Users");

            }

            return View();

        }
        public ActionResult Logout()
        {
            Session["UserName"] = null;
            Session["kh"] = null;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult ManageAccount(int Selection)
        {
            Models.User TaiKhoan = Session["kh"] as Models.User;
            if (TaiKhoan == null)
                return RedirectToAction("Index");

            var listdonhang = db.DonHangs.Where(dh => dh.User_id == TaiKhoan.id).ToList();
            if (Selection == 0 || Selection == 1 || Selection == 2)
            {
                listdonhang = db.DonHangs.Where(dh => dh.User_id == TaiKhoan.id && dh.TrangThai == Selection).ToList();
            }

            var chitietdonhang = (
             from CTDH in db.ChiTiet_DonHangs
             join DH in db.DonHangs on CTDH.DonHang_id equals DH.id
             join SP in db.San_Phams on CTDH.SanPham_id equals SP.id
             join HA in db.Images on SP.id equals HA.SanPham_id into NhomHA
             where DH.User_id == TaiKhoan.id
             select new Models.ChiTietDonHang
             {
                 IDSP = CTDH.SanPham_id,
                 TenSP = SP.Name,
                 HinhAnh = NhomHA
                 .OrderBy(ha => ha.id)
                 .Select(ha => ha.Name)
                 .FirstOrDefault(),
                 MaDonHang = DH.id,
                 SoLuong = CTDH.SoLuong,
                 TrangThai = DH.TrangThai,
                 Gia = SP.Gia,
                 TongTien = CTDH.SoLuong * SP.Gia
             }).ToList();

            var donhang_chitiet = new Models.DonHang_ChiTietDonHang
            {
                DonHangs = listdonhang,
                CTHD = chitietdonhang
            };

            return View(donhang_chitiet);
        }
        public ActionResult InforAccount()
        {
            if (Session["UserID"] != null)
            {
                int userId = (int)Session["UserID"];
                var user = data.Users.FirstOrDefault(t => t.id == userId);
                return View(user);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpPost]
        public ActionResult InforAccount(User u, string Tinh, string Quan, string Phuong, string SoNha)
        {
            if (Session["UserID"] != null)
            {
                int userId = (int)Session["UserID"];
                var user = data.Users.FirstOrDefault(t => t.id == userId);
                if (user != null)
                {
                    user.Name = u.Name;
                    user.Email = u.Email;
                    user.DienThoai = u.DienThoai;
                    user.Password = u.Password;
                    user.GioiTinh = u.GioiTinh;
                    user.DiaChi = SoNha + " " + Phuong + " " + Quan + " " + Tinh;
                    TempData["Message"] = "Lưu thông tin thành công";
                    data.SubmitChanges();
                    return RedirectToAction("ManageAccount", "Users");

                }
                else
                {
                    return View();
                }

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        public ActionResult DanhGia(int id)
        {
            if (Session["kh"] != null)
            {
                Models.User TaiKhoan = Session["kh"] as Models.User;
                var listdonhang = db.DonHangs.Where(dh => dh.User_id == TaiKhoan.id && dh.id == id).ToList();

                var chitietdonhang = (
                 from CTDH in db.ChiTiet_DonHangs
                 join DH in db.DonHangs on CTDH.DonHang_id equals DH.id
                 join SP in db.San_Phams on CTDH.SanPham_id equals SP.id
                 join HA in db.Images on SP.id equals HA.SanPham_id into NhomHA
                 where DH.User_id == TaiKhoan.id
                 select new Models.ChiTietDonHang
                 {
                     IDSP = CTDH.SanPham_id,
                     TenSP = SP.Name,
                     HinhAnh = NhomHA
                     .OrderBy(ha => ha.id)
                     .Select(ha => ha.Name)
                     .FirstOrDefault(),
                     MaDonHang = DH.id,
                     SoLuong = CTDH.SoLuong,
                     TrangThai = DH.TrangThai,
                     Gia = SP.Gia,
                     TongTien = CTDH.SoLuong * SP.Gia
                 }).ToList();

                var donhang_chitiet = new Models.DonHang_ChiTietDonHang
                {
                    DonHangs = listdonhang,
                    CTHD = chitietdonhang
                };

                return View(donhang_chitiet);
            }

            return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        public ActionResult DanhGia(FormCollection Data, int idsp, int idtk, int iddh)
        {
            Models.User TaiKhoan = Session["kh"] as Models.User;
            var danhgia = Data["DanhGia"];
            var sosao = Data["rating"];

            bool hasError = false;

            if (String.IsNullOrEmpty(sosao))
            {
                TempData["Message"] = "Lỗi số sao đánh giá";
                hasError = true;
            }

            if (hasError)
                return View();
            else
            {
                Models.DanhGia hasDG = new Models.DanhGia
                {
                    User_id = TaiKhoan.id,
                    SanPham_id = idsp,
                    NoiDung = danhgia,
                    Vote = int.Parse(sosao)
                };

                db.DanhGias.InsertOnSubmit(hasDG);
                db.SubmitChanges();
                return RedirectToAction("DanhGia", "Users", new { @id = iddh });
            }
        }

        public ActionResult HuyDonHang(int id_donhang)
        {
            Models.DonHang dh = db.DonHangs.FirstOrDefault(x=>x.id == id_donhang);
            if (dh != null)
            {
                dh.TrangThai = 2;
                db.SubmitChanges();
            }
            return RedirectToAction("ManageAccount", "Users", new { @Selection = 3 });
        }
        public ActionResult FeedBack_User()
        {
            return View(new Feedback());
        }
        [HttpPost]
        public ActionResult FeedBack_User(Feedback ds)
        {
            db.Feedbacks.InsertOnSubmit(ds);
            db.SubmitChanges();
            return RedirectToAction("Index", "Home");
        }

        
    }
}