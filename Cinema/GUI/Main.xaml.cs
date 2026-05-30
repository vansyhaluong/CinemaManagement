using BUS;
using Cinema.GUI.Converters;
using Cinema.Models;
using DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Xml.Linq;

namespace Cinema.GUI
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : UserControl
    {
		private const string SearchPlaceholder = "Tìm kiếm phim...";
		MovieBUS bus = new MovieBUS();
		public ObservableCollection<Phim> Phim { get; set; } 
		private Phim selectedMovie;
		private List<Phim> allMovies = new();
		private Dictionary<int, string> movieGenres = new();

        public Main()
        {
            InitializeComponent();
			DataContext = this;
			Phim = new ObservableCollection<Phim>();
			icPhim.ItemsSource = Phim;
			LoadMovieData();
		}
        public void loadPhim()
		{
			LoadMovieData();
		}

		private void LoadMovieData()
		{
			allMovies = bus.getMovies();
			movieGenres = allMovies.ToDictionary(
				movie => movie.MaPhim,
				movie => bus.getTheLoaiByPhim(movie.MaPhim));

			LoadFilterOptions();
			ApplyFilters();
		}

		private void LoadFilterOptions()
		{
			var selectedGenre = cbTheLoai.SelectedItem as string;
			var selectedCountry = cbQuocGia.SelectedItem as string;

			var genres = movieGenres.Values
				.Where(value => !string.IsNullOrWhiteSpace(value))
				.SelectMany(value => value.Split(new[] { " • " }, StringSplitOptions.RemoveEmptyEntries))
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

			var countries = allMovies
				.Select(movie => movie.QuocGia?.Trim())
				.Where(value => !string.IsNullOrWhiteSpace(value))
				.Distinct()
				.OrderBy(value => value)
				.ToList();

			cbQuocGia.Items.Clear();
			cbQuocGia.Items.Add("Tất cả quốc gia");
			foreach (var country in countries)
			{
				cbQuocGia.Items.Add(country);
			}

			cbTheLoai.SelectedItem = cbTheLoai.Items.Cast<object>().FirstOrDefault(item => Equals(item, selectedGenre)) ?? cbTheLoai.Items[0];
			cbQuocGia.SelectedItem = cbQuocGia.Items.Cast<object>().FirstOrDefault(item => Equals(item, selectedCountry)) ?? cbQuocGia.Items[0];
		}

		private void ApplyFilters()
		{
			var keyword = txtTimKiem.Text == SearchPlaceholder ? string.Empty : txtTimKiem.Text.Trim();
			var selectedGenre = cbTheLoai.SelectedItem as string;
			var selectedCountry = cbQuocGia.SelectedItem as string;

			var filtered = allMovies.Where(movie =>
			{
				var matchesKeyword = string.IsNullOrWhiteSpace(keyword)
					|| (movie.TieuDe?.Contains(keyword, StringComparison.CurrentCultureIgnoreCase) ?? false)
					|| (movie.QuocGia?.Contains(keyword, StringComparison.CurrentCultureIgnoreCase) ?? false);

				var genreText = movieGenres.TryGetValue(movie.MaPhim, out var genres) ? genres : string.Empty;
				var matchesGenre = string.IsNullOrWhiteSpace(selectedGenre)
					|| selectedGenre == "Tất cả thể loại"
					|| genreText.Split(new[] { " • " }, StringSplitOptions.RemoveEmptyEntries)
						.Any(value => value.Trim().Equals(selectedGenre, StringComparison.CurrentCultureIgnoreCase));

				var matchesCountry = string.IsNullOrWhiteSpace(selectedCountry)
					|| selectedCountry == "Tất cả quốc gia"
					|| string.Equals(movie.QuocGia?.Trim(), selectedCountry, StringComparison.CurrentCultureIgnoreCase);

				return matchesKeyword && matchesGenre && matchesCountry;
			}).ToList();

			Phim.Clear();
			foreach (var movie in filtered)
			{
				Phim.Add(movie);
			}
		}

        private void Poster_Click(object sender, MouseButtonEventArgs e)
		{

			var border = sender as FrameworkElement;
			var movie = border?.DataContext as Phim;
			if(movie == null)
			{
				MessageBox.Show("Movie Null");
				return;
			}
			var fullMovie =bus.getMovieById(movie.MaPhim);
			// show popup
			PopupContainer.Visibility = Visibility.Visible;

			// FIX ẢNH
			var converter = new ImageConverter();
			imgPoster.Source = converter.Convert(fullMovie.AnhBia, null, null, null) as ImageSource;
			txtTitle.Text = fullMovie.TieuDe;
			
			txtCountry.Text = fullMovie.QuocGia;
			txtDuration.Text = fullMovie.ThoiLuong + " min";
			txtStatus.Text = fullMovie.TrangThai;
            selectedMovie = fullMovie;
			txtDescription.Text = fullMovie.MoTa;

        }
		private void ClosePopup_Click(object sender, RoutedEventArgs e)
		{
			PopupContainer.Visibility = Visibility.Collapsed;
			
		}
		public void btnDatVe_Click(object sender, RoutedEventArgs e)
		{
            MainWindow main = (MainWindow)Window.GetWindow(this);

            main.MainContent.Content = new BanVe(selectedMovie);
        }

		private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

		private void btnThemPhim_Click(object sender, RoutedEventArgs e)
		{
			AddMovieWindow addMovieWindow = new AddMovieWindow();
			if (addMovieWindow.ShowDialog() == true)
			{
				LoadMovieData();
			}
		}

    }
}
