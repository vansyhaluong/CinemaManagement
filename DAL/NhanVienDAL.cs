using Cinema.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{

    public class NhanVienDAL
    {
        RapPhim2Context db = new RapPhim2Context();
        public List<NhanVien> getNhanVien()
        {
            return db.NhanViens.Include(x=>x.MaRapNavigation).ToList();
        }
        public bool removeStaff(int id)
        {
            try
            {
                var item = db.NhanViens.FirstOrDefault(x => x.MaNhanVien == id);
                if (item == null) return false;
                db.NhanViens.Remove(item);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
            
        }
        public bool addStaff(NhanVien nv)
        {
            try
            {
                db.NhanViens.Add(nv);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool updateStaff(NhanVien nv)
        {
            try
            {
                var item = db.NhanViens.FirstOrDefault(x => x.MaNhanVien == nv.MaNhanVien);
                if (item == null) return false;
                item.HoTen = nv.HoTen;
                item.SoDienThoai = nv.SoDienThoai;
                item.MaRap = nv.MaRap;
                item.MaTaiKhoan = nv.MaTaiKhoan;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi update nhân viên: " + ex.Message);
            }
        }
        public NhanVien? getNhanVienById(int id)
        {
            var item= db.NhanViens.FirstOrDefault(x=>x.MaNhanVien == id);
            if (item != null)
            {
                return item;
            }
            return null;
        }
        public List<TaiKhoan> getTaiKhoanByNhanVien(int id)
        {
            return db.TaiKhoans.Where(x=>x.MaTaiKhoan==id).ToList();
        }
    }
}
