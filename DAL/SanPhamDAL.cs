using Cinema.Models;
using DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class SanPhamDAL
    {
        RapPhim2Context db=new RapPhim2Context();
        public List<SanPhamKhoDTO> GetDanhSachKho()
        {
            var ds = db.SanPhams
                .Include(x => x.MaLoaiSpNavigation)
                .Include(x => x.Kho)
                .Select(x => new SanPhamKhoDTO
                {
                    MaSanPham = x.MaSanPham,
                    Ten = x.Ten,
                    Gia = x.Gia,

                    TenLoai = x.MaLoaiSpNavigation.TenLoai,

                    SoLuongTon = x.Kho != null? x.Kho.SoLuongTon ?? 0: 0,

                    HinhAnh = x.HinhAnh
                })
                .ToList();

            return ds;
        }
        public bool SuaSanPham(SanPhamKhoDTO sp)
        {
            

            var sanPham = db.SanPhams.FirstOrDefault(x => x.MaSanPham == sp.MaSanPham);

            if (sanPham == null)
                return false;

            sanPham.Ten = sp.Ten;
            sanPham.Gia = sp.Gia;
            sanPham.HinhAnh = sp.HinhAnh;

            var kho = db.Khos.FirstOrDefault(x => x.MaSanPham == sp.MaSanPham);

            if (kho == null)
            {
                kho = new Kho
                {
                    MaSanPham = sp.MaSanPham,
                    SoLuongTon = sp.SoLuongTon
                };

                db.Khos.Add(kho);
            }
            else
            {
                kho.SoLuongTon = sp.SoLuongTon;
            }

            db.SaveChanges();

            return true;
        }

        public bool XoaSanPham(int maSanPham)
        {


            //var sanPham = db.SanPhams.FirstOrDefault(x => x.MaSanPham == maSanPham);

            //if (sanPham == null)
            //    return false;

            //sanPham.TrangThai = "Ngừng bán";

            //db.SaveChanges();
            //return true;
            var kho = db.Khos
        .FirstOrDefault(x => x.MaSanPham == maSanPham);

            if (kho != null)
            {
                db.Khos.Remove(kho);
            }

            var sanPham = db.SanPhams
                .FirstOrDefault(x => x.MaSanPham == maSanPham);

            if (sanPham == null)
                return false;

            db.SanPhams.Remove(sanPham);

            db.SaveChanges();

            return true;
        }
        public bool ThemSanPham(SanPham sp)
        {
            

            db.SanPhams.Add(sp);
            db.SaveChanges();

            var kho = new Kho
            {
                MaSanPham = sp.MaSanPham,
                SoLuongTon = 0
            };

            db.Khos.Add(kho);
            db.SaveChanges();

            return true;
        }
        public List<LoaiSanPham> GetLoaiSanPham()
        {
           
            return db.LoaiSanPhams.ToList();
        }
        public bool NhapKhoSanPham(int maSanPham, int soLuong, decimal donGia, int maNhanVien)
        {
            

            using var transaction = db.Database.BeginTransaction();

            try
            {
                var phieuNhap = new PhieuNhapKho
                {
                    NgayNhap = DateTime.Now,
                    MaNhanVien = maNhanVien
                };

                db.PhieuNhapKhos.Add(phieuNhap);
                db.SaveChanges();

                var chiTiet = new ChiTietPhieuNhapKho
                {
                    MaPhieuNhap = phieuNhap.MaPhieuNhap,
                    MaSanPham = maSanPham,
                    SoLuong = soLuong,
                    DonGia = donGia
                };

                db.ChiTietPhieuNhapKhos.Add(chiTiet);

                var kho = db.Khos.FirstOrDefault(x => x.MaSanPham == maSanPham);

                if (kho == null)
                {
                    kho = new Kho
                    {
                        MaSanPham = maSanPham,
                        SoLuongTon = soLuong
                    };

                    db.Khos.Add(kho);
                }
                else
                {
                    kho.SoLuongTon = (kho.SoLuongTon ?? 0) + soLuong;
                }

                db.SaveChanges();
                transaction.Commit();

                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }
        public int TaoPhieuNhapKho(int maNhanVien, List<ChiTietNhapKhoDTO> dsChiTiet)
        {
            

            using var tran = db.Database.BeginTransaction();

            try
            {
                var phieuNhap = new PhieuNhapKho
                {
                    NgayNhap = DateTime.Now,
                    MaNhanVien = maNhanVien
                };

                db.PhieuNhapKhos.Add(phieuNhap);
                db.SaveChanges();

                foreach (var item in dsChiTiet)
                {
                    var chiTiet = new ChiTietPhieuNhapKho
                    {
                        MaPhieuNhap = phieuNhap.MaPhieuNhap,
                        MaSanPham = item.MaSanPham,
                        SoLuong = item.SoLuong,
                        DonGia = item.Gia
                    };

                    db.ChiTietPhieuNhapKhos.Add(chiTiet);

                    var kho = db.Khos.FirstOrDefault(x => x.MaSanPham == item.MaSanPham);

                    if (kho == null)
                    {
                        kho = new Kho
                        {
                            MaSanPham = item.MaSanPham,
                            SoLuongTon = item.SoLuong
                        };

                        db.Khos.Add(kho);
                    }
                    else
                    {
                        kho.SoLuongTon = (kho.SoLuongTon ?? 0) + item.SoLuong;
                    }
                }

                db.SaveChanges();
                tran.Commit();

                return phieuNhap.MaPhieuNhap;
            }
            catch
            {
                tran.Rollback();
                return 0;
            }
        }
        public List<PhieuNhapReportDTO> GetDuLieuPhieuNhapReport(int maPhieuNhap)
        {
            

            var data = (
                from ct in db.ChiTietPhieuNhapKhos
                join pn in db.PhieuNhapKhos
                    on ct.MaPhieuNhap equals pn.MaPhieuNhap

                join sp in db.SanPhams
                    on ct.MaSanPham equals sp.MaSanPham

                join nv in db.NhanViens
                    on pn.MaNhanVien equals nv.MaNhanVien

                where pn.MaPhieuNhap == maPhieuNhap

                select new PhieuNhapReportDTO
                {
                    MaPhieuNhap = pn.MaPhieuNhap,

                    TenSanPham = sp.Ten,

                    SoLuong = ct.SoLuong ?? 0,

                    DonGia = ct.DonGia ?? 0,

                    NgayNhap = pn.NgayNhap ?? DateTime.Now,

                    NhanVienNhap = nv.HoTen
                }
            ).ToList();

            return data;
        }
    }
}
