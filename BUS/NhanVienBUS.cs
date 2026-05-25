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
        public bool ThemNhanVienKemTaiKhoan(
            string hoTen,
            string soDienThoai,
            int maRap,
            string tenDangNhap,
            string matKhau)
        {
            if (string.IsNullOrWhiteSpace(hoTen)) return false;
            if (string.IsNullOrWhiteSpace(soDienThoai)) return false;
            if (maRap <= 0) return false;
            if (string.IsNullOrWhiteSpace(tenDangNhap)) return false;
            if (string.IsNullOrWhiteSpace(matKhau)) return false;

            return dal.ThemNhanVienKemTaiKhoan(
                hoTen,
                soDienThoai,
                maRap,
                tenDangNhap,
                matKhau
            );
        }
    }
}
