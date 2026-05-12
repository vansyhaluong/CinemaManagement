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
        public List<PhongChieu> getPhongChieu()
        {
            return db.PhongChieus.Include(p => p.MaRapNavigation).ToList();
        }
        public PhongChieu? getPhongChieuById(int id)
        {
            return db.PhongChieus.Include(p=>p.MaRapNavigation).FirstOrDefault(p => p.MaPhong == id);
        }
        public List<PhongChieu> getPhongChieuByRap(int maRap)
        {
            return db.PhongChieus.Include(p => p.MaRapNavigation).Where(x=>x.MaRap==maRap).ToList();

        }
        public bool addPhongChieu(PhongChieu phong)
        {
            try
            {
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
                var existing = db.PhongChieus.FirstOrDefault(p => p.MaPhong == phong.MaPhong);
                if (existing != null)
                {
                    existing.TenPhong = phong.TenPhong;
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
