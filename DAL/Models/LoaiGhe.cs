using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class LoaiGhe
{
    public int MaLoaiGhe { get; set; }

    public string? TenLoai { get; set; }

    public decimal? HeSoGia { get; set; }

    public virtual ICollection<Ghe> Ghes { get; set; } = new List<Ghe>();
}
