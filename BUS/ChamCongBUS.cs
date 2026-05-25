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
    public class ChamCongBUS
    {
        private readonly ChamCongDAL dal = new();
        public List<ChamCongDTO> GetDanhSachChamCong(DateTime ngay, int maRap = 0, int maCa = 0)
        {
            return dal.GetDanhSachChamCong(ngay, maRap, maCa);
        }
        public bool CheckIn(ChamCongDTO dto)
        {
            if (dto == null)
                throw new Exception("Dữ liệu chấm công không hợp lệ.");

            var existed = dal.GetChamCongTrongNgay(dto.MaNhanVien, dto.MaCa, dto.Ngay);

            if (existed != null)
                throw new Exception("Nhân viên này đã check-in rồi.");

            DateTime now = DateTime.Now;

            TimeOnly gioBatDau = TimeOnly.Parse(dto.GioBatDau);

            string trangThai = now.TimeOfDay > gioBatDau.ToTimeSpan().Add(TimeSpan.FromMinutes(5))
                ? "Trễ"
                : "Đúng giờ";

            var chamCong = new ChamCong
            {
                MaNhanVien = dto.MaNhanVien,
                MaCa = dto.MaCa,
                Ngay = DateOnly.FromDateTime(dto.Ngay),
                GioVao = now,
                GioRa = null,
                TrangThai = trangThai
            };

            return dal.CheckIn(chamCong);
        }

        public bool CheckOut(ChamCongDTO dto)
        {
            if (dto == null || dto.MaChamCong == null)
                throw new Exception("Nhân viên chưa check-in.");

            if (dto.GioRa != null)
                throw new Exception("Nhân viên này đã check-out rồi.");

            return dal.CheckOut(dto.MaChamCong.Value);
        }
    }
}
