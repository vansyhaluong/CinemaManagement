using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class SanPhamKhoDTO
    {
        public int MaSanPham { get; set; }
        public string? Ten { get; set; }
        public string? TenLoai { get; set; }
        public decimal Gia { get; set; }
        public int SoLuongTon { get; set; }
        private string? _hinhAnh;

        public string HinhAnh
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_hinhAnh))
                    return "";

                // 1. Đường dẫn pack URI cũ
                if (_hinhAnh.StartsWith("pack://"))
                    return _hinhAnh;

                // 2. Đường dẫn resource cũ: /Images/7up.png
                if (_hinhAnh.StartsWith("/"))
                    return _hinhAnh;

                // 3. Đường dẫn tuyệt đối: D:\...
                if (Path.IsPathRooted(_hinhAnh))
                    return _hinhAnh;

                // 4. Đường dẫn runtime mới: Images/abc.png
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _hinhAnh);
            }
            set
            {
                _hinhAnh = value;
            }
        }

        public string GiaDisplay => Gia.ToString("N0") + " đ";

        public string TrangThaiKho
        {
            get
            {
                if (SoLuongTon <= 0) return "Hết hàng";
                if (SoLuongTon <= 10) return "Sắp hết";
                return "Còn hàng";
            }
        }

        
    }
}
