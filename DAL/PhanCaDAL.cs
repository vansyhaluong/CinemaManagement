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
    public class PhanCaDAL
    {
        RapPhim2Context db=new RapPhim2Context();
        public List<PhanCaDTO> GetAll(DateTime ngay, int maRap = 0, int maCa = 0)
        {
            var query = db.PhanCas
                .Include(x => x.MaNhanVienNavigation)
                    .ThenInclude(nv => nv.MaRapNavigation)
                .Include(x => x.MaCaNavigation)
                .Where(x => x.Ngay == DateOnly.FromDateTime(ngay))
                .AsQueryable();

            if (maRap > 0)
                query = query.Where(x => x.MaNhanVienNavigation.MaRap == maRap);

            if (maCa > 0)
                query = query.Where(x => x.MaCa == maCa);

            return query.Select(x => new PhanCaDTO
            {
                MaPhanCa = x.MaPhanCa,
                MaNhanVien = x.MaNhanVien,
                HoTen = x.MaNhanVienNavigation.HoTen,
                MaRap = x.MaNhanVienNavigation.MaRap ?? 0,
                TenRap = x.MaNhanVienNavigation.MaRapNavigation.TenRap,
                MaCa = x.MaCa,
                TenCa = x.MaCaNavigation.TenCa,
                Ngay = x.Ngay.ToDateTime(TimeOnly.MinValue),
                GioBatDau = x.MaCaNavigation.GioBatDau.HasValue
    ? x.MaCaNavigation.GioBatDau.Value.ToString("HH:mm")
    : "",

                GioKetThuc = x.MaCaNavigation.GioKetThuc.HasValue
    ? x.MaCaNavigation.GioKetThuc.Value.ToString("HH:mm")
    : ""
            }).ToList();
        }
        public bool Add(PhanCa phanCa)
        {
            db.PhanCas.Add(phanCa);
            return db.SaveChanges() > 0;
        }

        public bool Update(PhanCa phanCa)
        {
            var pc = db.PhanCas.Find(phanCa.MaPhanCa);
            if (pc == null) return false;

            pc.MaNhanVien = phanCa.MaNhanVien;
            pc.MaCa = phanCa.MaCa;
            pc.Ngay = phanCa.Ngay;

            return db.SaveChanges() > 0;
        }

        public bool Delete(int maPhanCa)
        {
            var pc = db.PhanCas.Find(maPhanCa);
            if (pc == null) return false;

            db.PhanCas.Remove(pc);
            return db.SaveChanges() > 0;
        }

        public bool Exists(int maNhanVien, DateTime ngay, int? excludeMaPhanCa = null)
        {
            var date = DateOnly.FromDateTime(ngay);

            return db.PhanCas.Any(x =>
                x.MaNhanVien == maNhanVien &&
                x.Ngay == date &&
                (!excludeMaPhanCa.HasValue || x.MaPhanCa != excludeMaPhanCa.Value));
        }
    }
}
