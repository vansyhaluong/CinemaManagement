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
                .Where(v => v.MaSuatChieu == maSuatChieu && v.TrangThai != "Đã hủy" && v.TrangThai != "DaHuy" && v.TrangThai != "Đã hoàn" && v.TrangThai != "DaHoan")
                .Select(v => v.MaGhe)
                .ToHashSet();

            var gheDangGiu = db.GiuGheTams
                .Where(g => g.MaSuatChieu == maSuatChieu && (g.TrangThai == "Đang giữ" || g.TrangThai == "DangGiu") && g.HetHanLuc > now)
                .ToDictionary(g => g.MaGhe, g => g.HetHanLuc);

            return db.Ghes
                .Include(g => g.MaLoaiGheNavigation)
                .Where(g => g.MaPhong == suat.MaPhong)
                .OrderBy(g => g.HangGhe)
                .ThenBy(g => g.SoGhe)
                .AsEnumerable()
                .Select(g =>
                {
                    var trangThai = "Trống";
                    var giayConLai = 0;

                    if (g.TrangThai == "Bảo trì")
                    {
                        trangThai = "Bảo trì";
                    }
                    else if (gheDaBan.Contains(g.MaGhe))
                    {
                        trangThai = "Đã bán";
                    }
                    else if (gheDangGiu.TryGetValue(g.MaGhe, out var hetHan))
                    {
                        trangThai = "Đang giữ";
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
                v.TrangThai != "Đã hủy" &&
                v.TrangThai != "DaHuy" &&
                v.TrangThai != "Đã hoàn" &&
                v.TrangThai != "DaHoan");

            if (daBan)
                return false;

            var daGiu = db.GiuGheTams.Any(g =>
                g.MaSuatChieu == maSuatChieu &&
                g.MaGhe == maGhe &&
                (g.TrangThai == "Đang giữ" || g.TrangThai == "DangGiu") &&
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
                TrangThai = "Đang giữ"
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
                (g.TrangThai == "Đang giữ" || g.TrangThai == "DangGiu"));

            if (item != null)
            {
                db.GiuGheTams.Remove(item);
                db.SaveChanges();
            }
        }

        public KhuyenMaiApDungInfo KiemTraKhuyenMai(string maCode, decimal tamTinh)
        {
            using var db = new RapPhim2Context();
            return KiemTraKhuyenMaiNoiBo(db, maCode, tamTinh);
        }

        public KetQuaBanVe ThanhToan(
            int maSuatChieu,
            IEnumerable<int> maGhes,
            IEnumerable<DichVuTam> dichVus,
            string hoTen,
            string soDienThoai,
            string phuongThuc,
            string? maKhuyenMaiCode = null,
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
                    v.TrangThai != "Đã hủy" &&
                    v.TrangThai != "DaHuy" &&
                    v.TrangThai != "Đã hoàn" &&
                    v.TrangThai != "DaHoan");

                if (daBan)
                    return Loi("Có ghế vừa được bán. Vui lòng chọn lại.");

                var soGheDangGiu = db.GiuGheTams.Count(g =>
                    g.MaSuatChieu == maSuatChieu &&
                    dsGhe.Contains(g.MaGhe) &&
                    (g.TrangThai == "Đang giữ" || g.TrangThai == "DangGiu") &&
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
                    TrangThai = "Đã thanh toán",
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
                        TrangThai = "Đã bán"
                    });
                }

                var tongDichVu = ThemDichVu(db, donHang.MaDonHang, dichVus);
                var tongVe = gheInfos.Sum(g => g.Gia);
                var tamTinh = tongVe + tongDichVu;

                KhuyenMaiApDungInfo? kmInfo = null;
                if (!string.IsNullOrWhiteSpace(maKhuyenMaiCode))
                {
                    kmInfo = KiemTraKhuyenMaiNoiBo(db, maKhuyenMaiCode, tamTinh);
                    if (!kmInfo.ThanhCong || !kmInfo.MaKhuyenMai.HasValue)
                        return Loi(kmInfo.ThongBao);
                }

                donHang.TongTienVe = tongVe;
                donHang.TongTienDichVu = tongDichVu;
                donHang.MaKhuyenMai = kmInfo?.MaKhuyenMai;
                donHang.TienGiam = kmInfo?.SoTienGiam ?? 0;
                donHang.TongThanhToan = tamTinh - donHang.TienGiam;

                if (donHang.TongThanhToan < 0)
                {
                    donHang.TongThanhToan = 0;
                }

                if (kmInfo?.MaKhuyenMai.HasValue == true)
                {
                    var khuyenMai = db.KhuyenMais.First(x => x.MaKhuyenMai == kmInfo.MaKhuyenMai.Value);
                    khuyenMai.DaDung = (khuyenMai.DaDung ?? 0) + 1;
                }

                db.ThanhToans.Add(new ThanhToan
                {
                    MaDonHang = donHang.MaDonHang,
                    PhuongThuc = phuongThuc,
                    NgayThanhToan = DateTime.Now,
                    SoTien = donHang.TongThanhToan,
                    TrangThai = "Thành công"
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

        private static KhuyenMaiApDungInfo KiemTraKhuyenMaiNoiBo(RapPhim2Context db, string maCode, decimal tamTinh)
        {
            var code = (maCode ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(code))
            {
                return new KhuyenMaiApDungInfo
                {
                    ThanhCong = false,
                    ThongBao = "Vui lòng nhập mã khuyến mãi."
                };
            }

            var km = db.KhuyenMais.FirstOrDefault(x =>
                x.MaCode != null &&
                x.MaCode.Trim().ToLower() == code.ToLower());

            if (km == null)
            {
                return new KhuyenMaiApDungInfo
                {
                    ThanhCong = false,
                    ThongBao = "Mã khuyến mãi không tồn tại."
                };
            }

            var rawStatus = (km.TrangThai ?? string.Empty).Trim().ToLowerInvariant();
            if (rawStatus is "tạm dừng" or "tamdung")
            {
                return new KhuyenMaiApDungInfo
                {
                    ThanhCong = false,
                    ThongBao = "Khuyến mãi hiện đang tạm dừng."
                };
            }

            var now = DateTime.Now;
            if (km.NgayBatDau.HasValue && km.NgayBatDau.Value > now)
            {
                return new KhuyenMaiApDungInfo
                {
                    ThanhCong = false,
                    ThongBao = "Khuyến mãi chưa bắt đầu."
                };
            }

            if (km.NgayKetThuc.HasValue && km.NgayKetThuc.Value < now)
            {
                return new KhuyenMaiApDungInfo
                {
                    ThanhCong = false,
                    ThongBao = "Khuyến mãi đã hết hạn."
                };
            }

            var soLuong = km.SoLuong ?? 0;
            if (soLuong <= 0 || (km.DaDung ?? 0) >= soLuong)
            {
                return new KhuyenMaiApDungInfo
                {
                    ThanhCong = false,
                    ThongBao = "Khuyến mãi đã hết lượt sử dụng."
                };
            }

            var donToiThieu = km.DonToiThieu ?? 0;
            if (tamTinh < donToiThieu)
            {
                return new KhuyenMaiApDungInfo
                {
                    ThanhCong = false,
                    ThongBao = $"Hóa đơn chưa đạt giá trị tối thiểu {donToiThieu:N0} đ."
                };
            }

            var giaTri = km.GiaTriGiam ?? 0;
            decimal soTienGiam = 0;
            switch ((km.LoaiGiam ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "tienmat":
                case "giảm tiền":
                    soTienGiam = giaTri;
                    break;
                case "phantram":
                case "giảm phần trăm":
                    soTienGiam = Math.Round(tamTinh * giaTri / 100m, 0);
                    break;
            }

            if (soTienGiam <= 0)
            {
                return new KhuyenMaiApDungInfo
                {
                    ThanhCong = false,
                    ThongBao = "Khuyến mãi không hợp lệ."
                };
            }

            if (soTienGiam > tamTinh)
            {
                soTienGiam = tamTinh;
            }

            return new KhuyenMaiApDungInfo
            {
                ThanhCong = true,
                ThongBao = "Áp dụng khuyến mãi thành công.",
                MaKhuyenMai = km.MaKhuyenMai,
                MaCode = km.MaCode?.Trim() ?? code,
                TenKhuyenMai = km.TenKhuyenMai?.Trim() ?? string.Empty,
                SoTienGiam = soTienGiam
            };
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

        public int GetSoLuongTonDichVu(int maSanPham)
        {
            using var db = new RapPhim2Context();
            var sanPham = db.SanPhams
                .Include(s => s.Kho)
                .Include(s => s.MaLoaiSpNavigation)
                .FirstOrDefault(s => s.MaSanPham == maSanPham);

            if (sanPham == null)
                return 0;

            if (string.Equals(sanPham.MaLoaiSpNavigation?.TenLoai, "Combo", StringComparison.OrdinalIgnoreCase))
                return TinhSoLuongComboKhaDung(db, maSanPham);

            return sanPham.Kho?.SoLuongTon ?? 0;
        }

        private static KhachHang TimHoacTaoKhachHang(RapPhim2Context db, string hoTen, string soDienThoai)
        {
            var sdt = (soDienThoai ?? string.Empty).Trim();
            var ten = (hoTen ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(sdt))
            {
                var khachLe = db.KhachHangs
                    .Where(k => k.SoDienThoai == null && k.HoTen == "Khách lẻ")
                    .OrderBy(k => k.MaKhachHang)
                    .FirstOrDefault();

                if (khachLe != null)
                    return khachLe;

                khachLe = new KhachHang
                {
                    HoTen = "Khách lẻ",
                    SoDienThoai = null,
                    NgayTao = DateTime.Now
                };
                db.KhachHangs.Add(khachLe);
                db.SaveChanges();
                return khachLe;
            }

            var kh = db.KhachHangs.FirstOrDefault(k => k.SoDienThoai == sdt);
            if (kh != null)
            {
                if (!string.IsNullOrWhiteSpace(ten))
                    kh.HoTen = ten;

                return kh;
            }

            kh = new KhachHang
            {
                HoTen = string.IsNullOrWhiteSpace(ten) ? "Khách lẻ" : ten,
                SoDienThoai = sdt,
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
                var sanPham = db.SanPhams
                    .Include(s => s.Kho)
                    .Include(s => s.MaLoaiSpNavigation)
                    .FirstOrDefault(s => s.MaSanPham == dv.MaSanPham);
                if (sanPham == null)
                    throw new InvalidOperationException("Dich vu khong con ton tai.");

                if (string.Equals(sanPham.MaLoaiSpNavigation?.TenLoai, "Combo", StringComparison.OrdinalIgnoreCase))
                {
                    TruKhoThanhPhanCombo(db, sanPham, dv.SoLuong);
                }
                else if (sanPham.Kho != null)
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

        private static int TinhSoLuongComboKhaDung(RapPhim2Context db, int maCombo)
        {
            var thanhPhans = db.ComboChiTiets
                .Where(x => x.MaCombo == maCombo && (x.SoLuong ?? 0) > 0)
                .ToList();

            if (!thanhPhans.Any())
                return 0;

            return thanhPhans
                .Select(x =>
                {
                    var ton = db.Khos
                        .Where(k => k.MaSanPham == x.MaSanPhamCon)
                        .Select(k => k.SoLuongTon ?? 0)
                        .FirstOrDefault();

                    return ton / (x.SoLuong ?? 1);
                })
                .Min();
        }

        private static void TruKhoThanhPhanCombo(RapPhim2Context db, SanPham combo, int soLuongCombo)
        {
            var thanhPhans = db.ComboChiTiets
                .Where(x => x.MaCombo == combo.MaSanPham && (x.SoLuong ?? 0) > 0)
                .ToList();

            if (!thanhPhans.Any())
                throw new InvalidOperationException($"Combo {combo.Ten} chua co thanh phan.");

            foreach (var thanhPhan in thanhPhans)
            {
                var kho = db.Khos.FirstOrDefault(x => x.MaSanPham == thanhPhan.MaSanPhamCon);
                var soLuongCan = soLuongCombo * (thanhPhan.SoLuong ?? 0);
                var soLuongTon = kho?.SoLuongTon ?? 0;

                if (soLuongTon < soLuongCan)
                    throw new InvalidOperationException($"Combo {combo.Ten} khong du hang do thieu san pham thanh phan.");
            }

            foreach (var thanhPhan in thanhPhans)
            {
                var kho = db.Khos.First(x => x.MaSanPham == thanhPhan.MaSanPhamCon);
                kho.SoLuongTon = (kho.SoLuongTon ?? 0) - soLuongCombo * (thanhPhan.SoLuong ?? 0);
            }
        }

        private static void XoaGiuGheHetHan(RapPhim2Context db)
        {
            var now = DateTime.Now;
            var expired = db.GiuGheTams.Where(g => (g.TrangThai == "Đang giữ" || g.TrangThai == "DangGiu") && g.HetHanLuc <= now);
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
