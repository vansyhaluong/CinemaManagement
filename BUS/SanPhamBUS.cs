using Cinema.Models;
using DAL;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class SanPhamBUS
    {
        SanPhamDAL dal = new SanPhamDAL();

        public List<SanPhamKhoDTO> GetDanhSachKho()
        {
            return dal.GetDanhSachKho();
        }
        
        public bool NhapKhoSanPham(int maSanPham, int soLuong, decimal donGia, int maNhanVien)
        {
            if (soLuong <= 0)
                return false;

            if (donGia < 0)
                return false;

            return dal.NhapKhoSanPham(maSanPham, soLuong, donGia, maNhanVien);
        }
        public bool SuaSanPham(SanPhamKhoDTO sp)
        {
            if (sp == null)
                return false;

            if (string.IsNullOrWhiteSpace(sp.Ten))
                return false;

            if (sp.Gia < 0)
                return false;

            return dal.SuaSanPham(sp);
        }

        public bool XoaSanPham(int maSanPham)
        {
            if (maSanPham <= 0)
                return false;

            return dal.XoaSanPham(maSanPham);
        }
        public bool ThemSanPham(SanPham sp)
        {
            if (sp == null) return false;
            if (string.IsNullOrWhiteSpace(sp.Ten)) return false;
            if (sp.Gia < 0) return false;
            if (sp.MaLoaiSp == null || sp.MaLoaiSp <= 0) return false;

            return dal.ThemSanPham(sp);
        }
        public List<LoaiSanPham> GetLoaiSanPham()
        {
            return dal.GetLoaiSanPham();
        }
        public int TaoPhieuNhapKho(
    int maNhanVien,
    List<ChiTietNhapKhoDTO> dsChiTiet)
        {
            if (maNhanVien <= 0)
                return 0;

            if (dsChiTiet == null || dsChiTiet.Count == 0)
                return 0;

            return dal.TaoPhieuNhapKho(maNhanVien, dsChiTiet);
        }
        public List<PhieuNhapReportDTO> GetDuLieuPhieuNhapReport(int maPhieuNhap)
        {
            if (maPhieuNhap <= 0)
                return new List<PhieuNhapReportDTO>();

            return dal.GetDuLieuPhieuNhapReport(maPhieuNhap);
        }

    }
}
