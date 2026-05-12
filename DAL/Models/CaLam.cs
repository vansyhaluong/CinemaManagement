using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class CaLam
{
    public int MaCa { get; set; }

    public string? TenCa { get; set; }

    public TimeOnly? GioBatDau { get; set; }

    public TimeOnly? GioKetThuc { get; set; }

    public virtual ICollection<ChamCong> ChamCongs { get; set; } = new List<ChamCong>();
}
