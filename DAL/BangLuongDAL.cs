using Cinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class BangLuongDAL
    {
        RapPhim2Context db=new RapPhim2Context();
        public List<ChamCong> GetChamCongTheoThang(int thang, int nam)
        {
            return db.ChamCongs
                .Include(x => x.MaNhanVienNavigation)
                    .ThenInclude(nv => nv.MaRapNavigation)
                .Include(x => x.MaCaNavigation)
                .Where(x => x.Ngay != null
                         && x.Ngay.Value.Month == thang
                         && x.Ngay.Value.Year == nam)
                .ToList();
        }
        public List<ChamCong> GetChamCongTheoTuan(DateOnly tuNgay, DateOnly denNgay)
        {
            return db.ChamCongs
    .Include(x => x.MaNhanVienNavigation)
        .ThenInclude(nv => nv.MaRapNavigation)
    .Include(x => x.MaCaNavigation)
    .Where(x => x.Ngay != null
             && x.Ngay >= tuNgay
             && x.Ngay <= denNgay)
    .ToList();
        }
    }
}
