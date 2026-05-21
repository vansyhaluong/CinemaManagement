using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class ChamCongDTO
    {
        public int MaPhanCa { get; set; }

        public int MaNhanVien { get; set; }
        public string? HoTen { get; set; }

        public int MaRap { get; set; }
        public string? TenRap { get; set; }

        public int MaCa { get; set; }
        public string? TenCa { get; set; }

        public DateTime Ngay { get; set; }

        public string? GioBatDau { get; set; }
        public string? GioKetThuc { get; set; }

        public int? MaChamCong { get; set; }
        public DateTime? GioVao { get; set; }
        public DateTime? GioRa { get; set; }

        public string? TrangThai { get; set; }

        public string GioVaoText => GioVao == null ? "--:--" : GioVao.Value.ToString("HH:mm");
        public string GioRaText => GioRa == null ? "--:--" : GioRa.Value.ToString("HH:mm");

        public string ThoiGianCa => $"{GioBatDau} - {GioKetThuc}";
    }
}
