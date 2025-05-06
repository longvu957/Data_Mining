using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLi_WebDienThoai.Models
{
    public class LoaiSanPhamModel
    {
        public Models.San_Pham SP { get; set; }
        public string NameLoaiSP { get; set; }
        public List<string> ThuongHieuList { get; set; }
    }
}