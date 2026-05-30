using Cinema.Controls;
using Cinema.Models;
using DTO;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cinema.GUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private List<InventoryItem> _allItems = new();
		private ObservableCollection<InventoryItem> _filtered = new();
		public MainWindow()
		{
			InitializeComponent();
			ApplyPermission();
            MainContent.Content = new Main();
			//LoadData();
			//dgvItems.ItemsSource = _filtered;

			//txtSearch.TextChanged += (s, e) => ApplyFilter();
			//cmbCategory.SelectionChanged += (s, e) => ApplyFilter();
			//cmbStatus.SelectionChanged += (s, e) => ApplyFilter();
		}
		private void ThemeToggle_Click(object sender, RoutedEventArgs e)
			=> ((App)Application.Current).ThemeToggle();
		private void LoadData()
		{
			_allItems = new List<InventoryItem>
			{
				new(1001,"Dell Monitor 24\"",       "Electronics",12, 249.99m,"TechDist Inc",new DateTime(2024,3,11),"Active"),
				new(1002,"Office Chair (Black)",    "Furniture",   5, 189.00m,"FurnCo Ltd",  new DateTime(2024,3,9), "Active"),
				new(1003,"A4 Paper (500 sheets)",   "Stationery",142,   6.50m,"OfficeDepot", new DateTime(2024,3,10),"Active"),
				new(1004,"USB-C Hub 7-port",        "Electronics", 3,  39.99m,"TechDist Inc",new DateTime(2024,3,8), "Low Stock"),
				new(1005,"Standing Desk",           "Furniture",   0, 549.00m,"FurnCo Ltd",  new DateTime(2024,3,7), "Out of Stock"),
				new(1006,"Stapler (Heavy Duty)",    "Stationery", 28,  12.00m,"OfficeDepot", new DateTime(2024,3,6), "Active"),
				new(1007,"Logitech MX Keys",        "Electronics", 7,  99.99m,"TechDist Inc",new DateTime(2024,3,5), "Active"),
				new(1008,"Filing Cabinet (3-draw)", "Furniture",   2, 210.00m,"FurnCo Ltd",  new DateTime(2024,3,4), "Low Stock"),
				new(1009,"Whiteboard Markers",      "Stationery", 55,   3.20m,"OfficeDepot", new DateTime(2024,3,3), "Active"),
				new(1010,"Power Strip (6-outlet)",  "Electronics", 9,  22.50m,"TechDist Inc",new DateTime(2024,3,2), "Active"),
			};
			//ApplyFilter();
		}
		//private void ApplyFilter()
		//{
		//	string search = txtSearch.Text == "Search items..." ? "" : txtSearch.Text;
		//	string cat = (cmbCategory.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
		//	string status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

		//	var q = _allItems.AsEnumerable();
		//	if (!string.IsNullOrWhiteSpace(search))
		//		q = q.Where(i => i.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
		//	if (!string.IsNullOrWhiteSpace(cat) && cat != "All Categories")
		//		q = q.Where(i => i.Category == cat);
		//	if (!string.IsNullOrWhiteSpace(status) && status != "All Status")
		//		q = q.Where(i => i.Status == status);

		//	_filtered.Clear();
		//	foreach (var item in q) _filtered.Add(item);
		//	lblPageInfo.Text = $"Showing {_filtered.Count} of {_allItems.Count} items";
		//}
		//public void Movie_Click(object sender, MouseButtonEventArgs e)
		//{

		//	MainPage.Visibility = Visibility.Collapsed;
		//	MainContent.Visibility = Visibility.Visible;
		//	MainContent.Content = new Main();
		//}
		public void btnQuanLyPhim_Click(object sender, MouseButtonEventArgs e)
		{
			MainContent.Content = new QuanLyPhim();
		}
		public void btnQuanLyPhong_Click(object sender, MouseButtonEventArgs e)
		{
			MainContent.Content = new QuanLyPhong();
		}
		private void Nav_Click(object sender, RoutedEventArgs e)
		{
			// Tắt tất cả
			nav_Phim.IsActive = false;
			nav_Phong.IsActive = false;
			nav_QLPhim.IsActive = false;
			nav_SuatChieu.IsActive = false;
			nav_HoaDon.IsActive = false;
			nav_Kho.IsActive = false;
			nav_PhanCa.IsActive = false;
			nav_ChamCong.IsActive = false;
			nav_Logout.IsActive = false;
			nav_Luong.IsActive = false;
			nav_KhenThuong.IsActive = false;
			nav_QLNV.IsActive = false;
			nav_KhuyenMai.IsActive = false;
            // Bật item đang click
            NavItem? item = sender as NavItem;
			item.IsActive = true;
		}
		public void btnKhuyenMai_Click(object sender, MouseButtonEventArgs e)
		{
			MainContent.Content = new KhuyenMai();
        }
        public void btnListMovie_Click(object sender, MouseButtonEventArgs e)
		{
			MainContent.Content = new Main();
		}
		public void btnLuong_Click(object sender, MouseButtonEventArgs e)
		{
			MainContent.Content = new QuanLyBangLuong();
        }
        public void btnQLNhanVien_Click(object sender, MouseButtonEventArgs e)
		{
			MainContent.Content = new Student();
		}
		public void btnLogout_Click(object sender, MouseButtonEventArgs e)
		{
			Session.Clear();
			TaiKhoanDTO.Clear();
			Login login = new Login();
			login.Show();
			this.Close();
        }

        public void btnSuatChieu_Click(object sender, MouseButtonEventArgs e)
		{
			MainContent.Content = new QuanLySuatChieu();
        }
		public void btnKhenThuong_Click(object sender, MouseButtonEventArgs e)
		{
            MainContent.Content = new KhenThuong();
        }

        public void btnQuanLyHoaDon_Click(object sender, MouseButtonEventArgs e)
		{
			MainContent.Content = new QuanLyHoaDon();
		}
		public void btnDoanhThu_Click(object sender, MouseButtonEventArgs e)
		{
			MainContent.Content = new BaoCaoDoanhThuNgay();
		}
		public void btnKho_Click(object sender, MouseButtonEventArgs e)
		{
			MainContent.Content = new QuanLyKho();
		}
		public void btnPhanCa_Click(object sender, MouseButtonEventArgs e)
		{
            MainContent.Content = new PhanCa();
        }
		public void btnChamCong_Click(object sender, MouseButtonEventArgs e)
		{
            MainContent.Content = new ChamCong();
        }
        private void ApplyPermission()
        {
            string vaiTro = Session.VaiTro;

            if (vaiTro == "Admin")
            {
                return; // thấy hết
            }

			else
			{
				nav_DoanhThu.Visibility = Visibility.Collapsed;
				nav_QLNV.Visibility = Visibility.Collapsed;
				nav_PhanCa.Visibility = Visibility.Collapsed;
				nav_KhuyenMai.Visibility = Visibility.Collapsed;
				nav_Luong.Visibility = Visibility.Collapsed;
				nav_Phim.Visibility = Visibility.Collapsed;
				nav_KhenThuong.Visibility = Visibility.Collapsed;
				nav_Phong.Visibility = Visibility.Collapsed;
			}

            
        }

    }
}
