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
		RapPhim2Context db = new RapPhim2Context();
		public List<Phim> getMovies()
		{
			
			return db.Phims.ToList();
		}
		public Phim? getMovieById(int id)
		{
			
			return db.Phims.FirstOrDefault(p => p.MaPhim == id);
		}
		public bool addMovie(Phim phim)
		{
			try
			{
				db.Phims.Add(phim);
				db.SaveChanges();
				return true;
			}
			catch 
			{
				return false;
			}
		}
		public bool removeMovie(int id)
		{
			try
			{
				var phim = db.Phims.FirstOrDefault(p => p.MaPhim == id);
				if (phim != null)
				{
					db.Phims.Remove(phim);
					db.SaveChanges();
					return true;
				}
				return false;
			}
			catch
			{
				return false;
			}
		}
		public bool updateMovie(Phim phim)
		{
			try
			{
				var p = db.Phims.FirstOrDefault(p => p.MaPhim == phim.MaPhim);
				if (p != null)
				{
					p.TieuDe = phim.TieuDe;
					p.QuocGia = phim.QuocGia;
					p.MoTa = phim.MoTa;
					p.NgayKhoiChieu = phim.NgayKhoiChieu;
					p.ThoiLuong = phim.ThoiLuong;
					p.AnhBia = phim.AnhBia;
					p.MaTheLoais = phim.MaTheLoais;
					p.TrangThai = phim.TrangThai;
					db.SaveChanges();
					
				}
				return true;
			}
			catch
			{
				return false;
			}
			
		}

	}
}
