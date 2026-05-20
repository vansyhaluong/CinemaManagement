using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class DonHang
{
    public int MaDonHang { get; set; }

    public string? MaHoaDon { get; set; }

    public int? MaKhachHang { get; set; }

    public DateTime? NgayDat { get; set; }

    public string? TrangThai { get; set; }

    public int? MaNhanVien { get; set; }

    public int? MaKhuyenMai { get; set; }

    public decimal TongTienVe { get; set; }

    public decimal TongTienDichVu { get; set; }

    public decimal TienGiam { get; set; }

    public decimal TongThanhToan { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual KhachHang? MaKhachHangNavigation { get; set; }

    public virtual KhuyenMai? MaKhuyenMaiNavigation { get; set; }

    public virtual NhanVien? MaNhanVienNavigation { get; set; }

    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();

    public virtual ICollection<VeBan> VeBans { get; set; } = new List<VeBan>();
}
