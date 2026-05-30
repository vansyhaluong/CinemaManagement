using DAL;
using DAL.Models;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class PhanCaBUS
    {
        private readonly PhanCaDAL dal = new PhanCaDAL();
        public List<PhanCaDTO> GetAll(DateTime ngay, int maRap = 0, int maCa = 0)
        {
            return dal.GetAll(ngay, maRap, maCa);
        }
        public bool Add(int maNhanVien, int maCa, DateTime ngay)
        {
            if (maNhanVien <= 0 || maCa <= 0)
                throw new Exception("Vui lòng chọn nhân viên và ca làm.");

            if (dal.Exists(maNhanVien, ngay))
                throw new Exception("Nhân viên này đã được phân ca trong ngày này.");

            var pc = new PhanCa
            {
                MaNhanVien = maNhanVien,
                MaCa = maCa,
                Ngay = DateOnly.FromDateTime(ngay)
            };

            return dal.Add(pc);
        }
        public bool Update(int maPhanCa, int maNhanVien, int maCa, DateTime ngay)
        {
            if (maPhanCa <= 0)
                throw new Exception("Vui lòng chọn phân ca cần sửa.");

            if (maNhanVien <= 0 || maCa <= 0)
                throw new Exception("Vui lòng chọn nhân viên và ca làm.");

            if (dal.Exists(maNhanVien, ngay, maPhanCa))
                throw new Exception("Nhân viên này đã có phân ca trong ngày này.");

            var pc = new PhanCa
            {
                MaPhanCa = maPhanCa,
                MaNhanVien = maNhanVien,
                MaCa = maCa,
                Ngay = DateOnly.FromDateTime(ngay)
            };

            return dal.Update(pc);
        }

        public bool Delete(int maPhanCa)
        {
            if (maPhanCa <= 0)
                throw new Exception("Vui lòng chọn phân ca cần xóa.");

            return dal.Delete(maPhanCa);
        }
    }
}

