using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class KhuyenMai
{
    public int MaKhuyenMai { get; set; }

    public string? TenKhuyenMai { get; set; }

    public string? MoTa { get; set; }

    public string? LoaiGiam { get; set; }

    public decimal? GiaTriGiam { get; set; }

    public decimal? DonToiThieu { get; set; }

    public DateTime? NgayBatDau { get; set; }

    public DateTime? NgayKetThuc { get; set; }

    public int? SoLuong { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}
