using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLi_WebDienThoai.Models
{
    public class SanPham_DanhGia
    {
        public Models.San_Pham SanPham { get; set; }
        public List<Models.DanhGiaSP> DanhGias { get; set; }
    }
}