using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class ChamCong
{
    public int MaChamCong { get; set; }

    public int? MaNhanVien { get; set; }

    public DateOnly? Ngay { get; set; }

    public int? MaCa { get; set; }

    public DateTime? GioVao { get; set; }

    public DateTime? GioRa { get; set; }

    public string? TrangThai { get; set; }

    public virtual CaLam? MaCaNavigation { get; set; }

    public virtual NhanVien? MaNhanVienNavigation { get; set; }
}
