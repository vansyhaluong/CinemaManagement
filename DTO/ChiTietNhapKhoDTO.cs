using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public  class ChiTietNhapKhoDTO
    {
        public int MaSanPham { get; set; }
        public string? TenSanPham { get; set; }
        public string? TenLoai { get; set; }
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }

        public decimal ThanhTien => SoLuong * Gia;

        public string GiaDisplay => Gia.ToString("N0") + " đ";

        public string ThanhTienDisplay => ThanhTien.ToString("N0") + " đ";

    }
}
