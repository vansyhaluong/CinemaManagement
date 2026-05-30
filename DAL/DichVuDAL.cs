using Cinema.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class DichVuDAL
    {
        private readonly RapPhim2Context db = new RapPhim2Context();

        public List<SanPham> getDichVu()
        {
            return db.SanPhams
                .Include(x => x.Kho)
                .ToList();
        }
    }
}
