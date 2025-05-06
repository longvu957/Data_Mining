using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLi_WebDienThoai.Models
{
    public class DonHang_ChiTietDonHang
    {
        public List<Models.DonHang> DonHangs { get; set; }
        public List<Models.ChiTietDonHang> CTHD { get; set; }
    }
}