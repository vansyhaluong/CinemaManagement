using Cinema.Models;
using DAL;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class BangLuongBUS
    {
        private readonly BangLuongDAL dal = new BangLuongDAL();
        private readonly ChamCongDAL chamCongDal = new ChamCongDAL();
        private readonly KhenThuongDAL khenThuongDal = new KhenThuongDAL();
        private readonly KyLuatDAL kyLuatDal = new KyLuatDAL();

        public List<BangLuongDTO> TinhLuongTheoThang(int thang, int nam)
        {
            var tuNgay = new DateOnly(nam, thang, 1);
            var denNgay = tuNgay.AddMonths(1).AddDays(-1);
            var dsChamCong = chamCongDal.GetDanhSachChamCongTheoKhoang(tuNgay, denNgay);
            var dsThuong = khenThuongDal.GetByDateRange(tuNgay, denNgay);
            var dsKyLuat = kyLuatDal.GetByDateRange(tuNgay, denNgay);

            return dsChamCong
                .GroupBy(x => x.MaNhanVien)
                .Select(g =>
                {
                    var itemDau = g.First();

                    int ngayCong = g.Count(x => x.TrangThai == "Đúng giờ" || x.TrangThai == "Trễ");
                    int soLanTre = g.Count(x => x.TrangThai == "Trễ");
                    int soNgayNghi = g.Count(x =>
                        x.TrangThai == "Nghỉ" ||
                        x.TrangThai == "Vắng" ||
                        x.TrangThai == "Chưa chấm công");

                    double tongGio = g.Sum(x =>
                    {
                        if (x.GioVao == null || x.GioRa == null)
                            return 0;

                        return (x.GioRa.Value - x.GioVao.Value).TotalHours;
                    });

                    decimal luongCoBan = 25000;
                    decimal tongLuongGio = (decimal)tongGio * luongCoBan;
                    decimal thuong = dsThuong
                        .Where(x => x.MaNhanVien == g.Key)
                        .Sum(x => x.SoTienThuong);
                    decimal phat = dsKyLuat
                        .Where(x => x.MaNhanVien == g.Key)
                        .Sum(x => x.SoTienPhat);

                    return new BangLuongDTO
                    {
                        MaNhanVien = itemDau.MaNhanVien,
                        MaRap = itemDau.MaRap,
                        HoTen = itemDau.HoTen,
                        TenRap = itemDau.TenRap,
                        Thang = thang,
                        Nam = nam,
                        TongNgayCong = ngayCong,
                        TongGioLam = Math.Round(tongGio, 2),
                        SoLanTre = soLanTre,
                        SoNgayNghi = soNgayNghi,
                        LuongCoBan = luongCoBan,
                        Thuong = thuong,
                        Phat = phat,
                        TongLuong = tongLuongGio + thuong - phat
                    };
                })
                .ToList();
        }
        public List<BangLuongDTO> TinhLuongTheoTuan(DateOnly tuNgay, DateOnly denNgay)
        {
            var dsChamCong = chamCongDal.GetDanhSachChamCongTheoKhoang(tuNgay, denNgay);
            var dsThuong = khenThuongDal.GetByDateRange(tuNgay, denNgay);
            var dsKyLuat = kyLuatDal.GetByDateRange(tuNgay, denNgay);

            return dsChamCong
                .GroupBy(x => x.MaNhanVien)
                .Select(g =>
                {
                    var itemDau = g.First();

                    double tongGio = g.Sum(x =>
                    {
                        if (x.GioVao == null || x.GioRa == null)
                            return 0;

                        if (string.IsNullOrWhiteSpace(x.GioBatDau))
                            return 0;

                        DateTime gioBatDauCa = DateTime.Parse($"{x.Ngay:yyyy-MM-dd} {x.GioBatDau}");

                        DateTime gioBatDauTinhLuong = x.GioVao.Value > gioBatDauCa
                            ? x.GioVao.Value
                            : gioBatDauCa;

                        DateTime gioKetThucTinhLuong = x.GioRa.Value;

                        if (gioKetThucTinhLuong <= gioBatDauTinhLuong)
                            return 0;

                        return (gioKetThucTinhLuong - gioBatDauTinhLuong).TotalHours;
                    });

                    int ngayCong = g.Count(x =>
                        x.TrangThai == "Đúng giờ" ||
                        x.TrangThai == "Trễ");

                    int soLanTre = g.Count(x => x.TrangThai == "Trễ");

                    int soNgayNghi = g.Count(x =>
                        x.TrangThai == "Nghỉ" ||
                        x.TrangThai == "Vắng" ||
                        x.TrangThai == "Chưa chấm công");

                    decimal luongCoBan = 25000;
                    decimal tienLuongGio = (decimal)tongGio * luongCoBan;
                    decimal thuong = dsThuong
                        .Where(x => x.MaNhanVien == g.Key)
                        .Sum(x => x.SoTienThuong);
                    decimal phat = dsKyLuat
                        .Where(x => x.MaNhanVien == g.Key)
                        .Sum(x => x.SoTienPhat);

                    decimal tongLuong = tienLuongGio + thuong - phat;

                    return new BangLuongDTO
                    {
                        MaNhanVien = itemDau.MaNhanVien,
                        MaRap = itemDau.MaRap,
                        HoTen = itemDau.HoTen,
                        TenRap = itemDau.TenRap,

                        TuNgay = tuNgay,
                        DenNgay = denNgay,
                        Thang = tuNgay.Month,
                        Nam = tuNgay.Year,

                        TongNgayCong = ngayCong,
                        TongGioLam = Math.Round(tongGio, 2),
                        SoLanTre = soLanTre,
                        SoNgayNghi = soNgayNghi,

                        LuongCoBan = luongCoBan,
                        Thuong = thuong,
                        Phat = phat,
                        TongLuong = tongLuong
                    };
                })
                .ToList();
        }
    }
}
