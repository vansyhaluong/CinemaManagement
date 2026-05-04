using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class Phim
{
    public int MaPhim { get; set; }

    public string? TieuDe { get; set; }

    public string? MoTa { get; set; }

    public int? ThoiLuong { get; set; }

    public DateOnly? NgayKhoiChieu { get; set; }

    public string? QuocGia { get; set; }

    public string? AnhBia { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<SuatChieu> SuatChieus { get; set; } = new List<SuatChieu>();

    public virtual ICollection<TheLoai> MaTheLoais { get; set; } = new List<TheLoai>();
}
