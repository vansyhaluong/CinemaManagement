using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class LoaiDichVu
{
    public int MaLoai { get; set; }

    public string? TenLoai { get; set; }

    public virtual ICollection<DichVu> DichVus { get; set; } = new List<DichVu>();
}
