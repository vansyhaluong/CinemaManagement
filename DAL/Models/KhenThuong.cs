using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class KhenThuong
{
    public int MaKhenThuong { get; set; }

    public int? MaNhanVien { get; set; }

    public DateOnly? Ngay { get; set; }

    public string? LyDo { get; set; }

    public decimal? SoTienThuong { get; set; }

    public virtual NhanVien? MaNhanVienNavigation { get; set; }
}
