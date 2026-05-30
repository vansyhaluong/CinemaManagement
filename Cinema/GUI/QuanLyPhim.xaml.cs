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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BUS;
using Cinema.Models;
using System.Collections.Generic;
using System.Linq;

namespace Cinema.GUI
{
    /// <summary>
    /// Interaction logic for QuanLyPhim.xaml
    /// </summary>
    public partial class QuanLyPhim : UserControl
    {
        private const string SearchPlaceholder = "Tìm kiếm phim...";
        private readonly MovieBUS bus = new MovieBUS();
        private List<Phim> allMovies = new();
        private Dictionary<int, string> movieGenres = new();

        public QuanLyPhim()
        {
            InitializeComponent();
            LoadMovieData();
        }

        private void LoadMovieData()
        {
            allMovies = bus.getMovies();
            movieGenres = allMovies.ToDictionary(
                movie => movie.MaPhim,
                movie => bus.getTheLoaiByPhim(movie.MaPhim));

            LoadTheLoaiComboBox();
            ApplyFilters();
        }

        private void LoadTheLoaiComboBox()
        {
            var genres = movieGenres.Values
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .SelectMany(value => value.Split(new[] { " • " }, System.StringSplitOptions.RemoveEmptyEntries))
                .Select(value => value.Trim())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct()
                .OrderBy(value => value)
                .ToList();

            cbTheLoai.Items.Clear();
            cbTheLoai.Items.Add("Tất cả thể loại");
            foreach (var genre in genres)
            {
                cbTheLoai.Items.Add(genre);
            }

            cbTheLoai.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            var keyword = txtTimKiem.Text == SearchPlaceholder ? string.Empty : txtTimKiem.Text.Trim();
            var selectedGenre = cbTheLoai.SelectedItem as string;

            var filtered = allMovies.Where(movie =>
            {
                var matchesKeyword = string.IsNullOrWhiteSpace(keyword)
                    || (movie.TieuDe?.Contains(keyword, System.StringComparison.CurrentCultureIgnoreCase) ?? false)
                    || (movie.QuocGia?.Contains(keyword, System.StringComparison.CurrentCultureIgnoreCase) ?? false);

                var genreText = movieGenres.TryGetValue(movie.MaPhim, out var genres) ? genres : string.Empty;
                var matchesGenre = string.IsNullOrWhiteSpace(selectedGenre)
                    || selectedGenre == "Tất cả thể loại"
                    || genreText.Split(new[] { " • " }, System.StringSplitOptions.RemoveEmptyEntries)
                        .Any(value => value.Trim().Equals(selectedGenre, System.StringComparison.CurrentCultureIgnoreCase));

                return matchesKeyword && matchesGenre;
            }).ToList();

            dgPhim.ItemsSource = filtered;
        }

        public void btnThemPhim_Click(object sender, RoutedEventArgs e)
        {
            AddMovieWindow add = new AddMovieWindow();
            if (add.ShowDialog() == true)
            {
                LoadMovieData();
            }
        }
        public void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Button? btn = sender as Button;
            var movie = btn?.Tag as Phim;
            if (movie == null)
            {
                MessageBox.Show("Không tìm thấy phim để xóa!");
                return;
            }
            MessageBoxResult result = MessageBox.Show($"Bạn có chắc muốn xóa phim '{movie.TieuDe}'?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                if (bus.removeMovie(movie.MaPhim, out string message))
                {
                    MessageBox.Show(message);
                    LoadMovieData();
                }
                else
                {
                    MessageBox.Show(message);
                }

            }
        }
        public void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var movie = btn?.Tag as Phim;
			if (movie == null)
			{
				MessageBox.Show("Không thấy phim để sửa!");
				return;
			}
			var fullMovie = bus.getMovieById(movie.MaPhim);
			
			AddMovieWindow add = new AddMovieWindow(fullMovie);
			if (add.ShowDialog() == true)
			{
				LoadMovieData();
			}

		}

        private void cbTheLoai_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            ApplyFilters();
        }

        private void txtTimKiem_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            ApplyFilters();
        }

        private void txtTimKiem_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtTimKiem.Text != SearchPlaceholder)
            {
                return;
            }

            txtTimKiem.Text = string.Empty;
            txtTimKiem.Foreground = Brushes.White;
        }

        private void txtTimKiem_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtTimKiem.Text))
            {
                return;
            }

            txtTimKiem.Text = SearchPlaceholder;
            txtTimKiem.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));
        }
    }
}
