using Cinema.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class SuatChieuDAL
    {
        RapPhim2Context db = new RapPhim2Context();
        public List<SuatChieu> GetAllSuatChieu(int? maRap = null)
        {
            var query = db.SuatChieus
                .Include(x => x.MaPhimNavigation)
                .Include(x => x.MaPhongNavigation)
                .AsQueryable();

            if (maRap.HasValue && maRap.Value > 0)
            {
                query = query.Where(x => x.MaPhongNavigation != null && x.MaPhongNavigation.MaRap == maRap.Value);
            }

            return query.ToList();
        }
        public List<SuatChieu> getSuatChieuByPhim(int maPhim, int? maRap = null)
        {
            var query = db.SuatChieus
                .Include(x => x.MaPhimNavigation)
                .Include(x => x.MaPhongNavigation)
                .Where(x => x.MaPhim == maPhim);

            if (maRap.HasValue && maRap.Value > 0)
            {
                query = query.Where(x => x.MaPhongNavigation != null && x.MaPhongNavigation.MaRap == maRap.Value);
            }

            return query.OrderBy(x => x.ThoiGianBatDau).ToList();
        }
        public SuatChieu? getSuatChieuById(int id, int? maRap = null)
        {
            var query = db.SuatChieus
                .Include(x => x.MaPhimNavigation)
                .Include(x => x.MaPhongNavigation)
                .Where(x => x.MaSuatChieu == id);

            if (maRap.HasValue && maRap.Value > 0)
            {
                query = query.Where(x => x.MaPhongNavigation != null && x.MaPhongNavigation.MaRap == maRap.Value);
            }

            return query.FirstOrDefault();
        }
        public List<SuatChieu> getSuatChieuByNgay(DateTime day, int? maRap = null)
        {
            var query = db.SuatChieus
                .Include(x => x.MaPhimNavigation)
                .Include(x => x.MaPhongNavigation)
                .Where(x => x.ThoiGianBatDau.Value.Date == day.Date);

            if (maRap.HasValue && maRap.Value > 0)
            {
                query = query.Where(x => x.MaPhongNavigation != null && x.MaPhongNavigation.MaRap == maRap.Value);
            }

            return query.ToList();
        }
        public List<SuatChieu> getSuatChieuByPhong(int maPhong, int? maRap = null)
        {
            var query = db.SuatChieus
                .Include(x => x.MaPhimNavigation)
                .Include(x => x.MaPhongNavigation)
                .Where(x => x.MaPhong == maPhong);

            if (maRap.HasValue && maRap.Value > 0)
            {
                query = query.Where(x => x.MaPhongNavigation != null && x.MaPhongNavigation.MaRap == maRap.Value);
            }

            return query.OrderBy(x => x.ThoiGianBatDau).ToList();
        }
        public bool KiemTraTrungGio(int maPhong, DateTime start, DateTime end)
        {
            using (var db = new RapPhim2Context())
            {
                return db.SuatChieus.Any(x =>
                    x.MaPhong == maPhong &&
                    x.ThoiGianBatDau < end &&
                    x.ThoiGianKetThuc > start
                );
            }
        }
        public bool addSuatChieu(SuatChieu suat)
        {
            try
            {
                db.SuatChieus.Add(suat);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool removeSuatChieu(int id)
        {
            try
            {
                var suat = db.SuatChieus.FirstOrDefault(s => s.MaSuatChieu == id);
                if (suat != null)
                {
                    db.SuatChieus.Remove(suat);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        public bool updateSuatChieu(SuatChieu suat)
        {
            try
            {
                var existingSuat = db.SuatChieus.FirstOrDefault(s => s.MaSuatChieu == suat.MaSuatChieu);
                if (existingSuat != null)
                {
                    bool biTrungGio = db.SuatChieus.Any(s =>
                        s.MaSuatChieu != suat.MaSuatChieu &&
                        s.MaPhong == suat.MaPhong &&
                        s.ThoiGianBatDau < suat.ThoiGianKetThuc &&
                        s.ThoiGianKetThuc > suat.ThoiGianBatDau);

                    if (biTrungGio)
                    {
                        return false;
                    }

                    existingSuat.MaPhim = suat.MaPhim;
                    existingSuat.MaPhong = suat.MaPhong;
                    existingSuat.ThoiGianBatDau = suat.ThoiGianBatDau;
                    existingSuat.ThoiGianKetThuc = suat.ThoiGianKetThuc;
                    existingSuat.GiaVeCoBan = suat.GiaVeCoBan;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
