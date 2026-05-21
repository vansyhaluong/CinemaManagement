using Cinema.Models;
using DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class KyLuatDAL
    {
        RapPhim2Context db=new RapPhim2Context();

        public List<KyLuatDTO> GetAll()
        {
            return db.KyLuats
                .Include(x => x.MaNhanVienNavigation)
                .Select(x => new KyLuatDTO
                {
                    MaKyLuat = x.MaKyLuat,
                    MaNhanVien = x.MaNhanVien ?? 0,
                    HoTen = x.MaNhanVienNavigation.HoTen,
                    Ngay = x.Ngay.HasValue
                        ? x.Ngay.Value.ToDateTime(TimeOnly.MinValue)
                        : DateTime.Now,
                    LyDo = x.LyDo,
                    SoTienPhat = x.SoTienPhat ?? 0
                })
                .ToList();
        }

        public bool Add(KyLuat kl)
        {
            db.KyLuats.Add(kl);
            db.SaveChanges();
            return true;
        }
    }
}
