using Cinema.Models;
using DAL;

namespace BUS
{
    public class KhuyenMaiBUS
    {
        private readonly KhuyenMaiDAL dal = new KhuyenMaiDAL();

        public List<KhuyenMai> GetKhuyenMais()
        {
            return dal.GetKhuyenMais();
        }

        public KhuyenMai? GetKhuyenMaiById(int maKhuyenMai)
        {
            return dal.GetKhuyenMaiById(maKhuyenMai);
        }

        public void ThemKhuyenMai(KhuyenMai km)
        {
            dal.ThemKhuyenMai(km);
        }

        public void CapNhatKhuyenMai(KhuyenMai km)
        {
            dal.CapNhatKhuyenMai(km);
        }

        public void CapNhatTrangThai(int maKhuyenMai, string trangThai)
        {
            dal.CapNhatTrangThai(maKhuyenMai, trangThai);
        }
    }
}
