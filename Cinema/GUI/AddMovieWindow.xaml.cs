using Cinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Microsoft.Win32;
using System.IO;
using Path = System.IO.Path;
using BUS;

namespace Cinema.GUI
{
	/// <summary>
	/// Interaction logic for AddMovieWindow.xaml
	/// </summary>
	public partial class AddMovieWindow : Window
	{
		string posterPath = "";
		MovieBUS bus = new MovieBUS();
		private Phim? editingMovie;
		private bool isEdit;
		public AddMovieWindow(Phim? movie=null)
		{
			InitializeComponent();
			if (movie != null)
			{
				isEdit = true;
				editingMovie = movie;
				loadData(movie);
			}

		}
		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			if (isEdit)
			{
				editingMovie.TieuDe = txtTitle.Text;
				editingMovie.QuocGia = cbQuocGia.SelectedItem.ToString();
				editingMovie.ThoiLuong = int.Parse(txtThoiLuong.Text);
				editingMovie.TrangThai = cbTrangThai.SelectedItem.ToString();
				editingMovie.AnhBia = posterPath;

				bool result = bus.updateMovie(editingMovie);
				if (result)
				{
					MessageBox.Show("Cập nhật phim thành công!");
					this.DialogResult = true;
					this.Close();
				}
				else
				{
					MessageBox.Show("Cập nhật phim thất bại!");
				}
			}
			else
			{
				var phim = new Phim
				{
					AnhBia = posterPath,
					MoTa = txtMoTa.Text,
					ThoiLuong = int.Parse(txtThoiLuong.Text),
					NgayKhoiChieu = DateOnly.Parse(dpNgayKhoiChieu.Text),
					QuocGia = cbQuocGia.Text,
					TieuDe = txtTitle.Text,
					TrangThai = cbTrangThai.SelectedItem.ToString()

				};
				if (bus.addMovie(phim))
				{
					MessageBox.Show("Thêm phim thành công!");
					this.DialogResult = true;
					this.Close();
				}
				else
				{
					MessageBox.Show("Thêm phim thất bại!");
				}
			}
			

		}
		private void btnUpLoad_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Title = "Chọn ảnh poster";
			dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

			if (dlg.ShowDialog() == true)
			{
				string sourceFile = dlg.FileName;

				string fileName = Path.GetFileName(sourceFile);

				// Thư mục chạy app
				string folder = Path.Combine(
					AppDomain.CurrentDomain.BaseDirectory,
					"Images");

				if (!Directory.Exists(folder))
					Directory.CreateDirectory(folder);

				string destFile = Path.Combine(folder, fileName);

				File.Copy(sourceFile, destFile, true);

				posterPath = "Images/" + fileName;

				imgPoster.Source = new BitmapImage(
					new Uri(destFile));

				txtNoImage.Visibility = Visibility.Collapsed;
			}
		}
		public void loadData(Phim movie)
		{
			btnSave.Content = "Lưu thay đổi";
			txtTitle.Text = movie.TieuDe;
			txtMoTa.Text = movie.MoTa;
			txtThoiLuong.Text = movie.ThoiLuong.ToString();
			dpNgayKhoiChieu.Text = movie.NgayKhoiChieu.ToString();
			cbQuocGia.Text = movie.QuocGia;
			cbTrangThai.Text = movie.TrangThai;
			posterPath = movie.AnhBia;
			imgPoster.Source = new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, posterPath), UriKind.Absolute));
		}
	}
}
