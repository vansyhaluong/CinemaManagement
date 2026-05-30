using Cinema.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class PhongChieuDAL
    {
        RapPhim2Context db = new RapPhim2Context();
        public List<PhongChieu> getPhongChieu(int? maRap = null)
        {
            var query = db.PhongChieus
                .Include(p => p.MaRapNavigation)
                .AsQueryable();

            if (maRap.HasValue && maRap.Value > 0)
            {
                query = query.Where(p => p.MaRap == maRap.Value);
            }

            return query.ToList();
        }
        public PhongChieu? getPhongChieuById(int id, int? maRap = null)
        {
            var query = db.PhongChieus
                .Include(p => p.MaRapNavigation)
                .Where(p => p.MaPhong == id);

            if (maRap.HasValue && maRap.Value > 0)
            {
                query = query.Where(p => p.MaRap == maRap.Value);
            }

            return query.FirstOrDefault();
        }
        public List<PhongChieu> getPhongChieuByRap(int maRap)
        {
            return db.PhongChieus.Include(p => p.MaRapNavigation).Where(x=>x.MaRap==maRap).ToList();

        }
        public bool addPhongChieu(PhongChieu phong)
        {
            try
            {
                var tenPhong = (phong.TenPhong ?? string.Empty).Trim();
                phong.TenPhong = tenPhong;

                bool daTonTai = db.PhongChieus.Any(p =>
                    p.MaRap == phong.MaRap &&
                    p.TenPhong != null &&
                    p.TenPhong.Trim() == tenPhong);

                if (daTonTai)
                {
                    return false;
                }

                db.PhongChieus.Add(phong);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool removePhongChieu(int id)
        {
            try
            {
                var phong = db.PhongChieus.FirstOrDefault(p => p.MaPhong == id);
                if (phong != null)
                {
                    db.PhongChieus.Remove(phong);
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
        public bool updatePhongChieu(PhongChieu phong)
        {
            try
            {
                var tenPhong = (phong.TenPhong ?? string.Empty).Trim();
                var existing = db.PhongChieus.FirstOrDefault(p => p.MaPhong == phong.MaPhong);
                if (existing != null)
                {
                    bool daTonTai = db.PhongChieus.Any(p =>
                        p.MaPhong != phong.MaPhong &&
                        p.MaRap == phong.MaRap &&
                        p.TenPhong != null &&
                        p.TenPhong.Trim() == tenPhong);

                    if (daTonTai)
                    {
                        return false;
                    }

                    existing.TenPhong = tenPhong;
                    existing.MaRap = phong.MaRap;
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
