using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class BaoCaoDoanhThuNgay
{
    public int MaBaoCao { get; set; }

    public int? MaRap { get; set; }

    public DateOnly? Ngay { get; set; }

    public decimal? DoanhThuVe { get; set; }

    public decimal? DoanhThuDichVu { get; set; }

    public decimal? TongDoanhThu { get; set; }

    public int? SoDonHang { get; set; }

    public int? SoVeBan { get; set; }

    public virtual Rap? MaRapNavigation { get; set; }
}
