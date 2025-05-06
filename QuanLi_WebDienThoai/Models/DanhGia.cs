using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLi_WebDienThoai.Models
{
    public class DanhGiaSP
    {
        public int DanhGiaID { get; set; }
        public int? USERID { get; set; }
        public string NoiDung {  get; set; }
        public int? SoSao { get;set; }
        public int SanPhamId { get; set; }
        public string FullName { get; set; }
    }
}