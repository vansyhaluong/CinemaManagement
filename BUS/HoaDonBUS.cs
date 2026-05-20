using DAL;
using DAL.Models;

namespace BUS
{
    public class HoaDonBUS
    {
        private readonly HoaDonDAL dal = new HoaDonDAL();

        public List<HoaDonRowInfo> GetHoaDons(string? maHoaDon, string? soDienThoai, DateTime? tuNgay, DateTime? denNgay)
        {
            return dal.GetHoaDons(maHoaDon, soDienThoai, tuNgay, denNgay);
        }

        public HoaDonDetailInfo? GetHoaDonDetail(int maDonHang)
        {
            return dal.GetHoaDonDetail(maDonHang);
        }

        public bool HuyHoaDon(int maDonHang)
        {
            return dal.HuyHoaDon(maDonHang);
        }
    }
}
