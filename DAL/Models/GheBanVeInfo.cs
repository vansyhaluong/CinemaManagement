namespace DAL.Models
{
    public class GheBanVeInfo
    {
        public int MaGhe { get; set; }

        public int? MaPhong { get; set; }

        public string? HangGhe { get; set; }

        public int? SoGhe { get; set; }

        public int? MaLoaiGhe { get; set; }

        public string? TenLoaiGhe { get; set; }

        public decimal Gia { get; set; }

        public string TrangThai { get; set; } = "Trong";

        public int GiayConLai { get; set; }
    }
}
