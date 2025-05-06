using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLi_WebDienThoai.Models
{
    public class DonHangModel
    {
        public string TenKhachHang { get; set; }
        public DateTime? NgayDatHang { get; set; }
        public decimal TongTien { get; set; }
        public List<ChiTietDonHangModel> ChiTietDonHang { get; set; } // Danh sách chi tiết đơn hàng
        public int Id { get; internal set; }
        public int? TrangThai { get; internal set; }
        public string NgayNhanHang { get; internal set; }
        public string ThanhToan { get; internal set; }
    }
}