
using Cinema.Models;
using DAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
	public  class MovieBUS
	{
		MovieDAL dal = new MovieDAL();
		public List<Phim> getMovies()
		{
			return dal.getMovies();
		}
		public Phim? getMovieById(int id)
		{
			return dal.getMovieById(id);
		}
		public bool addMovie(Phim phim)
		{
			return dal.addMovie(phim);
		}
		public bool updateMovie(Phim phim)
		{
			return dal.updateMovie(phim);
		}
		public bool removeMovie(int id)
		{
			return dal.removeMovie(id);
		}
		public string getTheLoaiByPhim(int ma)
		{
            List<string> ds = dal.GetTenTheLoaiByPhim(ma);

            return string.Join(" • ", ds);
        }
	}
}
