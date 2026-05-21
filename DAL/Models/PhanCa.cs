using Cinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class PhanCa
    {
        public int MaPhanCa { get; set; }
        public int MaNhanVien { get; set; }
        public int MaCa { get; set; }
        public DateOnly Ngay { get; set; }

        public virtual NhanVien MaNhanVienNavigation { get; set; } = null!;
        public virtual CaLam MaCaNavigation { get; set; } = null!;
    }
}
