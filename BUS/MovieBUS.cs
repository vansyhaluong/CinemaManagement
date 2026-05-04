
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
	}
}
