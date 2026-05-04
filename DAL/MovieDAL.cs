using Cinema.Models;
using DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DAL
{
	public class MovieDAL
	{
		public List<Phim> getMovies()
		{
			var db = new RapPhimContext();
			return db.Phims.ToList();
		}
		public Phim? getMovieById(int id)
		{
			var db = new RapPhimContext();
			return db.Phims.FirstOrDefault(p => p.MaPhim == id);
		}

	}
}
