using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class LoaiSanPham
{
    public int MaLoaiSp { get; set; }

    public string? TenLoai { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
