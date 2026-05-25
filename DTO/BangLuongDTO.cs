using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class BangLuongDTO
    {
        public int MaNhanVien { get; set; }
        public string? HoTen { get; set; }
        public string? TenRap { get; set; }
        public int MaRap { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
        public DateOnly TuNgay { get; set; }
        public DateOnly DenNgay { get; set; }

        public int TongNgayCong { get; set; }
        public double TongGioLam { get; set; }
        public int SoLanTre { get; set; }
        public int SoNgayNghi { get; set; }

        public decimal LuongCoBan { get; set; }
        public decimal Thuong { get; set; }
        public decimal Phat { get; set; }
        public decimal TongLuong { get; set; }
    }
}
