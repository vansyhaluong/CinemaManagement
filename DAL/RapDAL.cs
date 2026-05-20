using Cinema.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class RapDAL
    {
        RapPhim2Context db = new RapPhim2Context();
        public List<Rap> getRap()
        {
            return db.Raps.ToList();
        }
        public Rap? getRapById(int id)
        {
            return db.Raps.FirstOrDefault(r => r.MaRap == id);
        }
        public List<Rap> getRapByPhim(int maPhim)
        {
            return db.Raps
            .Where(r => r.PhongChieus
            .Any(p => p.SuatChieus
             .Any(s => s.MaPhim == maPhim)))
            .ToList();
        }
    }
}
