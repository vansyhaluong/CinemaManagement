using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class DichVu
{
    public int MaDichVu { get; set; }

    public string? Ten { get; set; }

    public decimal? Gia { get; set; }

    public string? TrangThai { get; set; }

    public int? MaLoai { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<ChiTietPhieuNhapKho> ChiTietPhieuNhapKhos { get; set; } = new List<ChiTietPhieuNhapKho>();

    public virtual Kho? Kho { get; set; }

    public virtual LoaiDichVu? MaLoaiNavigation { get; set; }
}
