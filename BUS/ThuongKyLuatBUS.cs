using Cinema.Models;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class ThuongKyLuatBUS
    {
        private ChamCongDAL chamCongDAL = new ChamCongDAL();
        private KhenThuongDAL khenThuongDAL = new KhenThuongDAL();
        private KyLuatDAL kyLuatDAL = new KyLuatDAL();
        public void TinhThuongKyLuat(DateOnly tuNgay, DateOnly denNgay)
        {
            var dsChamCong =
                chamCongDAL.GetChamCongTheoTuan(tuNgay, denNgay);

            var nhomTheoNhanVien = dsChamCong
                .Where(x => x.MaNhanVien != null)
                .GroupBy(x => x.MaNhanVien)
                .ToList();

            foreach (var nhom in nhomTheoNhanVien)
            {
                int maNhanVien = nhom.Key.Value;

                int soLanTre = nhom.Count(x =>
                    x.TrangThai != null &&
                    x.TrangThai.ToLower().Contains("trễ"));

                int soLanVang = nhom.Count(x =>
                    x.TrangThai != null &&
                    (
                        x.TrangThai.ToLower().Contains("vắng") ||
                        x.TrangThai.ToLower().Contains("off") ||
                        x.TrangThai.ToLower().Contains("nghỉ")
                    ));

                if (soLanTre > 0)
                {
                    KyLuat kl = new KyLuat
                    {
                        MaNhanVien = maNhanVien,
                        Ngay = denNgay,
                        LyDo = $"Đi trễ {soLanTre} lần",
                        SoTienPhat = soLanTre * 100000
                    };

                    kyLuatDAL.Add(kl);
                }

                if (soLanVang > 0)
                {
                    KyLuat kl = new KyLuat
                    {
                        MaNhanVien = maNhanVien,
                        Ngay = denNgay,
                        LyDo = $"Vắng/off {soLanVang} ca",
                        SoTienPhat = soLanVang * 500000
                    };

                    kyLuatDAL.Add(kl);
                }

                if (soLanTre == 0 &&
                    soLanVang == 0 &&
                    nhom.Any())
                {
                    KhenThuong kt = new KhenThuong
                    {
                        MaNhanVien = maNhanVien,
                        Ngay = denNgay,
                        LyDo = "Thưởng chuyên cần tuần",
                        SoTienThuong = 500000
                    };

                    khenThuongDAL.Add(kt);
                }
            }
        }
    }
}
