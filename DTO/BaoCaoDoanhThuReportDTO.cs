using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class BaoCaoDoanhThuReportDTO
    {
        public string MaHoaDon { get; set; } = "";

        public string KhachHang { get; set; } = "";

        public decimal TienVe { get; set; }

        public decimal TienDichVu { get; set; }

        public decimal TongTien { get; set; }
    }
}
