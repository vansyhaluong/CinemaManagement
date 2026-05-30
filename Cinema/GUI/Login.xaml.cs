using BUS;
using DTO;
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

namespace Cinema.GUI
{
	/// <summary>
	/// Interaction logic for Login.xaml
	/// </summary>
	public partial class Login : Window
	{
        private TaiKhoanBUS tkBUS = new TaiKhoanBUS();
        public Login()
		{
			InitializeComponent();
		}
		private void btnExit_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show(
	 "Are you sure you want to exit?",
	 "Exit Confirmation",
	 MessageBoxButton.YesNo,
	 MessageBoxImage.Question);
		}
		private void btnLogin_Click(object sender, RoutedEventArgs e)
		{
            string tenDangNhap = txtUsername.Text.Trim();
            string matKhau = txtPassword.Password.Trim();

            var tk = tkBUS.DangNhap(tenDangNhap, matKhau);

            if (tk == null)
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!");
                return;
            }

            TaiKhoanDTO.MaTaiKhoan = tk.MaTaiKhoan;
            TaiKhoanDTO.VaiTro = tk.VaiTro;

            if (tk.NhanVien != null)
            {
                TaiKhoanDTO.MaNhanVien = tk.NhanVien.MaNhanVien;
                TaiKhoanDTO.HoTen = tk.NhanVien.HoTen;
                TaiKhoanDTO.MaRap = tk.NhanVien.MaRap ?? 0;
                TaiKhoanDTO.TenRap = tk.NhanVien.MaRapNavigation?.TenRap ?? "";
            }

            Session.Set(
                TaiKhoanDTO.MaTaiKhoan,
                TaiKhoanDTO.MaNhanVien,
                TaiKhoanDTO.HoTen,
                TaiKhoanDTO.VaiTro,
                TaiKhoanDTO.MaRap,
                TaiKhoanDTO.TenRap);

            MainWindow main = new MainWindow();
            main.Show();

            this.Close();
        }
	}
}
