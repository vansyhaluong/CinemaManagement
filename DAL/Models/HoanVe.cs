using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class HoanVe
{
    public int MaHoanVe { get; set; }

    public int? MaDatVe { get; set; }

    public DateTime? NgayHoan { get; set; }

    public string? LyDo { get; set; }

    public decimal? SoTienHoan { get; set; }

    public string? TrangThai { get; set; }

    public int? MaNhanVien { get; set; }

    public virtual VeBan? MaDatVeNavigation { get; set; }

    public virtual NhanVien? MaNhanVienNavigation { get; set; }
}
