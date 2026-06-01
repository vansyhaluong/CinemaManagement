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
    public class KyLuatBUS
    {
        private KyLuatDAL dal = new KyLuatDAL();

        public List<KyLuatDTO> GetAll()
        {
            return dal.GetAll();
        }

        public List<KyLuatDTO> GetByDateRange(DateOnly tuNgay, DateOnly denNgay)
        {
            return dal.GetByDateRange(tuNgay, denNgay);
        }

        public bool Add(KyLuat kl)
        {
            return dal.Add(kl);
        }

        public bool Update(KyLuat kl)
        {
            return dal.Update(kl);
        }

        public bool Delete(int maKyLuat)
        {
            return dal.Delete(maKyLuat);
        }
    }
}
