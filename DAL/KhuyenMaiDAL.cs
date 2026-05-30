using Cinema.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class KhuyenMaiDAL
    {
        private readonly RapPhim2Context db = new RapPhim2Context();

        public List<KhuyenMai> GetKhuyenMais()
        {
            return db.KhuyenMais
                .AsNoTracking()
                .OrderByDescending(x => x.NgayBatDau)
                .ThenByDescending(x => x.MaKhuyenMai)
                .ToList();
        }

        public KhuyenMai? GetKhuyenMaiById(int maKhuyenMai)
        {
            return db.KhuyenMais.FirstOrDefault(x => x.MaKhuyenMai == maKhuyenMai);
        }

        public void ThemKhuyenMai(KhuyenMai km)
        {
            ValidateKhuyenMai(km, null);

            db.KhuyenMais.Add(km);
            db.SaveChanges();
        }

        public void CapNhatKhuyenMai(KhuyenMai km)
        {
            var current = db.KhuyenMais.FirstOrDefault(x => x.MaKhuyenMai == km.MaKhuyenMai);
            if (current == null)
            {
                throw new Exception("Không tìm thấy khuyến mãi để cập nhật.");
            }

            ValidateKhuyenMai(km, km.MaKhuyenMai);

            current.TenKhuyenMai = km.TenKhuyenMai;
            current.MaCode = km.MaCode;
            current.MoTa = km.MoTa;
            current.LoaiGiam = km.LoaiGiam;
            current.GiaTriGiam = km.GiaTriGiam;
            current.DonToiThieu = km.DonToiThieu;
            current.NgayBatDau = km.NgayBatDau;
            current.NgayKetThuc = km.NgayKetThuc;
            current.SoLuong = km.SoLuong;
            current.TrangThai = km.TrangThai;

            db.SaveChanges();
        }

        public void CapNhatTrangThai(int maKhuyenMai, string trangThai)
        {
            var current = db.KhuyenMais.FirstOrDefault(x => x.MaKhuyenMai == maKhuyenMai);
            if (current == null)
            {
                throw new Exception("Không tìm thấy khuyến mãi để cập nhật trạng thái.");
            }

            if (string.IsNullOrWhiteSpace(trangThai))
            {
                throw new Exception("Trạng thái khuyến mãi không hợp lệ.");
            }

            current.TrangThai = trangThai.Trim();
            db.SaveChanges();
        }

        private void ValidateKhuyenMai(KhuyenMai km, int? ignoreId)
        {
            var tenKhuyenMai = km.TenKhuyenMai?.Trim() ?? string.Empty;
            var maCode = km.MaCode?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(tenKhuyenMai))
            {
                throw new Exception("Tên khuyến mãi không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(maCode))
            {
                throw new Exception("Mã khuyến mãi không được để trống.");
            }

            var trungCode = db.KhuyenMais.Any(x =>
                x.MaCode != null
                && x.MaCode.Trim().ToLower() == maCode.ToLower()
                && (!ignoreId.HasValue || x.MaKhuyenMai != ignoreId.Value));

            if (trungCode)
            {
                throw new Exception("Mã khuyến mãi đã tồn tại.");
            }
        }
    }
}
