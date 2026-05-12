using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class Kho
{
    public int MaKho { get; set; }

    public int? MaSanPham { get; set; }

    public int? SoLuongTon { get; set; }

    public virtual SanPham? MaSanPhamNavigation { get; set; }
}
