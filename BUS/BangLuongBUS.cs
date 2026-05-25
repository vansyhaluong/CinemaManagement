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
        private BangLuongDAL dal = new BangLuongDAL();

        public List<BangLuongDTO> TinhLuongTheoThang(int thang, int nam)
        {
            var dsChamCong = dal.GetChamCongTheoThang(thang, nam);

            var result = dsChamCong
                .Where(x => x.MaNhanVien != null)
                .GroupBy(x => x.MaNhanVien)
                .Select(g =>
                {
                    var nv = g.First().MaNhanVienNavigation;

                    int ngayCong = g.Count(x => x.TrangThai == "Đúng giờ" || x.TrangThai == "Trễ");
                    int soLanTre = g.Count(x => x.TrangThai == "Trễ");
                    int soNgayNghi = g.Count(x => x.TrangThai == "Nghỉ" || x.TrangThai == "Vắng");

                    double tongGio = g.Sum(x =>
                    {
                        if (x.GioVao == null || x.GioRa == null)
                            return 0;

                        return (x.GioRa.Value - x.GioVao.Value).TotalHours;
                    });

                    decimal luongCoBan = 25000;
                    decimal tongLuongGio = (decimal)tongGio * luongCoBan;

                    decimal phat = soLanTre * 20000 + soNgayNghi * 100000;
                    decimal thuong = ngayCong >= 26 ? 500000 : 0;

                    return new BangLuongDTO
                    {
                        MaNhanVien = nv.MaNhanVien,
                        HoTen = nv.HoTen,
                        TenRap = nv.MaRapNavigation?.TenRap ?? "",
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

            return result;
        }
        public List<BangLuongDTO> TinhLuongTheoTuan(DateOnly tuNgay, DateOnly denNgay)
        {
            var dsChamCong = dal.GetChamCongTheoTuan(tuNgay, denNgay);

            return dsChamCong
                .Where(x => x.MaNhanVien != null)
                .GroupBy(x => x.MaNhanVien)
                .Select(g =>
                {
                    var nv = g.First().MaNhanVienNavigation;

                    double tongGio = g.Sum(x =>
                    {
                        if (x.GioVao == null)
                            return 0;

                        if (x.Ngay == null || x.MaCaNavigation == null)
                            return 0;

                        if (x.MaCaNavigation.GioBatDau == null || x.MaCaNavigation.GioKetThuc == null)
                            return 0;

                        DateTime gioBatDauCa = x.Ngay.Value.ToDateTime(x.MaCaNavigation.GioBatDau.Value);
                        DateTime gioKetThucCa = x.Ngay.Value.ToDateTime(x.MaCaNavigation.GioKetThuc.Value);

                        DateTime gioBatDauTinhLuong = x.GioVao.Value > gioBatDauCa
                            ? x.GioVao.Value
                            : gioBatDauCa;

                        DateTime gioKetThucTinhLuong = x.GioRa ?? gioKetThucCa;

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
                        x.TrangThai == "Vắng");

                    decimal luongCoBan = 25000;
                    decimal tienLuongGio = (decimal)tongGio * luongCoBan;
                    decimal phat = soLanTre * 20000 + soNgayNghi * 100000;
                    decimal thuong = ngayCong >= 6 ? 200000 : 0;

                    decimal tongLuong = tienLuongGio + thuong - phat;


                    
                    return new BangLuongDTO
                    {
                        MaNhanVien = nv.MaNhanVien,
                        MaRap = nv.MaRap ?? 0,
                        HoTen = nv.HoTen ?? "",
                        TenRap = nv.MaRapNavigation?.TenRap ?? "",

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
