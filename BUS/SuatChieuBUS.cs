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
        public List<SuatChieu> GetAllSuatChieu()
        {
            return dal.GetAllSuatChieu();
        }
        public List<SuatChieu> getSuatChieuByPhim(int maPhim)
        {
            return dal.getSuatChieuByPhim(maPhim);
        }
        public List<SuatChieu> getSuatChieuByNgay(DateTime day)
        {
            return dal.getSuatChieuByNgay(day);
        }
        public bool KiemTraTrungGio(int maPhong, DateTime start, DateTime end)
        {
            return dal.KiemTraTrungGio(maPhong, start, end);
        }
        public SuatChieu? getSuatChieuById(int id)
        {
            return dal.getSuatChieuById(id);
        }
        public bool addSuatChieu(SuatChieu suat)
        {
            if (!suat.MaPhong.HasValue ||
            !suat.ThoiGianBatDau.HasValue ||
            !suat.ThoiGianKetThuc.HasValue)
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
            try
            {
                dal.addSuatChieu(suat);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool removeSuatChieu(int id)
        {
            try
            {
                dal.removeSuatChieu(id);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool updateSuatChieu(SuatChieu suat)
        {
            try
            {
                dal.updateSuatChieu(suat);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
