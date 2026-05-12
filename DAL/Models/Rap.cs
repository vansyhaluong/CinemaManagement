using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class Rap
{
    public int MaRap { get; set; }

    public string? TenRap { get; set; }

    public string? DiaChi { get; set; }

    public string? ThanhPho { get; set; }

    public virtual ICollection<BaoCaoDoanhThuNgay> BaoCaoDoanhThuNgays { get; set; } = new List<BaoCaoDoanhThuNgay>();

    public virtual ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();

    public virtual ICollection<PhongChieu> PhongChieus { get; set; } = new List<PhongChieu>();
}
