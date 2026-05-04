using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class Ghe
{
    public int MaGhe { get; set; }

    public int? MaPhong { get; set; }

    public string? HangGhe { get; set; }

    public int? SoGhe { get; set; }

    public string? LoaiGhe { get; set; }

    public virtual PhongChieu? MaPhongNavigation { get; set; }

    public virtual ICollection<VeBan> VeBans { get; set; } = new List<VeBan>();
}
