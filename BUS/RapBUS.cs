using Cinema.Models;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class RapBUS
    {
        RapDAL dal = new RapDAL();
        public List<Rap> getAllRap()
        {
            return dal.getRap();
        }
        public Rap? GetRapById(int id)
        {
            return dal.getRapById(id);
        }
        public List<Rap> getRapByPhim(int maPhim)
        {
            return dal.getRapByPhim(maPhim);
        }
    }
}
