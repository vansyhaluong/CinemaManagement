using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class PhieuNhapKho
{
    public int MaPhieuNhap { get; set; }

    public DateTime? NgayNhap { get; set; }

    public int? MaNhanVien { get; set; }

    public virtual ICollection<ChiTietPhieuNhapKho> ChiTietPhieuNhapKhos { get; set; } = new List<ChiTietPhieuNhapKho>();

    public virtual NhanVien? MaNhanVienNavigation { get; set; }
}
