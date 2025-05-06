using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLi_WebDienThoai.Models
{
    public class ChiTietDonHang
    {
        public int? IDSP { get; set; }
        public string TenSP { get; set; }
        public string HinhAnh { get; set; }
        public int MaDonHang { get; set; }
        public int? SoLuong { get; set; }
        public int? TrangThai { get; set; }
        public double? Gia { get; set; }
        public double? TongTien { get; set; }
    }
}