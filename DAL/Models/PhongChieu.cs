using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class PhongChieu
{
    public int MaPhong { get; set; }

    public int? MaRap { get; set; }

    public string? TenPhong { get; set; }

    public virtual ICollection<Ghe> Ghes { get; set; } = new List<Ghe>();

    public virtual Rap? MaRapNavigation { get; set; }

    public virtual ICollection<SuatChieu> SuatChieus { get; set; } = new List<SuatChieu>();
}
