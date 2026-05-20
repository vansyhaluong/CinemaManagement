using Cinema.Models;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class DichVuBUS
    {
        DichVuDAL dal=new DichVuDAL();
        public List<SanPham> getDichVu()
        {
            return dal.getDichVu();
        }
    }
}
