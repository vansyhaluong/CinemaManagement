using Cinema.Models;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class HoaDonDAL
    {
        public List<HoaDonRowInfo> GetHoaDons(string? maHoaDon, string? soDienThoai, DateTime? tuNgay, DateTime? denNgay, int? maRap = null)
        {
            using var db = new RapPhim2Context();

            var query = db.DonHangs
                .Include(x => x.MaKhachHangNavigation)
                .Include(x => x.MaNhanVienNavigation)
                .Include(x => x.VeBans)
                    .ThenInclude(v => v.MaSuatChieuNavigation)
                        .ThenInclude(s => s!.MaPhongNavigation)
                .AsQueryable();

            if (maRap.HasValue && maRap.Value > 0)
            {
                query = query.Where(x => x.VeBans.Any(v =>
                    v.MaSuatChieuNavigation != null &&
                    v.MaSuatChieuNavigation.MaPhongNavigation != null &&
                    v.MaSuatChieuNavigation.MaPhongNavigation.MaRap == maRap.Value));
            }

            if (!string.IsNullOrWhiteSpace(maHoaDon))
            {
                var keyword = maHoaDon.Trim();
                query = query.Where(x => (x.MaHoaDon ?? "").Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(soDienThoai))
            {
                var phone = soDienThoai.Trim();
                query = query.Where(x => (x.MaKhachHangNavigation!.SoDienThoai ?? "").Contains(phone));
            }

            if (tuNgay.HasValue)
            {
                var from = tuNgay.Value.Date;
                query = query.Where(x => x.NgayDat.HasValue && x.NgayDat.Value.Date >= from);
            }

            if (denNgay.HasValue)
            {
                var to = denNgay.Value.Date;
                query = query.Where(x => x.NgayDat.HasValue && x.NgayDat.Value.Date <= to);
            }

            return query
                .OrderByDescending(x => x.NgayDat)
                .AsEnumerable()
                .Select(x => new HoaDonRowInfo
                {
                    MaDonHang = x.MaDonHang,
                    MaHoaDon = x.MaHoaDon ?? $"HD{x.MaDonHang}",
                    NgayLap = x.NgayDat?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    KhachHang = TaoKhachHangText(x.MaKhachHangNavigation?.HoTen, x.MaKhachHangNavigation?.SoDienThoai),
                    NhanVien = x.MaNhanVienNavigation?.HoTen ?? "N/A",
                    SoVe = x.VeBans.Count(v => v.TrangThai != "Đã hủy" && v.TrangThai != "DaHuy"),
                    TongTien = x.TongThanhToan.ToString("N0") + " đ",
                    TrangThai = ChuanHoaTrangThai(x.TrangThai)
                })
                .ToList();
        }

        public HoaDonDetailInfo? GetHoaDonDetail(int maDonHang, int? maRap = null)
        {
            using var db = new RapPhim2Context();

            var donHang = db.DonHangs
                .Include(x => x.MaKhachHangNavigation)
                .Include(x => x.VeBans)
                    .ThenInclude(v => v.MaSuatChieuNavigation)
                        .ThenInclude(s => s!.MaPhimNavigation)
                .Include(x => x.VeBans)
                    .ThenInclude(v => v.MaSuatChieuNavigation)
                        .ThenInclude(s => s!.MaPhongNavigation)
                .Include(x => x.VeBans)
                    .ThenInclude(v => v.MaGheNavigation)
                .Include(x => x.ChiTietDonHangs)
                    .ThenInclude(c => c.MaSanPhamNavigation)
                .FirstOrDefault(x => x.MaDonHang == maDonHang);

            if (donHang == null)
                return null;

            if (maRap.HasValue && maRap.Value > 0)
            {
                var thuocRapDangNhap = donHang.VeBans.Any(v =>
                    v.MaSuatChieuNavigation != null &&
                    v.MaSuatChieuNavigation.MaPhongNavigation != null &&
                    v.MaSuatChieuNavigation.MaPhongNavigation.MaRap == maRap.Value);

                if (!thuocRapDangNhap)
                    return null;
            }

            var veDau = donHang.VeBans
                .Where(v => v.TrangThai != "Đã hủy" && v.TrangThai != "DaHuy")
                .OrderBy(v => v.MaDatVe)
                .FirstOrDefault()
                ?? donHang.VeBans.OrderBy(v => v.MaDatVe).FirstOrDefault();

            return new HoaDonDetailInfo
            {
                MaDonHang = donHang.MaDonHang,
                MaHoaDon = donHang.MaHoaDon ?? $"HD{donHang.MaDonHang}",
                KhachHang = TaoKhachHangText(donHang.MaKhachHangNavigation?.HoTen, donHang.MaKhachHangNavigation?.SoDienThoai),
                NgayLap = donHang.NgayDat?.ToString("dd/MM/yyyy HH:mm") ?? "--",
                TenPhim = veDau?.MaSuatChieuNavigation?.MaPhimNavigation?.TieuDe ?? "Chưa có dữ liệu",
                SuatChieu = veDau?.MaSuatChieuNavigation?.ThoiGianBatDau?.ToString("dd/MM/yyyy HH:mm") ?? "--",
                PhongChieu = veDau?.MaSuatChieuNavigation?.MaPhongNavigation?.TenPhong ?? "--",
                Ghe = string.Join(", ", donHang.VeBans
                    .Where(v => v.TrangThai != "Đã hủy" && v.TrangThai != "DaHuy")
                    .OrderBy(v => v.MaGheNavigation!.HangGhe)
                    .ThenBy(v => v.MaGheNavigation!.SoGhe)
                    .Select(v => $"{v.MaGheNavigation!.HangGhe}{v.MaGheNavigation!.SoGhe}")),
                DichVus = donHang.ChiTietDonHangs.Any()
                    ? donHang.ChiTietDonHangs
                        .Select(x => $"{x.MaSanPhamNavigation?.Ten} x{x.SoLuong} - {(x.ThanhTien ?? ((x.Gia ?? 0) * (x.SoLuong ?? 0))):N0} đ")
                        .ToList()
                    : new List<string>(),
                TongTienValue = donHang.TongThanhToan,
                TrangThai = ChuanHoaTrangThai(donHang.TrangThai)
            };
        }

        public bool HuyHoaDon(int maDonHang, string lyDo, int maNhanVien = 1, int? maRap = null)
        {
            using var db = new RapPhim2Context();
            using var transaction = db.Database.BeginTransaction();

            try
            {
                var donHang = db.DonHangs
                    .Include(x => x.VeBans)
                        .ThenInclude(v => v.MaSuatChieuNavigation)
                            .ThenInclude(s => s!.MaPhongNavigation)
                    .Include(x => x.ChiTietDonHangs)
                        .ThenInclude(c => c.MaSanPhamNavigation)
                            .ThenInclude(s => s!.Kho)
                    .Include(x => x.ThanhToans)
                    .FirstOrDefault(x => x.MaDonHang == maDonHang);

                if (donHang == null)
                    return false;

                if (maRap.HasValue && maRap.Value > 0)
                {
                    var thuocRapDangNhap = donHang.VeBans.Any(v =>
                        v.MaSuatChieuNavigation != null &&
                        v.MaSuatChieuNavigation.MaPhongNavigation != null &&
                        v.MaSuatChieuNavigation.MaPhongNavigation.MaRap == maRap.Value);

                    if (!thuocRapDangNhap)
                        return false;
                }

                if (donHang.TrangThai == "Đã hủy" || donHang.TrangThai == "DaHuy")
                    return true;

                var lyDoLuu = string.IsNullOrWhiteSpace(lyDo)
                    ? "Hủy từ quản lý hóa đơn"
                    : lyDo.Trim();

                foreach (var ve in donHang.VeBans.Where(v => v.TrangThai != "Đã hủy" && v.TrangThai != "DaHuy"))
                {
                    ve.TrangThai = "Đã hủy";
                    db.HoanVes.Add(new HoanVe
                    {
                        MaDatVe = ve.MaDatVe,
                        NgayHoan = DateTime.Now,
                        LyDo = lyDoLuu,
                        SoTienHoan = ve.Gia ?? 0,
                        TrangThai = "Đã hoàn",
                        MaNhanVien = maNhanVien
                    });
                }

                foreach (var ct in donHang.ChiTietDonHangs)
                {
                    if (ct.MaSanPhamNavigation?.Kho != null)
                    {
                        ct.MaSanPhamNavigation.Kho.SoLuongTon = (ct.MaSanPhamNavigation.Kho.SoLuongTon ?? 0) + (ct.SoLuong ?? 0);
                    }
                }

                foreach (var thanhToan in donHang.ThanhToans)
                {
                    thanhToan.TrangThai = "Đã hủy";
                }

                if (donHang.MaKhuyenMai.HasValue)
                {
                    var khuyenMai = db.KhuyenMais.FirstOrDefault(x => x.MaKhuyenMai == donHang.MaKhuyenMai.Value);
                    if (khuyenMai != null && (khuyenMai.DaDung ?? 0) > 0)
                    {
                        khuyenMai.DaDung = (khuyenMai.DaDung ?? 0) - 1;
                    }
                }

                donHang.TrangThai = "Đã hủy";
                db.SaveChanges();

                if (donHang.NgayDat.HasValue)
                {
                    BaoCaoDoanhThuDAL.RebuildBaoCaoTheoNgay(db, donHang.NgayDat.Value);
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }

        private static string TaoKhachHangText(string? hoTen, string? soDienThoai)
        {
            if (string.IsNullOrWhiteSpace(hoTen) && string.IsNullOrWhiteSpace(soDienThoai))
                return "Khách lẻ";

            if (string.IsNullOrWhiteSpace(soDienThoai))
                return hoTen ?? "Khách lẻ";

            return $"{hoTen ?? "Khách lẻ"} / {soDienThoai}";
        }

        private static string ChuanHoaTrangThai(string? trangThai)
        {
            return trangThai switch
            {
                "DaThanhToan" => "Đã thanh toán",
                "DaHuy" => "Đã hủy",
                "DaBan" => "Đã bán",
                "DaHoan" => "Đã hoàn",
                "ThanhCong" => "Thành công",
                _ => string.IsNullOrWhiteSpace(trangThai) ? "Chưa xác định" : trangThai
            };
        }
    }
}

