using Cinema.Models;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class PhongChieuBUS
    {
        DAL.PhongChieuDAL dal = new DAL.PhongChieuDAL();
        
        public List<PhongChieu> getPhongChieu(int? maRap = null)
        {
            return dal.getPhongChieu(maRap);
        }
        public PhongChieu? getPhongChieuById(int id, int? maRap = null)
        {
            return dal.getPhongChieuById(id, maRap);
        }
        public bool addPhongChieu(PhongChieu phong)
        {
            return dal.addPhongChieu(phong);
        }
        public bool removePhongChieu(int id)
        {
            return dal.removePhongChieu(id);
        }
        public bool updatePhongChieu(PhongChieu phong)
        {
            return dal.updatePhongChieu(phong);
        }
        public List<PhongChieu> getPhongChieuByRap(int maRap)
        {
            return dal.getPhongChieuByRap(maRap);
        }
    }
}
