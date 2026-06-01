using DAL;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class BaoCaoDoanhThuBUS
    {
        private BaoCaoDoanhThuDAL baoCaoDAL = new BaoCaoDoanhThuDAL();

        public List<BaoCaoDoanhThuNgayDTO> GetBaoCaoTheoNgay(DateTime ngay, int maRap)
        {
            return baoCaoDAL.GetBaoCaoTheoNgay(ngay, maRap);
        }
        public void TaoBaoCaoTheoNgay(DateTime ngay)
        {
            baoCaoDAL.TaoBaoCaoTheoNgay(ngay);
        }
        public List<BaoCaoDoanhThuReportDTO> GetChiTietReport(DateTime ngay, int maRap)
        {
            return baoCaoDAL.GetChiTietReport(ngay, maRap);
        }
        public List<BaoCaoDoanhThuExcelDTO> GetChiTietExcel(DateTime ngay, int maRap)
        {
            return baoCaoDAL.GetChiTietExcel(ngay, maRap);
        }
        public int CountDonHangTheoNgay(DateTime ngay, int maRap)
        {
            return baoCaoDAL.CountDonHangTheoNgay(ngay, maRap);
        }

        public int CountDonHangDaThanhToan(DateTime ngay, int maRap)
        {
            return baoCaoDAL.CountDonHangDaThanhToan(ngay, maRap);
        }
    }
}
