using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class HoaDon
{
    public int MaHoaDon { get; set; }

    public int? MaDonHang { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual DonHang? MaDonHangNavigation { get; set; }
}
