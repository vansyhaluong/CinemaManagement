using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class TaiKhoan
{
    public int MaTaiKhoan { get; set; }

    public string? TenDangNhap { get; set; }

    public string? MatKhau { get; set; }

    public string? VaiTro { get; set; }

    public virtual NhanVien? NhanVien { get; set; }
}
