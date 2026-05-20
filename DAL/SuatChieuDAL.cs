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
        public List<SuatChieu> GetAllSuatChieu()
        {
            return db.SuatChieus.Include(x => x.MaPhimNavigation).Include(x => x.MaPhongNavigation).ToList();
        }
        public List<SuatChieu> getSuatChieuByPhim(int maPhim)
        {
            return db.SuatChieus.Include(x => x.MaPhimNavigation).Include(x => x.MaPhongNavigation).Where(x => x.MaPhim == maPhim).OrderBy(x=>x.ThoiGianBatDau).ToList();
        }
        public SuatChieu? getSuatChieuById(int id)
        {
            return db.SuatChieus.Include(x => x.MaPhimNavigation).Include(x => x.MaPhongNavigation).FirstOrDefault(x => x.MaSuatChieu == id);
        }
        public List<SuatChieu> getSuatChieuByNgay(DateTime day)
        {
            return db.SuatChieus.Include(x => x.MaPhimNavigation).Include(x => x.MaPhongNavigation).Where(x => x.ThoiGianBatDau.Value.Date == day.Date).ToList();
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
                    existingSuat.MaPhim = suat.MaPhim;
                    existingSuat.MaPhong = suat.MaPhong;
                    existingSuat.ThoiGianBatDau = suat.ThoiGianBatDau;
                    existingSuat.ThoiGianKetThuc = suat.ThoiGianKetThuc;
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
