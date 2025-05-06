using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLi_WebDienThoai.Models
{
    public class KhuyenMaiGia
    {
        private readonly CSDLDataContext _context;

        public KhuyenMaiGia(CSDLDataContext context)
        {
            _context = context;
        }
        public KhuyenMaiGia()
        {
            CSDLDataContext data = new CSDLDataContext();
            _context = data;
        }

        public float TinhGiaSauKhuyenMai(int sanPhamId)
        {
            // Lấy danh sách chi tiết khuyến mãi của sản phẩm
            var dsKhuyenMai = _context.ChiTiet_KhuyenMais
                .Where(t => t.SanPham_id == sanPhamId)
                .ToList();

            // Tính tổng giá trị khuyến mãi
            float tongKhuyenMai = 0;
            foreach (var item in dsKhuyenMai)
            {
                tongKhuyenMai += Convert.ToSingle(item.KhuyenMai.Gia);
            }

            // Lấy thông tin sản phẩm
            var sanPham = _context.San_Phams.FirstOrDefault(sp => sp.id == sanPhamId);
            if (sanPham == null)
                throw new Exception("Sản phẩm không tồn tại.");

            // Tính giá sau khi giảm giá
            float giaSauGiam = (float)(sanPham.Gia - sanPham.Gia * tongKhuyenMai / 100);
            return Math.Max(giaSauGiam, 0); // Giá không âm
        }
    }
}