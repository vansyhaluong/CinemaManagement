using Cinema.Models;
using DTO;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class BaoCaoDoanhThuDAL
    {
        private readonly RapPhim2Context db = new RapPhim2Context();

        public List<BaoCaoDoanhThuNgayDTO> GetBaoCaoTheoNgay(DateTime ngay, int maRap)
        {
            var tuNgay = ngay.Date;
            var denNgay = tuNgay.AddDays(1);

            var donHangs = db.DonHangs
                .Include(dh => dh.VeBans)
                    .ThenInclude(v => v.MaSuatChieuNavigation)
                        .ThenInclude(sc => sc.MaPhongNavigation)
                            .ThenInclude(pc => pc.MaRapNavigation)
                .Where(dh => dh.NgayDat.HasValue
                         && dh.NgayDat.Value >= tuNgay
                         && dh.NgayDat.Value < denNgay
                         && dh.TrangThai != null
                         && (dh.TrangThai.Trim() == "Đã thanh toán" || dh.TrangThai.Trim() == "DaThanhToan"))
                .Where(dh => maRap == 0 || dh.VeBans.Any(v =>
                    v.MaSuatChieuNavigation != null &&
                    v.MaSuatChieuNavigation.MaPhongNavigation != null &&
                    v.MaSuatChieuNavigation.MaPhongNavigation.MaRap == maRap))
                .ToList();

            var data = donHangs
                .Select(dh =>
                {
                    var rapInfo = dh.VeBans
                        .Where(v => v.MaSuatChieuNavigation?.MaPhongNavigation != null)
                        .Select(v => new
                        {
                            MaRap = v.MaSuatChieuNavigation!.MaPhongNavigation!.MaRap,
                            TenRap = v.MaSuatChieuNavigation.MaPhongNavigation.MaRapNavigation != null
                                ? v.MaSuatChieuNavigation.MaPhongNavigation.MaRapNavigation.TenRap
                                : ""
                        })
                        .FirstOrDefault(x => maRap == 0 || x.MaRap == maRap);

                    return new
                    {
                        DonHang = dh,
                        RapInfo = rapInfo,
                        SoVeBan = dh.VeBans.Count(v =>
                            v.MaSuatChieuNavigation != null &&
                            v.MaSuatChieuNavigation.MaPhongNavigation != null &&
                            (maRap == 0 || v.MaSuatChieuNavigation.MaPhongNavigation.MaRap == maRap))
                    };
                })
                .Where(x => x.RapInfo != null)
                .GroupBy(x => new
                {
                    MaRap = x.RapInfo!.MaRap!.Value,
                    x.RapInfo.TenRap
                })
                .Select(g => new BaoCaoDoanhThuNgayDTO
                {
                    MaBaoCao = 0,
                    MaRap = g.Key.MaRap,
                    TenRap = g.Key.TenRap,
                    Ngay = tuNgay,
                    DoanhThuVe = g.Sum(x => x.DonHang.TongTienVe),
                    DoanhThuDichVu = g.Sum(x => x.DonHang.TongTienDichVu),
                    TongDoanhThu = g.Sum(x => x.DonHang.TongThanhToan),
                    SoDonHang = g.Count(),
                    SoVeBan = g.Sum(x => x.SoVeBan)
                })
                .OrderBy(x => x.TenRap)
                .ToList();

            return data;
        }

        public void TaoBaoCaoTheoNgay(DateTime ngay)
        {
            RebuildBaoCaoTheoNgay(db, ngay);
        }

        public List<BaoCaoDoanhThuReportDTO> GetChiTietReport(DateTime ngay, int maRap)
        {
            return db.DonHangs
                .Where(dh => dh.NgayDat.HasValue
                         && dh.NgayDat.Value.Date == ngay.Date
                         && (dh.TrangThai == "Đã thanh toán" || dh.TrangThai == "DaThanhToan"))
                .Where(dh => dh.VeBans.Any(v =>
                    v.MaSuatChieuNavigation!
                     .MaPhongNavigation!
                     .MaRap == maRap))
                .Select(dh => new BaoCaoDoanhThuReportDTO
                {
                    MaHoaDon = dh.MaHoaDon ?? "",
                    KhachHang = dh.MaKhachHangNavigation != null
                        ? dh.MaKhachHangNavigation.HoTen!
                        : "Khách lẻ",
                    TienVe = dh.TongTienVe,
                    TienDichVu = dh.TongTienDichVu,
                    TongTien = dh.TongThanhToan
                })
                .ToList();
        }

        public List<BaoCaoDoanhThuExcelDTO> GetChiTietExcel(DateTime ngay, int maRap)
        {
            DateTime tuNgay = ngay.Date;
            DateTime denNgay = tuNgay.AddDays(1);

            var donHangs = db.DonHangs
                .Include(dh => dh.MaKhachHangNavigation)
                .Include(dh => dh.VeBans)
                    .ThenInclude(v => v.MaGheNavigation)
                .Include(dh => dh.VeBans)
                    .ThenInclude(v => v.MaSuatChieuNavigation)
                        .ThenInclude(sc => sc.MaPhongNavigation)
                .Include(dh => dh.VeBans)
                    .ThenInclude(v => v.MaSuatChieuNavigation)
                        .ThenInclude(sc => sc.MaPhimNavigation)
                .Include(dh => dh.ChiTietDonHangs)
                    .ThenInclude(ct => ct.MaSanPhamNavigation)
                .Where(dh => dh.NgayDat.HasValue
                         && dh.NgayDat.Value >= tuNgay
                         && dh.NgayDat.Value < denNgay
                         && dh.TrangThai != null
                         && (dh.TrangThai.Trim() == "Đã thanh toán" || dh.TrangThai.Trim() == "DaThanhToan"))
                .ToList();

            var result = donHangs
                .Where(dh => dh.VeBans.Any(v =>
                    v.MaSuatChieuNavigation?.MaPhongNavigation?.MaRap == maRap))
                .Select(dh =>
                {
                    var veTheoRap = dh.VeBans
                        .Where(v => v.MaSuatChieuNavigation?.MaPhongNavigation?.MaRap == maRap)
                        .ToList();

                    return new BaoCaoDoanhThuExcelDTO
                    {
                        MaHoaDon = dh.MaHoaDon ?? "",
                        NgayDat = dh.NgayDat,
                        KhachHang = dh.MaKhachHangNavigation?.HoTen ?? "Khách lẻ",
                        SoDienThoai = dh.MaKhachHangNavigation?.SoDienThoai ?? "",
                        SoLuongVe = veTheoRap.Count,
                        Ghe = string.Join(", ", veTheoRap.Select(v =>
                            v.MaGheNavigation != null ? v.MaGheNavigation.SoGhe.ToString() : "")),
                        TenPhim = veTheoRap
                            .Select(v => v.MaSuatChieuNavigation?.MaPhimNavigation?.TieuDe ?? "")
                            .FirstOrDefault() ?? "",
                        SuatChieu = veTheoRap
                            .Select(v =>
                                v.MaSuatChieuNavigation != null &&
                                v.MaSuatChieuNavigation.ThoiGianBatDau.HasValue
                                    ? v.MaSuatChieuNavigation.ThoiGianBatDau.Value.ToString("dd/MM/yyyy HH:mm")
                                    : "")
                            .FirstOrDefault() ?? "",
                        DichVuDaMua = string.Join(", ",
                            dh.ChiTietDonHangs.Select(ct =>
                                (ct.MaSanPhamNavigation?.Ten ?? "") + " x" + ct.SoLuong)),
                        TienVe = dh.TongTienVe,
                        TienDichVu = dh.TongTienDichVu,
                        TongTien = dh.TongThanhToan
                    };
                })
                .ToList();

            return result;
        }

        public int CountDonHangTheoNgay(DateTime ngay, int maRap)
        {
            DateTime tuNgay = ngay.Date;
            DateTime denNgay = tuNgay.AddDays(1);

            return db.DonHangs.Count(dh =>
                dh.NgayDat.HasValue &&
                dh.NgayDat.Value >= tuNgay &&
                dh.NgayDat.Value < denNgay &&
                (maRap == 0 || dh.VeBans.Any(v =>
                    v.MaSuatChieuNavigation != null &&
                    v.MaSuatChieuNavigation.MaPhongNavigation != null &&
                    v.MaSuatChieuNavigation.MaPhongNavigation.MaRap == maRap)));
        }

        public int CountDonHangDaThanhToan(DateTime ngay, int maRap)
        {
            DateTime tuNgay = ngay.Date;
            DateTime denNgay = tuNgay.AddDays(1);

            return db.DonHangs.Count(dh =>
                dh.NgayDat.HasValue &&
                dh.NgayDat.Value >= tuNgay &&
                dh.NgayDat.Value < denNgay &&
                dh.TrangThai != null &&
                (dh.TrangThai.Trim() == "Đã thanh toán" || dh.TrangThai.Trim() == "DaThanhToan") &&
                (maRap == 0 || dh.VeBans.Any(v =>
                    v.MaSuatChieuNavigation != null &&
                    v.MaSuatChieuNavigation.MaPhongNavigation != null &&
                    v.MaSuatChieuNavigation.MaPhongNavigation.MaRap == maRap)));
        }

        public static void RebuildBaoCaoTheoNgay(RapPhim2Context db, DateTime ngay)
        {
            var ngayBaoCao = DateOnly.FromDateTime(ngay);

            var data = db.DonHangs
                .Where(dh => dh.NgayDat.HasValue
                         && dh.NgayDat.Value.Date == ngay.Date
                         && (dh.TrangThai == "Đã thanh toán" || dh.TrangThai == "DaThanhToan"))
                .SelectMany(dh => dh.VeBans.Select(v => new
                {
                    DonHang = dh,
                    Ve = v,
                    MaRap = v.MaSuatChieuNavigation!.MaPhongNavigation!.MaRap
                }))
                .Where(x => x.MaRap != null)
                .GroupBy(x => x.MaRap)
                .Select(g => new
                {
                    MaRap = g.Key!.Value,
                    DoanhThuVe = g.Select(x => x.DonHang).Distinct().Sum(x => x.TongTienVe),
                    DoanhThuDichVu = g.Select(x => x.DonHang).Distinct().Sum(x => x.TongTienDichVu),
                    TongDoanhThu = g.Select(x => x.DonHang).Distinct().Sum(x => x.TongThanhToan),
                    SoDonHang = g.Select(x => x.DonHang.MaDonHang).Distinct().Count(),
                    SoVeBan = g.Count()
                })
                .ToList();

            var baoCaosCu = db.BaoCaoDoanhThuNgays
                .Where(x => x.Ngay == ngayBaoCao)
                .ToList();

            foreach (var baoCaoCu in baoCaosCu)
            {
                if (!data.Any(x => x.MaRap == baoCaoCu.MaRap))
                {
                    db.BaoCaoDoanhThuNgays.Remove(baoCaoCu);
                }
            }

            foreach (var item in data)
            {
                var baoCao = baoCaosCu.FirstOrDefault(x => x.MaRap == item.MaRap);

                if (baoCao == null)
                {
                    baoCao = new BaoCaoDoanhThuNgay
                    {
                        MaRap = item.MaRap,
                        Ngay = ngayBaoCao
                    };

                    db.BaoCaoDoanhThuNgays.Add(baoCao);
                }

                baoCao.DoanhThuVe = item.DoanhThuVe;
                baoCao.DoanhThuDichVu = item.DoanhThuDichVu;
                baoCao.TongDoanhThu = item.TongDoanhThu;
                baoCao.SoDonHang = item.SoDonHang;
                baoCao.SoVeBan = item.SoVeBan;
            }

            db.SaveChanges();
        }
    }
}
