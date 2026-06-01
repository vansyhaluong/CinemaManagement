using Cinema.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using Microsoft.Win32;
using System.IO;
using Path = System.IO.Path;
using BUS;
using Microsoft.EntityFrameworkCore;

namespace Cinema.GUI
{
	/// <summary>
	/// Interaction logic for AddMovieWindow.xaml
	/// </summary>
	public partial class AddMovieWindow : Window
	{
		private string posterPath = "";
		private readonly MovieBUS bus = new MovieBUS();
		private readonly RapPhim2Context db = new RapPhim2Context();
		private Phim? editingMovie;
		private bool isEdit;

		public AddMovieWindow(Phim? movie = null)
		{
			InitializeComponent();
			LoadComboData();
			if (movie != null)
			{
				isEdit = true;
				editingMovie = movie;
				loadData(movie);
			}
		}
		public void btnHuy_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			if (!ValidateMovieInput(out int thoiLuong, out DateOnly ngayKhoiChieu, out string quocGia, out string trangThai, out string theLoai))
			{
				return;
			}

			if (isEdit)
			{
				if (editingMovie == null)
				{
					MessageBox.Show("Không tìm thấy phim để cập nhật!");
					return;
				}

				editingMovie.TieuDe = txtTitle.Text.Trim();
				editingMovie.MoTa = txtMoTa.Text.Trim();
				editingMovie.QuocGia = quocGia;
				editingMovie.ThoiLuong = thoiLuong;
				editingMovie.NgayKhoiChieu = ngayKhoiChieu;
				editingMovie.TrangThai = trangThai;
				editingMovie.AnhBia = posterPath;
				txtImageEdit.Visibility = Visibility.Collapsed;
                editingMovie.MaTheLoais.Clear();
				editingMovie.MaTheLoais.Add(new TheLoai { Ten = theLoai });

				bool result = bus.updateMovie(editingMovie);
				if (result)
				{
					MessageBox.Show("Cập nhật phim thành công!");
					DialogResult = true;
					Close();
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
					MoTa = txtMoTa.Text.Trim(),
					ThoiLuong = thoiLuong,
					NgayKhoiChieu = ngayKhoiChieu,
					QuocGia = quocGia,
					TieuDe = txtTitle.Text.Trim(),
					TrangThai = trangThai
				};

				phim.MaTheLoais.Add(new TheLoai { Ten = theLoai });

				if (bus.addMovie(phim))
				{
					MessageBox.Show("Thêm phim thành công!");
					DialogResult = true;
					Close();
				}
				else
				{
					MessageBox.Show("Thêm phim thất bại!");
				}
			}
		}

		private bool ValidateMovieInput(out int thoiLuong, out DateOnly ngayKhoiChieu, out string quocGia, out string trangThai, out string theLoai)
		{
			thoiLuong = 0;
			ngayKhoiChieu = default;
			quocGia = string.Empty;
			trangThai = string.Empty;
			theLoai = string.Empty;

			if (string.IsNullOrWhiteSpace(txtTitle.Text))
			{
				MessageBox.Show("Vui lòng nhập tiêu đề phim!");
				txtTitle.Focus();
				return false;
			}

			if (!int.TryParse(txtThoiLuong.Text?.Trim(), out thoiLuong) || thoiLuong <= 0)
			{
				MessageBox.Show("Thời lượng phim phải là số nguyên dương!");
				txtThoiLuong.Focus();
				return false;
			}

			if (dpNgayKhoiChieu.SelectedDate == null)
			{
				MessageBox.Show("Vui lòng chọn ngày khởi chiếu!");
				dpNgayKhoiChieu.Focus();
				return false;
			}

			ngayKhoiChieu = DateOnly.FromDateTime(dpNgayKhoiChieu.SelectedDate.Value);
			quocGia = GetSelectedComboBoxValue(cbQuocGia);
			trangThai = GetSelectedComboBoxValue(cbTrangThai);
			theLoai = GetSelectedComboBoxValue(cbTheLoai);

			if (string.IsNullOrWhiteSpace(quocGia))
			{
				MessageBox.Show("Vui lòng chọn quốc gia!");
				cbQuocGia.Focus();
				return false;
			}

			if (string.IsNullOrWhiteSpace(trangThai))
			{
				MessageBox.Show("Vui lòng chọn trạng thái phim!");
				cbTrangThai.Focus();
				return false;
			}

			if (string.IsNullOrWhiteSpace(theLoai))
			{
				MessageBox.Show("Vui lòng chọn thể loại phim!");
				cbTheLoai.Focus();
				return false;
			}

			return true;
		}

		private static string GetSelectedComboBoxValue(ComboBox comboBox)
		{
			if (comboBox.SelectedItem is ComboBoxItem item)
			{
				return item.Content?.ToString()?.Trim() ?? string.Empty;
			}

			if (comboBox.SelectedItem is string textItem)
			{
				return textItem.Trim();
			}

			return comboBox.Text?.Trim() ?? string.Empty;
		}

		private static void SetSelectedComboBoxValue(ComboBox comboBox, string? value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				comboBox.SelectedIndex = -1;
				return;
			}

			foreach (var item in comboBox.Items)
			{
				if (item is ComboBoxItem comboBoxItem &&
					string.Equals(comboBoxItem.Content?.ToString(), value, StringComparison.CurrentCultureIgnoreCase))
				{
					comboBox.SelectedItem = comboBoxItem;
					return;
				}

				if (item is string textItem &&
					string.Equals(textItem, value, StringComparison.CurrentCultureIgnoreCase))
				{
					comboBox.SelectedItem = item;
					return;
				}
			}

			comboBox.Text = value;
		}

		private void LoadComboData()
		{
			cbQuocGia.ItemsSource = db.Phims
				.AsNoTracking()
				.Where(x => !string.IsNullOrWhiteSpace(x.QuocGia))
				.Select(x => x.QuocGia!.Trim())
				.Distinct()
				.OrderBy(x => x)
				.ToList();

			cbTrangThai.ItemsSource = db.Phims
				.AsNoTracking()
				.Where(x => !string.IsNullOrWhiteSpace(x.TrangThai))
				.Select(x => x.TrangThai!.Trim())
				.Distinct()
				.OrderBy(x => x)
				.ToList();

			cbTheLoai.ItemsSource = db.TheLoais
				.AsNoTracking()
				.Where(x => !string.IsNullOrWhiteSpace(x.Ten))
				.Select(x => x.Ten!.Trim())
				.OrderBy(x => x)
				.ToList();
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

				string folder = Path.Combine(
					AppDomain.CurrentDomain.BaseDirectory,
					"Images");

				if (!Directory.Exists(folder))
					Directory.CreateDirectory(folder);

				string destFile = Path.Combine(folder, fileName);

				File.Copy(sourceFile, destFile, true);

				posterPath = "Images/" + fileName;
				imgPoster.Source = new BitmapImage(new Uri(destFile));
				txtNoImage.Visibility = Visibility.Collapsed;
			}
		}

		public void loadData(Phim movie)
		{
			Title = "Cập nhật phim";
			btnSave.Content = "Lưu thay đổi";
			txtTitle.Text = movie.TieuDe;
			txtMoTa.Text = movie.MoTa;
			txtThoiLuong.Text = movie.ThoiLuong?.ToString() ?? string.Empty;

			if (movie.NgayKhoiChieu.HasValue)
			{
				dpNgayKhoiChieu.SelectedDate = movie.NgayKhoiChieu.Value.ToDateTime(TimeOnly.MinValue);
			}

			SetSelectedComboBoxValue(cbQuocGia, movie.QuocGia);
			SetSelectedComboBoxValue(cbTrangThai, movie.TrangThai);
			SetSelectedComboBoxValue(cbTheLoai, movie.MaTheLoais.FirstOrDefault()?.Ten);

			posterPath = movie.AnhBia ?? string.Empty;
			if (!string.IsNullOrWhiteSpace(posterPath))
			{
				Uri imageUri;

				if (Uri.TryCreate(posterPath, UriKind.Absolute, out Uri? absoluteUri))
				{
					imageUri = absoluteUri;
				}
				else
				{
					string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, posterPath);
					imageUri = new Uri(fullPath, UriKind.Absolute);
				}

				imgPoster.Source = new BitmapImage(imageUri);
				txtNoImage.Visibility = Visibility.Collapsed;
			}
		}
	}
}
