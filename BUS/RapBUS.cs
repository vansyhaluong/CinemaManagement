using Cinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class RapBUS
    {
        DAL.RapDAL dal = new DAL.RapDAL();
        public List<Rap> getRap()
        {
            return dal.getRap();
        }
        public Rap? GetRapById(int id)
        {
            return dal.getRapById(id);
        }
    }
}
