using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class NhanVien
{
    public int MaNhanVien { get; set; }

    public int? MaTaiKhoan { get; set; }

    public string? HoTen { get; set; }

    public string? SoDienThoai { get; set; }

    public int? MaRap { get; set; }

    public virtual ICollection<BangLuong> BangLuongs { get; set; } = new List<BangLuong>();

    public virtual ICollection<ChamCong> ChamCongs { get; set; } = new List<ChamCong>();

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual ICollection<HoanVe> HoanVes { get; set; } = new List<HoanVe>();

    public virtual ICollection<KhenThuong> KhenThuongs { get; set; } = new List<KhenThuong>();

    public virtual ICollection<KyLuat> KyLuats { get; set; } = new List<KyLuat>();

    public virtual Rap? MaRapNavigation { get; set; }

    public virtual TaiKhoan? MaTaiKhoanNavigation { get; set; }

    public virtual ICollection<PhieuNhapKho> PhieuNhapKhos { get; set; } = new List<PhieuNhapKho>();
}
