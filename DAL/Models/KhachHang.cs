using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class KhachHang
{
    public int MaKhachHang { get; set; }

    public string? HoTen { get; set; }

    public string? SoDienThoai { get; set; }

    public string? Email { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}
