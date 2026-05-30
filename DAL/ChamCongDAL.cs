using Cinema.Models;
using DAL.Models;
using DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ChamCongDAL
    {
        RapPhim2Context db = new RapPhim2Context();
        public List<ChamCongDTO> GetDanhSachChamCong(DateTime ngay, int maRap = 0, int maCa = 0)
        {
            var date = DateOnly.FromDateTime(ngay);

            var query = db.PhanCas
                .Include(x => x.MaNhanVienNavigation)
                    .ThenInclude(nv => nv.MaRapNavigation)
                .Include(x => x.MaCaNavigation)
                .Where(x => x.Ngay == date)
                .AsQueryable();

            if (maRap > 0)
                query = query.Where(x => x.MaNhanVienNavigation.MaRap == maRap);

            if (maCa > 0)
                query = query.Where(x => x.MaCa == maCa);

            return BuildChamCongDtos(query.ToList());
        }
        public ChamCong? GetChamCongTrongNgay(int maNhanVien, int maCa, DateTime ngay)
        {
            var date = DateOnly.FromDateTime(ngay);

            return db.ChamCongs.FirstOrDefault(x =>
                x.MaNhanVien == maNhanVien &&
                x.MaCa == maCa &&
                x.Ngay == date);
        }

        public bool CheckIn(ChamCong chamCong)
        {
            db.ChamCongs.Add(chamCong);
            return db.SaveChanges() > 0;
        }

        //public bool CheckOut(int maChamCong, DateTime gioRa, string trangThai)
        //{
        //    var cc = db.ChamCongs.Find(maChamCong);

        //    if (cc == null)
        //        return false;

        //    cc.GioRa = gioRa;
        //    cc.TrangThai = trangThai;

        //    return db.SaveChanges() > 0;
        //}
        public bool CheckOut(int maChamCong)
        {
            var cc = db.ChamCongs
                .Include(x => x.MaCaNavigation)
                .FirstOrDefault(x => x.MaChamCong == maChamCong);

            if (cc == null)
                return false;

            if (cc.GioRa != null)
                throw new Exception("Nhân viên này đã check-out rồi.");

            if (cc.Ngay == null)
                throw new Exception("Không tìm thấy ngày chấm công.");

            if (cc.MaCaNavigation == null || cc.MaCaNavigation.GioKetThuc == null)
                throw new Exception("Không tìm thấy giờ kết thúc ca.");

            cc.GioRa = cc.Ngay.Value.ToDateTime(cc.MaCaNavigation.GioKetThuc.Value);

            return db.SaveChanges() > 0;
        }
        public List<ChamCong> GetChamCongTheoTuan(
    DateOnly tuNgay,
    DateOnly denNgay)
        {
            return db.ChamCongs
                .Include(x => x.MaNhanVienNavigation)
                .Where(x => x.Ngay >= tuNgay &&
                            x.Ngay <= denNgay)
                .ToList();
        }

        public List<ChamCongDTO> GetDanhSachChamCongTheoKhoang(DateOnly tuNgay, DateOnly denNgay)
        {
            var data = db.PhanCas
                .Include(x => x.MaNhanVienNavigation)
                    .ThenInclude(nv => nv.MaRapNavigation)
                .Include(x => x.MaCaNavigation)
                .Where(x => x.Ngay >= tuNgay && x.Ngay <= denNgay)
                .OrderBy(x => x.Ngay)
                .ThenBy(x => x.MaNhanVienNavigation.HoTen)
                .ToList();

            return BuildChamCongDtos(data);
        }

        private List<ChamCongDTO> BuildChamCongDtos(List<PhanCa> phanCas)
        {
            if (phanCas.Count == 0)
            {
                return new List<ChamCongDTO>();
            }

            var ngayBatDau = phanCas.Min(x => x.Ngay);
            var ngayKetThuc = phanCas.Max(x => x.Ngay);

            var chamCongMap = db.ChamCongs
                .Where(x => x.Ngay >= ngayBatDau && x.Ngay <= ngayKetThuc)
                .ToList()
                .ToDictionary(
                    x => $"{x.MaNhanVien}_{x.MaCa}_{x.Ngay}",
                    x => x);

            return phanCas.Select(pc =>
            {
                chamCongMap.TryGetValue($"{pc.MaNhanVien}_{pc.MaCa}_{pc.Ngay}", out var cc);

                return new ChamCongDTO
                {
                    MaPhanCa = pc.MaPhanCa,
                    MaNhanVien = pc.MaNhanVien,
                    HoTen = pc.MaNhanVienNavigation.HoTen,
                    MaRap = pc.MaNhanVienNavigation.MaRap ?? 0,
                    TenRap = pc.MaNhanVienNavigation.MaRapNavigation.TenRap,
                    MaCa = pc.MaCa,
                    TenCa = pc.MaCaNavigation.TenCa,
                    Ngay = pc.Ngay.ToDateTime(TimeOnly.MinValue),
                    GioBatDau = pc.MaCaNavigation.GioBatDau.HasValue
                        ? pc.MaCaNavigation.GioBatDau.Value.ToString("HH:mm")
                        : "",
                    GioKetThuc = pc.MaCaNavigation.GioKetThuc.HasValue
                        ? pc.MaCaNavigation.GioKetThuc.Value.ToString("HH:mm")
                        : "",
                    MaChamCong = cc?.MaChamCong,
                    GioVao = cc?.GioVao,
                    GioRa = cc?.GioRa,
                    TrangThai = cc?.TrangThai ?? "Chưa chấm công"
                };
            }).ToList();
        }

    }
}
