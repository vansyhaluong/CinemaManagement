using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class ThanhToan
{
    public int MaThanhToan { get; set; }

    public int? MaDonHang { get; set; }

    public string? PhuongThuc { get; set; }

    public DateTime? NgayThanhToan { get; set; }

    public decimal? SoTien { get; set; }

    public string? TrangThai { get; set; }

    public virtual DonHang? MaDonHangNavigation { get; set; }
}
