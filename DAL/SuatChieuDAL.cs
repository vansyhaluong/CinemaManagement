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
        public bool KiemTraTrungGio(int maPhong, DateTime start, DateTime end, int? excludeMaSuatChieu = null)
        {
            using (var db = new RapPhim2Context())
            {
                var query = db.SuatChieus.Where(x =>
                    x.MaPhong == maPhong &&
                    x.ThoiGianBatDau < end &&
                    x.ThoiGianKetThuc > start);

                if (excludeMaSuatChieu.HasValue)
                {
                    query = query.Where(x => x.MaSuatChieu != excludeMaSuatChieu.Value);
                }

                return query.Any();
            }
        }
        public bool addSuatChieu(SuatChieu suat)
        {
            try
            {
                if (!suat.MaPhong.HasValue ||
                    !suat.ThoiGianBatDau.HasValue ||
                    !suat.ThoiGianKetThuc.HasValue)
                {
                    return false;
                }

                if (KiemTraTrungGio(
                    suat.MaPhong.Value,
                    suat.ThoiGianBatDau.Value,
                    suat.ThoiGianKetThuc.Value))
                {
                    return false;
                }

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
                if (suat == null)
                    return false;

                bool daCoVeBan = db.VeBans.Any(v => v.MaSuatChieu == id);
                if (daCoVeBan)
                    return false;

                var dsGiuTam = db.GiuGheTams.Where(x => x.MaSuatChieu == id).ToList();
                if (dsGiuTam.Count > 0)
                {
                    db.GiuGheTams.RemoveRange(dsGiuTam);
                }

                db.SuatChieus.Remove(suat);
                db.SaveChanges();
                return true;
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
                    bool biTrungGio = suat.MaPhong.HasValue
                        && suat.ThoiGianBatDau.HasValue
                        && suat.ThoiGianKetThuc.HasValue
                        && KiemTraTrungGio(
                            suat.MaPhong.Value,
                            suat.ThoiGianBatDau.Value,
                            suat.ThoiGianKetThuc.Value,
                            suat.MaSuatChieu);

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
