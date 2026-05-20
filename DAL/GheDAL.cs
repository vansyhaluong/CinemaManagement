using Cinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class GheDAL
    {
        RapPhim2Context db = new RapPhim2Context();
        public List<Ghe> getAllGhe()
        {
            return db.Ghes.ToList();
        }
        public List<Ghe> getGheByPhongId(int phongId)
        {
            return db.Ghes.Where(g => g.MaPhong == phongId).ToList();
        }
        public int tongGhe(int ma)
        {
            return db.Ghes.Count(x => x.MaPhong == ma);
        }
        public int soGheTrong(int ma)
        {
            return db.Ghes.Count(x => x.MaPhong == ma && x.TrangThai == "Trống");
        }
        public int soGheDaDat(int ma)
        {
            return db.Ghes.Count(x => x.MaPhong == ma && x.TrangThai == "Đã đặt");

        }
        public int soGheBaoTri(int ma)
        {
            return db.Ghes.Count(x => x.MaPhong == ma && x.TrangThai == "Bảo trì");
        }
        public List<Ghe> getGheBySuatChieu(int maSuatChieu)
        {
            var suat = db.SuatChieus.FirstOrDefault(s => s.MaSuatChieu == maSuatChieu);

            if (suat == null)
                return new List<Ghe>();

            return db.Ghes
                .Where(x => x.MaPhong == suat.MaPhong)
                .ToList();
        }
    }
}
