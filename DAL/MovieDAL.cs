using Cinema.Models;
using DTO;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
	public class MovieDAL
	{
		private readonly RapPhim2Context db = new RapPhim2Context();

		public List<Phim> getMovies()
		{
			return db.Phims.ToList();
		}

		public Phim? getMovieById(int id)
		{
			return db.Phims
				.Include(p => p.MaTheLoais)
				.FirstOrDefault(p => p.MaPhim == id);
		}

		public bool addMovie(Phim phim)
		{
			try
			{
				GanTheLoaiTheoTen(phim);
				db.Phims.Add(phim);
				db.SaveChanges();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool removeMovie(int id, out string message)
		{
			try
			{
				var phim = db.Phims
					.Include(p => p.SuatChieus)
					.Include(p => p.MaTheLoais)
					.FirstOrDefault(p => p.MaPhim == id);

				if (phim == null)
				{
					message = "Không tìm thấy phim để xóa.";
					return false;
				}

				if (phim.SuatChieus.Any())
				{
					message = "Phim này đã có suất chiếu, không thể xóa.";
					return false;
				}

				phim.MaTheLoais.Clear();
				db.SaveChanges();

				db.Phims.Remove(phim);
				db.SaveChanges();

				message = "Xóa phim thành công.";
				return true;
			}
			catch (Exception ex)
			{
				message = $"Không thể xóa phim: {ex.Message}";
				return false;
			}
		}

		public bool updateMovie(Phim phim)
		{
			try
			{
				var p = db.Phims
					.Include(x => x.MaTheLoais)
					.FirstOrDefault(x => x.MaPhim == phim.MaPhim);

				if (p == null)
				{
					return false;
				}

				p.TieuDe = phim.TieuDe;
				p.QuocGia = phim.QuocGia;
				p.MoTa = phim.MoTa;
				p.NgayKhoiChieu = phim.NgayKhoiChieu;
				p.ThoiLuong = phim.ThoiLuong;
				p.AnhBia = phim.AnhBia;
				p.TrangThai = phim.TrangThai;

				p.MaTheLoais.Clear();
				var selectedGenreNames = phim.MaTheLoais
					.Select(genre => genre.Ten)
					.Where(name => !string.IsNullOrWhiteSpace(name))
					.Select(name => name!.Trim())
					.Distinct(StringComparer.CurrentCultureIgnoreCase)
					.ToList();

				if (selectedGenreNames.Any())
				{
					var genres = db.TheLoais
						.Where(genre => selectedGenreNames.Contains(genre.Ten!))
						.ToList();

					foreach (var genre in genres)
					{
						p.MaTheLoais.Add(genre);
					}
				}

				db.SaveChanges();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public List<string> GetTenTheLoaiByPhim(int maPhim)
		{
			return db.Phims
				.Include(p => p.MaTheLoais)
				.Where(p => p.MaPhim == maPhim)
				.SelectMany(p => p.MaTheLoais)
				.Select(t => t.Ten ?? "")
				.ToList();
		}

		private void GanTheLoaiTheoTen(Phim phim)
		{
			if (!phim.MaTheLoais.Any())
			{
				return;
			}

			var selectedGenreNames = phim.MaTheLoais
				.Select(genre => genre.Ten)
				.Where(name => !string.IsNullOrWhiteSpace(name))
				.Select(name => name!.Trim())
				.Distinct(StringComparer.CurrentCultureIgnoreCase)
				.ToList();

			phim.MaTheLoais.Clear();

			if (!selectedGenreNames.Any())
			{
				return;
			}

			var genres = db.TheLoais
				.Where(genre => selectedGenreNames.Contains(genre.Ten!))
				.ToList();

			foreach (var genre in genres)
			{
				phim.MaTheLoais.Add(genre);
			}
		}
	}
}
