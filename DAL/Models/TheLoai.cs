using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class TheLoai
{
    public int MaTheLoai { get; set; }

    public string? Ten { get; set; }

    public virtual ICollection<Phim> MaPhims { get; set; } = new List<Phim>();
}
