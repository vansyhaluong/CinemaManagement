using Cinema.Models;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class BanVeDAL
    {
        public List<GheBanVeInfo> GetGheTheoSuatChieu(int maSuatChieu)
        {
            using var db = new RapPhim2Context();
            XoaGiuGheHetHan(db);

            var suat = db.SuatChieus
                .Include(s => s.MaPhongNavigation)
                .FirstOrDefault(s => s.MaSuatChieu == maSuatChieu);

            if (suat?.MaPhong == null)
                return new List<GheBanVeInfo>();

            var now = DateTime.Now;
            var gheDaBan = db.VeBans
                .Where(v => v.MaSuatChieu == maSuatChieu && v.TrangThai != "DaHuy" && v.TrangThai != "DaHoan")
                .Select(v => v.MaGhe)
                .ToHashSet();

            var gheDangGiu = db.GiuGheTams
                .Where(g => g.MaSuatChieu == maSuatChieu && g.TrangThai == "DangGiu" && g.HetHanLuc > now)
                .ToDictionary(g => g.MaGhe, g => g.HetHanLuc);

            return db.Ghes
                .Include(g => g.MaLoaiGheNavigation)
                .Where(g => g.MaPhong == suat.MaPhong)
                .OrderBy(g => g.HangGhe)
                .ThenBy(g => g.SoGhe)
                .AsEnumerable()
                .Select(g =>
                {
                    var trangThai = "Trong";
                    var giayConLai = 0;

                    if (g.TrangThai == "Bảo trì")
                    {
                        trangThai = "BaoTri";
                    }
                    else if (gheDaBan.Contains(g.MaGhe))
                    {
                        trangThai = "DaBan";
                    }
                    else if (gheDangGiu.TryGetValue(g.MaGhe, out var hetHan))
                    {
                        trangThai = "DangGiu";
                        giayConLai = Math.Max(0, (int)(hetHan - now).TotalSeconds);
                    }

                    return new GheBanVeInfo
                    {
                        MaGhe = g.MaGhe,
                        MaPhong = g.MaPhong,
                        HangGhe = g.HangGhe,
                        SoGhe = g.SoGhe,
                        MaLoaiGhe = g.MaLoaiGhe,
                        TenLoaiGhe = g.MaLoaiGheNavigation?.TenLoai,
                        Gia = Math.Round(suat.GiaVeCoBan * (g.MaLoaiGheNavigation?.HeSoGia ?? 1m), 0),
                        TrangThai = trangThai,
                        GiayConLai = giayConLai
                    };
                })
                .ToList();
        }

        public bool GiuGhe(int maSuatChieu, int maGhe)
        {
            using var db = new RapPhim2Context();
            XoaGiuGheHetHan(db);

            var daBan = db.VeBans.Any(v =>
                v.MaSuatChieu == maSuatChieu &&
                v.MaGhe == maGhe &&
                v.TrangThai != "DaHuy" &&
                v.TrangThai != "DaHoan");

            if (daBan)
                return false;

            var daGiu = db.GiuGheTams.Any(g =>
                g.MaSuatChieu == maSuatChieu &&
                g.MaGhe == maGhe &&
                g.TrangThai == "DangGiu" &&
                g.HetHanLuc > DateTime.Now);

            if (daGiu)
                return false;

            var now = DateTime.Now;
            db.GiuGheTams.Add(new GiuGheTam
            {
                MaSuatChieu = maSuatChieu,
                MaGhe = maGhe,
                ThoiGianGiu = now,
                HetHanLuc = now.AddMinutes(1),
                TrangThai = "DangGiu"
            });

            db.SaveChanges();
            return true;
        }

        public void BoGiuGhe(int maSuatChieu, int maGhe)
        {
            using var db = new RapPhim2Context();
            var item = db.GiuGheTams.FirstOrDefault(g =>
                g.MaSuatChieu == maSuatChieu &&
                g.MaGhe == maGhe &&
                g.TrangThai == "DangGiu");

            if (item != null)
            {
                db.GiuGheTams.Remove(item);
                db.SaveChanges();
            }
        }

        public KetQuaBanVe ThanhToan(
            int maSuatChieu,
            IEnumerable<int> maGhes,
            IEnumerable<DichVuTam> dichVus,
            string hoTen,
            string soDienThoai,
            string phuongThuc,
            int? maNhanVien = 1)
        {
            using var db = new RapPhim2Context();
            using var transaction = db.Database.BeginTransaction();

            try
            {
                XoaGiuGheHetHan(db);

                var dsGhe = maGhes.Distinct().ToList();
                if (!dsGhe.Any())
                    return Loi("Vui lòng chọn ít nhất một ghế.");

                var suat = db.SuatChieus.FirstOrDefault(s => s.MaSuatChieu == maSuatChieu);
                if (suat == null)
                    return Loi("Không tìm thấy suất chiếu.");

                var daBan = db.VeBans.Any(v =>
                    v.MaSuatChieu == maSuatChieu &&
                    dsGhe.Contains(v.MaGhe ?? 0) &&
                    v.TrangThai != "DaHuy" &&
                    v.TrangThai != "DaHoan");

                if (daBan)
                    return Loi("Có ghế vừa được bán. Vui lòng chọn lại.");

                var soGheDangGiu = db.GiuGheTams.Count(g =>
                    g.MaSuatChieu == maSuatChieu &&
                    dsGhe.Contains(g.MaGhe) &&
                    g.TrangThai == "DangGiu" &&
                    g.HetHanLuc > DateTime.Now);

                if (soGheDangGiu != dsGhe.Count)
                    return Loi("Thời gian giữ ghế đã hết. Vui lòng chọn lại ghế.");

                var khachHang = TimHoacTaoKhachHang(db, hoTen, soDienThoai);
                var maHoaDon = "HD" + DateTime.Now.ToString("yyyyMMddHHmmss");

                var donHang = new DonHang
                {
                    MaHoaDon = maHoaDon,
                    MaKhachHang = khachHang.MaKhachHang,
                    NgayDat = DateTime.Now,
                    TrangThai = "DaThanhToan",
                    MaNhanVien = maNhanVien,
                    TongTienDichVu = 0,
                    TongTienVe = 0,
                    TienGiam = 0,
                    TongThanhToan = 0
                };

                db.DonHangs.Add(donHang);
                db.SaveChanges();

                var gheInfos = GetGheTinhTien(db, maSuatChieu, dsGhe);
                foreach (var ghe in gheInfos)
                {
                    var maVe = $"VE{DateTime.Now:yyyyMMdd}{donHang.MaDonHang:D4}{ghe.MaGhe:D3}";
                    db.VeBans.Add(new VeBan
                    {
                        MaVe = maVe,
                        MaDonHang = donHang.MaDonHang,
                        MaSuatChieu = maSuatChieu,
                        MaGhe = ghe.MaGhe,
                        Gia = ghe.Gia,
                        TrangThai = "DaBan"
                    });
                }

                var tongDichVu = ThemDichVu(db, donHang.MaDonHang, dichVus);
                var tongVe = gheInfos.Sum(g => g.Gia);

                donHang.TongTienVe = tongVe;
                donHang.TongTienDichVu = tongDichVu;
                donHang.TongThanhToan = tongVe + tongDichVu - donHang.TienGiam;

                db.ThanhToans.Add(new ThanhToan
                {
                    MaDonHang = donHang.MaDonHang,
                    PhuongThuc = phuongThuc,
                    NgayThanhToan = DateTime.Now,
                    SoTien = donHang.TongThanhToan,
                    TrangThai = "ThanhCong"
                });

                var holds = db.GiuGheTams.Where(g => g.MaSuatChieu == maSuatChieu && dsGhe.Contains(g.MaGhe));
                db.GiuGheTams.RemoveRange(holds);

                db.SaveChanges();
                transaction.Commit();

                return new KetQuaBanVe
                {
                    ThanhCong = true,
                    ThongBao = "Thanh toán thành công.",
                    MaDonHang = donHang.MaDonHang,
                    MaHoaDon = maHoaDon
                };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Loi("Không thể thanh toán: " + ex.Message);
            }
        }

        public List<TicketInfo> GetVeTheoDonHang(int maDonHang)
        {
            using var db = new RapPhim2Context();

            return db.VeBans
                .Include(v => v.MaDonHangNavigation)
                .Include(v => v.MaSuatChieuNavigation)
                    .ThenInclude(s => s!.MaPhimNavigation)
                .Include(v => v.MaSuatChieuNavigation)
                    .ThenInclude(s => s!.MaPhongNavigation)
                        .ThenInclude(p => p!.MaRapNavigation)
                .Include(v => v.MaGheNavigation)
                .Where(v => v.MaDonHang == maDonHang)
                .OrderBy(v => v.MaGheNavigation!.HangGhe)
                .ThenBy(v => v.MaGheNavigation!.SoGhe)
                .Select(v => new TicketInfo
                {
                    TenRap = v.MaSuatChieuNavigation!.MaPhongNavigation!.MaRapNavigation!.TenRap ?? "",
                    TenPhim = v.MaSuatChieuNavigation!.MaPhimNavigation!.TieuDe ?? "",
                    ThoiGianChieu = v.MaSuatChieuNavigation!.ThoiGianBatDau,
                    TenPhong = v.MaSuatChieuNavigation!.MaPhongNavigation!.TenPhong ?? "",
                    Ghe = (v.MaGheNavigation!.HangGhe ?? "") + v.MaGheNavigation!.SoGhe,
                    MaVe = v.MaVe ?? v.MaDatVe.ToString(),
                    MaHoaDon = v.MaDonHangNavigation!.MaHoaDon ?? "",
                    GiaVe = v.Gia ?? 0,
                    NgayLapVe = v.MaDonHangNavigation!.NgayDat
                })
                .ToList();
        }

        private static KhachHang TimHoacTaoKhachHang(RapPhim2Context db, string hoTen, string soDienThoai)
        {
            var sdt = soDienThoai.Trim();
            var kh = db.KhachHangs.FirstOrDefault(k => k.SoDienThoai == sdt);
            if (kh != null)
            {
                if (!string.IsNullOrWhiteSpace(hoTen))
                    kh.HoTen = hoTen.Trim();

                return kh;
            }

            kh = new KhachHang
            {
                HoTen = string.IsNullOrWhiteSpace(hoTen) ? "Khách lẻ" : hoTen.Trim(),
                SoDienThoai = string.IsNullOrWhiteSpace(sdt) ? null : sdt,
                NgayTao = DateTime.Now
            };
            db.KhachHangs.Add(kh);
            db.SaveChanges();
            return kh;
        }

        private static List<GheBanVeInfo> GetGheTinhTien(RapPhim2Context db, int maSuatChieu, List<int> maGhes)
        {
            var suat = db.SuatChieus.First(s => s.MaSuatChieu == maSuatChieu);

            return db.Ghes
                .Include(g => g.MaLoaiGheNavigation)
                .Where(g => maGhes.Contains(g.MaGhe))
                .AsEnumerable()
                .Select(g => new GheBanVeInfo
                {
                    MaGhe = g.MaGhe,
                    Gia = Math.Round(suat.GiaVeCoBan * (g.MaLoaiGheNavigation?.HeSoGia ?? 1m), 0)
                })
                .ToList();
        }

        private static decimal ThemDichVu(RapPhim2Context db, int maDonHang, IEnumerable<DichVuTam> dichVus)
        {
            decimal tong = 0;
            foreach (var dv in dichVus.Where(d => d.MaSanPham > 0 && d.SoLuong > 0))
            {
                var sanPham = db.SanPhams.Include(s => s.Kho).FirstOrDefault(s => s.MaSanPham == dv.MaSanPham);
                if (sanPham == null)
                    continue;

                if (sanPham.Kho != null)
                {
                    if ((sanPham.Kho.SoLuongTon ?? 0) < dv.SoLuong)
                        throw new InvalidOperationException($"Sản phẩm {sanPham.Ten} không đủ tồn kho.");

                    sanPham.Kho.SoLuongTon -= dv.SoLuong;
                }

                var thanhTien = dv.SoLuong * sanPham.Gia;
                tong += thanhTien;
                db.ChiTietDonHangs.Add(new ChiTietDonHang
                {
                    MaDonHang = maDonHang,
                    MaSanPham = dv.MaSanPham,
                    SoLuong = dv.SoLuong,
                    Gia = sanPham.Gia,
                    ThanhTien = thanhTien
                });
            }

            return tong;
        }

        private static void XoaGiuGheHetHan(RapPhim2Context db)
        {
            var now = DateTime.Now;
            var expired = db.GiuGheTams.Where(g => g.TrangThai == "DangGiu" && g.HetHanLuc <= now);
            db.GiuGheTams.RemoveRange(expired);
            db.SaveChanges();
        }

        private static KetQuaBanVe Loi(string thongBao)
        {
            return new KetQuaBanVe
            {
                ThanhCong = false,
                ThongBao = thongBao
            };
        }
    }
}
