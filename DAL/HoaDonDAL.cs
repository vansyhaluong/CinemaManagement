using Cinema.Models;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class HoaDonDAL
    {
        public List<HoaDonRowInfo> GetHoaDons(string? maHoaDon, string? soDienThoai, DateTime? tuNgay, DateTime? denNgay)
        {
            using var db = new RapPhim2Context();

            var query = db.DonHangs
                .Include(x => x.MaKhachHangNavigation)
                .Include(x => x.MaNhanVienNavigation)
                .Include(x => x.VeBans)
                .AsQueryable();

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
                    SoVe = x.VeBans.Count(v => v.TrangThai != "DaHuy"),
                    TongTien = x.TongThanhToan.ToString("N0") + " đ",
                    TrangThai = ChuanHoaTrangThai(x.TrangThai)
                })
                .ToList();
        }

        public HoaDonDetailInfo? GetHoaDonDetail(int maDonHang)
        {
            using var db = new RapPhim2Context();

            var donHang = db.DonHangs
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

            var veDau = donHang.VeBans
                .Where(v => v.TrangThai != "DaHuy")
                .OrderBy(v => v.MaDatVe)
                .FirstOrDefault();

            return new HoaDonDetailInfo
            {
                MaDonHang = donHang.MaDonHang,
                MaHoaDon = donHang.MaHoaDon ?? $"HD{donHang.MaDonHang}",
                TenPhim = veDau?.MaSuatChieuNavigation?.MaPhimNavigation?.TieuDe ?? "Chưa có dữ liệu",
                SuatChieu = veDau?.MaSuatChieuNavigation?.ThoiGianBatDau?.ToString("dd/MM/yyyy HH:mm") ?? "--",
                PhongChieu = veDau?.MaSuatChieuNavigation?.MaPhongNavigation?.TenPhong ?? "--",
                Ghe = string.Join(", ", donHang.VeBans
                    .Where(v => v.TrangThai != "DaHuy")
                    .OrderBy(v => v.MaGheNavigation!.HangGhe)
                    .ThenBy(v => v.MaGheNavigation!.SoGhe)
                    .Select(v => $"{v.MaGheNavigation!.HangGhe}{v.MaGheNavigation!.SoGhe}")),
                DichVus = donHang.ChiTietDonHangs
                    .Select(x => $"{x.MaSanPhamNavigation?.Ten} x{x.SoLuong} - {(x.ThanhTien ?? ((x.Gia ?? 0) * (x.SoLuong ?? 0))):N0} đ")
                    .ToList(),
                TongTienValue = donHang.TongThanhToan,
                TrangThai = ChuanHoaTrangThai(donHang.TrangThai)
            };
        }

        public bool HuyHoaDon(int maDonHang, int maNhanVien = 1)
        {
            using var db = new RapPhim2Context();
            using var transaction = db.Database.BeginTransaction();

            try
            {
                var donHang = db.DonHangs
                    .Include(x => x.VeBans)
                    .Include(x => x.ChiTietDonHangs)
                        .ThenInclude(c => c.MaSanPhamNavigation)
                            .ThenInclude(s => s!.Kho)
                    .Include(x => x.ThanhToans)
                    .FirstOrDefault(x => x.MaDonHang == maDonHang);

                if (donHang == null)
                    return false;

                if (donHang.TrangThai == "DaHuy")
                    return true;

                foreach (var ve in donHang.VeBans.Where(v => v.TrangThai != "DaHuy"))
                {
                    ve.TrangThai = "DaHuy";
                    db.HoanVes.Add(new HoanVe
                    {
                        MaDatVe = ve.MaDatVe,
                        NgayHoan = DateTime.Now,
                        LyDo = "Hủy từ quản lý hóa đơn",
                        SoTienHoan = ve.Gia ?? 0,
                        TrangThai = "DaHoan",
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
                    thanhToan.TrangThai = "DaHuy";
                }

                donHang.TrangThai = "DaHuy";
                db.SaveChanges();
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
                "ThanhCong" => "Thành công",
                _ => string.IsNullOrWhiteSpace(trangThai) ? "Chưa xác định" : trangThai
            };
        }
    }
}
