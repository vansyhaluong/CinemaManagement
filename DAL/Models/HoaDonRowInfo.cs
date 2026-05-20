namespace DAL.Models
{
    public class HoaDonRowInfo
    {
        public int MaDonHang { get; set; }

        public string MaHoaDon { get; set; } = string.Empty;

        public string NgayLap { get; set; } = string.Empty;

        public string KhachHang { get; set; } = string.Empty;

        public string NhanVien { get; set; } = string.Empty;

        public int SoVe { get; set; }

        public string TongTien { get; set; } = string.Empty;

        public string TrangThai { get; set; } = string.Empty;
    }
}
