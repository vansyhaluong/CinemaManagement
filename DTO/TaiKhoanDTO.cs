using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class TaiKhoanDTO
    {
        public static int MaTaiKhoan { get; set; }
        public static int MaNhanVien { get; set; }
        public static string? HoTen { get; set; }
        public static string? VaiTro { get; set; }

        public static int MaRap { get; set; }
        public static string TenRap { get; set; }

        public static void Clear()
        {
            MaTaiKhoan = 0;
            MaNhanVien = 0;
            HoTen = "";
            VaiTro = "";
            MaRap = 0;
            TenRap = "";
        }
    }
}
