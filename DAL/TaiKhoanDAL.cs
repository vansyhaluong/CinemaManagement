using Cinema.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class TaiKhoanDAL
    {
        RapPhim2Context db = new();
        public TaiKhoan? DangNhap(string tenDangNhap, string matKhau)
        {
            return db.TaiKhoans
            .Include(x => x.NhanVien)
            .ThenInclude(nv => nv.MaRapNavigation)
            .FirstOrDefault(x =>
            x.TenDangNhap == tenDangNhap &&
            x.MatKhau == matKhau);


        }
    }
}
