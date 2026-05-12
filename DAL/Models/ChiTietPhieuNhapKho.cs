using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class ChiTietPhieuNhapKho
{
    public int Ma { get; set; }

    public int? MaPhieuNhap { get; set; }

    public int? MaSanPham { get; set; }

    public int? SoLuong { get; set; }

    public decimal? DonGia { get; set; }

    public virtual PhieuNhapKho? MaPhieuNhapNavigation { get; set; }

    public virtual SanPham? MaSanPhamNavigation { get; set; }
}
