using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class ChiTietPhieuNhapKho
{
    public int Ma { get; set; }

    public int? MaPhieuNhap { get; set; }

    public int? MaDichVu { get; set; }

    public int? SoLuong { get; set; }

    public decimal? DonGia { get; set; }

    public virtual DichVu? MaDichVuNavigation { get; set; }

    public virtual PhieuNhapKho? MaPhieuNhapNavigation { get; set; }
}
