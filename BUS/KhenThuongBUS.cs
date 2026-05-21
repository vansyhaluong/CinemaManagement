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
    public class KhenThuongBUS
    {
        private KhenThuongDAL dal = new KhenThuongDAL();

        public List<KhenThuongDTO> GetAll()
        {
            return dal.GetAllKhenThuong();
        }

        public bool Add(KhenThuong kt)
        {
            return dal.Add(kt);
        }

        public bool Update(KhenThuong kt)
        {
            return dal.Update(kt);
        }

        public bool Delete(int maKhenThuong)
        {
            return dal.Delete(maKhenThuong);
        }
    }
}
