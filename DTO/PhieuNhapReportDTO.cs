using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class PhieuNhapReportDTO
    {
        public int MaPhieuNhap { get; set; }

        public string? TenSanPham { get; set; }

        public int SoLuong { get; set; }

        public decimal DonGia { get; set; }

        public decimal ThanhTien => SoLuong * DonGia;

        public DateTime NgayNhap { get; set; }

        public string? NhanVienNhap { get; set; }
    }
}
