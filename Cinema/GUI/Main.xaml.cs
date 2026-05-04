using BUS;
using Cinema.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		MovieBUS bus = new MovieBUS();
		public ObservableCollection<Phim> Phim { get; set; } 
		
		public Main()
        {
            InitializeComponent();
			DataContext = this;
			Phim = new ObservableCollection<Phim>(bus.getMovies());
			icPhim.ItemsSource = Phim;
		}
        public void loadPhim()
		{
			var ds = bus.getMovies();
            icPhim.ItemsSource = ds;

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

			imgPoster.Source = new BitmapImage(new Uri(fullMovie.AnhBia, UriKind.RelativeOrAbsolute));
			txtTitle.Text = fullMovie.TieuDe;
			
			txtCountry.Text = fullMovie.QuocGia;
			txtDuration.Text = fullMovie.ThoiLuong + " min";
			txtStatus.Text = fullMovie.TrangThai;
			
		}
		private void ClosePopup_Click(object sender, RoutedEventArgs e)
		{
			PopupContainer.Visibility = Visibility.Collapsed;
		}
	}
}
