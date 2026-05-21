using Cinema.Models;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class CaLamBUS
    {
        private readonly CaLamDAL dal = new();

        public List<CaLam> GetAll()
        {
            return dal.GetAll();
        }
    }
}
