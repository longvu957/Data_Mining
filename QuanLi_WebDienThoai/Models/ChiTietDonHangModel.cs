using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLi_WebDienThoai.Models
{
    public class ChiTietDonHangModel
    {
        public string TenSanPham { get; set; }
        public int? SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal? TongTien => SoLuong * DonGia;
    }
}