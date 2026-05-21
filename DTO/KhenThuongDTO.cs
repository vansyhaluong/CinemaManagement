using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class KhenThuongDTO
    {
        public int MaKhenThuong { get; set; }
        public int MaNhanVien { get; set; }
        public string? HoTen { get; set; }
        public DateTime Ngay { get; set; }
        public string? LyDo { get; set; }
        public decimal SoTienThuong { get; set; }
    }
}
