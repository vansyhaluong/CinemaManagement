namespace DAL.Models
{
    public class KetQuaBanVe
    {
        public bool ThanhCong { get; set; }

        public string ThongBao { get; set; } = string.Empty;

        public int? MaDonHang { get; set; }

        public string? MaHoaDon { get; set; }
    }
}
