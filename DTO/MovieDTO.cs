using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
	public class MovieDTO
	{
		public int maPhim { get; set; }
		public string? tenPhim { get; set; }
		public string? moTa { get; set; }
		public int thoiLuong { get; set; }
		public DateTime ngayKhoiChieu { get; set; }
		public string? image { get; set; }
		public string? trangThai { get; set; }
	}
}
