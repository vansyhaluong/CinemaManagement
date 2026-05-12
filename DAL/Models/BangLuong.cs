using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class BangLuong
{
    public int MaLuong { get; set; }

    public int? MaNhanVien { get; set; }

    public int? Thang { get; set; }

    public int? Nam { get; set; }

    public decimal? LuongCoBan { get; set; }

    public decimal? Thuong { get; set; }

    public decimal? Phat { get; set; }

    public decimal? TongLuong { get; set; }

    public virtual NhanVien? MaNhanVienNavigation { get; set; }
}
