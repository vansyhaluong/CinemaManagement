namespace DAL.Models
{
    public class HoaDonDetailInfo
    {
        public int MaDonHang { get; set; }

        public string MaHoaDon { get; set; } = string.Empty;

        public string KhachHang { get; set; } = string.Empty;

        public string NgayLap { get; set; } = string.Empty;

        public string TenPhim { get; set; } = string.Empty;

        public string SuatChieu { get; set; } = string.Empty;

        public string PhongChieu { get; set; } = string.Empty;

        public string Ghe { get; set; } = string.Empty;

        public List<string> DichVus { get; set; } = new List<string>();

        public decimal TongTienValue { get; set; }

        public string TongTien => TongTienValue.ToString("N0") + " đ";

        public string TrangThai { get; set; } = string.Empty;
    }
}
