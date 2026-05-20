using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class BaoCaoDoanhThuExcelDTO
    {
        public string MaHoaDon { get; set; } = "";
        public DateTime? NgayDat { get; set; }

        public string KhachHang { get; set; } = "";
        public string SoDienThoai { get; set; } = "";

        public string TenPhim { get; set; } = "";
        public string SuatChieu { get; set; } = "";
        public string Ghe { get; set; } = "";

        public int SoLuongVe { get; set; }
        public string DichVuDaMua { get; set; } = "";

        public decimal TienVe { get; set; }
        public decimal TienDichVu { get; set; }
        public decimal TongTien { get; set; }
    }
}
