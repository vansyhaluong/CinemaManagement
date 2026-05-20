namespace DAL.Models
{
    public class TicketInfo
    {
        public string TenRap { get; set; } = string.Empty;

        public string TenPhim { get; set; } = string.Empty;

        public DateTime? ThoiGianChieu { get; set; }

        public string TenPhong { get; set; } = string.Empty;

        public string Ghe { get; set; } = string.Empty;

        public string MaVe { get; set; } = string.Empty;

        public string MaHoaDon { get; set; } = string.Empty;

        public decimal GiaVe { get; set; }

        public DateTime? NgayLapVe { get; set; }

        public string NgayChieuText => ThoiGianChieu?.ToString("dd/MM/yyyy") ?? "";

        public string GioChieuText => ThoiGianChieu?.ToString("HH:mm") ?? "";

        public string NgayLapVeText => NgayLapVe?.ToString("dd/MM/yyyy HH:mm") ?? "";

        public string GiaVeText => GiaVe.ToString("N0") + " đ";
    }
}
