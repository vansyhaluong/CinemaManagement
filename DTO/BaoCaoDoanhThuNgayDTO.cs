using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class BaoCaoDoanhThuNgayDTO
    {
        public int MaBaoCao { get; set; }
        public int MaRap { get; set; }
        public string TenRap { get; set; } = "";
        public DateTime Ngay { get; set; }
        public decimal DoanhThuVe { get; set; }
        public decimal DoanhThuDichVu { get; set; }
        public decimal TongDoanhThu { get; set; }
        public int SoDonHang { get; set; }
        public int SoVeBan { get; set; }
    }
}
