using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class ChiTietDonHang
{
    public int Ma { get; set; }

    public int? MaDonHang { get; set; }

    public int? MaDichVu { get; set; }

    public int? SoLuong { get; set; }

    public decimal? Gia { get; set; }

    public virtual DichVu? MaDichVuNavigation { get; set; }

    public virtual DonHang? MaDonHangNavigation { get; set; }
}
