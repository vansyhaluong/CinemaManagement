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

namespace Cinema.GUI
{
    /// <summary>
    /// Interaction logic for QuanLyPhim.xaml
    /// </summary>
    public partial class QuanLyPhim : UserControl
    {
        MovieBUS bus = new MovieBUS();
        

        public QuanLyPhim()
        {
            InitializeComponent();
            dgPhim.ItemsSource = bus.getMovies();
        }
        public void btnThemPhim_Click(object sender, RoutedEventArgs e)
        {
            AddMovieWindow add = new AddMovieWindow();
            if (add.ShowDialog() == true)
            {
                dgPhim.ItemsSource = bus.getMovies();
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
                if (bus.removeMovie(movie.MaPhim))
                {
                    MessageBox.Show("Xóa phim thành công!");
                    dgPhim.ItemsSource = bus.getMovies();
                }
                else
                {
                    MessageBox.Show("Xóa phim thất bại!");
                }

            }
        }
        public void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var border = sender as FrameworkElement;
            var movie = border?.DataContext as Phim;
			if (movie == null)
			{
				MessageBox.Show("Không thấy phim để sửa!");
				return;
			}
			var fullMovie = bus.getMovieById(movie.MaPhim);
			
			AddMovieWindow add = new AddMovieWindow(fullMovie);
			if (add.ShowDialog() == true)
			{
				dgPhim.ItemsSource = bus.getMovies();
			}

		}
    }
}
