using BUS;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cinema.GUI
{
    /// <summary>
    /// Interaction logic for QuanLyGhe.xaml
    /// </summary>
    public partial class QuanLyPhong : UserControl
    {
        private PhongChieuBUS bus = new PhongChieuBUS();
        private GheBUS gheBus = new GheBUS();
        private RapBUS rapBus=new RapBUS();
        
        public QuanLyPhong()
        {
            InitializeComponent();
            dgPhong.ItemsSource = bus.getPhongChieu();
            loadComboRap();
        }
        public void btnThem_Click(object sender, RoutedEventArgs e)
        {
            ThemPhong themPhong = new ThemPhong();
            themPhong.ShowDialog();
            dgPhong.ItemsSource = bus.getPhongChieu();
        }
        public void btnSua_Click(object sender, RoutedEventArgs e)
        {
            var border = sender as FrameworkElement;
            var phong = border?.DataContext as PhongChieu;
            if (phong == null)
            {
                MessageBox.Show("Không tìm thấy phòng chiếu để sửa!");
                return;
            }
            var item = bus.getPhongChieuById(phong.MaPhong);
            ThemPhong edit = new ThemPhong(item);
            if (edit.ShowDialog() == true)
            {
                dgPhong.ItemsSource = bus.getPhongChieu();
            }
        }
        public void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            Button? btn = sender as Button;
            var phong = btn?.Tag as PhongChieu;
            if (phong == null)
            {
                MessageBox.Show("Không tìm thấy phòng chiếu để xóa!");
                return;
            }
            var msg = new CustomMessageBox(
                "Xác nhận xóa",
                $"Bạn có chắc muốn xóa phòng chiếu '{phong.TenPhong}'?");
            msg.ShowDialog();
            if (msg.Result)
            {
                if (bus.removePhongChieu(phong.MaPhong))
                {
                    MessageBox.Show("Xóa phòng chiếu thành công!");
                    dgPhong.ItemsSource = bus.getPhongChieu();
                }
                else
                {
                    MessageBox.Show("Xóa phòng chiếu thất bại!");
                }
            }
        }
        private void dgPhong_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgPhong.SelectedItem is PhongChieu phong)
            {
                txtTenPhong.Text = "🎬"+phong.TenPhong;
                //var phong2 = bus.getPhongChieuById(phong.MaPhong);
                txtTenRap.Text = "Rạp: "+phong.MaRapNavigation?.TenRap;
                loadGhe();
                loadThongTinGhe(phong.MaPhong);
            }
            
        }
        public void cbRap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int maRap = (int)(cbRap.SelectedValue ?? 0);
            if (maRap == 0)
            {
                dgPhong.ItemsSource = bus.getPhongChieu();
            }
            else
            {
                dgPhong.ItemsSource = bus.getPhongChieuByRap(maRap);
            }
        }
        private void loadGhe()
        {
            var dsGhe = gheBus.getGheByPhongId((dgPhong.SelectedItem as PhongChieu)?.MaPhong ?? 0);
            spSeat.Children.Clear();
            var dsHang=dsGhe.GroupBy(g => g.HangGhe).OrderBy(g => g.Key);
            foreach (var hang in dsHang)
            {
                StackPanel spHang = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };
                TextBlock txtHang = new TextBlock { Text = $"Hàng {hang.Key}:", FontWeight = FontWeights.Bold,Foreground=Brushes.White, Margin = new Thickness(0, 0, 10, 0) };
                spHang.Children.Add(txtHang);
                foreach (var ghe in hang)
                {
                    Button btnGhe = new Button
                    {
                        Content = ghe.SoGhe,
                        Width = 40,
                        Height = 40,
                        Margin = new Thickness(5),
                        Background = Brushes.LightGray,
                        Foreground = Brushes.White,

                    };
                    if (ghe.MaLoaiGhe == 1)
                    {
                        btnGhe.Background = Brushes.LightSkyBlue;
                    }
                    else if (ghe.MaLoaiGhe == 2)
                    {
                        btnGhe.Background = Brushes.Red;
                    }
                    else if (ghe.MaLoaiGhe == 3)
                    {
                        btnGhe.Background = Brushes.Purple;
                    }
                    else if (ghe.TrangThai=="Bảo trì")
                    {
                        btnGhe.Background = Brushes.Red;
                    }
                    else if (ghe.TrangThai=="Đã đặt")
                    {
                        btnGhe.Background = Brushes.LightGray;

                    }
                    spHang.Children.Add(btnGhe);
                }
                spSeat.Children.Add(spHang);
            }

        }
        public void loadThongTinGhe(int ma)
        {
            txtGheBaoTri.Text=gheBus.soGheBaoTri(ma).ToString();
            txtTongGhe.Text = gheBus.tongGhe(ma).ToString();
            txtGheHoatDong.Text = gheBus.soGheDaDat(ma).ToString();
            txtGheTrong.Text = gheBus.soGheTrong(ma).ToString();
        }
        public void loadComboRap()
        {
            var dsRap = rapBus.getAllRap();
            dsRap.Insert(0, new Rap { MaRap = 0, TenRap = "🏢 Tất cả rạp" });
            cbRap.ItemsSource = dsRap;
            cbRap.DisplayMemberPath = "TenRap";
            cbRap.SelectedValuePath = "MaRap";
            cbRap.SelectedIndex = 0;
        }

    }
}
