using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class VeBan
{
    public int MaDatVe { get; set; }

    public string? MaVe { get; set; }

    public int? MaDonHang { get; set; }

    public int? MaSuatChieu { get; set; }

    public int? MaGhe { get; set; }

    public decimal? Gia { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<GiuGheTam> GiuGheTams { get; set; } = new List<GiuGheTam>();

    public virtual ICollection<HoanVe> HoanVes { get; set; } = new List<HoanVe>();

    public virtual DonHang? MaDonHangNavigation { get; set; }

    public virtual Ghe? MaGheNavigation { get; set; }

    public virtual SuatChieu? MaSuatChieuNavigation { get; set; }
}
