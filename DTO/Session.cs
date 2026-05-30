namespace DTO
{
    public static class Session
    {
        public static int MaTaiKhoan { get; private set; }
        public static int MaNhanVien { get; private set; }
        public static string HoTen { get; private set; } = string.Empty;
        public static string VaiTro { get; private set; } = string.Empty;
        public static int MaRap { get; private set; }
        public static string TenRap { get; private set; } = string.Empty;

        public static bool IsLoggedIn => MaTaiKhoan > 0;
        public static bool IsAdmin => string.Equals(VaiTro, "Admin", System.StringComparison.OrdinalIgnoreCase);

        public static void Set(int maTaiKhoan, int maNhanVien, string? hoTen, string? vaiTro, int maRap, string? tenRap)
        {
            MaTaiKhoan = maTaiKhoan;
            MaNhanVien = maNhanVien;
            HoTen = hoTen ?? string.Empty;
            VaiTro = vaiTro ?? string.Empty;
            MaRap = maRap;
            TenRap = tenRap ?? string.Empty;
        }

        public static void Clear()
        {
            MaTaiKhoan = 0;
            MaNhanVien = 0;
            HoTen = string.Empty;
            VaiTro = string.Empty;
            MaRap = 0;
            TenRap = string.Empty;
        }
    }
}
