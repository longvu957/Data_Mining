using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLi_WebDienThoai.Models
{
    public class DanhGiaModel
    {
        public int Id { get; set; }
        public int User_id { get; set; }
        public int SanPham_id { get; set; }
        public string NoiDung { get; set; }
        public int Vote { get; set; }
        public DateTime NgayDanhGia { get; set; }

        // Các thuộc tính bổ sung cho thông tin sản phẩm và người mua
        public string TenSanPham { get; set; }
        public string TenNguoiMua { get; set; }
    }
}