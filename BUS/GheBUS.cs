using Cinema.Models;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class GheBUS
    {
        GheDAL dal = new GheDAL();
        public List<Ghe> geAllGhe()
        {
            return dal.getAllGhe();
        }
        public List<Ghe> getGheByPhongId(int phongId)
        {
            return dal.getGheByPhongId(phongId);
        }
        public int tongGhe(int ma)
        {
            return dal.tongGhe(ma);
        }
        public int soGheTrong(int ma)
        {
            return dal.soGheTrong(ma);
        }
        public int soGheDaDat(int ma)
        {
            return dal.soGheDaDat(ma);
        }
        public int soGheBaoTri(int ma)
        {
            return dal.soGheBaoTri(ma);
        }
        public List<Ghe> getGheBySuatChieu(int maSuatChieu)
        {
            return dal.getGheBySuatChieu(maSuatChieu);
        }
        public List<int> getMaGheDaBanTheoSuat(int maSuatChieu)
        {
            return dal.getMaGheDaBanTheoSuat(maSuatChieu);
        }
    }
}
