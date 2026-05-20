using Cinema.Models;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class NhanVienBUS
    {
         NhanVienDAL dal=new NhanVienDAL();
        public List<NhanVien> getNhanVien()
        {
            return dal.getNhanVien();
        }
        public bool xoaNhanVien(int id)
        {
            return dal.removeStaff(id);
        }
        public bool suaNhanVien(NhanVien nv)
        {
            return dal.updateStaff(nv);
        }
        public List<TaiKhoan> GetTaiKhoanByNhanVien(int id)
        {
            return dal.getTaiKhoanByNhanVien(id);
        }
        public bool themNhanVien(NhanVien nv)
        {
            return dal.addStaff(nv);
        }
    }
}
