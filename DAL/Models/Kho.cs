using System;
using System.Collections.Generic;

namespace Cinema.Models;

public partial class Kho
{
    public int MaKho { get; set; }

    public int? MaDichVu { get; set; }

    public int? SoLuongTon { get; set; }

    public virtual DichVu? MaDichVuNavigation { get; set; }
}
