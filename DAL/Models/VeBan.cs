using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class VeBan
{
    public int MaDatVe { get; set; }

    public int? MaDonHang { get; set; }

    public int? MaSuatChieu { get; set; }

    public int? MaGhe { get; set; }

    public decimal? Gia { get; set; }

    public virtual DonHang? MaDonHangNavigation { get; set; }

    public virtual Ghe? MaGheNavigation { get; set; }

    public virtual SuatChieu? MaSuatChieuNavigation { get; set; }
}
