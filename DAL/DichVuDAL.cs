using Cinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public  class DichVuDAL
    {
        RapPhim2Context db = new RapPhim2Context();
        public List<SanPham> getDichVu()
        {
            return db.SanPhams.ToList();
        }
    }
}
