using DAL;
using DAL.Models;

namespace BUS
{
    public class BanVeBUS
    {
        private readonly BanVeDAL dal = new BanVeDAL();

        public List<GheBanVeInfo> GetGheTheoSuatChieu(int maSuatChieu)
        {
            return dal.GetGheTheoSuatChieu(maSuatChieu);
        }

        public bool GiuGhe(int maSuatChieu, int maGhe)
        {
            return dal.GiuGhe(maSuatChieu, maGhe);
        }

        public void BoGiuGhe(int maSuatChieu, int maGhe)
        {
            dal.BoGiuGhe(maSuatChieu, maGhe);
        }

        public KetQuaBanVe ThanhToan(
            int maSuatChieu,
            IEnumerable<int> maGhes,
            IEnumerable<DichVuTam> dichVus,
            string hoTen,
            string soDienThoai,
            string phuongThuc)
        {
            return dal.ThanhToan(maSuatChieu, maGhes, dichVus, hoTen, soDienThoai, phuongThuc);
        }

        public List<TicketInfo> GetVeTheoDonHang(int maDonHang)
        {
            return dal.GetVeTheoDonHang(maDonHang);
        }
    }
}
