namespace DAL.Models
{
    public class KhuyenMaiApDungInfo
    {
        public bool ThanhCong { get; set; }
        public string ThongBao { get; set; } = string.Empty;
        public int? MaKhuyenMai { get; set; }
        public string MaCode { get; set; } = string.Empty;
        public string TenKhuyenMai { get; set; } = string.Empty;
        public decimal SoTienGiam { get; set; }
    }
}
