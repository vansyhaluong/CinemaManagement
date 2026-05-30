using DAL;
using DAL.Models;

namespace BUS
{
    public class HoaDonBUS
    {
        private readonly HoaDonDAL dal = new HoaDonDAL();

        public List<HoaDonRowInfo> GetHoaDons(string? maHoaDon, string? soDienThoai, DateTime? tuNgay, DateTime? denNgay, int? maRap = null)
        {
            return dal.GetHoaDons(maHoaDon, soDienThoai, tuNgay, denNgay, maRap);
        }

        public HoaDonDetailInfo? GetHoaDonDetail(int maDonHang, int? maRap = null)
        {
            return dal.GetHoaDonDetail(maDonHang, maRap);
        }

        public bool HuyHoaDon(int maDonHang, string lyDo, int maNhanVien = 1, int? maRap = null)
        {
            return dal.HuyHoaDon(maDonHang, lyDo, maNhanVien, maRap);
        }
    }
}
