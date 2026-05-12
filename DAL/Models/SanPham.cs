using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class SanPham
{
    public int MaSanPham { get; set; }

    public string? Ten { get; set; }

    public decimal? Gia { get; set; }

    public string? TrangThai { get; set; }

    public int? MaLoaiSp { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<ChiTietPhieuNhapKho> ChiTietPhieuNhapKhos { get; set; } = new List<ChiTietPhieuNhapKho>();

    public virtual Kho? Kho { get; set; }

    public virtual LoaiSanPham? MaLoaiSpNavigation { get; set; }
}
