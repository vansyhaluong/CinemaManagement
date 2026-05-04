using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class SuatChieu
{
    public int MaSuatChieu { get; set; }

    public int? MaPhim { get; set; }

    public int? MaPhong { get; set; }

    public DateTime? ThoiGianBatDau { get; set; }

    public DateTime? ThoiGianKetThuc { get; set; }

    public decimal? Gia { get; set; }

    public virtual Phim? MaPhimNavigation { get; set; }

    public virtual PhongChieu? MaPhongNavigation { get; set; }

    public virtual ICollection<VeBan> VeBans { get; set; } = new List<VeBan>();
}
