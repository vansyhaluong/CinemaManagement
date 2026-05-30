using System;

namespace Cinema.Models;

public partial class GiuGheTam
{
    public int MaGiuGhe { get; set; }

    public int MaSuatChieu { get; set; }

    public int MaGhe { get; set; }

    public DateTime ThoiGianGiu { get; set; }

    public DateTime HetHanLuc { get; set; }

    public string TrangThai { get; set; } = "Đang giữ";

    public int? MaDatVe { get; set; }

    public virtual Ghe MaGheNavigation { get; set; } = null!;

    public virtual SuatChieu MaSuatChieuNavigation { get; set; } = null!;

    public virtual VeBan? MaDatVeNavigation { get; set; }
}
