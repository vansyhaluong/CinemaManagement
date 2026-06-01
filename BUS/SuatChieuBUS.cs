using Cinema.Models;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class SuatChieuBUS
    {
         SuatChieuDAL dal=new SuatChieuDAL();
        public List<SuatChieu> GetAllSuatChieu(int? maRap = null)
        {
            return dal.GetAllSuatChieu(maRap);
        }
        public List<SuatChieu> getSuatChieuByPhim(int maPhim, int? maRap = null)
        {
            return dal.getSuatChieuByPhim(maPhim, maRap);
        }
        public List<SuatChieu> getSuatChieuByNgay(DateTime day, int? maRap = null)
        {
            return dal.getSuatChieuByNgay(day, maRap);
        }
        public List<SuatChieu> getSuatChieuByPhong(int maPhong, int? maRap = null)
        {
            return dal.getSuatChieuByPhong(maPhong, maRap);
        }
        public bool KiemTraTrungGio(int maPhong, DateTime start, DateTime end)
        {
            return dal.KiemTraTrungGio(maPhong, start, end);
        }
        public bool KiemTraTrungGio(int maPhong, DateTime start, DateTime end, int? excludeMaSuatChieu)
        {
            return dal.KiemTraTrungGio(maPhong, start, end, excludeMaSuatChieu);
        }
        public SuatChieu? getSuatChieuById(int id, int? maRap = null)
        {
            return dal.getSuatChieuById(id, maRap);
        }
        public bool addSuatChieu(SuatChieu suat)
        {
            if (!suat.MaPhong.HasValue ||
            !suat.MaPhim.HasValue ||
            !suat.ThoiGianBatDau.HasValue ||
            !suat.ThoiGianKetThuc.HasValue)
            {
                return false;
            }

            if (suat.ThoiGianKetThuc.Value <= suat.ThoiGianBatDau.Value)
            {
                return false;
            }

            if (KiemTraTrungGio(
                suat.MaPhong.Value,
                suat.ThoiGianBatDau.Value,
                suat.ThoiGianKetThuc.Value))
            {
                return false;
            }

            return dal.addSuatChieu(suat);
        }
        public bool removeSuatChieu(int id)
        {
            return dal.removeSuatChieu(id);
        }
        public bool updateSuatChieu(SuatChieu suat)
        {
            if (!suat.MaPhong.HasValue ||
                !suat.MaPhim.HasValue ||
                !suat.ThoiGianBatDau.HasValue ||
                !suat.ThoiGianKetThuc.HasValue)
            {
                return false;
            }

            if (suat.ThoiGianKetThuc.Value <= suat.ThoiGianBatDau.Value)
            {
                return false;
            }

            if (KiemTraTrungGio(
                suat.MaPhong.Value,
                suat.ThoiGianBatDau.Value,
                suat.ThoiGianKetThuc.Value,
                suat.MaSuatChieu))
            {
                return false;
            }

            return dal.updateSuatChieu(suat);
        }
    }
}
