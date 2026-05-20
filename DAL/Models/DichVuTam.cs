using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public  class DichVuTam
    {
        public int MaSanPham { get; set; }

        public string? Ten { get; set; }

        public decimal Gia { get; set; }
        public int SoLuong { get; set; }
    }
}
